using UnityEngine;
using System.Collections;

public class MyHeroController : MonoBehaviour
{
	public float forwardSpeed = 3f, jumpSpeed = 15f;
	float airSpeed;
	//backwardSpeed, strafeSpeed;
	Vector3 movement, velocity;
	float yJoystick, xJoystick;

	CharacterController character;
	Animator animator;

	void Start ()
	{
		character = transform.GetComponent<CharacterController> ();
		animator = transform.GetComponentInChildren<Animator> ();
		//Set Speeds
		airSpeed = forwardSpeed * 0.8f;
//		backwardSpeed = forwardSpeed * 0.5f;
//		strafeSpeed = forwardSpeed * 0.75f;
	}
	
	void Update ()
	{
		xJoystick = Input.GetAxis ("Horizontal");
		yJoystick = Input.GetAxis ("Vertical");

		if (Mathf.Round (xJoystick) != 0 || Mathf.Round (yJoystick) != 0) 
			animator.SetBool ("Walking", true);
		else
			animator.SetBool ("Walking", false);

		Jump ();
	}

	void FixedUpdate ()
	{
		Move ();
	}

	void Jump ()
	{
		//Check for jump
		if (character.isGrounded) {
			if (Input.GetButtonDown ("Jump")) {
				animator.SetTrigger ("Jump");
				velocity = character.velocity;
				velocity.y = jumpSpeed;
			}
		} else {
			//Apply gravity to our velocity to diminish it over time
			velocity.y += Physics.gravity.y * Time.deltaTime;
			
			//Movement in-air
			movement.x *= airSpeed;
			movement.z *= airSpeed;
		}
	}

	void Move ()
	{	
		if (xJoystick != 0 || yJoystick != 0) {
			//Rotate Towards Joystick Direction
			transform.rotation = Quaternion.LookRotation (new Vector3 (xJoystick, 0, yJoystick), Vector3.up);
			//Move Towards Joystick Direction
			movement = new Vector3 (xJoystick, 0, yJoystick) * forwardSpeed;
		}

		//Scale movement with physics
		movement += velocity;	
		movement += Physics.gravity;
		movement *= Time.deltaTime;
		
		//Move the character
		character.Move (movement);
		
		//Remove velocity after landing
		if (character.isGrounded)
			velocity = Vector3.zero;
	}
}
