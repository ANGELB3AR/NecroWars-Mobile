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
    [SerializeField] private float widthInterval;

    private void OnEnable()
    {
        Health.OnPlayerCreatureHealthUpdated += OnPlayerHoardHealthChanged;
        Health.OnPlayerHoardMaxHealthUpdated += OnPlayerHoardMaxHealthChanged;
        Hoard.OnPlayerHoardSizeUpdated += OnPlayerHoardSizeChanged;
    }

    private void OnDisable()
    {
        Health.OnPlayerCreatureHealthUpdated -= OnPlayerHoardHealthChanged;
        Health.OnPlayerHoardMaxHealthUpdated -= OnPlayerHoardMaxHealthChanged;
        Hoard.OnPlayerHoardSizeUpdated -= OnPlayerHoardSizeChanged;
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

    private void OnPlayerHoardSizeChanged(int hoardSize)
    {
        float newWidth = (float)hoardSize * widthInterval;
        newWidth = Mathf.Clamp(newWidth, minSliderWidth, maxSliderWidth);

        Vector2 size = sliderRect.sizeDelta;
        size.x = newWidth;
        sliderRect.sizeDelta = size;
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
