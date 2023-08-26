using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    [Header("Mechanics")]
    // TODO

    [Header("Visuals")]
    public bool useHitFlash = true;
    public bool useHitFreeze = true;
    public bool useScreenShake = true;
    public bool useAttackEffect = true;
    public bool useHitEffect = true;

    public float aiBufferTime = 0.25f;
    public float moveBufferTime = 0.35f;
    public float warpBufferTime = 0.5f;

    public float weaponDrawBufferTime;
    public float weaponMeleeBufferTime;
    public float weaponSheatheBufferTime;
}
