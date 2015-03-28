/////////////////////////////////////////////////////////////////////////////////
//
//	BoardManager.cs
//	© EternalVR, All Rights Reserved
//
//	description:	class for managing the hexagonal grid functionality	including
//					the mathematical interactions like finding neighbors, LOS, etc
//
//	sources:		http://www.redblobgames.com/grids/hexagons/ used for several 
//					formulas for hexagon map storage and array calculations
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[System.Serializable]
public class BoardManager : MonoBehaviour {

	public GameObject HexPrefab;
	[SerializeField]
	public List<Map> Maps; //List of stored maps on this BoardManager
	public static Map CurrentMap;

	protected float HexWidth;
	protected float HexHeight;
	protected int GridHexColumns {
		get { return CurrentMap.GridWidth; }
	}
	protected int GridHexRows {
		get { return CurrentMap.GridHeight; }
	}

	public static BoardManager instance; //Singleton reference

	[SerializeField]
	private Hexagon [] HexagonArray; 										//The array of hexagons on the grid, done with a 1d array so it can be serializable for editting in edit mode
	
	/// <summary>
	/// Gets a hexagon from array applying the neccesary math and checking if the hexagon is usable
	/// </summary>
	public Hexagon GetHexagonFromArray(int x, int y) {
		x = x + Mathf.FloorToInt (y/2);
		if (OutOfArray(x, y)) {
			return null;
		}
		return HexagonArray[y * GridHexColumns + x] != null ? HexagonArray[y * GridHexColumns + x] : null;
	}
	
	/// <summary>
	/// Sets a hexagon to the array, adds neccesary offset
	/// </summary>
	public void SetHexagonToArray(int x, int y, Hexagon hexData) {
		HexagonArray[y * GridHexColumns + x] = hexData;
	}
	
	/// <summary>
	/// 
	/// </summary>
	public int GetArrayLength() {
		if (HexagonArray != null) {
			return HexagonArray.Length;
		}
		else return 0;
	}

