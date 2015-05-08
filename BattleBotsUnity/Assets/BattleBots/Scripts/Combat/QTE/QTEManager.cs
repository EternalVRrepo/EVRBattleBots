using UnityEngine;
using System.Collections;

public class QTEManager : MonoBehaviour {

	public static QTEManager instance;

	public bool inProgress;

	void Awake() {
		instance = this;
	}

	public void StartQTE() {
		inProgress = true;
		StartCoroutine ("RunQTE");
	}

	public void StopQTE() {
		inProgress = false;
	}

	IEnumerator RunQTE() {
		yield return new WaitForSeconds(1f);
		StopQTE ();
	}

}
