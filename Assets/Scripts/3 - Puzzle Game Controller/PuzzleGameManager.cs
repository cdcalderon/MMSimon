using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PuzzleGameManager : MonoBehaviour {

	[SerializeField]
	private PuzzleGameSaver puzzleGameSaver;

	[SerializeField]
	private GameFinished gameFinished;

	private List<Button> puzzleButtons = new List<Button>();
	
	private List<Animator> puzzleButtonsAnimators = new List<Animator>();
	[SerializeField]
	private List<Sprite> gamePuzzleSprites = new List<Sprite>();

    [SerializeField]
    private List<AudioClip> gamePuzzleAudioClips = new List<AudioClip>();

    private int level;
	private string selectedPuzzle;

	private Sprite puzzleBackgroundImage;

	private bool firstGuess, secondGuess;
	private int firstGuessIndex, secondGuessIndex;
	private string firstGuessPuzzle, secondGuessPuzzle;

	private int countTryGuess;

	private int countCorrectGuess;
	private int gameGuess;


    private int color;
    int onInList = 0;
    List<int> pattern = new List<int>();
    Random rand = new Random();
    bool playingBack = false;

    public void SetInitialPattern()
    {
        pattern = new List<int>();
        pattern.Add(Random.Range(0, puzzleButtons.Count));
        StartCoroutine(PlayBack());
    }

    public void PickAPuzzle() {

        //		if (!firstGuess) {
        //			firstGuess = true;
        //
        //			firstGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
        //
        //			firstGuessPuzzle = gamePuzzleSprites[firstGuessIndex].name;
        //
        //			StartCoroutine(TurnPuzzleButtonUp(puzzleButtonsAnimators[firstGuessIndex], 
        //			                                  puzzleButtons[firstGuessIndex], 
        //                                              gamePuzzleSprites[firstGuessIndex], 
        //                                              gamePuzzleAudioClips[firstGuessIndex]));
        //
        //		} else if (!secondGuess) {
        //			secondGuess = true;
        //
        //			secondGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
        //
        //			secondGuessPuzzle = gamePuzzleSprites[secondGuessIndex].name;
        //			
        //			StartCoroutine(TurnPuzzleButtonUp(puzzleButtonsAnimators[secondGuessIndex], 
        //			                                  puzzleButtons[secondGuessIndex], 
        //                                              gamePuzzleSprites[secondGuessIndex], 
        //                                              gamePuzzleAudioClips[secondGuessIndex]));
        //
        //			StartCoroutine(CheckIfThePuzzlesMatch(puzzleBackgroundImage));
        //
        //			countTryGuess++;
        //
        //		}

        firstGuessIndex = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);

        StartCoroutine(TurnUpAndDownCard());

        color = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.name);
        if (playingBack)
            return;

        if (pattern[onInList] == color)
        {
            onInList++;
        }
        else
        {
           // MessageBox.Show("You Fail, Final Score: " + pattern.Count.ToString());
            onInList = 0;
            pattern = new List<int>();
            StartCoroutine(PlayBack());
            // new Thread(PlayBack).Start();
        }

        //Reached the end of the pattern successfully
        if (onInList >= pattern.Count)
        {
            pattern.Add(Random.Range(0, puzzleButtons.Count));
            onInList = 0;
            StartCoroutine(PlayBack());
            //  new Thread(PlayBack).Start();
        }

