using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;
    [SerializeField] bool isDead;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    private void Die()
    {
        isDead = true;
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) { return; }

        currentHealth -= damageAmount;
        Mathf.Clamp(currentHealth, 0f, maxHealth);

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
