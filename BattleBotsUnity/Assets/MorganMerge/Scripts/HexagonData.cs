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
	public Material CurrentBrushTexture;
	public int HexHeight;
	public Material[] typeMaterials; //List of materials

	public HexagonData(Hexagon h) {
		HexRow = h.HexRow;
		HexHeight = h.HexHeight;
		CurrentHexType = h.CurrentHexType;
		CurrentBrushTexture = h.CurrentBrushTexture;
		typeMaterials = h.typeMaterials;
		HexHeight = h.HexHeight;
	}
}
