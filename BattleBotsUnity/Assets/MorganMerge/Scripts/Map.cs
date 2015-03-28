/////////////////////////////////////////////////////////////////////////////////
//
//	Map.cs
//	© EternalVR, All Rights Reserved
//
//	description:	A class holding the information to create a hexagon map using
//					the MapEditor or BoardManager
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public class Map : ScriptableObject {

	[SerializeField]
	protected HexagonData[] map;			//The HexagonData stored to this map
	public int GridWidth;					//Width of this map
	public int GridHeight;					//Heigh of this map
	public int SaveIndex;					//Where in the BoardManager this map is saved

	/// <summary>
	/// Setup the hexagons of this map using the HexagonData class
	/// </summary>
	public void SetHexagons(Hexagon[] newHexagons) {
		map = new HexagonData[GridWidth*GridHeight];
		for (int x=0; x<GridWidth; x++) {
			for (int y=0; y<GridHeight; y++) {
				HexagonData h = new HexagonData(newHexagons[y * GridWidth + x]);
				map[y * GridWidth + x] = h;
				EditorUtility.SetDirty (this);	
			}
		}
	}

	/// <summary>
	/// Returns the map of hexagons
	/// </summary>
	public HexagonData[] GetMap() {
		if (map == null) {
			Debug.LogError ("Map is null, tell morgan to fix it or start over");
		}
		return map;
	}
}
