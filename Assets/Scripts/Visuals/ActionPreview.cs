using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPreview : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer actionIcon;
    [SerializeField] private LineRenderer lineRenderer;

    [Header("Data")]
    [SerializeField] private Entity entity;
    [SerializeField] private Action action;
    [SerializeField] private Vector3Int location;
    [SerializeField] private Color defaultColor = Color.white;
    [SerializeField] private Color highlightColor = Color.yellow;

    private bool isHovering;
    private bool isSelected;

    private void Awake()
    {
        actionIcon = GetComponentInChildren<SpriteRenderer>();
        lineRenderer = GetComponentInChildren<LineRenderer>();
        isHovering = false;
        isSelected = false;
    }

    public void Initialize(Entity entity, Vector3Int location, Action action, bool isPreview = false)
    {
        this.entity = entity;
        this.location = location;
        this.action = action;

        // Update action icon
        actionIcon.enabled = false;
        actionIcon.sprite = action.icon;

        // Hide line
        lineRenderer.enabled = false;

        // Create path
        GeneratePath(entity.location, location, entity.room);

        // Sub
        GameEvents.instance.onActionSelect += UnintializeOnSelect;
        GameEvents.instance.onActionConfirm += UnintializeOnConfirm;
        GameEvents.instance.onActionPerformEnd += UnintializeOnEnd;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onActionSelect -= UnintializeOnSelect;
        GameEvents.instance.onActionConfirm -= UnintializeOnConfirm;
        GameEvents.instance.onActionPerformEnd -= UnintializeOnEnd;
    }

    private void UnintializeOnSelect(Entity entity, Action action)
    {
        // If no action was selected, destroy this
        if (this.entity == entity && action == null && !actionIcon.enabled)
        {
            // Destroy self
            Destroy(gameObject);
        }
    }

    private void UnintializeOnConfirm(Entity entity, Action action, Vector3Int location)
    {
        // After this action is performed, delete outcome
        if (this.entity == entity && this.action == action && this.location != location)
        {
            // Destroy self
            Destroy(gameObject);
        }
    }

    private void UnintializeOnEnd(Entity entity, Action action, Vector3Int location, Room room)
    {
        // After this action is performed, delete outcome
        if (this.entity == entity && this.action == action)
        {
            // Destroy self
            Destroy(gameObject);
        }
    }

    private void GeneratePath(Vector3Int start, Vector3Int end, Room room)
    {
        // OLD CODE
        // Get path
        // var path = room.pathfinder.FindPath(start, end, room);
        // lineRenderer.positionCount = path.Count;
        // for (int i = 0; i < path.Count; i++)
        // {
        //     // Set each vertex
        //     lineRenderer.SetPosition(i, path[i] + Vector3.one * 0.5f);
        // }

        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, start + Vector3.one * 0.5f);
        lineRenderer.SetPosition(1, end + Vector3.one * 0.5f);

        // Set color
        lineRenderer.endColor = action.color;
    }

    private void OnMouseEnter()
    {
        if (isSelected) return;

        // Debug
        // print("Entered!");

        // Show path
        if (action.actionType == ActionType.Movement)
        {
            lineRenderer.enabled = true;
        }

        // Highlight
        actionIcon.enabled = true;
        actionIcon.color = highlightColor;

        // Select this location
        GameManager.instance.SelectLocation(location);

        // Update state
        isHovering = true;
    }

    private void Update()
    {
        if (isHovering)
        {
            // Check for left click release
            if (Input.GetMouseButtonUp(0))
            {
                // Debug
                // print("Dropped!");

                // Un-highlgiht
                actionIcon.color = defaultColor;

                // Confirm this action at this location
                GameManager.instance.ConfirmAction();

                // Update state
                isSelected = true;
            }
        }
    }

    private void OnMouseExit()
    {
        if (isSelected) return;

        // Debug
        // print("Exit!");

        // Hide path
        if (action.actionType == ActionType.Movement)
        {
            lineRenderer.enabled = false;
        }

        // Un-highlight
        actionIcon.enabled = false;
        actionIcon.color = defaultColor;

        // Select this location
        GameManager.instance.SelectLocation(Vector3Int.zero);

        // Update state
        isHovering = false;
    }
}
