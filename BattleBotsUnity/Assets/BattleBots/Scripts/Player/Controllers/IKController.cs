using UnityEngine;
using System.Collections;

public class IKController : MonoBehaviour
{
	
		public Transform leftHandObj, rightHandObj;

		[Range(0,1)]
		public float
				leftHandWeight, rightHandWeight;

		Animator animator;


		void Start ()
		{
				animator = GetComponent<Animator> ();
		}

		void OnAnimatorIK (int layerIndex)
		{
				if (rightHandObj) {
						animator.SetIKPositionWeight (AvatarIKGoal.RightHand, rightHandWeight);
						animator.SetIKPosition (AvatarIKGoal.RightHand, rightHandObj.position);
				}
				if (leftHandObj) { 
						animator.SetIKPositionWeight (AvatarIKGoal.LeftHand, leftHandWeight);
						animator.SetIKPosition (AvatarIKGoal.LeftHand, leftHandObj.position);
				}
		}
}