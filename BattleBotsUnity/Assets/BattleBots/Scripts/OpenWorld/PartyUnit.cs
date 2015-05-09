/////////////////////////////////////////////////////////////////////////////////
//
//	PartyUnit.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This class embodies a party member the player is using, the
//					player will have 1-4 of these at any point, each one holds
//					the stats, ability selection, equipped gear, talents, etc of
//					that unit.
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[SerializeField]
public class PartyUnit : ScriptableObject {

	public static int[] XPToLevel = { 5, 10, 15, 20, 25, 30, 35, 40, 45 };

	public string Name;
	public PlayerControlledBoardUnit.PlayerClass UnitClass;
	public int Health;
	public GameObject UnitPrefab;
	public GameObject newModel;
	public int MovementDistance;
	public List<AbilityDescription> ListOfAbilities = new List<AbilityDescription>();
	public AbilityDescription newAbility;
	public int currentXP;
	public int currentLevel;
	public TalentTree UnitTalentTree;

	public void AddXP(int amount) {
		currentXP += amount;

		while (currentXP >= XPToLevel[currentLevel-1]) {
			currentLevel++;
		}
	}

	public void UpdatePrefab() {
		if (newModel == null || newModel == UnitPrefab)
			return;
		UnitPrefab = newModel;
	}

	public void AddNewAbility() {
		if (newAbility == null || ListOfAbilities.Contains (newAbility)) {
			newAbility = null;
			return;
		}
		ListOfAbilities.Add (newAbility);
		newAbility = null;
	}
}
