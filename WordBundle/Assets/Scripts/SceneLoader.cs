using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Persists throughout the entire application and manages the loading of new scenes, returning to old scenes, and passing information
/// </summary>
public class SceneLoader : MonoBehaviour
{
    private const string LOG_TAG = nameof(SceneLoader);

    // Stack to keep track of the history of loaded scenes
    private Stack<int> sceneHistory;

    // Argument list
    private Dictionary<string, int> arguments;

    /// <summary>
    /// Called at the start of the scene, before Start() and the first frame update
    /// </summary>
    private void Awake()
    {
        // This will persist throughout the application
        DontDestroyOnLoad(this);

        sceneHistory = new Stack<int>();
        sceneHistory.Push(SceneManager.GetActiveScene().buildIndex);

        arguments = new Dictionary<string, int>();
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        // If escape is pressed (back button for android), move back one scene in the history
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (sceneHistory.Count == 1)
            {
                Debug.Log(LOG_TAG + ": Quitting Application");
                Application.Quit();
            }
            else
            {
                sceneHistory.Pop();
                SceneManager.LoadScene(sceneHistory.Peek());
            }
        }
    }

    /// <summary>
    /// Creates a new instance of sceneName and adds it to the history.
    /// If the sceneName isn't found, no scene is loaded
    /// </summary>
    /// <param name="buildIndex"> the scene to load </param>
    public void LoadNewScene(int buildIndex)
    {
        sceneHistory.Push(buildIndex);
        SceneManager.LoadScene(buildIndex);
    }

    /// <summary>
    /// Backtracks through the history to find sceneName and loads it, removing everything in front of it in the scene history.
    /// If the sceneName isn't found, no scene is loaded
    /// </summary>
    /// <param name="buildIndex"> the scene to load </param>
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

    /// <summary>
    /// Adds an argument to the argument list. If the key already exists, replace it
    /// </summary>
    /// <param name="key"> the key to use </param>
    /// <param name="value"> the value attached to the key</param>
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

    /// <summary>
    /// Gets the value of a particular key from the arguments or default if it doesn't exist
    /// </summary>
    /// <param name="key"> the key to search for </param>
    /// <param name="defaultValue"> the default to use if the key isn't found </param>
    /// <returns> either the value attached to the key or defaultValue </returns>
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
