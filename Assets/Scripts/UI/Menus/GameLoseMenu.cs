using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameLoseMenu : MonoBehaviour
{
    public void ReloadLevel()
    {
        GameManager.Instance.UpdateGameState(GameState.RoundStart);
    }

    public void LoadMainMenu()
    {
        GameManager.Instance.UpdateGameState(GameState.MainMenu);
    }
}
