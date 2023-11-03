using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHoardHealthUI : MonoBehaviour
{
    [SerializeField] Slider healthBarSlider;

    [SerializeField] private float maxHoardHealth = 0f;
    [SerializeField] private float currentHoardHealth = 0f;

    private void OnEnable()
    {
        Health.OnPlayerCreatureHealthUpdated += OnPlayerHoardHealthChanged;
        Health.OnPlayerHoardMaxHealthUpdated += OnPlayerHoardMaxHealthChanged;
    }

    private void OnDisable()
    {
        Health.OnPlayerCreatureHealthUpdated -= OnPlayerHoardHealthChanged;
        Health.OnPlayerHoardMaxHealthUpdated -= OnPlayerHoardMaxHealthChanged;
    }

    private void OnPlayerHoardMaxHealthChanged(float creatureMaxHealth)
    {
        maxHoardHealth += creatureMaxHealth;

        healthBarSlider.maxValue = maxHoardHealth;
    }

    private void OnPlayerHoardHealthChanged(float deltaHealth)
    {
        currentHoardHealth += deltaHealth;

        healthBarSlider.value = currentHoardHealth;
    }

    public void InitializePlayerHoardHealth(CreatureSO[] playerStartingHoard)
    {
        float health = 0;

        foreach (var creature in playerStartingHoard)
        {
            health += creature.maxHealth;
        }

        maxHoardHealth = health;
        currentHoardHealth = health;

        healthBarSlider.maxValue = maxHoardHealth;
        healthBarSlider.value = currentHoardHealth;
    }
}
