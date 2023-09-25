using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Knockback : IBonusAttackEffect
{
    [SerializeField] float power = 5f;
    [SerializeField] float radius = 3f;

    public void ApplyBonusAttackEffect(Health[] targets, bool isPlayer, Creature attacker)
    {
        Vector3 knockbackOrigin = attacker.transform.position;

        foreach (Health target in targets)
        {
            NavMeshAgent agent = target.GetComponent<NavMeshAgent>();

            agent.enabled = false;

            Vector3 direction = target.transform.position - knockbackOrigin;
            direction.y = 0f;
            float distance = direction.magnitude;
            float knockbackEffect = 1f - Mathf.Clamp01(distance / radius);  // Calculate knockback effect based on distance from origin
            direction.Normalize();
            Vector3 knockbackForce = direction * power * knockbackEffect;

            Vector3 knockbackDestination = target.transform.position + knockbackForce;
            target.transform.Translate(knockbackDestination);

            agent.enabled = true;
        }
    }
}
