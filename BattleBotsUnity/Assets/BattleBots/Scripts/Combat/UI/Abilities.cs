using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Abilities : MonoBehaviour {

	List<Icon> icons = new List<Icon>();

	void Awake() {
		foreach (Transform c in transform) {
			icons.Add(c.GetComponent<Icon>());
		}
	}

	public void SetAbilities(PlayerControlledBoardUnit u) {
		for (int i = 0; i < u.AbilityActivator.ListOfAbilities.Count; i++) {
			AbilityDescription a = u.AbilityActivator.ListOfAbilities [i];
			icons[i].SetIcon(a.AbilityIcon);
			icons[i].SetCooldown(a.Cooldown, a.currentCooldown);
			icons[i].gameObject.SetActive (true);			
		}
	}

	public void StopShowing() {
		foreach (Icon icon in icons) {
			icon.gameObject.SetActive (false);
		}
	}

	public void PressButton(int i, bool s) {
		foreach (Icon ic in icons) {
			ic.StopPress();
		}

		icons[i].PressButton(s);
	}
}
