using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    [SerializeField] GameObject healthCanvas;
    [SerializeField] Slider healthSlider;
    [SerializeField] float maxHealth;
    [SerializeField] float currentHealth;
    [SerializeField] bool isDead;

    [SerializeField] private bool isResurrected;

    public event Action OnCreatureDied;

    private void Start()
    {
        currentHealth = maxHealth;
        healthSlider.maxValue = maxHealth;
        healthSlider.value = currentHealth;
    }

    public bool IsDead()
    {
        return isDead;
    }

    private void Die()
    {
        isDead = true;

        OnCreatureDied?.Invoke();

        healthCanvas.SetActive(!isDead);
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) { return; }

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        healthSlider.value = currentHealth;

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

        healthSlider.value = currentHealth;
    }

    public void Resurrect()
    {
        if (isResurrected) { return; }

        isDead = false;
        currentHealth = maxHealth;
        isResurrected = true;
        healthCanvas.SetActive(!isDead);
        healthSlider.value = currentHealth;
    }
}
