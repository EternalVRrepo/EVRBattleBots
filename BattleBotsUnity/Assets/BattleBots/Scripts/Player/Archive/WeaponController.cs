//using UnityEngine;
//using System.Collections;
//
//public class WeaponController : MonoBehaviour
//{
//
//		Color catchColor;
//
//		void Update ()
//		{
//				for (int i=0; i < Merge.MSDK.ControllerCount; i++) {
//						transform.localRotation = Quaternion.Lerp (transform.localRotation, Merge.MSDK.Controllers [i].FusedSensorOrientation, Time.deltaTime * 20);
//				}
//		}
//
//		void OnCollisionEnter (Collision collision)
//		{
//				if (collision.gameObject.renderer.material.color != Color.gray)
//						catchColor = collision.gameObject.renderer.material.color;
//				collision.gameObject.renderer.material.color = Color.red;
//		}
//
//		void OnCollisionExit (Collision collision)
//		{
//				collision.gameObject.renderer.material.color = Color.gray;
//				StartCoroutine (ResetColor (collision.gameObject));
//		}
//
//		IEnumerator ResetColor (GameObject gameObject)
//		{
//				yield return new WaitForSeconds (10f);
//				gameObject.renderer.material.color = catchColor;
//				Debug.Log ("Color Reset");
//		}
//}