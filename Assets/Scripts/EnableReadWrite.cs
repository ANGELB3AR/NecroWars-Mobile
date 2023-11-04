using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class EnableReadWrite : MonoBehaviour
{
    void Start()
    {
        foreach (var mesh in Resources.FindObjectsOfTypeAll<Mesh>())
        {
            ModelImporter modelImporter = AssetImporter.GetAtPath(AssetDatabase.GetAssetPath(mesh)) as ModelImporter;
            if (modelImporter != null && !modelImporter.isReadable)
            {
                modelImporter.isReadable = true;
                modelImporter.SaveAndReimport();
            }
        }
    }
}
