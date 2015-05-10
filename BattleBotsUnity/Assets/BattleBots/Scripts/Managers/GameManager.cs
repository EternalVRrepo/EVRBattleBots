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
		CharacterCustomization,
	}
	public GameState gameState { get{ return _GameState;} private set {_GameState = value;} }
	public GameState _GameState;
	public delegate void OnStateChangeHandler ();
	public delegate void StateInputHandler();
	public StateInputHandler InputHandler;
	public event OnStateChangeHandler OnStateChange;
	public Vector3 OpenWorldPosition;
	public GameObject OpenWorldCharacterPrefab;
	public PartyUnit StartingCharacter;

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
		gameState = GameState.MainMenu;
		InputHandler = MainMenuInput;
		PartyUnit p = (ScriptableObject.CreateInstance<PartyUnit>());
		p.Initialize(StartingCharacter);
		CurrentParty.Add(p);
		LevelTransition.OnLevelLoad += OnLevelLoad;
		
	}

	/// <summary>
	/// 
	/// </summary>
	void Update ()
	{
		if ((Input.GetKeyDown (KeyCode.Y) || Input.GetAxis ("DebugDown") > 0) && gameState != GameState.Combat) {
			StartCombat();
		} 
		
		if (InputHandler != null) 
			InputHandler();
		else Debug.LogError("InputHandler null state");
	}

	/// <summary>
	/// Handles the on state change.
	/// </summary>
	void HandleOnStateChange ()
	{
		switch (gameState) {
		case GameState.Combat:
			{
				InputHandler = CombatInput;
				break;
			}
		case GameState.IntroScreen:
			{
				InputHandler = null;
				break;
			}
		case GameState.MainMenu:
			{
				InputHandler = MainMenuInput;
				break;
			}
		case GameState.NullState:
			{
				InputHandler = null;
				break;
			}
		case GameState.OpenWorld:
			{
				InputHandler = OpenWorldInput;
				break;
			}
		case GameState.CharacterCustomization:
			{
				InputHandler = CustomizationInput;
				break;
			}
		}
	}

	/// <summary>
	/// A public method to change the current game state
	/// </summary>
	public void SetGameState (GameState newState)
	{
		this.gameState = newState;
		if (OnStateChange != null) {
			OnStateChange ();
		}
	}

	/// <summary>
	/// Raises the level was loaded event.
	/// </summary>
	void OnLevelLoad() {
		if (_instance != this) //Object that is destroyed will still call this 
			return;
		
		if (Application.loadedLevelName == "CombatTest") {
			CombatLoaded();
		} else if (Application.loadedLevelName == "CustomizationMenu") {
			CustomizationMenuLoaded ();
		} else if (Application.loadedLevelName == "OpenWorld") {
			OpenWorldLoaded ();
		} else if (Application.loadedLevelName == "MainMenu") {
			MainMenuLoaded();
		}
	}

	/// <summary>
	/// Check if character is out of boudns and position needs to be reset, not a perfect fix but good enough that it wont break the game
	/// </summary>
	IEnumerator OutOfBoundsCheck() {
		while (gameState == GameState.OpenWorld) {
			if (openWorldCharacter != null) {
				if (openWorldCharacter.transform.position.x < -55 || openWorldCharacter.transform.position.x > 55
				    || openWorldCharacter.transform.position.y < -25 || openWorldCharacter.transform.position.y > 55
				    || openWorldCharacter.transform.position.z < -55 || openWorldCharacter.transform.position.z > 55) {

					openWorldCharacter.transform.position = new Vector3(0, -1.6f, -20);
				}
			}
			yield return new WaitForSeconds(1);
		}
	}

#region StateInput
	protected void MainMenuInput() {
		if (Input.GetButtonDown("Start") || Input.GetButtonDown("Confirm")) {
			LevelTransition.LoadLevel("OpenWorld");
		}
	}
	
	protected void OpenWorldInput() {
		
		if (Input.GetButtonDown ("Start")) {
			OpenWorldPosition = openWorldCharacter.transform.position;
			LevelTransition.LoadLevel("CustomizationMenu");
		}
	}

	protected void CombatInput() {

	}

	protected void CustomizationInput() {
		if (Input.GetButtonDown ("Start")) {
			FinishCustomizationMenu();
		}
	}
#endregion

#region SceneLoaded
	/// <summary>
	/// Function for initializing the open world 
	/// </summary>
	protected void OpenWorldLoaded ()
	{
		SetGameState(GameState.OpenWorld);
		openWorldCharacter = Instantiate(OpenWorldCharacterPrefab) as GameObject;
		openWorldCharacter.transform.position = OpenWorldPosition;
		StartCoroutine("OutOfBoundsCheck");
	}

	/// <summary>
	/// Called when main menu is loaded
	/// </summary>
	protected void MainMenuLoaded() {
		SetGameState(GameState.MainMenu);
	}

	/// <summary>
	/// Called when customization menu is loaded
	/// </summary>
	protected void CustomizationMenuLoaded ()
	{
		SetGameState(GameState.CharacterCustomization);
		CharacterCustomizerManager cm = GameObject.Find ("CharacterCustomizerManager").GetComponent<CharacterCustomizerManager> ();
		cm.Initialize (CurrentParty);
	}

	/// <summary>
	/// Called when combat is loaded
	/// </summary>
	protected void CombatLoaded() {
		SetGameState(GameState.Combat);
		combatManager = GameObject.Find ("CombatManager").GetComponent<CombatManager> ();
		combatManager.debug = false;
		BoardManager.instance.InitializeMapForCombat (0);
		
		GameObject mapEditor = GameObject.Find ("MapEditor");
		if (mapEditor != null)
			Destroy (mapEditor);
		
		if (enemies == null || enemies.Count == 0) {
			DebugEnemies ();
		}
		combatManager.SetupCombat (0, CurrentParty, enemies);
	}
#endregion
	
#region ExternalStateChanges
	/// <summary>
	/// Called to switch to and start combat
	/// </summary>
	public void StartCombat (List<EnemyUnitInfo> newEnemies = null)
	{
		enemies = newEnemies;
		LevelTransition.LoadLevel("CombatTest");
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
		LevelTransition.LoadLevel("OpenWorld");
	}

	/// <summary>
	/// Finishs the customization menu and returns to the open world, called from CustomizationManager
	/// </summary>
	public void FinishCustomizationMenu ()
	{
		LevelTransition.LoadLevel("OpenWorld");
	}
#endregion

	/// <summary>
	/// Create enemies if none are given
	/// </summary>
	void DebugEnemies ()
	{
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
}
