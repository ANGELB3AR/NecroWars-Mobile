using System;
using System.Globalization;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Zindeaxx.SoundSystem
{
    /// <summary>
    /// Draws a custom Inspector for InventoryItem assets
    /// Actually, we will draw the default inspector but use the ability to draw a custom preview and render a custom asset's thumbnail
    /// </summary>
    [CustomEditor(typeof(SoundOnDistance), true, isFallback = false)]
    public class SoundOnDistanceEditor : Editor
    {

        public override void OnInspectorGUI()
        {
            SerializedObject distanceEditor = new SerializedObject(target);
            distanceEditor.Update();

            EditorGUILayout.PropertyField(distanceEditor.FindProperty("AssignedSoundSet"));
            EditorGUILayout.PropertyField(distanceEditor.FindProperty("Distance"));
            EditorGUILayout.PropertyField(distanceEditor.FindProperty("PlayOnce"));
            EditorGUILayout.PropertyField(distanceEditor.FindProperty("Timeout"));
            EditorGUILayout.LabelField("Trigger Settings");
            var tagProp = distanceEditor.FindProperty("UseTag");
            EditorGUILayout.PropertyField(tagProp);
            if (tagProp.boolValue)
            {
                EditorGUILayout.PropertyField(distanceEditor.FindProperty("TriggerTag"));
            }
            else
            {
                EditorGUILayout.PropertyField(distanceEditor.FindProperty("TriggerTransform"));
            }

            distanceEditor.ApplyModifiedProperties();
        }


    }
}