//        ScoreLabel.Text = ("Score: " + pattern.Count.ToString());
//        PatternLabel.Text = ("Item within pattern: " + onInList.ToString());

    }

    IEnumerator TurnUpAndDownCard()
    {
        StartCoroutine(TurnPuzzleButtonUp(puzzleButtonsAnimators[firstGuessIndex],
                                                     puzzleButtons[firstGuessIndex],
                                                     gamePuzzleSprites[firstGuessIndex],
                                                     gamePuzzleAudioClips[firstGuessIndex]));
        yield return new WaitForSeconds(2f);
        StartCoroutine(
        TurnPuzzleButtonBack(puzzleButtonsAnimators[color],
                                    puzzleButtons[color], puzzleBackgroundImage));
    }

	IEnumerator CheckIfThePuzzlesMatch(Sprite puzzleBackgroundImage) {
	
		yield return new WaitForSeconds (1.7f);

		if (firstGuessPuzzle == secondGuessPuzzle) {
		
			puzzleButtonsAnimators [firstGuessIndex].Play ("FadeOut");
			puzzleButtonsAnimators [secondGuessIndex].Play ("FadeOut");

			CheckIfTheGameIsFinished();

		} else {

			StartCoroutine(TurnPuzzleButtonBack(puzzleButtonsAnimators[firstGuessIndex], 
			                                    puzzleButtons[firstGuessIndex], puzzleBackgroundImage));

			StartCoroutine(TurnPuzzleButtonBack(puzzleButtonsAnimators[secondGuessIndex], 
			                                    puzzleButtons[secondGuessIndex], puzzleBackgroundImage));
		
		}

		yield return new WaitForSeconds (.7f);

		firstGuess = secondGuess = false;
		
	}

	void CheckIfTheGameIsFinished() {
		countCorrectGuess++;

		if (countCorrectGuess == gameGuess) {
//			Debug.Log("Game Ends No More Puzzles");
			CheckHowManyGuesses();
		}

	}

	void CheckHowManyGuesses() {
		int howManyGuesses = 0;

		switch(level) {
			
		case 0:
			howManyGuesses = 5;
			break;
			
		case 1:
			howManyGuesses = 10;
			break;
			
		case 2:
			howManyGuesses = 15;
			break;
			
		case 3:
			howManyGuesses = 20;
			break;
			
		case 4:
			howManyGuesses = 25;
			break;
			
		}

		if (countTryGuess < howManyGuesses) {
			gameFinished.ShowGameFinishedPanel (3);

			puzzleGameSaver.Save(level, selectedPuzzle, 3);

		} else if (countTryGuess < (howManyGuesses + 5)) {
			gameFinished.ShowGameFinishedPanel (2);

			puzzleGameSaver.Save(level, selectedPuzzle, 2);

		} else {
			gameFinished.ShowGameFinishedPanel (1);
			puzzleGameSaver.Save(level, selectedPuzzle, 1);
		}

	}

    IEnumerator PlayBack()
    {
        //        anim.Play("TurnBack");
        //        yield return new WaitForSeconds(.4f);
        //        btn.image.sprite = puzzleImage;

        playingBack = true;

        foreach (int color in pattern)
        {
            var sec = pattern.Count*1;
            yield return new WaitForSeconds(sec);

            StartCoroutine(TurnPuzzleButtonUp(puzzleButtonsAnimators[color],
                                                     puzzleButtons[color],
                                                     gamePuzzleSprites[color],
                                                     gamePuzzleAudioClips[color]));
            yield return new WaitForSeconds(2f);
            StartCoroutine(
            TurnPuzzleButtonBack(puzzleButtonsAnimators[color],
                                        puzzleButtons[color], puzzleBackgroundImage));

            //            switch (color)
            //            {
            //                case 0:
            //                    //                    Red.BackColor = Color.Red;
            //                    //                    //  Thread.Sleep(200);
            //                    //                    Red.BackColor = Color.Transparent;
            //                    StartCoroutine(TurnPuzzleButtonUp(puzzleButtonsAnimators[color],
            //                                                      puzzleButtons[color],
            //                                                      gamePuzzleSprites[color],
            //                                                      gamePuzzleAudioClips[color]));
            //                    yield return new WaitForSeconds(2f);
            //                    StartCoroutine(
            //                    TurnPuzzleButtonBack(puzzleButtonsAnimators[color],
            //                                                puzzleButtons[color], puzzleBackgroundImage));
            //                    break;
            //                case 1:
            //                    //                    Blue.BackColor = Color.Blue;
            //                    //                    // Thread.Sleep(200);
            //                    //                    Blue.BackColor = Color.Transparent;
            //                    StartCoroutine(TurnPuzzleButtonUp(puzzleButtonsAnimators[color],
            //                                                      puzzleButtons[color],
            //                                                      gamePuzzleSprites[color],
            //                                                      gamePuzzleAudioClips[color]));
            //                    yield return new WaitForSeconds(2f);
            //                    StartCoroutine(
            //                    TurnPuzzleButtonBack(puzzleButtonsAnimators[firstGuessIndex],
            //                                                puzzleButtons[firstGuessIndex], puzzleBackgroundImage));
            //                    break;
            //                case 2:
            //                    //                    Yellow.BackColor = Color.Yellow;
            //                    //                    //  Thread.Sleep(200);
            //                    //                    Yellow.BackColor = Color.Transparent;
            //                    StartCoroutine(TurnPuzzleButtonUp(puzzleButtonsAnimators[color],
            //                                                      puzzleButtons[color],
            //                                                      gamePuzzleSprites[color],
            //                                                      gamePuzzleAudioClips[color]));
            //                    yield return new WaitForSeconds(2f);
            //                    StartCoroutine(
            //                    TurnPuzzleButtonBack(puzzleButtonsAnimators[firstGuessIndex],
            //                                                puzzleButtons[firstGuessIndex], puzzleBackgroundImage));
            //                    break;
            //                case 3:
            //                    //                    Green.BackColor = Color.Green;
            //                    //                    // Thread.Sleep(200);
            //                    //                    Green.BackColor = Color.Transparent;
            //                    StartCoroutine(TurnPuzzleButtonUp(puzzleButtonsAnimators[color],
            //                                                      puzzleButtons[color],
            //                                                      gamePuzzleSprites[color],
            //                                                      gamePuzzleAudioClips[color]));
            //                    yield return new WaitForSeconds(2f);
            //                    StartCoroutine(
            //                    TurnPuzzleButtonBack(puzzleButtonsAnimators[firstGuessIndex],
            //                                                puzzleButtons[firstGuessIndex], puzzleBackgroundImage));
            //                    break;
            //
            //            }
            //  Thread.Sleep(200);
        }
        playingBack = false;
    }


    public List<Animator> ResetGameplay() {
		firstGuess = secondGuess = false;

		countTryGuess = 0;
		countCorrectGuess = 0;

		gameFinished.HideGameFinishedPanel ();

		return puzzleButtonsAnimators;
	}

	IEnumerator TurnPuzzleButtonUp(Animator anim, Button btn, Sprite puzzleImage, AudioClip audioClip) {
		anim.Play ("TurnUp");
		yield return new WaitForSeconds (.4f);
		btn.image.sprite = puzzleImage;

        AudioSource aSource = GetComponent<AudioSource>();
        aSource.clip = audioClip;
        aSource.Play();
    }

	IEnumerator TurnPuzzleButtonBack(Animator anim, Button btn, Sprite puzzleImage) {
		anim.Play ("TurnBack");
		yield return new WaitForSeconds (.4f);
		btn.image.sprite = puzzleImage;
	}

	void AddListeners() {
		foreach (Button btn in puzzleButtons) {
			btn.onClick.RemoveAllListeners();
			btn.onClick.AddListener(() => PickAPuzzle());
		}
	}
	
	public void SetUpButtonsAndAnimators(List<Button> buttons, List<Animator> animators) {
		this.puzzleButtons = buttons;
		this.puzzleButtonsAnimators = animators;

		gameGuess = puzzleButtons.Count / 2;

		puzzleBackgroundImage = puzzleButtons [0].image.sprite;

		AddListeners ();

	} 
	
	public void SetGamePuzzleSprites(List<Sprite> gamePuzzleSprites,  List<AudioClip> gamePuzzleAudioClips ) {
		this.gamePuzzleSprites = gamePuzzleSprites;
        this.gamePuzzleAudioClips = gamePuzzleAudioClips;
    } 
	
	public void SetLevel(int level) {
		this.level = level;
	}
	
	public void SetSelectedPuzzle(string selectedPuzzle) {
		this.selectedPuzzle = selectedPuzzle;
	}
		
} // PuzzleGameManager


















































