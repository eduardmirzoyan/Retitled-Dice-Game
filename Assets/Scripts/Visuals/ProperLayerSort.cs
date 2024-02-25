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

    private void Awake()
    {
        rend = GetComponent<Renderer>();
    }

    private void Start()
    {
        UpdateLayer();
    }

    public void UpdateLayer()
    {
        rend.sortingOrder = -(int)(transform.position.y * 10 - offset);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position - new Vector3(0, offset, 0), 0.15f);
    }
}
