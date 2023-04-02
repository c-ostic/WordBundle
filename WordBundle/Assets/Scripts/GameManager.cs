using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private const string LOG_TAG = nameof(GameManager);
    private const string NO_WORD_FOUND_ERROR = "No Word Found";
    private const string ALREADY_GUESSED_ERROR = "Already Guessed";

    [SerializeField]
    private float timer_length = 3.0f; // timer length in seconds
    [SerializeField]
    private List<ToggleButton> letterButtons;
    [SerializeField]
    private Image timer;
    [SerializeField]
    private TextMeshProUGUI wordEntryText;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI previousWord;
    [SerializeField]
    private TextMeshProUGUI previousWordScore;

    // The current state of the game
    private GameState gameState;

    // The current time on the timer
    private float current_timer_value;

    /**
     * Called at the beginning of the scene starting up
     */
    private void Awake()
    {
        // TODO: Make this available to change
        gameState = new GameState(true);

        current_timer_value = 0;
    }

    /**
     * Called at the beginning of the scene starting up, but after Awake()
     */
    private void Start()
    {
        // Add a listener to each of the buttons 
        for (int i = 0;i < letterButtons.Count;i++)
        {
            int index = i;
            letterButtons[i].Toggled += (bool toggled) => PressLetterButton(toggled, index);
        }

        // Load the score to match any save data
        scoreText.text = gameState.GetScore().ToString();

        // Refresh the letters to match any save data
        RefreshLetterButtons();
    }

    /**
     * Called every frame
     */
    private void Update()
    {
        // Update the timer
        current_timer_value += Time.deltaTime;
        timer.fillAmount = current_timer_value / timer_length;

        if (current_timer_value >= timer_length)
        {
            current_timer_value = 0;
            AddLetter();
        }

        // Check for the end of the game
        if (gameState.IsGameOver())
        {
            GameOver();
        }
    }

    /**
     * Tells the game state to add a new letter
     */
    private void AddLetter()
    {
        int letterAffected = gameState.AddNewLetter();

        if (letterAffected != -1)
        {
            char letter = gameState.GetLetter(letterAffected);
            letterButtons[letterAffected].SetToggleState(false);
            letterButtons[letterAffected].gameObject.SetActive(true);
            letterButtons[letterAffected].GetComponentInChildren<TextMeshProUGUI>().text = letter.ToString();
        }
    }

    /**
     * Updates each letter button to sync them with the current gameState
     */
    private void RefreshLetterButtons()
    {
        for (int i = 0;i < letterButtons.Count;i++)
        {
            char letter = gameState.GetLetter(i);
            ToggleButton currentButton = letterButtons[i];

            // There is no letter
            if (letter == GameState.NULL_CHARACTER)
            {
                currentButton.SetToggleState(false);
                currentButton.gameObject.SetActive(false);
            }
            else
            {
                // There is a letter
                currentButton.SetToggleState(false);
                currentButton.gameObject.SetActive(true);
                currentButton.GetComponentInChildren<TextMeshProUGUI>().text = letter.ToString();
            }
        }
    }

    /**
     * Loads the Game Over Screen
     */
    private void GameOver()
    {
        // TODO: connect to scene changer
    }

    /**
     * OnCheckedChanged function linked to each of the buttons to affect the game state correctly
     * @param isChecked whether the toggle on the button was checked or unchecked
     * @param index the index of the button for gameState context
     */
    private void PressLetterButton(bool toggled, int index)
    {
        if (toggled)
        {
            wordEntryText.text = gameState.PressLetter(index);
        }
        else
        {
            wordEntryText.text = gameState.UnPressLetter(index);
        }
    }

    /**
     * Submits the current word to the gameState
     * If it was successful, update the score and display the word
     * Otherwise, display the error message returned from the gameState
     */
    public void SubmitWord()
    {
        string currentWord = wordEntryText.text;
        int currentScore = gameState.GetScore();

        WordSubmitResult result = gameState.SubmitWord();

        // Empty the word entry field and refresh the buttons
        wordEntryText.text = "";
        RefreshLetterButtons();

        if (result == WordSubmitResult.SUCCESS)
        {
            // Set the previous word and score
            previousWord.text = currentWord;
            previousWordScore.text = (gameState.GetScore() - currentScore).ToString();

            // Update the score to the new total
            scoreText.text = gameState.GetScore().ToString();
        }
        else if (result == WordSubmitResult.NO_WORD_FOUND)
        {
            // Use the previous word text to display the error
            previousWord.text = NO_WORD_FOUND_ERROR;
            previousWordScore.text = "";
        }
        else // result == WordSubmitResult.ALREADY_GUESSED
        {
            // Use the previous word text to display the error
            previousWord.text = ALREADY_GUESSED_ERROR;
            previousWordScore.text = "";
        }
    }

    /**
     * Tells the game state to shuffle the letters, then updates accordingly
     */
    public void ShuffleLetters()
    {
        // TODO: add functionality for this in game state
    }

    /**
     * Tells the game state to remove the last letter
     */
    public void Backspace()
    {
        // TODO: add functionality for this in game state
    }

    /**
     * Tells the game state to clear the current word
     */
    public void ClearWord()
    {
        // TODO: add functionality for this in game state
    }

    /**
     * Loads the Pause Screen
     */
    public void PauseGame()
    {
        // TODO: connect to scene changer
    }
}
