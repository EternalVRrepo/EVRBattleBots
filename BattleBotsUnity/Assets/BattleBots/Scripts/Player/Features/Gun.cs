using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour
{
	public int damagePerShot = 20;                  // The damage inflicted by each bullet.
	public float timeBetweenBullets = 0.15f;        // The time between each shot.
	//		public float range = 100f;                      // The distance the gun can fire. HeroStats.playerStats.attackRange
	public IKController ikController;
		
	float timer;                                    // A timer to determine when to fire.
	Ray shootRay;                                   // A ray from the gun end forwards.
	RaycastHit shootHit;                            // A raycast hit to get information about what was hit.
//		int shootableMask;                              // A layer mask so the raycast only hits things on the shootable layer.
	ParticleSystem gunParticles;                    // Reference to the particle system.
	LineRenderer gunLine;                           // Reference to the line renderer.
	AudioSource gunAudio;                           // Reference to the audio source.
	Light gunLight;                                 // Reference to the light component.
	float effectsDisplayTime = 0.2f;                // The proportion of the timeBetweenBullets that the effects will display for.

	public GameObject projectile;
	GameObject cachedProjectile;	
		
	void Awake ()
	{
		// Create a layer mask for the Shootable layer.
//				shootableMask = LayerMask.GetMask ("Shootable");
			
//				gunParticles = GetComponent<ParticleSystem> ();
		gunLine = GetComponent <LineRenderer> ();
//				gunAudio = GetComponent<AudioSource> ();
		gunLight = GetComponent<Light> ();
	}
		
	void Update ()
	{
		// Add the time since Update was last called to the timer.
		timer += Time.deltaTime;
			
		// If the Fire1 button is being press and it's time to fire...
		if (Input.GetButton ("Shoot") && timer >= timeBetweenBullets) {
			Shoot ();
		}
			
		// If the timer has exceeded the proportion of timeBetweenBullets that the effects should be displayed for...
		if (timer >= timeBetweenBullets * effectsDisplayTime) {
			DisableEffects ();
		}
	}
		
	public void DisableEffects ()
	{
		gunLine.enabled = false;
		gunLight.enabled = false;
		ikController.rightHandWeight = Mathf.Lerp (ikController.rightHandWeight, 0, Time.deltaTime * 3);
	}
		
	void Shoot ()
	{
		ikController.rightHandWeight = 1;
		cachedProjectile = (GameObject)Instantiate (projectile, transform.position, Quaternion.LookRotation (transform.forward));
		Projectile projectileClass;
		
		if (cachedProjectile.GetComponent<Projectile> ()) {
			projectileClass = cachedProjectile.GetComponent<Projectile> ();
			
		} else {
			Debug.Log ("Added a Projectile Class to: " + cachedProjectile);
			cachedProjectile.AddComponent<Projectile> ();
			projectileClass = cachedProjectile.GetComponent<Projectile> ();
		}
		
		if (!cachedProjectile.rigidbody) {
			Debug.Log ("Added a RigidBody to: " + cachedProjectile);
			cachedProjectile.AddComponent<Rigidbody> ();
			if (!projectileClass.isMortor)
				cachedProjectile.rigidbody.useGravity = false;
		}


		// Reset the timer.
		timer = 0f;
			
//				// Play the gun shot audioclip.
//				gunAudio.Play ();
			
		// Enable the light.
		gunLight.enabled = true;
			
//				// Stop the particles from playing if they were, then start the particles.
//				gunParticles.Stop ();
//				gunParticles.Play ();
			
		// Enable the line renderer and set it's first position to be the end of the gun.
		gunLine.enabled = true;
		gunLine.SetPosition (0, transform.position);
			
		// Set the shootRay so that it starts at the end of the gun and points forward from the barrel.
		shootRay.origin = transform.position;
		shootRay.direction = transform.forward;
			
		// Perform the raycast against gameobjects on the shootable layer and if it hits something...
		if (Physics.Raycast (shootRay, out shootHit, HeroStats.playerStats.attackRange)) {
			// Try and find an EnemyHealth script on the gameobject hit.
//						EnemyHealth enemyHealth = shootHit.collider.GetComponent <EnemyHealth> ();
				
			// If the EnemyHealth component exist...
//						if (enemyHealth != null) {
//								// ... the enemy should take damage.
//								enemyHealth.TakeDamage (damagePerShot, shootHit.point);
//						}
				
			// Set the second position of the line renderer to the point the raycast hit.
			gunLine.SetPosition (1, shootHit.point);
		}
			// If the raycast didn't hit anything on the shootable layer...
			else {
			// ... set the second position of the line renderer to the fullest extent of the gun's range.
			gunLine.SetPosition (1, shootRay.origin + shootRay.direction * HeroStats.playerStats.attackRange);
		}
	}
}
