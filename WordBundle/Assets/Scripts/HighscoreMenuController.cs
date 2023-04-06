using System.Collections.Generic;
using UnityEngine;
using TMPro;

/// <summary>
/// Controller for the Highscore scene UI
/// </summary>
public class HighscoreMenuController : MonoBehaviour
{
    private HighscoreManager highscoreManager;
    private SceneLoader sceneLoader;

    [SerializeField]
    List<TextMeshProUGUI> scoreFields;

    /// <summary>
    /// Called before the first frame update
    /// </summary>
    private void Awake()
    {
        sceneLoader = FindObjectOfType<SceneLoader>();

        highscoreManager = new HighscoreManager();

        List<int> scores = new List<int>();
        scores.AddRange(highscoreManager.GetScores());

        for (int i = 0;i < scores.Count;i++)
        {
            scoreFields[i].text = scores[i].ToString();
        }
    }

    /// <summary>
    /// Returns the user to the main menu
    /// </summary>
    public void ReturnToMain()
    {
        sceneLoader.BacktrackToScene(SceneConstants.MAIN_MENU_INDEX);
    }
}
