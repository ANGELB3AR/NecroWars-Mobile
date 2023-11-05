using UnityEngine;


namespace Zindeaxx.SoundSystem
{
    [RequireComponent(typeof(SoundManager))]
    public class SoundOnAwake : MonoBehaviour
    {
        /// <summary>
        /// The Sound we would like to play on awake
        /// </summary>
        public SoundSet AssignedSoundSet;
        private SoundManager m_Sound;

        private void Awake()
        {
            //First setup the sound manager
            InitSounds();
            //Play the sound
            PlaySounds();
        }

        /// <summary>
        /// Creates an instance of SoundManager if no instance can be found on this object
        /// </summary>
        private void InitSounds()
        {
            m_Sound = GetComponent<SoundManager>();

            if (m_Sound == null)
                m_Sound = gameObject.AddComponent<SoundManager>();
        }

        /// <summary>
        /// Plays the sound
        /// </summary>
        private void PlaySounds()
        {
            if (AssignedSoundSet != null && AssignedSoundSet.RandomSound != null)
            {
                if (m_Sound == null)
                    InitSounds();

                m_Sound.PlaySound(AssignedSoundSet);
            }
            else
            {
                Debug.LogError(gameObject.name + " has no assigned SoundSet to play!");
            }
        }

    }
}