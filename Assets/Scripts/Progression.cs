using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Progression : MonoBehaviour
{
    private int currentLevel;

    public const string CURRENT_LEVEL_KEY = "Level";

    private void Start()
    {
        StartNewLevel();
    }

    private void StartNewLevel()
    {
        currentLevel = PlayerPrefs.GetInt(CURRENT_LEVEL_KEY, 1);


    }
}
