using UnityEngine;
using System.Collections;

public class Hover : MonoBehaviour
{

	public void Enter ()
	{
		gameObject.renderer.material.color = Color.green;
	}

	public void Exit ()
	{
		gameObject.renderer.material.color = Color.red;
	}

	public void Trigger ()
	{
		gameObject.renderer.material.color = Color.yellow;
	}
}
