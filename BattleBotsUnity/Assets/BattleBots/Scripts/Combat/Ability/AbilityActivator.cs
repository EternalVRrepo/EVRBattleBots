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
	public AbilityDescription AbilityInProgress;
	protected bool castingAbility;
	public bool isCasting {
		get {
			return castingAbility;
		}
	}
	
	public List<AbilityDescription> ListOfAbilities = new List<AbilityDescription>();

//	public List<BoardUnit> UnitsToHit = new List<BoardUnit>(); //Units to be hit once the QTE finishes
	public List<AbilityModifier> mods = new List<AbilityModifier>(); //Mods to hit with

	/// <summary>
	/// Activates an ability 
	/// </summary>
	public AbilityDescription ActivateAbility(int abilityNumber) {

		if (ListOfAbilities[abilityNumber].currentCooldown > 0)
			return null;

		AbilityInProgress = ListOfAbilities[abilityNumber];
//		UnitsToHit.Clear (); //Reset units we are gonna hit
		mods.Clear ();

		if (AbilityInProgress.AbilityTargetType != AbilityDescription.TargetType.CustomTemplate) {
			BoardManager.instance.HighlightAbility(GetComponent<BoardUnit>().CurrentlyOccupiedHexagon, AbilityInProgress, true);
			TemplateManager.instance.StartHexHighlighting(GetComponent<BoardUnit>(), AbilityInProgress);
		}
		else {
			TemplateManager.instance.StartHighlighting(GetComponent<BoardUnit>(), AbilityInProgress);
		}
		

		return ListOfAbilities[abilityNumber];
	}
	
	/// <summary>
	/// Animations and such for ability go here
	/// </summary>
	public void ChannelAbility() {
		AbilityInProgress.currentCooldown = AbilityInProgress.Cooldown;
		BoardManager.instance.FinishAbility();
		StartCoroutine ("CastAbility", TemplateManager.instance.FinishAbility ());
	}
	
	/// <summary>
	/// Starts the channeling.
	/// </summary>
	IEnumerator CastAbility(List<Hexagon> hits) {
		castingAbility = true;
		List<BoardUnit> unitsHit = new List<BoardUnit>();

		QTEManager.instance.StartQTE();
		while (QTEManager.instance.inProgress) {
			yield return null;
		}

		EffectManager.instance.CreateEffects(AbilityInProgress, targetHexagon, hits);

//		List<AbilityModifier> mods = new List<AbilityModifier>();
		if (GetComponent<BoardUnit>().isEnfeebled) {
			mods.Add (new AbilityModifier(AbilityModifier.Modifier.Damage, .5f));
		}
		
		if (AbilityInProgress.AbilityTargetType == AbilityDescription.TargetType.CustomTemplate) {
			foreach (Hexagon h in hits) {
				if (AbilityInProgress.FriendlyFireEnabled) {
					if (h.OccupiedUnit is BoardUnit) {
						if (h.OccupiedUnit.AbilityActivator == this && !AbilityInProgress.SelfFireEnabled)
							goto Skip;

						BoardUnit u = h.OccupiedUnit;

						if (unitsHit.Contains (u))
							break;
						else unitsHit.Add (u);

						if (AbilityInProgress.DisplayName == "FluxBlast") {
							u.KnockBack(AbilityInProgress.SourceHexagon, 2);
						}
						else if (AbilityInProgress.DisplayName == "StaticBomb") {
							u.PullIn(AbilityInProgress.SourceHexagon, 2);
						}
						else if (AbilityInProgress.DisplayName == "PulseForce") {
							u.KnockBack(GetComponent<BoardUnit>().CurrentlyOccupiedHexagon, 3);
						}
						u.ReceiveAbilityHit (AbilityInProgress, mods);
					Skip: {}
					}
				}
				else {
					if (h.OccupiedUnit is NonPlayerControlledBoardUnit) {
						h.OccupiedUnit.ReceiveAbilityHit (AbilityInProgress, mods);
					}
				}
			}
			castingAbility = false;
		}
		else if (AbilityInProgress.AbilityTargetType == AbilityDescription.TargetType.TargetHexagon) {
			if (AbilityInProgress.DisplayName == "ElectromagneticField") {
				if (targetHexagon.OccupiedUnit != null) {
					targetHexagon.OccupiedUnit.KnockBack (GetComponent<BoardUnit>().CurrentlyOccupiedHexagon, 1);
				}
			}
//			targetHexagon.ReceiveAbilityHit(AbilityInProgress, mods);

			castingAbility = false;
		}
		else StartCoroutine ("CastSingleTargetAbility");
	}

	public void EndTurn() {
		foreach (AbilityDescription a in ListOfAbilities) {
			if (a.currentCooldown > 0)
				a.currentCooldown--;
		}
	}
	
	/// <summary>
	/// Casts an single target ability.
	/// </summary>
	public bool waiting;
	public List<Hexagon> targets;
	public IEnumerator CastSingleTargetAbility() {
		waiting = true;
		targets = null;
//		List<AbilityModifier> mods = new List<AbilityModifier>();
//		if (GetComponent<BoardUnit>().isEnfeebled) {
//			mods.Add (new AbilityModifier(AbilityModifier.Modifier.Damage, .5f));
//		}
		switch(AbilityInProgress.DisplayName) {
			case "SonicStrike": {
				targetHexagon.OccupiedUnit.ReceiveAbilityHit(AbilityInProgress, mods);
				mods.Add(new AbilityModifier(AbilityModifier.Modifier.Damage, .5f));
				TemplateManager.instance.TemplateHit(this, TemplateManager.TargetTemplate.Cone, 3, GetComponent<BoardUnit>().CurrentlyOccupiedHexagon, targetHexagon);
				while (waiting)
					yield return null;
				for (int i = 0; i < targets.Count; i++) {
					Hexagon h = targets [i];
					if (HexagonHittable (h))
						h.OccupiedUnit.ReceiveAbilityHit (AbilityInProgress, mods);
				}
				break;
			}
			case "StaticRush": {
				BoardUnit u = targetHexagon.OccupiedUnit;
				bool collision = false;
				List<Hexagon> path = BoardManager.instance.CanPushCharacter(GetComponent<BoardUnit>().CurrentlyOccupiedHexagon, targetHexagon, out collision);

				int i = 1;
				Hexagon curr = null;
				while (i < path.Count-1) { //Count-1 because the path leads ontop of another unit, so stop 1 short
					curr = path[i];
					if (!collision && i == path.Count-2) { //If there was no collision, push them forwards
						path[path.Count-2].OccupiedUnit.IssueMovement (path[path.Count-1]);
						mods.Add (new AbilityModifier(AbilityModifier.Modifier.RemoveStunEffect, 1)); //If we are pushing them we dont stun
					}
					GetComponent<BoardUnit>().IssueMovement (curr);
					yield return new WaitForSeconds(0.33f);
					i++;
				}
				u.ReceiveAbilityHit(AbilityInProgress, mods);

				break;
			}
			default: {
				targetHexagon.OccupiedUnit.ReceiveAbilityHit(AbilityInProgress, mods);
				break;
			}
		}
		castingAbility = false;
	}

	/// <summary>
	/// Check if a hexagon should be hit with the ability
	/// </summary>
	protected bool HexagonHittable(Hexagon h) {
		if (h == targetHexagon)
			return false;

		if (h.OccupiedUnit == null)
			return false;

		if (AbilityInProgress.FriendlyFireEnabled) {
			if (h.OccupiedUnit is BoardUnit)
				return true;
		}
		else {
			if (h.OccupiedUnit is NonPlayerControlledBoardUnit)
				return true;
		}
		return false;
	}

	/// <summary>
	/// Activates the static shell.
	/// </summary>
	public void ActivateStaticShell(StatusEffect e) {
		foreach (Hexagon h in BoardManager.instance.GetStaticShellNeighbors(GetComponent<BoardUnit>().CurrentlyOccupiedHexagon, (e as BuffEffect).StaticShellDistance)) {
			if (h.OccupiedUnit != null) {
				h.OccupiedUnit.ReceiveStaticShellHit(e as BuffEffect);
			}
		}
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
		TemplateManager.instance.FinishAbility();
		AbilityInProgress = null;
		targetHexagon = null;
	}
	
}
