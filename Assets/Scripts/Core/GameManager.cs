using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zindeaxx.SoundSystem;

public class GameManager : Singleton<GameManager>
{
    private Progression progression;
    private Hoard playerHoard;

    public GameState State;

    [SerializeField] private SoundSet gameWinSFX;
    [SerializeField] private SoundSet gameLoseSFX;

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
                HandleGameLost();
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
        int currentLevel = PlayerPrefs.GetInt(Progression.CURRENT_LEVEL_KEY, 1);
        int nextLevel = currentLevel++;

        PlayerPrefs.SetInt(Progression.CURRENT_LEVEL_KEY, nextLevel);

        SoundManager soundManager = FindFirstObjectByType<SoundManager>();
        soundManager.PlaySound(gameWinSFX);
    }

    private void HandleGameLost()
    {
        SoundManager soundManager = FindFirstObjectByType<SoundManager>();
        soundManager.PlaySound(gameLoseSFX);
    }

    private void HandleRoundStart()
    {
        SceneManager.LoadScene(GAME_SCENE);
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