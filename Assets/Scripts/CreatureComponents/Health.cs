using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Health : MonoBehaviour
{
    Animator animator;
    Creature creature;

    [TabGroup("General Settings")] [DisallowModificationsIn(PrefabKind.Variant)]
    [SerializeField] ParticleSystem impactParticleEffect;
    [TabGroup("General Settings")]
    [ProgressBar(0,1000)] [DisallowModificationsIn(PrefabKind.Regular)]
    [SerializeField] float maxHealth;

    [TabGroup("Debug Options")] [PropertyTooltip("Disables creature from taking damage. Ensure this is disabled prior to building.")]
    [DisallowModificationsIn(PrefabKind.Regular)]
    [SerializeField] bool isImpervious = false;

    [PropertyOrder(-1)] [GUIColor(0,1,0,1)] [DisableInEditorMode] [HideInEditorMode]
    [ProgressBar(0, 100, MaxGetter = nameof(maxHealth))]
    [SerializeField] private float currentHealth;
    private bool isDead;
    private bool isResurrected;
    readonly int isDeadHash = Animator.StringToHash("isDead");
    readonly int movementSpeedHash = Animator.StringToHash("movementSpeed");

    public static event Action<float> OnPlayerCreatureHealthUpdated;
    public static event Action<float> OnPlayerHoardMaxHealthUpdated;
    
    public event Action<Creature> OnCreatureDied;
    public event Action OnCreatureResurrected;


    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        creature = GetComponent<Creature>();

        currentHealth = maxHealth;
    }

    #region Public Getters & Setters

    public bool IsDead()
    {
        return isDead;
    }

    public float GetCurrentHealth()
    {
        return currentHealth;
    }

    public float GetMaxHealth()
    {
        return maxHealth;
    }

    public bool IsResurrected()
    {
        return isResurrected;
    }

    public void SetIsResurrected(bool status)
    {
        isResurrected = status;
    }

    public void SetMaxHealth(float maxHealth)
    {
        this.maxHealth = maxHealth;
    }

    #endregion

    private void Die()
    {
        isDead = true;
        animator.SetBool(isDeadHash, isDead);
        animator.SetFloat(movementSpeedHash, 0f);

        OnCreatureDied?.Invoke(this.GetComponent<Creature>());

        if (creature.designatedHoard.isPlayer)
        {
            OnPlayerHoardMaxHealthUpdated?.Invoke(-maxHealth);
        }
    }

    public void TakeDamage(float damageAmount)
    {
        if (isDead) { return; }
        if (isImpervious) { return; }

        float originalHealth = currentHealth;

        currentHealth -= damageAmount;
        currentHealth = Mathf.Clamp(currentHealth, 0f, maxHealth);

        float deltaHealth = currentHealth - originalHealth;

        if (creature.designatedHoard.isPlayer)
        {
            OnPlayerCreatureHealthUpdated?.Invoke(deltaHealth);
        }

        Instantiate(impactParticleEffect, transform.position, Quaternion.identity);

        if (currentHealth == 0)
        {
            Die();
        }
    }

    public void Heal(float healAmount)
    {
        if (isDead) { return; }

        float originalHealth = currentHealth;

        currentHealth += healAmount;
        Mathf.Clamp(currentHealth, 0f, maxHealth);
       
        float deltaHealth = currentHealth - originalHealth;

        if (creature.designatedHoard.isPlayer)
        {
            OnPlayerCreatureHealthUpdated?.Invoke(deltaHealth);
        }
    }

    public void Resurrect()
    {
        if (isResurrected) { return; }

        isDead = false;
        animator.SetBool(isDeadHash, isDead);
        currentHealth = maxHealth;
        isResurrected = true;

        OnCreatureResurrected?.Invoke();
        OnPlayerHoardMaxHealthUpdated?.Invoke(maxHealth);
    }
}
