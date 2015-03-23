/////////////////////////////////////////////////////////////////////////////////
//
//	BoardManager.cs
//	© EternalVR, All Rights Reserved
//
//	description:	class for creating and managing the hexagonal grid functionality
//					including the generation of it, and mathematical interactions
//					like finding neighbors, LOS, etc
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

[ExecuteInEditMode]
[System.Serializable]
public class BoardManager : MonoBehaviour {
	
	//The hex prefab and how many we want rows/columns
	public GameObject HexPrefab; //Pulled from Resources at grid creation
	public int GridHexRows = 6;
	public int GridHexColumns = 6;
	
	//Height/Width of the hexagon we are making used for positioning the hexagons
	[SerializeField]
	private float HexHeight;
	[SerializeField]
	private float HexWidth;

	protected List<Hexagon> HighlightedHexagonList = new List<Hexagon>(); //List of currently highlighted hexagons
	protected Hexagon CurrentlySelectedHexagon; //Hexagon currently selected
	[SerializeField]
	protected int HexTargetMask; //layer mask for the hexagons

	public static BoardManager instance; //Should only have 1 BoardManager

	public Hexagon [] HexagonArray; //The array of hexagons on the grid, done with a 1d array so it can be serializable for editting in edit mode
	public Hexagon GetHexagonFromArray(int x, int y) {
		x = x + Mathf.FloorToInt (y/2);
		if (OutOfArray(x, y)) {
			return null;
		}
		return HexagonArray[y * GridHexColumns + x] != null ? HexagonArray[y * GridHexColumns + x] : null;
	}
	public void SetHexagonToArray(int x, int y, Hexagon hexData) {
		HexagonArray[y * GridHexColumns + x] = hexData;
	}
	public int ArrayLength() {
		if (HexagonArray != null) {
			return HexagonArray.Length;
		}
		else return 0;
	}
	
	void Awake() {
		instance = this; //Set singleton
	}

	/// <summary>
	/// Recreate the grid, called from the inspector
	/// </summary>
	public bool initialized;
	public void Recreate() {
		HexPrefab = Resources.Load ("HexPrefab") as GameObject;
		if (HexPrefab == null) {
			Debug.LogError ("Add Hexagon Prefab to \"/Assets/Resources/\" with the name \"HexPrefab\" or change the path in BoardManager.cs");
			return;
		}
		if (GridHexColumns == 0 || GridHexRows == 0) {
			Debug.LogError ("Need to set more than 1 Column and Row of Hexagons on Board Manager to be created.");
			return;
		}
		DestroyHexGrid();
		SetHexSizes ();
		CreateHexGrid();
		CreateHexLayerMask ();
		initialized = true;
	}
	
	/// <summary>
	/// Destroys the hexgrid so it can be recreated from scratch
	/// </summary>
	protected void DestroyHexGrid() {
		List<Transform> children = new List<Transform>();
		foreach (Transform c in transform) {
			children.Add (c);
		}
		foreach (Transform c in children) { //Cant destroy objects while in the foreach, have to do it seperately
			DestroyImmediate (c.gameObject);
		}
	}

	/// <summary>
	/// Sets the sizes of the hexagon based on the prefab, also allocates the array
	/// </summary>
	protected void SetHexSizes() {
		HexHeight = HexPrefab.renderer.bounds.size.z;
		HexWidth = HexPrefab.renderer.bounds.size.x;
		
		//Allocate the array of arrays for HexagonArray matching the grid size
		HexagonArray = new Hexagon[GridHexRows*GridHexColumns];
	}

