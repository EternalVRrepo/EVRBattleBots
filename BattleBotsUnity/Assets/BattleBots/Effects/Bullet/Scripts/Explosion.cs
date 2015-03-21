using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour
{
		[Range(0,5)]
		public float
				spawnDestroyDelay = 1;
		public GameObject particles;
		public GameObject decale;
		GameObject particlesSpawnInstance;
		GameObject decaleSpawnInstance;

		void OnCollisionEnter ()
		{
				Explode ();
		}

		public void Explode ()
		{
				Destroy (gameObject);
				
				if (particles) {
						particlesSpawnInstance = (GameObject)Instantiate (particles, transform.position, particles.transform.rotation);
						particlesSpawnInstance.transform.LookAt (Camera.main.transform.position);

						if (particlesSpawnInstance.GetComponent<ParticleSystem> ()) 
								Destroy (particlesSpawnInstance, particlesSpawnInstance.GetComponent<ParticleSystem> ().duration);
						else
								Destroy (particlesSpawnInstance, spawnDestroyDelay);
				}
				if (decale) {
						decaleSpawnInstance = (GameObject)Instantiate (decale, transform.position, transform.rotation);
						Destroy (decaleSpawnInstance, spawnDestroyDelay);
				}	
		}
}
