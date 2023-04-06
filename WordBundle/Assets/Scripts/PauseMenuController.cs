using UnityEngine;
using TMPro;

/// <summary>
/// Controller for the Pause Menu UI
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    private const string PAUSE_TEXT = "Paused";
    private const string GAME_OVER_TEXT = "Game Over";
    private const string HOW_TO_PLAY_TEXT = "How To Play";

    [SerializeField]
    private TextMeshProUGUI scoreLabel;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI title;
    [SerializeField]
    private GameObject resumeButton;

    private SceneLoader sceneLoader;

    /// <summary>
    /// Called at the start of the scene, before Start() and the first frame update
    /// </summary>
    private void Awake()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();

        switch (sceneLoader.GetArgument(SceneConstants.REQUEST_INFO, SceneConstants.PAUSE_REQUEST))
        {
            case SceneConstants.PAUSE_REQUEST:
                {
                    title.text = PAUSE_TEXT;
                    resumeButton.SetActive(true);
                    scoreLabel.gameObject.SetActive(true);
                    scoreText.gameObject.SetActive(true);
                    break;
                }
            case SceneConstants.GAME_OVER_REQUEST:
                {
                    title.text = GAME_OVER_TEXT;
                    resumeButton.SetActive(false);
                    scoreLabel.gameObject.SetActive(true);
                    scoreText.gameObject.SetActive(true);
                    break;
                }
            case SceneConstants.HOW_TO_PLAY_REQUEST:
                {
                    title.text = HOW_TO_PLAY_TEXT;
                    resumeButton.SetActive(false);
                    scoreLabel.gameObject.SetActive(false);
                    scoreText.gameObject.SetActive(false);
                    break;
                }
        }

        scoreText.text = sceneLoader.GetArgument(SceneConstants.SCORE_INFO, 0).ToString();

        // Makes sure the game is resumed whether the user presses the resume button, or hits the back button on Android (Escape on keyboard)
        sceneLoader.AddArgument(SceneConstants.NEW_GAME_INFO, SceneConstants.RESUME_GAME_REQUEST);
    }

    /// <summary>
    /// Returns the user to the game that was paused
    /// </summary>
    public void ResumeGame()
    {
        sceneLoader.BacktrackToScene(SceneConstants.GAME_SCREEN_INDEX);
    }

    /// <summary>
    /// Returns the user to the main menu
    /// </summary>
    public void ReturnToMain()
    {
        sceneLoader.BacktrackToScene(SceneConstants.MAIN_MENU_INDEX);
    }
}
