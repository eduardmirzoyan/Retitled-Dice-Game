using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/*
~~~ Logic ~~~

If mouse enters while holding an item, darken screen and show icon

Releasing mouse over UI, freezes game, pops confirmation UI

Accepting, deletes item with vfx
Declining, returns item to slot
*/

public class DropItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    [Header("Debug")]
    private ItemUI itemUI;

    public void OnPointerEnter(PointerEventData eventData)
    {
        // If dragging an item in
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent(out ItemUI itemUI))
        {
            // Make sure item is from an inventory
            if (itemUI.GetSlotUI() is not InventorySlotUI)
                return;

            this.itemUI = itemUI;
            itemUI.ShowTrashIcon(true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent(out ItemUI _) && itemUI != null)
        {
            itemUI.ShowTrashIcon(false);
            itemUI = null;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (itemUI != null)
        {
            // Drop item
            DropAlertUI.instance.Show(itemUI);

            // Reset
            itemUI.ShowTrashIcon(false);
            itemUI = null;
        }
    }
}
