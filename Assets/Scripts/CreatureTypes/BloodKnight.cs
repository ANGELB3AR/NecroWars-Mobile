using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BloodKnight : Creature
{
    [SerializeField] float bonusAttackRange;
    [PropertyRange(0,100)]
    [SerializeField] float percentageHealthToSteal;
    [SerializeField] ParticleSystem playerImpactEffect;
    [SerializeField] ParticleSystem aiImpactEffect;
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

                Instantiate((designatedHoard.isPlayer) ? playerImpactEffect : aiImpactEffect,
                    targetCreature.transform.position,
                    Quaternion.identity);
            }
        }

        health.Heal(healthStolen);
        Instantiate(healingEffect, transform.position, Quaternion.identity);
    }
}
