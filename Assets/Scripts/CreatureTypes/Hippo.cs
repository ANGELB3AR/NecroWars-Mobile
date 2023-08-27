using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Hippo : Creature
{
    [Header("Bonus Attack")]
    [SerializeField] float bonusAttackRadius;
    [SerializeField] float bonusAttackPushForce;
    [SerializeField] ParticleSystem bonusAttackParticleEffect;

    public override void BonusAttack()
    {
        Instantiate(bonusAttackParticleEffect, transform.position, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(transform.position, bonusAttackRadius);

        foreach (Collider collider in colliders)
        {
            Creature creature = collider.GetComponent<Creature>();

            if (creature.GetDesignatedHoard().isPlayer == designatedHoard.isPlayer) { continue; }

            creature.GetHealthComponent().TakeDamage(attackDamage);

            NavMeshAgent creatureAgent = creature.GetNavMeshAgent();

            Vector3 pushDirection = (creature.transform.position - transform.position).normalized;
            Vector3 pushForceVector = pushDirection * bonusAttackPushForce;

            creatureAgent.Warp(creature.transform.position += pushForceVector);
        }

        base.BonusAttack();
    }
}
