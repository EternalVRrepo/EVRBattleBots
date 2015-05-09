using UnityEngine;
using System.Collections;

public class EffectManager : MonoBehaviour {

	public static EffectManager instance;

	void Awake() {
		instance = this;
	}

	public void CreateEffects(AbilityDescription ab, Hexagon targetHexagon) {
		object[] param = new object[2] {ab, targetHexagon};
		StartCoroutine(ab.ToString(), param); //Starts coroutine based on name of ability 
	}

	IEnumerator SonicStrike(object[] param) {
		AbilityDescription ab = (AbilityDescription)param[1]; //Ability being used
		Hexagon targetHex = (Hexagon)param[2]; //Hexagon targetted (only exists if the ability requires a hexagon or is single target



		yield return null;
	}
}
