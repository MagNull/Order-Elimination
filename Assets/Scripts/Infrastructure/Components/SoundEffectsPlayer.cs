using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEffectsPlayer : MonoBehaviour
{
    public class PlayingSound
    {

    }

    [SerializeField]
    private AudioSource _audioSource;

    public PlayingSound PlaySound(AudioClip clip, float playbackSpeed = 1)
    {
        _audioSource.pitch = playbackSpeed;
        _audioSource.PlayOneShot(clip);
        return new PlayingSound();
    }

    public bool StopSound(PlayingSound sound)
    {
        _audioSource.Stop();
        return true;
    }
}
