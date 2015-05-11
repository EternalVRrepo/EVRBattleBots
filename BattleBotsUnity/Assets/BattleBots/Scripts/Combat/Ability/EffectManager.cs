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

	#region WarriorAbilities
	IEnumerator SonicStrike(object[] param) {
		AbilityDescription ab = (AbilityDescription)param[0]; //Ability being used
		Hexagon targetHex = (Hexagon)param[1]; //Hexagon targetted (only exists if the ability requires a hexagon or is single target

		GameObject effect = Instantiate(ab.EffectPrefab) as GameObject;
		effect.transform.position = targetHex.transform.position;
		effect.transform.LookAt(ab.sourceUnit.transform.position);
		effect.transform.Rotate(new Vector3(0, 180, 0));

		yield return new WaitForSeconds(3f);
		effect.SetActive (false);
	}

	IEnumerator Debilitate(object[] param) {
		yield return null;
	}

	IEnumerator StaticRush(object[] param) {
		yield return null;
	}

	IEnumerator TectonicSlam(object[] param) {
		AbilityDescription ab = (AbilityDescription)param[0]; 

		GameObject effect = Instantiate (ab.EffectPrefab) as GameObject;
		effect.transform.position = ab.sourceUnit.CurrentlyOccupiedHexagon.transform.position;

		yield return new WaitForSeconds(3f);
		effect.SetActive (false);
	}
	#endregion

	#region WizardAbilities
	IEnumerator RadiantEnergy(object[] param) {
		AbilityDescription ab = (AbilityDescription)param[0]; 
		
		GameObject effect = Instantiate(ab.EffectPrefab) as GameObject;
		effect.transform.position = ab.targetHexagon.transform.position;


        yield return new WaitForSeconds(15f);
        effect.SetActive (false);
	}

	IEnumerator CircuitBreak(object[] param) {
		AbilityDescription ab = (AbilityDescription)param[0]; 
		Hexagon targetHex = (Hexagon)param[1];
		List<Hexagon> hits = (List<Hexagon>)param[2];

		foreach (Hexagon h in hits) { 
			if (h.OccupiedUnit != null) {
				GameObject effect = Instantiate (ab.EffectPrefab) as GameObject;
				effect.transform.position = h.OccupiedUnit.transform.position;
			}
		}

		yield return null;
	}

	IEnumerator StaticGrip(object[] param) {
		AbilityDescription ab = (AbilityDescription)param[0]; 
		Hexagon targetHex = (Hexagon)param[1];
		
		GameObject effect = Instantiate (ab.EffectPrefab) as GameObject;
		effect.transform.position = targetHex.transform.position;
		effect.SetActive (true);

		yield return new WaitForSeconds(15f);
		effect.SetActive (false);
	}

	IEnumerator FluxBlast(object[] param) {
		AbilityDescription ab = (AbilityDescription)param[0]; 
		Hexagon targetHex = (Hexagon)param[1]; 
		
		GameObject effect = Instantiate (ab.EffectPrefab) as GameObject;
		effect.transform.position = ab.targetHexagon.transform.position;
		
		yield return new WaitForSeconds(3f);
		effect.SetActive (false);
	}
	#endregion
}
