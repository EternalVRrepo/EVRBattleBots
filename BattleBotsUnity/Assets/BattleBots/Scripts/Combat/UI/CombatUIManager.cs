using UnityEngine;
using System.Collections;

public class CombatUIManager : MonoBehaviour {

	public Abilities abilities;
	public Transform centerEyeAnchor;
	public Phases phases;
	public SelectedCharacter selectedCharacter;

	public static CombatUIManager instance;

	void Awake() {
		instance = this;
	}

	public void SelectCharacter(Transform t) {
		selectedCharacter.SetSelectedCharacter(t);
	}

	public void PressButton(int i, bool success) {
		abilities.PressButton(i, success);
	}

	public void SetCurrentUnit(PlayerControlledBoardUnit u) {
		if (u == null) {
			abilities.StopShowing();
		}
		else {
			abilities.SetAbilities(u);
		}
	}

	public void StartMovementPhase() {
		phases.StartMovementPhase();
	}

	public void StartSelectAttackPhase() {
		phases.StartSelectAttackPhase();
	}

	public void StartTargetAttackPhase() {
		phases.StartTargetAttackPhase();
	}

	public void StartEnemyTurnPhase() {
		phases.StartEnemyTurnPhase();
	}

	void Update() {
//		transform.rotation = Quaternion.LookRotation(GameObject.Find ("CenterEyeAnchor").transform.rotation.eulerAngles);
//		transform.rotation = Quaternion.AngleAxis(GameObject.Find("RightEyeAnchor").transform.rotation.y, Vector3.up);
//		transform.rotation = Quaternion.Euler(35, OVRManager.instance.GetComponent<OVRCameraRig>().centerEyeAnchor.rotation.y, 0);
//		transform.rotation = Quaternion.Euler(35, GameObject.Find("TrackingSpace").transform.rotation.y, 0);
		transform.eulerAngles = new Vector3 (0, centerEyeAnchor.eulerAngles.y, 0f);
		transform.position = new Vector3 (centerEyeAnchor.position.x, transform.position.y, centerEyeAnchor.position.z);
		
	}
}
