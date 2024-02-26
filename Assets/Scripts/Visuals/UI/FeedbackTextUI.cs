using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FeedbackTextUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private TextMeshProUGUI messageLabel;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Settings")]
    [SerializeField] private float hoverDuration;
    [SerializeField] private float hoverOffset;
    [SerializeField] private float holdDuration;
    [SerializeField] private float fadeDuration;

    public void Initialize(string message)
    {
        // Set message
        messageLabel.text = message;

        // Fade over time
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        float elapsed = 0f;

        // Hover up
        Vector3 startPosition = transform.position;
        Vector3 endPosition = transform.position + Vector3.up * hoverOffset;
        while (elapsed < hoverDuration)
        {
            // Lerping
            canvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsed / hoverDuration);
            transform.position = Vector3.Lerp(startPosition, endPosition, elapsed / hoverDuration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        canvasGroup.alpha = 1f;
        transform.position = endPosition;

        // Pause
        yield return new WaitForSeconds(holdDuration);

        // Fade out
        elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            // Lerping
            canvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Destroy when done
        Destroy(gameObject);
    }
}
