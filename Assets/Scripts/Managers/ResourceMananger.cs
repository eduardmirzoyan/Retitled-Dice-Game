using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ResourceMananger : MonoBehaviour
{
    [Header("Cursor")]
    [SerializeField] private Texture2D cursorTexture;
    [SerializeField] private CursorMode cursorMode = CursorMode.Auto;

    [Header("Data")]
    [SerializeField] private ColorPalette colorPalette;

    public static ResourceMananger instance;
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

    public Color GetColor(EnchantmentRarity rarity)
    {
        if (colorPalette == null)
            throw new System.Exception("Color palette not set!");

        return rarity switch
        {
            EnchantmentRarity.Common => colorPalette.common,
            EnchantmentRarity.Uncommon => colorPalette.uncommon,
            EnchantmentRarity.Rare => colorPalette.rare,
            _ => throw new System.Exception("Unhandled rarity color!"),
        };
    }

    public Color GetColor(ActionType actionType)
    {
        if (colorPalette == null)
            throw new System.Exception("Color palette not set!");

        return actionType switch
        {
            ActionType.Attack => colorPalette.attackAction,
            ActionType.Movement => colorPalette.movementAction,
            ActionType.Utility => colorPalette.utilityAction,
            _ => throw new System.Exception("Unhandled action type color!"),
        };
    }

    public Color GetDieColor()
    {
        return colorPalette.dieColor;
    }

    public string GetDieHex()
    {
        return $"#{colorPalette.dieColor.ToHexString()}";
    }

    public Color GetDamageColor()
    {
        return colorPalette.damageColor;
    }

    public string GetDamageHex()
    {
        return $"#{colorPalette.damageColor.ToHexString()}";
    }

    public void SetGrabCursor()
    {
        Cursor.SetCursor(cursorTexture, Vector2.zero, cursorMode);
    }

    public void SetDefaultCursor()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}
