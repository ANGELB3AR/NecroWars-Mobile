using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Level 00 Config", menuName = "Level Data/New Level Config")]
public class LevelConfig : SerializedScriptableObject
{
    public int difficultyTier1Hoards = 0;
}
