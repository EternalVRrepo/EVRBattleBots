/////////////////////////////////////////////////////////////////////////////////
//
//	AbilityUser.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This class gives a unity the ability to use abilities
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(BoardUnit))]
public class AbilityActivator : MonoBehaviour {

	protected AbilityDescription AbilityInProgress;

	public List<AbilityDescription> ListOfAbilities = new List<AbilityDescription>();

	/// <summary>
	/// Activates an ability 
	/// </summary>
	public AbilityDescription ActivateAbility(int abilityNumber) {

		AbilityInProgress = ListOfAbilities[abilityNumber];
		return ListOfAbilities[abilityNumber];
	}

	/// <summary>
	/// Check if a target is valid for this ability
	/// </summary>
	public bool GiveTarget(Hexagon hex) {
		if (hex == null)
			return false;
		
		if (hex.CurrentHexType == Hexagon.HexType.Null)
			return false;
		
		switch(AbilityInProgress.AbilityTargetType) {
		case AbilityDescription.TargetType.TargetAlly: {
			if (hex.OccupiedUnit is PlayerControlledBoardUnit)
				return true;
			break;
		}
		case AbilityDescription.TargetType.TargetEnemy: {
			if (hex.OccupiedUnit is NonControlledBoardUnit) 
				return true;
			break;
		}
		case AbilityDescription.TargetType.TargetUnit: {
			if (hex.OccupiedUnit is NonControlledBoardUnit || hex.OccupiedUnit is PlayerControlledBoardUnit)
				return true;
			break;
		}
		case AbilityDescription.TargetType.TargetHexagon: {
			return true;
		}
		}
		return false; 
	}

	public void FinishAbility() {

		AbilityInProgress = null;
	}

}
