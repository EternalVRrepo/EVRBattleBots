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
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{

	protected GameManager ()
	{
	}
	public static GameManager instance {
		get {
			if (GameManager._instance == null) {
				_instance = GameObject.FindObjectOfType<GameManager> ();
				DontDestroyOnLoad (_instance);
			}
			return _instance;
		}
	}
	public CombatManager combatManager;
	public List<PartyUnit> CurrentParty = new List<PartyUnit> ();
	public List<PowerUp> CurrentPowerUps = new List<PowerUp> ();
	public enum GameState
	{
		NullState,
		IntroScreen,
		MainMenu,
		OpenWorld,
		Combat,
		CharacterCustomization
	}
	public GameState gameState { get; private set; }
	public delegate void OnStateChangeHandler ();
	public event OnStateChangeHandler OnStateChange;
	public Vector3 OpenWorldPosition;
	public GameObject OpenWorldCharacterPrefab;
	
	protected List<EnemyUnitInfo> enemies = new List<EnemyUnitInfo> ();
	protected GameObject openWorldCharacter;

	private static GameManager _instance = null;

	/// <summary>
	/// 
	/// </summary>
	void Awake ()
	{
		if (_instance == null) {
			_instance = this;
			DontDestroyOnLoad (this);
		} else {
			if (this != _instance)
				Destroy (this.gameObject);
		}
		OnStateChange += HandleOnStateChange;
		SetGameState (GameState.MainMenu);
	}

	/// <summary>
	/// Handles the on state change.
	/// </summary>
	void HandleOnStateChange ()
	{
		switch (gameState) {
		case GameState.Combat:
			{
				StartCombat ();
				break;
			}
		case GameState.IntroScreen:
			{
				break;
			}
		case GameState.MainMenu:
			{
				break;
			}
		case GameState.NullState:
			{
				break;
			}
		case GameState.OpenWorld:
			{
				StartOpenWorld ();
				break;
			}
		case GameState.CharacterCustomization:
			{
				StartCustomization ();
				break;
			}
		}
	}

	/// <summary>
	/// A public method to change the current game state
	/// </summary>
	public void SetGameState (GameState newState)
	{
		if (gameState == GameState.OpenWorld) {
			OpenWorldPosition = openWorldCharacter.transform.position;
		}
		this.gameState = newState;
		if (OnStateChange != null) {
			OnStateChange ();
		}
	}

	protected void StartCustomization ()
	{
		Application.LoadLevel ("CustomizationMenu");
	}

	protected void CustomizationMenuLoaded ()
	{
		CharacterCustomizerManager cm = GameObject.Find ("CharacterCustomizerManager").GetComponent<CharacterCustomizerManager> ();
		cm.Initialize (CurrentParty);
	}

	/// <summary>
	/// 
	/// </summary>
	void Start ()
	{
		gameState = GameState.OpenWorld;
		openWorldCharacter = Instantiate (OpenWorldCharacterPrefab, new Vector3(0, 0, -35), Quaternion.identity) as GameObject;
		
		//TODO: debug stuff for combat so we have a party, shouldnt be here
		PartyUnit newUnit = ScriptableObject.CreateInstance<PartyUnit> ();
		newUnit.UnitPrefab = Resources.Load ("Characters/Hero") as GameObject;
		newUnit.MovementDistance = 4;
		newUnit.Health = 150;
		newUnit.UnitClass = PlayerControlledBoardUnit.PlayerClass.Support;
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription> ("Abilities/Support/ElectromagneticField")) as AbilityDescription);
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription> ("Abilities/Support/ConcussiveBlast")) as AbilityDescription);
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription> ("Abilities/Support/StaticShell")) as AbilityDescription);
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription> ("Abilities/Support/PulseForce")) as AbilityDescription);
		newUnit.currentLevel = 1;
		newUnit.UnitTalentTree = Instantiate (Resources.Load<TalentTree> ("TalentTrees/WarriorTree")) as TalentTree;
		CurrentParty.Add (newUnit);
		newUnit = ScriptableObject.CreateInstance<PartyUnit> ();
		newUnit.UnitPrefab = Resources.Load ("Characters/Hero") as GameObject;
		newUnit.MovementDistance = 4;
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription> ("Abilities/Wizard/RadiantEnergy")) as AbilityDescription);
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription> ("Abilities/Wizard/CircuitBreak")) as AbilityDescription);
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription> ("Abilities/Wizard/StaticGrip")) as AbilityDescription);
		newUnit.ListOfAbilities.Add (Instantiate (Resources.Load<AbilityDescription> ("Abilities/Wizard/FluxBlast")) as AbilityDescription);
		newUnit.Health = 120;
		newUnit.UnitClass = PlayerControlledBoardUnit.PlayerClass.Wizard;
		newUnit.currentLevel = 1;
		newUnit.UnitTalentTree = Instantiate (Resources.Load<TalentTree> ("TalentTrees/WizardTree")) as TalentTree;
		CurrentParty.Add (newUnit);

		CurrentPowerUps.Add (Instantiate (Resources.Load<PowerUp> ("PowerUps/MoveSpeed")) as PowerUp);
	}

	/// <summary>
	/// 
	/// </summary>
	void Update ()
	{
		if ((Input.GetKeyDown (KeyCode.Y) || Input.GetAxis ("DebugDown") > 0) && gameState != GameState.Combat) {
			SetGameState (GameState.Combat);
		} else if (Input.GetButtonDown ("Start")) {
			if (gameState != GameState.CharacterCustomization)	
				SetGameState (GameState.CharacterCustomization);
			else if (gameState == GameState.CharacterCustomization)
				FinishCustomizationMenu ();
		}
	}

	protected void StartOpenWorld ()
	{
		Application.LoadLevel ("OpenWorld");
	}

	/// <summary>
	/// Called to switch to and start combat
	/// </summary>
	public void StartCombat (List<EnemyUnitInfo> newEnemies = null)
	{
		enemies = newEnemies;
		Application.LoadLevel ("CombatTest");
	}

	/// <summary>
	/// Finish combat and return to the open world, called from combat manager when its finished
	/// </summary>
	public void FinishCombat (bool win)
	{
		if (win) { //victory
			foreach (PartyUnit u in CurrentParty) {
				u.AddXP (10);
			}
		} else { //defeat

		}
		SetGameState (GameState.OpenWorld);
	}

	/// <summary>
	/// Finishs the customization menu and returns to the open world, called from CustomizationManager
	/// </summary>
	public void FinishCustomizationMenu ()
	{
		SetGameState (GameState.OpenWorld);
	}

	/// <summary>
	/// Function for initializing the open world 
	/// </summary>
	protected void OpenWorldLoaded ()
	{
		openWorldCharacter = Instantiate(OpenWorldCharacterPrefab) as GameObject;
		openWorldCharacter.transform.position = OpenWorldPosition;
	}

	/// <summary>
	/// Raises the level was loaded event.
	/// </summary>
	void OnLevelWasLoaded (int level)
	{
		if (_instance != this) //Object that is destroyed will still call this 
			return;

		if (Application.loadedLevelName == "CombatTest") {
			combatManager = GameObject.Find ("CombatManager").GetComponent<CombatManager> ();
			combatManager.debug = false;
			BoardManager.instance.InitializeMapForCombat (0);

			GameObject mapEditor = GameObject.Find ("MapEditor");
			if (mapEditor != null)
				Destroy (mapEditor);

			if (enemies == null || enemies.Count == 0) {
				enemies = new List<EnemyUnitInfo> ();
				EnemyUnitInfo newUnit = ScriptableObject.CreateInstance<EnemyUnitInfo> ();
				newUnit.UnitPrefab = Resources.Load ("EnemyUnitPrefabTest") as GameObject;
				newUnit.MovementDistance = 3;
				newUnit.Health = 400;
				newUnit.AIType = CombatAIManager.AIType.Melee;
				newUnit.ListOfAbilities.Add (Resources.Load<AbilityDescription> ("Abilities/EnemyAbilities/Bite"));
				enemies.Add (newUnit);
				newUnit = ScriptableObject.CreateInstance<EnemyUnitInfo> ();
				newUnit.UnitPrefab = Resources.Load ("EnemyUnitPrefabTest") as GameObject;
				newUnit.MovementDistance = 3;
				newUnit.Health = 400;
				newUnit.AIType = CombatAIManager.AIType.Melee;
				newUnit.ListOfAbilities.Add (Resources.Load<AbilityDescription> ("Abilities/EnemyAbilities/Bite"));
				enemies.Add (newUnit);
			}
			combatManager.SetupCombat (0, CurrentParty, enemies);
		} else if (Application.loadedLevelName == "CustomizationMenu") {
			CustomizationMenuLoaded ();
		} else if (Application.loadedLevelName == "OpenWorld") {
			OpenWorldLoaded ();
		}
	}
}
