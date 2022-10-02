using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LocationIndicatorUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    [Header("Components")]
    [SerializeField] private Image actionIcon;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Data")]
    [SerializeField] private Vector3Int location;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;

    public void Initialize(Vector3Int source, Vector3Int location, Action action) {
        this.location = location;

        // Update icon
        actionIcon.sprite = action.icon;

        // Get world Position
        var worldSourceLocation = RoomUI.instance.floorTilemap.GetCellCenterWorld(source);
        var worldLocation = RoomUI.instance.floorTilemap.GetCellCenterWorld(location);

        // Draw line from source to location
        lineRenderer.SetPosition(0, worldSourceLocation);
        lineRenderer.SetPosition(1, worldLocation);
        lineRenderer.endColor = action.color;
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
