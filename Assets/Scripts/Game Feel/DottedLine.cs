using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DottedLine : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Material dottedLineMaterial;
    [SerializeField] private float speed = 5f;

    [Header("Dynamic Data")]
    [SerializeField] private bool isActive;

    private void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.material = dottedLineMaterial;
    }

    private void Update()
    {
        lineRenderer.material.mainTextureOffset += Vector2.left * speed * Time.deltaTime;
    }
}
