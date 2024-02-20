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

    private void Start()
    {
        // Initialize slot
        blacksmithSlot.Initialize(null);

        weaponUpgradesLayout.gameObject.SetActive(false);

        isOpen = false;

        GameEvents.instance.onOpenBlacksmith += Open;
        GameEvents.instance.onInsertBlacksmith += DisplayWeapon;
        GameEvents.instance.onRemoveBlacksmith += RemoveWeapon;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onOpenBlacksmith -= Open;
        GameEvents.instance.onInsertBlacksmith -= DisplayWeapon;
        GameEvents.instance.onRemoveBlacksmith -= RemoveWeapon;
    }

    public void Open(Entity entity)
    {
        // Make sure it is closed
        if (!isOpen)
        {
            this.customer = entity;

            // Allow visibiltiy and interaction
            canvasGroup.alpha = 1f;
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;

            isOpen = true;
        }
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

        instructionsLabel.text = "Insert an item to upgrade it.";

        // Update Size
        StartCoroutine(DelayedRebuild());
    }

    private IEnumerator DelayedRebuild()
    {
        yield return new WaitForEndOfFrame();

        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>());
        LayoutRebuilder.ForceRebuildLayoutImmediate(weaponUpgradesLayout.GetComponent<RectTransform>());
    }

    public void Close()
    {
        // Make sure it is open
        if (isOpen)
        {
            // Prevent visibiltiy and interaction
            canvasGroup.alpha = 0f;
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;

            // Remove weapon
            RemoveWeapon(null);

            // Trigger event
            GameEvents.instance.TriggerOnCloseBlacksmith(customer);

            isOpen = false;
        }
    }
}
