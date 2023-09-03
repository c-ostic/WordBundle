using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Controller for the Main Menu UI
/// </summary>
public class MainMenuController : MonoBehaviour
{
    private const string LOG_TAG = nameof(MainMenuController);

    [SerializeField]
    private Button resumeButton;
    [SerializeField]
    private Button quitButton;

    private SceneLoader sceneLoader;

    /// <summary>
    /// Called before the first frame update
    /// </summary>
    private void Start()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();

        if (PlayerPrefs.HasKey(GameState.SAVE_FILE_EXISTS_KEY) && PlayerPrefs.GetInt(GameState.SAVE_FILE_EXISTS_KEY) == 1)
        {
            resumeButton.interactable = true;
        }
        else
        {
            resumeButton.interactable = false;
        }

#if UNITY_WEBGL
        quitButton.gameObject.SetActive(false);
# endif
    }

    /// <summary>
    /// Starts a new instance of the game
    /// </summary>
    public void StartNewGame()
    {
        sceneLoader.AddArgument(SceneConstants.NEW_GAME_INFO, SceneConstants.NEW_GAME_REQUEST);
        sceneLoader.LoadNewScene(SceneConstants.GAME_SCREEN_INDEX);
    }

    /// <summary>
    /// Resumes an existing game
    /// </summary>
    public void ResumeGame()
    {
        sceneLoader.AddArgument(SceneConstants.NEW_GAME_INFO, SceneConstants.RESUME_GAME_REQUEST);
        sceneLoader.LoadNewScene(SceneConstants.GAME_SCREEN_INDEX);
    }

    /// <summary>
    /// Brings the user to a version of the pause menu that shows how to play
    /// </summary>
    public void HowToPlay()
    {
        sceneLoader.AddArgument(SceneConstants.REQUEST_INFO, SceneConstants.HOW_TO_PLAY_REQUEST);
        sceneLoader.LoadNewScene(SceneConstants.PAUSE_MENU_INDEX);
    }

    /// <summary>
    /// Brings the user to the highscores scene
    /// </summary>
    public void ViewHighscores()
    {
        sceneLoader.LoadNewScene(SceneConstants.HIGHSCORE_MENU_INDEX);
    }

    /// <summary>
    /// Quits the application
    /// </summary>
    public void Quit()
    {
        Debug.Log(LOG_TAG + ": Quitting Application");
        Application.Quit();
    }
}
