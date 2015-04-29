/////////////////////////////////////////////////////////////////////////////////
//
//	AbilityDescription.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This class encapsulates what an ability is, what it does, how it
//					targets, etc
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AbilityDescription : ScriptableObject {

	public string DisplayName = ""; //Name of this ability 
	public string TooltipText = ""; 
	public TargetType AbilityTargetType;
	public enum TargetType {
		TargetSelf,
		TargetUnit,
		TargetEnemy,
		TargetAlly,
		CustomTemplate
	}

	public TemplateManager.Target TemplateType;
	public TemplateManager.TargetTemplate Template;

	public int TemplateLength;
	public int TemplateWidth;

	public bool FriendlyFireEnabled;
	
//	public DamageType AbilityDamageType;
//	public enum DamageType {
//		Damage,
//		Heal,
//		Absorb
//	}

//	public AbilityType AbilityAbilityType;
//	public enum AbilityType {
//		SingleTarget,
//		Area,
//		AreaOverTime
//	}

	public int castRange; //How far from the player this ability can be cast
//	public int damage; //damage or healing per tick = damage or healing/duration if it is a DoT
//	public int Duration { //Used for DoT attacks
//		get {
//			if (AbilityAbilityType == AbilityType.AreaOverTime)
//				return duration;
//			else return -1;
//		} 
//		set {
//			duration = value;
//		}
//	}

	public int AreaOfEffectDistance; //How far from the target is affected by this ability
	public int duration;
	
	public List<DebuffEffect> debuffs = new List<DebuffEffect>();
	public List<BuffEffect> buffs = new List<BuffEffect>();

}
