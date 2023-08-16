using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoard : MonoBehaviour
{
    public Transform hoardMovementTransform;

    [SerializeField] List<Creature> creaturesInHoard = new List<Creature>();
}
