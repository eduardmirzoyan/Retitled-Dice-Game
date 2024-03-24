using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[CreateAssetMenu]
public class TileResourceContainer : ScriptableObject
{
    public TileBase wallTile;
    public TileBase floorTile;
    public TileBase entranceTile;
}
