using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using UnityEngine.UI;

public abstract class Creature : MonoBehaviour, IAttack, IBonusAttack
{
    [Header("Components")]
    [SerializeField] Health health;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] protected Animator animator;
    [SerializeField] AnimationEventReceiver animationEventReceiver;
    [SerializeField] Collider creatureCollider;
    [SerializeField] Material resurrectedMaterial;
    [ShowIf(nameof(hasBonusAttack))]
    [SerializeField] Outline bonusAttackOutline = null;
    [Header("Attacking")]
    [SerializeField] float attackCooldown;
    [Tooltip("Must be greater than Attack Range")]
    [SerializeField] float chaseRange;
    [Tooltip("Must be lesser than Chase Range")]
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackDamage;
    [SerializeField] protected LayerMask targetMask;
    [Header("Bonus Attack")]
    [SerializeField] protected bool hasBonusAttack;
    [ShowIf(nameof(hasBonusAttack))]
    [SerializeField] protected float bonusAttackChargeTime;
    [ShowIf(nameof(hasBonusAttack))]

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
        lastBonusAttackTime = bonusAttackChargeTime;

        if (bonusAttackOutline != null)
        {
            bonusAttackOutline.enabled = false;
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
    }

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
