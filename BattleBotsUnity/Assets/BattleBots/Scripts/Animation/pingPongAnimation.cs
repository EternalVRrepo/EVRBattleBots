using UnityEngine;
using System.Collections;

/// <summary>
/// Goal is to fucking have lines in the eye, particales raining and them sending line renders to each other at the do product of targeted move positions
/// </summary>
public class pingPongAnimation : MonoBehaviour
{
	public float speed;
	public float scale;
	public float shotsFiredSpeed;
	public Transform target;

	Vector3 startPosition;
	Vector3 startPoint, endPoint, randomPosition;
	LineRenderer lineR;

	void Start ()
	{
		startPosition = transform.position;
		lineR = GetComponent<LineRenderer> ();
		InvokeRepeating ("setRandom", 0f, shotsFiredSpeed);
		StartCoroutine ("ShotsFired");
	}

	void setRandom ()
	{	
		randomPosition = Random.insideUnitSphere * scale + startPosition;
		randomPosition.y = startPosition.y;
		Debug.Log (randomPosition);
	}

	IEnumerator ShotsFired ()
	{
		lineR.enabled = true;
		lineR.SetPosition (0, transform.position);
		lineR.SetPosition (1, target.position);
		yield return new WaitForSeconds (1f);
		lineR.enabled = false;
		yield return new WaitForSeconds (shotsFiredSpeed);
		StartCoroutine ("ShotsFired");
	}

	void Update ()
	{
		pingPong ();
	}

	void pingPong ()
	{
		transform.position = Vector3.Lerp (transform.position, randomPosition, Time.deltaTime * speed);
	}
}
