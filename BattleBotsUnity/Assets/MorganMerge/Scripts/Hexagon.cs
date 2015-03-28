/////////////////////////////////////////////////////////////////////////////////
//
//	Hexagon.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This class is instantiated by the BoardManager and holds all the 
//					information for each hexagon, including its position, the type
//					of hexagon, what is currently occupying it, etc
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[System.Serializable]
public class Hexagon : MonoBehaviour {
	
	public Material[] typeMaterials;				//List of materials for viewing type of hexagon
	public int HexHeight;							//The height of this hexagon
	public int ViewMode = 0;						//0 is type mode, 1 is texture mode
	public BoardUnit OccupiedUnit = null;			//What unit occupies this space
	public int HexRow;								//The row in the grid this hexagon occupies
	public int HexColumn;							//The column in the grid this hexagon occupies
	public int z {									//Z coordinate derived from x and y to get cubic coordinates
		get {
			return (-HexRow - HexColumn);
		}
	}
	public HexType CurrentHexType {					//The current type of hexagon using Hexagon.HexagonType
		get { return currentHexType; }
		set { 
			//			Debug.Log (value);
			currentHexType = value;
		}
	}
	public Material CurrentBrushTexture {			//The current material on this hexagon 
		get { return currentBrushTexture; } 
		set { currentBrushTexture = value; }
	}
	public Vector3 UnitAnchorPoint {				//Where the unit on this hexagon is placed
		get {
			return transform.position + new Vector3(0, renderer.bounds.size.y, 0);
		}
	}

	[SerializeField]
	protected HexType currentHexType;				//What type of hexagon this is using Hexagon.HexType
	[SerializeField]
	protected Material currentBrushTexture;			//Texture currently assigned to this hex
	protected bool highlighted;						//If this hexagon is currently highlighted

	public enum HexType {
		Normal = 0,				//Can be walked on normally
		Impassable = 1,			//Terrain that can't be walked on but can be seen/attacked over
		WalledImpassable = 2,	//Terrain that can't be walked over and cant be seen/attacked over
		Null = 3,				//Empty block, remove it from the map
	}

	/// <summary>
	/// 
	/// </summary>
	void Start() {
		if (gameObject.layer != 10) {
			Debug.LogError ("Hexagon prefab must be assigned Layer 10 with the name \"Hexagon\"");
		}
	}

	/// <summary>
	/// Constructor given HexagonData
	/// </summary>
	public Hexagon(HexagonData h) {
		HexRow = h.HexRow;
		HexHeight = h.HexHeight;
		CurrentHexType = h.CurrentHexType;
		CurrentBrushTexture = h.CurrentBrushTexture;
		typeMaterials = h.typeMaterials;
		HexHeight = h.HexHeight;
	}

	/// <summary>
	/// Setup a hexagon using hexagon data manually
	/// </summary>
	public void SetupHexagon(HexagonData h) {
		HexRow = h.HexRow;
		HexHeight = h.HexHeight;
		CurrentHexType = h.CurrentHexType;
		CurrentBrushTexture = h.CurrentBrushTexture;
		typeMaterials = h.typeMaterials;
		HexHeight = h.HexHeight;
		SetToType (); //TODO: needs to set to texture whenever that happens
	}

	/// <summary>
	/// Removes the unit occupying this hexagon
	/// </summary>
	public void RemoveUnit(BoardUnit u) {
		if (OccupiedUnit == u)
			OccupiedUnit = null;
		else Debug.LogError ("Trying to remove a unit from " + name + " that doesn't exist");
	}

	/// <summary>
	/// Adds a unit to this hexagon
	/// </summary>
	public void AddUnit(BoardUnit u) {
		OccupiedUnit = u;
	}

#if UNITY_EDITOR
	/// <summary>
	/// Updates the type of a hexagon
	/// </summary>
	public void SetToType() {
		if (CurrentHexType == HexType.Null) {
			renderer.enabled = true;
		}
		if (highlighted) {
			Highlight ();
			return;
		}
		if (CurrentHexType == HexType.Normal) {
			renderer.material = typeMaterials[0];
		}
		else if (CurrentHexType == HexType.Impassable) {
			renderer.material = typeMaterials[1];
		}
		else if (CurrentHexType == HexType.WalledImpassable) {
			renderer.material = typeMaterials[2];
		}
		else if (CurrentHexType == HexType.Null) {
			renderer.material = typeMaterials[3];
		}
		ViewMode = 0;
	}

	/// <summary>
	/// Updates the texture of a hexagon
	/// </summary>
	public void SetToTexture() {
		if (CurrentHexType == HexType.Null) {
			renderer.enabled = false;
			return;
		}
		if (highlighted) {
			Highlight();
			return;
		}
		if (currentHexType == HexType.Null) {
			renderer.material = typeMaterials[3];
			return;
		}
		if (CurrentBrushTexture == null) {
			renderer.material = typeMaterials[0];
			ViewMode = 1;
			return;
		}
		renderer.material = CurrentBrushTexture;
		ViewMode = 1;
	}
#endif

	/// <summary>
	/// Highlight this hexagon
	/// </summary>
	public void Highlight() {
		highlighted = true;
		renderer.material = typeMaterials[4];
	}

	/// <summary>
	/// stops highlighting this hexagon
	/// </summary>
	public void StopHighlight() {
		highlighted = false;
		if (ViewMode == 0)
			SetToType ();
		else if (ViewMode == 1)
			SetToTexture ();
	}
}
