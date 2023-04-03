using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField]
    private Button resumeButton;

    private SceneLoader sceneLoader;

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
    }

    public void StartNewGame()
    {
        sceneLoader.AddArgument(SceneConstants.NEW_GAME_INFO, SceneConstants.NEW_GAME_REQUEST);
        sceneLoader.LoadNewScene(SceneConstants.GAME_SCREEN_INDEX);
    }

    public void ResumeGame()
    {
        sceneLoader.AddArgument(SceneConstants.NEW_GAME_INFO, SceneConstants.RESUME_GAME_REQUEST);
        sceneLoader.LoadNewScene(SceneConstants.GAME_SCREEN_INDEX);
    }

    public void HowToPlay()
    {
        sceneLoader.AddArgument(SceneConstants.REQUEST_INFO, SceneConstants.HOW_TO_PLAY_REQUEST);
        sceneLoader.LoadNewScene(SceneConstants.PAUSE_MENU_INDEX);
    }

    public void ViewHighscores()
    {

    }

    public void Quit()
    {
        Application.Quit();
    }
}
