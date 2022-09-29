using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityModel : MonoBehaviour
{
    public static float moveSpeed = 0.35f;

    [Header("Components")]
    [SerializeField] private Animator animator;

    [Header("Data")]
    [SerializeField] private Entity entity;
    [SerializeField] private bool isFacingRight = true;

    private Coroutine coroutine;

    private void Awake() {
        animator = GetComponentInChildren<Animator>();
        animator.Play("Idle");
    }

    public void Initialize(Entity entity) {
        this.entity = entity;

        // Sub to events
        GameEvents.instance.onEntityMove += MoveEntity;
    }

    public void Uninitialize() {
        // Unsub to events
        GameEvents.instance.onEntityMove -= MoveEntity;
    }

    private void OnDestroy() {
        Uninitialize();
    }

    private void MoveEntity(Entity entity, Vector3Int from, Vector3Int to) {
        // If this entity moved, then move it
        if (this.entity == entity) {
            // If from == to, this is a stop command
            if (from == to) {
                // Stop animation
                animator.Play("Idle");
                return;
            }

            if (coroutine != null) StopCoroutine(coroutine);

            // Get world positions
            Vector3 startPoint = DungeonUI.instance.floorTilemap.GetCellCenterWorld(from);
            Vector3 endPoint = DungeonUI.instance.floorTilemap.GetCellCenterWorld(to);

            // Check if sprite should be flipped
            Vector3 dir = to - from;
            FlipSprite(dir);

            // Start animation
            animator.Play("Run");

            // Start moving routine
            coroutine = StartCoroutine(Move(startPoint, endPoint, moveSpeed));
        }
    }

    private void FlipSprite(Vector3 direction) {
        // If you are moving right and facing left, then flip
        if (direction.x > 0 && !isFacingRight) {
            isFacingRight = true;
            transform.Rotate(0f, 180f, 0f);
        }
        // Else if you are moving left and facing right, also flip
        else if (direction.x < 0 && isFacingRight) {
            isFacingRight = false;
            transform.Rotate(0f, 180f, 0f);
        }
    }

    private IEnumerator Move(Vector3 startPoint, Vector3 endPoint, float duration) {
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
}
