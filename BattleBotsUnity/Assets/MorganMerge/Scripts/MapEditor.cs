/////////////////////////////////////////////////////////////////////////////////
//
//	MapEditor.cs
//	© EternalVR, All Rights Reserved
//
//	description:	class for creating and editing Hexagon "Maps" for saving and loading
//
//	sources:		http://www.redblobgames.com/grids/hexagons/ used for several 
//					formulas for hexagon map storage and array calculations
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

#if UNITY_EDITOR
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

[ExecuteInEditMode]
[System.Serializable]
public class MapEditor : MonoBehaviour {

	public GameObject hexPrefab; 											//Pulled from Resources at grid creation
	public GameObject HexPrefab {											//Getter incase MapEditor prefab is reset and hexPrefab doesnt get initialized when trying to load
		get {
			if (hexPrefab == null) 
				hexPrefab = Resources.Load ("HexPrefab") as GameObject;
			return hexPrefab;
		}
		set { hexPrefab = value; }
	}
	public int GridHexRows = 6;												//How many rows to generate or have been generated
	public int GridHexColumns = 6;											//How many columns to generate or have been generated
	public bool initialized;												//If the grid has been initialized (check for saving map)
	public HexagonData[] newHexData;										//HexData populated when the user loads a map
	public int mapSliderIndex;												//A slider for what map we are seleced in the editor
	public List<Material> textures = new List<Material>();					//Textures to be painted
	public Material newTexture; 											//Texture to be added to list of textures
	public Hexagon.HexType CurrentBrushType = Hexagon.HexType.Normal;		//Current type of HexType being brushed on
	public Hexagon.SpawnType CurrentBrushSpawn = Hexagon.SpawnType.None;	//What type of spawn we are applying to the map
	public int textureBrushIndex = 0;										//Index of the texture being brushed on
	public bool inDebug = false;											//Debug mode in editor
	public bool collapse = false; 											//Collapse flag for debugging the array
	public bool recreateConfirmation = false;								//A bool used to setting a confirmation in the editor
	public int HexTypeMode {												//Used to set the HexType being viewed from editor
		get {
			return hexTypeMode;
		}
		set {
			SetHexViewMode(value);
			hexTypeMode = value;
		}
	}
	public BoardManager boardManager {
		get {
			return GameObject.Find ("BoardManager").GetComponent<BoardManager>();
		}
	}

	protected Hexagon CurrentlySelectedHexagon; 							//Hexagon currently selected
	[SerializeField]
	protected int HexTargetMask;											//layer mask for the hexagons
	protected int hexTypeMode = 0;											//0 = HexTypeMode, 1 = HexTextureMode
	protected Map currentMap;												//the currently loaded map used for saving and loading
	[SerializeField]
	protected float HexHeight;												//the height of the hex prefab
	[SerializeField]
	protected float HexWidth;												//the width of the hex prefab
	
	public static MapEditor instance;										//Should only have 1 MapEditor at a time
	
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
	/// 
	/// </summary>
	void Awake() {
		instance = this; 
	}

	/// <summary>
	/// Recreate the grid, called from the inspector
	/// </summary>
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
		initialized = true;
	}

	/// <summary>
	/// Creates the hexagon targetting layer mask, if something should be tested for collision with the raycast
	/// it should be added here (I can't forsee needing anything else, but its here)
	/// </summary>
	public void CreateHexLayerMask() {
		if (LayerMask.LayerToName(10) != "Hexagon") {
			Debug.LogError ("Layer 10 Needs to be named \"Hexagon\"");
		}
		int Layer1 = 10; //Hexagon
		int LayerMask1 = 1 << Layer1;
		HexTargetMask = LayerMask1; //... | LayerMask2 | LayerMask3;
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
	/// Loads a map and creates it
	/// </summary>
	public void LoadMap() {
		Map m = boardManager.LoadMap(mapSliderIndex);
		newHexData = m.GetMap ();
		GridHexColumns = m.GridWidth;
		GridHexRows = m.GridHeight;
		currentMap = m;
		SetHexSizes ();
		DestroyHexGrid ();
		CreateHexGridFromHexagonData();
	}

	/// <summary>
	/// Saves the map, overwriting the currently selected save
	/// </summary>
	public void SaveMap() {
//		Map m = new Map();
		Map m = ScriptableObject.CreateInstance <Map>();
		m.SaveIndex = mapSliderIndex;
		m.GridWidth = GridHexColumns;
		m.GridHeight = GridHexRows;
		m.SetHexagons (HexagonArray);
		boardManager.SaveMap(mapSliderIndex, m);
	}

	/// <summary>
	/// Deletes the map.
	/// </summary>
	public void DeleteMap() {
		currentMap = new Map();
		boardManager.RemoveMap(mapSliderIndex);
	}

	/// <summary>
	/// Saves a new map instead of overwriting an existing one
	/// </summary>
	public void SaveNewMap() {
		Map m = new Map();
		m.SaveIndex = mapSliderIndex;
		m.GridWidth = GridHexColumns;
		m.GridHeight = GridHexRows;
		m.SetHexagons (HexagonArray);
		boardManager.SaveNewMap(m);
	}

	/// <summary>
	/// Creates a hex grid using a new list of HexagonData
	/// </summary>
	protected void CreateHexGridFromHexagonData() {
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
		initialized = true;
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
		else if (mode == 2) {
			foreach (Hexagon h in HexagonArray) {
				h.SetToSpawn();
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
			return null;
		}
	}

	void OnGUI() {
		if (EditorApplication.isPlaying) //Only edit in play mode
			return;
		
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
						if (textures.Count == 0) {
							Debug.LogWarning ("No textures found to use, try adding one where it says \"Drag New Materials Here\"");
							return;
						}
						CurrentlySelectedHexagon.CurrentBrushTexture = (textures[textureBrushIndex]);
					}
					else if (hexTypeMode == 2) {
						CurrentlySelectedHexagon.CurrentSpawnType = CurrentBrushSpawn; 
					}
				}
			}
			else if (Event.current.button == 1) {
//				if((CurrentlySelectedHexagon = EditorRaycastHexagon()) != null) {
//					HighlightNeighbors(CurrentlySelectedHexagon.HexRow, CurrentlySelectedHexagon.HexColumn);
//				}
			}
		}
		else if (Event.current.type == EventType.KeyDown) {			
//			if (Event.current.keyCode == KeyCode.Q) {
//				foreach (Hexagon h in HighlightedHexagonList) {
//					h.StopHighlight ();
//				}
//			}
		}
	}
}
#endif
