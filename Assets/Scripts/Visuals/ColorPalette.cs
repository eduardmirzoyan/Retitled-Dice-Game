using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class ColorPalette : ScriptableObject
{
    [Header("Action Types")]
    public Color attackAction;
    public Color movementAction;
    public Color utilityAction;

    [Header("Enchantment Rarities")]
    public Color common;
    public Color uncommon;
    public Color rare;
}
