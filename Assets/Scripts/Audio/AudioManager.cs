using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager : MonoBehaviour
{

    [SerializeField] private List<Sound> ostSounds;
    [SerializeField] private List<Sound> sfxSounds;
    [SerializeField] private float fadeTime = 1f;
    private Coroutine coroutine;

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
        foreach (var sound in ostSounds)
        {
            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.audioClip;

            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.loop = sound.loop;

            sound.audioSource.outputAudioMixerGroup = sound.audioMixerGroup;
        }

        foreach (var sound in sfxSounds)
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

    public void PlayOST(string name)
    {
        Sound sound = ostSounds.Find(sound => sound.name == name);
        if (sound != null)
        {
            if (coroutine != null) StopCoroutine(coroutine);

            coroutine = StartCoroutine(FadeInAudio(sound));
        }
        else { throw new System.Exception("Sound with that name not found: " + name); }
    }

    public void StopOST(string name)
    {
        Sound sound = ostSounds.Find(sound => sound.name == name);
        if (sound != null)
        {
            if (coroutine != null) StopCoroutine(coroutine);

            coroutine = StartCoroutine(FadeOutAudio(sound));
        }
        else { throw new System.Exception("Sound with that name not found: " + name); }
    }

    public void PlaySFX(string name)
    {
        // Get all sounds with name
        var sounds = sfxSounds.FindAll(sound => sound.name == name);
        if (sounds.Count > 0)
        {
            // Randomly choose one
            var sound = sounds[Random.Range(0, sounds.Count)];

            // Set volume
            sound.audioSource.volume = sound.volume;
            sound.audioSource.loop = sound.loop;

            // Play sound
            sound.audioSource.Play();
        }
        else { throw new System.Exception("No sounds with that name found: " + name); }
    }

    public void StopSFX(string name)
    {
        // Get all sounds with name
        var sounds = sfxSounds.FindAll(sound => sound.name == name);
        if (sounds.Count > 0)
        {
            // Randomly choose one
            var sound = sounds[Random.Range(0, sounds.Count)];

            // Play sound
            sound.audioSource.Stop();
        }
        else { throw new System.Exception("No sounds with that name found: " + name); }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.D))
        {
            InvokeRepeating("FootstepsSFX", 0f, 0.25f);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            // InvokeRepeating("PlayFootsteps", 0f, 1f);
            CancelInvoke("FootstepsSFX");
        }
    }



}
