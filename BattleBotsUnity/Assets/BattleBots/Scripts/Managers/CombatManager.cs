/////////////////////////////////////////////////////////////////////////////////
//
//	CombatManager.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This class handles player input during combat and anything else
//					combat related
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CombatManager : MonoBehaviour {

	public Map map;
	public PhaseState CurrentPhase;
	public enum PhaseState {
		Waiting,
		SelectMovement,
		SelectAttack,
		TargetAttack,
		AttackQTE,
		EndOfTurn,
		EnemyTurn,
		NULL
	}
	public List<PlayerControlledBoardUnit> CurrentParty = new List<PlayerControlledBoardUnit>();
	public List<NonPlayerControlledBoardUnit> CurrentEnemies = new List<NonPlayerControlledBoardUnit>();
	public PlayerControlledBoardUnit CurrentlySelectedUnit;
	public static CombatManager instance;

	protected bool PowerUpMenuOpen;
	protected AbilityDescription currentAbility;
	protected Camera RaycastCamera;
	protected int HexTargetMask;
	protected Hexagon CurrentlySelectedHexagon;
	[SerializeField]
	protected CombatAIManager AIManager;
	private delegate void CurrentPhaseState();
	private CurrentPhaseState currentPhaseStateMethod;
	private int currentMoveDistance;

	/// <summary>
	/// 
	/// </summary>
	void Awake() {
		if (instance == null)
			instance = this;
	}

	/// <summary>
	/// 
	/// </summary>
	void Start () {
		CreateHexLayerMask();
//		RaycastCamera = Camera.main;
//		RaycastCamera = GameObject.Find("CenterEyeAnchor").camera;

		if (RaycastCamera == null) 
			Debug.LogError ("No Camera Found for RaycastCamera in CombatManager.cs");
	}

	/// <summary>
	/// 
	/// </summary>
	void Update () {
		if (currentPhaseStateMethod != null)
			currentPhaseStateMethod();
		else Debug.LogError("No Current Combat State, most likely not initialized from GameManager");

		PowerUpInput (); //Check for input to use powerups
	}

	/// <summary>
	/// Check for input to use power ups
	/// </summary>
	void PowerUpInput ()
	{
		if (Input.GetButtonDown ("Select")) {
			CyclePowerUpMenu ();
		}
		//TODO DEBUG only
		if (Input.GetKeyDown (KeyCode.A) && PowerUpPhase() && GameManager.instance.CurrentPowerUps.Count > 0) { //Needs to be VR input and choose a power up based on it once popup menu is ready
			UsePowerUp (GameManager.instance.CurrentPowerUps [0]);
		}
	}

	/// <summary>
	/// Uses the power up.
	/// </summary>
	protected void UsePowerUp(PowerUp pu) {
		if (pu.PowerUpTargetType == PowerUp.PowerUpTarget.WholeParty) {
			foreach (PlayerControlledBoardUnit p in CurrentParty) {
				p.ApplyPowerUp(pu);
			}
		}
		else if (pu.PowerUpTargetType == PowerUp.PowerUpTarget.CurrentUnit) {
			CurrentlySelectedUnit.ApplyPowerUp(pu);
		}
		GameManager.instance.CurrentPowerUps.Remove (pu);
	}

	/// <summary>
	/// Check if its a phase we can use power ups in
	/// </summary>
	protected bool PowerUpPhase() {
		if (CurrentPhase == PhaseState.SelectMovement)
			return true;

		if (CurrentPhase == PhaseState.SelectAttack)
			return true;

		if (CurrentPhase == PhaseState.TargetAttack) 
			return true;

		return false;
	}

	/// <summary>
	/// Cycles the power up menu between on and off
	/// </summary>
	protected void CyclePowerUpMenu() {
		if (PowerUpMenuOpen) {
			//PowerUpMenu.setActive(false);
		}
		else {
			//PowerUpMenu.setActive(true)
		}
	}

	/// <summary>
	/// Starts the turn for a unit
	/// </summary>
	void StartTurn(PlayerControlledBoardUnit nextUnit) {
		CurrentlySelectedUnit = nextUnit as PlayerControlledBoardUnit;
		BoardManager.instance.StartTurn();
		CurrentlySelectedUnit.StartTurn();
		currentAbility = null;

		if (!CurrentlySelectedUnit.CanTakeTurn()) 
			EnterStateEndOfTurn();
		else EnterStateMovementSelection();
	}

	/// <summary>
	/// Set the current state to movement selection
	/// </summary>
	void EnterStateMovementSelection() {
		currentPhaseStateMethod = new CurrentPhaseState(StateMovementSelection);
		CurrentPhase = PhaseState.SelectMovement;
		currentMoveDistance = CurrentlySelectedUnit.remainingMoveDistance;
		if (CurrentlySelectedUnit.CanMove())
			BoardManager.instance.HighlightMovement(currentMoveDistance, CurrentlySelectedUnit.CurrentlyOccupiedHexagon);
		CurrentlySelectedUnit.MovementIsDirty = false;
	}

	/// <summary>
	/// Set the current state to Waiting
	/// </summary>
	void EnterStateWaiting() {
		currentPhaseStateMethod = new CurrentPhaseState(StateWait);
		CurrentPhase = PhaseState.Waiting;
	}

	/// <summary>
	/// Set the current state to select an attack
	/// </summary>
	void EnterStateSelectAttack() {
		currentPhaseStateMethod = new CurrentPhaseState(StateSelectAttack);

		CurrentlySelectedUnit.AbilityActivator.FinishAbility ();
		BoardManager.instance.FinishMovement();

		CurrentPhase = PhaseState.SelectAttack;
	}

	/// <summary>
	/// Set the current state to select an attack
	/// </summary>
	void EnterStateTargetAttack() {
		currentPhaseStateMethod = new CurrentPhaseState(StateTargetAttack);
		CurrentPhase = PhaseState.TargetAttack;
	}

	/// <summary>
	/// Set current state to performing a QTE
	/// </summary>
	void EnterStateAttackQTE() {
		currentPhaseStateMethod = new CurrentPhaseState(StateAttackQTE);

		CurrentlySelectedUnit.AbilityActivator.FinishAbility ();

		CurrentPhase = PhaseState.AttackQTE;
	}

	/// <summary>
	/// Set current state to the end of turn state
	/// </summary>
	void EnterStateEndOfTurn() {
		currentPhaseStateMethod = new CurrentPhaseState(StateEndOfTurn);
		CurrentlySelectedUnit.EndTurn();
		BoardManager.instance.EndTurn ();
		CurrentPhase = PhaseState.EndOfTurn;
	}

	/// <summary>
	/// Enter state while waiting for enemy team to go
	/// </summary>
	void EnterStateEnemyTurn() {
		currentPhaseStateMethod = new CurrentPhaseState(StateEnemyTurn);

		AIManager.StartEnemyTurn ();

		CurrentPhase = PhaseState.EnemyTurn;
	}

	public void KillUnit(BoardUnit unit) {
		if (unit is PlayerControlledBoardUnit) {
			CurrentParty.Remove (unit as PlayerControlledBoardUnit);
			if (CurrentParty.Count == 0) {
				GameManager.instance.FinishCombat(false);
			}
		}
		else if (unit is NonPlayerControlledBoardUnit) {
			CurrentEnemies.Remove (unit as NonPlayerControlledBoardUnit);
			if (CurrentEnemies.Count == 0) { //End of combat
				GameManager.instance.FinishCombat(true);
			}
		}
	}

	/// <summary>
	/// State called when in movement selection
	/// </summary>
	void StateMovementSelection() {

		if (CurrentlySelectedUnit.MovementIsDirty)
			EnterStateMovementSelection();

		if (Input.GetButtonDown("Ability1") || Input.GetKeyDown (KeyCode.Mouse0)) {
			Hexagon h = RaycastHexagon ();

			if (h == null)
				return;

			if (!BoardManager.instance.CanMove(h)) //Every hexagon should be -1 when not able to be moved to
				return;

			EnterStateWaiting ();
			CurrentlySelectedUnit.remainingMoveDistance -= h.CurrentDistance;
			BoardManager.instance.FinishMovement ();
			StartCoroutine("MoveUnit", h);
		}
		else if (Input.GetButtonDown ("Cancel") || Input.GetKeyDown (KeyCode.Escape)) { //Skip movement phase
			EnterStateSelectAttack ();
		}
	}

	/// <summary>
	/// Moves the unit after finding a path
	/// </summary>
	IEnumerator MoveUnit(Hexagon h) {
		List<Hexagon> path = BoardManager.instance.GetPath(CurrentlySelectedUnit.CurrentlyOccupiedHexagon, h);

		int i = 0;
		Hexagon curr = null;
		while (curr != h) {
			curr = path[i];
			CurrentlySelectedUnit.IssueMovement (curr);
			yield return new WaitForSeconds(0.74f);
			i++;
		}

		if (CurrentlySelectedUnit.remainingMoveDistance > 0)
			EnterStateMovementSelection();
		else EnterStateSelectAttack();

		yield return null;
	}

	/// <summary>
	/// State called when selecting what kind of attack to use
	/// </summary>
	void StateSelectAttack() {

		if (Input.GetButtonDown ("Cancel") || Input.GetKeyDown (KeyCode.Escape)) {
			EnterStateEndOfTurn();
		}
		if (!CurrentlySelectedUnit.CanCastAbility ()){ //if we are silenced or something, we cant select an ability
			return;
		}

		if (Input.GetButtonDown ("Ability1")) { //Choose ability 1
			if ((currentAbility = CurrentlySelectedUnit.AbilityActivator.ActivateAbility(0)) != null)
				EnterStateTargetAttack ();
		}
		else if (Input.GetButtonDown ("Ability2")) { //Choose ability 2
			if ((currentAbility = CurrentlySelectedUnit.AbilityActivator.ActivateAbility(1)) != null)
				EnterStateTargetAttack ();
		}
		else if (Input.GetButtonDown ("Ability3")) { //Choose ability 3
			if ((currentAbility = CurrentlySelectedUnit.AbilityActivator.ActivateAbility(2)) != null)
				EnterStateTargetAttack ();
		}
		else if (Input.GetButtonDown ("Ability4")) { //Choose ability 4
			if ((currentAbility = CurrentlySelectedUnit.AbilityActivator.ActivateAbility(3)) != null)
				EnterStateTargetAttack ();
		}
	}

	/// <summary>
	/// State called to select a target with an attack
	/// </summary>
	void StateTargetAttack() {
		if (Input.GetButtonDown ("Ability1") || Input.GetKeyDown (KeyCode.Mouse0)) {
			if (!TemplateManager.instance.TemplateInUse) { 
				Hexagon h = RaycastHexagon ();
				if (h != null && h.InLOS()) {
					if (CurrentlySelectedUnit.AbilityActivator.CheckValidTarget(h)) {
						EnterStateWaiting ();
						StartCoroutine ("UseAbility");
					}
				}
			}
			else {
				if (currentAbility.RequireSourceHexagon) {
					Hexagon h = RaycastHexagon ();
					if (h != null) {
						currentAbility.SourceHexagon = h;
						EnterStateWaiting();
						StartCoroutine ("UseAbility");
					}
				}
				else {
					EnterStateWaiting();
					StartCoroutine ("UseAbility");
				}
			}
		}

		if (Input.GetButtonDown ("Cancel")) { //Cancel using this ability
			EnterStateSelectAttack ();
		}
	}

	/// <summary>
	/// Uses an ability.
	/// </summary>
	IEnumerator UseAbility() {
		CurrentlySelectedUnit.AbilityActivator.ChannelAbility();
		while (CurrentlySelectedUnit.AbilityActivator.isCasting) {
			yield return null;
		}
		EnterStateAttackQTE();
	}

	/// <summary>
	/// State where you perform a QTE
	/// </summary>
	void StateAttackQTE() {
		EnterStateEndOfTurn();
	}

	/// <summary>
	/// State where the logic for the next turn is decided
	/// </summary>
	void StateEndOfTurn() {
		int i = CurrentParty.IndexOf (CurrentlySelectedUnit);
		i++;
		if (CurrentParty.Count == i) {
			EnterStateEnemyTurn ();
		}
		else StartTurn (CurrentParty[i]);
	}

	/// <summary>
	/// State to wait, no input should be received
	/// </summary>
	void StateWait() {
//		Debug.Log ("waiting");
	}

	/// <summary>
	/// State while enemy team takes their turn, probably no input here either 
	/// </summary>
	void StateEnemyTurn() {
		if (!AIManager.TurnInProgress)
			StartTurn (CurrentParty[0]);
	}

	/// <summary>
	/// Setup combat initialization like party, enemies, map, etc
	/// </summary>
	public void SetupCombat(int mapIndex, List<PartyUnit> currentParty, List<EnemyUnitInfo> listEnemies) {
		map = BoardManager.instance.Maps[mapIndex];
		AIManager = gameObject.AddComponent<CombatAIManager>();
		AIManager.combatManager = this;
		EnterStateWaiting ();

		if (listEnemies.Count == 0)
			Debug.LogError ("No enemies found for combat, lots of errors incoming");

		foreach (PartyUnit unit in currentParty) {
			GameObject go = Instantiate (unit.UnitPrefab) as GameObject;
			if (go.GetComponent<MyHeroController3rdPerson>())
				Destroy (go.GetComponent<MyHeroController3rdPerson>());
			go.AddComponent <PlayerControlledBoardUnit>();
			go.AddComponent <AbilityActivator>();
			PlayerControlledBoardUnit bu = go.GetComponent<PlayerControlledBoardUnit>();
			bu.Initialize(unit);
			go.name = bu.UnitClass.ToString ();
			bu.Spawn (BoardManager.instance.GetHexagonFromArray((int)map.PlayerSpawns[currentParty.IndexOf (unit)].x, (int)map.PlayerSpawns[currentParty.IndexOf (unit)].y));
			CurrentParty.Add (bu);
		}

		foreach (EnemyUnitInfo unit in listEnemies) {
			GameObject go = Instantiate (unit.UnitPrefab) as GameObject;
			go.AddComponent<NonPlayerControlledBoardUnit>();
			go.AddComponent<AbilityActivator>();
			NonPlayerControlledBoardUnit bu = go.GetComponent<NonPlayerControlledBoardUnit>();
			bu.Initialize(unit);
			bu.Spawn(BoardManager.instance.GetHexagonFromArray((int)map.EnemySpawns[listEnemies.IndexOf(unit)].x, (int)map.EnemySpawns[listEnemies.IndexOf(unit)].y));
			CurrentEnemies.Add (bu);
		}

		StartTurn (CurrentParty[0]); //should be started from a list of characters
	}

	/// <summary>
	/// Raycasts to find/select a hexagon
	/// </summary>
	public Hexagon RaycastHexagon() {
		RaycastHit hit;
		Ray ray = RaycastCamera.ScreenPointToRay(Input.mousePosition); //Create a ray from camera  mouse position
		if (Physics.Raycast (ray, out hit, 100, HexTargetMask)) //If an object is found
		{
			if (hit.collider.GetComponent<Hexagon>()) {
				Debug.Log (hit.transform.name);
				return hit.collider.GetComponent<Hexagon>(); //Return the game object as a GameObject
			}
			else return null;
		}
		else {
			return null;
		}
	}

	/// <summary>
	/// Creates the hexagon targetting layer mask, if something should be tested for collision with the raycast
	/// it should be added here (I can't forsee needing anything else, but its here)
	/// </summary>
	public void CreateHexLayerMask() {
		if (LayerMask.LayerToName(10) != "Hexagon") {
			Debug.LogError ("Layer 10 Needs to be named \"Hexagon\" and should be assigned to HexPrefab");
		}
		if (LayerMask.LayerToName(11) != "HexagonLOS") {
			Debug.LogError("Layer 11 Needs to be named \"HexagonLOS\" and should be assigned to the HexPrefab child LOSCollider");
		}
		int Layer1 = 10; //Hexagon
		int LayerMask1 = 1 << Layer1;
		HexTargetMask = LayerMask1; //... | LayerMask2 | LayerMask3;
	}
}
