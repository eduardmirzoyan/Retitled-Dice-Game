using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitFreeze : MonoBehaviour
{
    private bool isWaiting = false;

    public static HitFreeze instance;
    private void Awake()
    {
        // Singleton Logic
        if (HitFreeze.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void StartHitFreeze(float duration) {
        
        // Make sure we are not already freezing
        if (isWaiting) return;

        StartCoroutine(FreezeTime(duration));
    }

    private IEnumerator FreezeTime(float duration) {
        // Pause time
        isWaiting = true;
        Time.timeScale = 0f;

        // Wait
        yield return new WaitForSecondsRealtime(duration);

        // Resume time
        Time.timeScale = 1f;
        isWaiting = false;
    }
}
