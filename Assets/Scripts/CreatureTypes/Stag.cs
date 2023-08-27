using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stag : Creature
{
    public override void Attack()
    {
        Collider[] hitColliders = Physics.OverlapCapsule(transform.position, transform.forward * attackRange, 1f, targetMask);

        foreach (Collider collider in hitColliders)
        {
            if (collider.TryGetComponent<Creature>(out Creature targetCreature))
            {
                if (targetCreature.GetDesignatedHoard().isPlayer != designatedHoard.isPlayer)
                {
                    Health targetHealth = targetCreature.GetHealthComponent();

                    targetHealth.TakeDamage(attackDamage);

                    if (targetHealth.IsDead())
                    {
                        targetCreature = null;
                    }

                    return;
                }
            }
        }
    }
}
