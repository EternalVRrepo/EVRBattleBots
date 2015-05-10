﻿// =======================================================================================
// Copyright (c) 2014, Merge Labs Inc.
// =======================================================================================
/// The MergePlayerController: Allows you to move the character forwards and backwards
/// ======================================================================================
using UnityEngine;
using System.Collections;

public class MyHeroController3rdPerson : MonoBehaviour
{
	#region Variables
	public float forwardSpeed = 3f, jumpSpeed = 15f;
	public Transform CenterEyeAnchor;
	public Transform cameraRig;
	float airSpeed;
//	float airSpeed, backwardSpeed, strafeSpeed;
	Vector3 movement, velocity;
	float yJoystick, xJoystick;

	CharacterController character;
	Animator animator;

	GameObject thirdPersonCameraLockPivot; //For Third Person Camera Lock, a cached transform set to the base of the character

	//Used For A Forward Camera Direction Not Influenced By The Roll & Tilt Of Headtracking. 
	GameObject thirdPersonCameraDummyObject; 
	#endregion

	//============================================================================
	// Initilization
	//============================================================================

	void Start ()
	{
		CenterEyeAnchor = GameObject.Find ("CenterEyeAnchor").transform;
		cameraRig = GameObject.Find ("OVRCameraRig").transform;


		//Get Components
		character = transform.GetComponent<CharacterController> ();
		animator = transform.GetComponentInChildren<Animator> ();

		//Set Speeds
		airSpeed = forwardSpeed * 0.8f;

		//Set Up Rig
		if (thirdPersonCameraLockPivot == null) {
			//Create Dummy Object
			thirdPersonCameraDummyObject = new GameObject ("thirdPersonCameraDummyObject"); 
			//Create Pivot
			thirdPersonCameraLockPivot = new GameObject ("thirdPersonCameraLockPivot");
			//Set Pivot in center of the character
			thirdPersonCameraLockPivot.transform.position = transform.position + new Vector3 (0, character.height / 2, 0); 
			//Move Camera To Perspective
			cameraRig.position = new Vector3 (transform.position.x, transform.position.y + 2, transform.position.z - 4f); 
			//Parents Camera to Pivot
			cameraRig.parent = thirdPersonCameraLockPivot.transform;
		}	
	}

	//============================================================================
	// Update
	//============================================================================

	void FixedUpdate ()
	{
		CameraSetUp ();
		Move ();
	}

	//The Camera Rig
	void CameraSetUp ()
	{
		//Keep Pivot On Character's Center
		thirdPersonCameraLockPivot.transform.position = Vector3.Lerp (thirdPersonCameraLockPivot.transform.position, transform.position + new Vector3 (0, character.height / 2, 0), Time.deltaTime * 10);

		//Set Up Dummmy Object
		//Set Position
		thirdPersonCameraDummyObject.transform.position = CenterEyeAnchor.position; 
		//Sets Forward Direction
		thirdPersonCameraDummyObject.transform.forward = CenterEyeAnchor.forward;

		thirdPersonCameraDummyObject.transform.eulerAngles = new Vector3 (0, thirdPersonCameraDummyObject.transform.eulerAngles.y, 0); //Removed Roll & Tilt
	}

	void Move ()
	{
		//Move
		if (Mathf.Round (xJoystick) != 0 || Mathf.Round (yJoystick) != 0) {

			//Player Rotation Lock To Camera Y-Axis
			transform.rotation = Quaternion.Euler (0, CenterEyeAnchor.eulerAngles.y, 0);

			//Rotate Towards Joystick Direction
			transform.rotation *= Quaternion.LookRotation (new Vector3 (xJoystick, 0, yJoystick), Vector3.up);


			//Move Towards Joystick Direction
			movement = new Vector3 (thirdPersonCameraDummyObject.transform.forward.x * yJoystick * forwardSpeed, 0, thirdPersonCameraDummyObject.transform.forward.z * yJoystick * forwardSpeed);
			movement += new Vector3 (thirdPersonCameraDummyObject.transform.right.x * xJoystick * forwardSpeed, 0, thirdPersonCameraDummyObject.transform.right.z * xJoystick * forwardSpeed);
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

	void Update ()
	{
		//Collect Data
		xJoystick = Input.GetAxis ("Horizontal");
		yJoystick = Input.GetAxis ("Vertical");
		
		//Animation
		if (Mathf.Round (xJoystick) != 0 || Mathf.Round (yJoystick) != 0) 
			animator.SetBool ("Walking", true);
		else
			animator.SetBool ("Walking", false);
		
		//Check for Jump
		Jump ();
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
	//============================================================================
	// Events
	//============================================================================

	void OnControllerColliderHit (ControllerColliderHit hit)
	{
		if (hit.gameObject.tag == "Infection") {
			Debug.Log ("Hit Infected Object: " + hit.gameObject.name);
			hit.gameObject.GetComponent<Infection> ().Interact ();
		}
	}


	/// Tries to interact with whatever its looking at
	void TryInteract ()
	{
		if (!character.isGrounded)
			return;
		
		//Raycast for an infected object here
		//InfectedObject.Interact();
	}
	//============================================================================
}
