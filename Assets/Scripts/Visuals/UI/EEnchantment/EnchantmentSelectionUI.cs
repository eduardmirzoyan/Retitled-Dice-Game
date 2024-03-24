using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnchantmentSelectionUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private List<EnityEnchantmentUI> enityEnchantments;

    public static EnchantmentSelectionUI instance;
    private void Awake()
    {
        // Singleton Logic
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    private void Start()
    {
        GameEvents.instance.onEntityGainEnchantment += Close;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onEntityGainEnchantment -= Close;
    }

    public void Open(List<EntityEnchantment> enchantments)
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

    public void Close(Entity _, EntityEnchantment __)
    {
        Close();
    }

    public void Close()
    {
        // Hide
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
