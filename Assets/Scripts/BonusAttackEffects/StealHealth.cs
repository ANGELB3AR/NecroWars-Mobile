using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealHealth : ScriptableObject, IBonusAttackEffect
{
    // Enter fields available for Designer to customize
    public float percentageHealthToSteal;
    
    public ParticleSystem playerAttackEffect;
    public ParticleSystem aiAttackEffect;
    public ParticleSystem healingEffect;

    // Enter logic for stealing health
    public void ApplyBonusAttackEffect(Health[] targets, bool isPlayer, Creature attacker)
    {
        float healthStolen = 0f;

        foreach (Health targetHealth in targets)
        {
            float healthToSteal = targetHealth.GetCurrentHealth() * (percentageHealthToSteal / 100);
            targetHealth.TakeDamage(healthToSteal);
            healthStolen += healthToSteal;

            // Plays a VFX based on whether the attacking creature is the Player or AI
            Instantiate((isPlayer) ? playerAttackEffect : aiAttackEffect,
                targetHealth.transform.position,
                Quaternion.identity);
        }

        attacker.GetHealthComponent().Heal(healthStolen);
        Instantiate(healingEffect, attacker.transform.position, Quaternion.identity);
    }
}
