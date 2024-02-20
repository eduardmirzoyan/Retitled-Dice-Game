using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;
public class WeaponUpgradeUI : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private TextMeshProUGUI weaponNameText;
    [SerializeField] private TextMeshProUGUI weaponDamageText;
    [SerializeField] private LayoutGroup actionUpgradesLayout;
    [SerializeField] private GameObject actionUpgradePrefab;
    [SerializeField] private Button upgradeWeaponButton;
    [SerializeField] private TextMeshProUGUI upgradeCostText;

    [Header("Dynamic Data")]
    [SerializeField] private Weapon weapon;
    [SerializeField] private Entity entity;

    [Header("Settings")]
    [SerializeField] private Color unafforableColor;

    public void Initialize(Weapon weapon, Entity entity)
    {
        this.weapon = weapon;
        this.entity = entity;

        UpdatePrice(entity, 0);

        // Display actions
        foreach (var action in weapon.actions)
        {
            var upgradeUI = Instantiate(actionUpgradePrefab, actionUpgradesLayout.transform).GetComponent<ActionUpgradeUI>();
            upgradeUI.Initialize(action, entity);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(actionUpgradesLayout.GetComponent<RectTransform>());

        GameEvents.instance.onRemoveBlacksmith += Uninitialize;
        GameEvents.instance.onEntityGoldChange += UpdatePrice;
    }

    private void OnDestroy()
    {
        GameEvents.instance.onRemoveBlacksmith -= Uninitialize;
        GameEvents.instance.onEntityGoldChange -= UpdatePrice;
    }

    private void Uninitialize(Weapon weapon)
    {
        Destroy(gameObject);
    }

    private void UpdatePrice(Entity entity, int change)
    {
        if (this.entity == entity)
        {
            // Check if buttons can be interacted with
            bool affordable = entity.gold >= weapon.UpgradeCost();
            upgradeWeaponButton.interactable = affordable;

            // Update labels
            weaponNameText.text = weapon.name;
            weaponDamageText.text = $"Base Damage: <color=yellow>{weapon.baseDamage}</color>";
            string goldIcon = "<sprite name=\"Gold\">";
            string cost = affordable ? $"{weapon.UpgradeCost()}" : $"<color=#{unafforableColor.ToHexString()}>{weapon.UpgradeCost()}</color>";
            upgradeCostText.text = $"Cost: {cost}{goldIcon}";
        }
    }

    public void IncreaseBaseDamage()
    {
        int cost = -weapon.UpgradeCost();

        // Apply upgrade
        weapon.IncreaseBaseDamage();

        // Spend money
        entity.AddGold(cost);
    }
}
