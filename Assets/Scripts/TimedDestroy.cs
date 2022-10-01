using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedDestroy : MonoBehaviour
{
    [SerializeField] private float duration = 0.25f;

    private void Awake()
    {
        // Start timed destroy
        Destroy(gameObject, duration);
    }
}
