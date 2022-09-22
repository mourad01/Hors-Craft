// DecompilerFi decompiler from Assembly-CSharp.dll class: BookmarksPanelController
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BookmarksPanelController : MonoBehaviour
{
	[Serializable]
	public class BookmarkConfig
	{
		public string translationKey;

		public string defaultText;

		public Action onChoose;
	}

	public Transform inactiveBookmarksParent;

	public Transform activeBookmarksParent;

	public GameObject inactiveBookmarkPrefab;

	public GameObject activeBookmarkPrefab;

	public GameObject separatorPrefab;

	public Action<int> onChoose;

	private List<BookmarkConfig> bookmarkConfig = new List<BookmarkConfig>();

	private List<GameObject> inactiveBookmarksInstances = new List<GameObject>();

	private List<GameObject> activeBookmarksInstances = new List<GameObject>();

	public void Rebuild(List<BookmarkConfig> bookmarkConfig)
	{
		Clear();
		this.bookmarkConfig = bookmarkConfig;
		if (bookmarkConfig.Count > 1)
		{
			for (int i = 0; i < bookmarkConfig.Count; i++)
			{
				BookmarkConfig config = bookmarkConfig[i];
				SpawnBookmark(config, i, i == bookmarkConfig.Count - 1);
			}
		}
	}

	public void Choose(int i)
	{
		OnChoose(i);
	}

	private void Clear()
	{
		foreach (GameObject inactiveBookmarksInstance in inactiveBookmarksInstances)
		{
			UnityEngine.Object.Destroy(inactiveBookmarksInstance);
		}
		foreach (GameObject activeBookmarksInstance in activeBookmarksInstances)
		{
			UnityEngine.Object.Destroy(activeBookmarksInstance);
		}
		inactiveBookmarksInstances = new List<GameObject>();
		activeBookmarksInstances = new List<GameObject>();
		bookmarkConfig = new List<BookmarkConfig>();
	}

	private void SpawnBookmark(BookmarkConfig config, int i, bool isLast)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate(inactiveBookmarkPrefab, inactiveBookmarksParent, worldPositionStays: false);
		gameObject.transform.localScale = Vector3.one;
		inactiveBookmarksInstances.Add(gameObject);
		SetButton(gameObject, i);
		SetText(gameObject, config);
		if (!isLast)
		{
			SpawnSeparator();
		}
		GameObject gameObject2 = UnityEngine.Object.Instantiate(activeBookmarkPrefab, activeBookmarksParent, worldPositionStays: false);
		gameObject2.transform.localScale = Vector3.one;
		activeBookmarksInstances.Add(gameObject2);
		SetText(gameObject2, config);
		SetActiveInChildren(gameObject2, enable: false);
	}

	private void SetButton(GameObject bookmark, int i)
	{
		Button componentInChildren = bookmark.GetComponentInChildren<Button>();
		componentInChildren.onClick.RemoveAllListeners();
		componentInChildren.onClick.AddListener(delegate
		{
			OnChoose(i);
		});
	}

	private void SetText(GameObject bookmark, BookmarkConfig config)
	{
		TranslateText componentInChildren = bookmark.GetComponentInChildren<TranslateText>();
		if (componentInChildren != null)
		{
			componentInChildren.translationKey = config.translationKey;
			componentInChildren.defaultText = config.defaultText;
			componentInChildren.ForceRefresh();
		}
		else
		{
			Text componentInChildren2 = bookmark.GetComponentInChildren<Text>();
			componentInChildren2.text = config.defaultText;
		}
	}

	private void SpawnSeparator()
	{
		if (!(separatorPrefab == null))
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(separatorPrefab, inactiveBookmarksParent, worldPositionStays: false);
			gameObject.transform.localScale = Vector3.one;
		}
	}

	private void OnChoose(int i)
	{
		activeBookmarksInstances.ForEach(delegate(GameObject b)
		{
			SetActiveInChildren(b, enable: false);
		});
		SetActiveInChildren(activeBookmarksInstances[i], enable: true);
		if (bookmarkConfig[i].onChoose != null)
		{
			bookmarkConfig[i].onChoose();
		}
	}

	private void SetActiveInChildren(GameObject go, bool enable)
	{
		Transform[] componentsInChildren = go.GetComponentsInChildren<Transform>(includeInactive: true);
		Transform[] array = componentsInChildren;
		foreach (Transform transform in array)
		{
			if (transform != go.transform)
			{
				transform.gameObject.SetActive(enable);
			}
		}
	}
}
