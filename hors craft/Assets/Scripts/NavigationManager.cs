// DecompilerFi decompiler from Assembly-CSharp.dll class: NavigationManager
using com.ootii.Cameras;
using Common.Managers;
using Common.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavigationManager : Manager
{
	public bool _NavigationEnable;

	public GameObject navigationPointPrefab;

	public List<Vector3> navigationHistory = new List<Vector3>();

	public Transform playerPosition;

	public List<Transform> navigationTransforms = new List<Transform>();

	private const string PREF_KEY = "navigation.history";

	public override void Init()
	{
		if (_NavigationEnable)
		{
			LoadNavigationHistory();
		}
	}

	private void Update()
	{
		if (_NavigationEnable)
		{
			if (playerPosition == null)
			{
				try
				{
					playerPosition = CameraController.instance.MainCamera.transform;
				}
				catch
				{
				}
			}
			else
			{
				UpdatePrimitives();
			}
		}
	}

	public void Add(Vector3 position)
	{
		if (_NavigationEnable)
		{
			if (!navigationHistory.Contains(position))
			{
				navigationHistory.Add(position);
			}
			navigationHistory = navigationHistory.Distinct().ToList();
		}
	}

	private void UpdatePrimitives()
	{
		if (!_NavigationEnable)
		{
			return;
		}
		for (int i = 0; i < navigationHistory.Count; i++)
		{
			try
			{
				navigationTransforms[i].position = navigationHistory[i] + new Vector3(0f, 11.5f, 0f);
			}
			catch
			{
				GameObject gameObject = Object.Instantiate(navigationPointPrefab);
				gameObject.transform.position = navigationHistory[i] + new Vector3(0f, 11.5f, 0f);
				navigationTransforms.Add(gameObject.transform);
			}
			if (Vector3.Distance(playerPosition.position, navigationTransforms[i].position) > 64f)
			{
				navigationTransforms[i].gameObject.SetActive(value: true);
			}
			else
			{
				navigationTransforms[i].gameObject.SetActive(value: false);
			}
		}
	}

	private void SaveNavigationHistory()
	{
		if (_NavigationEnable)
		{
			navigationHistory = navigationHistory.Distinct().ToList();
			string value = JSONHelper.ToJson(navigationHistory.ToArray());
			WorldPlayerPrefs.get.SetString("navigation.history", value);
		}
	}

	private void LoadNavigationHistory()
	{
		if (_NavigationEnable)
		{
			string @string = WorldPlayerPrefs.get.GetString("navigation.history", string.Empty);
			try
			{
				Vector3[] array = JSONHelper.FromJson<Vector3>(@string);
				navigationHistory = new List<Vector3>();
				Vector3[] array2 = array;
				foreach (Vector3 item in array2)
				{
					navigationHistory.Add(item);
				}
				navigationHistory = navigationHistory.Distinct().ToList();
			}
			catch
			{
				UnityEngine.Debug.LogWarning("[NAVIGATION MANAGER]: Didnt find any positions history in prefs.");
			}
		}
	}

	private void OnApplicationPause(bool pause)
	{
		if (_NavigationEnable)
		{
			SaveNavigationHistory();
		}
	}

	private void OnApplicationQuit()
	{
		if (_NavigationEnable)
		{
			SaveNavigationHistory();
		}
	}
}
