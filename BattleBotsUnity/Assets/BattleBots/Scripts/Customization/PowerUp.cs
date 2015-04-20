/////////////////////////////////////////////////////////////////////////////////
//
//	PowerUp.cs
//	© EternalVR, All Rights Reserved
//
//	description:	A power up that you can use in combat for special bonuses
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;

[System.Serializable]
public class PowerUp : ScriptableObject {

	public PowerUpTarget PowerUpTargetType;
	public enum PowerUpTarget {
		CurrentUnit,
		WholeParty
	}

	public PowerUpBonus PowerUpBonusType;
	public enum PowerUpBonus {
		Healing,
		MovementDistance,
		AllAbilityRanges,
		PercentDamageIncrease,
		RemoveDebuffs
	}

	public float PowerUpBonusValue;
	public int PowerUpBonusDuration;
}
