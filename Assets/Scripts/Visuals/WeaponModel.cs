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

    private float animationLength = 0.5f;

    public void Initialize(Weapon weapon)
    {
        this.weapon = weapon;
        weapon.model = this;
        transform.name = weapon.name + " Weapon";

        // Set components
        spriteRenderer.sprite = weapon.sprite;
        spriteRenderer.sortingLayerName = "Entities";
        animator.runtimeAnimatorController = weapon.controller;

        animator.Play("Idle");
    }

    public void Uninitialize()
    {
        spriteRenderer.sprite = null;
        animator.runtimeAnimatorController = null;

        weapon = null;
        transform.name = "Unused Weapon";
    }

    public IEnumerator Draw(Vector3 direction)
    {
        // Holder z = 90, attack up
        // Holder z = 0, attack facing direction
        // Holder z = -90, attack down

        // print("draw");

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

        // Play sound
        AudioManager.instance.PlaySFX("draw");

        // Wait for animation
        yield return new WaitForSeconds(animationLength);
    }

    public IEnumerator Attack()
    {
        // Play animation
        animator.Play("Attack");

        // Play sfx?
        // TODO

        // Spawn particle in the same orientation as the weapon
        if (weapon.attackParticlePrefab != null)
            Instantiate(weapon.attackParticlePrefab, transform.position + transform.right, transform.rotation);

        // Give small delay
        yield return new WaitForSeconds(0.25f);
    }

    public IEnumerator Sheathe()
    {
        // Play animation
        animator.Play("Sheathe");

        // Move weapon down in sorting
        spriteRenderer.sortingLayerName = "Entities";

        // Reset rotation
        transform.localEulerAngles = Vector3.zero;

        yield return new WaitForSeconds(animationLength);
    }
}
