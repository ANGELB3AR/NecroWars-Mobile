using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : Creature
{
    public override void Attack()
    {

        Collider[] hitColliders = Physics.OverlapCapsule(transform.position, transform.forward * attackRange, 1f, targetMask);

        foreach (Collider collider in hitColliders)
        {
            Bunny bunny = collider.GetComponent<Bunny>();

            if (bunny.designatedHoard.isPlayer != designatedHoard.isPlayer)
            {
                if (collider.TryGetComponent<Health>(out Health targetHealth))
                {
                    targetHealth.TakeDamage(attackDamage);

                    if (targetHealth.IsDead())
                    {
                        targetCreature = null;
                    }
                }
            }
        }
    }
}
