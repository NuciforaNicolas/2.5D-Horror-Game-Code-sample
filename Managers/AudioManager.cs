
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Managers{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] Sound[] sounds;
        Dictionary<string, Sound> soundsMap;

        private void Awake()
        {
            base.Awake();
            Debug.Log("AudioManager Instance: " + Instance);
            soundsMap = new Dictionary<string, Sound>();
            if(sounds != null && sounds.Length > 0){
                foreach(Sound s in sounds){
                    s.source = gameObject.AddComponent<AudioSource>();

                    s.source.clip = s.clip;
                    s.source.priority = s.priority;
                    s.source.volume = s.volume;
                    s.source.pitch = s.pitch;
                    s.source.panStereo = s.stereoPan;
                    s.source.spatialBlend = s.spatialBlend;
                    s.source.dopplerLevel = s.dopplerLevel;
                    s.source.spread = s.spread;
                    s.source.minDistance = s.minDistance;
                    s.source.maxDistance = s.maxDistance;
                    s.source.loop = s.loop;
                    s.source.playOnAwake = s.playOnAwake;

                    soundsMap.Add(s.name, s);
                }
            }
        }

        private void Start()
        {
            Play("MainTheme");
        }

        public void Play(string name){
            Sound s = (soundsMap != null && soundsMap.ContainsKey(name)) ? soundsMap[name] : null;
            if(s == null){
                Debug.LogError("Sound " + name + " not present");
                return;
            }
            Debug.Log("Reproducing: " + name);
            s.source.Play();
        }

        public void ChangeVolume(float volume){
            if(soundsMap != null){
                foreach (Sound s in soundsMap.Values)
                {
                    s.source.volume = volume;
                }
            }
        }

        public void Stop(string name)
        {
            Sound s = (soundsMap != null && soundsMap.ContainsKey(name)) ? soundsMap[name] : null;
            if (s == null)
            {
                Debug.LogError("Sound " + name + " not present");
                return;
            }
            s.source.Stop();
        }

        public void Pause(string name)
        {
            Sound s = (soundsMap != null && soundsMap.ContainsKey(name)) ? soundsMap[name] : null;
            if(s == null)
            {
                Debug.LogError("Sound " + name + " not present");
                return;
            }
            s.source.Pause();
        }

        public void UnPause(string name)
        {
            Sound s = (soundsMap != null && soundsMap.ContainsKey(name)) ? soundsMap[name] : null;
            if (s == null)
            {
                Debug.LogError("Sound " + name + " not present");
                return;
            }
            s.source.UnPause();
        }

        /// <summary>
        /// Play the clip with a delay in seconds
        /// </summary>
        /// <param name="name">clip name</param>
        /// <param name="time">how long to delay in seconds</param>
        public void PlayDelayed(string name, float time)
        {
            Sound s = (soundsMap != null && soundsMap.ContainsKey(name)) ? soundsMap[name] : null;
            if (s == null)
            {
                Debug.LogError("Sound " + name + " not present");
                return;
            }
            s.source.PlayDelayed(time);
        }

        /// <summary>
        /// Play the clip at a specified time
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        public void PlayScheduled(string name, float time)
        {
            Sound s = (soundsMap != null && soundsMap.ContainsKey(name)) ? soundsMap[name] : null;
            if (s == null)
            {
                Debug.LogError("Sound " + name + " not present");
                return;
            }
            s.source.PlayScheduled(time);
        }

        /// <summary>
        /// Change the time at witch a clip that has already been scheduled to play will start
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        public void SetScheduledStartTime(string name, float time)
        {
            Sound s = (soundsMap != null && soundsMap.ContainsKey(name)) ? soundsMap[name] : null;
            if (s == null)
            {
                Debug.LogError("Sound " + name + " not present");
                return;
            }
            s.source.SetScheduledStartTime(time);
        }

        /// <summary>
        /// Change the time at witch a clip that has already been scheduled to play will end
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        public void SetScheduledEndTime(string name, float time)
        {
            Sound s = (soundsMap != null && soundsMap.ContainsKey(name)) ? soundsMap[name] : null;
            if (s == null)
            {
                Debug.LogError("Sound " + name + " not present");
                return;
            }
            s.source.SetScheduledEndTime(time);
        }

        /// <summary>
        /// Seek the playback time
        /// </summary>
        /// <param name="name"></param>
        /// <param name="time"></param>
        public void SetPlayback(string name, float time)
        {
            Sound s = (soundsMap != null && soundsMap.ContainsKey(name)) ? soundsMap[name] : null;
            if (s == null)
            {
                Debug.LogError("Sound " + name + " not present");
                return;
            }
            s.source.time += time;
        }

        public bool isPlaying(string name)
        {
            Sound s = (soundsMap != null && soundsMap.ContainsKey(name)) ? soundsMap[name] : null;
            if (s == null)
            {
                Debug.LogError("Sound " + name + " not present");
                return false;
            }
            return s.source.isPlaying;
        }
    }
}
