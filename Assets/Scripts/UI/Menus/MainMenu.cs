using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public static readonly string GAME_SCENE = "Scene_Game";

    public void PlayGame()
    {
        SceneManager.LoadScene(GAME_SCENE);
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
