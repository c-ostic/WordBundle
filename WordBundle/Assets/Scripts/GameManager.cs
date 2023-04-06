using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// The object in the scene that manages the game and connects the UI to the Game State
/// </summary>
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

    private SceneLoader sceneLoader;

    /// <summary>
    /// Called at the start of the scene, before Start() and the first frame update
    /// </summary>
    private void Awake()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();

        // Start a new game if the new game info argument is 1, or if there's no argument
        int gameRequest = sceneLoader.GetArgument(SceneConstants.NEW_GAME_INFO, SceneConstants.NEW_GAME_REQUEST);
        gameState = new GameState(gameRequest == SceneConstants.NEW_GAME_REQUEST);

        current_timer_value = 0;
    }

    /// <summary>
    /// Called before the first frame update
    /// </summary>
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

    /// <summary>
    /// Update is called once per frame
    /// </summary>
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

    /// <summary>
    /// Called when this object is destroyed (such as when the scene changes)
    /// </summary>
    private void OnDestroy()
    {
        gameState.SaveGame();
    }

    /// <summary>
    /// Tells the game state to add a new letter
    /// </summary>
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

    /// <summary>
    /// Updates each letter button to sync them with the current gameState.
    /// Also clears the current word since all buttons are set to a false state.
    /// </summary>
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

        wordEntryText.text = "";
    }

    /// <summary>
    /// Loads the Game Over Screen (a special version of the pause menu)
    /// </summary>
    private void GameOver()
    {
        sceneLoader.AddArgument(SceneConstants.REQUEST_INFO, SceneConstants.GAME_OVER_REQUEST);
        sceneLoader.AddArgument(SceneConstants.SCORE_INFO, gameState.GetScore());
        sceneLoader.LoadNewScene(SceneConstants.PAUSE_MENU_INDEX);
    }

    /// <summary>
    /// Linked to each of the buttons to affect the game state correctly
    /// </summary>
    /// <param name="toggled"> whether the toggle on the button was checked or unchecked </param>
    /// <param name="index"> the index of the button for gameState context </param>
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

    /// <summary>
    /// Submits the current word to the gameState.
    /// If it was successful, update the score and display the word.
    /// Otherwise, display the error message returned from the gameState.
    /// </summary>
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

    /// <summary>
    /// Tells the game state to shuffle the letters, then updates accordingly
    /// </summary>
    public void ShuffleLetters()
    {
        gameState.ShuffleLetters();
        RefreshLetterButtons();
    }

    /// <summary>
    /// Tells the game state to remove the last letter
    /// </summary>
    public void Backspace()
    {
        int removedLetter = gameState.Backspace();

        // If a letter was removed, update the corresponding button
        if (removedLetter >= 0)
        {
            letterButtons[removedLetter].SetToggleState(false);

            wordEntryText.text = gameState.GetCurrentWord();
        }
    }

    /// <summary>
    /// Tells the game state to clear the current word
    /// </summary>
    public void ClearWord()
    {
        gameState.ClearWord();
        RefreshLetterButtons();
    }

    /// <summary>
    /// Loads the Pause Menu
    /// </summary>
    public void PauseGame()
    {
        sceneLoader.AddArgument(SceneConstants.REQUEST_INFO, SceneConstants.PAUSE_REQUEST);
        sceneLoader.AddArgument(SceneConstants.SCORE_INFO, gameState.GetScore());
        sceneLoader.LoadNewScene(SceneConstants.PAUSE_MENU_INDEX);
    }
}
