using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// #FFEB04
public class ActionIndicator : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer actionIcon;
    [SerializeField] private SpriteRenderer shadowIcon;

    [Header("Data")]
    [SerializeField] private Entity entity;
    [SerializeField] private Action action;
    [SerializeField] private Vector3Int location;

    private bool isHovering;
    private bool isSelected;

    public void Initialize(Entity entity, Vector3Int location, Action action)
    {
        this.entity = entity;
        this.location = location;
        this.action = action;

        // Update action icon
        actionIcon.enabled = false;
        shadowIcon.enabled = false;
        actionIcon.sprite = action.icon;
        shadowIcon.sprite = action.icon;

        // Set state
        isHovering = false;
        isSelected = false;

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
        if (this.entity == entity && action == null)
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

    private void OnMouseEnter()
    {
        if (isSelected) return;

        // Debug
        //print("Entered!");

        // Highlight
        actionIcon.enabled = true;
        shadowIcon.enabled = true;

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
        //print("Exit!");

        // Un-highlight
        actionIcon.enabled = false;
        shadowIcon.enabled = false;

        // Select this location
        GameManager.instance.SelectLocation(Vector3Int.back);

        // Update state
        isHovering = false;
    }
}
