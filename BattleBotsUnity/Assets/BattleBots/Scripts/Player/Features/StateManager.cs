using UnityEngine;
using System.Collections;

public class StateManager : MonoBehaviour
{
	public delegate void StateHandler ();
	public StateHandler playerState;

	void Update ()
	{
		if (playerState != null) {
			playerState ();	
		}
	}

	
	void Attack ()
	{
		
	}	

}
