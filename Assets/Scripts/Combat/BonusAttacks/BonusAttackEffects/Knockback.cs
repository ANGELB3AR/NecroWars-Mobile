using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using DG.Tweening;
using System;

[CreateAssetMenu(fileName = "NewKnockbackEffect", menuName = "Bonus Attack Effect/Knockback")]
public class Knockback : SerializedScriptableObject, IBonusAttackEffect
{
    [PropertyTooltip("If checked, the amount of force calculated will diminish as targets are further away from the attacker at the time the effect is applied")]
    public bool applyDiminishingForce = false;
    [PropertyTooltip("Amount of force applied to targets")]
    public float power = 5f;
    [ShowIf(nameof(applyDiminishingForce))]
    [PropertyTooltip("The radius around the attacker from which the force applied is calculated. Larger radius means greater force applied to nearby enemies")]
    public float radius = 3f;

    public bool hasVFX = false;

    [ShowIf(nameof(hasVFX))]
    public bool playerAndAIAreDifferent = false;

    // IF PLAYER ATTACKING
    [TabGroup("VFX", "Player Attacker")]
    [ShowIf(nameof(ShouldShowComplexVFXProperty))]
    [PropertyTooltip("VFX are optional. This effect will play on Player attackers")]
    public ParticleSystem attackVFXOnPlayerAttacker = null;
    [TabGroup("VFX", "Player Attacker")]
    [ShowIf(nameof(ShouldShowComplexVFXProperty))]
    [PropertyTooltip("VFX are optional. This effect will play on AI creatures damaged by the Player")]
    public ParticleSystem damageVFXIfPlayerAttacks = null;

    // IF AI ATTACKING
    [TabGroup("VFX", "AI Attacker")]
    [ShowIf(nameof(ShouldShowComplexVFXProperty))]
    [PropertyTooltip("VFX are optional. This effect will play on AI attackers")]
    public ParticleSystem attackVFXOnAIAttacker = null;
    [TabGroup("VFX", "AI Attacker")]
    [ShowIf(nameof(ShouldShowComplexVFXProperty))]
    [PropertyTooltip("VFX are optional. This effect will play on Player creatures damaged by AI")]
    public ParticleSystem damageVFXIfAIAttacks = null;

    // IF NOT DIFFERENT
    [TabGroup("VFX")]
    [ShowIf(nameof(ShouldShowVFXProperty))]
    [PropertyTooltip("VFX are optional. This effect will play on any attacker")]
    public ParticleSystem attackVFX;
    [TabGroup("VFX")]
    [ShowIf(nameof(ShouldShowVFXProperty))]
    [PropertyTooltip("VFX are optional. This effect will play on any creature damaged by this effect")]
    public ParticleSystem damageVFX;


    public void ApplyBonusAttackEffect(Health[] targets, bool isPlayer, Creature attacker)
    {
        Vector3 knockbackOrigin = attacker.transform.position;

        foreach (Health target in targets)
        {
            NavMeshAgent agent = target.GetComponent<NavMeshAgent>();

            agent.enabled = false;

            Vector3 knockbackForce = CalculateKnockbackForce(knockbackOrigin, target);

            Vector3 knockbackDestination = target.transform.position + knockbackForce;

            NavMeshHit hit;
            if (NavMesh.Raycast(target.transform.position, knockbackDestination, out hit, NavMesh.AllAreas))
            {
                Debug.Log("Target is off the navmesh");
                continue;
            }

            // Find closest valid position on the NavMesh for the knockback destination
            if (NavMesh.SamplePosition(knockbackDestination, out hit, 1f, NavMesh.AllAreas))
            {
                knockbackDestination = hit.position;
            }
            else if (NavMesh.FindClosestEdge(knockbackDestination, out hit, NavMesh.AllAreas))
            {
                Vector3 directionToEdge = hit.position - knockbackDestination;

                float knockbackDistance = directionToEdge.magnitude;
                Vector3 limitedKnockbackForce = directionToEdge.normalized * knockbackDistance;

                knockbackDestination = target.transform.position + limitedKnockbackForce;
            }

            //target.transform.Translate(knockbackDestination);
            target.transform.DOMove(knockbackDestination, 0.5f).SetEase(Ease.OutQuad).OnComplete(() => RestoreTarget(target));

            PlayDamageVFX(isPlayer, target);
        }

        PlayAttackVFX(isPlayer, attacker);
    }

    private void RestoreTarget(Health target)
    {
        NavMeshAgent agent = target.GetComponent<NavMeshAgent>();
        agent.enabled = true;
    }

    private Vector3 CalculateKnockbackForce(Vector3 knockbackOrigin, Health target)
    {
        Vector3 direction = target.transform.position - knockbackOrigin;
        direction.y = 0f;
        float distance = direction.magnitude;
        direction.Normalize();

        float knockbackEffect = 1f;

        if (applyDiminishingForce)
        {
            knockbackEffect = 1f - Mathf.Clamp01(distance / radius);  // Calculate knockback effect based on distance from origin
        }
        
        Vector3 knockbackForce = direction * power * knockbackEffect;
        
        return knockbackForce;
    }

    private void PlayDamageVFX(bool isPlayer, Health targetHealth)
    {
        if (!hasVFX) { return; }

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
        if (!hasVFX) { return; }

        if (playerAndAIAreDifferent)
        {
            if (attackVFXOnPlayerAttacker == null || attackVFXOnAIAttacker == null) { return; }

            // Plays a VFX based on whether the attacking creature is the Player or AI
            Instantiate((isPlayer) ? attackVFXOnPlayerAttacker : attackVFXOnAIAttacker,
                attacker.transform.position,
                Quaternion.identity);
        }
        else
        {
            if (attackVFX == null) { return; }

            Instantiate(attackVFX, attacker.transform.position, Quaternion.identity);
        }
    }

    #region Custom Inspector Methods

    private bool ShouldShowVFXProperty()
    {
        if (hasVFX)
        {
            if (playerAndAIAreDifferent)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        else
        {
            return false;
        }
    }

    private bool ShouldShowComplexVFXProperty()
    {
        if (hasVFX)
        {
            if (playerAndAIAreDifferent)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    #endregion
}
