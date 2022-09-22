// DecompilerFi decompiler from Assembly-CSharp.dll class: CookingManager
using Common.Managers;
using Cooking;
using Gameplay;
using States;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CookingManager : Manager
{
	public GameObject[] workPlaces;

	public GameObject topBarPrefab;

	public GameObject timerPrefab;

	public string defaultSceneName = "Cooking";

	[HideInInspector]
	public int prestigeAcquired;

	private string currentKey = "modes.kitchen";

	private bool _isProgressDirty;

	private List<string> devicesKeys = new List<string>();

	private WorkController _workController;

	public string firstKitchenKey
	{
		get
		{
			return PlayerPrefs.GetString("first.kitchen", string.Empty);
		}
		set
		{
			PlayerPrefs.SetString("first.kitchen", value);
		}
	}

	public bool IsProgressDirty
	{
		get
		{
			bool isProgressDirty = _isProgressDirty;
			_isProgressDirty = false;
			return isProgressDirty;
		}
		set
		{
			_isProgressDirty = value;
			if (topBar != null)
			{
				topBar.Update();
			}
		}
	}

	public WorkController workController
	{
		get
		{
			if (_workController == null)
			{
				_workController = UnityEngine.Object.FindObjectOfType<WorkController>();
			}
			return _workController;
		}
	}

	public CookingTopBar topBar
	{
		get;
		private set;
	}

	public override void Init()
	{
		UpdateSettings();
	}

	public void UpdateSettings()
	{
		TitleState titleState = Manager.Get<StateMachineManager>().GetStateInstance(typeof(TitleState)) as TitleState;
		if (Manager.Get<ModelManager>().cookingSettings.IsCookingOnStart() && titleState != null)
		{
			titleState.customOnStartAction = delegate
			{
				UnityEngine.Object.FindObjectOfType<Camera>().GetComponent<AudioListener>().enabled = false;
				Manager.Get<StateMachineManager>().SetState<LoadLevelState>(new LoadLevelDefaultStartParameter
				{
					sceneToLoadName = defaultSceneName,
					stateType = typeof(CookingChooseLevelState)
				});
			};
		}
	}

	public void AssingKitchenKey(string key)
	{
		if (string.IsNullOrEmpty(firstKitchenKey))
		{
			firstKitchenKey = key;
		}
		if (firstKitchenKey == key)
		{
			currentKey = "modes.kitchen";
		}
	}

	public string GetKitchenKey()
	{
		return currentKey;
	}

	public Timer SpawnTimer(Vector3 spacePosition, float time, bool burnVersion = false)
	{
		GameObject original = Manager.Get<CookingManager>().timerPrefab;
		GameObject gameObject = UnityEngine.Object.Instantiate(original);
		Vector3 position = workController.mainCam.WorldToScreenPoint(spacePosition);
		workController.cookingGameplay.MoveToConnector(gameObject);
		gameObject.transform.position = position;
		gameObject.transform.localScale = Vector3.one;
		Timer component = gameObject.GetComponent<Timer>();
		if (burnVersion)
		{
			component.Init(time, Timer.Colors.ORANGE, Timer.Colors.RED);
		}
		else
		{
			component.Init(time, Timer.Colors.NONE, Timer.Colors.GREEN);
		}
		return component;
	}

	public void ShowTopBar()
	{
		if (topBar == null)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(topBarPrefab, Manager.Get<CanvasManager>().canvas.transform, worldPositionStays: false);
			gameObject.transform.localScale = Vector3.one;
			topBar = gameObject.GetComponent<CookingTopBar>();
		}
	}

	public void HideTopBar()
	{
		if (topBar != null)
		{
			UnityEngine.Object.Destroy(topBar.gameObject);
		}
	}

	public List<string> GetAllDevicesKeys()
	{
		if (devicesKeys == null || devicesKeys.Count == 0)
		{
			devicesKeys = new List<string>();
			Array.ForEach(workPlaces, delegate(GameObject w)
			{
				devicesKeys.AddRange(GetAllDevices(w));
			});
		}
		workPlaces = null;
		return devicesKeys;
	}

	private List<string> GetAllDevices(GameObject workPlace)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(workPlace);
		List<string> devices = new List<string>();
		WorkPlace component = gameObject.GetComponent<WorkPlace>();
		component.devices.ForEach(delegate(Device ds)
		{
			devices.Add(ds.Key);
		});
		UnityEngine.Object.Destroy(gameObject);
		return devices;
	}
}
