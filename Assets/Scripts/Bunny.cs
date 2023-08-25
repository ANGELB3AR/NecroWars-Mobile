using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : Creature
{
    public override void Attack()
    {
        //if (targetCreature == null) { return; }

        //RotateTowardTarget(targetCreature.transform);

        //if (Time.time - lastAttackTime < attackCooldown) { return; }

        //Collider[] hitColliders = Physics.OverlapCapsule(transform.position, transform.forward * attackRange, 1f, targetMask);

        //foreach (Collider collider in hitColliders)
        //{
        //    if (collider.gameObject.GetComponent<Creature>().designatedHoard.isPlayer != designatedHoard.isPlayer)
        //    {
        //        if (collider.TryGetComponent<Health>(out Health targetHealth))
        //        {
        //            targetHealth.TakeDamage(attackDamage);

        //            if (targetHealth.IsDead())
        //            {
        //                targetCreature = null;
        //            }
        //        }
        //    }
        //}

        //lastAttackTime = Time.time;    // Reset attack cooldown
    }
}
