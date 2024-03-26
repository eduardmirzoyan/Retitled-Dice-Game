using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GameSettings : ScriptableObject
{
    public float diceRollTime = 0.5f;
    public float gameStartBufferTime = 1f;
    public float aiBufferTime = 0.25f;
    public float moveBufferTime = 0.35f;
    public float warpBufferTime = 0.5f;
    public float jumpBufferTime = 0.5f;

    public float actionDuration = 0.5f;
}
