using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileModel : MonoBehaviour
{
    [Header("Model")]
    [SerializeField] private SpriteRenderer modelSpriteRenderer;
    [SerializeField] private SpriteRenderer shadowSpriteRenderer;
    [SerializeField] private Animator modelAnimator;

    [Header("Data")]
    [SerializeField] private Projectil projectil;

    private RoomUI roomUI;
    private Coroutine coroutine;

    public void Initialize(Projectil projectil, RoomUI roomUI) 
    {
        this.projectil = projectil;
        this.roomUI = roomUI;

        // Set visuals
        modelSpriteRenderer.sprite = projectil.modelSprite;
        shadowSpriteRenderer.sprite = projectil.modelSprite;
        modelAnimator.runtimeAnimatorController = projectil.modelController;

        // Sub to events
        GameEvents.instance.onProjectileMove += Move;
        GameEvents.instance.onProjectileDespawn += Despawn;
    }

    public void Uninitialize()
    {
        // Unsub
        GameEvents.instance.onProjectileMove -= Move;
        GameEvents.instance.onProjectileDespawn -= Despawn;
    }

    private void OnDestroy()
    {
        Uninitialize();
    }

    public void Move(Projectil projectil)
    {
        if (this.projectil == projectil)
        {
            Vector3 currentPosition = transform.position;
            Vector3 newPosition = roomUI.GetLocationCenter(projectil.location);

            // Allign model if needed
            RotateModel(newPosition - currentPosition);

            // Start moving routine
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(Move(currentPosition, newPosition, projectil.travelSpeed));
        }
    }

    private void RotateModel(Vector3 direction)
    {
        // Right
        if (direction.x > 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
        }
        // Left
        else if (direction.x < 0)
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
        }
        // Up
        else if (direction.y > 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, 90);
        }
        // Down
        else if (direction.y < 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, -90);
        }
    }

    private IEnumerator Move(Vector3 startPoint, Vector3 endPoint, float duration)
    {
        // Start timer
        Vector3 position;
        float elapsed = 0;
        while (elapsed < duration)
        {
            // Lerp model position
            position = Vector3.Lerp(startPoint, endPoint, elapsed / duration);
            transform.position = position;

            // Increment time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set to final destination
        transform.position = endPoint;
    }

    private void Despawn(Projectil projectil)
    {
        if (this.projectil == projectil)
        {
            // Spawn death particles
            Instantiate(projectil.deathEffectPrefab, transform.position, Quaternion.identity);

            // Destroy self
            Destroy(gameObject);
        }
    }
}
