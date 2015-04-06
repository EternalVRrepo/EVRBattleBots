/////////////////////////////////////////////////////////////////////////////////
//
//	BoardUnit.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This is the base class for any unit that will be placed on 
//					the hexagon board
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BoardUnit : MonoBehaviour {

	public int MoveDistance; //How far we can move in 1 turn
	public int remainingMoveDistance; //How far left we can move this turn
	public Hexagon CurrentlyOccupiedHexagon {
		get { return currentlyOccupiedHexagon; }
		set { currentlyOccupiedHexagon = value; }
	}

	public int xPos {
		get {
			return CurrentlyOccupiedHexagon.HexRow;
		}
	}

	public int yPos {
		get {
			return CurrentlyOccupiedHexagon.HexColumn;
		}
	}
	private AbilityActivator abilityActivator;
	public AbilityActivator AbilityActivator {
		get {
			if (abilityActivator == null) 
				abilityActivator = GetComponent<AbilityActivator>();
			return abilityActivator;
		}
	}

	[SerializeField]
	private Hexagon currentlyOccupiedHexagon;

	abstract public void Spawn(Hexagon hex);
	abstract public void IssueMovement(Hexagon hex);

	/// <summary>
	/// Initialize unit for start turn, reset movement and things like that
	/// </summary>
	public void StartTurn() {
		remainingMoveDistance = MoveDistance;
	}

	/// <summary>
	/// Moves this Unit from the current hexagon to a new hexagon
	/// </summary>
	protected void MoveToHexagon(Hexagon hex) {
		if (RemoveFromHexagon ())
			AddToHexagon (hex);
	}

	/// <summary>
	/// Removes this Unit from the current hexagon 
	/// </summary>
	protected bool RemoveFromHexagon() {
		CurrentlyOccupiedHexagon.RemoveUnit(this);
		CurrentlyOccupiedHexagon = null;
		return true;
	}

	/// <summary>
	/// Places this unit on the new hexagon
	/// </summary>
	protected void AddToHexagon(Hexagon hex) {
		hex.AddUnit(this);
		CurrentlyOccupiedHexagon = hex;
		transform.position = hex.UnitAnchorPoint;
	}
}
