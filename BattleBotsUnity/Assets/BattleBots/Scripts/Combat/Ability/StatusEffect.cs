/////////////////////////////////////////////////////////////////////////////////
//
//	StatusEffect.cs
//	© EternalVR, All Rights Reserved
//
//	description:	Whenever an ability or powerup hits a target, an AbilityHit is created which
//					creates X number of StatusEffects, and sends them to the target
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;

[System.Serializable]
public class StatusEffect {

	public string EffectName;
	
	public EffectDuration EffectDurationType;
	public enum EffectDuration {
		Instant,
		OverTime
	}
	public int Duration;
	public int HexDuration;

//	/// <summary>
//	/// 0 Arg constructor for adding status effects in editor
//	/// </summary>
//	public StatusEffect() {}
//
//	/// <summary>
//	/// Constructor for an effect from another effect (used to clone editor made effects)
//	/// </summary>
//	public StatusEffect(StatusEffect effect) {
//		damageDurationType = effect.damageDurationType;
//		statusType = effect.statusType;
//		damage = effect.damage;
//		effectDuration = effect.effectDuration;
//		slowPercent = effect.slowPercent;
//	}
//
//	/// <summary>
//	/// Constructor for turning the base ability into a status effect as opposed to creating one just for the editor
//	/// </summary>
//	public StatusEffect(int type, int value, int duration) {
//		statusType = (StatusType)type;
//
//		if (statusType == StatusType.Damage || statusType == StatusType.Heal || statusType == StatusType.Absorb) {
//			if (duration != -1) {
//				damageDurationType = DamageDurationType.OverTime;
//				damage = Mathf.CeilToInt (value/duration);
//				effectDuration = duration;		
//			}
//			else {
//				damageDurationType = DamageDurationType.Instant;
//				damage = value;
//			}
//		}
//	}
}
