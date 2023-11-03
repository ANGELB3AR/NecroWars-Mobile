using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] Slider progressBarSlider;

    private HoardSpawner hoardSpawner;

    private int startingHoardQuantity = 0;
    private int currentHoardCount = 0;

    private void Awake()
    {
        hoardSpawner = FindObjectOfType<HoardSpawner>();
    }

    private void OnEnable()
    {
        hoardSpawner.OnLevelCreationComplete += HoardSpawner_OnLevelCreationComplete;
    }

    private void OnDisable()
    {
        hoardSpawner.OnLevelCreationComplete -= HoardSpawner_OnLevelCreationComplete;
    }

    private void HoardSpawner_OnLevelCreationComplete(int hoardCount)
    {
        startingHoardQuantity = hoardCount;
        currentHoardCount = hoardCount;

        Hoard[] hoards = FindObjectsOfType<Hoard>();

        foreach (var hoard in hoards)
        {
            hoard.OnHoardDied += HandleHoardDied;
        }
        
        progressBarSlider.maxValue = startingHoardQuantity;
        progressBarSlider.value = 0f;
    }

    private void HandleHoardDied(Hoard hoard)
    {
        currentHoardCount--;

        progressBarSlider.value = startingHoardQuantity - currentHoardCount;

        hoard.OnHoardDied -= HandleHoardDied;

        if (currentHoardCount == 0)
        {
            GameManager.Instance.UpdateGameState(GameState.GameWon);
        }
    }
}