/*A class to handle all audio within the game*/
using UnityEngine.Audio;
using System;
using UnityEngine;

public class AudioManager : MonoBehaviour
{

    public Sound[] sounds; //creates an array of Sounds

    public static AudioManager instance;
    private float masterVolume;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }


        DontDestroyOnLoad(gameObject);//allows it to persist through levels

        foreach (Sound s in sounds)//loops through the sound array
        {
            s.source = gameObject.AddComponent<AudioSource>();
            s.source.clip = s.clip;

            s.source.volume = s.volume;
            s.source.spatialBlend = s.spatialblend;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
        }
    }

    void Start()
    {
        Play("Music");//plays the sound called music
        Play("BoostEngine");//plays the sound called BoostEngine
        Play("MoveEngine");//plays the sound called MoveEngine
    }

    public void Play(string name)//plays sounds 
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);//finds the specified sound in the array
        if (s == null)
        {
            Debug.LogWarning("Sound" + s + "not found.");//if sound cant be found
            return;
        }
        s.source.Play();//plays the sound
    }

    public void ChangeVolume(string name, float newVolume)
    {
        Sound s = Array.Find(sounds, sound => sound.name == name);//finds the specified sound in the array
        if (s == null)
        {
            Debug.LogWarning("Sound" + s + "not found.");//if sound cant be found
            return;
        }
        s.source.volume = newVolume;
    }

    public void ChangeAudioVolume(float newVolume, Sound.Type soundType)
    {
        foreach (Sound s in sounds)
        {
            if (s.SoundType == soundType)
            {
                s.source.volume = newVolume;
            }
        }
    }

    public void ChangeMasterVolume(float newVolume)
    {
        masterVolume = newVolume;
        foreach (Sound s in sounds)
        {
            s.source.volume = s.source.volume*newVolume;
        }
    }
}
