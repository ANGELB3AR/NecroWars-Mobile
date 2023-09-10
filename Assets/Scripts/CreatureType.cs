using UnityEngine;

[CreateAssetMenu(fileName = "NewCreatureType", menuName = "Creature Type")]
public class CreatureType : ScriptableObject
{
    public AnimationCurve WeightedScore;
    public GameObject Prefab;
}
