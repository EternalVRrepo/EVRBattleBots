/////////////////////////////////////////////////////////////////////////////////
//
//	Infection.cs
//	© EternalVR, All Rights Reserved
//
//	description:	Gives an object Infection which allows the player to interact with it
//					and try to stop the infection
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Infection : MonoBehaviour
{
	public bool collisionTriggersBattle = true;

	public List<EnemyUnitInfo> enemies = new List<EnemyUnitInfo> ();
	public int xpReward;
	public PartyUnit partyUnitReward;

	public Color infected;
	public Color infected2;
	public Color cleaned;

	public bool Interactable = true;

	void Start() {
		if (!PlayerPrefs.HasKey(GenerateKey())) {
			PlayerPrefs.SetInt("Infection0", 0);
			Interactable = true;
			PlayerPrefs.Save();
		}
		else {
			if (PlayerPrefs.GetInt(GenerateKey()) == 1) {
				Interactable = false;
				renderer.material.SetColor("_tin", cleaned);
			}
			else {
				Interactable = true;
				renderer.material.SetColor("_tin", infected);				
			}
		}
	}

	/// <summary>
	/// Interact with this instance, intiate combat
	/// </summary>
	public void Interact ()
	{
		if (!Interactable)
			return;

		List<EnemyUnitInfo> combatEnemies = new List<EnemyUnitInfo>();

		foreach (EnemyUnitInfo e in enemies) {
//			combatEnemies.Add(new EnemyUnitInfo(e));
			EnemyUnitInfo enemy = ScriptableObject.CreateInstance<EnemyUnitInfo>();
			enemy.Initialize(e);
			combatEnemies.Add(enemy);
		}

		GameManager.instance.StartCombat (combatEnemies, this);
	}

	public void Clean() {
		PlayerPrefs.SetInt(GenerateKey(), 1);
		PlayerPrefs.Save();
		Interactable = false;
		StartCoroutine("ChangeToClean");
	}

	void Update() {
		if (Interactable) {
			float lerp = Mathf.PingPong(Time.time, 2.5f) / 2.5f;
			renderer.material.SetColor("_tin", Color.Lerp(infected, infected2, lerp));
		}
	}

	IEnumerator ChangeToClean() {
		float t = 0;
		
		while (renderer.material.GetColor("_tin") != cleaned) {
			renderer.material.SetColor("_tin", Color.Lerp(infected, cleaned, t));
			t += Time.deltaTime/2;
			yield return null;
		}
	}

	public string GenerateKey() {
		string s = name;
		foreach (EnemyUnitInfo e in enemies) {
			s += e.Name[3];
			s += e.ListOfAbilities[0].DisplayName[3];
		}
		return s;
	}
}
