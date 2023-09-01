using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(DamageFlash))]
public class EntityModel : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer modelSpriteRenderer;
    [SerializeField] private Animator modelAnimator;

    [Header("Offhand Weapon")]
    [SerializeField] private Transform mainWeaponHolder;

    [Header("Mainhand Weapon")]
    [SerializeField] private Transform offWeaponHolder;

    [Header("UI")]
    [SerializeField] private DamageFlash damageFlash;
    [SerializeField] private ProperLayerSort properLayerSort;

    [Header("Particles")]
    [SerializeField] private ParticleSystem warpGenerateParticles;
    [SerializeField] private ParticleSystem warpDustParticles;
    [SerializeField] private GameObject deathCloud;

    [Header("Data")]
    [SerializeField] private Entity entity;
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float footstepSfxSpeed = 0.25f;

    private RoomUI roomUI;
    private Coroutine coroutine;

    private void Awake()
    {
        properLayerSort = GetComponentInChildren<ProperLayerSort>();
    }

    public void Initialize(Entity entity, RoomUI roomUI)
    {
        this.entity = entity;
        this.roomUI = roomUI;

        // Set up model
        modelSpriteRenderer.sprite = entity.modelSprite;
        modelAnimator.runtimeAnimatorController = entity.modelController;

        // Update sorting layer once
        properLayerSort.UpdateLayer();

        // Spawn weapon models
        SpawnWeapon(entity, entity.weapons[0], 0);
        SpawnWeapon(entity, entity.weapons[1], 1);

        // Sub to events
        GameEvents.instance.onEquipWeapon += SpawnWeapon;

        GameEvents.instance.onEntityMoveStart += StartMove;
        GameEvents.instance.onEntityMove += MoveEntity;
        GameEvents.instance.onEntityMoveStop += StopMove;

        GameEvents.instance.onEntityWarp += WarpEntity;
        GameEvents.instance.onEntityJump += JumpEntity;
        GameEvents.instance.onEntityTakeDamage += TakeDamage;
        GameEvents.instance.onEntityDrawWeapon += FaceDirection;
        GameEvents.instance.onEntityDespawn += Despawn;

        // Set name
        transform.name = entity.name + " Model";
    }

    private void OnDestroy()
    {
        // Unsub to events
        GameEvents.instance.onEquipWeapon -= SpawnWeapon;

        GameEvents.instance.onEntityMoveStart -= StartMove;
        GameEvents.instance.onEntityMove -= MoveEntity;
        GameEvents.instance.onEntityMoveStop -= StopMove;

        GameEvents.instance.onEntityWarp -= WarpEntity;
        GameEvents.instance.onEntityTakeDamage -= TakeDamage;
        GameEvents.instance.onEntityDrawWeapon -= FaceDirection;
        GameEvents.instance.onEntityDespawn -= Despawn;
    }

    private void Despawn(Entity entity)
    {
        if (this.entity == entity)
        {
            // Play sound
            AudioManager.instance.PlaySFX("death");

            // Spawn death cloud 
            Instantiate(deathCloud, transform.position, Quaternion.identity);

            // Destroy self
            Destroy(gameObject);
        }
    }

    private void SpawnWeapon(Entity entity, Weapon weapon, int index)
    {
        if (this.entity == entity && weapon != null)
        {
            if (index == 0)
            {
                // Create and Initalize weapon
                Instantiate(weapon.weaponPrefab, mainWeaponHolder).GetComponent<WeaponModel>().Initialize(weapon);
            }
            else if (index == 1)
            {
                // Create and Initalize weapon
                Instantiate(weapon.weaponPrefab, offWeaponHolder).GetComponent<WeaponModel>().Initialize(weapon);
            }

        }
    }

    private void StartMove(Entity entity)
    {
        if (this.entity == entity)
        {
            // Play proper animation
            modelAnimator.Play("Run");

            // Start layering
            properLayerSort.SetActive(true);

            // Play sound
            InvokeRepeating("FootstepsSFX", 0f, footstepSfxSpeed);
        }
    }

    private void MoveEntity(Entity entity)
    {
        // If this entity moved, then move it
        if (this.entity == entity)
        {
            Vector3 currentPosition = transform.position;
            Vector3 newPosition = roomUI.GetLocationCenter(entity.location);

            // Flip model if needed
            FlipModel(newPosition - currentPosition);

            // Start moving routine
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(Move(currentPosition, newPosition));
        }
    }

    private void StopMove(Entity entity)
    {
        if (this.entity == entity)
        {
            // Stop proper animation
            modelAnimator.Play("Idle");

            // Stop layering
            properLayerSort.SetActive(false);

            // Stop sound
            CancelInvoke("FootstepsSFX");
        }
    }

    private void FootstepsSFX()
    {
        AudioManager.instance.PlaySFX("footstep");
    }

    private void WarpEntity(Entity entity)
    {
        if (this.entity == entity)
        {
            // Get world location
            Vector3 newLocation = roomUI.GetLocationCenter(entity.location);

            // Start routine
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(Warp(newLocation));

            // Play sound
            AudioManager.instance.PlaySFX("teleport");
        }
    }

    private void JumpEntity(Entity entity)
    {
        if (this.entity == entity)
        {
            Vector3 currentPosition = transform.position;
            Vector3 newPosition = roomUI.GetLocationCenter(entity.location);

            // Start routine
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(Jump(currentPosition, newPosition));

            // Play sound
            AudioManager.instance.PlaySFX("jump");
        }
    }

    private IEnumerator Move(Vector3 startPoint, Vector3 endPoint)
    {
        // Start timer
        Vector3 position;
        float elapsed = 0;
        float duration = GameManager.instance.gameSettings.moveBufferTime;

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

    private IEnumerator Warp(Vector3 newLocation)
    {
        // Start particles
        warpGenerateParticles.Play();

        // Wait
        yield return new WaitForSeconds(GameManager.instance.gameSettings.warpBufferTime);

        // Spawn dust
        warpDustParticles.Play();

        // Stop particles
        warpGenerateParticles.Stop();

        yield return null;

        // Move transform
        transform.position = newLocation;
    }

    private IEnumerator Jump(Vector3 startPoint, Vector3 endPoint)
    {
        // Start timer
        Vector3 position;
        float elapsed = 0;
        float duration = GameManager.instance.gameSettings.jumpBufferTime;

        Vector3 control = (endPoint + startPoint) / 2 + Vector3.up * jumpHeight;
        while (elapsed < duration)
        {
            // Projectile motion
            float ratio = elapsed / duration;
            Vector3 ac = Vector3.Lerp(startPoint, control, ratio);
            Vector3 cb = Vector3.Lerp(control, endPoint, ratio);

            position = Vector3.Lerp(ac, cb, ratio);
            transform.position = position;

            // Increment time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set to final destination
        transform.position = endPoint;
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
            // Display damage flash
            if (GameManager.instance.gameSettings.useHitFlash)
                damageFlash.Flash();

            // Spawn particle
            if (entity.hitEffectPrefab != null)
            {
                Instantiate(entity.hitEffectPrefab, transform.position, transform.rotation);
            }

            // Shake screen
            if (GameManager.instance.gameSettings.useScreenShake)
                CameraShake.instance.ScreenShake(0.15f);

        }
    }

    private void FaceDirection(Entity entity, Vector3 direction, Weapon weapon)
    {
        if (this.entity == entity)
        {
            // Face direction of attack
            FlipModel(direction);
        }
    }
}
