using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Knockback : IBonusAttackEffect
{
    [SerializeField] bool applyDiminishingForce = false;
    [SerializeField] float power = 5f;
    [SerializeField] float radius = 3f;

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
