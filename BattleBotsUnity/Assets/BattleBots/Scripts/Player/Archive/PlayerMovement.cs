//using UnityEngine;
//using System.Collections;
//
//public class PlayerMovement : MonoBehaviour
//{
//	Animator animator;
//	bool isJumping;
//	float speed = 0.01f;
//	CharacterController characterController;
//	Vector3 direction;
//
//	void Start ()
//	{
//		animator = GetComponent<Animator> ();
//		characterController = GetComponent<CharacterController> ();
//	}
//	
//	void Update ()
//	{
//		MovementEventHandler ();
//			
//		direction = new Vector3 (Input.GetAxis ("Horizontal"), 0, Input.GetAxis ("Vertical"));
//		if (Input.GetAxis ("Vertical") != 0 || Input.GetAxis ("Horizontal") != 0) {
//			animator.SetBool ("Walking", true);
//			characterController.Move (transform.forward * speed);
//			transform.rotation = Quaternion.LookRotation (direction, Vector3.up);
//		} else {
//			animator.SetBool ("Walking", false);
//		}
//
//	}
//
//	void MovementEventHandler ()
//	{
////				if (animator.GetBool ("Crouch")) {
////						speed = 0.05f;
////				} else {
////						speed = 0.1f;
////				}
//
//		if (Input.GetButtonDown ("Fire1")) {
//			animator.SetBool ("Crouch", true);
//		}
//
//		if (Input.GetButtonUp ("Fire1")) {
//			animator.SetBool ("Crouch", false);
//		}
//		
//		if (Input.GetButtonDown ("Fire2")) {
//			StartCoroutine ("Jump");
//		}
//	}
//	
//	void CrouchToggle ()
//	{
//		animator.SetBool ("Crouch", !animator.GetBool ("Crouch"));
//	}
//		
//	IEnumerator Jump ()
//	{
//		if (characterController.isGrounded) {
//			animator.SetTrigger ("Jump");
//			isJumping = true;
//			yield return new WaitForSeconds (0.5f);	//TODO find a standard jump time. universal for all characters
//			isJumping = false;
//		}
//	}
//}
