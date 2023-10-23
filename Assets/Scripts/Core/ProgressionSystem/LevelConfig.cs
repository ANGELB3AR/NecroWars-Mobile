using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Level Config 01", menuName = "Level Data/New Level Config")]
public class LevelConfig : SerializedScriptableObject
{
    [MinValue(0)]
    public int[] difficultyTierHoards = new int[5];
}
