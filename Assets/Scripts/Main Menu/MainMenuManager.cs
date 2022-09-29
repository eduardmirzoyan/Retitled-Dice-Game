using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    public static MainMenuManager instance;

    private void Awake()
    {
        // Singleton Logic
        if (MainMenuManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void StartGame() {
        // Debug
        print("Start Game");

        // Create new player
        GameManager.instance.CreateNewPlayer();

        // Load next scene
        TransitionManager.instance.LoadNextScene();
    }

    public void HowToPlay() {
        // Debug
        print("Open Help Screen");
    }

    public void QuitGame() {
        // Debug
        print("Quit Game");

        // Close application
        Application.Quit();
    }

}
