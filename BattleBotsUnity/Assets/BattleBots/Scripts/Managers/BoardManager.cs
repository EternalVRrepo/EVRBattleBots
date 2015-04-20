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
#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class BoardManager : MonoBehaviour {

	public GameObject HexPrefab;					//Prefab for the hexagons used for map generation
	[SerializeField]
	public List<Map> Maps; 							//List of stored maps on this BoardManager
	public static Map CurrentMap;					//Map currently being used
	public static BoardManager instance {
		get {
			if (_instance == null) {
				_instance = GameObject.FindObjectOfType<BoardManager>();
				DontDestroyOnLoad (_instance);
			}
			return _instance;
		}
	}

	protected float HexWidth;						//Width of a hex prefab
	protected float HexHeight;						//Height of a hex prefab
	protected int GridHexColumns {					//How many columns on the map
		get { return CurrentMap.GridWidth; }
	}
	protected int GridHexRows {						//How many rows on the map
		get { return CurrentMap.GridHeight; }
	}
	protected static BoardManager _instance;

	[SerializeField]
	private Hexagon [] HexagonArray;				//The array of hexagons on the grid, done with a 1d array so it can be serializable for editting in edit mode
	
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
	/// Public getter for the array length
	/// </summary>
	public int GetArrayLength() {
		if (HexagonArray != null) {
			return HexagonArray.Length;
		}
		else return 0;
	}

	/// <summary>
	/// If the x,y coord is in the scope of the array
	/// This only works in the case of a rectangular grid, which it should always be
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
	if (_instance == null) {
		_instance = this;
		DontDestroyOnLoad(this);
	}
	else {
		if (this != _instance)
			Destroy (this.gameObject);
	}
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
				go.transform.position += Vector3.up*hex.HexHeight*.2f;
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

	#region CombatMethods

	public List<Hexagon> HighlightedHexagons = new List<Hexagon>(); //List of hexagons being highlighted right now
	
	protected Queue<Hexagon> frontier = new Queue<Hexagon>(); //We need two queues to switch between as we go through each layer of frontier, to keep track of distance
	protected Queue<Hexagon> distanceQueue = new Queue<Hexagon>();
	protected List<Hexagon> visited = new List<Hexagon>();

	/// <summary>
	/// Highlights hexagons within currentMoveDistance from currentlyOccupiedHexagon using a Breadth First Search
	/// </summary>
	public void HighlightMovement (int currentMoveDistance, Hexagon currentlyOccupiedHexagon)
	{
		visited.Clear ();
		frontier.Clear ();
		distanceQueue.Clear ();
		frontier.Enqueue (currentlyOccupiedHexagon); //Starting hex

		Queue<Hexagon> activeQueue = frontier;
		Queue<Hexagon> inactiveQueue = distanceQueue;

		int d = 1;
		while (activeQueue.Count > 0 && currentMoveDistance > 0) { //Breadth first search
			Hexagon curr = activeQueue.Dequeue ();
			foreach (Hexagon h in GetNeighborsMovement(curr)) {
				if (!visited.Contains (h)) {
					h.CurrentDistance = d;
					inactiveQueue.Enqueue (h);
					visited.Add (h);
				}
			}

			if (activeQueue.Count == 0) { //Switching between active and inactive queues to track distance
				d++;
				Queue<Hexagon> t = activeQueue;
				activeQueue = inactiveQueue;
				inactiveQueue = t;
				currentMoveDistance--;
			}
		}
		currentlyOccupiedHexagon.CurrentDistance = 0;
		visited.Remove (currentlyOccupiedHexagon);
		foreach (Hexagon h in visited) { //Highlight the hexagons we found
			h.Highlight();
			HighlightedHexagons.Add (h);
		}

	}

	/// <summary>
	/// Gets the path from the start to end hexagon using an AStarSearch
	/// </summary>
	public List<Hexagon> GetPath(Hexagon start, Hexagon end, Hexagon AIHex = null) {
		AStarSearch astar;
		astar = new AStarSearch(start, end, AIHex);
		if (!astar.FoundPath) //Couldnt find a path 
			return null;
		List<Hexagon> path = new List<Hexagon>();

		Hexagon h = end;
		path.Add (h);
		while (h != start) { //Backtrack the path from the end to the start then reverse the list
			astar.cameFrom.TryGetValue (h, out h);
			path.Add (h);
		}
		path.Reverse ();
		return path;
	}
	
	/// <summary>
	/// Returns where or not a hexagon can be moved to
	/// </summary>
	public bool CanMove(Hexagon h) {
		if (h.CurrentDistance == -1)
			return false;

		if (h.IsOccupied())
			return false;

		return true;
	}

	/// <summary>
	/// Finishes the movement highlighting.
	/// </summary>
	public void FinishMovement() {
		foreach (Hexagon h in HighlightedHexagons) {
			h.CurrentDistance = -1;
			h.StopHighlight ();
		}
	}

	/// <summary>
	/// Returns a list of all hexagons neighboring the passed start hexagon
	/// </summary>
	public List<Hexagon> GetNeighborsMovement(Hexagon start, Hexagon AITarget = null) {
		List<Hexagon> NeighborHexList = new List<Hexagon>();
		int ArrayHexRow = start.HexRow;
		int ArrayHexColumn = start.HexColumn;
		Hexagon h;

		//The formula for the 6 neighboring hexagons, also checks if they can be moved to
		if (HexagonMoveable(h = GetHexagonFromArray(ArrayHexRow, ArrayHexColumn-1), start, AITarget) && h.HexRow == ArrayHexRow && h.HexColumn == ArrayHexColumn-1)
			NeighborHexList.Add (h);
		if (HexagonMoveable(h = GetHexagonFromArray(ArrayHexRow+1, ArrayHexColumn-1), start, AITarget) && h.HexRow == ArrayHexRow+1 && h.HexColumn == ArrayHexColumn-1)
			NeighborHexList.Add (h);
		if (HexagonMoveable(h = GetHexagonFromArray(ArrayHexRow+1, ArrayHexColumn), start, AITarget) && h.HexRow == ArrayHexRow+1 && h.HexColumn == ArrayHexColumn)
			NeighborHexList.Add (h);
		if (HexagonMoveable(h = GetHexagonFromArray(ArrayHexRow, ArrayHexColumn+1), start, AITarget) && h.HexRow == ArrayHexRow && h.HexColumn == ArrayHexColumn+1)
			NeighborHexList.Add (h);
		if (HexagonMoveable(h = GetHexagonFromArray(ArrayHexRow-1, ArrayHexColumn+1), start, AITarget) && h.HexRow == ArrayHexRow-1 && h.HexColumn == ArrayHexColumn+1) 
			NeighborHexList.Add (h);
		if (HexagonMoveable(h = GetHexagonFromArray(ArrayHexRow-1, ArrayHexColumn), start, AITarget) && h.HexRow == ArrayHexRow-1 && h.HexColumn == ArrayHexColumn)
			NeighborHexList.Add (h);

		return NeighborHexList;
	}

	/// <summary>
	/// Returns a list of all hexagons neighboring the passed start hexagon
	/// </summary>
