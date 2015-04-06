/////////////////////////////////////////////////////////////////////////////////
//
//	GameManager.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This class handles the game state, which includes switching levels
//					or to combat/open world mode
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

	public static GameManager instance;
	public CombatManager combatManager;
	public List<PartyUnit> CurrentParty = new List<PartyUnit>();

	/// <summary>
	/// 
	/// </summary>
	void Awake() {
		instance = this;
		DontDestroyOnLoad(this);
	}

	/// <summary>
	/// 
	/// </summary>
	void Start() {
		PartyUnit newUnit = ScriptableObject.CreateInstance<PartyUnit>();
		newUnit.UnitPrefab = Resources.Load ("PlayerUnitPrefabTest") as GameObject;
		newUnit.MovementDistance = 3;
		newUnit.ListOfAbilities.Add (Resources.Load <AbilityDescription>("Abilities/BasicAttack"));
		newUnit.ListOfAbilities.Add (Resources.Load <AbilityDescription>("Abilities/TestAbility"));
		CurrentParty.Add (newUnit);
		newUnit = ScriptableObject.CreateInstance<PartyUnit>();
		newUnit.UnitPrefab = Resources.Load ("PlayerUnitPrefabTest") as GameObject;
		newUnit.MovementDistance = 4;
		newUnit.ListOfAbilities.Add (Resources.Load <AbilityDescription>("Abilities/BasicAttack"));
		newUnit.ListOfAbilities.Add (Resources.Load <AbilityDescription>("Abilities/TestAbility"));
		CurrentParty.Add (newUnit);
	}

	/// <summary>
	/// 
	/// </summary>
	void Update() {
		if (Input.GetKeyDown (KeyCode.T)) {
			StartCombat();
		}
	}

	/// <summary>
	/// Called to switch to and start combat
	/// </summary>
	protected void StartCombat() {
		Debug.Log ("Loading Level: CombatTest"); 
		Application.LoadLevel ("CombatTest");
	}

	/// <summary>
	/// Raises the level was loaded event.
	/// </summary>
	void OnLevelWasLoaded(int level) {
		if (Application.loadedLevelName == "CombatTest") {
			combatManager = GameObject.Find ("CombatManager").GetComponent<CombatManager>();
			BoardManager.instance.InitializeMapForCombat(0);
			combatManager.SetupCombat (0, CurrentParty);
		}
	}
}
