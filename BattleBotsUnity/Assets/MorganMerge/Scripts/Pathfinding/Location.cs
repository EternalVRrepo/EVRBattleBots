using UnityEngine;
using System.Collections;
using Priority_Queue;

public class Location : PriorityQueueNode {
	public Hexagon hex;

	public Location(Hexagon h) {
		hex = h;
	}
}
