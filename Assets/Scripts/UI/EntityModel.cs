using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DamageFlash))]
public class EntityModel : MonoBehaviour
{
    public static float moveSpeed = 0.35f;
    public static float warpSpeed = 0.5f;

    [Header("Sprites")]
    [SerializeField] private Transform offsetTransform;
    [SerializeField] private SpriteRenderer modelSpriteRenderer;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    

    [Header("Animation")]
    [SerializeField] private Animator modelAnimator;
    [SerializeField] private Animator weaponAnimator;

    [Header("UI")]
    [SerializeField] private DamageFlash damageFlash;
    [SerializeField] private DieUI dieUI;

    [Header("Particles")]
    [SerializeField] private ParticleSystem warpGenerateParticles;
    [SerializeField] private ParticleSystem warpDustParticles;
    [SerializeField] private GameObject deathCloud;

    [Header("Data")]
    [SerializeField] private Entity entity;
    [SerializeField] private bool isFacingRight = true;
    
    private Coroutine coroutine;

    public void Initialize(Entity entity)
    {
        this.entity = entity;

        // Set model sprite
        modelSpriteRenderer.sprite = entity.modelSprite;

        // Apply offset
        offsetTransform.localPosition = entity.offsetDueToSize;

        // Set weapon sprite
        if (entity.weapon != null)
        {
            weaponSpriteRenderer.sprite = entity.weapon.sprite;
        }
        else {
            weaponSpriteRenderer.enabled = false;
        }

        // Set model animator
        modelAnimator.runtimeAnimatorController = entity.modelController;

        // Set weapon animator
        weaponAnimator.runtimeAnimatorController = entity.weaponController;

        // If entity is an AI, display it's first die
        if (entity.AI != null)
        {
            dieUI.Initialize(entity.actions[0], false);
        }
        else {
            dieUI.gameObject.SetActive(false);
        }

        // Sub to events
        GameEvents.instance.onEntityMove += MoveEntity;
        GameEvents.instance.onEntityWarp += WarpEntity;
        GameEvents.instance.onEntityTakeDamage += TakeDamage;
        GameEvents.instance.onEntityMeleeAttack += MeleeAttack;
        GameEvents.instance.onEntityReadyWeapon += ReadyWeapon;
        GameEvents.instance.onRemoveEntity += RemoveEntity;
    }

    public void Uninitialize()
    {
        // Unsub to events
        GameEvents.instance.onEntityMove -= MoveEntity;
        GameEvents.instance.onEntityWarp -= WarpEntity;
        GameEvents.instance.onEntityTakeDamage -= TakeDamage;
        GameEvents.instance.onEntityMeleeAttack -= MeleeAttack;
        GameEvents.instance.onEntityReadyWeapon -= ReadyWeapon;
        GameEvents.instance.onRemoveEntity -= RemoveEntity;

        if (dieUI != null) dieUI.Uninitialize();
    }

    private void OnDestroy()
    {
        Uninitialize();
    }

    private void RemoveEntity(Entity entity)
    {
        if (this.entity == entity)
        {
            // Spawn death cloud 
            Instantiate(deathCloud, transform.position, Quaternion.identity);

            // Destroy self
            Destroy(gameObject);
        }
    }

    private void MoveEntity(Entity entity, bool isMoving)
    {
        // If this entity moved, then move it
        if (this.entity == entity)
        {
            if (isMoving)
            {
                // Play proper animation
                modelAnimator.Play("Run");

                // Get positions
                Vector3 startPoint = transform.position;
                Vector3 endPoint = RoomUI.instance.floorTilemap.GetCellCenterWorld(entity.location);

                // Check if sprite should be flipped
                Vector3 dir = endPoint - startPoint;
                FlipModel(dir);

                // Start moving routine
                if (coroutine != null) StopCoroutine(coroutine);
                coroutine = StartCoroutine(Move(startPoint, endPoint, moveSpeed));
            }
            else
            {
                // Stop proper animation
                modelAnimator.Play("Idle");

                // Done :)
            }
        }
    }

    private void WarpEntity(Entity entity)
    {
        if (this.entity == entity)
        {
            // Get world location
            Vector3 newLocation = RoomUI.instance.floorTilemap.GetCellCenterWorld(entity.location);

            // Start routine
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(Warp(newLocation, warpSpeed));
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

    private IEnumerator Warp(Vector3 newLocation, float duration)
    {
        // Start particles
        warpGenerateParticles.Play();

        // Wait
        yield return new WaitForSeconds(duration);

        // Spawn dust
        warpDustParticles.Play();

        // Stop particles
        warpGenerateParticles.Stop();

        yield return null;

        // Move transform
        transform.position = newLocation;
    }

    private void FlipModel(Vector3 direction)
    {
        // If you are moving right and facing left, then flip
        if (direction.x > 0 && !isFacingRight)
        {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        // Else if you are moving left and facing right, also flip
        else if (direction.x < 0 && isFacingRight)
        {
            isFacingRight = false;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private void TakeDamage(Entity entity, int damage)
    {
        // If this entity took damage
        if (this.entity == entity && damage > 0)
        {
            // Play animation
            modelAnimator.Play("Hurt");

            // Display damage flash
            if (GameManager.instance.isHitFlash)
                damageFlash.Flash();

            // Spawn particle
            if (GameManager.instance.isHitEffect && entity.hitEffectPrefab != null)
            {
                Instantiate(entity.hitEffectPrefab, transform.position, transform.rotation);
            }
        }
    }

    private void MeleeAttack(Entity entity)
    {
        // If this entity attacked
        if (this.entity == entity)
        {
            // Play weapon animation
            weaponAnimator.Play("Attack");

            // Spawn particle
            if (GameManager.instance.isSlashEffect && entity.weapon.attackParticlePrefab != null)
            {
                Instantiate(entity.weapon.attackParticlePrefab, transform.position, transform.rotation);
            }

            // Shake screen
            if (GameManager.instance.isScreenShake)
                CameraShake.instance.ScreenShake(0.15f);

            // Hit freeze
            if (GameManager.instance.isHitFreeze)
                HitFreeze.instance.StartHitFreeze(0.1f);
        }
    }

    private void ReadyWeapon(Entity entity, bool state)
    {
        if (this.entity == entity)
        {
            // Play proper animation
            if (state) weaponAnimator.Play("Active");
            else weaponAnimator.Play("Idle");
        }
    }
}
