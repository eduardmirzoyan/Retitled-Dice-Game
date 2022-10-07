using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static float drawTime = 1f;
    public static float travelSpeed = 0.1f;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void Initialize(Vector3 targetPosition, Weapon weapon)
    {
        // Set sprite
        spriteRenderer.sprite = weapon.sprite;

        // Calc time
        float time = travelSpeed * Vector3.Distance(transform.position, targetPosition);

        // Start traveling to target
        StartCoroutine(Travel(transform.position, targetPosition, time));
    }

    private IEnumerator Travel(Vector3 startPoint, Vector3 endPoint, float duration)
    {
        // Start timer
        Vector3 position;
        float elapsed = 0;
        while (elapsed < duration)
        {
            // Slerp model position
            position = Vector3.Slerp(startPoint, endPoint, elapsed / duration);
            transform.position = position;

            // Increment time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set to final destination
        transform.position = endPoint;

        // Destroy
        Destroy(gameObject);
    }
}
