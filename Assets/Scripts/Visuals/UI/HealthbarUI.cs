using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthbarUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private LayoutGroup heartsLayoutGroup;

    [Header("Data")]
    [SerializeField] private GameObject heartUIPrefab;
    [SerializeField] private List<HeartUI> heartUIs;
    [SerializeField] private Entity entity;

    private void Awake()
    {
        heartUIs = new List<HeartUI>();
    }

    private void OnDestroy()
    {
        Uninitialize();
    }

    public void Initialize(Entity entity)
    {
        this.entity = entity;

        for (int i = 0; i < entity.maxHealth; i++)
        {
            // Create UI
            var heartUI = Instantiate(heartUIPrefab, heartsLayoutGroup.transform).GetComponent<HeartUI>();
            // Intialize it
            heartUI.Initialize(i >= entity.currentHealth);
            // Save it
            heartUIs.Add(heartUI);
        }

        // Sub to events
        GameEvents.instance.onEntityTakeDamage += UpdateHealth;
    }

    public void Uninitialize()
    {
        this.entity = null;

        // Destroy hearts
        for (int i = 0; i < heartUIs.Count; i++)
        {
            Destroy(heartUIs[i].gameObject);
        }
        heartUIs.Clear();

        // Unsub
        GameEvents.instance.onEntityTakeDamage -= UpdateHealth;
    }

    public void UpdateHealth(Entity entity, int amount)
    {
        // Warning, does not handle increasing max health!
        if (this.entity == entity)
        {
            // Handle damage
            if (amount > 0)
            {
                for (int i = 0; i < amount; i++)
                {
                    // Set the heart to empty
                    heartUIs[entity.currentHealth + i].Deplete();
                }
            }
            else if (amount < 0)
            {
                // Handle healing
                for (int i = 0; i < -amount; i++)
                {
                    // Set the heart to full
                    heartUIs[entity.currentHealth - i - 1].Restore();
                }
            }

        }
    }
}
