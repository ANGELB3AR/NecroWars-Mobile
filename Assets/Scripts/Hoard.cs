using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Hoard : MonoBehaviour
{
    public Transform hoardMovementTransform;

    [SerializeField] GameObject resurrectUI;
    [SerializeField] List<Creature> creaturesInHoard = new List<Creature>();
    [SerializeField] Vector2 mapBounds = new Vector2();
    [SerializeField] float randomMovementInterval;

    private float timer;
    private bool canBeResurrected = false;
    private int creaturesAliveInHoard;

    public bool isPlayer = false;


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
    }

    private void OnDisable()
    {
        foreach (Creature creature in creaturesInHoard)
        {
            creature.GetHealthComponent().OnCreatureDied += HandleCreatureDied;
        }
    }

    private void Update()
    {
        if (isPlayer) { return; }

        ProcessRandomMovementTimer();
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

    private void HandleCreatureDied()
    {
        creaturesAliveInHoard--;

        if (creaturesAliveInHoard == 0)
        {
            if (!isPlayer)
            {
                canBeResurrected = true;
                transform.position = CalculateCenterPosition();
                resurrectUI.SetActive(canBeResurrected);
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
}
