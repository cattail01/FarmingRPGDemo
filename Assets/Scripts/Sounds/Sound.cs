using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sound: MonoBehaviour
{
    private AudioSource audioSource;

    public void SetSound(SoundItem soundItem)
    {
        audioSource.pitch =
            Random.Range(soundItem.SoundPitchRandomVariationMin, soundItem.SoundPitchRandomVariationMax);
        audioSource.volume = soundItem.SoundVolume;
        audioSource.clip = soundItem.SoundClip;
    }

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        if (audioSource.clip != null)
        {
            audioSource.Play();
        }
    }

    private void OnDisable()
    {
        audioSource.Stop();
    }
}
