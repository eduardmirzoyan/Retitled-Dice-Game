using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enchantments/Weapon/Replenish On Kill")]
public class ReplenishOnKillWE : WeaponEnchantment
{
    [SerializeField] private bool replenishAllActions;

    public override void Initialize(Weapon weapon)
    {
        GameEvents.instance.onEntityKillEntity += ReplenishActions;
    }

    public override void Uninitialize(Weapon weapon)
    {
        GameEvents.instance.onEntityKillEntity -= ReplenishActions;
    }

    private void ReplenishActions(Entity killer, Weapon weapon, Entity victim)
    {
        // FIXME: Change to replenish AND reroll

        // If this weapon killed someone
        if (this.weapon == weapon)
        {
            if (replenishAllActions)
            {
                foreach (var action in weapon.actions)
                {
                    if (action.die.isExhausted)
                        action.die.Replenish();
                }
            }
            else
            {
                foreach (var action in weapon.actions)
                {
                    if (action.actionType == ActionType.Attack)
                        if (action.die.isExhausted)
                            action.die.Replenish();
                }
            }
        }
    }
}
