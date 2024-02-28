using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class SettingsManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TMP_Dropdown resolutionsDropdown;
    [SerializeField] private KeyCode toggleKey = KeyCode.Tab;

    [Header("Settings")]
    [SerializeField] private bool logChanges;

    [Header("Debug")]
    [SerializeField] private bool isOpen;

    private Resolution[] resolutions;

    public static SettingsManager instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        canvasGroup = GetComponentInChildren<CanvasGroup>();
    }

    private void Start()
    {
        // Cache possible resolutions based on hardware
        resolutions = Screen.resolutions;

        // Clear any options
        resolutionsDropdown.ClearOptions();

        // Format resolutions
        List<string> options = new List<string>();
        int currentResIndex = 0;

        for (int i = 0; i < resolutions.Length; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height;
            options.Add(option);

            if (resolutions[i].height == Screen.currentResolution.height && resolutions[i].width == Screen.currentResolution.width)
            {
                currentResIndex = i;
            }
        }

        // Update possible resolutions
        resolutionsDropdown.AddOptions(options);
        // Mark your current resolution
        resolutionsDropdown.value = currentResIndex;
        // Update the options
        resolutionsDropdown.RefreshShownValue();

        Close();
    }

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey) && isOpen)
        {
            Close();
        }
    }

    public void Open()
    {
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        isOpen = true;
    }

    public void Close()
    {
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        isOpen = false;
    }

    /// ~~~~~ Functionality Here ~~~~~

    public void SetMasterVolume(float volume)
    {
        // Convert linear to log scaling
        float realVolume = Mathf.Log10(volume) * 25f;

        // Set mixer volume 
        audioMixer.SetFloat("MasterVolume", realVolume);

        // Debug
        if (logChanges) Debug.Log("Master volume set to: " + realVolume);
    }

    public void SetMusicVolume(float volume)
    {
        // Convert linear to log scaling
        float realVolume = Mathf.Log10(volume) * 25f;

        // Set mixer volume 
        audioMixer.SetFloat("MusicVolume", realVolume);

        // Debug
        if (logChanges) Debug.Log("Music volume set to: " + realVolume);
    }

    public void SetSFXVolume(float volume)
    {
        // Convert linear to log scaling
        float realVolume = Mathf.Log10(volume) * 25f;

        // Set mixer volume 
        audioMixer.SetFloat("SFXVolume", realVolume);

        // Debug
        if (logChanges) Debug.Log("SFX volume set to: " + realVolume);
    }

    public void SetQuality(int qualityIndex)
    {
        // Not used yet...

        QualitySettings.SetQualityLevel(qualityIndex);

        // Debug
        if (logChanges) Debug.Log("Quality set to: " + qualityIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        // Get resolution from our dropdown
        Resolution resolution = resolutions[resolutionIndex];

        // Update resolution
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);

        // Debug
        if (logChanges) Debug.Log("Resolution set to: " + resolution.width + " x " + resolution.height);
    }

    public void SetFullscreen(bool isFullscreen)
    {
        // Set fullscreen
        Screen.fullScreen = isFullscreen;

        // Debug
        if (logChanges) Debug.Log("Fullscreen set to: " + isFullscreen.ToString());
    }
}
