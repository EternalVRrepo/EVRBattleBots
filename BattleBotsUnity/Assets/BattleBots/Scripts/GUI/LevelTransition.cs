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

public class LevelTransition : MonoBehaviour {

	public static LevelTransition instance {
		get {
			if (_instance == null) {
				_instance = GameObject.Find ("GameManager").AddComponent<LevelTransition>();
			}
			return _instance;
		}
	}
	public delegate void LevelLoad();
	public static event LevelLoad OnLevelLoad;
	public bool fading;

	private static LevelTransition _instance;
	private GameObject FadeCube {
		get {
			if (_FadeCube == null)
				CreateFadeCube();
			return _FadeCube;
		}
	}
	private GameObject _FadeCube;
	public Color clear = new Color(0,0,0,0);
	public Color black = new Color(0,0,0,1);

	/// <summary>
	/// Loads the level given
	/// </summary>
	public static void LoadLevel(string levelName) {
		if (!instance.fading)
			instance.StartCoroutine("FadeLevel", levelName);
	}

	/// <summary>
	/// Creates the fade cube and attaches it to the camera
	/// </summary>
	private void CreateFadeCube() {
		GameObject anchor = GameObject.Find ("LeftEyeAnchor");
		GameObject cube = Instantiate(Resources.Load<GameObject>("Misc/FadeCube")) as GameObject;
		cube.renderer.material.color = clear;
		cube.transform.parent = anchor.transform;
		cube.transform.localPosition = new Vector3(0,0,0);
		_FadeCube = cube;
	}

	/// <summary>
	/// Load the level with the fade in and out
	/// </summary>
	IEnumerator FadeLevel(string levelName) {
		fading = true;
		while (FadeCube.renderer.material.color.a < .99f) {
			FadeCube.renderer.material.color = Color.Lerp(FadeCube.renderer.material.color, black, .02f);
			yield return null;
		}
		Application.LoadLevel(levelName);
		while (Application.loadedLevelName != levelName) {
			yield return null;
		}
		OnLevelLoad();
		CreateFadeCube();
		FadeCube.renderer.material.color = black;
		yield return new WaitForSeconds(.15f);
		while (FadeCube.renderer.material.color.a > .01f) {
			FadeCube.renderer.material.color = Color.Lerp(FadeCube.renderer.material.color, clear, .02f);
			yield return null;
		}
		fading = false;
		FadeCube.renderer.material.color = clear;
		yield return null;
	}
}
