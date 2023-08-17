using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Creature : MonoBehaviour
{
    [SerializeField] Health health;

    [SerializeField] float movementSpeed;
    [SerializeField] float attackCooldown;
    [SerializeField] float attackRange;
    [SerializeField] float attackDamage;
    [SerializeField] Transform movementTarget;
    [SerializeField] float stopDistance = 0.1f;

    public bool isResurrected;

    private float lastAttackTime;

    private void Start()
    {
        lastAttackTime = -attackCooldown;
    }

    private void Update()
    {
        MoveTowardTarget();
    }

    private void MoveTowardTarget()
    {
        if (Vector3.Distance(transform.position, movementTarget.position) < stopDistance) { return; }

        Vector3 direction = (movementTarget.position - transform.position).normalized;
        transform.Translate(direction * movementSpeed * Time.deltaTime);
    }
}
