using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FPSDisplay : MonoBehaviour
{
	public Text FPS;
	float deltaTime = 0.0f;
		
	void Update ()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}
		
	void OnGUI ()
	{
		int w = Screen.width, h = Screen.height;
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		FPS.text = string.Format ("{0:0.0} ms ({1:0.} fps)", msec, fps);
	}
}