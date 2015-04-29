/////////////////////////////////////////////////////////////////////////////////
//
//	PlatformManager.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This class handles a Customization platform for a party unit
//					in the customization level/menu
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlatformManager : MonoBehaviour
{
	public bool debug = true;
	
	public Hover AbiltiyButton;
	public Hover TalentButton;

	public GameObject Stats;
	public GameObject NamePlates;

	public GameObject GUIIcon;

	//Initilization
	public PartyUnit CurrentlyDisplayedPartyUnit;
	public Transform UnitAnchorPoint;
	public Vector3 startPosition;

	//States
	public bool isActive;

	//============================================================================
	// Initilization
	//============================================================================

	public void Initialize (PartyUnit unit)
	{ 
		CreateUnit (unit);
		startPosition = transform.position;
		foreach (Hover h in GetComponentsInChildren<Hover>()) {
			h.platformManager = this;
		}
	}

	//============================================================================
	// Events
	//============================================================================

	public void SetFocused (bool b)
	{
		if (b) {
			if (debug)
				Debug.Log ("Set Focus To: " + CurrentlyDisplayedPartyUnit.Name);
			Stats.SetActive (true);

		} else {
			Stats.SetActive (false);
		}
	}
	
	protected void CreateUnit (PartyUnit unit)
	{
		if (unit != null) {
			CurrentlyDisplayedPartyUnit = unit;

			if (debug)
				Debug.Log (string.Format (":Loaded Character:\n" +
					"Name: {0} \n" +
					"Level: {1} \n", unit.name, unit.currentLevel));

			GameObject obj = Instantiate (unit.UnitPrefab) as GameObject;
			if (obj.GetComponent<MyHeroController3rdPerson> ())
				Destroy (obj.GetComponent<MyHeroController3rdPerson> ());
			obj.transform.position = UnitAnchorPoint.position;
			obj.transform.parent = UnitAnchorPoint;

		} else {
			NamePlates.SetActive (false);
		}
	}
	
	void CreateTalentTree (PartyUnit unit)
	{
		if (debug)
			Debug.Log ("Talent Tree: " + unit.UnitTalentTree.name);

		int row = 0;
		int collum = 0;
//		foreach (Talent t in unit.UnitTalentTree.Tree) {
//		}

//		for (int i; i<unit.UnitTalentTree.Tree.Count; i++) {
//			int xPos;
//			GameObject gIcon = Instantiate (GUIIcon, Vector3.one, Quaternion.identity) as GameObject;
//			if (i % 3 == 0) {
//				collum
//			}
//			row = i % 3;
//			gIcon.AddComponent<TalentIcon>.Initilize (unit.UnitTalentTree.Tree[i], );

//		}
//		for (int i; i<unit.UnitTalentTree.Tree.Count; i++) {
//			if (i % 3 == 1) {
//				row++;
//			}
//		}
	}

	void CreateAbiltiyWindow (AbilityDescription[] abilities)
	{
		if (debug)
			foreach (AbilityDescription a in abilities) {
				Debug.Log (a.DisplayName);
			}
	}
	//============================================================================
}
