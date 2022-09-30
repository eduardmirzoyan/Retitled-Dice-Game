using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform containerTransform;
    [SerializeField] private List<Image> heartImages;

    [Header("Data")]
    [SerializeField] private GameObject heartPrefab;
    [SerializeField] private Sprite fullHeart;
    [SerializeField] private Sprite emptyHeart;

    private void Awake() {
        heartImages = new List<Image>();
    }

    private void Start() {
        // Sub to events
        GameEvents.instance.onEnterFloor += InitializeHearts;
        GameEvents.instance.onEntityTakeDamage += ChangeHearts;
    }

    private void OnDestroy() {
        // Unsub to events
        GameEvents.instance.onEnterFloor -= InitializeHearts;
        GameEvents.instance.onEntityTakeDamage -= ChangeHearts;
    }

    private void InitializeHearts(Dungeon dungeon) {
        // Spawn empty containers
        for (int i = 0; i < dungeon.player.maxHealth; i++)
        {
            var image = Instantiate(heartPrefab, containerTransform).GetComponent<Image>();
            heartImages.Add(image);

            if (i < dungeon.player.currentHealth) {
                image.sprite = fullHeart;
            }
            else {
                image.sprite = emptyHeart;
            }
        }
    }

    private void ChangeHearts(Entity attacker, Entity target, int damage) 
    {
        // If player was attacked
        if (target is Player) {
            // Update UI?
            for (int i = 0; i < heartImages.Count; i++)
            {
                if (i < target.currentHealth)
                {
                    heartImages[i].sprite = fullHeart;
                }
                else
                {
                    heartImages[i].sprite = emptyHeart;
                }
            }
        }
    }
}
