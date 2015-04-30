using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UnitNamePlateGUI : MonoBehaviour
{
	#region Variables
	public Text NameText;
	public Text LevelText;

	[HideInInspector]
	public string
		name;
	[HideInInspector]
	public int
		level;
	#endregion
	//============================================================================
	// Initilization
	//============================================================================
	public void Initilize (PartyUnit unit)
	{
		NameText.text = unit.Name;
		LevelText.text = unit.currentLevel.ToString ();

		name = unit.Name;
		level = unit.currentLevel;
	}
	//============================================================================
	// Update
	//============================================================================
	//============================================================================
	// Events
	//============================================================================
	//============================================================================
}
