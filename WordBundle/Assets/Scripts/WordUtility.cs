using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WordUtility
{
    private const int NUM_LETTERS = 26;
    private const string LOG_TAG = nameof(WordUtility);
    private const string WORD_LIST_FILE = "wordlist";
    private const string LETTER_WEIGHTS_FILE = "letterweights";

    // List of words to check against during the game
    private string[] wordList;

    // Weights to use when generating a random character
    // Constructed to be decimal ranges between 0-1 based on numerical
    //      weights in a file
    // Ex. Given 3 letters with weights 1, 3, and 1
    //  Letter    Weight    Calc Percentage    Stored Value
    //    A         1             20%  (1/5)       0.2  (0% to 20%)
    //    B         3             60%  (3/5)       0.8  (21% to 80%)
    //    C         1             20%  (1/5)       1.0  (80% to 100%)
    private double[] letterWeights;

    /**
     * Loads the word list from a file, and calculates letter weights from file
     */
    public WordUtility()
    {
        // Load the wordlist, 
        TextAsset wordListFile = Resources.Load(WORD_LIST_FILE) as TextAsset;

        if (wordListFile != null)
        {
            // If its not null, split it, and save it to the array
            wordList = wordListFile.text.Split('\n');
            Debug.Log(LOG_TAG + ": Successfully loaded word list");
        }
        else
        {
            // If it is null, create an empty array
            wordList = new string[0];
            Debug.LogWarning(LOG_TAG + ": Word list not found");
        }


        // Construct the letter weights from file
        letterWeights = new double[NUM_LETTERS];
        double sum = 0;


        // First load the numerical weights from the file
        bool failed = false;
        try
        {
            TextAsset letterWeightsFile = Resources.Load(LETTER_WEIGHTS_FILE) as TextAsset;

            using (StringReader reader = new StringReader(letterWeightsFile.text))
            {
                // There should be 26 numeric lines in the file, otherwise there's an error
                for (int i = 0; i < NUM_LETTERS; i++)
                {
                    double weight = double.Parse(reader.ReadLine());
                    sum += weight;
                    letterWeights[i] = weight;
                }
            }

            Debug.Log(LOG_TAG + ": Successfully read letter weights");
        }
        catch (NullReferenceException)
        {
            Debug.LogWarning(LOG_TAG + ": Letter Weight file not found. Using evenly distributed weights");
            failed = true;
        }
        catch (ArgumentNullException)
        {
            Debug.LogWarning(LOG_TAG + ": Not enough values in file. Using evenly distributed weights");
            failed = true;
        }
        catch (FormatException)
        {
            Debug.LogWarning(LOG_TAG + ": Non-numeric value found in letter weights. Using evenly distributed weights");
            failed = true;
        }

        // Something went wrong trying to read the file, so use defaults instead
        if (failed)
        {
            // Fill letterWeights with 1's
            for (int i = 0; i < NUM_LETTERS; i++)
            {
                letterWeights[i] = 1;
            }
            sum = NUM_LETTERS;
        }

        for (int i = 0; i < NUM_LETTERS; i++)
        {
            // calculate the percentages
            letterWeights[i] /= sum;

            // calculate the consecutive ranges
            if (i > 0)
                letterWeights[i] += letterWeights[i - 1];
        }
        
    }

    /**
     * Checks if a word exists in the word list
     * @param word the word to check
     * @return true if the word exists, false otherwise
     */
    public bool exists(string word)
    {
        return Array.BinarySearch(wordList, word, new CaseInsensitiveComparer()) >= 0;
    }

    /**
     * Calculates the score to give a given word
     * @param word the word to calcualte the score from
     * @return the calculated score
     */
    public int calculateScore(string word)
    {
        // Score equals the square of the word length to reward the player for longer words
        return word.Length * word.Length;
    }

    /**
     * Generates a random character using the letter weights
     * @return the random character
     */
    public char generateRandomChar()
    {
        double random = UnityEngine.Random.value;

        for (int i = 0; i < NUM_LETTERS; i++)
        {
            if (random <= letterWeights[i])
                return (char)('A' + i);
        }

        // The last letter weight should be ~1, so if the code got here,
        // then there was likely some unlucky floating point precision problem where
        // random was also ~1, so return the last character 'Z'
        return 'Z';
    }
}
