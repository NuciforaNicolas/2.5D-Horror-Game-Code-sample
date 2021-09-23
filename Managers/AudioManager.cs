
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace Managers{
    public class AudioManager : Singleton<AudioManager>
    {
        [SerializeField] Sound[] sounds;
        Dictionary<string, Sound> soundsMap;

        private void Start()
        {
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
                Play("MainTheme");
            }
        }

       public void Play(string name){
            Sound s = soundsMap.ContainsKey(name) ? soundsMap[name] : null;
            if(s == null){
                Debug.LogError("Sound " + name + " not present");
                return;
            }
            Debug.Log("Reproducing: " + name);
            s.source.Play();
        }
    }
}
