using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestKillCreature : MonoBehaviour
{
    [SerializeField] Creature creature;

    public void HurtCreature()
    {
        creature.GetHealthComponent().TakeDamage(25);
    }
}
