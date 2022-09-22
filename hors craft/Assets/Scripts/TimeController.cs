// DecompilerFi decompiler from Assembly-CSharp.dll class: TimeController
using UnityEngine;

public class TimeController : MonoBehaviour
{
	private const float checkingFrequencyInSecs = 1f;

	private float gameplayTime;

	private float timeSinceFirstStartup;

	private float lastCheckTime;

	private void Start()
	{
		timeSinceFirstStartup = PlayerPrefs.GetInt("timeSinceStartup", 0);
		gameplayTime = PlayerPrefs.GetInt("scaledTimeStartup", 0);
		lastCheckTime = 0f;
	}

	private void Update()
	{
		if (Time.realtimeSinceStartup > lastCheckTime + 1f)
		{
			float num = Time.realtimeSinceStartup - lastCheckTime;
			lastCheckTime = Time.realtimeSinceStartup;
			timeSinceFirstStartup += num;
			PlayerPrefs.SetInt("timeSinceStartup", (int)timeSinceFirstStartup);
			PlayerPrefs.SetInt("scaledTimeStartup", (int)gameplayTime);
		}
		gameplayTime += Time.deltaTime;
		Shader.SetGlobalFloat("_UnscaledTime", Time.unscaledTime);
	}
}
