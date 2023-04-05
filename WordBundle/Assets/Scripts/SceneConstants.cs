using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SceneConstants
{
    // Build indexes of the scenes
    public const int MAIN_MENU_INDEX = 0;
    public const int GAME_SCREEN_INDEX = 1;
    public const int PAUSE_MENU_INDEX = 2;
    public const int HIGHSCORE_MENU_INDEX = 3;

    // Request key and values for pause menu (which doubles as game over and how to play)
    public const string REQUEST_INFO = "request";
    public const string SCORE_INFO = "score";

    public const int PAUSE_REQUEST = 0;
    public const int GAME_OVER_REQUEST = 1;
    public const int HOW_TO_PLAY_REQUEST = 2;

    // Request key and values for starting a new game vs. resuming a game
    public const string NEW_GAME_INFO = "newGame";

    public const int NEW_GAME_REQUEST = 1;
    public const int RESUME_GAME_REQUEST = 0;
}
