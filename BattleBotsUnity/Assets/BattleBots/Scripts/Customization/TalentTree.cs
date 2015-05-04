/////////////////////////////////////////////////////////////////////////////////
//
//	TalentTree.cs
//	© EternalVR, All Rights Reserved
//
//	description:	A scriptable object holding the information/list of talents
//					for a talent tree
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class TalentTree : ScriptableObject {

	public List<Talent> Tree = new List<Talent>(); 

	[SerializeField]
	protected List<Talent> TalentsChosen = new List<Talent>(); //might want to move this to the party unit itselfA

	public Talent GetTalentAt(int row) {

		while (TalentsChosen.Count <= row) 
			TalentsChosen.Add (Tree[row*3]);

		return TalentsChosen[row];
	}

	public void UpdateTalents(List<Talent> newTalents) {
		TalentsChosen.Clear ();
		foreach (Talent t in newTalents) {
			TalentsChosen.Add (t);
		}
	}
}
