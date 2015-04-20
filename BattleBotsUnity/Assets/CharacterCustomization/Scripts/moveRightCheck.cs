using UnityEngine;
using System.Collections;

public class moveRightCheck : MonoBehaviour
{

	int ranOnceTimer = 2;
	float lastTime;
	float timer;
	bool returnedOnce;
	public Transform parent;

	void Update ()
	{
		timer += Time.deltaTime;
		float zRot = transform.eulerAngles.z;
		if (zRot > 180 && zRot < 330) {
			MoveRight ();
		}

		if (zRot < 180 && zRot > 30) {
			MoveLeft ();
		}
	}

	void MoveRight ()
	{
		if (timer > ranOnceTimer) {
			timer = 0;
			parent.position += Vector3.right * 5;
		}
	}

	void MoveLeft ()
	{
		if (timer > ranOnceTimer) {
			timer = 0;
			parent.position += -Vector3.right * 5;
		}
	}
}
