using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum ActionSpeed { Instant, Reactive, Delayed }
public enum ActionType { Movement, Attack, Utility }

public abstract class Action : ScriptableObject
{
    [Header("Static Data")]
    public new string name;
    [TextArea(4, 2)]
    public string rawInactiveDescription;
    [TextArea(4, 2)]
    [SerializeField] private string rawActiveDescription;
    public Sprite icon;
    public Color color;
    public ActionSpeed actionSpeed;
    public ActionType actionType;
    public GameObject pathPrefab;

    [Header("Dynamic Data")]
    public Die die;
    public Weapon weapon;
    public List<Buff> buffs;
    public int bonusDamage;

    public void Initialize(Weapon weapon)
    {
        this.weapon = weapon;

        // Init die
        die.Initialize(this);

        buffs = new List<Buff>();
    }

    public void Uninitialize()
    {
        this.weapon = null;

        // Uninit die
        die.Uninitialize();

        // Remove all buffs
        RemoveAllBuffs();
        buffs = null;
    }

    public static Vector3Int[] cardinalDirections = new Vector3Int[] { Vector3Int.up, Vector3Int.right, Vector3Int.down, Vector3Int.left, };

    public abstract List<Vector3Int> GetValidLocations(Vector3Int startLocation, Room room);

    public abstract List<Vector3Int> GetThreatenedLocations(Entity entity, Vector3Int targetLocation);

    public abstract IEnumerator Perform(Entity entity, Vector3Int targetLocation, List<Vector3Int> threatenedLocations, Room room);

    public string GetInactiveDescription()
    {
        return rawInactiveDescription.Replace("{dmg}", $"<color=yellow>{weapon.baseDamage}</color>").Replace("{die}", "<sprite name=\"Die\">");
    }

    public string GetActiveDescription()
    {
        if (weapon == null)
            return rawActiveDescription.Replace("{dmg}", $"<color=yellow>{bonusDamage}</color>").Replace("{die}", $"<color=yellow>{die.value}</color>");

        return rawActiveDescription.Replace("{dmg}", $"<color=yellow>{GetTotalDamage()}</color>").Replace("{die}", $"<color=yellow>{die.value}</color>");
    }

    public string GetDynamicName()
    {
        int count = die.maxValue + die.minValue - 2;
        if (count > 0)
            return $"{name}+{die.maxValue + die.minValue - 2}";

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

    public void AddBuff(Buff buffToAdd, string source)
    {
        // Check if action already has the buff
        foreach (var buff in buffs)
        {
            // Is the effect the same type and source as the one being added
            if (buff.GetType() == buffToAdd.GetType() && buff.source == source)
            {
                // Debug
                Debug.Log(buffToAdd);

                // Then just stack the effect instead of adding new one
                buff.Stack(buffToAdd);

                // Finish
                return;
            }
        }

        // Make copy
        buffToAdd = buffToAdd.Copy();

        // Add to list
        buffs.Add(buffToAdd);

        // Initialize it
        buffToAdd.Initialize(this, source);
    }

    public void RemoveBuff(Buff buffToRemove, string source)
    {
        // Check if action has buff
        foreach (var buff in buffs)
        {
            // Is the effect the same type and source as the one being added
            if (buff.GetType() == buffToRemove.GetType() && buff.source == source)
            {
                // Uninit
                buff.Uninitialize();

                // Remove from list
                buffs.Remove(buff);

                // Finish
                return;
            }
        }
    }

    private void RemoveAllBuffs()
    {
        foreach (var buff in buffs)
        {
            // Uninit
            buff.Uninitialize();
        }
        buffs.Clear();
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
