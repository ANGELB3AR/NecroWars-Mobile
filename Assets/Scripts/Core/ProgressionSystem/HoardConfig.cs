using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "Hoard Config 00", menuName = "Level Data/New Hoard Config")]
public class HoardConfig : SerializedScriptableObject
{
    [DictionaryDrawerSettings(DisplayMode = DictionaryDisplayOptions.OneLine, KeyLabel = "Creature Type", ValueLabel = "Quantity")]
    public Dictionary<CreatureSO, int> hoardConfiguration = new Dictionary<CreatureSO, int>();
}
