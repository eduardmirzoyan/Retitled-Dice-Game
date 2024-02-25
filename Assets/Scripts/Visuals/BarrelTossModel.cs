using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelTossModel : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform spriteTransform;

    [Header("Settings")]
    [SerializeField] private float tossHeight;
    [SerializeField] private float rotationSpeed;

    public IEnumerator Toss(Vector3Int startLocation, Vector3Int endLocation)
    {
        Vector3 startPoint = RoomManager.instance.GetLocationCenter(startLocation);
        Vector3 endPoint = RoomManager.instance.GetLocationCenter(endLocation);

        // Start timer
        Vector3 position;
        float elapsed = 0;
        float duration = GameManager.instance.gameSettings.actionDuration;

        Vector3 control = (startPoint + endPoint) / 2 + Vector3.up * tossHeight;
        while (elapsed < duration)
        {
            // Projectile motion
            float ratio = elapsed / duration;
            Vector3 ac = Vector3.Lerp(startPoint, control, ratio);
            Vector3 cb = Vector3.Lerp(control, endPoint, ratio);

            position = Vector3.Lerp(ac, cb, ratio);
            transform.position = position;

            // Rotate over time
            spriteTransform.Rotate(new Vector3(0, 0, rotationSpeed));

            // Increment time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set to final destination
        transform.position = endPoint;
    }
}
