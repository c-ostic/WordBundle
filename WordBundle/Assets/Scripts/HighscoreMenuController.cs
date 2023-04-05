using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using TMPro;
using System;

public class HighscoreMenuController : MonoBehaviour
{
    private HighscoreManager highscoreManager;
    private SceneLoader sceneLoader;

    [SerializeField]
    List<TextMeshProUGUI> scoreFields;

    /**
     * Start is called before the first frame update
     */
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

    public void ReturnToMain()
    {
        sceneLoader.BacktrackToScene(SceneConstants.MAIN_MENU_INDEX);
    }
}
