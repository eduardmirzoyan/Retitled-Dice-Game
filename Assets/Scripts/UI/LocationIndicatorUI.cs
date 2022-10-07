using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;

public class LocationIndicatorUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    [Header("Components")]
    [SerializeField] private Image actionIcon;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Data")]
    [SerializeField] private Entity entity;
    [SerializeField] private Action action;
    [SerializeField] private Vector3Int location;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;

    public void Initialize(Entity entity, Vector3Int location, Vector3 sourceLocation, Vector3 targetLocation, Action action)
    {
        this.entity = entity;
        this.location = location;
        this.action = action;

        // Update icon
        actionIcon.sprite = action.icon;

        // Draw line from source to location
        lineRenderer.SetPosition(0, sourceLocation);
        lineRenderer.SetPosition(1, targetLocation);
        lineRenderer.endColor = action.color;

        // Sub
        GameEvents.instance.onActionSelect += Unintialize;
        GameEvents.instance.onLocationSelect += Unintialize;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onActionSelect -= Unintialize;
        GameEvents.instance.onLocationSelect -= Unintialize;
    }

    private void Unintialize(Entity entity, Action action, Room roomD)
    {
        // If no action was selected, destroy this
        if (this.entity == entity && action == null)
        {
            // Destroy self
            Destroy(gameObject);
        }
    }

    private void Unintialize(Entity entity, Vector3Int location)
    {
        if (this.entity == entity) {
            // Destroy self
            Destroy(gameObject);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // If you are dragging over a die
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent(out DieUI dieUI))
        {
            // Highlight
            actionIcon.color = highlightColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // If you are dragging over a die
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent(out DieUI dieUI))
        {
            // Un-highlgiht
            actionIcon.color = defaultColor;
        }
    }

    public void OnDrop(PointerEventData eventData)
    {
        // Make sure a die was dropped here
        // If you are dragging over a die
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent(out DieUI dieUI))
        {
            // Un-highlgiht
            actionIcon.color = defaultColor;

            // Select this location
            GameManager.instance.ConfirmLocation(location);
        }
    }
}
