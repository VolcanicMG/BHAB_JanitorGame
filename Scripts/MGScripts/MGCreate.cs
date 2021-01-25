/*
 * Scripted By: Wolfgang Sandtner
 * Date: 3/05/2020
 * Purpose: This script is applied to nothing, it is a developer script used to create a custom Unity Engine Toolbar.
 * That Toolbar can then be used to apply bulk settings to items, such as the floor tiles.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class MGCreate
{
    //Variables:
    /*[MenuItem("Tools/Assign Tile Material")] //Bulk assigns a material to the MovementGrid Tiles
    public static void AssignTileMaterial()
    {
        GameObject[] tiles = GameObject.FindGameObjectsWithTag("MGTile");
        Material material = Resources.Load<Material>("MGTile");

        foreach (GameObject t in tiles)
        {
            t.GetComponent<Renderer>().material = material; //Adds MGTile Material
        }
    }*/

    //[MenuItem("Tools/Assign Tile Script")] //Bulk assigns a script to the MovementGrid Tiles
    //public static void AssignTileScript()
    //{
    //    GameObject[] tiles = GameObject.FindGameObjectsWithTag("MGTile"); //Finds all game tiles

    //    foreach (GameObject t in tiles)
    //    {
    //        t.AddComponent<MGTile>(); //Adds MGTile Script
    //    }
    //}

    //[MenuItem("Tools/Assign All Tile Materials")] //Bulk assigns materials to the MovementGrid Tiles Scripts
    //public static void AssignAllTileMaterials()
    //{
    //    GameObject[] tiles = GameObject.FindGameObjectsWithTag("MGTile"); //Finds all game tiles
    //    Material material1 = Resources.Load<Material>("MGCurrentMaterial");
    //    Material material2 = Resources.Load<Material>("MGTargetMaterial");
    //    Material material3 = Resources.Load<Material>("MGSelectableMaterial");
    //    Material material4 = Resources.Load<Material>("MGDefaultMaterial");
    //    Material material5 = Resources.Load<Material>("MGUnwalkableMaterial");

    //    foreach (GameObject t in tiles)
    //    {
    //        t.GetComponent<MGTile>().MGCurrentMaterial = material1; //Adds MGTile Material
    //        t.GetComponent<MGTile>().MGTargetMaterial = material2; //Adds MGTile Material
    //        t.GetComponent<MGTile>().MGSelectableMaterial = material3; //Adds MGTile Material
    //        t.GetComponent<MGTile>().MGDefaultMaterial = material4; //Adds MGTile Material
    //        t.GetComponent<MGTile>().MGUnwalkableMaterial = material5; //Adds MGTile Material
    //    }
    //}

}
