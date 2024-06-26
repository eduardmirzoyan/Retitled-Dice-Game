using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ActionSpeed { Instant, Delayed }
public enum ActionType { Movement, Attack, Utility }

public abstract class Action : ScriptableObject
{
    [Header("Static Data")]
    public new string name;
    [TextArea(4, 2)]
    public string rawInactiveDescription;
    public Sprite icon;
    public Color color;
    public ActionSpeed actionSpeed;
    public ActionType actionType;
    public GameObject pathPrefab;

    [Header("Dynamic Data")]
    public Die die;
    public Weapon weapon;
    public List<ModifierTag> modifiers;

    [Header("Bonuses")]
    public int bonusDamage;

    public void Initialize(Weapon weapon)
    {
        this.weapon = weapon;

        die.Initialize(this);
        modifiers = new List<ModifierTag>();
    }

    public void Uninitialize()
    {
        this.weapon = null;

        die.Uninitialize();
        modifiers.Clear();
    }

    public static Vector3Int[] cardinalDirections = new Vector3Int[] { Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left, };

    public abstract List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room);

    public abstract List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation, Room room);

    public abstract IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room);

    public string GetInactiveDescription()
    {
        return rawInactiveDescription.Replace("{dmg}", "<sprite name=\"Dmg\">").Replace("{die}", "<sprite name=\"Die\">");
    }

    public string GetActiveDescription()
    {
        string dmgHex = ResourceMananger.instance.GetDamageHex();
        string dieHex = ResourceMananger.instance.GetDieHex();

        if (weapon == null)
            return rawInactiveDescription.Replace("{dmg}", $"<color={dmgHex}>{bonusDamage}</color>").Replace("{die}", $"<color={dieHex}>{die.value}</color>");

        return rawInactiveDescription.Replace("{dmg}", $"<color={dmgHex}>{GetTotalDamage()}</color>").Replace("{die}", $"<color={dieHex}>{die.value}</color>");
    }

    public string GetDynamicName()
    {
        int count = die.maxValue + die.minValue - 2;
        if (count > 0)
            return $"{name}<color=grey>+{die.maxValue + die.minValue - 2}</color>";

        return name;
    }

    public int GetTotalDamage()
    {
        if (weapon != null)
            return weapon.baseDamage + bonusDamage;

        return bonusDamage;
    }

    public int UpgradeCost()
    {
        return (int)Mathf.Pow(2, die.maxValue + die.minValue - 2);
    }

    public void UpgradeMaxValue()
    {
        die.maxValue += 1;
    }

    public void AddOrOverwriteModifier(ModifierTag modifier)
    {
        for (int i = 0; i < modifiers.Count; i++)
        {
            // Overwrite
            if (modifiers[i].source == modifier.source)
            {
                modifiers[i] = modifier;

                // Finish
                return;
            }
        }

        // If not, add to list
        modifiers.Add(modifier);
    }

    public void RemoveModifier(string source)
    {
        foreach (var modifier in modifiers)
        {
            // Find by source
            if (modifier.source == source)
            {
                // Remove from list
                modifiers.Remove(modifier);

                // Finish
                return;
            }
        }
    }

    public Action Copy()
    {
        // Make a copy
        var copy = Instantiate(this);

        // Make a copy of its die as well
        copy.die = die.Copy();

        return copy;
    }
}
