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
using System.Collections.Generic;
using UnityEngine.UI;

public class PlatformManager : MonoBehaviour
{
	public bool debug;

	public GameObject TalentWindow;

	public GameObject Stats;
	public GameObject NamePlate;

	public GameObject GUIIcon;

	//Initilization
	public PartyUnit CurrentlyDisplayedPartyUnit;
	public TalentIcon[] talentIcons;
	public List<Talent> currentTalents = new List<Talent> ();

	public Transform UnitAnchorPoint, TalentAnchorPoint;
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
		SetWindowsActive (false);
	}
	
	protected void CreateUnit (PartyUnit unit)
	{
		if (unit != null) {
			CurrentlyDisplayedPartyUnit = unit;
			
			if (debug) {
				Debug.Log (string.Format (":Loaded Character:" +
					"Name: {0} " +
					"Level: {1} ", unit.name, unit.currentLevel));

				unit.currentLevel = 5;
			}
			GameObject obj = Instantiate (unit.UnitPrefab) as GameObject;
			if (obj.GetComponent<MyHeroController3rdPerson> ())
				Destroy (obj.GetComponent<MyHeroController3rdPerson> ());
			obj.transform.position = UnitAnchorPoint.position;
			obj.transform.parent = UnitAnchorPoint;
//			obj.transform.Rotate(new Vector3(0, 180, 0));
			obj.transform.rotation  = Quaternion.Euler(new Vector3(0, 180, 0));
			
			NamePlate.GetComponent<UnitNamePlateGUI> ().Initialize (unit);
			CreateTalentTree (unit);
			
		} else {
			NamePlate.SetActive (false);
			Stats.SetActive (false);
			TalentWindow.SetActive (false);
		}
	}

	void CreateTalentTree (PartyUnit unit)
	{
		if (debug) {
			Debug.Log ("Talent Tree: " + unit.UnitTalentTree.name);
			Debug.Log ("Talent Count: " + unit.UnitTalentTree.Tree.Count);
		}
		
		GameObject talentTreeAnchor = new GameObject ("TalentTreeAnchor");
		int row = 0;
		int collum = 0;

		//Initilize Icons
		for (int i = 0; i< unit.UnitTalentTree.Tree.Count; i++) {
			collum = i % 3 + 1;
			row = (int)(i / 3) + 1;
			GameObject talentIcon = Instantiate (GUIIcon, new Vector3 (collum, -row * 1.125f, 0), Quaternion.identity) as GameObject;
			TalentIcon talentIconCached = talentIcon.GetComponent<TalentIcon> ();
			talentIconCached.Initilize (CurrentlyDisplayedPartyUnit, unit.UnitTalentTree.Tree [i], collum, row);
			talentIcon.transform.parent = talentTreeAnchor.transform;
			talentIconCached.pm = this;
		}
		//Set Transform
		talentTreeAnchor.transform.parent = TalentAnchorPoint;
		talentTreeAnchor.transform.localPosition = Vector3.zero;
		talentTreeAnchor.transform.localRotation = Quaternion.identity;
		talentTreeAnchor.transform.localScale = Vector3.one * 0.52f;
		//Set Array
		talentIcons = GetComponentsInChildren<TalentIcon> ();
		TalentWindow.SetActive (false);
	}

	//============================================================================
	// Events
	//============================================================================
	public void SetWindowsActive (bool b)
	{
		isActive = b;
		if (b) {
			Stats.SetActive (true);
			TalentWindow.SetActive (true);
		} else {
			Stats.SetActive (false);
			TalentWindow.SetActive (false);
		}
	}

	public void UpdateTalentTree ()
	{
		currentTalents.Clear ();

		for (int i = 0; i < talentIcons.Length - 1; i++) {
			if (talentIcons [i].selected) {
				currentTalents.Add (talentIcons [i].talent);
			}
		}
		CurrentlyDisplayedPartyUnit.UnitTalentTree.UpdateTalents (currentTalents);
	}

	void CreateAbilityWindow (AbilityDescription[] abilities)
	{
		if (debug)
			foreach (AbilityDescription a in abilities) {
				Debug.Log (a.DisplayName);
			}
	}
	//============================================================================
}
