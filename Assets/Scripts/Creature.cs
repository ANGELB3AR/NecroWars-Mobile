using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Health health;
    [SerializeField] Hoard designatedHoard;
    [Header("Movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] float stopDistance = 0.1f;
    [Header("Attacking")]
    [SerializeField] float attackCooldown;
    [Tooltip("Must be greater than Attack Range")]
    [SerializeField] float attackRange;
    [SerializeField] float attackDamage;
    [Header("Health")]
    [SerializeField] bool isResurrected;

    private float lastAttackTime;
    private Transform movementTarget;

    private void Start()
    {
        movementTarget = designatedHoard.hoardMovementTransform;

        lastAttackTime = -attackCooldown;

        StartCoroutine(SearchRoutine());
    }

    private void Update()
    {
        MoveTowardTarget();
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

    private void MoveTowardTarget()
    {
        if (Vector3.Distance(transform.position, movementTarget.position) < stopDistance) { return; }

        Vector3 direction = (movementTarget.position - transform.position).normalized;
        transform.Translate(direction * movementSpeed * Time.deltaTime);
    }

    private void SearchForOpposingCreatures()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, attackRange);

        foreach (Collider collider in colliders)
        {
            Creature targetCreature = collider.GetComponent<Creature>();

            if (targetCreature.designatedHoard.isPlayer == designatedHoard.isPlayer) { return; }    // If on the same team then do nothing

            Attack();
        }
    }

    private void Attack()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, attackRange))
        {
            Debug.DrawRay(transform.position, transform.forward, Color.red, 1f);
        }
    }
}
