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

public class PlatformManager : MonoBehaviour {

	public GameObject TalentTreeTalents;
	public GameObject TalentTreeStats;
	public GameObject Stats;
	public GameObject NamePlates;
	public PartyUnit CurrentlyDisplayedPartyUnit;
	public Transform UnitAnchorPoint;
	public bool isActive;
	public Vector3 startPosition;

	public void Initialize(PartyUnit unit) { 
		CreateUnit(unit);
		startPosition = transform.position;
		foreach (Hover h in GetComponentsInChildren<Hover>()) {
			h.platformManager = this;
		}
		//Load current party units model to the platform
		//Update Talent list
		//Update displayed stats
		//Update displayed abilities
	}

	/// <summary>
	/// Creates a unit for this platform
	/// </summary>
	protected void CreateUnit(PartyUnit unit) {
		if (unit != null) {
			CurrentlyDisplayedPartyUnit = unit;
			GameObject obj = Instantiate (unit.UnitPrefab) as GameObject;
			if (obj.GetComponent<MyHeroController3rdPerson>())
				Destroy (obj.GetComponent<MyHeroController3rdPerson>());
			obj.transform.position = UnitAnchorPoint.position;
			obj.transform.parent = UnitAnchorPoint;
		}
		else {
			NamePlates.SetActive (false);
		}
	}

	/// <summary>
	/// Sets this platform to the currently focused one, so it will display the talents/stats/etc
	/// </summary>
	public void SetFocused(bool b) {
		if (b) {
			TalentTreeStats.SetActive (true);
			TalentTreeTalents.SetActive (true);
			Stats.SetActive(true);
		}
		else {
			TalentTreeStats.SetActive (false);
			TalentTreeTalents.SetActive (false);
			Stats.SetActive (false);
		}
	}
}
