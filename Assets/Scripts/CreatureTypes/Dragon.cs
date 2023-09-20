using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Dragon : Creature
{
    //[TabGroup("Combat", "Normal Attack")]
    //[SerializeField] Transform fireBreathOrigin;
    //[TabGroup("Combat", "Normal Attack")]
    //[SerializeField] ParticleSystem fireBreathEffect;

    //private void Awake()
    //{
    //    fireBreathOrigin = GetComponentInChildren<Transform>();
    //}

    //private void OnEnable()
    //{
    //    animationEventReceiver.OnAttackAnimationEvent += BreathFire;
    //    health.OnCreatureResurrected += Health_OnCreatureResurrected;
    //}

    //private void OnDisable()
    //{
    //    animationEventReceiver.OnAttackAnimationEvent -= BreathFire;
    //    health.OnCreatureResurrected -= Health_OnCreatureResurrected;
    //}

    //private void BreathFire()
    //{
    //    Instantiate(fireBreathEffect, fireBreathOrigin.position, fireBreathOrigin.rotation);
    //}

    //private void Health_OnCreatureResurrected()
    //{
    //    bonusAttackOutline.OutlineColor = (designatedHoard.isPlayer) ? Color.white : Color.red;

    //    Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

    //    foreach (Renderer renderer in renderers)
    //    {
    //        renderer.material = resurrectedMaterial;
    //    }
    //}
}
