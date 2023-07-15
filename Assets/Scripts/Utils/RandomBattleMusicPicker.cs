using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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

    private CancellationTokenSource _currentTokenSource;

    private void OnEnable()
    {
        if (PlayerPrefs.GetInt("Battle Tutorial") > 0)
            PlayTutorialMusic();
        else
            PlayRandomMusic();
    }

    
    private async void PlayRandomMusic()
    {
        AudioClip clip = _musicClips[Random.Range(0, _musicClips.Length)];
        _audioSource.clip = clip;
        _audioSource.Play();
        if (_currentTokenSource != null)
            _currentTokenSource.Cancel();
        _currentTokenSource = new CancellationTokenSource();
        await UniTask
            .Delay(TimeSpan.FromSeconds(clip.length), cancellationToken: _currentTokenSource.Token)
            .SuppressCancellationThrow()
            .ContinueWith(isCanceled => { if (!isCanceled) PlayRandomMusic(); });
    }

    private void PlayTutorialMusic()
    {
        _audioSource.clip = _tutorialMusic;
        _audioSource.Play();
    }

    private void OnDestroy()
    {
        _currentTokenSource.Cancel();
    }
}
