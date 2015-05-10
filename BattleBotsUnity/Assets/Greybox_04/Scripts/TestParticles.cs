using UnityEngine;
using System.Collections;

public class TestParticles : MonoBehaviour
{

	public ParticleSystem[] groupA, groupB;
	public GameObject[] destructableObjects;

	void Start ()
	{
		foreach (ParticleSystem ps in groupA) {
			ps.Play ();
		}
	}
	
	void Update ()
	{
		if (Input.GetMouseButtonDown (0)) {
			playGroupB ();
		}
		if (Input.GetMouseButtonDown (1)) {
			ResetParticles ();
		}
	}

	void playGroupB ()
	{
		foreach (ParticleSystem ps in groupA) {
			ps.Stop ();
		}

		foreach (GameObject go in destructableObjects) {
			go.renderer.enabled = false;
		}

		foreach (ParticleSystem ps in groupB) {
			ps.Play ();
		}
	}

	void ResetParticles ()
	{
		foreach (ParticleSystem ps in groupA) {
			ps.Play ();
		}
		
		foreach (ParticleSystem ps in groupB) {
			ps.Stop ();
		}

		foreach (GameObject go in destructableObjects) {
			go.renderer.enabled = true;
		}
	}
}
