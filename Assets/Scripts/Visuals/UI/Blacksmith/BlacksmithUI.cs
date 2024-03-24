using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BlacksmithUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private BlacksmithSlot blacksmithSlot;
    [SerializeField] private LayoutGroup weaponUpgradesLayout;
    [SerializeField] private TextMeshProUGUI instructionsLabel;
    [SerializeField] private GameObject weaponUpgradePrefab;
    [SerializeField] private GameObject titleObject;


    [Header("Dynamic Data")]
    [SerializeField] private Entity customer;
    [SerializeField] private bool isOpen;

    [Header("Debug")]
    [SerializeField] private bool requestClose;

    public static BlacksmithUI instance;
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
        // Initialize slot
        blacksmithSlot.Initialize(null);

        weaponUpgradesLayout.gameObject.SetActive(false);

        GameEvents.instance.onInsertBlacksmith += DisplayWeapon;
        GameEvents.instance.onRemoveBlacksmith += RemoveWeapon;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onInsertBlacksmith -= DisplayWeapon;
        GameEvents.instance.onRemoveBlacksmith -= RemoveWeapon;
    }

    public IEnumerator Browse(Entity customer)
    {
        // Allow moving of items while in menu
        GameEvents.instance.TriggerOnToggleAllowInventory(true);

        this.customer = customer;

        // Reset state
        requestClose = false;

        // Allow visibiltiy and interaction
        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;

        // Infinitely wait until player closes
        while (!requestClose)
            yield return null;

        // Prevent visibiltiy and interaction
        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;

        // Remove weapon
        RemoveWeapon(null);

        this.customer = null;

        // Disallow items again
        GameEvents.instance.TriggerOnToggleAllowInventory(false);
    }

    public void Close()
    {
        requestClose = true;
    }

    private void DisplayWeapon(Weapon weapon)
    {
        // Show layout
        weaponUpgradesLayout.gameObject.SetActive(true);

        var upgradeUI = Instantiate(weaponUpgradePrefab, weaponUpgradesLayout.transform).GetComponent<WeaponUpgradeUI>();
        upgradeUI.Initialize(weapon, customer);

        // Hide texts
        titleObject.SetActive(false);

        instructionsLabel.text = "[Right Click] to remove item.";

        // Update UI
        LayoutRebuilder.ForceRebuildLayoutImmediate(weaponUpgradesLayout.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
    }

    private void RemoveWeapon(Weapon weapon)
    {
        // Hide layout
        weaponUpgradesLayout.gameObject.SetActive(false);

        // Show texts
        titleObject.SetActive(true);

        // Update instructions
        instructionsLabel.text = "Insert an item to upgrade it.";

        // Remove item from slot
        // blacksmithSlot.StoreItem(null); // FIXME

        // Update Size
        StartCoroutine(DelayedRebuild());
    }

    private IEnumerator DelayedRebuild()
    {
        yield return new WaitForEndOfFrame();

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(weaponUpgradesLayout.GetComponent<RectTransform>());
    }
}
