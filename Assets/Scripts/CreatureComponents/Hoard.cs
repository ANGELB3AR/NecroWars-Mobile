using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Sirenix.OdinInspector;

public class Hoard : MonoBehaviour
{
    public Transform hoardMovementTransform;

    [SerializeField] GameObject resurrectUI;
    [SerializeField] List<Creature> creaturesInHoard = new List<Creature>();
    [SerializeField] Vector2 mapBounds = new Vector2();
    [SerializeField] float randomMovementInterval;
    [SerializeField] GameObject creatureBasePrefab;

    private float timer;
    private bool canBeResurrected = false;
    private int creaturesAliveInHoard;

    public bool isPlayer = false;

    public event Action<Hoard> OnHoardDied;
    public event Action<Creature> OnCreatureAddedToHoard;

    public static event Action<int> OnPlayerHoardSizeUpdated;


    private void OnEnable()
    {
        foreach (Creature creature in creaturesInHoard)
        {
            creature.GetHealthComponent().OnCreatureDied += HandleCreatureDied;
        }
    }

    private void Start()
    {
        creaturesAliveInHoard = creaturesInHoard.Count;

        if (isPlayer)
        {
            OnPlayerHoardSizeUpdated?.Invoke(creaturesAliveInHoard);
        }
    }

    private void OnDisable()
    {
        foreach (Creature creature in creaturesInHoard)
        {
            creature.GetHealthComponent().OnCreatureDied -= HandleCreatureDied;
        }

        creaturesInHoard.Clear();
    }

    private void Update()
    {
        if (isPlayer) { return; }

        ProcessRandomMovementTimer();
    }

    public List<Creature> GetCreaturesInHoard()
    {
        return creaturesInHoard;
    }

    public void ResurrectHoard()
    {
        Hoard playerHoard = FindObjectOfType<PlayerController>().playerHoard;

        foreach (Creature creature in creaturesInHoard)
        {
            playerHoard.AddToHoard(creature);

            creature.GetHealthComponent().Resurrect();
        }

        creaturesInHoard.Clear();
        resurrectUI.SetActive(false);
        Destroy(gameObject);
    }

    public Creature CreateNewCreature(CreatureSO creatureConfig)
    {
        GameObject prefabInstance = Instantiate(creatureBasePrefab, transform.position, Quaternion.identity);

        Creature newCreature = prefabInstance.GetComponent<Creature>();

        AddToHoard(newCreature);

        newCreature.SetCreatureConfig(creatureConfig);

        return newCreature;
    }

    public void AddToHoard(Creature creature)
    {
        if (creature == null)
        {
            Debug.LogWarning("Attempted to add nullified creature to hoard");
        }

        creaturesInHoard.Add(creature);
        creaturesAliveInHoard++;

        if (isPlayer)
        {
            OnPlayerHoardSizeUpdated?.Invoke(creaturesAliveInHoard);
        }

        creature.GetHealthComponent().OnCreatureDied += HandleCreatureDied;
        creature.SetDesignatedHoard(this);

        OnCreatureAddedToHoard?.Invoke(creature);
    }

    private void ProcessRandomMovementTimer()
    {
        timer += Time.deltaTime;

        if (timer > randomMovementInterval)
        {
            MoveHoardToRandomLocation();
            timer = 0f;
        }
    }

    private void MoveHoardToRandomLocation()
    {
        hoardMovementTransform.position = GenerateNewRandomLocation();
    }

    private Vector3 GenerateNewRandomLocation()
    {
        Vector3 randomLocation = new Vector3();

        randomLocation.x = Random.Range(-mapBounds.x, mapBounds.x);
        randomLocation.z = Random.Range(-mapBounds.y, mapBounds.y);

        return randomLocation;
    }

    private void HandleCreatureDied(Creature creature)
    {
        creaturesAliveInHoard--;

        if (isPlayer)
        {
            OnPlayerHoardSizeUpdated?.Invoke(creaturesAliveInHoard);

            if (!creaturesInHoard.Contains(creature))
            {
                Debug.LogError($"{creature.name} was not found in hoard");
            }

            creature.GetHealthComponent().OnCreatureDied -= HandleCreatureDied;
            creature.SetDesignatedHoard(null);
            creature.gameObject.SetActive(false);
            creaturesInHoard.Remove(creature);
        }

        if (creaturesAliveInHoard == 0)
        {
            if (isPlayer)
            {
                GameManager.Instance.UpdateGameState(GameState.GameLost);
            }
            else
            {
                canBeResurrected = true;
                transform.position = CalculateCenterPosition();
                resurrectUI.SetActive(canBeResurrected);

                OnHoardDied?.Invoke(this);
            }
        }
    }

    private Vector3 CalculateCenterPosition()
    {
        Vector3 centerPosition = Vector3.zero;

        foreach (Creature creature in creaturesInHoard)
        {
            centerPosition += creature.transform.position;
        }

        centerPosition /= creaturesInHoard.Count;
        
        return centerPosition;
    }

    [Button]
    private void KillAllCreaturesInHoard()
    {
        foreach (Creature creature in creaturesInHoard)
        {
            creature.GetHealthComponent().TakeDamage(Mathf.Infinity);
        }
    }
}
