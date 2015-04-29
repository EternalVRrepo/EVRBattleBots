using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class StatsWindow : MonoBehaviour
{
	#region Variables
	public float healthValue;
	public float powerValue;
	public float speedValue;

	[SerializeField]
	Slider
		healthSlider, powerSlider, speedSlider;

	[HideInInspector]
	public int
		maxHealth, maxPower, maxSpeed;
	#endregion

	//============================================================================
	// Initilization
	//============================================================================

	public void Initilize (PartyUnit unit)
	{
		//Set Up Maxes baseValue * maxLevel
		//Set Up Current Value = baseValue * currentLevel
		//Set Values for Sliders
	}

	//============================================================================
	// Update
	//============================================================================

	void Update ()
	{
		//Update Slider's position
	}

	//============================================================================
	// Events
	//============================================================================
	//replaces running in update
	//public void UpdateSliders ()
	//{

	//}

	//============================================================================
}