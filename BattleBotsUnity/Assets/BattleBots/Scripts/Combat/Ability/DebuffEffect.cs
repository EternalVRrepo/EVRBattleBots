/////////////////////////////////////////////////////////////////////////////////
//
//	DebuffEffect.cs
//	© EternalVR, All Rights Reserved
//
//	description:	A negative effect that can be added to an ability or power up to be applied
//					to a board unit
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;

[System.Serializable]
public class DebuffEffect : StatusEffect {
	
	public Debuff DebuffType;
	public enum Debuff {
		Damage,
		Slow,
		Stun,
		Silence,
		Root,
		UnstableStatic
	}

	public float Damage;
	public float SlowPercent;

	public DebuffEffect() {}

	/// <summary>
	/// constructor for copying a debuff
	/// </summary>
	public DebuffEffect(DebuffEffect e) {
		EffectName = e.EffectName;
		EffectDurationType = e.EffectDurationType;
		Duration = e.Duration;

		DebuffType = e.DebuffType;
		Damage = e.Damage;
		SlowPercent = e.SlowPercent;
	}
	
}
