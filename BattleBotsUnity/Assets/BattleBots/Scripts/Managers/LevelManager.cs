using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
	AsyncOperation async;
	public GameObject loadingPopup	;
	public Slider loadingSlider;
	delegate void LoadingState ();
	LoadingState loadState;

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.L) || Input.GetButtonDown ("Start") || Input.GetMouseButtonDown (1)) {
			if (Application.loadedLevelName == "LoadB") 
				AsyncLoadLevel ("LoadA");
			else
				AsyncLoadLevel ("LoadB");
		}

		if (async != null) {
			if (async.isDone)
				loadingPopup.SetActive (false);
			else 
				loadingPopup.SetActive (true);

			loadingSlider.value = Mathf.Lerp (loadingSlider.value, async.progress, Time.deltaTime);
		} else {
			loadingPopup.SetActive (false);
		}
	}

	public void AsyncLoadLevel (string levelName)
	{
		StartCoroutine ("load", levelName);
	}
	
	IEnumerator load (string levelName)
	{
		Debug.LogWarning ("ASYNC LOAD STARTED - " +
			"DO NOT EXIT PLAY MODE UNTIL SCENE LOADS... UNITY WILL CRASH");
		async = Application.LoadLevelAsync (levelName);
//		async.allowSceneActivation = false;
		yield return async;
	}
	
	public void InitilizeLoad ()
	{
//		async.allowSceneActivation = true;
	}

}
