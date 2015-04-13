/////////////////////////////////////////////////////////////////////////////////
//
//	AStarSearch.cs
//	© EternalVR, All Rights Reserved
//
//	description:	class holding the information/algorithm of the A* pathfinding
//					algorithm
//
//	sources:		http://www.redblobgames.com/pathfinding/a-star/implementation.html#csharp
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Priority_Queue;

public class AStarSearch {	

	public Dictionary<Hexagon, Hexagon> cameFrom = new Dictionary<Hexagon, Hexagon>();
	public Dictionary<Hexagon, int> costSoFar = new Dictionary<Hexagon, int>();

	/// <summary>
	/// Heuristic value moving from hex a to hex b, should add any other sort of "difficult" terrain here
	/// </summary>
	static public int Heuristic(Hexagon a, Hexagon b) {

		return Mathf.Abs (a.HexColumn-b.HexColumn) + Mathf.Abs (a.HexRow-b.HexRow);

	}

	/// <summary>
	/// Cost of moving from hex start to hex end, difficult terrain/unsafe conditions and such should be put here
	/// </summary>
	public int Cost(Hexagon start, Hexagon end) {
		return 1;
	}

	/// <summary>
	/// Creates a new A* Search
	/// </summary>
	public AStarSearch(Hexagon[] map, Hexagon start, Hexagon end) {
		HeapPriorityQueue<Location> frontier = new HeapPriorityQueue<Location>(10); //TODO could cause issues if pathfinding more than 10 units

		frontier.Enqueue(new Location(start), 0);

		cameFrom.Add (start, start);
		costSoFar.Add(start, 0);

		while (frontier.Count > 0) {
			Location curr = frontier.Dequeue ();

			if (curr.hex == end) {
				break;
			}

			foreach (Hexagon h in BoardManager.instance.GetNeighborsMovement (curr.hex)) {
				int newCost = costSoFar[curr.hex] + Cost(curr.hex, h);
				if (!costSoFar.ContainsKey (h) || newCost < costSoFar[h]) {
					costSoFar.Add(h, newCost);
					int priority = newCost + Heuristic(h, end);
					frontier.Enqueue(new Location(h), priority);
					cameFrom.Add(h, curr.hex);
				}
			}
		}
	}

}


