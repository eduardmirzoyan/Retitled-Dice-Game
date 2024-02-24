using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthbarUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Image fillImage;
    [SerializeField] private Image flashImage;
    [SerializeField] private TextMeshProUGUI healthLabel;

    [Header("Settings")]
    [SerializeField] private float flashDuration;

    [Header("Debug")]
    [SerializeField] private Entity entity;

    private void OnDestroy()
    {
        Uninitialize();
    }

    public void Initialize(Entity entity)
    {
        this.entity = entity;

        healthLabel.text = $"{entity.currentHealth}/{entity.maxHealth}";
        fillImage.fillAmount = (float)entity.currentHealth / entity.maxHealth;

        // Sub to events
        GameEvents.instance.onEntityTakeDamage += UpdateHealth;
    }

    public void Uninitialize()
    {
        this.entity = null;

        healthLabel.text = $"?/?";
        fillImage.fillAmount = 0f;

        // Unsub
        GameEvents.instance.onEntityTakeDamage -= UpdateHealth;
    }

    private void UpdateHealth(Entity entity, int _)
    {
        if (this.entity == entity)
        {
            healthLabel.text = $"{entity.currentHealth}/{entity.maxHealth}";
            fillImage.fillAmount = (float)entity.currentHealth / entity.maxHealth;

            StopAllCoroutines();
            StartCoroutine(Flash(flashDuration));
        }
    }

    private IEnumerator Flash(float duration)
    {
        flashImage.enabled = true;

        yield return new WaitForSeconds(duration);

        flashImage.enabled = false;
    }
}
