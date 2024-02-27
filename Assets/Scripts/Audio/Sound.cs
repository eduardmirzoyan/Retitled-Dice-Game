using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[System.Serializable]
public class Sound
{
    public string name;
    public List<AudioClip> audioClips;

    [Range(0, 1f)]
    public float volume;

    [Range(-1f, 1f)]
    public float pitch;

    public bool loop;

    [HideInInspector]
    public AudioSource audioSource;

    public AudioMixerGroup audioMixerGroup;
}
