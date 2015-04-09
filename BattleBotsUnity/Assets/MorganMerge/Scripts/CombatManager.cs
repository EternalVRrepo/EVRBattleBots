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

	public GameObject PlayerObject;
	public Map map;
	public PhaseState CurrentPhase;
	public enum PhaseState {
		Waiting,
		SelectMovement,
		SelectAttack,
		TargetAttack,
		AttackQTE,
		EndOfTurn,
		NULL
	}

	protected AbilityDescription currentAbility;
	protected Camera RaycastCamera;
	protected int HexTargetMask;
	protected Hexagon CurrentlySelectedHexagon;
	[SerializeField]
	protected PlayerControlledBoardUnit CurrentlySelectedUnit;
	protected List<PlayerControlledBoardUnit> CurrentParty = new List<PlayerControlledBoardUnit>();
	private delegate void CurrentPhaseState();

	private CurrentPhaseState currentPhaseStateMethod;
	private int currentMoveDistance;

	/// <summary>
	/// 
	/// </summary>
	void Start () {
		CreateHexLayerMask();
		RaycastCamera = Camera.main;
		if (RaycastCamera == null) 
			Debug.LogError ("No Camera Found for RaycastCamera in CombatManager.cs");
	}

	/// <summary>
	/// 
	/// </summary>
	void Update () {
		if (currentPhaseStateMethod != null)
			currentPhaseStateMethod();
		else Debug.LogError("No current state for some reason");
	}

	/// <summary>
	/// Starts the turn for a unit
	/// </summary>
	void StartTurn(PlayerControlledBoardUnit nextUnit) {
		CurrentlySelectedUnit = nextUnit as PlayerControlledBoardUnit;
		CurrentlySelectedUnit.StartTurn();
		currentAbility = null;

		EnterStateMovementSelection();
	}

	/// <summary>
	/// Set the current state to movement selection
	/// </summary>
	void EnterStateMovementSelection() {
		currentPhaseStateMethod = new CurrentPhaseState(StateMovementSelection);
		CurrentPhase = PhaseState.SelectMovement;
		currentMoveDistance = CurrentlySelectedUnit.remainingMoveDistance;
		BoardManager.instance.HighlightMovement(currentMoveDistance, CurrentlySelectedUnit.CurrentlyOccupiedHexagon);
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

		BoardManager.instance.FinishAbility();
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

		BoardManager.instance.FinishAbility();

		CurrentPhase = PhaseState.AttackQTE;
	}

	/// <summary>
	/// Set current state to the end of turn state
	/// </summary>
	void EnterStateEndOfTurn() {
		currentPhaseStateMethod = new CurrentPhaseState(StateEndOfTurn);
		CurrentPhase = PhaseState.EndOfTurn;
	}

	/// <summary>
	/// State called when in movement selection
	/// </summary>
	void StateMovementSelection() {
		if (Input.GetKeyDown (KeyCode.Mouse1)) {
			Hexagon h = RaycastHexagon ();

			if (h == null)
				return;

			if (h.CurrentDistance == -1) //Every hexagon should be -1 when not able to be moved to
				return;

			CurrentlySelectedUnit.remainingMoveDistance -= h.CurrentDistance;
			CurrentlySelectedUnit.IssueMovement(h);
			BoardManager.instance.FinishMovement ();

			if (CurrentlySelectedUnit.remainingMoveDistance > 0)
				EnterStateMovementSelection();
			else EnterStateSelectAttack();
		}
		else if (Input.GetKeyDown (KeyCode.Return)) { //Skip movement phase
			EnterStateSelectAttack ();
		}
	}

	/// <summary>
	/// State called when selecting what kind of attack to use
	/// </summary>
	void StateSelectAttack() {
		if (Input.GetKeyDown (KeyCode.Alpha1)) { //Choose basic attack
			currentAbility = CurrentlySelectedUnit.AbilityActivator.ActivateAbility(0);
			BoardManager.instance.HighlightAbility(CurrentlySelectedUnit.CurrentlyOccupiedHexagon, currentAbility);
			EnterStateTargetAttack ();
		}
		else if (Input.GetKeyDown (KeyCode.Alpha2)) { //Choose ability 1
			currentAbility = CurrentlySelectedUnit.AbilityActivator.ActivateAbility (1);
			BoardManager.instance.HighlightAbility(CurrentlySelectedUnit.CurrentlyOccupiedHexagon, currentAbility);
			EnterStateTargetAttack ();
		}
		else if (Input.GetKeyDown (KeyCode.Return)) {
			EnterStateEndOfTurn();
		}
	}

	/// <summary>
	/// State called to select a target with an attack
	/// </summary>
	void StateTargetAttack() {

		if (Input.GetKeyDown (KeyCode.Mouse0)) {
			Hexagon h = RaycastHexagon ();
			if (h != null && h.InLOS()) {
				if (CurrentlySelectedUnit.AbilityActivator.GiveTarget(h)) {
					CurrentlySelectedUnit.AbilityActivator.FinishAbility ();
					BoardManager.instance.FinishAbility();
					EnterStateAttackQTE();
				}
			}
		}

		if (Input.GetKeyDown (KeyCode.Escape)) { //Cancel using this ability
			EnterStateSelectAttack ();
		}
		else if (Input.GetKeyDown (KeyCode.Return)) {
			EnterStateAttackQTE();
		}
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
			EnterStateWaiting ();
			Debug.Log ("end of turn, enemy team would take their turn now");
			//TODO Begin doing enemy team stuff here
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
	/// Setup combat initialization like party, enemies, map, etc
	/// </summary>
	public void SetupCombat(int mapIndex, List<PartyUnit> currentParty) {
		map = BoardManager.instance.Maps[mapIndex];
		EnterStateWaiting ();

		foreach (PartyUnit unit in currentParty) {
			GameObject go = Instantiate (unit.UnitPrefab) as GameObject;
			go.AddComponent <PlayerControlledBoardUnit>();
			go.AddComponent <AbilityActivator>();
			PlayerControlledBoardUnit bu = go.GetComponent<PlayerControlledBoardUnit>();
			bu.Initialize(unit);
			bu.Spawn (BoardManager.instance.GetHexagonFromArray((int)map.PlayerSpawns[currentParty.IndexOf (unit)].x, (int)map.PlayerSpawns[currentParty.IndexOf (unit)].y));
			CurrentParty.Add (bu);
		}

		StartTurn (CurrentParty[0]); //should be started from a list of characters
	}

	/// <summary>
	/// Raycasts to find/select a hexagon
	/// </summary>
	Hexagon RaycastHexagon() {
		RaycastHit hit;
		Ray ray = RaycastCamera.ScreenPointToRay(Input.mousePosition); //Create a ray from camera  mouse position
		if (Physics.Raycast (ray, out hit, 100, HexTargetMask)) //If an object is found
		{
			if (hit.collider.GetComponent<Hexagon>()) {
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
