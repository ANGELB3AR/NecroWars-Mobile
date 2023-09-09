using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Creature
{
    [SerializeField] ParticleSystem explosionEffect;
    [SerializeField] float damage;
    [SerializeField] float range;
    [Header("Regular Attack Modifications")]
    [SerializeField] Transform fireBreathOrigin;
    [SerializeField] ParticleSystem fireBreathEffect;

    private void OnEnable()
    {
        animationEventReceiver.OnAttackAnimationEvent += BreathFire;
    }

    private void OnDisable()
    {
        animationEventReceiver.OnAttackAnimationEvent -= BreathFire;
    }

    private void BreathFire()
    {
        Instantiate(fireBreathEffect, fireBreathOrigin.position, fireBreathOrigin.rotation);
    }

    public override void BonusAttack()
    {
        if (!bonusAttackReady) { return; }

        base.BonusAttack();

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, range);

        foreach (Collider collider in hitColliders)
        {
            if (collider.TryGetComponent<Creature>(out Creature targetCreature))
            {
                if (targetCreature.GetDesignatedHoard().isPlayer == designatedHoard.isPlayer) { continue; }

                Health targetHealth = targetCreature.GetHealthComponent();

                if (targetHealth.IsDead()) { continue; }

                targetHealth.TakeDamage(damage);
            }
        }

        Instantiate(explosionEffect, transform.position, Quaternion.identity);
    }
}
