using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class ProperLayerSort : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Renderer rend;

    [Header("Settings")]
    [SerializeField] private float offset = 0;
    [SerializeField] private bool runOnce = false;
    [SerializeField] private bool isActive;

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private void LateUpdate()
    {
        if (isActive)
        {
            // Constantly update layer
            UpdateLayer();

            // Remove this component after sorting once
            if (runOnce)
                Destroy(this);
        }
    }

    public void UpdateLayer()
    {
        rend.sortingOrder = -(int)((transform.position.y - offset) * 10);
    }

    public void SetActive(bool isActive)
    {
        this.isActive = isActive;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - new Vector3(0, offset, 0), 0.15f);
    }
}
