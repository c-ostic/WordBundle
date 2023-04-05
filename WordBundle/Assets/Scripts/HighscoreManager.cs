using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class HighscoreManager
{
    private string scores_file = Application.persistentDataPath + "\\scoresFile";
    private const string LOG_TAG = nameof(HighscoreManager);
    private const int NUM_SCORES = 10;
    private int[] scores;

    /**
     * Creates the high score manager, and loads the top ten scores from file
     * If no file exists, the scores will be filled with zeros
     */
    public HighscoreManager()
    {
        // load the scores from file
        scores = new int[NUM_SCORES];

        // If the loading failed, fill the array with 0's
        if (!LoadScores())
        {
            Array.Fill(scores, 0);
        }

        // Sort the scores (should already be sorted, but just in case)
        SortScores();
    }

    /**
     * Accessor for scores
     * @return a clone of the scores array (to prevent changes of the scores outside this class)
     */
    public IEnumerable<int> GetScores()
    {
        // return a version that doesn't allow for changing the array
        // Clone() only makes a shallow copy, but this is okay since int is primitive
        return (int[]) scores.Clone();
    }

    /**
     * Attempts to add a score to the list of high scores
     * Only adds the score if it is greater than the lowest score in the top ten
     * @param newScore the score to add
     * @return true if the score was added, false if it wasn't
     */
    public bool AddScore(int newScore)
    {
        // at this point, the scores should be sorted,
        // so just check against the last (smallest) score
        if (newScore > scores[NUM_SCORES - 1])
        {
            // replace the least score, re-sort, and save
            scores[NUM_SCORES - 1] = newScore;
            SortScores();
            SaveScores();
            return true;
        }
        else
        {
            // no change to the array
            return false;
        }
    }

    /**
     * Saves the scores to the scores file
     * @return true if the save succeeded, false if it failed
     */
    private bool SaveScores()
    {
        // clear previous scores to save any new scores
        ClearScores();

        StringBuilder builder = new StringBuilder();

        foreach (int score in scores)
        {
            builder.Append(score);
            builder.Append("\n");
        }

        String scoreString = builder.ToString().Trim();

        try
        {
            using (FileStream fileConnection = new FileStream(scores_file, FileMode.OpenOrCreate))
            using (StreamWriter sWriter = new StreamWriter(fileConnection))
            {
                sWriter.WriteLine(scoreString);
            }
            Debug.Log(LOG_TAG + ": Successfully saved scores");

            return true;
        }
        catch (IOException)
        {
            Debug.LogWarning(LOG_TAG + ": Unable to save scores to file");
            return false;
        }
    }

    /**
     * Clears the scores file
     * @return true if the clear succeeded, false if it failed
     */
    private bool ClearScores()
    {
        if (File.Exists(scores_file))
        {
            File.Delete(scores_file);
            Debug.Log(LOG_TAG + ": Successfully deleted file");
            return true;
        }
        else
        {
            Debug.LogWarning(LOG_TAG + ": Could not delete file");
            return false;
        }
    }

    /**
     * Loads scores from the file
     * @return true if it succeeded, false if it failed
     */
    private bool LoadScores()
    {
        try
        {
            using (FileStream fileConnection = new FileStream(scores_file, FileMode.Open))
            using (StreamReader reader = new StreamReader(fileConnection))
            {
                // load the first NUM_SCORES scores from the file
                for (int i = 0; i < NUM_SCORES; i++)
                {
                    if (!int.TryParse(reader.ReadLine(), out scores[i]))
                    {
                        scores[i] = 0;
                    }
                }
            }

            return true;
        }
        catch (IOException)
        {
            Debug.Log(LOG_TAG + ": Unable to load scores");
            return false;
        }
    }

    /**
     * Sorts the scores in descending order
     */
    private void SortScores()
    {
        Array.Sort(scores, (i, j) => j.CompareTo(i));
    }
}
