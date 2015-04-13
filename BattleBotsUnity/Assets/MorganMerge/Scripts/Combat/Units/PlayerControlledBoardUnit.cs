/////////////////////////////////////////////////////////////////////////////////
//
//	PlayedControlledBoardUnit.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This is a class extending BoardUnit that encompasses units that
//					are controlled by the player, so it will function and respond
//					to player input and anything else a player needs
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 


using UnityEngine;
using System.Collections;

public class PlayerControlledBoardUnit : BoardUnit {


	public override void IssueMovement(Hexagon h) {
		MoveToHexagon (h);
	}

	public override void Spawn(Hexagon h) {
		AddToHexagon (h);
	}

	/// <summary>
	/// Initialize the unit based  on a PartyUnit
	/// </summary>
	public void Initialize(PartyUnit u) {
		MoveDistance = u.MovementDistance;
		AbilityActivator.ListOfAbilities = u.ListOfAbilities;
		MaxHealth = u.Health;
		alive = true;
	}
	
}
