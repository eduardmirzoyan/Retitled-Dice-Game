using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantmentSelectionUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private List<EnityEnchantmentUI> enityEnchantments;

    private void Start()
    {
        GameEvents.instance.onPresentEnchantments += Open;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onPresentEnchantments -= Open;
    }

    private void Open(List<EntityEnchantment> enchantments)
    {
        if (enchantments == null)
            throw new System.Exception($"Input choices was null.");

        if (enchantments.Count != enityEnchantments.Count)
            throw new System.Exception($"There must be at least {enchantments.Count} choices.");


        for (int i = 0; i < enchantments.Count; i++)
        {
            enityEnchantments[i].Initialize(enchantments[i]);
        }

        // Show 
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Close()
    {
        // Hide
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
