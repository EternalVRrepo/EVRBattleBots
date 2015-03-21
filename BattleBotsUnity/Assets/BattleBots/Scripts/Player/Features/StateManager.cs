using UnityEngine;
using System.Collections;

public class StateManager : MonoBehaviour
{
	public delegate void StateHandler ();
	public StateHandler playerState;
//	HeroStats heroStats;
//	MyHeroController heroController;

	void Start ()
	{
//		heroController = GetComponent<MyHeroController> ();
//		heroStats = GetComponent<HeroStats> ();
//		playerSkill = this.GetComponent<PlayerSkill> ();
	}
	
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
