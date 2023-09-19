using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NewBonusAttackEffect", menuName = "Create New Bonus Attack Effect/StealHealth")]
public class StealHealth : SerializedScriptableObject, IBonusAttackEffect
{
    // Enter fields available for Designer to customize
    public float percentageHealthToSteal;
    
    public ParticleSystem damageVFXIfPlayerAttacks;
    public ParticleSystem damageVFXIfAIAttacks;
    public ParticleSystem healingVFXOnPlayerAttacker;
    public ParticleSystem healingVFXOnAIAttacker;

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
            Instantiate((isPlayer) ? damageVFXIfPlayerAttacks : damageVFXIfAIAttacks,
                targetHealth.transform.position,
                Quaternion.identity);
        }

        attacker.GetHealthComponent().Heal(healthStolen);

        // Plays a VFX based on whether the attacking creature is the Player or AI
        Instantiate((isPlayer) ? healingVFXOnPlayerAttacker : healingVFXOnAIAttacker, 
            attacker.transform.position, 
            Quaternion.identity);
    }
}