//	public List<Hexagon> GetNeighborsEnemies(Hexagon start) {
//		List<Hexagon> NeighborHexList = new List<Hexagon>();
//		int ArrayHexRow = start.HexRow;
//		int ArrayHexColumn = start.HexColumn;
//		Hexagon h;
//		
//		//The formula for the 6 neighboring hexagons, also checks if they can be moved to
//		if (HexagonTargettable(h = GetHexagonFromArray(ArrayHexRow, ArrayHexColumn-1)) && h.HexRow == ArrayHexRow && h.HexColumn == ArrayHexColumn-1)
//			NeighborHexList.Add (h);
//		if (HexagonTargettable(h = GetHexagonFromArray(ArrayHexRow+1, ArrayHexColumn-1)) && h.HexRow == ArrayHexRow+1 && h.HexColumn == ArrayHexColumn-1)
//			NeighborHexList.Add (h);
//		if (HexagonTargettable(h = GetHexagonFromArray(ArrayHexRow+1, ArrayHexColumn)) && h.HexRow == ArrayHexRow+1 && h.HexColumn == ArrayHexColumn)
//			NeighborHexList.Add (h);
//		if (HexagonTargettable(h = GetHexagonFromArray(ArrayHexRow, ArrayHexColumn+1)) && h.HexRow == ArrayHexRow && h.HexColumn == ArrayHexColumn+1)
//			NeighborHexList.Add (h);
//		if (HexagonTargettable(h = GetHexagonFromArray(ArrayHexRow-1, ArrayHexColumn+1)) && h.HexRow == ArrayHexRow-1 && h.HexColumn == ArrayHexColumn+1) 
//			NeighborHexList.Add (h);
//		if (HexagonTargettable(h = GetHexagonFromArray(ArrayHexRow-1, ArrayHexColumn)) && h.HexRow == ArrayHexRow-1 && h.HexColumn == ArrayHexColumn)
//			NeighborHexList.Add (h);
//		
//		return NeighborHexList;
//	}

	/// <summary>
	/// Check if a hexagon is a player character
	/// </summary>
	public bool HexagonTargettable(Hexagon h) {

		if (h.OccupiedUnit is PlayerControlledBoardUnit)
			return true;

		return false;
	}

	/// <summary>
	/// Highlights all the hexagons in a distance of the designated type
	/// </summary>
	public List<PlayerControlledBoardUnit> GetEnemiesInRange(Hexagon currentlyOccupiedHexagon, int distance, AbilityDescription.TargetType targetType) {
		visited.Clear ();
		frontier.Clear ();
		distanceQueue.Clear ();
		frontier.Enqueue (currentlyOccupiedHexagon); //Starting hex
		visited.Add (currentlyOccupiedHexagon);
		currentlyOccupiedHexagon.DisableLOSCollider();
		
		Queue<Hexagon> activeQueue = frontier;
		Queue<Hexagon> inactiveQueue = distanceQueue;
		
		while (activeQueue.Count > 0 && distance > 0) { //Breadth first search
			Hexagon curr = activeQueue.Dequeue ();
			foreach (Hexagon h in GetNeighborsAbilityCast(curr)) {		
				if (inLOS(currentlyOccupiedHexagon, h) && !visited.Contains (h)) {
					inactiveQueue.Enqueue (h);
					visited.Add (h);
				}
			}
			
			if (activeQueue.Count == 0) { //Switching between active and inactive queues to track distance
				Queue<Hexagon> t = activeQueue;
				activeQueue = inactiveQueue;
				inactiveQueue = t;
				distance--;
			}
		}

		List<PlayerControlledBoardUnit> targets = new List<PlayerControlledBoardUnit>();
		foreach (Hexagon h in visited) { //Highlight the hexagons we found
			if (HexagonTargettable (h)) {
				targets.Add (h.OccupiedUnit as PlayerControlledBoardUnit);
			}
			h.EnableLOSCollider();
		}
		return targets;
	}

	/// <summary>
	/// Check if hexagon can be cast on with the current ability
	/// </summary>
	protected bool HexagonCastable(Hexagon hex, AbilityDescription.TargetType targetType) {
		if (hex == null)
			return false;

		if (hex.CurrentHexType == Hexagon.HexType.Null)
			return false;

		switch(targetType) {
		case AbilityDescription.TargetType.TargetAlly: {
				if (hex.OccupiedUnit is PlayerControlledBoardUnit)
					return true;
				break;
			}
		case AbilityDescription.TargetType.TargetEnemy: {
				if (hex.OccupiedUnit is NonPlayerControlledBoardUnit) 
					return true;
				break;
			}
		case AbilityDescription.TargetType.TargetUnit: {
				if (hex.OccupiedUnit is NonPlayerControlledBoardUnit || hex.OccupiedUnit is PlayerControlledBoardUnit)
					return true;
				break;
			}
		case AbilityDescription.TargetType.TargetHexagon: {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Check if the hexagon can be moved onto 
	/// </summary>
	protected bool HexagonMoveable(Hexagon end, Hexagon start, Hexagon AITarget) {
		if (end == null)
			return false;

		if (end.CurrentHexType == Hexagon.HexType.Null)
			return false;

		if (end.CurrentHexType == Hexagon.HexType.Impassable)
			return false;

		if (end.CurrentHexType == Hexagon.HexType.WalledImpassable)
			return false;

		if (Mathf.Abs (start.HexHeight - end.HexHeight) > 2)
			return false;

		if (AITarget == null) {
			if (end.IsOccupied())
				return false;
		}
		else {
			if (end.IsOccupied() && end != AITarget)
				return false;
		}

		return true;
	}

	/// <summary>
	/// Highlights hexagons for an ability
	/// </summary>
	public void HighlightAbility(Hexagon currentHexagon, AbilityDescription ability) {
		HighlightLOSNeighbors(currentHexagon, ability.castRange, ability.AbilityTargetType);
	}

	/// <summary>
	/// Highlights all the hexagons in a distance of the designated type
	/// </summary>
	public void HighlightLOSNeighbors(Hexagon currentlyOccupiedHexagon, int distance, AbilityDescription.TargetType targetType) {
		visited.Clear ();
		frontier.Clear ();
		distanceQueue.Clear ();
		frontier.Enqueue (currentlyOccupiedHexagon); //Starting hex
		visited.Add (currentlyOccupiedHexagon);
		currentlyOccupiedHexagon.DisableLOSCollider();

		Queue<Hexagon> activeQueue = frontier;
		Queue<Hexagon> inactiveQueue = distanceQueue;

		while (activeQueue.Count > 0 && distance > 0) { //Breadth first search
			Hexagon curr = activeQueue.Dequeue ();
			foreach (Hexagon h in GetNeighborsAbilityCast(curr)) {		
				if (inLOS(currentlyOccupiedHexagon, h) && !visited.Contains (h)) {
					inactiveQueue.Enqueue (h);
					visited.Add (h);
				}
			}
			
			if (activeQueue.Count == 0) { //Switching between active and inactive queues to track distance
				Queue<Hexagon> t = activeQueue;
				activeQueue = inactiveQueue;
				inactiveQueue = t;
				distance--;
			}
		}

		foreach (Hexagon h in visited) { //Highlight the hexagons we found
			if (HexagonCastable (h, targetType)) {
				h.Highlight();
				HighlightedHexagons.Add (h);
			}
			else h.EnableLOSCollider(); //If we cant cast on this hex, reenable the collider so we know its not a valid target
		}
	}

	/// <summary>
	/// Checks if a destination hexagon is in LOS of the source, it checks for a collider, if it exists it disables the collider
	/// so that it doesnt block future raycasts, and also indicates that the box is in LOS
	/// </summary>
	protected bool inLOS(Hexagon source, Hexagon destination) {
		RaycastHit hit;
		if (Physics.Linecast(source.transform.position, destination.transform.position, out hit, 1 << 11)) {
			if (hit.transform.parent.GetComponent<Hexagon>() && Mathf.Abs(source.HexHeight - hit.transform.parent.GetComponent<Hexagon>().HexHeight) > 3)
				return false;

			if (hit.transform.parent.GetComponent<Hexagon>() && LOSHexagon(hit.transform.parent.GetComponent<Hexagon>())) {
				destination.DisableLOSCollider();
				return true;
			}
		}

		return false;
	}

	/// <summary>
	/// Finishes the ability highlighting.
	/// </summary>
	public void FinishAbility() {
		foreach (Hexagon h in HighlightedHexagons) {
			h.EnableLOSCollider();
			h.StopHighlight ();
		}
	}

	/// <summary>
	/// Check if a hexagon can be seen over/through
	/// </summary>
	protected bool LOSHexagon(Hexagon h) {
		if (h.CurrentHexType == Hexagon.HexType.Null) 
			return true;

		if (h.CurrentHexType == Hexagon.HexType.Normal) 
			return true;

		if (h.CurrentHexType == Hexagon.HexType.Impassable) 
			return true;

		return false;
	}

	/// <summary>
	/// Returns a list of all hexagons neighboring the passed start hexagon
	/// </summary>
	protected List<Hexagon> GetNeighborsAbilityCast(Hexagon start) {
		List<Hexagon> NeighborHexList = new List<Hexagon>();
		int ArrayHexRow = start.HexRow;
		int ArrayHexColumn = start.HexColumn;
		Hexagon h;
		
		//The formula for the 6 neighboring hexagons, also checks if they can be moved to
		if ((h = GetHexagonFromArray(ArrayHexRow, ArrayHexColumn-1)) && h.HexRow == ArrayHexRow && h.HexColumn == ArrayHexColumn-1)
			NeighborHexList.Add (h);
		if ((h = GetHexagonFromArray(ArrayHexRow+1, ArrayHexColumn-1)) && h.HexRow == ArrayHexRow+1 && h.HexColumn == ArrayHexColumn-1)
			NeighborHexList.Add (h);
		if ((h = GetHexagonFromArray(ArrayHexRow+1, ArrayHexColumn)) && h.HexRow == ArrayHexRow+1 && h.HexColumn == ArrayHexColumn)
			NeighborHexList.Add (h);
		if ((h = GetHexagonFromArray(ArrayHexRow, ArrayHexColumn+1)) && h.HexRow == ArrayHexRow && h.HexColumn == ArrayHexColumn+1)
			NeighborHexList.Add (h);
		if ((h = GetHexagonFromArray(ArrayHexRow-1, ArrayHexColumn+1)) && h.HexRow == ArrayHexRow-1 && h.HexColumn == ArrayHexColumn+1) 
			NeighborHexList.Add (h);
		if ((h = GetHexagonFromArray(ArrayHexRow-1, ArrayHexColumn)) && h.HexRow == ArrayHexRow-1 && h.HexColumn == ArrayHexColumn)
			NeighborHexList.Add (h);

		return NeighborHexList;
	}

	#endregion


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
	public float DistanceBetweenHexagons(Hexagon hex1, Hexagon hex2) {
		return (Mathf.Abs(hex1.HexRow - hex2.HexRow) 
		        + Mathf.Abs (hex1.HexRow + hex1.HexColumn - hex2.HexRow - hex2.HexColumn)
		        + Mathf.Abs (hex1.HexColumn - hex2.HexColumn)) / 2; 
	}

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
