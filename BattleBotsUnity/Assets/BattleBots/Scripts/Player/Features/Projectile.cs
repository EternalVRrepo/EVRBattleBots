using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]

public class Projectile : MonoBehaviour
{
		public bool isMortor;
		public float lifeTime = 3f;
		public float speed = 10f;
		
		Vector3 startPosition;
		float distance;

		void Start ()
		{
				if (isMortor) 
						rigidbody.useGravity = true;
				startPosition = transform.position;
				Destroy (gameObject, lifeTime);
		}
		
		void Update ()
		{
				distance = Vector3.Distance (startPosition, transform.position);
				if (distance > HeroStats.playerStats.attackRange) {
						Destroy (gameObject);
				}

				rigidbody.velocity = transform.forward * speed;
		}
}
