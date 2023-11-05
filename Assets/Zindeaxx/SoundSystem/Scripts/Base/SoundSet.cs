using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Zindeaxx.SoundSystem.Utils;

namespace Zindeaxx.SoundSystem
{
    [CreateAssetMenu(fileName = "New Soundset", menuName = "SimpleSoundSystem/Soundset", order = 1)]
    public class SoundSet : ScriptableObject, IEnumerable<AudioClip>
    {
        [Header("Sound Settings")]
        [Tooltip("Which audio mixer group should be used by this sound")]
        public AudioMixerGroup MixerGroup ;
        [Range(0, 1)]
        [Tooltip("Defines the loudness of the sound. 1 is loud, 0 is silent.")]
        public float VolumeAmount = 1;

        [Tooltip("This defines the category of the sound which will be used for adjusting the volume based on the settings. Examples: Ambient, Environment, Voices")]
        public string VolumeID = "None";

        [Tooltip("Is this a collection of looped sounds?")]
        public bool LoopedSounds = false;

        [Tooltip("The minimum distance this sound can travel")]
        public float MinDistance = 1;

        [Tooltip("The maximum distance this sound can travel")]
        public float MaxDistance = 500;

        [Range(-3, 3)]
        public float Pitch = 1;

        [Tooltip("This is the same settings as the usual unity setting!")]
        public int Priority = 128;

        [Tooltip("Sets how much the sound is affected by 3D spatialisation calculations (attenuation, doppler etc). 0.0 makes the sound full 2D, 1.0 makes it full 3D.")]
        [Range(0, 1)] public float SpatialBlend = 1;

        public bool Spatialize = false;

        public bool SpatializePostEffects = false;

        /// <summary>
        /// All the clips of this soundset.
        /// </summary>
        [SerializeField]
        private List<AudioClip> m_Clips;

        public AudioClip[] Clips => m_Clips?.ToArray();

        public int ClipAmount => Clips.Length;

        /// <summary>
        /// Returns a random sound from this set
        /// </summary>
        public AudioClip RandomSound
        {
            get
            {
                if (Clips.Length != 0)
                {
                    return Clips[SoundUtils.RandomRange(0, Clips.Length)];
                }
                return default(AudioClip);
            }
        }

        public IEnumerator<AudioClip> GetEnumerator()
        {
            return m_Clips.GetEnumerator();
        }

        /// <summary>
        /// Returns a given sound from this set
        /// </summary>
        public AudioClip GetSound(int index)
        {
            return Clips[index];
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Clips.GetEnumerator();
        }

#if UNITY_EDITOR
        public void AddSoundClip(AudioClip clip)
        {
            if (m_Clips == null)
                m_Clips = new List<AudioClip>();

            m_Clips.Add(clip);
        }

        public void ReplaceClip(AudioClip clip, int index)
        {
            m_Clips[index] = clip;
        }

        public void RemoveClip(AudioClip clip)
        {
            m_Clips.Remove(clip);
        }
#endif
    }
}