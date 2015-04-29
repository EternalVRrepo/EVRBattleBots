/////////////////////////////////////////////////////////////////////////////////
//
//	AbilityActivator.cs
//	© EternalVR, All Rights Reserved
//
//	description:	Gives a unit the ability to cast AbilityDescriptions
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

		if (AbilityInProgress.AbilityTargetType != AbilityDescription.TargetType.CustomTemplate) 
			BoardManager.instance.HighlightAbility(GetComponent<BoardUnit>().CurrentlyOccupiedHexagon, AbilityInProgress);
		else if (AbilityInProgress.AbilityTargetType == AbilityDescription.TargetType.CustomTemplate)
			TemplateManager.instance.StartHighlighting(GetComponent<BoardUnit>(), AbilityInProgress);

		return ListOfAbilities[abilityNumber];
	}
	
	/// <summary>
	/// Animations and such for ability go here
	/// </summary>
	public void ChannelAbility() {
		BoardManager.instance.FinishAbility();
		StartCoroutine ("StartChanneling", TemplateManager.instance.FinishAbility ());
	}
	
	/// <summary>
	/// Starts the channeling.
	/// </summary>
	IEnumerator StartChanneling(List<BoardUnit> hits) {
		castingAbility = true;

		yield return new WaitForSeconds(1f);

		if (AbilityInProgress.AbilityTargetType == AbilityDescription.TargetType.CustomTemplate) {
			foreach (BoardUnit u in hits) {
				if (AbilityInProgress.FriendlyFireEnabled) {
					if (u is BoardUnit) {
						u.ReceiveAbilityHit (AbilityInProgress);
					}
				}
				else {
					if (u is NonPlayerControlledBoardUnit) {
						u.ReceiveAbilityHit (AbilityInProgress);
					}
				}
			}
		}
		else CastSingleTargetAbility();

		castingAbility = false;
	}
	
	/// <summary>
	/// Casts the ability.
	/// </summary>
	//	public void CastAbility() {
	//		switch(AbilityInProgress.AbilityAbilityType) {
	//		case AbilityDescription.AbilityType.Area: CastAreaAbility(); break;
	//		case AbilityDescription.AbilityType.AreaOverTime: CastAreaAbility(true); break;
	//		case AbilityDescription.AbilityType.SingleTarget: CastSingleTargetAbility(); break;
	////		case AbilityDescription.AbilityType.SingleTargetOverTime: CastSingleTargetAbility(true); break;
	//		}
	//	}
	
	/// <summary>
	/// Casts an area ability
	/// </summary>
	//	public void CastAreaAbility(bool overTime = false) {
	//
	//	}
	
	/// <summary>
	/// Casts an single target ability.
	/// </summary>
	public void CastSingleTargetAbility(bool overTime = false) {
		targetHexagon.OccupiedUnit.ReceiveAbilityHit(AbilityInProgress);
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
			//		case AbilityDescription.TargetType.TargetHexagon: {
			//			return true;
			//		}
		}
		return false; 
	}
	
	public void FinishAbility() {
		BoardManager.instance.FinishAbility();
		TemplateManager.instance.FinishAbility();
		AbilityInProgress = null;
		targetHexagon = null;
	}
	
}
