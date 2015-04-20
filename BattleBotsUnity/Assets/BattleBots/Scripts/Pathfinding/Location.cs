/////////////////////////////////////////////////////////////////////////////////
//
//	Location.cs
//	© EternalVR, All Rights Reserved
//
//	description:	Class made to hold a hexagon location, needs its own class so it
//					can extend PriorityQueueNode, used in A*Pathfinding
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;
using Priority_Queue;

public class Location : PriorityQueueNode {
	public Hexagon hex;

	public Location(Hexagon h) {
		hex = h;
	}
}
