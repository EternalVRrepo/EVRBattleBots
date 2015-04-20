/////////////////////////////////////////////////////////////////////////////////
//
//	Talent.cs
//	© EternalVR, All Rights Reserved
//
//	description:	What type of talent this is and what it does
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;

[System.Serializable]
public class Talent {

	public string TalentName;
	public Texture TalentTexture;

	public TalentType ThisTalentType;
	public enum TalentType { //What type of bonus is gained from this talent
		StatBoost,
		AbilityBoost,
		Special
	}

	public float StatBoostAmount; //How much of a stat boost
	public Stat StatType;
	public enum Stat { //What type of stat
		Health,
		MoveDistance,
		CCReductionPercent,
		DamageReductionPercent,
		DamageDealtPercent,
		HealingIncreasePercent,
		HealingReceivedIncreasePercent
	}

	public string AbilityModified; //What ability is getting a bonus
	public int AbilityModificationAmount;
	public AbilityModification AbilityModType; 
	public enum AbilityModification { //What type of bonus we are giving
		CooldownReduction,
		Range,
		Damage
	}

	public AbilityDescription SpecialAbilityModification; //If this is a special ability mod, what ability is it
	public int SpecialAbilityModIdentifer; //What type of mod incase there are multiple choices
}
