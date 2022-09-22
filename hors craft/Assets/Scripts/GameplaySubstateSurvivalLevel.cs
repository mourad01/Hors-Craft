// DecompilerFi decompiler from Assembly-CSharp.dll class: GameplaySubstateSurvivalLevel
using States;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameplaySubstateSurvivalLevel : MonoBehaviour, IGameplaySubstateAction
{
	public GameObject prefab;

	private void Awake()
	{
	}

	public Action<ModuleLoader, GameObject> GetAction()
	{
		return SpawnLevel;
	}

	public void SpawnLevel(ModuleLoader moduleLoader, GameObject other)
	{
		if (!(other.GetComponent<LevelModule>() == null))
		{
			while (other.transform.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(other.transform.GetChild(0).gameObject);
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, other.transform, worldPositionStays: false);
			LevelModule component = other.GetComponent<LevelModule>();
			component.expSlider = gameObject.GetComponentInChildren<Slider>();
			component.levelText = gameObject.GetComponentInChildren<Text>();
		}
	}
}
