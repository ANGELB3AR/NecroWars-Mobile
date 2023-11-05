using System.Collections;
using UnityEngine;

namespace Zindeaxx.SoundSystem
{
    [RequireComponent(typeof(SoundManager))]
    public class LoopedSounds : MonoBehaviour
    {
        public SoundSet AssignedSoundSet;
        public bool Autoplay = true;

        private SoundManager m_Sound;

        private void Start()
        {
            InitSounds();
            StartCoroutine(PlaySounds());
        }

        private void InitSounds()
        {
            m_Sound = GetComponent<SoundManager>();

            if (m_Sound == null)
                m_Sound = gameObject.AddComponent<SoundManager>();

        }

        private IEnumerator PlaySounds()
        {
            while (Autoplay)
            {
                if (AssignedSoundSet != null && AssignedSoundSet.RandomSound != null)
                {
                    if (m_Sound == null)
                        InitSounds();

                    yield return new WaitForSecondsRealtime(m_Sound.PlaySound(AssignedSoundSet).length + 2);
                }
                else
                {
                    Debug.LogError(gameObject.name + " has no assigned SoundSet to play!");
                }
                yield return null;
            }
        }

    }

}