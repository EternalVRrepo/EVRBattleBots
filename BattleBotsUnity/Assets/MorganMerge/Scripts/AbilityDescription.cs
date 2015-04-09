/////////////////////////////////////////////////////////////////////////////////
//
//	Ability.cs
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

[System.Serializable]
public class AbilityDescription : ScriptableObject {

	public string DisplayName = ""; //Name of this ability 

	public TargetType AbilityTargetType;
	public enum TargetType {
		TargetSelf,
		TargetUnit,
		TargetEnemy,
		TargetAlly,
		TargetHexagon,
		LineDraw
	}

	public int castRange; //How far from the player this ability can be cast
	public int damage; //A negative number for healing, a positive number for damage, damage per tick = damage/duration if it is a DoT
	public float duration; //Used for DoT attacks

	public DamageType AbilityDamageType;
	public enum DamageType {
		None,
		SingleTarget,
		SingleTargetOverTime,
		Area,
		AreaOverTime
	}
}
