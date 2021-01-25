using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AstarDebugger : MonoBehaviour
{
    //private static AstarDebugger instance;

    //public static AstarDebugger MyIsntace
    //{
    //    get
    //    {
    //        if (instance == null)
    //        {
    //            instance = FindObjectOfType<AstarDebugger>();
    //        }

    //        return instance;
    //    }
    //}

    //[SerializeField]
    //private Grid grid;

    //private Tilemap tilemap;

    //private Tile tile;

    //[SerializeField]
    //private Color openColor, closedColor, pathColor, currentColor, startColor, goalColor;

    //[SerializeField]
    //private Canvas canvas;

    //[SerializeField]
    //private GameObject debugTextPrefab;

    //private List<GameObject> debugObjects = new List<GameObject>();

    //public void CreateTiles(Vector3Int start, Vector3Int goal)
    //{
    //    ColorTile(start, startColor);
    //    ColorTile(goal, goalColor);
    //}

    //public void ColorTile(Vector3Int pos, Color color)
    //{
    //    tilemap.SetTile(pos, tile);
    //    tilemap.SetTileFlags(pos, TileFlags.None);
    //    tilemap.SetColor(pos, color);
    //}
}
