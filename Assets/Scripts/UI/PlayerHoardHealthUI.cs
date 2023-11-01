using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHoardHealthUI : MonoBehaviour
{
    [SerializeField] Slider healthBarSlider;

    private Hoard playerHoard;
    [SerializeField] private float maxHoardHealth = 0f;
    [SerializeField] private float currentHoardHealth = 0f;

    private void Awake()
    {
        playerHoard = GetComponentInParent<Hoard>();
    }

    private void OnEnable()
    {
        playerHoard.OnCreatureAddedToHoard += PlayerHoard_OnCreatureAddedToHoard;
    }

    private void OnDisable()
    {
        playerHoard.OnCreatureAddedToHoard -= PlayerHoard_OnCreatureAddedToHoard;
    }

    private void PlayerHoard_OnCreatureAddedToHoard(Creature creature)
    {
        Health creatureHealth = creature.GetHealthComponent();
        creatureHealth.OnHealthUpdated += Creature_OnHealthUpdated;
        creatureHealth.OnCreatureDied += Creature_OnCreatureDied;

        maxHoardHealth += creatureHealth.GetMaxHealth();
        healthBarSlider.maxValue = maxHoardHealth;

        currentHoardHealth += creatureHealth.GetCurrentHealth();
        healthBarSlider.value = currentHoardHealth;
    }

    private void Creature_OnCreatureDied(Creature creature)
    {
        Health creatureHealth = creature.GetHealthComponent();
        creatureHealth.OnHealthUpdated -= Creature_OnHealthUpdated;
        creatureHealth.OnCreatureDied -= Creature_OnCreatureDied;

        maxHoardHealth -= creatureHealth.GetMaxHealth();
    }

    private void Creature_OnHealthUpdated(float changeInHealth)
    {
        currentHoardHealth += changeInHealth;
        
        healthBarSlider.value = currentHoardHealth;
    }
}
