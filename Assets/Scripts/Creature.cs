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
        Vector3 direction = (movementTarget.position - transform.position).normalized;
        transform.Translate(direction * movementSpeed * Time.deltaTime);
    }
}
