using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackParticle : MonoBehaviour
{
    [Header("Static Data")]
    [SerializeField] private float lifetime = 0.25f;
    [SerializeField] private string sfxName;

    private void Start()
    {
        // Play sound effect
        if (sfxName != "")
            AudioManager.instance.PlaySFX(sfxName);

        // Start timed destroy
        Destroy(gameObject, lifetime);
    }
}
