// DecompilerFi decompiler from Assembly-CSharp.dll class: CutScene
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class CutScene
{
	public bool freezePlayer;

	public string sceneName = "test cut scene";

	public List<CutSceneAct> acts = new List<CutSceneAct>();

	public int currentAct;

	public int timesAllowedToPlay = -1;

	public bool introScene;

	public bool outroScene;

	public int introOutroForWorld = -1;

	public int PartsInActCount
	{
		get
		{
			if (acts == null || currentAct < 0 || acts.Count <= currentAct)
			{
				return 0;
			}
			return acts[currentAct].PartsCount;
		}
	}

	public bool CanPlayNormalScene(int sceneIndex)
	{
		string key = $"scene.{sceneIndex}.PlayedTime";
		if (timesAllowedToPlay == -1)
		{
			return true;
		}
		int @int = PlayerPrefs.GetInt(key, 0);
		if (@int >= timesAllowedToPlay)
		{
			return false;
		}
		return true;
	}

	public bool CanPlayIntro(int currentWorld, int sceneIndex, bool outro = false)
	{
		if (currentWorld != introOutroForWorld)
		{
			return false;
		}
		if (!outro && !introScene)
		{
			return false;
		}
		if (outro && !outroScene)
		{
			return false;
		}
		string key = $"scene.{sceneIndex}.PlayedTime";
		if (timesAllowedToPlay == -1)
		{
			return true;
		}
		int @int = PlayerPrefs.GetInt(key, 0);
		if (@int >= timesAllowedToPlay)
		{
			return false;
		}
		return true;
	}

	public void RegisterPlayed(int sceneIndex)
	{
		string key = $"scene.{sceneIndex}.PlayedTime";
		int @int = PlayerPrefs.GetInt(key, 0);
		@int++;
		PlayerPrefs.SetInt(key, @int);
		PlayerPrefs.Save();
	}

	public float GetCurrentActTime()
	{
		return acts[currentAct].ActTime;
	}

	internal void Init()
	{
		foreach (CutSceneAct act in acts)
		{
			act.Init();
		}
	}

	public bool StartScene()
	{
		currentAct = 0;
		return StartAct(currentAct);
	}

	public bool StartNextAct()
	{
		CleanUp(currentAct);
		currentAct++;
		UnityEngine.Debug.Log("Starting act " + currentAct);
		return StartAct(currentAct);
	}

	public bool StartAct(int actNr)
	{
		if (acts == null || acts.Count <= actNr || acts[actNr] == null)
		{
			UnityEngine.Debug.LogError("Cannot start act!");
			return false;
		}
		acts[actNr].Run();
		return true;
	}

	public void CleanUp(int actNumber)
	{
		if (acts == null || acts.Count <= actNumber || acts[actNumber] == null)
		{
			UnityEngine.Debug.LogError("Cannot clean up act!");
		}
		else
		{
			acts[actNumber].CleanUp();
		}
	}
}
