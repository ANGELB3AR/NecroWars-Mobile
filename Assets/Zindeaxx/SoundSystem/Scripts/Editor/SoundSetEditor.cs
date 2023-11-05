using System;
using System.Collections.Generic;
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
    [CustomEditor(typeof(SoundSet), true, isFallback = false)]
    public class SoundSetEditor : Editor
    {
        private SerializedProperty volumeAmountProperty;
        private SerializedProperty pitchProperty;
        private SerializedProperty spatialBlendProperty;
        private SerializedProperty priorityProperty;
        private SerializedProperty volumeIDProperty;
        private SerializedProperty mixerProperty;
        private SerializedProperty minDistanceProperty;
        private SerializedProperty maxDistanceProperty;
        private SerializedProperty loopedSoundsProperty;
        private SerializedProperty spatializeProperty;
        private SerializedProperty spatializePostEffectsProperty;
        private SerializedProperty clipsProperty;

        /// <summary>
        /// Tells if the Object has a custom preview
        /// </summary>
        public override bool HasPreviewGUI()
        {
            return false;
        }

        public override VisualElement CreateInspectorGUI()
        {
            return base.CreateInspectorGUI();
        }

        void RegisterProperties ()
        {

            volumeAmountProperty = serializedObject.FindProperty("VolumeAmount");
            pitchProperty = serializedObject.FindProperty("Pitch");
            spatialBlendProperty = serializedObject.FindProperty("SpatialBlend");
            priorityProperty = serializedObject.FindProperty("Priority");
            volumeIDProperty = serializedObject.FindProperty("VolumeID");
            mixerProperty = serializedObject.FindProperty("MixerGroup");
            minDistanceProperty = serializedObject.FindProperty("MinDistance");
            maxDistanceProperty = serializedObject.FindProperty("MaxDistance");
            loopedSoundsProperty = serializedObject.FindProperty("LoopedSounds");
            spatializeProperty = serializedObject.FindProperty("Spatialize");
            spatializePostEffectsProperty = serializedObject.FindProperty("SpatializePostEffects");
            clipsProperty = serializedObject.FindProperty("m_Clips");
        }

        bool m_Setup = false;
        public override void OnInspectorGUI()
        {
            if(!m_Setup) {
                RegisterProperties();
            }


            serializedObject.Update();

            SoundSet sounds = (SoundSet)target;

            EditorGUILayout.LabelField("Sound Settings", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(volumeAmountProperty);
            EditorGUILayout.PropertyField(mixerProperty);
            EditorGUILayout.PropertyField(pitchProperty);
            EditorGUILayout.PropertyField(spatialBlendProperty);
            EditorGUILayout.PropertyField(priorityProperty);
            EditorGUILayout.PropertyField(volumeIDProperty, new GUIContent("Volume Identifier"));

            if (spatialBlendProperty.floatValue > 0)
            {
                EditorGUILayout.PropertyField(minDistanceProperty, new GUIContent("Minimum Distance: "));
                EditorGUILayout.PropertyField(maxDistanceProperty, new GUIContent("Maximum Distance: "));
            }

            EditorGUILayout.PropertyField(loopedSoundsProperty);
            EditorGUILayout.PropertyField(spatializeProperty);

            if (spatializeProperty.boolValue)
            {
                EditorGUILayout.PropertyField(spatializePostEffectsProperty);
            }
            EditorGUILayout.PropertyField(clipsProperty);

            serializedObject.ApplyModifiedProperties();
        }

        public float GetFloatValueFromString(string s)
        {
            s = s.Replace(',', '.');
            if (float.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out float f))
            {
                return f;
            }
            else
                return 0;
        }

        public int GetIntValueFromString(string s)
        {
            s = s.Replace(',', '.');
            if (int.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture.NumberFormat, out int f))
            {
                return f;
            }
            else
                return 0;
        }
    }
}
