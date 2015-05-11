/////////////////////////////////////////////////////////////////////////////////
//
//	LevelTransition.cs
//	© EternalVR, All Rights Reserved
//
//	description:	This class gives the static function LoadLevel to load a level
//					with built in fade to black and back functionality
//
//	authors:		Morgan Holbart
//
///////////////////////////////////////////////////////////////////////////////// 

using UnityEngine;
using System.Collections;

public class LevelTransition : MonoBehaviour
{
	
	public static LevelTransition instance {
		get {
			if (_instance == null) {
				_instance = GameObject.Find ("GameManager").AddComponent<LevelTransition> ();
			}
			return _instance;
		}
	}
	public delegate void LevelLoad ();
	public static event LevelLoad OnLevelLoad;
	public bool fading;
	
	private static LevelTransition _instance;
	private GameObject LeftFadeCube {
		get {
			if (_LeftFadeCube == null)
				CreateLeftFadeCube ();
			return _LeftFadeCube;
		}
	}
	private GameObject _LeftFadeCube;
	private GameObject RightFadeCube {
		get {
			if (_RightFadeCube == null)
				CreateRightFadeCube ();
			return _RightFadeCube;
		}
	}
	private GameObject _RightFadeCube;
	
	public Color clear = new Color (0, 0, 0, 0);
	public Color black = new Color (0, 0, 0, 1);
	
	/// <summary>
	/// Loads the level given
	/// </summary>
	public static void LoadLevel (string levelName)
	{
		if (!instance.fading)
			instance.StartCoroutine ("FadeLevel", levelName);
	}
	
	/// <summary>
	/// Creates the fade cube and attaches it to the camera
	/// </summary>
	private void CreateLeftFadeCube ()
	{
		GameObject anchor = GameObject.Find ("LeftEyeAnchor");
		GameObject cube = Instantiate (Resources.Load<GameObject> ("Misc/FadeCube")) as GameObject;
		cube.renderer.material.color = clear;
		cube.transform.parent = anchor.transform;
		cube.transform.localPosition = new Vector3 (0, 0, 0);
		_LeftFadeCube = cube;
	}
	
	/// <summary>
	/// Creates the fade cube and attaches it to the camera
	/// </summary>
	private void CreateRightFadeCube ()
	{
		GameObject anchor = GameObject.Find ("RightEyeAnchor");
		GameObject cube = Instantiate (Resources.Load<GameObject> ("Misc/FadeCube")) as GameObject;
		cube.renderer.material.color = clear;
		cube.transform.parent = anchor.transform;
		cube.transform.localPosition = new Vector3 (0, 0, 0);
		_RightFadeCube = cube;
	}
	
	
	/// <summary>
	/// Load the level with the fade in and out
	/// </summary>
	IEnumerator FadeLevel (string levelName)
	{
		fading = true;
		GameManager.instance.UpdateWorldPos ();
		while (LeftFadeCube.renderer.material.color.a < .99f) {
			LeftFadeCube.renderer.material.color = Color.Lerp (LeftFadeCube.renderer.material.color, black, .026f);
			RightFadeCube.renderer.material.color = Color.Lerp (LeftFadeCube.renderer.material.color, black, .026f);
			yield return null;
		}
		Application.LoadLevel (levelName);
		while (Application.loadedLevelName != levelName) {
			yield return null;
		}
		OnLevelLoad ();
		CreateLeftFadeCube ();
		LeftFadeCube.renderer.material.color = black;
		RightFadeCube.renderer.material.color = black;
		
		yield return new WaitForSeconds (.15f);
		while (LeftFadeCube.renderer.material.color.a > .01f) {
			LeftFadeCube.renderer.material.color = Color.Lerp (LeftFadeCube.renderer.material.color, clear, .026f);
			RightFadeCube.renderer.material.color = Color.Lerp (LeftFadeCube.renderer.material.color, clear, .026f);
			yield return null;
		}
		fading = false;
		LeftFadeCube.renderer.material.color = clear;
		RightFadeCube.renderer.material.color = clear;
		yield return null;
	}
}