	/// <summary>
	/// If the x,y coord is in the scope of the array
	/// This only works in the case of a rectangular grid, which it should
	/// </summary>
	protected bool OutOfArray(int x, int y) {
		if (x >= 0 && x < GridHexColumns) {
			if (y >= 0 && y < GridHexRows) {
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// 
	/// </summary>
	void Awake() {
		instance = this;
		DontDestroyOnLoad(this);
	}

	/// <summary>
	/// Initialize a map for combat
	/// </summary>
	public void InitializeMapForCombat(int index) {
		if (Maps.Count < index || Maps[index] == null) {
			Debug.LogError ("Trying to load an invalid map for combat");
			return;
		}

		HexPrefab = Resources.Load ("HexPrefab") as GameObject;
		if (HexPrefab == null) {
			Debug.LogError ("Add Hexagon Prefab to \"/Assets/Resources/\" with the name \"HexPrefab\" or change the path in BoardManager.cs");
			return;
		}

		CurrentMap = Maps[index];
		SetHexSizes();
		CreateMap();
	}

	/// <summary>
	/// Creates a hex grid using a new list of HexagonData
	/// </summary>
	protected void CreateMap() {
		HexagonData[] newHexData = CurrentMap.GetMap();
		for (int y = 0 ; y < GridHexRows; y++) {
			for (int x = 0; x < GridHexColumns; x++) {
				GameObject go = Instantiate (HexPrefab) as GameObject;
				Hexagon hex = go.GetComponent<Hexagon>();
				hex.SetupHexagon(newHexData[y * GridHexColumns + x]);
				
				Vector2 gridCoord = new Vector2(x, y);
				go.transform.position = GetHexWorldPos(gridCoord);
				go.transform.SetParent (this.transform);
				
				hex.HexRow = x - Mathf.FloorToInt (y/2);
				hex.HexColumn = y;
				SetHexagonToArray(x, y, go.GetComponent<Hexagon>());
				go.name = "HexPos: " + hex.HexRow + " " + hex.HexColumn; 
			}
		}
	}

	/// <summary>
	/// Calculates the starting point of the hex grid at the top left
	/// </summary>
	protected Vector3 CalculateStartPoint() {
		Vector3 startPoint; 
		startPoint = new Vector3(-HexWidth * GridHexRows / 2f + HexWidth / 2, 0, GridHexColumns / 2f * HexHeight - HexHeight / 2);
		return startPoint;
	}
	
	/// <summary>
	/// Find where in the world the hex should be created based on a grid coordinate
	/// </summary>
	protected Vector3 GetHexWorldPos(Vector2 HexGridCoordinate) {
		Vector3 startPoint = CalculateStartPoint();
		float offset = 0;
		if (HexGridCoordinate.y % 2 != 0)
			offset = HexWidth / 2;
		
		float x = startPoint.x + offset + HexGridCoordinate.x * HexWidth;
		float z = startPoint.z - HexGridCoordinate.y * HexHeight * 0.75f;
		return new Vector3(x, 0, z);
	}

	/// <summary>
	/// Sets the sizes of the hexagon based on the prefab, also allocates the array
	/// </summary>
	protected void SetHexSizes() {
		HexHeight = HexPrefab.renderer.bounds.size.z;
		HexWidth = HexPrefab.renderer.bounds.size.x;
		
		//Allocate the array of arrays for HexagonArray matching the grid size
		HexagonArray = new Hexagon[GridHexColumns*GridHexRows];
	}




//	protected List<Hexagon> HighlightedHexagonList = new List<Hexagon>(); 	//List of currently highlighted hexagons
	
/// <summary>
/// Draw a line between two hexagons, was used for testing and probably wont be needed
/// </summary>
//	Hexagon lineHexStart;
//	public void DrawLineBetweenHexagons(Hexagon hex1, Hexagon hex2) {
//		if (lineHexStart)
//			lineHexStart = null;
//		foreach (Hexagon hex in HighlightedHexagonList) {
//			if (hex != null)
//				hex.StopHighlight();
//		}
//		HighlightedHexagonList.Clear ();
//		
//		int distance = (int)DistanceBetweenHexagons (hex1, hex2);
//		for (int i = 0; i <= distance; i++) {
//			HighlightedHexagonList.Add (CubeToHex(CubeRound(CubeLerp(hex1, hex2, 1.0f/distance * i))));
//		}
//		
//		foreach(Hexagon hex in HighlightedHexagonList) {
//			if (hex != null) 
//				hex.Highlight ();
//		}
//	}

/// <summary>
/// Cube to Hex coordinate conversion
/// </summary>
//	protected Hexagon CubeToHex(Vector3 v) {
//		return GetHexagonFromArray((int)v.x, (int)v.y);
//	}

/// <summary>
///	Round the cubic coordinates to find a hex position and returns the cubic coordinates
/// </summary>
//	protected Vector3 CubeRound(Vector3 v) {
//		var rx = Mathf.Round(v.x);
//		var ry = Mathf.Round(v.y);
//		var rz = Mathf.Round(v.z);
//		
//		var x_diff = Mathf.Abs(rx - v.x);
//		var y_diff = Mathf.Abs(ry - v.y);
//		var z_diff = Mathf.Abs(rz - v.z);
//		
//		if (x_diff > y_diff && x_diff > z_diff)
//			rx = -ry-rz;
//		else if (y_diff > z_diff)
//			ry = -rx-rz;
//		else
//			rz = -rx-ry;
//		
//		return new Vector3(rx, ry, rz);
//	}

/// <summary>
/// 
/// </summary>
//	protected Vector3 CubeLerp(Hexagon a, Hexagon b, float t) {
//		return new Vector3(a.HexRow + (b.HexRow - a.HexRow) * t,
//		                   a.HexColumn + (b.HexColumn - a.HexColumn) * t,
//		                   a.z + (b.z - a.z) * t);
//	}

/// <summary>
/// The distance between two hexagons
/// </summary>
//	protected Hexagon distanceHexStart;
//	public float DistanceBetweenHexagons(Hexagon hex1, Hexagon hex2) {
//		distanceHexStart = null;
//		return (Mathf.Abs(hex1.HexRow - hex2.HexRow) 
//		        + Mathf.Abs (hex1.HexRow + hex1.HexColumn - hex2.HexRow - hex2.HexColumn)
//		        + Mathf.Abs (hex1.HexColumn - hex2.HexColumn)) / 2; 
//	}

/// <summary>
/// Highlights neighbors to a certain hexagon
/// </summary>
//	public void HighlightNeighbors(int ArrayHexRow, int ArrayHexColumn) {
//		foreach (Hexagon curr in HighlightedHexagonList) {
//			if (curr != null)
//				curr.StopHighlight();
//		}
//
//		Hexagon h;
//		HighlightedHexagonList.Clear ();
//
//		if (HexagonHighlightable(h = GetHexagonFromArray(ArrayHexRow, ArrayHexColumn-1)) && h.HexRow == ArrayHexRow && h.HexColumn == ArrayHexColumn-1)
//			HighlightedHexagonList.Add (h);
//		if (HexagonHighlightable(h = GetHexagonFromArray(ArrayHexRow+1, ArrayHexColumn-1)) && h.HexRow == ArrayHexRow+1 && h.HexColumn == ArrayHexColumn-1)
//			HighlightedHexagonList.Add (h);
//		if (HexagonHighlightable(h = GetHexagonFromArray(ArrayHexRow+1, ArrayHexColumn)) && h.HexRow == ArrayHexRow+1 && h.HexColumn == ArrayHexColumn)
//			HighlightedHexagonList.Add (h);
//		if (HexagonHighlightable(h = GetHexagonFromArray(ArrayHexRow, ArrayHexColumn+1)) && h.HexRow == ArrayHexRow && h.HexColumn == ArrayHexColumn+1)
//			HighlightedHexagonList.Add (h);
//		if (HexagonHighlightable(h = GetHexagonFromArray(ArrayHexRow-1, ArrayHexColumn+1)) && h.HexRow == ArrayHexRow-1 && h.HexColumn == ArrayHexColumn+1) 
//			HighlightedHexagonList.Add (h);
//		if (HexagonHighlightable(h = GetHexagonFromArray(ArrayHexRow-1, ArrayHexColumn)) && h.HexRow == ArrayHexRow-1 && h.HexColumn == ArrayHexColumn)
//			HighlightedHexagonList.Add (h);
//
//		foreach (Hexagon curr in HighlightedHexagonList) {
//			if (curr != null)
//				curr.Highlight();
//		}
//	}

/// <summary>
/// Check if the hexagon can/should be highlighted
/// </summary>
//	protected bool HexagonHighlightable(Hexagon hex) {
//		if (hex == null)
//			return false;
//
//		if (hex.CurrentHexType == Hexagon.HexType.Null)
//			return false;
//
//		return true;
//	}

#if UNITY_EDITOR	
	/// <summary>
	/// Loads a map from an index
	/// </summary>
	public Map LoadMap(int index) {
		if (Maps.Count > index) {
			if (Maps[index] == null)
				Debug.Log ("No map found at slot: " + index);
			return Maps[index];
		}
		else Debug.Log ("No map found at index: " + index);
		return null;
	}
	
	/// <summary>
	/// Saves the map to a specific index
	/// </summary>
	public void SaveMap(int index, Map newMap) {
		if (Maps.Count > index) {
			Maps[index] = newMap;
			Debug.Log ("Saving map at index: " + index);
		}
		else {
			Maps.Add (newMap);
			Debug.Log ("Creating new map to index: " + index);
		}
		EditorUtility.SetDirty(this); 
	}
	
	/// <summary>
	/// Saves a new map
	/// </summary>
	public void SaveNewMap(Map newMap) {
		for (int i=0; i < Maps.Count; i++) {
			if (Maps[i] == null) {
				Maps.Add(newMap);
				Debug.Log ("Creating new map to index: " + (i));
				return;
			}
		}
		Maps.Add (newMap);
		Debug.Log ("Creating new map to index: " + (Maps.Count-1));
	}
	
	/// <summary>
	/// Deletes a map at the index
	/// </summary>
	public void RemoveMap(int index) {
		Maps[index] = null;
	}
#endif
}
