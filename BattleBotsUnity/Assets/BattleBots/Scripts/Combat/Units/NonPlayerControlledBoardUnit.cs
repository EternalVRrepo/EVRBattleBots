/////////////////////////////////////////////////////////////////////////////////
//
//	NonControlledBoardUnit.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This class extends BoardUnit and encompasses anything placed on
//					the board that isnt controlled by AI or the player, this means
//					things like traps, placed walls, etc
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 


using UnityEngine;
using System.Collections;

public class NonPlayerControlledBoardUnit : BoardUnit {

	public CombatAIManager.AIType AIType;

	public override void IssueMovement(Hexagon h) {
		MoveToHexagon (h);
	}

	/// <summary>
	/// Initialize the unit based  on a PartyUnit
	/// </summary>
	public void Initialize(EnemyUnitInfo u) {
		MoveDistance = u.MovementDistance;
		AbilityActivator.ListOfAbilities = u.ListOfAbilities;
		MaxHealth = u.Health;
		CurrentHealth = u.Health;
		alive = true;
		AIType = u.AIType;
	}
}