	/// <summary>
	/// Creates each hexagon, positions it, and places it in the array
	/// </summary>
	protected void CreateHexGrid() {
		for (int y = 0 ; y < GridHexRows; y++) {
			for (int x = 0; x < GridHexColumns; x++) {
				GameObject go = Instantiate (HexPrefab) as GameObject;
				Hexagon hex = go.GetComponent<Hexagon>();
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
	/// Creates the hexagon targetting layer mask, if something should be tested for collision with the raycast
	/// it should be added here (I can't forsee needing anything else, but its here)
	/// </summary>
	public void CreateHexLayerMask() {
		int Layer1 = 10; //Hexagon
		int LayerMask1 = 1 << Layer1;
		HexTargetMask = LayerMask1; //... | LayerMask2 | LayerMask3;
	}
	
	/// <summary>
	/// Draw a line between two hexagons, was used for testing and probably wont be needed
	/// </summary>
	Hexagon lineHexStart;
	public void DrawLineBetweenHexagons(Hexagon hex1, Hexagon hex2) {
		if (lineHexStart)
			lineHexStart = null;
		foreach (Hexagon hex in HighlightedHexagonList) {
			if (hex != null)
				hex.StopHighlight();
		}
		HighlightedHexagonList.Clear ();
		
		int distance = (int)DistanceBetweenHexagons (hex1, hex2);
		for (int i = 0; i <= distance; i++) {
			HighlightedHexagonList.Add (CubeToHex(CubeRound(CubeLerp(hex1, hex2, 1.0f/distance * i))));
		}
		
		foreach(Hexagon hex in HighlightedHexagonList) {
			if (hex != null) 
				hex.Highlight ();
		}
	}

	/// <summary>
	/// Cube to Hex coordinate conversion
	/// </summary>
	protected Hexagon CubeToHex(Vector3 v) {
		return GetHexagonFromArray((int)v.x, (int)v.y);
	}

	/// <summary>
	///	Round the cubic coordinates to find a hex position and returns the cubic coordinates
	/// </summary>
	protected Vector3 CubeRound(Vector3 v) {
		var rx = Mathf.Round(v.x);
		var ry = Mathf.Round(v.y);
		var rz = Mathf.Round(v.z);
		
		var x_diff = Mathf.Abs(rx - v.x);
		var y_diff = Mathf.Abs(ry - v.y);
		var z_diff = Mathf.Abs(rz - v.z);
		
		if (x_diff > y_diff && x_diff > z_diff)
			rx = -ry-rz;
		else if (y_diff > z_diff)
			ry = -rx-rz;
		else
			rz = -rx-ry;
		
		return new Vector3(rx, ry, rz);
	}

	/// <summary>
	/// 
	/// </summary>
	protected Vector3 CubeLerp(Hexagon a, Hexagon b, float t) {
		return new Vector3(a.HexRow + (b.HexRow - a.HexRow) * t,
		                   a.HexColumn + (b.HexColumn - a.HexColumn) * t,
		                   a.z + (b.z - a.z) * t);
	}
	
	/// <summary>
	/// The distance between two hexagons
	/// </summary>
	protected Hexagon distanceHexStart;
	public float DistanceBetweenHexagons(Hexagon hex1, Hexagon hex2) {
		distanceHexStart = null;
		return (Mathf.Abs(hex1.HexRow - hex2.HexRow) 
		        + Mathf.Abs (hex1.HexRow + hex1.HexColumn - hex2.HexRow - hex2.HexColumn)
		        + Mathf.Abs (hex1.HexColumn - hex2.HexColumn)) / 2; 
	}
	
	/// <summary>
	/// Highlights neighbors to a certain hexagon
	/// </summary>
	public void HighlightNeighbors(int ArrayHexRow, int ArrayHexColumn) {
		foreach (Hexagon curr in HighlightedHexagonList) {
			if (curr != null)
				curr.StopHighlight();
		}

		Hexagon h;
		HighlightedHexagonList.Clear ();

		if (HexagonHighlightable(h = GetHexagonFromArray(ArrayHexRow, ArrayHexColumn-1)) && h.HexRow == ArrayHexRow && h.HexColumn == ArrayHexColumn-1)
			HighlightedHexagonList.Add (h);
		if (HexagonHighlightable(h = GetHexagonFromArray(ArrayHexRow+1, ArrayHexColumn-1)) && h.HexRow == ArrayHexRow+1 && h.HexColumn == ArrayHexColumn-1)
			HighlightedHexagonList.Add (h);
		if (HexagonHighlightable(h = GetHexagonFromArray(ArrayHexRow+1, ArrayHexColumn)) && h.HexRow == ArrayHexRow+1 && h.HexColumn == ArrayHexColumn)
			HighlightedHexagonList.Add (h);
		if (HexagonHighlightable(h = GetHexagonFromArray(ArrayHexRow, ArrayHexColumn+1)) && h.HexRow == ArrayHexRow && h.HexColumn == ArrayHexColumn+1)
			HighlightedHexagonList.Add (h);
		if (HexagonHighlightable(h = GetHexagonFromArray(ArrayHexRow-1, ArrayHexColumn+1)) && h.HexRow == ArrayHexRow-1 && h.HexColumn == ArrayHexColumn+1) 
			HighlightedHexagonList.Add (h);
		if (HexagonHighlightable(h = GetHexagonFromArray(ArrayHexRow-1, ArrayHexColumn)) && h.HexRow == ArrayHexRow-1 && h.HexColumn == ArrayHexColumn)
			HighlightedHexagonList.Add (h);

		foreach (Hexagon curr in HighlightedHexagonList) {
			if (curr != null)
				curr.Highlight();
		}
	}
	
	/// <summary>
	/// Check if the hexagon can/should be highlighted
	/// </summary>
	protected bool HexagonHighlightable(Hexagon hex) {
		if (hex == null)
			return false;

		if (hex.CurrentHexType == Hexagon.HexType.Null)
			return false;

		return true;
	}
	
	/// <summary>
	/// Raycasts to find/select a hexagon
	/// Will need this for the game itself, since the editor uses a different ray to raycast with
	/// </summary>
//	protected Hexagon RaycastHexagon() {
//		RaycastHit hit;
//		Ray ray = RaycastCamera.ScreenPointToRay(Input.mousePosition); //Create a ray from camera  mouse position
//		if (Physics.Raycast (ray, out hit, 100, HexTargetMask)) //If an object is found
//		{
//			if (hit.collider.GetComponent<Hexagon>()) {
//				//Debug.Log (objectSelectionRay.collider.name); //Log the name of the object
//				return hit.collider.GetComponent<Hexagon>(); //Return the game object as a GameObject
//			}
//			else return null;
//		}
//		else {
//			return null;
//		}
//	}
	
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

	#region MapEditor
	#if UNITY_EDITOR
	public List<Material> textures = new List<Material>(); //Textures to be painted
	public Material newTexture; //Texture to be added to list of textures

	protected int hexTypeMode = 0; //0 = HexTypeMode, 1 = HexTextureMode
	public int HexTypeMode {
		get {
			return hexTypeMode;
		}
		set {
			SetHexViewMode(value);
			hexTypeMode = value;
		}
	}
	public Hexagon.HexType CurrentBrushType = Hexagon.HexType.Normal;
	public int textureBrushIndex = 0;

	public bool inDebug = false; //Debug mode in editor
	public bool collapse = false; 
	public bool recreateConfirmation = false;
	
	void OnGUI() {
		if (Event.current.type == EventType.Layout || Event.current.type == EventType.Repaint) {
			EditorUtility.SetDirty(this); // needed for whatever reason
		}
		else if (Event.current.type == EventType.MouseDown)	{
			if (Event.current.button == 0) {
				if((CurrentlySelectedHexagon = EditorRaycastHexagon()) != null) {
					if (hexTypeMode == 0) {
						CurrentlySelectedHexagon.CurrentHexType = (CurrentBrushType);
					}
					else if (hexTypeMode == 1) {
						CurrentlySelectedHexagon.CurrentBrushTexture = (textures[textureBrushIndex]);
					}
				}
			}
			else if (Event.current.button == 1) {
				if((CurrentlySelectedHexagon = EditorRaycastHexagon()) != null) {
					HighlightNeighbors(CurrentlySelectedHexagon.HexRow, CurrentlySelectedHexagon.HexColumn);
				}
			}
		}
		else if (Event.current.type == EventType.KeyDown) {
			if (Event.current.keyCode == KeyCode.D) {
				if ((CurrentlySelectedHexagon = EditorRaycastHexagon ()) != null) {
					if (distanceHexStart == null) 
						distanceHexStart = CurrentlySelectedHexagon;
					else Debug.Log (DistanceBetweenHexagons(distanceHexStart, CurrentlySelectedHexagon));
				}
			}
			
			if (Event.current.keyCode == KeyCode.Q) {
				foreach (Hexagon h in HighlightedHexagonList) {
					h.StopHighlight ();
				}
			}
		}
	}

	/// <summary>
	/// Adds a new texture to the list of textures
	/// </summary>
	public void AddNewTexture() {
		if (newTexture == null)
			return;
		textures.Add (newTexture);
		newTexture = null;
	}

	/// <summary>
	/// Removes the currently selected texture
	/// </summary>
	public void RemoveCurrentTexture() {
		textures.RemoveAt (textureBrushIndex);
		textureBrushIndex = 0;
	}

	/// <summary>
	/// Sets the hex view mode to view the map based on its types or its looks
	/// </summary>
	protected void SetHexViewMode(int mode) {
		if (mode == 0) { //Set to type mode
			foreach (Hexagon h in HexagonArray) {
				h.SetToType();
			}
		}
		else if (mode == 1) { //Set to texture mode
			foreach (Hexagon h in HexagonArray) {
				h.SetToTexture();
			}
		}
	}
	
	/// <summary>
	/// Raycasts to find/select a hexagon
	/// </summary>
	protected Hexagon EditorRaycastHexagon() {
		RaycastHit hit;
		Camera.SetupCurrent(Camera.main); //Make sure we have the camera set or raycasts will be missed
		Ray ray = HandleUtility.GUIPointToWorldRay(Event.current.mousePosition);
		if (ray.origin == Vector3.zero) {
			Debug.LogError ("having issues finding current camera");
			return null;
		}
		if (Physics.Raycast (ray, out hit, 100, HexTargetMask)) //If an object is found
		{
			if (hit.collider.GetComponent<Hexagon>()) {
				return hit.collider.GetComponent<Hexagon>(); //Return the game object as a GameObject
			}
			else return null;
		}
		else {
//			Debug.Log ("raycast broke as shit");
			return null;
		}
	}
	#endif
	#endregion
}
