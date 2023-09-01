using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : Creature
{
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
}
