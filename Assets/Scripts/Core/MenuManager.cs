using UnityEngine;

public class MenuManager : Singleton<MenuManager>
{
    [SerializeField] GameObject mainMenu;
    [SerializeField] GameObject optionsMenu;

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
    }
}
