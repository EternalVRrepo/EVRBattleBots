using UnityEngine;
using System.Collections;

public class TalentIcon : MonoBehaviour
{
	#region Variables
	[HideInInspector]
	public PlatformManager
		pm;
	public string displayName, description;
	public bool selected {
		get { 
			return selectedInstance; 
		} 
	}
	bool selectedInstance;

	public bool	meetsLevelRequirement;
	public int row;
	public int collum;
	public Talent talent;
	public Texture iconTexture;
	#endregion

	//============================================================================
	// Initilization
	//============================================================================
	public void Initilize (PartyUnit unit, Talent tal, int col, int ro)
	{
		talent = tal;
		GetTalentData ();

		row = ro;
		collum = col;
		meetsLevelRequirement = (unit.currentLevel >= row * 2);

		SetColor ();
	}
        
	void GetTalentData ()
	{
		gameObject.name = talent.TalentName;
		name = talent.TalentName;
		description = talent.TalentDescription;
		if (talent.TalentTexture) {
			iconTexture = talent.TalentTexture;
		}
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
	public void Select (bool b)
	{
		selectedInstance = b;
		if (b) {
			foreach (TalentIcon ti in pm.talentIcons) {
				if (ti.row == row) {
					if (ti != this) {
						ti.Select (false);
					}
				}
			}
		}
		UpdateCurrentTalents ();
		SetColor ();
	}
	void UpdateCurrentTalents ()
	{
		pm.UpdateTalentTree ();
		//add sound
	}

	void SetColor ()
	{
		if (renderer) {
			if (selected)
				renderer.material.color = Color.white;
			else 
				renderer.material.color = Color.gray;
			
			if (meetsLevelRequirement) {
				if (iconTexture)
					renderer.material.mainTexture = iconTexture;
			} else
				renderer.material.color = Color.Lerp (Color.black, Color.white, 0.05f);
		}
	}
	//============================================================================
}