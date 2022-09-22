// DecompilerFi decompiler from Assembly-CSharp.dll class: ExclamationMarkController
using Common.Utils;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class ExclamationMarkController
{
	public delegate GameObject CreateMarkPlacement();

	public class ExclamationMarkData
	{
		public string key;
	}

	private const string KEY = "exclamation.mark.data";

	public static Dictionary<ExclamationMarkData, GameObject> currentMarks = new Dictionary<ExclamationMarkData, GameObject>();

	private static List<ExclamationMarkData> _exclamationData;

	private static List<ExclamationMarkData> exclamationData
	{
		get
		{
			if (_exclamationData == null)
			{
				if (PlayerPrefs.HasKey("exclamation.mark.data"))
				{
					_exclamationData = JSONHelper.Deserialize<List<ExclamationMarkData>>(PlayerPrefs.GetString("exclamation.mark.data"));
				}
				else
				{
					_exclamationData = new List<ExclamationMarkData>();
				}
			}
			return _exclamationData;
		}
		set
		{
			string value2 = JSONHelper.ToJSON(value);
			PlayerPrefs.SetString("exclamation.mark.data", value2);
		}
	}

	private static GameObject GetDefaultExclamationMark()
	{
		return Resources.Load<GameObject>("prefabs/exclamation_mark");
	}

	public static bool ExclamationMarkShown(string key)
	{
		return exclamationData.Any((ExclamationMarkData d) => d.key == key);
	}

	public static GameObject ShowExclamationMark(string key)
	{
		return ShowExclamationMark(key, () => null);
	}

	public static GameObject ShowExclamationMark(string key, CreateMarkPlacement markPlacementMethod)
	{
		List<ExclamationMarkData> exclamationData = ExclamationMarkController.exclamationData;
		if (!exclamationData.Any((ExclamationMarkData d) => d.key == key))
		{
			if (markPlacementMethod != null)
			{
				GameObject gameObject = markPlacementMethod();
				AddMarkToData(gameObject, key);
				return gameObject;
			}
			AddMarkToData(null, key);
			return null;
		}
		return null;
	}

	public static GameObject ShowExclamationMark(string key, GameObject parent)
	{
		return ShowExclamationMark(key, CreateDefaultMarkPlacement(parent, GetDefaultExclamationMark()));
	}

	private static void AddMarkToData(GameObject mark, string key)
	{
		List<ExclamationMarkData> exclamationData = ExclamationMarkController.exclamationData;
		ExclamationMarkData exclamationMarkData = new ExclamationMarkData();
		exclamationMarkData.key = key;
		ExclamationMarkData exclamationMarkData2 = exclamationMarkData;
		exclamationData.Add(exclamationMarkData2);
		if (mark != null)
		{
			currentMarks.Add(exclamationMarkData2, mark);
		}
		ExclamationMarkController.exclamationData = exclamationData;
	}

	public static void ClearMarksWithKey(string key)
	{
		List<ExclamationMarkData> list = new List<ExclamationMarkData>();
		foreach (ExclamationMarkData key2 in currentMarks.Keys)
		{
			if (key2.key == key)
			{
				list.Add(key2);
				UnityEngine.Object.Destroy(currentMarks[key2]);
			}
		}
		list.ForEach(delegate(ExclamationMarkData m)
		{
			currentMarks.Remove(m);
		});
	}

	public static CreateMarkPlacement CreateDefaultMarkPlacement(GameObject parentGO, GameObject prefab = null)
	{
		if (prefab == null)
		{
			prefab = GetDefaultExclamationMark();
		}
		return delegate
		{
			Vector2 sizeDelta = (prefab.transform as RectTransform).sizeDelta;
			GameObject gameObject = Object.Instantiate(prefab, parentGO.transform, worldPositionStays: false);
			gameObject.transform.localScale = Vector3.one;
			gameObject.transform.rotation = Quaternion.identity;
			RectTransform rectTransform = gameObject.transform as RectTransform;
			Vector2 vector = new Vector2(0.8f, 1f);
			rectTransform.pivot = new Vector2(0.5f, 0.5f);
			rectTransform.anchorMin = vector;
			rectTransform.anchorMax = vector;
			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;
			rectTransform.sizeDelta = sizeDelta;
			return gameObject;
		};
	}
}
