using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemActionsTabUI : MonoBehaviour
{
    [Header("Componenets")]
    [SerializeField] private TextMeshProUGUI tabLabel;
    [SerializeField] private Image tabBackground;
    [SerializeField] private GameObject actionsHolder;
    [SerializeField] private GameObject slotsHolder;

    [Header("Data")]
    [SerializeField] private GameObject weaponActionPrefab;
    [SerializeField] private GameObject slotIconPrefab;

    [Header("Settings")]
    [SerializeField] private Color activeTabColor;
    [SerializeField] private Color inactiveTabColor;

    [Header("Debug")]
    [SerializeField] private List<WeaponActionUI> actionUIs;
    [SerializeField] private List<SlotIconUI> slotIconUIs;

    private void Awake()
    {
        actionUIs = new List<WeaponActionUI>();
        slotIconUIs = new List<SlotIconUI>();
    }

    public void Initialize(Weapon weapon)
    {
        foreach (var action in weapon.actions)
        {
            // Spawn UI
            var weaponAction = Instantiate(weaponActionPrefab, actionsHolder.transform).GetComponent<WeaponActionUI>();
            weaponAction.Initialize(action);
            actionUIs.Add(weaponAction);

            // Spawn Icon
            var slotIcon = Instantiate(slotIconPrefab, slotsHolder.transform).GetComponent<SlotIconUI>();
            Color color = ResourceMananger.instance.GetColor(action.actionType);
            slotIcon.Initialize(color);
            slotIconUIs.Add(slotIcon);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(actionsHolder.GetComponent<RectTransform>());
    }

    public void Show()
    {
        tabLabel.enabled = true;
        tabBackground.color = activeTabColor;
        actionsHolder.SetActive(true);
        slotsHolder.SetActive(false);

        LayoutRebuilder.ForceRebuildLayoutImmediate(actionsHolder.GetComponent<RectTransform>());
    }

    public void Hide()
    {
        tabLabel.enabled = false;
        tabBackground.color = inactiveTabColor;
        actionsHolder.SetActive(false);
        slotsHolder.SetActive(true);
    }

    public void Uninitialize()
    {
        foreach (var tooltip in actionUIs)
        {
            // Destroy
            Destroy(tooltip.gameObject);
        }
        actionUIs.Clear();

        foreach (var slot in slotIconUIs)
        {
            // Destroy
            Destroy(slot.gameObject);
        }
        slotIconUIs.Clear();
    }
}
