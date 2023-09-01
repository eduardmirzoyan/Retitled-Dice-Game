using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerClickHandler
{
    [Header("Components")]
    [SerializeField] private RectTransform rectTransform;
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Image itemSprite;
    [SerializeField] private Image outlineSprite;
    [SerializeField] private Outline outline;
    [SerializeField] private Material grayMaterial;

    [Header("Data")]
    [SerializeField] private Item item;
    [SerializeField] private ItemSlotUI itemSlotUI;
    [SerializeField] private bool preventRemove;

    private bool isBeingDragged;
    private Transform currentParent;
    private Transform playerScreen;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();
        outline = GetComponentInChildren<Outline>();
        playerScreen = transform.root;
    }

    private void FixedUpdate()
    {
        if (isBeingDragged)
        {
            var point = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            point.z = 0;
            transform.position = point;
        }
    }

    private void OnDestroy()
    {
        // Hide tooltip if destroyed
        ItemTooltipUI.instance.Hide();
    }

    public void Initialize(Item item, ItemSlotUI itemSlotUI)
    {
        this.item = item;
        this.itemSlotUI = itemSlotUI;

        itemSprite.sprite = item.sprite;
        outlineSprite.sprite = item.sprite;
        preventRemove = false;

        // Move as second to last
        transform.SetSiblingIndex(2);

        // Set name
        name = item.name + " Item UI";
    }

    public void PreventRemove(bool prevent)
    {
        if (prevent)
        {
            itemSprite.material = grayMaterial;
            outline.enabled = false;
            preventRemove = true;
        }
        else
        {
            itemSprite.material = null;
            outline.enabled = true;
            preventRemove = false;
        }
    }

    public Item GetItem()
    {
        return item;
    }

    public void SetItemSlot(ItemSlotUI itemSlotUI)
    {
        this.itemSlotUI = itemSlotUI;
    }

    public ItemSlotUI GetSlotUI()
    {
        return itemSlotUI;
    }

    public void SetParent(Transform transform)
    {
        currentParent = transform;
    }

    public Transform GetParent()
    {
        return currentParent;
    }

    public void ResetTo(Transform transform)
    {
        // Return parent
        rectTransform.SetParent(transform);

        // Reset old parent
        currentParent = null;

        // Reset rotation
        this.transform.rotation = Quaternion.identity;

        // Reset position
        this.transform.localPosition = Vector3.zero;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Show outline
        outline.effectColor = Color.white;

        // Show tooltip at this items location
        ItemTooltipUI.instance.Show(item, itemSlotUI is ShopSlotUI);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Hide outline
        outline.effectColor = Color.clear;

        // HIde item info
        ItemTooltipUI.instance.Hide();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // If this item is right clicked
        if (eventData.button == PointerEventData.InputButton.Right && item != null)
        {
            // And the item is a consumable
            if (item is Consumable)
            {
                // Use the consumable and store result
                bool isSucessful = (item as Consumable).Use(DataManager.instance.GetPlayer());

                // Delete item if it was used
                if (isSucessful)
                {
                    // Remove item
                    itemSlotUI.StoreItem(null);

                    // Then delete it
                    Destroy(gameObject);
                }
            }
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            // Check restrictions
            if (itemSlotUI == null || preventRemove)
            {
                eventData.Reset();
                return;
            }

            // Toggle flag
            isBeingDragged = true;

            // Update visuals
            canvasGroup.alpha = 0.6f;
            canvasGroup.blocksRaycasts = false;

            // Save parent
            currentParent = rectTransform.parent;

            // Remove parent
            rectTransform.SetParent(playerScreen);

            // Change cursor
            GameManager.instance.SetGrabCursor();
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Does nothing
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isBeingDragged)
        {
            // Stop dragging
            isBeingDragged = false;

            // Update visuals
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;

            // Return parent
            rectTransform.SetParent(currentParent);

            // Reset old parent
            currentParent = null;

            // Reset rotation
            transform.rotation = Quaternion.identity;

            // Reset position
            transform.localPosition = Vector3.zero;

            // Change cursor
            GameManager.instance.SetDefaultCursor();
        }
    }
}
