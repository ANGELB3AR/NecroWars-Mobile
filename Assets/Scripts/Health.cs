using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class Health : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] GameObject healthCanvas;
    [SerializeField] Slider healthSlider;
    [SerializeField] float maxHealth;
    [ProgressBar(0, 100, MaxGetter = nameof(maxHealth))]
    [SerializeField] float currentHealth;
    [SerializeField] bool isDead;

    [SerializeField] private bool isResurrected;
    readonly int isDeadHash = Animator.StringToHash("isDead");
    readonly int movementSpeedHash = Animator.StringToHash("movementSpeed");

    public event Action<Creature> OnCreatureDied;
    public event Action OnCreatureResurrected;

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

        //healthCanvas.SetActive(!isDead);
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
        animator.SetBool(isDeadHash, isDead);
        currentHealth = maxHealth;
        isResurrected = true;
        //healthCanvas.SetActive(!isDead);
        healthSlider.value = currentHealth;

        OnCreatureResurrected?.Invoke();
    }
}
