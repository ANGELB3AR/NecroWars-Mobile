using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public GameState State;

    public static event Action<GameState> OnGameStateChanged;

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
                break;
            case GameState.Paused:
                break;
            default:
                break;
        }

        OnGameStateChanged?.Invoke(newState);
    }
}

public enum GameState
{
    MainMenu,
    OptionsMenu,
    Playing,
    Paused
}