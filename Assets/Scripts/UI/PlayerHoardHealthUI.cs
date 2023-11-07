using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHoardHealthUI : MonoBehaviour
{
    [SerializeField] Slider healthBarSlider;

    [Header("Hoard Health")]
    [SerializeField] private float maxHoardHealth = 0f;
    [SerializeField] private float currentHoardHealth = 0f;

    [Header("Hoard Size")]
    [SerializeField] private RectTransform sliderRect;
    [SerializeField] private float minSliderWidth;
    [SerializeField] private float maxSliderWidth;

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
        
        if (creatureMaxHealth > 0)
        {
            currentHoardHealth += creatureMaxHealth;
        }

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
