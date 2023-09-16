using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI currentLevelText;

    private void Start()
    {
        currentLevelText.text = $"Current Level: {PlayerPrefs.GetInt(Progression.CURRENT_LEVEL_KEY, 1)}";
    }

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
