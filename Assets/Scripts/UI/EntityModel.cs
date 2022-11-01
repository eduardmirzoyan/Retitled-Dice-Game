using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(DamageFlash))]
public class EntityModel : MonoBehaviour
{
    public static float moveSpeed = 0.35f;
    public static float warpSpeed = 0.5f;

    [SerializeField] private Transform offsetTransform;

    [Header("Model")]
    [SerializeField] private SpriteRenderer modelSpriteRenderer;
    [SerializeField] private Animator modelAnimator;

    [Header("Offahnd Weapon")]
    [SerializeField] private SpriteRenderer mainWeaponSpriteRenderer;
    [SerializeField] private Animator mainWeaponAnimator;
    [SerializeField] private Transform mainWeaponHolder;

    [Header("Mainhand Weapon")]
    [SerializeField] private SpriteRenderer offWeaponSpriteRenderer;
    [SerializeField] private Animator offWeaponAnimator;
    [SerializeField] private Transform offWeaponHolder;

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
    [SerializeField] private GameObject projectilePrefab;

    private RoomUI roomUI;
    private Coroutine coroutine;

    public void Initialize(Entity entity, RoomUI roomUI)
    {
        this.entity = entity;
        this.roomUI = roomUI;

        // Set up model
        modelSpriteRenderer.sprite = entity.modelSprite;
        modelAnimator.runtimeAnimatorController = entity.modelController;

        // Set up main weapon
        if (entity.mainWeapon != null)
        {
            mainWeaponSpriteRenderer.sprite = entity.mainWeapon.sprite;
            mainWeaponAnimator.runtimeAnimatorController = entity.mainWeapon.controller;
        }
        else
        {
            mainWeaponSpriteRenderer.enabled = false;
        }

        // Set up off weapon
        if (entity.offWeapon != null)
        {
            offWeaponSpriteRenderer.sprite = entity.offWeapon.sprite;
            offWeaponAnimator.runtimeAnimatorController = entity.offWeapon.controller;
        }
        else
        {
            offWeaponSpriteRenderer.enabled = false;
        }

        // If entity is an AI, display it's first die
        if (entity.AI != null)
        {
            dieUI.Initialize(entity.GetActions()[0], false);
        }
        else
        {
            dieUI.gameObject.SetActive(false);
        }

        // Apply offset
        offsetTransform.localPosition = entity.offsetDueToSize;

        // Sub to events
        GameEvents.instance.onEntityStartMove += StartMove;
        GameEvents.instance.onEntityMove += MoveEntity;
        GameEvents.instance.onEntityStopMove += StopMove;

        GameEvents.instance.onEntityWarp += WarpEntity;
        GameEvents.instance.onEntityTakeDamage += TakeDamage;
        GameEvents.instance.onEntityMeleeAttack += MeleeAttack;
        // GameEvents.instance.onEntityRangedAttack += RangedAttack;
        GameEvents.instance.onEntityRangedAttackTimed += RangedAttackTimed;

        GameEvents.instance.onEntityDrawWeapon += DrawWeapon;
        GameEvents.instance.onEntitySheatheWeapon += SheatheWeapon;
        GameEvents.instance.onRemoveEntity += RemoveEntity;
    }

