using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;

public class Creature : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Health health;
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Animator animator;
    [SerializeField] Hoard designatedHoard;
    [Header("Movement")]
    [SerializeField] float rotationSpeed;
    [Header("Attacking")]
    [SerializeField] GameObject attackRaycastOrigin;
    [SerializeField] float attackCooldown;
    [Tooltip("Must be greater than Attack Range")]
    [SerializeField] float chaseRange;
    [Tooltip("Must be lesser than Chase Range")]
    [SerializeField] float attackRange;
    [SerializeField] float attackDamage;
    [SerializeField] LayerMask targetMask;    

    private float lastAttackTime;
    private Transform movementTarget;
    private bool isAttacking;
    private Creature targetCreature;
    readonly int movementSpeedHash = Animator.StringToHash("movementSpeed");
    readonly int attackHash = Animator.StringToHash("attack");

    private void Start()
    {
        movementTarget = designatedHoard.hoardMovementTransform;

        lastAttackTime = -attackCooldown;

        StartCoroutine(SearchRoutine());
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
    }

    public Health GetHealthComponent()
    {
        return health;
    }

    public void SetDesignatedHoard(Hoard hoard)
    {
        designatedHoard = hoard;
        movementTarget = designatedHoard.hoardMovementTransform;
    }

    // Adds a delay between searches for better performance and to give priority to movement controls in Update
    IEnumerator SearchRoutine()
    {
        float delay = 0.2f;
        WaitForSeconds wait = new WaitForSeconds(delay);

        while (true)
        {
            yield return wait;

            if (targetCreature == null)
            {
                SearchForOpposingCreatures();
            }
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

        foreach (Collider collider in colliders)
        {
            targetCreature = collider.GetComponent<Creature>();

            if (targetCreature.designatedHoard.isPlayer == designatedHoard.isPlayer) { continue; }    // If on the same team then move on to the next creature
            if (targetCreature.GetHealthComponent().IsDead()) { return; }   // If already dead then move on to the next creature

            ChaseTargetCreature();
            return;
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
            Attack();
            agent.ResetPath();  // Stop moving when attacking
            return;
        }

        agent.SetDestination(targetCreature.transform.position);
    }

    private void Attack()
    {
        RotateTowardTarget(targetCreature.transform);

        if (Time.time - lastAttackTime < attackCooldown) { return; }

        Collider[] hitColliders = Physics.OverlapCapsule(transform.position, transform.forward * attackRange, 1f, targetMask);

        foreach (Collider collider in hitColliders)
        {
            if (collider.gameObject.GetComponent<Creature>().designatedHoard.isPlayer != designatedHoard.isPlayer)
            {
                if (collider.TryGetComponent<Health>(out Health targetHealth))
                {
                    targetHealth.TakeDamage(attackDamage);

                    if (targetHealth.IsDead())
                    {
                        targetCreature = null;
                    }
                }
            }
        }

        animator.SetTrigger(attackHash);
        lastAttackTime = Time.time;    // Reset attack cooldown
    }

    private void RotateTowardTarget(Transform targetPosition)
    {
        Vector3 targetDirection = (targetPosition.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
