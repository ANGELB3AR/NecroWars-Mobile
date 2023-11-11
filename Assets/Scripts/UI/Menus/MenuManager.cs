using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
    [SerializeField] GameObject startingHoardMenu;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject gameWinMenu;
    [SerializeField] GameObject gameLoseMenu;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void GameManager_OnGameStateChanged(GameState gameState)
    {
        mainMenu.SetActive(gameState == GameState.MainMenu);
        optionsMenu.SetActive(gameState == GameState.OptionsMenu);
        startingHoardMenu.SetActive(gameState == GameState.StartingHoardMenu);
        pauseMenu.SetActive(gameState == GameState.Paused);
        gameWinMenu.SetActive(gameState == GameState.GameWon);
        gameLoseMenu.SetActive(gameState == GameState.GameLost);
    }
}
