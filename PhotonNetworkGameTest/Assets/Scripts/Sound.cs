/*Sound class used by the audiomanager to play different kinds of sounds with different attributes*/
using UnityEngine.Audio;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public AudioClip clip;//audio clip of sound
    public string name;//name of sound

    [HideInInspector]
    public AudioSource source;

    public bool loop;//whether to loop
    public enum Type {Music, Effects};
    public Type SoundType;

    [Range(0f, 1f)]
    public float volume = 1;//volume max and min
    [Range(0f, 1f)]
    public float spatialblend = 0;//spatial blend max and min
    [Range(0f, 3f)]
    public float pitch = 1;//pitch max and min
}
