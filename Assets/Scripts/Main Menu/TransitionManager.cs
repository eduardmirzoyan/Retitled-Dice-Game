using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Transform maskTransform;
    [SerializeField] private Transform backgroundTransform;

    [Header("Data")]
    [SerializeField] private float transitionTime = 1f;


    private Coroutine coroutine;
    public static TransitionManager instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;

        animator = GetComponentInChildren<Animator>();
    }

    public int GetSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    public void OpenScene(Vector3 location)
    {
        // Save child location
        var temp = backgroundTransform.position;

        // Center tranform on location
        if (location != Vector3.zero)
            maskTransform.position = Camera.main.WorldToScreenPoint(location);
        else
            maskTransform.localPosition = Vector3.zero;

        // Restore child location
        backgroundTransform.position = temp;

        // Play animation
        animator.Play("Transition In");

        // Play background music
        AudioManager.instance.PlayOST("OST " + GetSceneIndex());
    }

    public void LoadNextScene(Vector3 location)
    {
        // Stop any background music
        AudioManager.instance.StopOST("OST " + GetSceneIndex());

        // Stop any transition if one was happening
        if (coroutine != null) StopCoroutine(coroutine);

        // Transition to next scene
        coroutine = StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1, location));
    }

    public void ReloadScene(Vector3 location)
    {
        // Stop any background music
        // AudioManager.instance.Stop("Background " + GetSceneIndex());

        // Stop any transition if one was happening
        if (coroutine != null) StopCoroutine(coroutine);

        // Transition to same scene
        coroutine = StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex, location));
    }

    public void LoadMainMenuScene(Vector3 location)
    {
        // Stop any background music
        AudioManager.instance.StopOST("OST " + GetSceneIndex());

        // Stop any transition if one was happening
        if (coroutine != null) StopCoroutine(coroutine);

        // Transition to main menu, scene 0
        coroutine = StartCoroutine(LoadScene(0, location));
    }

    private IEnumerator LoadScene(int index, Vector3 location)
    {
        // Save child location
        var temp = backgroundTransform.position;

        // Move transform
        if (location != Vector3.zero)
            maskTransform.position = Camera.main.WorldToScreenPoint(location);
        else
            maskTransform.localPosition = Vector3.zero;

        // Restore child location
        backgroundTransform.position = temp;

        // Play animation
        animator.Play("Transition Out");

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load scene
        SceneManager.LoadScene(index);
    }
}