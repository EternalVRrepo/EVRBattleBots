// =======================================================================================
// Copyright (c) 2014, Merge Labs Inc.
// =======================================================================================
/// The MergePlayerController: Allows you to move the character forwards and backwards
/// ======================================================================================
using UnityEngine;
using System.Collections;

public class MyHeroController3rdPerson : MonoBehaviour
{
	public float forwardSpeed = 3f, jumpSpeed = 15f;
	public Transform cameraController;
	public Transform OVRCamera, OVRCenterAnchor;
	public Transform cameraRig;
	float airSpeed;
//	float airSpeed, backwardSpeed, strafeSpeed;
	Vector3 movement, velocity;
	float yJoystick, xJoystick;

	CharacterController character;
	Animator animator;

	GameObject thirdPersonCameraLockPivot; //For Third Person Camera Lock, a cached transform set to the base of the character
	GameObject thirdPersonCameraDummyObject; //Used For A Forward Camera Direction Not Influenced By The Roll & Tilt Of Headtracking. 
	
	void Start ()
	{
		//Get Components
		character = transform.GetComponent<CharacterController> ();
		animator = transform.GetComponentInChildren<Animator> ();

		//Set Speeds
		airSpeed = forwardSpeed * 0.8f;
//		backwardSpeed = forwardSpeed * 0.5f;
//		strafeSpeed = forwardSpeed * 0.75f;

		//Set Up Rig
		if (thirdPersonCameraLockPivot == null) {
			thirdPersonCameraDummyObject = new GameObject ("thirdPersonCameraDummyObject"); //Create Dummy Object
			thirdPersonCameraLockPivot = new GameObject ("thirdPersonCameraLockPivot"); //Create Pivot
			thirdPersonCameraLockPivot.transform.position = transform.position + new Vector3 (0, character.height / 2, 0); //Set Pivot in center of the character
			cameraRig.position = new Vector3 (transform.position.x, transform.position.y + 2, transform.position.z - 4f); //Move Camera To Perspective
			cameraRig.parent = thirdPersonCameraLockPivot.transform; //Parents Camera to Pivot
		}
	}

	//The Camera Rig
	void CameraSetUp ()
	{
		//Keep Pivot On Character's Center
		thirdPersonCameraLockPivot.transform.position = Vector3.Lerp (thirdPersonCameraLockPivot.transform.position, transform.position + new Vector3 (0, character.height / 2, 0), Time.deltaTime * 10);
		
		//Set Up Dummmy Object
		thirdPersonCameraDummyObject.transform.position = cameraController.position; //Set Position
		thirdPersonCameraDummyObject.transform.forward = cameraController.forward; //Sets Forward Direction
		thirdPersonCameraDummyObject.transform.eulerAngles = new Vector3 (0, thirdPersonCameraDummyObject.transform.eulerAngles.y, 0); //Removed Roll & Tilt
	}

	void FixedUpdate ()
	{
		CameraSetUp ();
		Move ();
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

	void Move ()
	{
		//Move
		if (Mathf.Round (xJoystick) != 0 || Mathf.Round (yJoystick) != 0) {

			//Player Rotation Lock To Camera Y-Axis
			transform.rotation = Quaternion.Euler (0, cameraController.eulerAngles.y, 0);

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
			//						//Apply gravity to our velocity to diminish it over time
			velocity.y += Physics.gravity.y * Time.deltaTime;
			
			//Movement in-air
			movement.x *= airSpeed;
			movement.z *= airSpeed;
		}
	}
}
