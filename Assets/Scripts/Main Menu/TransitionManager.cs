using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionManager : MonoBehaviour
{
    public static TransitionManager instance;
    [SerializeField] private Animator animator;
    [SerializeField] private float transitionTime;
    private Coroutine coroutine;

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

    public void LoadNextScene()
    {
        // Stop any background music
        // AudioManager.instance.Stop("Background " + GetSceneIndex());

        // Stop any transition if one was happening
        if (coroutine != null) StopCoroutine(coroutine);

        // Transition to next scene
        coroutine = StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void ReloadScene() {
        // Stop any transition if one was happening
        if (coroutine != null) StopCoroutine(coroutine);

        // Transition to same scene
        coroutine = StartCoroutine(LoadScene(SceneManager.GetActiveScene().buildIndex));
    }

    public void LoadMainMenu()
    {
        // Stop any background music
        // AudioManager.instance.Stop("Background " + GetSceneIndex());

        // Stop any transition if one was happening
        if (coroutine != null) StopCoroutine(coroutine);

        // Transition to main menu
        coroutine = StartCoroutine(LoadScene(0));
    }

    private IEnumerator LoadScene(int index)
    {
        // Play animation
        animator.Play("Transition Out");

        // Wait
        yield return new WaitForSeconds(transitionTime);

        // Load scene
        SceneManager.LoadScene(index);
    }
}