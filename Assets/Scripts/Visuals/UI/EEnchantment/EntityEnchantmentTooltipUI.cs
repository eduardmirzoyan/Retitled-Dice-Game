using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EntityEnchantmentTooltipUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private TextMeshProUGUI nameLabel;
    [SerializeField] private TextMeshProUGUI rarityLabel;
    [SerializeField] private TextMeshProUGUI descriptionLabel;

    public static EntityEnchantmentTooltipUI instance;
    private void Awake()
    {
        // Singleton logic
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
    }

    public void Show(EntityEnchantment enchantment, Vector3 position)
    {
        nameLabel.text = enchantment.name;
        rarityLabel.text = $"{enchantment.rarity} Enchantment";
        rarityLabel.color = ResourceMananger.instance.GetColor(enchantment.rarity);
        descriptionLabel.text = enchantment.description;

        // Show
        canvasGroup.alpha = 1f;

        // Relocate
        transform.position = position;

        // Update Canvas
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
    }

    public void Hide()
    {
        // Show
        canvasGroup.alpha = 0f;
    }
}
