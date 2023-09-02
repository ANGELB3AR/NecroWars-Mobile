using System;

public class GameManager : Singleton<GameManager>
{
    public GameState State;

    public static event Action<GameState> OnGameStateChanged;

    private Progression progression;
    private Hoard playerHoard;

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
                break;
            case GameState.OptionsMenu:
                break;
            case GameState.Playing:
                HandleGamePlaying();
                break;
            case GameState.Paused:
                break;
            case GameState.GameWon:
                break;
            case GameState.GameLost:
                break;
            default:
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private void HandleGamePlaying()
    {
        progression = FindObjectOfType<Progression>();
    }
}

public enum GameState
{
    MainMenu,
    OptionsMenu,
    Playing,
    Paused,
    GameWon,
    GameLost
}