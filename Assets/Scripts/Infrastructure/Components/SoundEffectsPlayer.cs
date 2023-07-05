using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Audio;

public class SoundEffectsPlayer : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;

    public void PlaySound(AudioClip clip, float playbackSpeed = 1)
    {
        _audioSource.pitch = playbackSpeed;
        _audioSource.PlayOneShot(clip);
    }
}
