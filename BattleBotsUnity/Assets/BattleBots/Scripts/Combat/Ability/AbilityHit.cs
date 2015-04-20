/////////////////////////////////////////////////////////////////////////////////
//
//	AbilityHit.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This is sent to a BoardUnit when some sort of ability affects them
//					it holds a list of status effects	
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityHit
{
	public List<StatusEffect> effects = new List<StatusEffect>();

	public AbilityHit (AbilityDescription ability) {
		StatusEffect effect = new StatusEffect((int)ability.AbilityDamageType, ability.damage, ability.Duration);
		effects.Add (effect);
		foreach (StatusEffect eff in ability.effects) {
			effects.Add (eff);
		}
	}
}


