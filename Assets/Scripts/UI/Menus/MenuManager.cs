using System.Collections;
using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;
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
        if (gameState == GameState.GameWon)
        {
            AddDelay(3);
        }

        mainMenu.SetActive(gameState == GameState.MainMenu);
        optionsMenu.SetActive(gameState == GameState.OptionsMenu);
        pauseMenu.SetActive(gameState == GameState.Paused);
        gameWinMenu.SetActive(gameState == GameState.GameWon);
        gameLoseMenu.SetActive(gameState == GameState.GameLost);
    }

    private IEnumerator AddDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Debug.Log("Waited some time");
    }
}
