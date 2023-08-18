using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Creature : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Health health;
    [SerializeField] Hoard designatedHoard;
    [Header("Movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float stopDistance = 0.1f;
    [Header("Attacking")]
    [SerializeField] float attackCooldown;
    [Tooltip("Must be greater than Attack Range")]
    [SerializeField] float chaseRange;
    [Tooltip("Must be lesser than Chase Range")]
    [SerializeField] float attackRange;
    [SerializeField] float attackDamage;
    [SerializeField] LayerMask targetMask;
    [Header("Health")]
    [SerializeField] bool isResurrected;

    private float lastAttackTime;
    private Transform movementTarget;
    private bool isAttacking;
    private Creature targetCreature;

    private void Start()
    {
        movementTarget = designatedHoard.hoardMovementTransform;

        lastAttackTime = -attackCooldown;

        StartCoroutine(SearchRoutine());
    }

    private void Update()
    {
        FollowHoard();

        if (!isAttacking) { return; }

        ChaseTargetCreature();
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
        if (Vector3.Distance(transform.position, movementTarget.position) < stopDistance) { return; }

        RotateTowardMovementDirection(movementTarget);

        Vector3 direction = (movementTarget.position - transform.position).normalized;
        transform.Translate(direction * movementSpeed * Time.deltaTime);
    }

    private void SearchForOpposingCreatures()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, chaseRange, targetMask);

        foreach (Collider collider in colliders)
        {
            targetCreature = collider.GetComponent<Creature>();

            if (targetCreature.designatedHoard.isPlayer == designatedHoard.isPlayer) { continue; }    // If on the same team then move on to the next creature

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
            if (Touchscreen.current.primaryTouch.press.isPressed) { return; }
        }

        RotateTowardMovementDirection(targetCreature.transform);

        if (Vector3.Distance(transform.position, targetCreature.transform.position) <= attackRange)
        {
            Attack();
            return; // Stop moving when attacking
        }

        Vector3 direction = (targetCreature.transform.position - transform.position).normalized;
        transform.Translate(direction * movementSpeed * Time.deltaTime);
    }

    private void Attack()
    {
        if (Time.time - lastAttackTime < attackCooldown) { return; }

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange))
        {
            Debug.DrawRay(transform.position, transform.forward, Color.red, 1f);

            if (TryGetComponent<Health>(out Health health))
            {
                health.TakeDamage(attackDamage);
            }
            Debug.Log(hit.transform.gameObject.name);
        }
        
        lastAttackTime = Time.time;    // Reset attack cooldown
    }

    private void RotateTowardMovementDirection(Transform targetPosition)
    {
        Vector3 targetDirection = (targetPosition.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(targetDirection, Vector3.up);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}
