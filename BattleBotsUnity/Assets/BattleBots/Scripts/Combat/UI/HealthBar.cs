using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour {

	public BoardUnit unitFollowed;
	public int displayedHealth;
	public Slider healthBar;

	public void SetUnit(BoardUnit u) {
		unitFollowed = u;
	}

	void Update() {
		transform.position = unitFollowed.transform.position + Vector3.up*2.5f;
		transform.LookAt(OVRManager.instance.transform.position, Vector3.up);
		if (displayedHealth != unitFollowed.CurrentHealth) {
			displayedHealth = unitFollowed.CurrentHealth;
			healthBar.value = (float)displayedHealth/(float)unitFollowed.MaxHealth;
		}
	}

}
