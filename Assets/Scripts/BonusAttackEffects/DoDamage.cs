using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "NewDoDamageEffect", menuName = "Create New Bonus Attack Effect/DoDamage")]

public class DoDamage : SerializedScriptableObject, IBonusAttackEffect
{
    [EnumToggleButtons]
    public DamageType damageType;

    [ShowIf(nameof(damageType), DamageType.HitPoints)]
    public float damageAmount;
    [ShowIf(nameof(damageType), DamageType.Percentage)]
    [ProgressBar(0, 100)]
    public float damagePercentage;
    [ShowIf(nameof(damageType), DamageType.AreaMultiplier)]
    public float baseDamage;

    public bool playerAndAIAreDifferent = false;

    // IF PLAYER ATTACKING
    [TabGroup("VFX", "Player Attacker")]
    [ShowIf(nameof(playerAndAIAreDifferent))]
    public ParticleSystem attackVFXOnPlayerAttacker = null;
    [TabGroup("VFX", "Player Attacker")]
    [ShowIf(nameof(playerAndAIAreDifferent))]
    [PropertyTooltip("Damage effects are optional. This effect will play on AI creatures damaged by the Player")]
    public ParticleSystem damageVFXIfPlayerAttacks = null;

    // IF AI ATTACKING
    [TabGroup("VFX", "AI Attacker")]
    [ShowIf(nameof(playerAndAIAreDifferent))]
    public ParticleSystem attackVFXOnAIAttacker = null;
    [TabGroup("VFX", "AI Attacker")]
    [ShowIf(nameof(playerAndAIAreDifferent))]
    [PropertyTooltip("Damage effects are optional. This effect will play on Player creatures damaged by AI")]
    public ParticleSystem damageVFXIfAIAttacks = null;

    // IF NOT DIFFERENT
    [TabGroup("VFX")]
    [HideIf(nameof(playerAndAIAreDifferent))]
    public ParticleSystem attackVFX;
    [TabGroup("VFX")]
    [HideIf(nameof(playerAndAIAreDifferent))]
    [PropertyTooltip("Damage effects are optional. This effect will play on any creature damaged by this effect")]
    public ParticleSystem damageVFX;

    public void ApplyBonusAttackEffect(Health[] targets, bool isPlayer, Creature attacker)
    {
        foreach (Health targetHealth in targets)
        {
            DealDamage(targetHealth, targets.Length);

            PlayDamageVFX(isPlayer, targetHealth);
        }

        PlayAttackVFX(isPlayer, attacker);
    }

    private void DealDamage(Health targetHealth, int areaMultiplier)
    {
        float damageToDeal = 0f;

        switch (damageType)
        {
            case DamageType.HitPoints:
                damageToDeal = damageAmount;
                break;
            case DamageType.Percentage:
                damageToDeal = targetHealth.GetCurrentHealth() * (damagePercentage / 100);
                break;
            case DamageType.AreaMultiplier:
                damageToDeal = baseDamage * areaMultiplier;
                break;
            default:
                break;
        }

        targetHealth.TakeDamage(damageToDeal);
    }

    private void PlayDamageVFX(bool isPlayer, Health targetHealth)
    {
        if (playerAndAIAreDifferent)
        {
            if (damageVFXIfPlayerAttacks == null || damageVFXIfAIAttacks == null) { return; }

            // Plays a VFX based on whether the attacking creature is the Player or AI
            Instantiate((isPlayer) ? damageVFXIfPlayerAttacks : damageVFXIfAIAttacks,
                targetHealth.transform.position,
                Quaternion.identity);
        }
        else
        {
            if (damageVFX == null) { return; }

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
