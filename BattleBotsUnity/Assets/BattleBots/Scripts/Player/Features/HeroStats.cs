using UnityEngine;
using System.Collections;

public class HeroStats : MonoBehaviour
{
		public string playerName;

		public class Attribute
		{
				public int level = 1, health = 100, mana = 50, attack = 1, defense = 1;
				public float criticalRate = 5, attackSpeed = 1, attackRange = 15, movementSpeed = 1, experience;
		}

		public static Attribute playerStats = new Attribute ();

		void Start ()
		{
		}
	
		void Update ()
		{
		
		}
}
