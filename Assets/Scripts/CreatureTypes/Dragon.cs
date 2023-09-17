using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Dragon : Creature
{
    [TabGroup("Combat", "Bonus Attack")]
    [SerializeField] ParticleSystem explosionEffect;
    [TabGroup("Combat", "Bonus Attack")]
    [SerializeField] float damage;
    [TabGroup("Combat", "Bonus Attack")]
    [SerializeField] float range;
    [TabGroup("Combat", "Normal Attack")]
    [SerializeField] Transform fireBreathOrigin;
    [TabGroup("Combat", "Normal Attack")]
    [SerializeField] ParticleSystem fireBreathEffect;

    private void OnEnable()
    {
        animationEventReceiver.OnAttackAnimationEvent += BreathFire;
        health.OnCreatureResurrected += Health_OnCreatureResurrected;
    }

    private void OnDisable()
    {
        animationEventReceiver.OnAttackAnimationEvent -= BreathFire;
        health.OnCreatureResurrected -= Health_OnCreatureResurrected;
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

    private void Health_OnCreatureResurrected()
    {
        bonusAttackOutline.OutlineColor = (designatedHoard.isPlayer) ? Color.white : Color.red;

        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderer.material = resurrectedMaterial;
        }
    }
}
