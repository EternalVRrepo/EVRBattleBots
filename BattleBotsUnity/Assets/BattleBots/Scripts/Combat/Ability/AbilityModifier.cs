using UnityEngine;
using System.Collections;

public class AbilityModifier {

	public Modifier ModifierType;
	public enum Modifier {
		Damage,
		RemoveStunEffect,
	}

	public float ModifierAmount;

	public AbilityModifier(Modifier mod, float amount) {
		ModifierType = mod;
		ModifierAmount = amount;
	}
}
