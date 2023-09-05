using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Weapon/Roll Damage Buff")]
public class RollDamageWE : WeaponEnchantment
{
    [SerializeField] private int damageBonus = 1;
    [SerializeField] private bool checkForHighRoll;

    public override void Initialize(Weapon weapon)
    {
        // Check right now
        // CheckRoll();

        GameEvents.instance.onDieRoll += CheckRoll;
    }

    public override void Uninitialize(Weapon weapon)
    {
        GameEvents.instance.onDieRoll -= CheckRoll;
    }

    private void CheckRoll(Die die)
    {
        if (checkForHighRoll && die.IsHighRoll())
        {
            if (die.action.weapon == weapon)
            {
                // TODO
            }
        }
        else if (!checkForHighRoll && die.IsLowRoll())
        {
            if (die.action.weapon == weapon)
            {
                // TODO
            }
        }
    }
}
