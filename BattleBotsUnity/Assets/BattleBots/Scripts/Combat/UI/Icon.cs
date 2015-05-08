using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Icon : MonoBehaviour {

	public Image image;
	public Image Cooldown;
	
	public Color normalColor;
	public Color pressedColor;
	public Color disabledColor;

	public Color cooldownColor;

	void Awake() {
		image = GetComponent<Image>();
		Cooldown = transform.GetChild(0).GetComponent<Image>();
		cooldownColor = Cooldown.color;
	}

	public void SetIcon(Sprite s) {
		image.color = normalColor;
		Cooldown.color = cooldownColor;
		GetComponent<Image>().sprite = s;
	}

	public void SetCooldown(int Total, int Remaining) {
		if (Total == 0 || Remaining == 0) {
			Cooldown.fillAmount = 0;
		}
		else {
			float fill = (float)Remaining/(float)Total;
			Cooldown.fillAmount = fill;
			
		}
	}

	public void PressButton(bool s) {
//		Debug.Log (name + " pressed");
		if (pressing) {
			image.color = normalColor;
			Cooldown.color = cooldownColor;
			StopCoroutine("SimulateButtonPress");
		}

		StartCoroutine ("SimulateButtonPress", s);
	}

	bool pressing;
	IEnumerator SimulateButtonPress(bool s) {
		pressing = true;

		float t = 0;
		if (s) {
			while (t < .25f) {
				image.color = Color.Lerp (image.color, pressedColor, .25f);
				Cooldown.color = Color.Lerp (image.color, pressedColor, .25f);
				t += Time.deltaTime;
				yield return null;
			}
		}
		else {
			while (t < .25f) {
				image.color = Color.Lerp (image.color, disabledColor, .25f);
				Cooldown.color = Color.Lerp (image.color, disabledColor, .25f);
				
				t += Time.deltaTime;
				yield return null;
			}
		}

		yield return new WaitForSeconds(.2f);

		while (t > 0) {
			image.color = Color.Lerp(image.color, normalColor, .25f);
			Cooldown.color = cooldownColor;
			t -= Time.deltaTime;
			yield return null;
		}
		image.color = normalColor;

		pressing = false;
	}
}
