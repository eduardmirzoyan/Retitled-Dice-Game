using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    [SerializeField] private TextMeshPro textBox;
    [SerializeField] private Rigidbody2D body;

    private void Awake()
    {
        textBox = GetComponentInChildren<TextMeshPro>();
        body = GetComponentInChildren<Rigidbody2D>();
    }

    public void Initialize(string message, Color color, float initialXVel = 0f, float intitialYVel = 0f, float duration = 1f)
    {
        this.textBox.text = message;
        textBox.color = color;

        // Set start velocity
        body.velocity = new Vector2(initialXVel, intitialYVel);

        // Timed destroy
        Destroy(gameObject, duration);
    }
}
