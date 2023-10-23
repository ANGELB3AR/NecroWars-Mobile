using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Level Config 01", menuName = "Level Data/New Level Config")]
public class LevelConfig : SerializedScriptableObject
{
    [MinValue(0)]
    public int difficultyTier1Hoards = 0;
    [MinValue(0)]
    public int difficultyTier2Hoards = 0;
    [MinValue(0)]
    public int difficultyTier3Hoards = 0;
    [MinValue(0)]
    public int difficultyTier4Hoards = 0;
    [MinValue(0)]
    public int difficultyTier5Hoards = 0;
}
