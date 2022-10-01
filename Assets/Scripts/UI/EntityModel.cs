using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DamageFlash))]
public class EntityModel : MonoBehaviour
{
    public static float moveSpeed = 0.35f;

    [Header("Components")]
    [SerializeField] private SpriteRenderer modelSpriteRenderer;
    [SerializeField] private SpriteRenderer weaponSpriteRenderer;
    [SerializeField] private Animator modelAnimator;
    [SerializeField] private Animator weaponAnimator;
    [SerializeField] private DamageFlash damageFlash;

    [Header("Data")]
    [SerializeField] private Entity entity;
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private GameObject deathCloud;

    private Coroutine coroutine;

    private void Awake()
    {
        damageFlash = GetComponent<DamageFlash>();
    }

    public void Initialize(Entity entity)
    {
        this.entity = entity;

        // Set model sprite
        modelSpriteRenderer.sprite = entity.sprite;

        // Set weapon sprite
        if (entity.weapon != null)
        {
            weaponSpriteRenderer.sprite = entity.weapon.sprite;
        }

        // Sub to events
        GameEvents.instance.onEntityMove += MoveEntity;
        GameEvents.instance.onEntityTakeDamage += TakeDamage;
        GameEvents.instance.onEntityMeleeAttack += MeleeAttack;
        GameEvents.instance.onEntityReadyWeapon += ReadyWeapon;
        GameEvents.instance.onRemoveEntity += RemoveEntity;
    }

    public void Uninitialize()
    {
        // Unsub to events
        GameEvents.instance.onEntityMove -= MoveEntity;
        GameEvents.instance.onEntityTakeDamage -= TakeDamage;
        GameEvents.instance.onEntityMeleeAttack -= MeleeAttack;
        GameEvents.instance.onEntityReadyWeapon -= ReadyWeapon;
        GameEvents.instance.onRemoveEntity -= RemoveEntity;
    }

    private void OnDestroy()
    {
        Uninitialize();
    }

    private void RemoveEntity(Entity entity) {
        if (this.entity == entity) {
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
                Vector3 startPoint = transform.position; // DungeonUI.instance.floorTilemap.GetCellCenterWorld(transform.position);
                Vector3 endPoint = DungeonUI.instance.floorTilemap.GetCellCenterWorld(entity.location);

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

    private IEnumerator Move(Vector3 startPoint, Vector3 endPoint, float duration)
    {
        // Start timer
        Vector3 position;
        float timer = 0;
        while (timer < duration)
        {
            // Lerp model position
            position = Vector3.Lerp(startPoint, endPoint, timer / duration);
            transform.position = position;

            // Increment time
            timer += Time.deltaTime;
            yield return null;
        }

        // Set to final destination
        transform.position = endPoint;
    }

    private void TakeDamage(Entity entity, int damage)
    {
        // If this entity took damage
        if (this.entity == entity)
        {
            // Display damage flash
            damageFlash.Flash();
        }
    }

    private void MeleeAttack(Entity entity)
    {
        // If this entity attacked
        if (this.entity == entity)
        {
            // Play weapon animation
            weaponAnimator.Play("Attack");

            // Shake screen
            CameraShake.instance.ScreenShake(0.15f);

            // Hit freeze
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
