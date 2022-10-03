using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{
    
    [SerializeField] private List<Sound> sounds;
    [SerializeField] private float fadeTime = 1f;
    private Coroutine coroutine;

    private string song;
    public static AudioManager instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        // Format sounds
        foreach (var sound in sounds)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.audioClip;

            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.loop = sound.loop;

            sound.audioSource.outputAudioMixerGroup = sound.audioMixerGroup;
        }

        // Persist between scenes
        DontDestroyOnLoad(this);
    }

    private void Start()
    {
        // Play background music based on which scene you are in
        // Play("Background " + TransitionManager.instance.GetSceneIndex());
    }

    private IEnumerator FadeInAudio(Sound sound)
    {
        var startVolume = 0;
        var endVolume = sound.volume;
        float timer = 0;

        // Play source
        sound.audioSource.Play();

        while (timer < fadeTime)
        {
            // Lerp volume towards max
            sound.audioSource.volume = Mathf.Lerp(startVolume, endVolume, timer / fadeTime);

            // Decrement time
            timer += Time.deltaTime;
            yield return null;
        }

        // Set to desired volume
        sound.audioSource.volume = sound.volume;
    }

    private IEnumerator FadeOutAudio(Sound sound)
    {
        var startVolume = sound.volume;
        var endVolume = 0;
        float timer = 0;

        while (timer < fadeTime)
        {
            // Lerp volume towards max
            sound.audioSource.volume = Mathf.Lerp(startVolume, endVolume, timer / fadeTime);

            // Decrement time
            timer += Time.deltaTime;
            yield return null;
        }

        // Stop playing
        sound.audioSource.Stop();

        // Reset volume
        sound.audioSource.volume = sound.volume;

    }

    public void Play(string name)
    {
        // Don't replay same song
        if (song == name) return;

        Sound sound = sounds.Find(sound => sound.name == name);
        if (sound != null)
        {
            this.song = name;
            if (coroutine != null) StopCoroutine(coroutine);

            coroutine = StartCoroutine(FadeInAudio(sound));
        }
        else { throw new System.Exception("Sound with that name not found: " + name); }
    }

    public void Stop(string name)
    {
        Sound sound = sounds.Find(sound => sound.name == name);
        if (sound != null)
        {
            if (coroutine != null) StopCoroutine(coroutine);

            coroutine = StartCoroutine(FadeOutAudio(sound));
        }
        else { throw new System.Exception("Sound with that name not found: " + name); }
    }
}
