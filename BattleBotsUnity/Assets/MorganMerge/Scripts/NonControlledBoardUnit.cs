﻿/////////////////////////////////////////////////////////////////////////////////
//
//	NonControlledBoardUnit.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This class extends BoardUnit and encompasses anything placed on
//					the board that isnt controlled by AI or the player, this means
//					things like traps, placed walls, etc
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 


using UnityEngine;
using System.Collections;

public class NonControlledBoardUnit : BoardUnit {

	public override void IssueMovement(Hexagon h) {

	}

	public override void Spawn(Hexagon h) {
		
	}
}
