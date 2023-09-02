using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        GameManager.Instance.UpdateGameState(GameState.RoundStart);
    }

    public void PlayAd()
    {
        Debug.Log("Ads coming soon!");
    }

    public void SwitchToSettingsMenu()
    {
        Debug.Log("Settings Menu under construction");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
