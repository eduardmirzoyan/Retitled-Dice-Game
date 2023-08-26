using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModel : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Animator animator;

    [Header("Data")]
    [SerializeField] private Weapon weapon;

    private void Awake()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
    }

    public void Initialize(Weapon weapon)
    {
        this.weapon = weapon;

        // Update components
        spriteRenderer.sprite = weapon.sprite;

        spriteRenderer.sortingLayerName = "Entities";

        // Set name
        transform.name = weapon.name + " Model";

        // Sub to events
        GameEvents.instance.onEntityDrawWeapon += DrawWeapon;
        GameEvents.instance.onEntityUseWeapon += UseWeapon;
        GameEvents.instance.onEntitySheatheWeapon += SheatheWeapon;
        GameEvents.instance.onUnequipMainhand += Despawn;
        GameEvents.instance.onUnequipOffhand += Despawn;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onEntityDrawWeapon -= DrawWeapon;
        GameEvents.instance.onEntityUseWeapon -= UseWeapon;
        GameEvents.instance.onEntitySheatheWeapon -= SheatheWeapon;
        GameEvents.instance.onUnequipMainhand -= Despawn;
        GameEvents.instance.onUnequipOffhand -= Despawn;
    }

    private void Despawn(Entity entity, Weapon weapon)
    {
        print("Hit");

        // If this weapon was un-equipped
        if (this.weapon == weapon)
        {
            // Destroy this object
            Destroy(gameObject);
        }
    }

    private void DrawWeapon(Entity entity, Vector3 direction, Weapon weapon)
    {
        // Holder z = 90, attack up
        // Holder z = 0, attack facing direction
        // Holder z = -90, attack down

        // If this weapon was drawn
        if (this.weapon == weapon)
        {
            // Change attack orientation based which direction you are attacking
            // If attacking upward
            if (direction.y > 0)
            {
                transform.localEulerAngles = new Vector3(0, 0, 90);
            }
            // If attacking downward
            else if (direction.y < 0)
            {
                transform.localEulerAngles = new Vector3(0, 0, -90);
            }
            // Else set to facing direction
            else
            {
                transform.localEulerAngles = new Vector3(0, 0, 0);
            }

            // Play animation
            animator.Play("Draw");

            // Move weapon up in sorting
            spriteRenderer.sortingLayerName = "Weapons";
        }
    }

    private void UseWeapon(Entity entity, Weapon weapon, Vector3Int direction)
    {
        if (this.weapon == weapon)
        {
            // Play animation
            animator.Play("Attack");

            // Spawn particle in the same orientation as the weapon
            if (GameManager.instance.gameSettings.useAttackEffect && weapon.attackParticlePrefab != null)
            {
                Instantiate(weapon.attackParticlePrefab, transform.position + transform.right, transform.rotation);
            }
        }
    }

    private void SheatheWeapon(Entity entity, Weapon weapon)
    {
        if (this.weapon == weapon)
        {
            // Play animation
            animator.Play("Sheathe");

            // Move weapon down in sorting
            spriteRenderer.sortingLayerName = "Entities";

            // Reset rotation
            transform.localEulerAngles = Vector3.zero;
        }
    }
}
