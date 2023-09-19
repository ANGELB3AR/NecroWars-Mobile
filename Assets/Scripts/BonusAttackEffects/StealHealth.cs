using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NewStealHealthEffect", menuName = "Create New Bonus Attack Effect/StealHealth")]
public class StealHealth : SerializedScriptableObject, IBonusAttackEffect
{
    [EnumToggleButtons]
    public DamageType damageType;

    [ShowIf(nameof(damageType), DamageType.HitPoints)]
    public float amountHealthToSteal = 0f;
    [ShowIf(nameof(damageType), DamageType.Percentage)]
    [ProgressBar(0, 100)]
    public float percentageHealthToSteal = 0f;
    [ShowIf(nameof(damageType), DamageType.AreaMultiplier)]
    public float baseHealthToSteal = 0f;

    public bool playerAndAIAreDifferent = false;

    // IF PLAYER ATTACKING
    [TabGroup("VFX", "Player Attacker")]
    [ShowIf(nameof(playerAndAIAreDifferent))]
    public ParticleSystem healVFXOnPlayerAttacker = null;
    [TabGroup("VFX", "Player Attacker")]
    [ShowIf(nameof(playerAndAIAreDifferent))]
    public ParticleSystem damageVFXIfPlayerAttacks = null;

    // IF AI ATTACKING
    [TabGroup("VFX", "AI Attacker")]
    [ShowIf(nameof(playerAndAIAreDifferent))]
    public ParticleSystem healVFXOnAIAttacker = null;
    [TabGroup("VFX", "AI Attacker")]
    [ShowIf(nameof(playerAndAIAreDifferent))]
    public ParticleSystem damageVFXIfAIAttacks = null;

    // IF NOT DIFFERENT
    [TabGroup("VFX")]
    [HideIf(nameof(playerAndAIAreDifferent))]
    public ParticleSystem healVFX;
    [TabGroup("VFX")]
    [HideIf(nameof(playerAndAIAreDifferent))]
    public ParticleSystem damageVFX;


    // Enter logic for stealing health
    public void ApplyBonusAttackEffect(Health[] targets, bool isPlayer, Creature attacker)
    {
        float healthStolen = 0f;

        foreach (Health targetHealth in targets)
        {
            healthStolen = DealDamage(healthStolen, targetHealth, targets.Length);

            PlayDamageVFX(isPlayer, targetHealth);
        }

        attacker.GetHealthComponent().Heal(healthStolen);

        PlayHealVFX(isPlayer, attacker);
    }

    private float DealDamage(float healthStolen, Health targetHealth, int areaMultiplier)
    {
        float healthToSteal = 0f;

        switch (damageType)
        {
            case DamageType.HitPoints:
                healthToSteal = amountHealthToSteal;
                break;
            case DamageType.Percentage:
                healthToSteal = targetHealth.GetCurrentHealth() * (percentageHealthToSteal / 100);
                break;
            case DamageType.AreaMultiplier:
                healthToSteal = baseHealthToSteal * areaMultiplier;
                break;
            default:
                break;
        }

        targetHealth.TakeDamage(healthToSteal);
        healthStolen += healthToSteal;
        return healthStolen;
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

    private void PlayHealVFX(bool isPlayer, Creature attacker)
    {
        if (playerAndAIAreDifferent)
        {
            // Plays a VFX based on whether the attacking creature is the Player or AI
            Instantiate((isPlayer) ? healVFXOnPlayerAttacker : healVFXOnAIAttacker,
                attacker.transform.position,
                Quaternion.identity);
        }
        else
        {
            Instantiate(healVFX, attacker.transform.position, Quaternion.identity);
        }
    }
}
