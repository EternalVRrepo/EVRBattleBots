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

	protected GameManager() {}
	public static GameManager instance {
		get {
			if (GameManager._instance == null)
				Debug.LogError ("GameManager instance is null");
			return GameManager._instance;
		}
	}
	public CombatManager combatManager;
	public List<PartyUnit> CurrentParty = new List<PartyUnit>();
	public enum GameState {
		NullState,
		IntroScreen,
		MainMenu,
		OpenWorld,
		Combat
	}
	public GameState gameState { get; private set; }
	public delegate void OnStateChangeHandler();
	public event OnStateChangeHandler OnStateChange;

	private static GameManager _instance = null;

	/// <summary>
	/// 
	/// </summary>
	void Awake() {
		DontDestroyOnLoad(this);
		_instance = this;
		OnStateChange += HandleOnStateChange;
		SetGameState (GameState.MainMenu);
	}

	/// <summary>
	/// Handles the on state change.
	/// </summary>
	void HandleOnStateChange () {
		switch (gameState) {
		case GameState.Combat: {
			StartCombat ();
			break;
		}
		case GameState.IntroScreen: {
			break;
		}
		case GameState.MainMenu: {
			break;
		}
		case GameState.NullState: {
			break;
		}
		case GameState.OpenWorld: {
			break;
		}
		}
	}

	/// <summary>
	/// A public method to change the current game state
	/// </summary>
	public void SetGameState(GameState newState) {
		this.gameState = newState;
		if (OnStateChange != null) {
			OnStateChange();
		}
	}

	/// <summary>
	/// 
	/// </summary>
	void Start() {
		//TODO: debug stuff for combat so we have a party, shouldnt be here
		PartyUnit newUnit = ScriptableObject.CreateInstance<PartyUnit>();
		newUnit.UnitPrefab = Resources.Load ("PlayerUnitPrefabTest") as GameObject;
		newUnit.MovementDistance = 3;
		newUnit.Health = 20;
		newUnit.ListOfAbilities.Add (Resources.Load <AbilityDescription>("Abilities/TestDamage"));
		newUnit.ListOfAbilities.Add (Resources.Load <AbilityDescription>("Abilities/TestHeal and Slow"));
		newUnit.ListOfAbilities.Add (Resources.Load <AbilityDescription>("Abilities/TestStun"));
		CurrentParty.Add (newUnit);
		newUnit = ScriptableObject.CreateInstance<PartyUnit>();
		newUnit.UnitPrefab = Resources.Load ("PlayerUnitPrefabTest") as GameObject;
		newUnit.MovementDistance = 4;
		newUnit.ListOfAbilities.Add (Resources.Load <AbilityDescription>("Abilities/TestDamage"));
		newUnit.ListOfAbilities.Add (Resources.Load <AbilityDescription>("Abilities/TestHeal and Slow"));
		newUnit.ListOfAbilities.Add (Resources.Load <AbilityDescription>("Abilities/TestStun"));
		newUnit.Health = 25;
		CurrentParty.Add (newUnit);
	}

	/// <summary>
	/// 
	/// </summary>
	void Update() {
		if (Input.GetButtonDown ("Start")) {
			SetGameState(GameState.Combat);
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
