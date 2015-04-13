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

	protected Hexagon targetHexagon;
	protected AbilityDescription AbilityInProgress;
	protected bool castingAbility;
	public bool isCasting {
		get {
			return castingAbility;
		}
	}

	public List<AbilityDescription> ListOfAbilities = new List<AbilityDescription>();

	/// <summary>
	/// Activates an ability 
	/// </summary>
	public AbilityDescription ActivateAbility(int abilityNumber) {
		AbilityInProgress = ListOfAbilities[abilityNumber];
		BoardManager.instance.HighlightAbility(GetComponent<BoardUnit>().CurrentlyOccupiedHexagon, AbilityInProgress);
		return ListOfAbilities[abilityNumber];
	}

	/// <summary>
	/// Animations and such for ability go here
	/// </summary>
	public void ChannelAbility() {
		BoardManager.instance.FinishAbility();
		StartCoroutine ("StartChanneling");
	}

	/// <summary>
	/// Starts the channeling.
	/// </summary>
	IEnumerator StartChanneling() {
		castingAbility = true;
		yield return new WaitForSeconds(1f);
		CastAbility();
		castingAbility = false;
	}

	/// <summary>
	/// Casts the ability.
	/// </summary>
	public void CastAbility() {
		switch(AbilityInProgress.AbilityAbilityType) {
		case AbilityDescription.AbilityType.Area: CastAreaAbility(); break;
		case AbilityDescription.AbilityType.AreaOverTime: CastAreaAbility(true); break;
		case AbilityDescription.AbilityType.SingleTarget: CastSingleTargetAbility(); break;
		case AbilityDescription.AbilityType.SingleTargetOverTime: CastSingleTargetAbility(true); break;
		}
	}

	/// <summary>
	/// Casts an area ability
	/// </summary>
	public void CastAreaAbility(bool overTime = false) {

	}

	/// <summary>
	/// Casts an single target ability.
	/// </summary>
	public void CastSingleTargetAbility(bool overTime = false) {
		if (AbilityInProgress.AbilityTargetType == AbilityDescription.TargetType.TargetHexagon) {
			return;
		}
		else {
			targetHexagon.OccupiedUnit.ReceiveAbilityHit(CreateAbilityHit());
		}
	}

	/// <summary>
	/// Creates an ability hit holding the information of status effects needed to be received by the target
	/// </summary>
	public AbilityHit CreateAbilityHit() {
		AbilityHit hit = new AbilityHit(AbilityInProgress);
		return hit;
	}

	/// <summary>
	/// Check if a target is valid for this ability
	/// </summary>
	public bool CheckValidTarget(Hexagon hex) {
		targetHexagon = hex;

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
			if (hex.OccupiedUnit is NonPlayerControlledBoardUnit) 
				return true;
			break;
		}
		case AbilityDescription.TargetType.TargetUnit: {
			if (hex.OccupiedUnit is NonPlayerControlledBoardUnit || hex.OccupiedUnit is PlayerControlledBoardUnit)
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
		BoardManager.instance.FinishAbility();
		AbilityInProgress = null;
		targetHexagon = null;
	}

}
