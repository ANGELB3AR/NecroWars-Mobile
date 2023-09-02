using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameWinMenu : MonoBehaviour
{
    public void LoadNextLevel()
    {
        GameManager.Instance.UpdateGameState(GameState.RoundStart);
    }

    public void LoadMainMenu()
    {
        GameManager.Instance.UpdateGameState(GameState.MainMenu);
    }
}
