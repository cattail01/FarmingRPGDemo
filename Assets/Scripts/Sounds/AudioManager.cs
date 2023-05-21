using Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class AudioManager : SingletonMonoBehavior<AudioManager>
{
    [SerializeField] private GameObject soundPrefab = null;

    [Header("Other")] [SerializeField] private SO_SoundList soSoundList = null;

    [Header("Audio Sources")] [SerializeField]
    private AudioSource ambientSoundAudioSource = null;

    [SerializeField] private AudioSource gameMusicAudioSource;

    [Header("Audio Mixers")] [SerializeField]
    private AudioMixer gameAudioMixer;

    [Header("Audio Snapshots")] [SerializeField]
    private AudioMixerSnapshot gameMusicSnapshot;

    [SerializeField] private AudioMixerSnapshot gameAmbientSnapshot;

    [SerializeField] private SO_SceneSoundsList soSceneSoundsList;
    [SerializeField] private float defaultSceneMusicPayTimeSeconds = 120f;
    [SerializeField] private float sceneMusicStartMinSecs = 20f;
    [SerializeField] private float sceneMusicStartMaxSecs = 40f;
    [SerializeField] private float musicTransitionSecs = 8f;

    private Dictionary<SoundName, SoundItem> soundDictionary;
    private Dictionary<SceneName, SceneSoundsItem> sceneSoundsDictionary;

    private Coroutine playSceneSoundsCoroutine;

    private IEnumerator DisableSound(GameObject soundGameObject, float soundDuration)
    {
        yield return new WaitForSeconds(soundDuration);
        soundGameObject.SetActive(false);
    }

    public void PlaySound(SoundName soundName)
    {
        if (soundDictionary.TryGetValue(soundName, out SoundItem soundItem) && soundPrefab != null)
        {
            GameObject soundGameObject =
                PoolManager.Instance.ReuseObject(soundPrefab, Vector3.zero, Quaternion.identity);
            Sound sound = soundGameObject.GetComponent<Sound>();

            sound.SetSound(soundItem);
            soundGameObject.SetActive(true);
            StartCoroutine(DisableSound(soundGameObject, soundItem.SoundClip.length));
        }
    }

    private float ConvertSoundVolumeDecimalFractionToDecibels(float volumeDecimalFraction)
    {
        return (volumeDecimalFraction * 100f - 80f);
    }

    private void PlayMusicSoundClip(SoundItem musicSoundItem, float transitionTimeSounds)
    {
        gameAudioMixer.SetFloat("MusicVolume", ConvertSoundVolumeDecimalFractionToDecibels(musicSoundItem.SoundVolume));
        gameMusicAudioSource.clip = musicSoundItem.SoundClip;
        gameMusicAudioSource.Play();

        gameMusicSnapshot.TransitionTo(transitionTimeSounds);
    }

    private void PlayAmbientSoundClip(SoundItem ambientSoundItem, float transitionTimeSeconds)
    {
        gameAudioMixer.SetFloat("AmbientVolume",
            ConvertSoundVolumeDecimalFractionToDecibels(ambientSoundItem.SoundVolume));

        ambientSoundAudioSource.clip = ambientSoundItem.SoundClip;
        ambientSoundAudioSource.Play();

        gameAmbientSnapshot.TransitionTo(transitionTimeSeconds);
    }

    private IEnumerator PlayerSceneSoundsRoutine(float musicPlaySeconds, SoundItem musicSoundItem,
        SoundItem ambientSoundItem)
    {
        if (musicSoundItem != null && ambientSoundItem != null)
        {
            PlayAmbientSoundClip(ambientSoundItem, 0f);
            yield return new WaitForSeconds(UnityEngine.Random.Range(sceneMusicStartMinSecs, sceneMusicStartMaxSecs));
            PlayMusicSoundClip(musicSoundItem, musicTransitionSecs);
            yield return new WaitForSeconds(musicPlaySeconds);
            PlayAmbientSoundClip(ambientSoundItem, musicTransitionSecs);
        }
    }

    private void PlaySceneSounds()
    {
        SoundItem musicSoundItem = null;
        SoundItem ambientSoundItem = null;

        float musicPlayTime = defaultSceneMusicPayTimeSeconds;

        if (Enum.TryParse<SceneName>(SceneManager.GetActiveScene().name, true, out SceneName currentSceneName))
        {
            if (sceneSoundsDictionary.TryGetValue(currentSceneName, out SceneSoundsItem sceneSoundsItem))
            {
                soundDictionary.TryGetValue(sceneSoundsItem.MusicForScene, out musicSoundItem);
                soundDictionary.TryGetValue(sceneSoundsItem.AmbientSoundForScene, out ambientSoundItem);
            }
            else
            {
                return;
            }

            if (playSceneSoundsCoroutine != null)
            {
                StopCoroutine(playSceneSoundsCoroutine);
            }

            playSceneSoundsCoroutine =
                StartCoroutine(PlayerSceneSoundsRoutine(musicPlayTime, musicSoundItem, ambientSoundItem));
        }
    }

    protected override void Awake()
    {
        base.Awake();

        soundDictionary = new Dictionary<SoundName, SoundItem>();
        foreach (SoundItem soundItem in soSoundList.soundDetails)
        {
            soundDictionary.Add(soundItem.SoundName, soundItem);
        }

        sceneSoundsDictionary = new Dictionary<SceneName, SceneSoundsItem>();
        foreach (SceneSoundsItem sceneSoundsItem in soSceneSoundsList.SceneSoundsDetails)
        {
            sceneSoundsDictionary.Add(sceneSoundsItem.SceneName, sceneSoundsItem);
        }
    }

    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += PlaySceneSounds;
    }

    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= PlaySceneSounds;
    }
}