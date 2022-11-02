using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeartUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Animator animator;

    [Header("Data")]
    [SerializeField] private bool isEmpty = false;

    public void Initialize(bool isEmpty)
    {
        this.isEmpty = isEmpty;

        // Set animation based on state
        if (isEmpty)
        {
            animator.Play("Empty");
        }
        else
        {
            animator.Play("Full");
        }
    }

    public void Deplete()
    {
        if (!isEmpty)
        {
            // Play transition
            animator.Play("Deplete");

            // Change states
            isEmpty = true;
        }

    }

    public void Restore()
    {
        if (isEmpty)
        {
            // Play transition
            animator.Play("Restore");

            // Change states
            isEmpty = false;
        }

    }
}
