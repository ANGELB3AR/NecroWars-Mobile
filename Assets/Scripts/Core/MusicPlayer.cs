using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zindeaxx.SoundSystem;

public class MusicPlayer : Singleton<MusicPlayer>
{
    private SoundManager soundManager;

    [SerializeField] SoundSet menuMusic;
    [SerializeField] SoundSet gameMusic;
    [SerializeField] SoundSet winMusic;
    [SerializeField] SoundSet loseMusic;

    private void OnEnable()
    {
        GameManager.OnGameStateChanged += GameManager_OnGameStateChanged;
    }

    private void OnDisable()
    {
        GameManager.OnGameStateChanged -= GameManager_OnGameStateChanged;
    }

    private void Awake()
    {
        soundManager = GetComponent<SoundManager>();
    }

    private void GameManager_OnGameStateChanged(GameState gameState)
    {
        switch (gameState)
        {
            case GameState.MainMenu:
                soundManager.PlaySound(menuMusic);
                break;
            case GameState.OptionsMenu:
                break;
            case GameState.RoundStart:
                soundManager.PlaySound(gameMusic);
                break;
            case GameState.Paused:
                break;
            case GameState.GameWon:
                soundManager.PlaySound(winMusic);
                break;
            case GameState.GameLost:
                soundManager.PlaySound(loseMusic);
                break;
            default:
                break;
        }
    }

}
