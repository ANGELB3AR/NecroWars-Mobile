using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

namespace Zindeaxx.SoundSystem
{

    public class SoundManager : MonoBehaviour
    {
        /// <summary>
        /// Current List of Volume Settings
        /// </summary>
        public static List<VolumeControl> VolumeControllers;

        /// <summary>
        /// The default volume of all sounds. If this gets set to 0 the game will be completely silent
        /// </summary>
        public static float DefaultVolume = 1;

        /// <summary>
        /// Current instances of sounds
        /// </summary>
        private List<AudioSource> m_Audio;

        /// <summary>
        /// All Audiosources that are currently fading out and should not be checked for volume changes
        /// </summary>
        private List<AudioSource> _volumeIgnoreList = new List<AudioSource>();

        /// <summary>
        /// This enables sounds to keep playing even if the object is already deleted 
        /// </summary>
        public static bool UseInstances = true;

        public static bool SoundEnabled = true;

        private AudioSource CreateChannel()
        {
            CheckNullChannels();
            if (UseInstances)
            {
                if (m_Audio == null)
                    m_Audio = new List<AudioSource>();

                //Create a new object where we can attach the sound onto
                AudioSource source = new GameObject("SoundPlayer", typeof(AudioSource)).GetComponent<AudioSource>();

                //Set the position to the same as for the origin object
                source.transform.position = transform.position;

                //Add the sound to the running instances
                m_Audio.Add(source);

                return source;
            }
            else
            {
                //Adds the sound to this gameObject
                AudioSource output = gameObject.AddComponent<AudioSource>();

                //Now refresh the channels since we added another sound
                RefreshChannels();
                return output;
            }
        }

        /// <summary>
        /// Reloads all sound instances
        /// </summary>
        private void RefreshChannels()
        {
            m_Audio = new List<AudioSource>(GetComponents<AudioSource>());
        }

        private void CheckNullChannels()
        {
            if (m_Audio != null && m_Audio.Count > 0)
            {
                int nullIndex = m_Audio.IndexOf(null);
                if (nullIndex != -1)
                    m_Audio.RemoveAt(nullIndex);
            }
        }

        /// <summary>
        /// Base Method for playing sounds
        /// </summary>
        /// <param name="audio">The audio to be played</param>
        /// <param name="VolumeID">Settings identifier to adjust volume</param>
        /// <param name="Secondaryvolume">The volume of the sound itself</param>
        /// <param name="looped">Will this sound be looped</param>
        /// <param name="MinDistance">Minimum distance of this sound</param>
        /// <param name="MaxDistance">Maximum distance of this sound</param>
        /// <param name="pitch">Pitch of the sound</param>
        /// <param name="Priority">Sound priority</param>
        /// <param name="SpatialBlend">The spatial blend of the played sound</param>
        /// <param name="spatialized">Will this sound work with a spatialized sound system</param>
        /// <param name="spatializePost">Will the sound be spatialized after the post effects are applied</param>
        /// <returns></returns>
        public AudioClip PlaySound(AudioClip audio, string VolumeID,AudioMixerGroup mixerGroup, float Secondaryvolume, bool looped, float MinDistance = 1, float MaxDistance = 500, float pitch = 1, int Priority = 128, float SpatialBlend = 1, bool spatialized = false, bool spatializePost = false)
        {
            //First we create a channel (AudioSource) which will be used to play the sound
            AudioSource SelectedChannel = CreateChannel();

            //Now we apply the soundset settings to the audiosource
            SelectedChannel.clip = audio;
            SelectedChannel.volume = Secondaryvolume * GetVolume(VolumeID.ToLower());
            SelectedChannel.outputAudioMixerGroup = mixerGroup;
            SelectedChannel.loop = looped;
            SelectedChannel.minDistance = MinDistance;
            SelectedChannel.maxDistance = MaxDistance;
            SelectedChannel.rolloffMode = AudioRolloffMode.Linear;
            SelectedChannel.priority = Priority;
            SelectedChannel.spatialBlend = SpatialBlend;
            SelectedChannel.pitch = pitch;
            SelectedChannel.spatialize = spatialized;
            SelectedChannel.spatializePostEffects = spatializePost;

            //Since the volume could change during playing, we have to check constantly if the settings have been changed and change the volume accordingly
            if (gameObject.activeInHierarchy)
                StartCoroutine(CheckVolumeChange(SelectedChannel, VolumeID, Secondaryvolume));

            //Play the sound :)
            SelectedChannel.Play();

            if (!looped)
            {
                if (UseInstances)
                    Destroy(SelectedChannel.gameObject, audio.length);
                else
                    Destroy(SelectedChannel, audio.length);
            }

            return audio;
        }

        /// <summary>
        /// Constantly checks if the volume of the assigned audio has changed
        /// </summary>
        private IEnumerator CheckVolumeChange(AudioSource channel, string VolumeID, float Secondaryvolume)
        {
            while (channel != null)
            {
                if (!_volumeIgnoreList.Contains(channel))
                {
                    float targetVolume = Secondaryvolume * GetVolume(VolumeID.ToLower());
                    if (channel.volume != targetVolume)
                    {
                        channel.volume = targetVolume;
                    }
                }
                yield return null;
            }
        }

