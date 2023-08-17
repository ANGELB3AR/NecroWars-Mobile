using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Hoard : MonoBehaviour
{
    public Transform hoardMovementTransform;

    [SerializeField] List<Creature> creaturesInHoard = new List<Creature>();
    [SerializeField] bool isPlayer = false;
    [SerializeField] Vector2 mapBounds = new Vector2();
    [SerializeField] float randomMovementInterval;

    private float timer;

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
}
