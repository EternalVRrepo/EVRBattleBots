/////////////////////////////////////////////////////////////////////////////////
//
//	BoardManager.cs
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

	public int HexRow;
	public int HexColumn;
	public int z {
		get {
			return (-HexRow - HexColumn);
		}
	}

	//Possible types of hexagons
	public enum HexType {
		Normal = 0,				//Can be walked on normally
		Impassable = 1,			//Terrain that can't be walked on but can be seen/attacked over
		WalledImpassable = 2,	//Terrain that can't be walked over and cant be seen/attacked over
		Null = 3,				//Empty block, remove it from the map
	}
	[SerializeField]
	protected HexType currentHexType = HexType.Normal;
	public HexType CurrentHexType {
		get { return currentHexType; }
		set { currentHexType = value; }
	}

	[SerializeField]
	protected Material currentBrushTexture; //Texture currently assigned to this hex
	public Material CurrentBrushTexture {
		get { return currentBrushTexture; } 
		set { currentBrushTexture = value; }
	}

	public Material[] typeMaterials; //List of materials

	//The height of this hexagon
	public int HexHeight;

	public int ViewMode = 0; //0 is type mode, 1 is texture mode

	public void SetToType() {
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

	public void SetToTexture() {
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

	protected bool highlighted;
	public void Highlight() {
		highlighted = true;
		renderer.material = typeMaterials[4];
	}

	public void StopHighlight() {
		highlighted = false;
		if (ViewMode == 0)
			SetToType ();
		else if (ViewMode == 1)
			SetToTexture ();
	}
}
