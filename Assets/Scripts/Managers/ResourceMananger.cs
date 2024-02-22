using System.Collections;
using System.Collections.Generic;
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

        switch (rarity)
        {
            case EnchantmentRarity.Common:
                return colorPalette.common;
            case EnchantmentRarity.Uncommon:
                return colorPalette.uncommon;
            case EnchantmentRarity.Rare:
                return colorPalette.rare;
            default:
                throw new System.Exception("Unhandled rarity color!");
        }
    }

    public Color GetColor(ActionType actionType)
    {
        if (colorPalette == null)
            throw new System.Exception("Color palette not set!");

        switch (actionType)
        {
            case ActionType.Attack:
                return colorPalette.attackAction;
            case ActionType.Movement:
                return colorPalette.movementAction;
            case ActionType.Utility:
                return colorPalette.utilityAction;
            default:
                throw new System.Exception("Unhandled action type color!");
        }
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
