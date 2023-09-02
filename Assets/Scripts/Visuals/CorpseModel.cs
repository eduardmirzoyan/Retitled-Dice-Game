using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CorpseModel : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Rigidbody2D rigidbody2d;
    [SerializeField] private Vector2 randomAngleRange;
    [SerializeField] private Vector2 randomPowerRange;
    [SerializeField] private float lifetimeDuration;
    [SerializeField] private float fadeDuration;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        rigidbody2d = GetComponentInChildren<Rigidbody2D>();
    }

    public void Initialize(Entity entity, Vector2 direction)
    {
        // Update visual
        spriteRenderer.sprite = entity.modelSprite;

        // Get random angle and power to generate corpse
        float angle = Random.Range(randomAngleRange.x, randomAngleRange.y);
        var quaternion = Quaternion.Euler(0, 0, angle);
        float power = Random.Range(randomPowerRange.x, randomPowerRange.y);

        // Push corpse in a direction
        rigidbody2d.AddForce(quaternion * direction * power, ForceMode2D.Impulse);

        // Remove corpse after some time
        StartCoroutine(FadeOverTime(lifetimeDuration, fadeDuration));
    }

    private IEnumerator FadeOverTime(float delay, float duration)
    {
        // Wait first
        yield return new WaitForSeconds(delay);

        var color = spriteRenderer.color;
        var startAlpha = color.a;
        var endAlpha = 0f;

        // Fade out
        float elapsed = 0;
        while (elapsed < duration)
        {
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / duration);
            spriteRenderer.color = color;

            // Increment time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Destroy
        Destroy(gameObject);
    }
}
