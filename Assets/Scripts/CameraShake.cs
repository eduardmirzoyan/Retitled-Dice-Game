using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private float magnitude = 0.4f;

    private Coroutine coroutine;

    public static CameraShake instance;
    private void Awake()
    {
        // Singleton Logic
        if (CameraShake.instance != null)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
    }

    public void ScreenShake(float duration) {
        // Start shaking screen
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(Shake(duration, magnitude));
    }

    private IEnumerator Shake(float duration, float magnitude)
    {
        // Save original position
        Vector3 originalPosition = transform.localPosition;

        // Start timer
        float elapsed = 0f;
        while (elapsed < duration)
        {
            // Generate random displacement
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

            // Translate
            transform.localPosition = new Vector3(x, y, originalPosition.z);
            
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Restore position
        transform.localPosition = originalPosition;
    }
}
