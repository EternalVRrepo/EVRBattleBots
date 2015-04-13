/////////////////////////////////////////////////////////////////////////////////
//
//	HexagonData.cs
//	© EternalVR, All Rights Reserved
//
//	description:	Class used to store information about a hexagon, used to save/load
//					premade maps
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;

[System.Serializable]
public class HexagonData : ScriptableObject {
	public int HexRow;
	public int HexColumn;
	public Hexagon.HexType CurrentHexType;
	public Hexagon.SpawnType CurrentSpawnType;
	public Material CurrentBrushTexture;
	public int HexHeight;
	public Material[] typeMaterials; //List of materials

	public void CreateHexagonData(Hexagon h) {
		HexRow = h.HexRow;
		HexHeight = h.HexHeight;
		CurrentHexType = h.CurrentHexType;
		CurrentSpawnType = h.CurrentSpawnType;
		CurrentBrushTexture = h.CurrentBrushTexture;
		typeMaterials = h.typeMaterials;
		HexHeight = h.HexHeight;
	}
}
