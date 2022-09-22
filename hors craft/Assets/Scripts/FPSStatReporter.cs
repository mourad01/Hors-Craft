// DecompilerFi decompiler from Assembly-CSharp.dll class: FPSStatReporter
using Common.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSStatReporter : StatReporter
{
	public class FPSStat : StatsManager.Stat
	{
		public string fps;

		public string game_mode_name;

		public string data;

		public FPSStat(float fpsValue, GameMode gameMode, string additionalData)
		{
			serversideId = "fpsAvg";
			fps = fpsValue.ToString("n2");
			game_mode_name = gameMode.ToString();
			data = additionalData;
		}

		public override string ToString()
		{
			return $"[FPSStat] Fps:{fps} Mode:{game_mode_name} Data:{data}";
		}
	}

	public const int REPORT_TIME = 10;

	private float time;

	private GameMode currentMode;

	private string additionalData;

	private Dictionary<GameMode, float> timeDeltaSums;

	private Dictionary<GameMode, int> counters;

	public FPSStatReporter()
	{
		timeDeltaSums = new Dictionary<GameMode, float>();
		counters = new Dictionary<GameMode, int>();
		IEnumerator enumerator = Enum.GetValues(typeof(GameMode)).GetEnumerator();
		try
		{
			while (enumerator.MoveNext())
			{
				GameMode key = (GameMode)enumerator.Current;
				timeDeltaSums.Add(key, 0f);
				counters.Add(key, 0);
			}
		}
		finally
		{
			IDisposable disposable;
			if ((disposable = (enumerator as IDisposable)) != null)
			{
				disposable.Dispose();
			}
		}
		time = 0f;
	}

	public void SetGameMode(GameMode mode)
	{
		currentMode = mode;
	}

	public void SetAdditionalData(string data)
	{
		additionalData = data;
	}

	public override bool UpdateAndCheck()
	{
		time += Time.unscaledDeltaTime;
		Dictionary<GameMode, float> dictionary;
		GameMode key;
		(dictionary = timeDeltaSums)[key = currentMode] = dictionary[key] + Time.unscaledDeltaTime;
		Dictionary<GameMode, int> dictionary2;
		GameMode key2;
		(dictionary2 = counters)[key2 = currentMode] = dictionary2[key2] + 1;
		return time > 10f;
	}

	public override StatsManager.Stat GetStat()
	{
		float num = timeDeltaSums[currentMode] / (float)counters[currentMode];
		FPSStat result = new FPSStat(1f / num, currentMode, additionalData);
		int num2 = 0;
		counters[currentMode] = num2;
		float value = num2;
		timeDeltaSums[currentMode] = value;
		time = value;
		return result;
	}
}
