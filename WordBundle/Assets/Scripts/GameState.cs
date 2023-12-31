using System.Text;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The results that can be sent back by a word submission
/// </summary>
public enum WordSubmitResult
{
    SUCCESS,
    ALREADY_GUESSED,
    NO_WORD_FOUND
}

/// <summary>
/// Represents all of the current game's info including the game save file
/// </summary>
public class GameState
{
    private const string LOG_TAG = nameof(GameState);
    private const int LETTER_POOL_SIZE = 16;
    private const int NUM_STARTING_LETTERS = 8;

    public const char NULL_CHARACTER = '\0';
    public const string SAVE_FILE_EXISTS_KEY = "saveFileExists";

    // The name of the file used to save game data
    private string save_file = Application.persistentDataPath + "\\gameStateFile";
    private char[] letters;
    private int score;
    private bool gameOverFlag;
    private HashSet<string> guessedWords;

    // Stores the current letters in terms of positions in the "letters" array
    private List<int> currWord;

    private WordUtility wordUtility;
    private HighscoreManager highscoreManager;

    /// <summary>
    /// Creates a game state, loading from file or creating a new game
    /// </summary>
    /// <param name="newGame"> whether to create a new game </param>
    public GameState(bool newGame)
    {
        wordUtility = new WordUtility();
        highscoreManager = new HighscoreManager();

        // Set defaults for everything
        guessedWords = new HashSet<string>();
        currWord = new List<int>();
        score = 0;
        gameOverFlag = false;
        letters = new char[LETTER_POOL_SIZE];

        // Set the defaults if a new game is requested or if there's an error with the file
        if (newGame || !LoadGame())
        {
            // Fill the letters array with some starting letters
            for (int i = 0; i < NUM_STARTING_LETTERS; i++)
            {
                letters[i] = wordUtility.generateRandomChar();
            }

            // Fill the rest with nulls
            for (int j = NUM_STARTING_LETTERS; j < letters.Length; j++)
            {
                letters[j] = NULL_CHARACTER;
            }
        }
    }

    /// <summary>
    /// Accessor for individual letters
    /// </summary>
    /// <param name="letterNum"> the index of the letter to get </param>
    /// <returns> the letter at that index </returns>
    public char GetLetter(int letterNum)
    {
        return letters[letterNum];
    }

    /// <summary>
    /// Accessor for the score
    /// </summary>
    /// <returns> the score </returns>
    public int GetScore()
    {
        return score;
    }

    /// <summary>
    /// Checks if the game is over
    /// </summary>
    /// <returns> true if the game is over, false if the game is still running </returns>
    public bool IsGameOver()
    {
        return gameOverFlag;
    }

    /// <summary>
    /// Provides functionality for when a letter is pressed.
    /// Adds a letter to the current word
    /// </summary>
    /// <param name="letterNum"> the index of letter to add </param>
    /// <returns> the current word after pressing the letter </returns>
    public string PressLetter(int letterNum)
    {
        // If the letter isn't null add it to the current word
        if (letters[letterNum] != NULL_CHARACTER)
        {
            currWord.Add(letterNum);
        }
        // Otherwise, log a warning and return the unchanged current word
        else
        {
            Debug.LogWarning(LOG_TAG + ": Tried to add a letter with no current value to the word");
        }

        return GetCurrentWord();
    }

    /// <summary>
    /// Provides functionality for a letter is unpressed.
    /// Removes a letter from the current word.
    /// </summary>
    /// <param name="letterNum"> the index of the letter to remove </param>
    /// <returns> the current word after unpressing the letter </returns>
    public string UnPressLetter(int letterNum)
    {
        // Remove the letter index from the current word
        bool wasRemoved = currWord.Remove(letterNum);

        // If the letter wasn't part of the word, log a warning
        if (!wasRemoved)
        {
            Debug.LogWarning(LOG_TAG + ": Tried to remove letter that wasn't a part of the word");
        }

        return GetCurrentWord();
    }

    /// <summary>
    /// Adds a new letter to the letter pool in the first open space.
    /// If there are no open spaces, set the game over flag.
    /// </summary>
    /// <returns> the index of the new letter, or -1 if there was no space </returns>
    public int AddNewLetter()
    {
        int newLetterPosition = -1;

        // Find the first instance of a null character (effectively empty position)
        for (int i = 0; i < letters.Length; i++)
        {
            // If one is found, save the position, generate a new letter, and break out
            if (letters[i] == NULL_CHARACTER)
            {
                newLetterPosition = i;
                letters[i] = wordUtility.generateRandomChar();
                break;
            }
        }

        // If none were found, the there are no open spaces, -1 is returned, and the game is over
        if (newLetterPosition == -1)
        {
            gameOverFlag = true;
            highscoreManager.AddScore(score);
            ClearSave();
        }

        return newLetterPosition;
    }

