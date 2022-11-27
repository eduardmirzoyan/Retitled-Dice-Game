using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class LocationIndicatorUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IDropHandler
{
    [Header("Components")]
    [SerializeField] private Image outlineImage;
    [SerializeField] private Image actionIcon;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Data")]
    [SerializeField] private Entity entity;
    [SerializeField] private Action action;
    [SerializeField] private Vector3Int location;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;

    [Header("Config")]
    [SerializeField] private bool drawOnHover = false;

    public void Initialize(Entity entity, Vector3Int location, Vector3 sourceLocation, Vector3 targetLocation, Action action, bool isPreview = false)
    {
        this.entity = entity;
        this.location = location;
        this.action = action;

        // Update color
        outlineImage.color = Color.clear;

        // Update action icon
        actionIcon.enabled = false;
        actionIcon.sprite = action.icon;

        // Hide line
        lineRenderer.enabled = false;

        // Draw path from source to location
        // lineRenderer.SetPosition(0, sourceLocation);
        // lineRenderer.SetPosition(1, targetLocation);
        // 

        // Hide line if needed
        // if (drawOnHover)
        // {
        //     lineRenderer.enabled = false;
        //     actionIcon.enabled = false;
        // }

        // Hide outline if needed
        // if (isPreview)
        // {
        //     outlineImage.enabled = false;
        // }


        // Sub
        GameEvents.instance.onActionSelect += Unintialize;
        GameEvents.instance.onActionPerformEnd += Unintialize;
        GameEvents.instance.onInspectAction += Unintialize;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onActionSelect -= Unintialize;
        GameEvents.instance.onActionPerformEnd -= Unintialize;
        GameEvents.instance.onInspectAction -= Unintialize;
    }

    private void Unintialize(Entity entity, Action action)
    {
        // If no action was selected, destroy this
        if (this.entity == entity && action == null && !actionIcon.enabled)
        {
            // Destroy self
            Destroy(gameObject);
        }
    }

    private void Unintialize(Entity entity, Action action, Vector3Int location, Room room)
    {
        // After this action is performed, delete outcome
        if (this.entity == entity && this.action == action)
        {
            // Destroy self
            Destroy(gameObject);
        }
    }

    private void RemoveHighlight(Entity entity, Vector3Int location)
    {
        if (this.entity == entity)
        {
            // Remove highlight
            outlineImage.enabled = false;
        }
    }

    private void GeneratePath()
    {
        // Get path
        var path = entity.room.pathfinder.FindPath(entity.location, location, entity.room);

        lineRenderer.positionCount = path.Count;
        for (int i = 0; i < path.Count; i++)
        {
            // Set each vertex
            lineRenderer.SetPosition(i, path[i] + Vector3.one * 0.5f);
        }

        // Set color
        lineRenderer.endColor = action.color;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // If you are dragging over a die
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent(out DieUI dieUI))
        {
            GeneratePath();

            // Show path
            lineRenderer.enabled = true;
            actionIcon.enabled = true;

            // Highlight
            actionIcon.color = highlightColor;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // If you are dragging over a die
        if (eventData.pointerDrag != null && eventData.pointerDrag.TryGetComponent(out DieUI dieUI))
        {
            // Hide path
            lineRenderer.enabled = false;
            actionIcon.enabled = false;

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
