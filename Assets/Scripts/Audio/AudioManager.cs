using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private List<Sound> ostSounds;
    [SerializeField] private List<Sound> sfxSounds;
    [SerializeField] private float fadeTime = 1f;
    private Coroutine coroutine;

    [Header("Debug")]
    [SerializeField] private string debugName;

    private string song;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.G))
        {
            PlaySFX(debugName);
        }
    }

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
            if (sound.audioClips == null || sound.audioClips.Count == 0)
                throw new System.Exception($"OST {sound.name} has no clips associated with it.");

            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.audioClips[0];

            sound.audioSource.volume = sound.volume;
            sound.audioSource.pitch = sound.pitch;
            sound.audioSource.loop = sound.loop;

            sound.audioSource.outputAudioMixerGroup = sound.audioMixerGroup;
        }

        foreach (var sound in sfxSounds)
        {
            if (sound.audioClips == null || sound.audioClips.Count == 0)
                throw new System.Exception($"SFX {sound.name} has no clips associated with it.");

            sound.audioSource = gameObject.AddComponent<AudioSource>();
            sound.audioSource.clip = sound.audioClips[0];

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
        if (song == name) return;

        Sound sound = ostSounds.Find(sound => sound.name == name);
        if (sound != null)
        {
            song = name;

            if (coroutine != null) StopCoroutine(coroutine);

            coroutine = StartCoroutine(FadeInAudio(sound));
        }
        else throw new System.Exception("Sound with that name not found: " + name);
    }

    public void StopOST(string name)
    {
        Sound sound = ostSounds.Find(sound => sound.name == name);
        if (sound != null)
        {
            if (coroutine != null) StopCoroutine(coroutine);

            coroutine = StartCoroutine(FadeOutAudio(sound));
        }
        else throw new System.Exception("Sound with that name not found: " + name);
    }

    public void PlayFiller()
    {
        // Using this function as a placeholder for replacing sfx
        PlaySFX("filler");
    }

    public void PlaySFX(string name)
    {
        // Get all sounds with name
        var sound = sfxSounds.Find(sound => sound.name == name);
        if (sound != null)
        {
            // Choose a random clip
            var clip = sound.audioClips[Random.Range(0, sound.audioClips.Count)];
            sound.audioSource.clip = clip;

            // Set volume
            sound.audioSource.volume = sound.volume;
            sound.audioSource.loop = sound.loop;

            // Play sound
            sound.audioSource.Play();
        }
        else throw new System.Exception("No sounds with that name found: " + name);
    }

    public void StopSFX(string name)
    {
        // Get all sounds with name
        var sound = sfxSounds.Find(sound => sound.name == name);
        if (sound != null)
        {
            // Play sound
            sound.audioSource.Stop();
        }
        else throw new System.Exception("No sounds with that name found: " + name);
    }
}
