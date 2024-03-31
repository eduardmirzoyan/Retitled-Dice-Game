using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathUtil
{
    public static int ManhattanDistance(Vector3Int a, Vector3Int b)
    {
        return Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }

    public static Vector3Int Direction(Vector3Int from, Vector3Int to)
    {
        Vector3Int direction = to - from;
        direction.Clamp(-Vector3Int.one, Vector3Int.one);
        return direction;
    }
}
