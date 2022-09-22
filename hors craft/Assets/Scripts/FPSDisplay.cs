// DecompilerFi decompiler from Assembly-CSharp.dll class: FPSDisplay
using Common.Utils;
using System;
using System.Text;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
	private float deltaTime;

	private float deltaTime2;

	private int counter;

	private Text text;

	private string avgFps = string.Empty;

	private string lastMemory = string.Empty;

	private float lastMemoryCheck;

	private void Start()
	{
		text = GetComponent<Text>();
	}

	private void Update()
	{
		if (PlayerPrefs.GetInt("debugFpsCounter", 0) == 2)
		{
			this.text.text = MonoBehaviourSingleton<FrameLatencyStats>.get.GetLogs();
			return;
		}
		if (PlayerPrefs.GetInt("debugFpsCounter", 0) == 3)
		{
			if (string.IsNullOrEmpty(lastMemory) || Time.time - lastMemoryCheck > 1.5f)
			{
				lastMemoryCheck = Time.time;
				lastMemory = GetMemoryShort();
			}
			this.text.text = lastMemory;
			return;
		}
		if (PlayerPrefs.GetInt("debugFpsCounter", 0) == 4)
		{
			if (string.IsNullOrEmpty(lastMemory) || Time.time - lastMemoryCheck > 1.5f)
			{
				lastMemoryCheck = Time.time;
				lastMemory = GetMemoryLong();
			}
			this.text.text = lastMemory;
			return;
		}
		if (PlayerPrefs.GetInt("debugFpsCounter", 0) == 5)
		{
			this.text.text = MonoBehaviourSingleton<FrameLatencyStats>.get.GetLogs();
			return;
		}
		deltaTime += Time.deltaTime;
		counter++;
		if (deltaTime >= 5f)
		{
			deltaTime -= 5f;
			avgFps = $"{(float)counter / 5f:0.00}";
			counter = 0;
		}
		deltaTime2 += (Time.deltaTime - deltaTime2) * 0.1f;
		float num = deltaTime2 * 1000f;
		float num2 = 1f / deltaTime2;
		string text = $"{num:0.0} ms ({num2:0.} fps){Environment.NewLine}avg fps: {avgFps}";
		if (Time.timeScale > 0f)
		{
			this.text.text = text;
		}
		else
		{
			this.text.text = string.Empty;
		}
	}

	private string GetMemoryShort()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("GetMonoHeapSize: ");
		stringBuilder.AppendLine(RuntimeMemoryProfiler.GetFormatedSize(Profiler.GetMonoHeapSizeLong()));
		stringBuilder.Append("GetTotalAllocatedMemory: ");
		stringBuilder.AppendLine(RuntimeMemoryProfiler.GetFormatedSize(Profiler.GetTotalAllocatedMemoryLong()));
		stringBuilder.Append("GetTotalReservedMemory: ");
		stringBuilder.AppendLine(RuntimeMemoryProfiler.GetFormatedSize(Profiler.GetTotalReservedMemoryLong()));
		return stringBuilder.ToString();
	}

	private string GetMemoryLong()
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("GetMonoHeapSize: ");
		stringBuilder.AppendLine(RuntimeMemoryProfiler.GetFormatedSize(Profiler.GetMonoHeapSizeLong()));
		stringBuilder.Append("GetMonoUsedSize: ");
		stringBuilder.AppendLine(RuntimeMemoryProfiler.GetFormatedSize(Profiler.GetMonoUsedSizeLong()));
		stringBuilder.Append("GetTotalAllocatedMemory: ");
		stringBuilder.AppendLine(RuntimeMemoryProfiler.GetFormatedSize(Profiler.GetTotalAllocatedMemoryLong()));
		stringBuilder.Append("GetTotalReservedMemory: ");
		stringBuilder.AppendLine(RuntimeMemoryProfiler.GetFormatedSize(Profiler.GetTotalReservedMemoryLong()));
		stringBuilder.AppendLine(RuntimeMemoryProfiler.GetFullLogObject(typeof(Mesh)));
		stringBuilder.AppendLine(RuntimeMemoryProfiler.GetFullLogObject(typeof(Texture)));
		stringBuilder.AppendLine(RuntimeMemoryProfiler.GetFullLogObject(typeof(Texture2D)));
		stringBuilder.AppendLine(RuntimeMemoryProfiler.GetFullLogObject(typeof(AudioClip)));
		return stringBuilder.ToString();
	}
}
