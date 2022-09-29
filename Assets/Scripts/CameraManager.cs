using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;
    private void Awake()
    {
        // Singleton Logic
        if (CameraManager.instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public void SetPosition(Vector3 position) {
        position.z = -10;
        transform.position = position;
    }
}
