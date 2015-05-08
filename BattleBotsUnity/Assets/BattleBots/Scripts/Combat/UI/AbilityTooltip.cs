using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AbilityTooltip : MonoBehaviour {

	public Text AbilityNameText;
	public Text AbilityDescriptionText;

	public GameObject AbilityNameBackground;
	public GameObject AbilityDescriptionBackground;
	public GameObject Background;

	public bool displayed {
		set {
			SetDisplayed(value);
			_displayed = value;
		}
		get {
			return _displayed;
		}
	}

	private bool _displayed;

	public void SetAbility(AbilityDescription ab) {
		AbilityNameText.text = ab.DisplayName;
		AbilityDescriptionText.text = ab.TooltipText;
	}

	protected void SetDisplayed(bool b) {
		AbilityNameBackground.gameObject.SetActive (b);
		AbilityDescriptionBackground.gameObject.SetActive (b);
		Background.SetActive (b);
	}
}
