using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.AI;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using Zindeaxx.SoundSystem;

public class Creature : MonoBehaviour, IBonusAttack
{
    private Health health;
    private NavMeshAgent agent;
    private Animator animator;
    private AnimationEventReceiver animationEventReceiver;
    private Collider creatureCollider;
    private SoundManager soundManager;

    private string creatureName;

    private Material resurrectedMaterial;

    private float attackCooldown;
    private float chaseRange = 5f;
    private float attackRange;
    private float attackDamage;
    private Transform normalAttackVFXTransform;
    private ParticleSystem normalAttackVFX = null;
    
    private LayerMask targetMask;

    private bool hasBonusAttack;
    private float bonusAttackChargeTime;
    private Outline bonusAttackOutline = null;

    [InlineEditor]
    [SerializeField] CreatureSO creatureConfig = null;
    
    BonusAttackSO bonusAttackConfig = null;

    public Hoard designatedHoard { get; private set; }
    private float lastAttackTime;
    private Transform movementTarget;
    private bool isAttacking;
    private Creature targetCreature;
    private bool bonusAttackReady = false;
    private float lastBonusAttackTime;
    private float rotationSpeed = 500f;

    readonly int movementSpeedHash = Animator.StringToHash("movementSpeed");
    readonly int attackHash = Animator.StringToHash("attack");
    private readonly int bonusAttackHash = Animator.StringToHash("bonusAttack");
    readonly string normalAttackVFXOriginKey = "NormalAttackVFXOrigin";

    private void Awake()
    {
        health = GetComponent<Health>();
        agent = GetComponent<NavMeshAgent>();
        creatureCollider = GetComponent<Collider>();
        soundManager = GetComponent<SoundManager>();

        targetMask = LayerMask.GetMask("Creatures");

        normalAttackVFXTransform = transform.Find(normalAttackVFXOriginKey);
    }

    private void OnEnable()
    {
        health.OnCreatureDied += HandleCreatureDied;
        health.OnCreatureResurrected += HandleCreatureResurrected;
    }

    private void Start()
    {
        lastAttackTime = -attackCooldown;
        lastBonusAttackTime = Time.time;

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
        if (animator == null) { return; }

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

    public void SetCreatureConfig(CreatureSO creatureConfig)
    {
        this.creatureConfig = creatureConfig;
        InitializeConfig();
    }

    #endregion

    private void InitializeConfig()
    {
        if (creatureConfig == null)
        {
            Debug.LogWarning("Creature config not found");
            return;
        }

        creatureName = creatureConfig.creatureName;
        resurrectedMaterial = creatureConfig.resurrectedMaterial;
        attackCooldown = creatureConfig.attackCooldown;
        attackRange = creatureConfig.attackRange;
        attackDamage = creatureConfig.attackDamage;
        hasBonusAttack = creatureConfig.hasBonusAttack;
        bonusAttackChargeTime = creatureConfig.bonusAttackChargeTime;
        bonusAttackConfig = creatureConfig.bonusAttackConfig;

        if (creatureConfig.hasAttackVFX)
        {
            normalAttackVFXTransform.position = creatureConfig.attackVFXPosition;
            normalAttackVFXTransform.rotation = Quaternion.Euler(creatureConfig.attackVFXRotation);
            normalAttackVFX = creatureConfig.normalAttackVFX;
        }

        health.SetMaxHealth(creatureConfig.maxHealth);
        agent.stoppingDistance = creatureConfig.stoppingDistance;

        Instantiate(creatureConfig.creatureModel, this.gameObject.transform);

        if (creatureConfig.hasBonusAttack)
        {
            bonusAttackOutline = gameObject.AddComponent<Outline>();
            bonusAttackOutline.enabled = false;
            bonusAttackOutline.OutlineWidth = 4f;
            bonusAttackOutline.OutlineColor = (designatedHoard.isPlayer) ? Color.white : Color.red;
        }
        
        animator = GetComponentInChildren<Animator>();
        animationEventReceiver = GetComponentInChildren<AnimationEventReceiver>();

        animationEventReceiver.OnAttackAnimationEvent += Attack;
    }

    private void AITriggerBonusAttack()
    {
        if (!hasBonusAttack) { return; }
        if (designatedHoard.isPlayer) { return; }
        if (targetCreature == null) { return; }
        if (!bonusAttackReady) { return; }

        BonusAttack();
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

        if (creatureConfig.hasAttackVFX)
        {
            if (normalAttackVFX == null)
            {
                Debug.LogError($"{creatureName} was given a normal attack VFX but a particle effect was never assigned");
            }

            Instantiate(normalAttackVFX, normalAttackVFXTransform.position, normalAttackVFXTransform.rotation);
        }

        if (creatureConfig.attackSFX != null)
        {
            soundManager.PlaySound(creatureConfig.attackSFX);
        }
    }

    public void BonusAttack()
    {
        if (!bonusAttackReady) { return; }

        animator.SetTrigger(bonusAttackHash);

        lastBonusAttackTime = Time.time;

        List<Health> targetCreatures = new List<Health>();

        // Use switch statement to get all targets from AimType
        switch (bonusAttackConfig.aimType)
        {
            case AimType.Line_FirstHit:
                Collider[] hitColliders = Physics.OverlapCapsule(transform.position, 
                    transform.forward * bonusAttackConfig.range, 
                    1f, 
                    targetMask);

                foreach (Collider collider in hitColliders)
                {
                    if (collider.TryGetComponent<Creature>(out Creature targetCreature))
                    {
                        if (targetCreature.GetDesignatedHoard().isPlayer != designatedHoard.isPlayer)
                        {
                            Health targetHealth = targetCreature.GetHealthComponent();
                            targetCreatures.Add(targetHealth);

                            return;
                        }
                    }
                }
                break;
            case AimType.Line_HitAll:
                Collider[] hitColliders1 = Physics.OverlapCapsule(transform.position, 
                    transform.forward * bonusAttackConfig.range, 
                    1f, 
                    targetMask);

                foreach (Collider collider in hitColliders1)
                {
                    if (collider.TryGetComponent<Creature>(out Creature targetCreature))
                    {
                        if (targetCreature.GetDesignatedHoard().isPlayer != designatedHoard.isPlayer)
                        {
                            Health targetHealth = targetCreature.GetHealthComponent();
                            targetCreatures.Add(targetHealth);
                        }
                    }
                }
                
                break;
            case AimType.SurroundingArea:
                Collider[] hitColliders2 = Physics.OverlapSphere(transform.position, 
                    bonusAttackConfig.range, 
                    targetMask);

                foreach (Collider collider in hitColliders2)
                {
                    if (collider.TryGetComponent<Creature>(out Creature targetCreature))
                    {
                        if (targetCreature.GetDesignatedHoard().isPlayer == designatedHoard.isPlayer) { continue; }

                        Health targetHealth = targetCreature.GetHealthComponent();

                        if (targetHealth.IsDead()) { continue; }

                        targetCreatures.Add(targetHealth);
                    }
                }
                
                break;
            default:
                break;
        }

        // Play VFX
        foreach (IBonusAttackEffect bonusAttackEffect in creatureConfig.bonusAttackConfig.bonusAttackEffects)
        {
            bonusAttackEffect.ApplyBonusAttackEffect(targetCreatures.ToArray(), designatedHoard.isPlayer, this);
        }

        // Play SFX
        if (bonusAttackConfig.bonusAttackSFX != null)
        {
            soundManager.PlaySound(bonusAttackConfig.bonusAttackSFX);
        }
    }
}
