using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using Sirenix.OdinInspector;

public abstract class Creature : MonoBehaviour, IAttack
{
    [Header("Components")]
    [SerializeField] Health health;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] protected Hoard designatedHoard;
    [SerializeField] AnimationEventReceiver animationEventReceiver;
    [Header("Movement")]
    [SerializeField] float rotationSpeed;
    [Header("Attacking")]
    [SerializeField] GameObject attackRaycastOrigin;
    [SerializeField] float attackCooldown;
    [Tooltip("Must be greater than Attack Range")]
    [SerializeField] float chaseRange;
    [Tooltip("Must be lesser than Chase Range")]
    [SerializeField] protected float attackRange;
    [SerializeField] protected float attackDamage;
    [SerializeField] protected LayerMask targetMask;    

    private float lastAttackTime;
    private Transform movementTarget;
    private bool isAttacking;
    [ShowInInspector] protected Creature targetCreature;
    readonly int movementSpeedHash = Animator.StringToHash("movementSpeed");
    readonly int attackHash = Animator.StringToHash("attack");

    private void OnEnable()
    {
        animationEventReceiver.OnAttackAnimationEvent += Attack;
        health.OnCreatureDied += HandleCreatureDied;
    }

    private void Start()
    {
        movementTarget = designatedHoard.hoardMovementTransform;

        lastAttackTime = -attackCooldown;

        StartCoroutine(SearchRoutine());
    }

    private void OnDisable()
    {
        animationEventReceiver.OnAttackAnimationEvent -= Attack;
        health.OnCreatureDied -= HandleCreatureDied;
    }

    private void Update()
    {
        DebugCheck();

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
    }

    private void DebugCheck()
    {
        if (GetHealthComponent().IsDead() && designatedHoard.isPlayer)
        {
            Debug.LogError($"{gameObject.name} should be disabled");
        }
    }

    public Health GetHealthComponent()
    {
        return health;
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
    }


    private void OnDrawGizmos()
    {
        if (GetHealthComponent().IsDead()) { return;}

        Vector3 endPoint = transform.position + transform.forward * attackRange;
        
        if (designatedHoard.isPlayer)
        {
            Gizmos.color = Color.blue;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireSphere(endPoint, attackRange);
    }

    public abstract void Attack();

}
