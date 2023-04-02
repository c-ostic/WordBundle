using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    private const string LOG_TAG = nameof(GameManager);

    [SerializeField]
    private int timer_length = 3; // timer length in seconds
    [SerializeField]
    private List<ToggleButton> letterButtons;
    [SerializeField]
    private Image letterTimer;
    [SerializeField]
    private TextMeshProUGUI wordEntryText;
    [SerializeField]
    private TextMeshProUGUI scoreText;
    [SerializeField]
    private TextMeshProUGUI previousWord;
    [SerializeField]
    private TextMeshProUGUI previousWordScore;

    // The current state of the game
    private GameState gameState;

    // The current time on the timer
    private int current_timer_value;

    private void Awake()
    {
        // TODO: Make this available to change
        gameState = new GameState(true);

        current_timer_value = 0;
    }

    private void Start()
    {
        // Add a listener to each of the buttons 
        for (int i = 0;i < letterButtons.Count;i++)
        {
            int index = i;
            letterButtons[i].Toggled += (bool toggled) => pressLetterButton(toggled, index);
        }
    }

    private void Update()
    {
        
    }

    private void pressLetterButton(bool toggled, int index)
    {
        Debug.Log(toggled + " " + index);
    }
}
