using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Phases : MonoBehaviour {

	public Sprite MovePhaseSprite;
	public Sprite SelectAttackPhaseSprite;
//	public Sprite TargetAttackPhaseSprite;
	public Sprite EnemyTurnPhaseSprite;

	public Image image;
	public Color clearColor;

	public float fadeSpeed = 2;

	void Start() {
		clearColor = image.color;
		clearColor.a = 0;
	}

	public void StartMovementPhase() {
		if (popupInProgress) {
			image.color = clearColor;
			StopCoroutine ("PhasePopup");
		}
		image.sprite = MovePhaseSprite;
		StartCoroutine ("PhasePopup");
	}
	
	public void StartSelectAttackPhase() {
		if (popupInProgress) {
			image.color = clearColor;
			StopCoroutine ("PhasePopup");
		}

		image.sprite = SelectAttackPhaseSprite;
		StartCoroutine ("PhasePopup");
	}
	
//	public void StartTargetAttackPhase() {
//		if (popupInProgress) {
//			image.color = clearColor;
//			StopCoroutine ("PhasePopup");
//		}
//
//		image.sprite = TargetAttackPhaseSprite;
//		StartCoroutine ("PhasePopup");
//	}

	public void StartEnemyTurnPhase() {
		if (popupInProgress) {
			image.color = clearColor;
			StopCoroutine ("PhasePopup");
		}
		
		image.sprite = EnemyTurnPhaseSprite;
		StartCoroutine ("PhasePopup");
	}


	public bool popupInProgress;
	IEnumerator PhasePopup() {

		popupInProgress = true;
		image.gameObject.SetActive (true);

		while (image.color.a < 1) {
//			image.CrossFadeAlpha(2, fadeSpeed, true);
			Color c = image.color;
			c.a = Mathf.MoveTowards(image.color.a, 1, Time.deltaTime*fadeSpeed);
			image.color = c;
			yield return null;
		}

		yield return new WaitForSeconds(1f);

		while (image.color.a > 0) {
			Color c = image.color;
			c.a = Mathf.MoveTowards(image.color.a, 0, Time.deltaTime*fadeSpeed);
			image.color = c;
			yield return null;
		}

		image.gameObject.SetActive (false);
		popupInProgress = false;
	}
}
