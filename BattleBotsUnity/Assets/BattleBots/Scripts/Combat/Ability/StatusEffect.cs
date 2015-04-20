/////////////////////////////////////////////////////////////////////////////////
//
//	StatusEffect.cs
//	© EternalVR, All Rights Reserved
//
//	description:	Whenever an ability hits a target, an AbilityHit is created which
//					creates X number of StatusEffects, and sends them to the target
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;

[System.Serializable]
public class StatusEffect {

	public string StatusEffectName;

	public enum StatusType {
		Damage,
		Heal,
		Absorb,
		Slow,
		Stun
	}

	public enum DamageDurationType {
		Instant,
		OverTime
	}

	public DamageDurationType damageDurationType;
	public StatusType statusType;
	public float damage;
	public int effectDuration;
	public float slowPercent;

	/// <summary>
	/// 0 Arg constructor for adding status effects in editor
	/// </summary>
	public StatusEffect() {}

	/// <summary>
	/// Initializes a new instance cloning an old ne
	/// </summary>
	public StatusEffect(StatusEffect effect) {
		damageDurationType = effect.damageDurationType;
		statusType = effect.statusType;
		damage = effect.damage;
		effectDuration = effect.effectDuration;
		slowPercent = effect.slowPercent;
	}

	/// <summary>
	/// Constructor for turning the base ability into a status effect as opposed to creating one just for the editor
	/// </summary>
	public StatusEffect(int type, int value, int duration) {
		statusType = (StatusType)type;

		if (statusType == StatusType.Damage || statusType == StatusType.Heal || statusType == StatusType.Absorb) {
			if (duration != -1) {
				damageDurationType = DamageDurationType.OverTime;
				damage = Mathf.CeilToInt (value/duration);
				effectDuration = duration;		
			}
			else {
				damageDurationType = DamageDurationType.Instant;
				damage = value;
			}
		}
	}
}
