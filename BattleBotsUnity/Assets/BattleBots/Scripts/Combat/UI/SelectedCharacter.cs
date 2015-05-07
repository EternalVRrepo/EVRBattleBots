using UnityEngine;
using System.Collections;

public class SelectedCharacter : MonoBehaviour
{

	Transform target;

	public void SetSelectedCharacter (Transform t)
	{
		if (t == null) {
			gameObject.SetActive (false);
			target = null;
		} else {
			gameObject.SetActive (true);
			target = t;
			transform.position = t.position + Vector3.up * 3.5f;
		}
	}

	void Update ()
	{
		if (target != null) {
			transform.LookAt (OVRManager.instance.transform.position, Vector3.up);
			transform.position = target.position + Vector3.up * 3.5f;
			transform.position = new Vector3 (transform.position.x, Mathf.PingPong (Time.time * 0.5f, 0.5f) + 2.5f, transform.position.z);
		}
	}

}
