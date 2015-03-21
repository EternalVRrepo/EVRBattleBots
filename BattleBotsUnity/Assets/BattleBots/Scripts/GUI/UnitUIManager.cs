using UnityEngine;
using System.Collections;

public class UnitUIManager : MonoBehaviour
{
		public GameObject groundedUI;

		void Start ()
		{

		}
	
		void Update ()
		{
				GroundedUI ();
		}

		void 	GroundedUI ()
		{
				RaycastHit hit;
				if (Physics.Raycast (transform.position, -Vector3.up, out hit, LayerMask.NameToLayer ("Ground")))
						groundedUI.transform.position = hit.point;
		}
}