    /// <summary>
    /// Submits a word to the WordUtility to check if it exists.
    /// Clears the current word.
    /// If it exists, calculate and add to the score.
    /// </summary>
    /// <returns> the result of the submission </returns>
    public WordSubmitResult SubmitWord()
    {
        string word = GetCurrentWord();
        WordSubmitResult result;

        // Check if the word was guessed previously
        if (guessedWords.Contains(word))
        {
            result = WordSubmitResult.ALREADY_GUESSED;
        }
        // Then check if the word exists
        else if (!wordUtility.exists(word))
        {
            result = WordSubmitResult.NO_WORD_FOUND;
        }
        // This means it is a successful guess
        else
        {
            // Add to the score
            score += wordUtility.calculateScore(word);

            // "Remove" the used characters from the letter array
            foreach (int charPos in currWord)
            {
                letters[charPos] = NULL_CHARACTER;
            }

            // Add to the guessed list
            guessedWords.Add(word);

            result = WordSubmitResult.SUCCESS;
        }

        // Regardless of the result, the word should be cleared
        ClearWord();

        return result;
    }

    /// <summary>
    /// Clears the current word
    /// </summary>
    public void ClearWord()
    {
        currWord.Clear();
    }

    /// <summary>
    /// Removes the last letter from the current word
    /// </summary>
    /// <returns> the position of the letter removed (or -1 if none were removed) </returns>
    public int Backspace()
    {
        if (currWord.Count == 0)
            return -1;

        int removed = currWord[currWord.Count - 1];
        currWord.RemoveAt(currWord.Count - 1);
        return removed;
    }

    /// <summary>
    /// Clears the current word and shuffles the letters randomly
    /// </summary>
    public void ShuffleLetters()
    {
        ClearWord();

        // Fisher-Yates shuffle algorithm
        for (int i = 0;i < letters.Length - 2;i++)
        {
            int swap = Random.Range(i, letters.Length - 1);

            char temp = letters[i];
            letters[i] = letters[swap];
            letters[swap] = temp;
        }
    }

    /// <summary>
    /// Returns the string version of the current word
    /// </summary>
    /// <returns> the current word </returns>
    public string GetCurrentWord()
    {
        StringBuilder stringBuilder = new StringBuilder();

        foreach (int charPos in currWord)
        {
            stringBuilder.Append(letters[charPos]);
        }

        return stringBuilder.ToString();
    }

    /// <summary>
    /// Saves the current game state to a file
    /// </summary>
    /// <returns> true if the save succeeded, false otherwise </returns>
    public bool SaveGame()
    {
        // Don't save a game over
        if (gameOverFlag)
        {
            return false;
        }

        // Clear any previous save before saving the new game
        ClearSave();

        StringBuilder builder = new StringBuilder();

        // Save score on the first line
        builder.Append(score);
        builder.Append("\n");

        // Save letters on the next line
        foreach (char c in letters)
        {
            builder.Append(c);
        }
        builder.Append("\n");

        // Save guessed words on subsequent lines
        foreach (string word in guessedWords)
        {
            builder.Append(word);
            builder.Append("\n");
        }

        string saveState = builder.ToString();

        try
        {
            using (FileStream fileConnection = new FileStream(save_file, FileMode.OpenOrCreate))
            using (StreamWriter sWriter = new StreamWriter(fileConnection))
            {
                sWriter.WriteLine(saveState);
            }

            PlayerPrefs.SetInt(SAVE_FILE_EXISTS_KEY, 1);
            PlayerPrefs.Save();

            Debug.Log(LOG_TAG + ": Successfully saved game");

            return true;
        }
        catch (IOException)
        {
            Debug.LogWarning(LOG_TAG + ": Unable to save game to file");
            return false;
        }
    }

    /// <summary>
    /// Clears the save file
    /// </summary>
    /// <returns> true if it succeeded, false otherwise </returns>
    private bool ClearSave()
    {
        if (File.Exists(save_file))
        {
            File.Delete(save_file);

            if (PlayerPrefs.HasKey(SAVE_FILE_EXISTS_KEY))
            {
                PlayerPrefs.SetInt(SAVE_FILE_EXISTS_KEY, 0);
                PlayerPrefs.Save();
            }

            Debug.Log(LOG_TAG + ": Successfully deleted file");

            return true;
        }
        else
        {
            Debug.LogWarning(LOG_TAG + ": Could not delete file");
            return false;
        }
    }

    /// <summary>
    /// Loads a game state from the save file.
    /// If the file exists but there is missing information, fill in with defaults.
    /// </summary>
    /// <returns> true if it succeeded, false otherwise </returns>
    private bool LoadGame()
    {
        // If there is no save file, don't try to load
        if (!PlayerPrefs.HasKey(SAVE_FILE_EXISTS_KEY) || PlayerPrefs.GetInt(SAVE_FILE_EXISTS_KEY) == 0)
        {
            return false;
        }

        try
        {
            using (FileStream fileConnection = new FileStream(save_file, FileMode.Open))
            using (StreamReader reader = new StreamReader(fileConnection))
            {

                // First line should be the score
                if (!int.TryParse(reader.ReadLine(), out score))
                {
                    score = 0;
                }

                // Next line is the letters
                string lettersString = reader.ReadLine();

                for (int i = 0; i < lettersString.Length; i++)
                {
                    letters[i] = lettersString[i];
                }

                // Just in case the string is shorter than 16 characters
                for (int i = lettersString.Length; i < letters.Length; i++)
                {
                    letters[i] = NULL_CHARACTER;
                }

                // Every subsequent line is a previously guessed word
                string word = reader.ReadLine();
                while (word != null)
                {
                    guessedWords.Add(word);
                    word = reader.ReadLine();
                }
            }

            return true;
        }
        catch (IOException)
        {
            Debug.LogWarning(LOG_TAG + ": Unable to load game file");
            return false;
        }
    }
}
