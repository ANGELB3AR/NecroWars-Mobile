using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public abstract class Creature : MonoBehaviour, IBonusAttack
{
    [TabGroup("General", "Components")]
    [SerializeField] protected Health health;
    [TabGroup("General", "Components")]
    [SerializeField] private NavMeshAgent agent;
    [TabGroup("General", "Components")]
    [SerializeField] protected Animator animator;
    [TabGroup("General", "Components")]
    [SerializeField] protected AnimationEventReceiver animationEventReceiver;
    [TabGroup("General", "Components")]
    [SerializeField] private Collider creatureCollider;

    [TabGroup("General", "Settings")]
    [PreviewField]
    [SerializeField] protected Material resurrectedMaterial;

    [TabGroup("Combat", "Normal Attack")]
    [SerializeField] private float attackCooldown;
    private float chaseRange = 5f;
    [TabGroup("Combat", "Normal Attack")]
    [ProgressBar(0.1f, 4.9f)]
    [SerializeField] protected float attackRange;
    [TabGroup("Combat", "Normal Attack")]
    [ProgressBar(0, 300)]
    [SerializeField] protected float attackDamage;
    
    [TabGroup("General", "Settings")]
    [PropertyTooltip("This should ALWAYS be set to Creatures")]
    [SerializeField] protected LayerMask targetMask;

    [TabGroup("Combat", "Bonus Attack")]
    [PropertyOrder(-1)]
    [SerializeField] protected bool hasBonusAttack;
    [TabGroup("Combat", "Bonus Attack")]
    [ShowIf(nameof(hasBonusAttack))]
    [SerializeField] protected float bonusAttackChargeTime;
    [TabGroup("Combat", "Bonus Attack")]
    [ShowIf(nameof(hasBonusAttack))]
    [SerializeField] protected Outline bonusAttackOutline = null;

    protected Hoard designatedHoard;
    private float lastAttackTime;
    private Transform movementTarget;
    private bool isAttacking;
    protected Creature targetCreature;
    protected bool bonusAttackReady = false;
    protected float lastBonusAttackTime;
    private float rotationSpeed = 500f;

    readonly int movementSpeedHash = Animator.StringToHash("movementSpeed");
    readonly int attackHash = Animator.StringToHash("attack");
    protected readonly int bonusAttackHash = Animator.StringToHash("bonusAttack");

    private void OnEnable()
    {
        animationEventReceiver.OnAttackAnimationEvent += Attack;
        health.OnCreatureDied += HandleCreatureDied;
        health.OnCreatureResurrected += HandleCreatureResurrected;
    }

    private void Start()
    {
        movementTarget = designatedHoard.hoardMovementTransform;

        lastAttackTime = -attackCooldown;
        lastBonusAttackTime = Time.time;

        if (bonusAttackOutline != null)
        {
            bonusAttackOutline.enabled = false;
            bonusAttackOutline.OutlineColor = (designatedHoard.isPlayer) ? Color.white : Color.red;
        }

        StartCoroutine(SearchRoutine());

        if (health.IsResurrected())
        {
            gameObject.GetComponentInChildren<Renderer>().material = resurrectedMaterial;
        }
    }

    private void OnDisable()
    {
        animationEventReceiver.OnAttackAnimationEvent -= Attack;
        health.OnCreatureDied -= HandleCreatureDied;
        health.OnCreatureResurrected -= HandleCreatureResurrected;
    }

    private void Update()
    {
        if (health.IsDead()) { return; }

        animator.SetFloat(movementSpeedHash, agent.velocity.magnitude);

        if (isAttacking)
        {
            ChaseTargetCreature();
        }
        else
        {
            FollowHoard();
        }

        if (!hasBonusAttack) { return; }

        bonusAttackReady = Time.time - lastBonusAttackTime > bonusAttackChargeTime;
        bonusAttackOutline.enabled = bonusAttackReady;

        AITriggerBonusAttack();
    }

    private void AITriggerBonusAttack()
    {
        if (!hasBonusAttack) { return; }
        if (designatedHoard.isPlayer) { return; }
        if (targetCreature == null) { return; }
        if (!bonusAttackReady) { return; }

        BonusAttack();
    }

    #region Public Getters & Setters

    public Health GetHealthComponent()
    {
        return health;
    }

    public Hoard GetDesignatedHoard()
    {
        return designatedHoard;
    }

    public Material GetResurrectionMaterial()
    {
        return resurrectedMaterial;
    }

    public NavMeshAgent GetNavMeshAgent()
    {
        return agent;
    }

    public void SetDesignatedHoard(Hoard hoard)
    {
        designatedHoard = hoard;
        targetCreature = null;
        
        if (hoard != null)
        {
            movementTarget = designatedHoard.hoardMovementTransform;
        }
    }

    #endregion

    // Adds a delay between searches for better performance and to give priority to movement controls in Update
    IEnumerator SearchRoutine()
    {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;

            SearchForOpposingCreatures();
        }
    }

    private void FollowHoard()
    {
        targetCreature = null;

        agent.SetDestination(movementTarget.position);
    }

    private void SearchForOpposingCreatures()
    {
        if (health.IsDead()) { return; }

        Collider[] colliders = Physics.OverlapSphere(transform.position, chaseRange, targetMask);
        bool foundTarget = false;

        foreach (Collider collider in colliders)
        {
            targetCreature = collider.GetComponent<Creature>();

            if (targetCreature.designatedHoard.isPlayer == designatedHoard.isPlayer) { continue; }    // If on the same team then move on to the next creature
            if (targetCreature.GetHealthComponent().IsDead()) { continue; }   // If already dead then move on to the next creature

            foundTarget = true;
            ChaseTargetCreature();
            return;
        }

        if (!foundTarget)
        {
            targetCreature = null;
        }

        isAttacking = false;
    }

    private void ChaseTargetCreature()
    {
        if (health.IsDead()) { return; }

        isAttacking = true;

        if (designatedHoard.isPlayer)
        {
            if (Touchscreen.current.primaryTouch.press.isPressed) 
            {
                agent.SetDestination(movementTarget.position);
                return; 
            }
        }

        if (targetCreature == null)
        {
            isAttacking = false;
            return;
        }

        if (Vector3.Distance(transform.position, targetCreature.transform.position) <= attackRange)
        {
            if (targetCreature == null) { return; }

            RotateTowardTarget(targetCreature.transform);

            if (Time.time - lastAttackTime < attackCooldown) { return; }

            animator.SetTrigger(attackHash);
            agent.ResetPath();  // Stop moving when attacking

            lastAttackTime = Time.time;    // Reset attack cooldown

            return;
        }

        agent.SetDestination(targetCreature.transform.position);
    }


    private void RotateTowardTarget(Transform targetPosition)
    {
        Vector3 targetDirection = (targetPosition.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }

    private void HandleCreatureDied(Creature creature)
    {
        targetCreature = null;
        creatureCollider.enabled = false;
    }

    private void HandleCreatureResurrected()
    {
        creatureCollider.enabled = true;
        ChangeMaterial(resurrectedMaterial);

        if (bonusAttackOutline != null)
        {
            bonusAttackOutline.OutlineColor = (designatedHoard.isPlayer) ? Color.white : Color.red;
        }
    }

    public void ChangeMaterial(Material newMaterial)
    {
        Renderer[] renderers = gameObject.GetComponentsInChildren<Renderer>();

        foreach (Renderer renderer in renderers)
        {
            renderer.material = newMaterial;
        }
    }

    public void Attack()
    {
        if (health.IsDead()) { return; }

        Collider[] hitColliders = Physics.OverlapCapsule(transform.position, transform.forward * attackRange, 1f, targetMask);
        
        foreach (Collider collider in hitColliders)
        {
            if (collider.TryGetComponent<Creature>(out Creature targetCreature))
            {
                if (targetCreature.GetDesignatedHoard().isPlayer != designatedHoard.isPlayer)
                {
                    Health targetHealth = targetCreature.GetHealthComponent();

                    targetHealth.TakeDamage(attackDamage);

                    if (targetHealth.IsDead())
                    {
                        targetCreature = null;
                    }

                    return;
                }
            }
        }
    }

    public virtual void BonusAttack()
    {
        animator.SetTrigger(bonusAttackHash);

        lastBonusAttackTime = Time.time;
    }
}
