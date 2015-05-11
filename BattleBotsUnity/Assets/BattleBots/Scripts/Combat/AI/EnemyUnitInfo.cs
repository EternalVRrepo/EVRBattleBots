/////////////////////////////////////////////////////////////////////////////////
//
//	EnemyUnitInfo.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This class holds the information for an enemy outside of combat,
//					once combat is started, it will load a list of these and create
//					the enemy units based on it
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class EnemyUnitInfo : ScriptableObject {

	public string Name;
	public int Health;
	public int MovementDistance;
	public List<AbilityDescription> ListOfAbilities = new List<AbilityDescription>();
	public CombatAIManager.AIType AIType;
	public GameObject UnitPrefab;
	public GameObject newModel;
	public AbilityDescription newAbility;

	public void Initialize(EnemyUnitInfo e) {
		Name = e.Name;
		Health = e.Health;
		MovementDistance = e.MovementDistance;
		AIType = e.AIType;
		UnitPrefab = e.UnitPrefab;
		newModel = e.newModel;
		newAbility = e.newAbility;

		foreach (AbilityDescription ab in e.ListOfAbilities) {
			AbilityDescription a = ScriptableObject.CreateInstance<AbilityDescription>();
			a.Initialize(ab);
			ListOfAbilities.Add(a);
		}
	}

	public EnemyUnitInfo() {}

	/// <summary>
	/// Adds a new ability to the list of abilities
	/// </summary>
	public void AddNewAbility() {
		if (newAbility == null || ListOfAbilities.Contains (newAbility)) {
			newAbility = null;
			return;
		}
		ListOfAbilities.Add (newAbility);
		newAbility = null;
	}

	public void UpdatePrefab() {
		if (newModel == null || newModel == UnitPrefab)
			return;
		UnitPrefab = newModel;
	}
}
