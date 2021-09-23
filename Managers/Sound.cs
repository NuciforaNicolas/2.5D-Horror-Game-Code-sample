using UnityEngine;
using UnityEngine.Audio;

namespace Managers{
    [System.Serializable]
    public class Sound
    {
        [HideInInspector]
        [Tooltip("Audio source: put here audio clip and set volume, etc...")]
        public AudioSource source;
        [Tooltip("Reference to the sound clip file that will be played.")]
        public AudioClip clip;
        [Tooltip("Name of the audio clip used by audio manager to reference it")]
        public string name;
        [Tooltip("Determines the priority of this audio source among all the ones that coexist in the scene. (Priority: 0 = most important. 256 = least important. Default = 128.). Use 0 for music tracks to avoid it getting occasionally swapped out.")]
        [Range(0, 256)]
        public int priority = 128;
        [Tooltip("How loud the sound is at a distance of one world unit (one meter) from the Audio Listener")]
        [Range(0,1)]
        public float volume = 1;
        [Tooltip("Amount of change in pitch due to slowdown/speed up of the Audio Clip. Value 1 is normal playback speed.")]
        [Range(-3,3)]
        public float pitch = 1;
        [Tooltip("Sets the position in the stereo field of 2D sounds.")]
        [Range(-1, 1)]
        public float stereoPan = 0;
        [Tooltip("Sets how much the 3D engine has an effect on the audio source.")]
        [Range(0, 1)]
        public float spatialBlend = 0;
        [Tooltip("Determines how much doppler effect will be applied to this audio source (if is set to 0, then no effect is applied).")]
        [Range(0, 5)]
        public float dopplerLevel = 1;
        [Tooltip("Sets the spread angle to 3D stereo or multichannel sound in speaker space.")]
        [Range(0, 360)]
        public float spread = 0;
        [Tooltip("Within the MinDistance, the sound will stay at loudest possible. Outside MinDistance it will begin to attenuate. Increase the MinDistance of a sound to make it ‘louder’ in a 3d world, and decrease it to make it ‘quieter’ in a 3d world.")]
        public float minDistance = 1;
        [Tooltip("The distance where the sound stops attenuating at. Beyond this point it will stay at the volume it would be at MaxDistance units from the listener and will not attenuate any more.")]
        public float maxDistance = 500;
        [Tooltip("Enable this to make the Audio Clip loop when it reaches the end.")]
        public bool loop = false;
        [Tooltip("If enabled, the sound will start playing the moment the scene launches. If disabled, you need to start it using the Play() command from scripting.")]
        public bool playOnAwake = true;
    }
}
