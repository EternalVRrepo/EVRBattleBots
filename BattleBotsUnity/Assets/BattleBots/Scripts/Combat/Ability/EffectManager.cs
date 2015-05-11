using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EffectManager : MonoBehaviour {

	public static EffectManager instance;

	void Awake() {
		instance = this;
	}

	public void CreateEffects(AbilityDescription ab, Hexagon targetHexagon, List<Hexagon> hexagonsHit) {
		object[] param = new object[3] {ab, targetHexagon, hexagonsHit};
		StartCoroutine(ab.DisplayName, param); //Starts coroutine based on name of ability 
	}

	IEnumerator SonicStrike(object[] param) {
//		AbilityDescription ab = (AbilityDescription)param[0]; //Ability being used
//		Hexagon targetHex = (Hexagon)param[1]; //Hexagon targetted (only exists if the ability requires a hexagon or is single target
//		List<Hexagon> hits = (List<Hexagon>)param[2];



		yield return null;
	}
}
