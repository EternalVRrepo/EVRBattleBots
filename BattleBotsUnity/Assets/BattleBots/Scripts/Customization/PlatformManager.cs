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
	public Hover TalentTreeAbilities;
	public Hover TalentTreeStats;
	public Hover AbiltiyButton;
	public Hover TalentButton;

	public GameObject Stats;
	public GameObject NamePlates;
	public PartyUnit CurrentlyDisplayedPartyUnit;
	public Transform UnitAnchorPoint;
	public bool isActive;
	public Vector3 startPosition;

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
		//Load current party units model to the platform
		//Update Talent list
		//Update displayed stats
		//Update displayed abilities
	}

	//============================================================================
	// Events
	//============================================================================

	/// Sets this platform to the currently focused one, so it will display the talents/stats/etc
	public void SetFocused (bool b)
	{
		if (b) {
			Debug.Log ("Set Focus To: " + CurrentlyDisplayedPartyUnit.Name);

			TalentTreeStats.SetActive (true);
			TalentTreeAbilities.SetActive (true);
			Stats.SetActive (true);

		} else {
			TalentTreeStats.SetActive (false);
			TalentTreeAbilities.SetActive (false);
			Stats.SetActive (false);
		}
	}
	
	/// Creates a unit for this platform
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
	
	void CreateTalentTree (TalentTree tree)
	{
		if (debug)
			Debug.Log ("Talent Tree: " + tree.name);
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
