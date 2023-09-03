using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

// Alive from OnLocationSelect and onwards
public class ActionPathRenderer : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Material unfocusedMaterial;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float arcHeight = 0f;
    [SerializeField] private int numPoints = 50;
    [SerializeField] private bool isAnimated;

    [Header("Dynamic Data")]
    [SerializeField] private Material defaultMaterial;
    [SerializeField] private Entity entity;
    [SerializeField] private Action action;
    [SerializeField] private Vector3Int location;
    [SerializeField] private bool isFocused;

    private void Awake()
    {
        lineRenderer = GetComponentInChildren<LineRenderer>();
        defaultMaterial = lineRenderer.material;
    }

    public void Initialize(Entity entity, Action action, Vector3Int location, Vector3 start, Vector3 end, Color color)
    {
        this.entity = entity;
        this.action = action;
        this.location = location;

        lineRenderer.endColor = color;
        lineRenderer.positionCount = numPoints;

        if (start.x == end.x)
        {
            arcHeight = 0f;
        }
        Vector3 middle = (end + start) / 2 + Vector3.up * arcHeight;

        for (int i = 0; i < numPoints; i++)
        {
            float ratio = (float)i / numPoints;
            Vector3 ac = Vector3.Lerp(start, middle, ratio);
            Vector3 cb = Vector3.Lerp(middle, end, ratio);

            Vector3 point = Vector3.Lerp(ac, cb, ratio);
            lineRenderer.SetPosition(i, point);
        }

        Focus();

        // Sub
        GameEvents.instance.onActionSelect += UnintializeOnActionSelect;
        GameEvents.instance.onLocationSelect += UnintializeOnLocationChange;
        GameEvents.instance.onActionPerformEnd += UnintializeOnActionEnd;
        GameEvents.instance.onActionUnthreatenLocations += UnintializeOnUnthreaten;
        GameEvents.instance.onTurnEnd += UnfocusOnTurnEnd;
        GameEvents.instance.onTurnStart += FocusOnTurnStart;
    }

    private void OnDestroy()
    {
        // Unsub
        GameEvents.instance.onActionSelect -= UnintializeOnActionSelect;
        GameEvents.instance.onLocationSelect -= UnintializeOnLocationChange;
        GameEvents.instance.onActionPerformEnd -= UnintializeOnActionEnd;
        GameEvents.instance.onActionUnthreatenLocations -= UnintializeOnUnthreaten;
        GameEvents.instance.onTurnEnd -= UnfocusOnTurnEnd;
        GameEvents.instance.onTurnStart -= FocusOnTurnStart;
    }
    private void UnintializeOnActionSelect(Entity entity, Action action)
    {
        if (this.entity == entity && action == null)
        {
            Destroy(gameObject);
        }
    }

    private void UnintializeOnUnthreaten(Action action, List<Vector3Int> locations)
    {
        if (this.action == action)
        {
            Destroy(gameObject);
        }
    }

    private void UnintializeOnLocationChange(Entity entity, Action action, Vector3Int location)
    {
        // If you a different location with this action
        if (this.action == action && this.location != location)
        {
            Destroy(gameObject);
        }
    }

    private void UnintializeOnActionEnd(Entity entity, Action action, Vector3Int location, Room room)
    {
        // If this action was performed
        if (this.action == action)
        {
            Destroy(gameObject);
        }
    }

    private void UnfocusOnTurnEnd(Entity entity)
    {
        if (this.entity == entity)
        {
            // Unfocus();
        }
    }

    private void FocusOnTurnStart(Entity entity)
    {
        if (this.entity == entity)
        {
            Focus();
        }
    }

    private void FocusOnInspect(Entity entity)
    {
        if (this.entity == entity)
        {
            Focus();
        }
    }

    private void Focus()
    {
        isFocused = true;
    }

    private void Unfocus()
    {
        isFocused = false;
    }

    private void Update()
    {
        if (isAnimated && isFocused)
            lineRenderer.material.mainTextureOffset += Vector2.left * speed * Time.deltaTime;
    }
}
