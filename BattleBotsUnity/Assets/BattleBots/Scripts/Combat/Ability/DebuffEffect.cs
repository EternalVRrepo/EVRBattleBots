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
		StaticGrip,
		UnstableStatic,
		Enfeeble,
	}

	public float Damage;
	public float SlowPercent;
	public int StaticGripDamagePerHex; 

	public bool RequireUnstableStatic;

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

	/// <summary>
	/// New unstable static effect
	/// </summary>
	public DebuffEffect(string name, Debuff debuffType, int damage, int duration) {
		EffectName = name;
		DebuffType = debuffType;
		Duration = duration;
		EffectDurationType = EffectDuration.OverTime;
		Damage = damage;
	}
}
