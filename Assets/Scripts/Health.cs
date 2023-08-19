using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;
    [SerializeField] bool isDead;

    public event Action OnCreatureDied;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }

    private void Die()
    {
        isDead = true;

        OnCreatureDied?.Invoke();
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) { return; }

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

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
        isDead = false;
        currentHealth = maxHealth;
    }
}
