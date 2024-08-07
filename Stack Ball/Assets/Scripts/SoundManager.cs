using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    private AudioSource audioSource;
    public bool sound = true;

    private void Awake()
    {
        makeSingleton();
        audioSource = GetComponent<AudioSource>();
        UpdateAudioSourceMute(); // Başlangıçta mute durumunu güncelle
    }

    private void makeSingleton()
    {
        if (instance != null)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SoundOnOff()
    {
        sound = !sound;
        UpdateAudioSourceMute(); // `sound` değişkenine göre mute durumunu güncelle
    }

    private void UpdateAudioSourceMute()
    {
        audioSource.mute = !sound; // `sound` true ise mute false olur ve ses açılır, tersi durumda ses kapanır
    }

    public void playSoundFX(AudioClip clip, float volume)
    {
        if (!audioSource.mute) // Ses kapalı değilse ses çalar
        {
            audioSource.PlayOneShot(clip, volume);
        }
    }
}