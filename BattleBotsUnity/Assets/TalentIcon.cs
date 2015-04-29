using UnityEngine;
using System.Collections;

public class TalentIcon : MonoBehaviour
{
	#region Variables
	public string name, discription;
	public bool active;
	public int row;
	public int collum;
	#endregion
	//============================================================================
	// Initilization
	//============================================================================
	public void Initilization (Talent t, int r, int c)
	{
		name = t.TalentName;
		discription = t.TalentDiscription;
	}

	void Start ()
	{
		
	}
	//============================================================================
	// Update
	//============================================================================
	void Update ()
	{
		
	}
	//============================================================================
	// Events
	//============================================================================
	//============================================================================
}
