using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public static float drawTime = 1f;

    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer shadowRenderer;
    [SerializeField] private Transform spriteTransform;

    [Header("Data")]
    [SerializeField] private float lingerDuration = 0.5f;
    [SerializeField] private float fadeDuration = 0.25f;
    [SerializeField] private float gravity = 9.81f;
    [SerializeField] private GameObject impactDustPrefab;

    public float Initialize(Vector3 targetPosition, float intialAngle, Weapon weapon)
    {
        // Set sprites
        spriteRenderer.sprite = weapon.sprite;
        shadowRenderer.sprite = weapon.sprite;

        // Set rotation
        spriteTransform.localEulerAngles = new Vector3(0, 0, intialAngle);

        // Setup positions
        Vector3 spriteStart = spriteTransform.localPosition;
        Vector3 spriteEnd = shadowRenderer.transform.localPosition;

        // Setup rotations
        Vector3 startRotation = spriteTransform.localEulerAngles;

        // Calculate distance to target
        float totalDistance = Vector3.Distance(transform.position, targetPosition);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float magnitudeVelocity = Mathf.Sqrt(totalDistance * gravity / Mathf.Sin(2 * intialAngle * Mathf.Deg2Rad));

        // Calc x vel
        float xVelocity = magnitudeVelocity * Mathf.Cos(intialAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightTime = totalDistance / xVelocity;

        // Calc y vel
        float yVelocity = ((spriteEnd.y - spriteStart.y) + 0.5f * gravity * flightTime * flightTime) / flightTime;

        // Calc movement offset
        float deltaX = xVelocity;

        // Start traveling to target
        StartCoroutine(Travel(xVelocity, yVelocity, flightTime)); ;

        // Return flight time
        return flightTime;
    }

    private IEnumerator Travel(float xVelocity, float yVelocity, float duration)
    {

        // Start timer
        float elapsed = 0;
        while (elapsed < duration)
        {
            // Move shadow in chosen direction depending on orientation
            transform.localPosition += transform.right * xVelocity * Time.deltaTime;

            // Move sprite seperately
            float deltaY = (yVelocity - (gravity * elapsed));
            spriteTransform.localPosition += new Vector3(0f, deltaY * Time.deltaTime, 0f);

            // Rotate sprite seprately based on change in position
            float angle = Mathf.Atan2(deltaY, Mathf.Abs(xVelocity)) * Mathf.Rad2Deg;
            spriteTransform.localRotation = Quaternion.AngleAxis(angle, Vector3.forward);

            // Increment time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Spawn dust
        Instantiate(impactDustPrefab, transform.position, transform.rotation);

        // Start to fade
        yield return FadeAway(lingerDuration, fadeDuration);
    }

    private IEnumerator FadeAway(float lingerDuration, float fadeDuration)
    {
        // Linger for a bit
        yield return new WaitForSeconds(lingerDuration);

        // Start color
        Color startSpriteColor = spriteRenderer.color;
        Color startShadowColor = shadowRenderer.color;
        Color endColor = Color.clear;

        // Start timer
        float elapsed = 0;
        while (elapsed < fadeDuration)
        {
            // Fade color over time
            spriteRenderer.color = Color.Lerp(startSpriteColor, endColor, elapsed / fadeDuration);
            shadowRenderer.color = Color.Lerp(startShadowColor, endColor, elapsed / fadeDuration);

            // Increment time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set final colors
        spriteRenderer.color = endColor;
        shadowRenderer.color = endColor;

        // Destroy
        Destroy(gameObject);
    }

    private IEnumerator SimulateProjectile(Vector3 target)
    {
        float firingAngle = 15f;

        // Calculate distance to target
        float target_Distance = Vector3.Distance(transform.position, target);

        // Calculate the velocity needed to throw the object to the target at specified angle.
        float projectile_Velocity = target_Distance / (Mathf.Sin(2 * firingAngle * Mathf.Deg2Rad) / gravity);

        // Extract the X  Y componenent of the velocity
        float Vx = Mathf.Sqrt(projectile_Velocity) * Mathf.Cos(firingAngle * Mathf.Deg2Rad);
        float Vy = Mathf.Sqrt(projectile_Velocity) * Mathf.Sin(firingAngle * Mathf.Deg2Rad);

        // Calculate flight time.
        float flightDuration = target_Distance / Vx;

        float elapse_time = 0;

        while (elapse_time < flightDuration)
        {
            transform.Translate(0, (Vy - (gravity * elapse_time)) * Time.deltaTime, Vx * Time.deltaTime);

            elapse_time += Time.deltaTime;

            yield return null;
        }
    }
}
