using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
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

    public void Initialize(Weapon weapon, Entity entity)
    {
        this.weapon = weapon;
        this.entity = entity;

        // Update text
        weaponNameText.text = weapon.name;
        weaponDamageText.text = $"Base Damage: {weapon.baseDamage}";

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
            upgradeWeaponButton.interactable = entity.gold >= weapon.UpgradeCost();

            // Update labels
            weaponNameText.text = weapon.name;
            weaponDamageText.text = $"Base Damage: {weapon.baseDamage}";
            string goldIcon = "<sprite name=\"Gold\">";
            upgradeCostText.text = $"Cost: {weapon.UpgradeCost()}{goldIcon}";
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
