using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(DamageFlash))]
public class EntityModel : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer modelSpriteRenderer;
    [SerializeField] private Animator modelAnimator;
    [SerializeField] private Animator dangerAnimator;

    [Header("Offhand Weapon")]
    [SerializeField] private Transform mainWeaponHolder;

    [Header("Mainhand Weapon")]
    [SerializeField] private Transform offWeaponHolder;

    [Header("QoL")]
    [SerializeField] private DamageFlash damageFlash;
    [SerializeField] private ProperLayerSort properLayerSort;

    [Header("Particles")]
    [SerializeField] private ParticleSystem warpGenerateParticles;
    [SerializeField] private ParticleSystem warpDustParticles;
    [SerializeField] private GameObject corpsePrefab;

    [Header("Data")]
    [SerializeField] private Entity entity;
    [SerializeField] private bool isFacingRight = true;
    [SerializeField] private float jumpHeight = 1f;
    [SerializeField] private float footstepSfxSpeed = 0.25f;

    private Coroutine coroutine;

    private void Awake()
    {
        properLayerSort = GetComponentInChildren<ProperLayerSort>();
    }

    public void Initialize(Entity entity)
    {
        this.entity = entity;

        // Set up model
        modelSpriteRenderer.sprite = entity.modelSprite;
        modelAnimator.runtimeAnimatorController = entity.modelController;
        properLayerSort.UpdateLayer();

        // Spawn weapon models
        SpawnWeapon(entity, entity.weapons[0], 0);
        SpawnWeapon(entity, entity.weapons[1], 1);

        // Sub to events
        GameEvents.instance.onEquipWeapon += SpawnWeapon;
        GameEvents.instance.onEntityDrawWeapon += FaceDirection;

        // Set name
        transform.name = $"{entity.name} Renderer";
    }

    private void OnDestroy()
    {
        // Unsub to events
        GameEvents.instance.onEquipWeapon -= SpawnWeapon;
        GameEvents.instance.onEntityDrawWeapon -= FaceDirection;
    }

    public void Uninitialize(Entity entity)
    {
        if (this.entity != entity)
            throw new System.Exception("Unknown entity was chosen to despawn.");

        // Play sound
        AudioManager.instance.PlaySFX("death");

        // BANDADE SOLUTION
        Vector2 fromLocation = entity.room.player.location + new Vector3(0.5f, 0.5f, 0);
        Vector2 direction = (Vector2)transform.position - fromLocation;
        direction.Normalize();

        // Spawn corpse FIXME
        if (entity.name != "Barrel")
            Instantiate(corpsePrefab, transform.position, Quaternion.identity).GetComponent<CorpseModel>().Initialize(entity, direction);
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

    public void MoveSetup()
    {
        modelAnimator.Play("Run");
        InvokeRepeating(nameof(FootstepsSFX), 0f, footstepSfxSpeed);
    }

    public IEnumerator Move(Vector3Int startLocation, Vector3Int endLocation)
    {
        Vector3 startPosition = RoomManager.instance.GetLocationCenter(startLocation);
        Vector3 endPosition = RoomManager.instance.GetLocationCenter(endLocation);

        FlipModel(endPosition - startPosition);

        Vector3 currentPosition;
        float elapsed = 0;
        float duration = GameManager.instance.gameSettings.moveBufferTime;
        while (elapsed < duration)
        {
            // Lerp model position
            currentPosition = Vector3.Lerp(startPosition, endPosition, elapsed / duration);
            transform.position = currentPosition;

            // Update layering
            properLayerSort.UpdateLayer();

            // Increment time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Cleanup
        transform.position = endPosition;

    }

    public void MoveCleanup()
    {
        modelAnimator.Play("Idle");
        CancelInvoke(nameof(FootstepsSFX));
    }

    public IEnumerator Jump(Vector3Int startLocation, Vector3Int endLocation)
    {
        // Play sound
        AudioManager.instance.PlaySFX("jump");

        Vector3 startPosition = RoomManager.instance.GetLocationCenter(startLocation);
        Vector3 endPosition = RoomManager.instance.GetLocationCenter(endLocation);

        float elapsed = 0;
        float duration = GameManager.instance.gameSettings.actionDuration;

        Vector3 currentPosition;
        Vector3 control = (startPosition + endPosition) / 2 + Vector3.up * jumpHeight;
        while (elapsed < duration)
        {
            // Projectile motion
            float ratio = elapsed / duration;
            Vector3 ac = Vector3.Lerp(startPosition, control, ratio);
            Vector3 cb = Vector3.Lerp(control, endPosition, ratio);

            currentPosition = Vector3.Lerp(ac, cb, ratio);
            transform.position = currentPosition;

            // Increment time
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Set to final destination
        transform.position = endPosition;
    }

    public IEnumerator Warp(Vector3Int _, Vector3Int endLocation)
    {
        Vector3 endPosition = RoomManager.instance.GetLocationCenter(endLocation);

        // Start particles
        warpGenerateParticles.Play();

        // Wait
        yield return new WaitForSeconds(GameManager.instance.gameSettings.actionDuration);

        // Spawn dust
        warpDustParticles.Play();

        // Stop particles
        warpGenerateParticles.Stop();

        // Move transform
        transform.position = endPosition;
    }

    public void TakeDamage(Entity entity, int damage)
    {
        // If this entity took damage
        if (damage > 0)
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

    public void SetDangerStatus(bool inDanger)
    {
        if (inDanger)
        {
            dangerAnimator.Play("Flash");
        }
        else
        {
            dangerAnimator.Play("Idle");
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

    // ~~~~~ HELPERS ~~~~~

    private void FootstepsSFX()
    {
        AudioManager.instance.PlaySFX("footstep");
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
}
