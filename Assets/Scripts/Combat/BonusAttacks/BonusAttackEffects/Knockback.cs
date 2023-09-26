using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Sirenix.OdinInspector;

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



    public void ApplyBonusAttackEffect(Health[] targets, bool isPlayer, Creature attacker)
    {
        Vector3 knockbackOrigin = attacker.transform.position;

        foreach (Health target in targets)
        {
            NavMeshAgent agent = target.GetComponent<NavMeshAgent>();

            agent.enabled = false;

            Vector3 knockbackForce = CalculateKnockbackForce(knockbackOrigin, target);

            Vector3 knockbackDestination = target.transform.position + knockbackForce;
            target.transform.Translate(knockbackDestination);

            agent.enabled = true;
        }
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
}