        /// <summary>
        /// Plays a random sound from a given Soundset
        /// </summary>
        /// <param name="soundSet">The Sounds we want to use</param>
        /// <param name="looped">Are these sounds looped (This is an override)</param>
        /// <returns></returns>
        public AudioClip PlaySound(SoundSet soundSet, bool looped)
        {
            if (soundSet == null)
            {
                Debug.LogError("Tried to play a nulled SoundSet on: " + gameObject.name);
                return null;
            }
            else
            {
                return PlaySound(
                    soundSet.RandomSound,
                    soundSet.VolumeID,
                    soundSet.MixerGroup,
                    soundSet.VolumeAmount,
                    soundSet.LoopedSounds,
                    soundSet.MinDistance,
                    soundSet.MaxDistance,
                    soundSet.Pitch,
                    soundSet.Priority,
                    soundSet.SpatialBlend,
                    soundSet.Spatialize,
                    soundSet.SpatializePostEffects
                    );
            }
        }

        /// <summary>
        /// Stops sounds from this soundset
        /// </summary>
        public void StopSound(SoundSet soundSet)
        {
            if (soundSet == null)
            {
                Debug.LogError("Tried to stop a NULL SoundSet at: " + gameObject.name);

            }
            else
            {
                foreach (AudioClip s in soundSet.Clips)
                    StopSound(s);
            }
        }

        /// <summary>
        /// Stops the given sound
        /// </summary>
        /// <param name="sound">The sound to stop</param>
        /// <param name="delete">Should the sound instance be deleted</param>
        public void StopSound(AudioClip sound, bool delete = true, bool fadeout = true, float fadeoutSpeed = 1.5f)
        {
            if (sound == null)
            {
                Debug.LogError("Tried to stop a NULL sound at: " + gameObject.name);
            }
            else
            {
                foreach (AudioSource s in m_Audio)
                {
                    if (s != null && s.clip == sound)
                    {
                        if (fadeout)
                            StartCoroutine(RemoveSoundFaded(s, delete, fadeoutSpeed));
                        else
                            RemoveSound(s, delete);
                        break;
                    }
                }
            }
        }

        private IEnumerator RemoveSoundFaded(AudioSource s, bool delete, float fadeoutSpeed)
        {
            _volumeIgnoreList.Add(s);
            while (s.volume > 0)
            {
                s.volume -= Time.deltaTime * fadeoutSpeed;
                yield return null;
            }
            s.volume = 0;
            RemoveSound(s,delete);
        }

        private void RemoveSound(AudioSource s, bool delete)
        {
                s.Stop();
                if (delete)
                {
                    if (UseInstances)
                        Destroy(s.gameObject);
                    else
                        Destroy(s);

                    m_Audio.Remove(s);
                }
        }

        public void StopAllSounds(bool delete = false)
        {
            if (m_Audio != null)
            {
                foreach (AudioSource s in m_Audio)
                {
                    StopSound(s.clip, delete);
                }
            }
            else
            {
                Debug.Log("There was no audio to stop!");
            }
        }

        /// <summary>
        /// Plays sounds from this soundset
        /// </summary>
        public AudioClip PlaySound(SoundSet soundSet)
        {
            if (soundSet != null)
            {
                return PlaySound(soundSet, soundSet.LoopedSounds);
            }
            else
            {
                Debug.LogWarning("Tried to play NULL SoundSet on " + gameObject.name);
                return null;
            }
        }

        /// <summary>
        /// This can be used to play sounds defined in an Animator. (Use this with Animation Events)
        /// </summary>
        /// <param name="soundSet"></param>
        public void PlayAnimatorSound(SoundSet soundSet)
        {
            PlaySound(soundSet, soundSet.LoopedSounds);

        }

        public AudioSource[] AudioChannels => m_Audio.ToArray();

        /// <summary>
        /// Returns the current volume for the given identifier
        /// </summary>
        /// <param name="ID">The settings id of the volume to be checked. Example: Ambient, Environment, Voices, Music</param>
        /// <returns></returns>
        public float GetVolume(string ID)
        {
            if (!SoundEnabled)
                return 0;

            if (VolumeControllers != null)
            {
                if (VolumeControllers.Any(x => x.ControlID.ToLower() == ID.ToLower()))
                {
                    //Debug.Log("SOUND: Returning Volume of: " + ID);
                    return VolumeControllers[VolumeControllers.FindIndex(x => x.ControlID.ToLower() == ID.ToLower())].ControlVolume * DefaultVolume;
                }
            }
            return DefaultVolume;
        }

        public static void SetVolumeControl(VolumeControl ToAdd)
        {
            SetVolumeControl(ToAdd.ControlID, ToAdd.ControlVolume);
        }

        public static void SetVolumeControl(string Id, float Volume)
        {
            if (VolumeControllers == null)
                VolumeControllers = new List<VolumeControl>();

            if (!VolumeControllers.Any(x => x.ControlID == Id))
            {
                VolumeControl newvol = new VolumeControl();
                newvol.ControlID = Id;
                newvol.ControlVolume = Volume;
                VolumeControllers.Add(newvol);
            }
            else
                VolumeControllers.Find(x => x.ControlID == Id).ControlVolume = Volume;
        }


    }

    public class VolumeControl
    {
        public string ControlID;
        public float ControlVolume;
    }
}