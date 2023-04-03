using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private const string LOG_TAG = nameof(SceneLoader);

    // Stack to keep track of the history of loaded scenes
    private Stack<int> sceneHistory;

    // Argument list
    private Dictionary<string, int> arguments;

    private void Awake()
    {
        // This will persist throughout the application
        DontDestroyOnLoad(this);

        sceneHistory = new Stack<int>();
        sceneHistory.Push(SceneManager.GetActiveScene().buildIndex);

        arguments = new Dictionary<string, int>();
    }

    private void Update()
    {
        // If escape is pressed (back button for android), move back one scene in the history
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            sceneHistory.Pop();
            SceneManager.LoadScene(sceneHistory.Peek());
        }
    }

    /**
     * Creates a new instance of sceneName and adds it to the history
     * If the sceneName isn't found, no scene is loaded
     * @param sceneName the scene to load
     */
    public void LoadNewScene(int buildIndex)
    {
        sceneHistory.Push(buildIndex);
        SceneManager.LoadScene(buildIndex);
    }

    /**
     * Backtracks through the history to find sceneName and loads it, removing everything in front of it in the scene history
     * If the sceneName isn't found, no scene is loaded
     * @param sceneName the scene to load
     */
    public void BacktrackToScene(int buildIndex)
    {
        if (sceneHistory.Contains(buildIndex))
        {
            // Keep popping off scenes until you reach the desired index
            while (sceneHistory.Peek() != buildIndex) sceneHistory.Pop();

            SceneManager.LoadScene(buildIndex);
        }
        else
        {
            Debug.LogWarning(LOG_TAG + ": " + buildIndex + " is not in the scene history");
        }
    }

    /**
     * Adds an argument to the argument list. If the key already exists, replace it
     * @param key the key to use
     * @param value the value attached to the key
     */
    public void AddArgument(string key, int value)
    {
        if (arguments.ContainsKey(key))
        {
            arguments[key] = value;
        }
        else
        {
            arguments.Add(key, value);
        }
    }

    /**
     * Gets the value of a particular key from the arguments or default if it doesn't exist
     * @param key the key to search for
     * @param defaultValue the default to use if the key isn't found
     * @return either the value attached to the key or defaultValue
     */
    public int GetArgument(string key, int defaultValue)
    {
        if (arguments.TryGetValue(key, out int value))
        {
            return value;
        }
        else
        {
            return defaultValue;
        }
    }
}
