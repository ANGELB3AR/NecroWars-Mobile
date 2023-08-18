using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] Health health;
    [SerializeField] Transform movementTarget;
    [Header("Movement")]
    [SerializeField] float movementSpeed;
    [SerializeField] float stopDistance = 0.1f;
    [Header("Attacking")]
    [SerializeField] float attackCooldown;
    [SerializeField] float attackRange;
    [SerializeField] float attackDamage;
    [Header("Health")]
    [SerializeField] bool isResurrected;

    private float lastAttackTime;

    private void Start()
    {
        lastAttackTime = -attackCooldown;
    }

    private void Update()
    {
        MoveTowardTarget();
        SearchForOpposingCreatures();
    }

    private void MoveTowardTarget()
    {
        if (Vector3.Distance(transform.position, movementTarget.position) < stopDistance) { return; }

        Vector3 direction = (movementTarget.position - transform.position).normalized;
        transform.Translate(direction * movementSpeed * Time.deltaTime);
    }

    private void SearchForOpposingCreatures()
    {
        
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
