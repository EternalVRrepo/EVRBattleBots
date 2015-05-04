/////////////////////////////////////////////////////////////////////////////////
//
//	BuffEffect.cs
//	© EternalVR, All Rights Reserved
//
//	description:	A positive effect that can be added to an ability or power up to be applied
//					to a board unit
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;

[System.Serializable]
public class BuffEffect : StatusEffect {

	public Buff BuffType;
	public enum Buff {
		Heal,
		Absorb,
		MovementIncrease,
		AllAbilityRangeIncrease,
		PercentDamageIncrease,
		PercentHealingIncrease,
		FullHeal,
		RemoveDebuffs,
		ApplyForceField,
		StaticShell
	}

	public int StaticShellDistance;
	public float amount;

	public BuffEffect() {}

	public BuffEffect (BuffEffect e) {
		EffectName = e.EffectName;
		EffectDurationType = e.EffectDurationType;
		Duration = e.Duration;

		BuffType = e.BuffType;
		amount = e.amount;
		StaticShellDistance = e.StaticShellDistance;
	}

	public BuffEffect (PowerUp p) {
		EffectName = p.name;
		EffectDurationType = EffectDuration.OverTime;
		Duration = p.PowerUpBonusDuration;
		BuffType = p.PowerUpEffect;
		amount = p.PowerUpBonusValue;
	}
}