using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Health : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] float maxHealth;
    [SerializeField] ParticleSystem impactParticleEffect;
    [SerializeField] bool isImpervious = false;

    [ProgressBar(0, 100, MaxGetter = nameof(maxHealth))]
    private float currentHealth;
    private bool isDead;
    private bool isResurrected;
    readonly int isDeadHash = Animator.StringToHash("isDead");
    readonly int movementSpeedHash = Animator.StringToHash("movementSpeed");

    public event Action<Creature> OnCreatureDied;
    public event Action OnCreatureResurrected;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public bool IsResurrected()
    {
        return isResurrected;
    }

    public void SetIsResurrected(bool status)
    {
        isResurrected = status;
    }

    private void Die()
    {
        isDead = true;
        animator.SetBool(isDeadHash, isDead);
        animator.SetFloat(movementSpeedHash, 0f);

        OnCreatureDied?.Invoke(this.GetComponent<Creature>());
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) { return; }
        if (isImpervious) { return; }

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        Instantiate(impactParticleEffect, transform.position, Quaternion.identity);

        if (currentHealth == 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        if (isDead) { return; }

        currentHealth += healAmount;
        Mathf.Clamp(currentHealth, 0f, maxHealth);
    }

    public void Resurrect()
    {
        if (isResurrected) { return; }

        isDead = false;
        animator.SetBool(isDeadHash, isDead);
        currentHealth = maxHealth;
        isResurrected = true;

        OnCreatureResurrected?.Invoke();
    }
}
