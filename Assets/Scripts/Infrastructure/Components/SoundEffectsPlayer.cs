using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEffectsPlayer : MonoBehaviour
{
    private readonly HashSet<AudioClip> _recentClips = new();

    [SerializeField]
    private AudioSource _audioSource;
    [SerializeField]
    private int _sameClipCooldownMs = 100;

    public bool PlaySound(AudioClip clip, float playbackSpeed = 1)
    {
        if (_recentClips.Contains(clip))
            return false;
        _audioSource.pitch = playbackSpeed;
        _audioSource.PlayOneShot(clip);
        _recentClips.Add(clip);
        UniTask.Delay(_sameClipCooldownMs).ContinueWith(() => _recentClips.Remove(clip));
        return true;
    }

    private void OnEnable()
    {
        _recentClips.Clear();
    }

    private void OnDisable()
    {
        _recentClips.Clear();
    }
}