    public void Uninitialize()
    {
        // Unsub to events
        GameEvents.instance.onEntityStartMove -= StartMove;
        GameEvents.instance.onEntityMove -= MoveEntity;
        GameEvents.instance.onEntityStopMove -= StopMove;

        GameEvents.instance.onEntityWarp -= WarpEntity;
        GameEvents.instance.onEntityTakeDamage -= TakeDamage;
        GameEvents.instance.onEntityMeleeAttack -= MeleeAttack;
        //GameEvents.instance.onEntityRangedAttack -= RangedAttack;
        GameEvents.instance.onEntityRangedAttackTimed -= RangedAttackTimed;

        GameEvents.instance.onEntityDrawWeapon -= DrawWeapon;
        GameEvents.instance.onEntitySheatheWeapon -= SheatheWeapon;
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

    private void StartMove(Entity entity, Vector3Int direction)
    {
        if (this.entity == entity)
        {
            // Play proper animation
            modelAnimator.Play("Run");

            // Flip model if needed
            FlipModel(direction);
        }
    }

    private void MoveEntity(Entity entity)
    {
        // If this entity moved, then move it
        if (this.entity == entity)
        {
            Vector3 currentPosition = transform.position;
            Vector3 newPosition = roomUI.GetLocationCenter(entity.location);

            // Start moving routine
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(Move(currentPosition, newPosition, moveSpeed));
        }
    }

    private void StopMove(Entity entity)
    {
        if (this.entity == entity)
        {
            // Stop proper animation
            modelAnimator.Play("Idle");
        }
    }

    private void WarpEntity(Entity entity)
    {
        if (this.entity == entity)
        {
            // Get world location
            Vector3 newLocation = roomUI.GetLocationCenter(entity.location);

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

            // Shake screen
            if (GameManager.instance.isScreenShake)
                CameraShake.instance.ScreenShake(0.15f);

            // Hit freeze
            if (GameManager.instance.isHitFreeze)
                HitFreeze.instance.StartHitFreeze(0.1f);
        }
    }

    private void DrawWeapon(Entity entity, Vector3 direction, Weapon weapon)
    {
        if (this.entity == entity)
        {
            // Debug
            print("Attacking dir: " + direction);

            // Flip model if needed
            FlipModel(direction);

            // Holder z = 90, attack up
            // Holder z = 0, attack facing direction
            // Holder z = -90, attack down

            // Play proper animation
            if (weapon.controller == mainWeaponAnimator.runtimeAnimatorController)
            {
                // Change attack orientation based which direction you are attacking
                // If attacking upward
                if (direction.y > 0)
                {
                    mainWeaponHolder.localEulerAngles = new Vector3(0, 0, 90);
                }
                // If attacking downward
                else if (direction.y < 0)
                {
                    mainWeaponHolder.localEulerAngles = new Vector3(0, 0, -90);
                }
                // Else set to facing direction
                else
                {
                    mainWeaponHolder.localEulerAngles = new Vector3(0, 0, 0);
                }

                // Randomly select a starting position
                if (Random.Range(0, 1) == 1)
                {
                    mainWeaponAnimator.Play("Attack 2");
                }
                else
                {
                    mainWeaponAnimator.Play("Attack 3");
                }
            }
            else if (weapon.controller == offWeaponAnimator.runtimeAnimatorController)
            {
                // If attacking upward
                if (direction.y > 0)
                {
                    offWeaponHolder.localEulerAngles = new Vector3(0, 0, 90);
                }
                // If attacking downward
                else if (direction.y < 0)
                {
                    offWeaponHolder.localEulerAngles = new Vector3(0, 0, -90);
                }
                // Else set to facing direction
                else
                {
                    offWeaponHolder.localEulerAngles = new Vector3(0, 0, 0);
                }

                // Randomly select a starting position
                if (Random.Range(0, 1) == 1)
                {
                    offWeaponAnimator.Play("Attack 2");
                }
                else
                {
                    offWeaponAnimator.Play("Attack 3");
                }
            }
        }
    }

    private void MeleeAttack(Entity entity, Weapon weapon)
    {
        // If this entity attacked
        if (this.entity == entity)
        {
            // If attacking with primary weapon
            if (weapon.controller == mainWeaponAnimator.runtimeAnimatorController)
            {
                // Flip to other end position
                mainWeaponAnimator.SetTrigger("Attack");

                // Spawn particle in the same orientation as your attacking weapon
                if (GameManager.instance.isSlashEffect && entity.mainWeapon.attackParticlePrefab != null)
                {
                    Instantiate(weapon.attackParticlePrefab, transform.position, mainWeaponHolder.rotation);
                }
            }
            // Or attacking with secondary
            else if (weapon.controller == offWeaponAnimator.runtimeAnimatorController)
            {
                // Flip to other end position
                offWeaponAnimator.SetTrigger("Attack");

                // Spawn particle in the same orientation as your attacking weapon
                if (GameManager.instance.isSlashEffect && entity.mainWeapon.attackParticlePrefab != null)
                {
                    Instantiate(weapon.attackParticlePrefab, transform.position, offWeaponHolder.rotation);
                }
            }
        }
    }

    private void RangedAttack(Entity entity, Vector3Int targetLocation, Weapon weapon)
    {
        if (this.entity == entity)
        {
            var targetWorld = roomUI.GetLocationCenter(targetLocation);

            // If attacking with primary weapon
            if (weapon.controller == mainWeaponAnimator.runtimeAnimatorController)
            {
                // Spawn projectile
                var projectile = Instantiate(projectilePrefab, transform.position, mainWeaponHolder.rotation).GetComponent<Projectile>();
                projectile.Initialize(targetWorld, 25f, weapon);
            }
            // Or attacking with secondary
            else if (weapon.controller == offWeaponAnimator.runtimeAnimatorController)
            {
                // Spawn projectile
                var projectile = Instantiate(projectilePrefab, transform.position, offWeaponHolder.rotation).GetComponent<Projectile>();
                projectile.Initialize(targetWorld, 25f, weapon);
            }
        }
    }

    private void RangedAttackTimed(Entity entity, Vector3Int targetLocation, Weapon weapon, ActionInfo info)
    {
        if (this.entity == entity)
        {
            var targetWorld = roomUI.GetLocationCenter(targetLocation);

            // If attacking with primary weapon
            if (weapon.controller == mainWeaponAnimator.runtimeAnimatorController)
            {
                // Spawn projectile
                var projectile = Instantiate(projectilePrefab, transform.position, mainWeaponHolder.rotation).GetComponent<Projectile>();
                // Wait for projectile to travel
                var travelTime = projectile.Initialize(targetWorld, 25f, weapon);
                // Set action time
                info.waitTime = travelTime;
            }
            // Or attacking with secondary
            else if (weapon.controller == offWeaponAnimator.runtimeAnimatorController)
            {
                // Spawn projectile
                var projectile = Instantiate(projectilePrefab, transform.position, offWeaponHolder.rotation).GetComponent<Projectile>();
                // Wait for projectile to travel
                var travelTime = projectile.Initialize(targetWorld, 25f, weapon);
                // Set action time
                info.waitTime = travelTime;
            }
            else
            {
                // Don't do anything
                Debug.Log("Unknown weapon chosen: " + weapon.name);
                // Set action time
                info.waitTime = 0f;
            }

            // Sheathe weapon
            SheatheWeapon(entity, weapon);
        }
    }

    private void SheatheWeapon(Entity entity, Weapon weapon)
    {
        // Play proper animation
        if (weapon.controller == mainWeaponAnimator.runtimeAnimatorController)
        {
            mainWeaponAnimator.Play("Idle");
            // Reset rotation
            mainWeaponHolder.localEulerAngles = Vector3.zero;
        }
        else if (weapon.controller == offWeaponAnimator.runtimeAnimatorController)
        {
            offWeaponAnimator.Play("Idle");
            // Reset rotation
            offWeaponHolder.localEulerAngles = Vector3.zero;
        }
    }
}
