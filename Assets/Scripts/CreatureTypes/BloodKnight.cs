using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BloodKnight : Creature
{
    [SerializeField] float bonusAttackRange;
    [PropertyRange(0,100)]
    [SerializeField] float percentageHealthToSteal;
    [SerializeField] ParticleSystem impactEffect;
    [SerializeField] ParticleSystem healingEffect;

    public override void BonusAttack()
    {
        if (!bonusAttackReady) { return; }

        base.BonusAttack();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, bonusAttackRange);
        float healthStolen = 0f;

        foreach (Collider collider in hitColliders)
        {
            if (collider.TryGetComponent<Creature>(out Creature targetCreature))
            {
                if (targetCreature.GetDesignatedHoard().isPlayer == designatedHoard.isPlayer) { continue; }

                Health targetHealth = targetCreature.GetHealthComponent();

                if (targetHealth.IsDead()) { continue; }

                float healthToSteal = targetHealth.GetCurrentHealth() * (percentageHealthToSteal / 100);
                targetHealth.TakeDamage(healthToSteal);
                healthStolen += healthToSteal;

                Instantiate(impactEffect, targetCreature.transform.position, Quaternion.identity);
            }
        }

        health.Heal(healthStolen);
        Instantiate(healingEffect, transform.position, Quaternion.identity);

        Debug.Log($"Blood Knight stole {healthStolen} health from your enemies!");
    }
}
