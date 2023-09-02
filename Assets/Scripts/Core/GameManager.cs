using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    private Progression progression;
    private Hoard playerHoard;

    public GameState State;
    public static readonly string GAME_SCENE = "Scene_Game";
    public static readonly string MAIN_MENU_SCENE = "Scene_MainMenu";

    public static event Action<GameState> OnGameStateChanged;


    private void Start()
    {
        UpdateGameState(GameState.MainMenu);
    }

    public void UpdateGameState(GameState newState)
    {
        State = newState;

        switch (newState)
        {
            case GameState.MainMenu:
                HandleMainMenu();
                break;
            case GameState.OptionsMenu:
                break;
            case GameState.RoundStart:
                HandleRoundStart();
                break;
            case GameState.Paused:
                break;
            case GameState.GameWon:
                HandleGameWon();
                break;
            case GameState.GameLost:
                break;
            default:
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private void HandleMainMenu()
    {
        if (SceneManager.GetActiveScene().name == MAIN_MENU_SCENE) { return; }

        SceneManager.LoadScene(MAIN_MENU_SCENE);
    }

    private void HandleGameWon()
    {
        int currentLevel = PlayerPrefs.GetInt(Progression.CURRENT_LEVEL_KEY);
        int nextLevel = currentLevel++;

        PlayerPrefs.SetInt(Progression.CURRENT_LEVEL_KEY, nextLevel);
    }

    private void HandleRoundStart()
    {
        SceneManager.LoadScene(GAME_SCENE);

        progression = FindObjectOfType<Progression>();
    }
}

public enum GameState
{
    MainMenu,
    OptionsMenu,
    RoundStart,
    Paused,
    GameWon,
    GameLost
}