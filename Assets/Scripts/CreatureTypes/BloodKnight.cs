using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class BloodKnight : Creature
{
    [TabGroup("Combat", "Bonus Attack")]
    [ShowIf(nameof(hasBonusAttack))]
    [SerializeField] float bonusAttackRange;
    [TabGroup("Combat", "Bonus Attack")]
    [ShowIf(nameof(hasBonusAttack))]
    [ProgressBar(0,100)]
    [SerializeField] float percentageHealthToSteal;
    [TabGroup("Combat", "Bonus Attack")]
    [ShowIf(nameof(hasBonusAttack))]
    [SerializeField] ParticleSystem playerImpactEffect;
    [TabGroup("Combat", "Bonus Attack")]
    [ShowIf(nameof(hasBonusAttack))]
    [SerializeField] ParticleSystem aiImpactEffect;
    [TabGroup("Combat", "Bonus Attack")]
    [ShowIf(nameof(hasBonusAttack))]
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
