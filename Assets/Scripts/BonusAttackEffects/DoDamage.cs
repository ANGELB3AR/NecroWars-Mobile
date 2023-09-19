using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NewBonusAttackEffect", menuName = "Create New Bonus Attack Effect/DoDamage")]

public class DoDamage : SerializedScriptableObject, IBonusAttackEffect
{
    public float damageAmount;

    public bool playerAndAIAreDifferent = false;

    // IF PLAYER ATTACKING
    [TabGroup("VFX", "Player Attacker")]
    [ShowIf(nameof(playerAndAIAreDifferent))]
    public ParticleSystem attackVFXOnPlayerAttacker = null;
    [TabGroup("VFX", "Player Attacker")]
    [ShowIf(nameof(playerAndAIAreDifferent))]
    public ParticleSystem damageVFXIfPlayerAttacks = null;

    // IF AI ATTACKING
    [TabGroup("VFX", "AI Attacker")]
    [ShowIf(nameof(playerAndAIAreDifferent))]
    public ParticleSystem attackVFXOnAIAttacker = null;
    [TabGroup("VFX", "AI Attacker")]
    [ShowIf(nameof(playerAndAIAreDifferent))]
    public ParticleSystem damageVFXIfAIAttacks = null;

    // IF NOT DIFFERENT
    [TabGroup("VFX")]
    [HideIf(nameof(playerAndAIAreDifferent))]
    public ParticleSystem attackVFX;
    [TabGroup("VFX")]
    [HideIf(nameof(playerAndAIAreDifferent))]
    public ParticleSystem damageVFX;

    public void ApplyBonusAttackEffect(Health[] targets, bool isPlayer, Creature attacker)
    {
        foreach (Health targetHealth in targets)
        {
            targetHealth.TakeDamage(damageAmount);

            PlayDamageVFX(isPlayer, targetHealth);
        }

        PlayAttackVFX(isPlayer, attacker);
    }

    private void PlayDamageVFX(bool isPlayer, Health targetHealth)
    {
        if (playerAndAIAreDifferent)
        {
            // Plays a VFX based on whether the attacking creature is the Player or AI
            Instantiate((isPlayer) ? damageVFXIfPlayerAttacks : damageVFXIfAIAttacks,
                targetHealth.transform.position,
                Quaternion.identity);
        }
        else
        {
            Instantiate(damageVFX, targetHealth.transform.position, Quaternion.identity);
        }

    }

    private void PlayAttackVFX(bool isPlayer, Creature attacker)
    {
        if (playerAndAIAreDifferent)
        {
            // Plays a VFX based on whether the attacking creature is the Player or AI
            Instantiate((isPlayer) ? attackVFXOnPlayerAttacker : attackVFXOnAIAttacker,
                attacker.transform.position,
                Quaternion.identity);
        }
        else
        {
            Instantiate(attackVFX, attacker.transform.position, Quaternion.identity);
        }

    }
}
