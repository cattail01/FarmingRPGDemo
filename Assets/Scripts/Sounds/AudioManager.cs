using System.Collections;
using System.Collections.Generic;
using Enums;
using UnityEngine;

public class AudioManager : SingletonMonoBehavior<AudioManager>
{
    [SerializeField] private GameObject soundPrefab = null;

    [Header("Other")] [SerializeField] private SO_SoundList soSoundList = null;

    private Dictionary<SoundName, SoundItem> soundDictionary;

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

    protected override void Awake()
    {
        base.Awake();

        soundDictionary = new Dictionary<SoundName, SoundItem>();
        foreach (SoundItem soundItem in soSoundList.soundDetails)
        {
            soundDictionary.Add(soundItem.SoundName, soundItem);
        }
    }
}
