// DecompilerFi decompiler from Assembly-CSharp.dll class: FPSMeasurer
using Common.Managers;
using States;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class FPSMeasurer : MonoBehaviour
{
	public int measurementsDestCount = 30;

	public float measureLength = 1f;

	private int fpsCount;

	private float fpsSum;

	private float lastMeasureTime;

	private List<float> measurements;

	private void Start()
	{
		Reset();
		measurements = new List<float>();
	}

	private void Reset()
	{
		fpsSum = 0f;
		fpsCount = 0;
		lastMeasureTime = Time.realtimeSinceStartup;
	}

	private void Update()
	{
		if (Manager.Get<StateMachineManager>().IsCurrentStateA<GameplayState>())
		{
			fpsCount++;
			fpsSum += Time.unscaledDeltaTime;
			if (Time.realtimeSinceStartup > lastMeasureTime + measureLength)
			{
				AddToReport();
				Reset();
			}
			if (measurements.Count > measurementsDestCount)
			{
				SendReport();
			}
		}
	}

	private void AddToReport()
	{
		measurements.Add(fpsSum / (float)Mathf.Max(1, fpsCount));
	}

	private void SendReport()
	{
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < measurements.Count; i++)
		{
			float value = measurements[i];
			stringBuilder.Append(value);
			if (i != measurements.Count - 1)
			{
				stringBuilder.Append(',');
			}
		}
		UnityEngine.Debug.LogError("FPS MEASUREMENT: " + stringBuilder.ToString());
		UnityEngine.Object.Destroy(this);
	}
}
