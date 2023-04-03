using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using Random = UnityEngine.Random;

[RequireComponent(typeof(AudioSource))]
public class RandomBattleMusicPicker : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _musicClips;
    [SerializeField]
    private AudioClip _tutorialMusic;
    [SerializeField]
    private AudioSource _audioSource;

    private void OnEnable()
    {
        //_audioSource = GetComponent<AudioSource>();
        if (PlayerPrefs.GetInt("Battle Tutorial") > 0)
            PlayTutorialMusic();
        else
            PlayRandomMusic();
    }

    
    //TODO: Fix missing reference exception
    private async void PlayRandomMusic()
    {
        AudioClip clip = _musicClips[Random.Range(0, _musicClips.Length)];
        _audioSource.clip = clip;
        _audioSource.Play();

        await UniTask.Delay(TimeSpan.FromSeconds(clip.length));
        PlayRandomMusic();
    }

    private void PlayTutorialMusic()
    {
        _audioSource.clip = _tutorialMusic;
        _audioSource.Play();
    }
}
