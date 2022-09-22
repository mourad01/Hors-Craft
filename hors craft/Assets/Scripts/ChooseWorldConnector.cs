// DecompilerFi decompiler from Assembly-CSharp.dll class: ChooseWorldConnector
using Common.Managers;
using Common.Managers.States.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChooseWorldConnector : UIConnector
{
	public WorldSelectElement worldPrefabElement;

	public GameObject elementsParent;

	public Button returnButton;

	public Button renameButton;

	public Button deleteButton;

	public Button playButton;

	public GameObject fakeLoading;

	private Action onReturnPressed;

	private Action<string> onWorldSelected;

	private Action<string> onWorldPlay;

	private Action<string> onWorldDelete;

	private Action<string> onWorldRename;

	private Action onWorldAdd;

	private List<GameObject> elements = new List<GameObject>();

	public virtual void Init(Action onReturnPressed, Action<string> onWorldSelected, Action onWorldAdd, Action<string> onWorldPlay, Action<string> onWorldDelete, Action<string> onWorldRename)
	{
		this.onReturnPressed = onReturnPressed;
		this.onWorldSelected = onWorldSelected;
		this.onWorldAdd = onWorldAdd;
		this.onWorldPlay = onWorldPlay;
		this.onWorldDelete = onWorldDelete;
		this.onWorldRename = onWorldRename;
		fakeLoading.SetActive(value: false);
		InitButtons();
		InitializeWorlds();
	}

	public void CheckList()
	{
		foreach (GameObject element in elements)
		{
			UnityEngine.Object.Destroy(element);
		}
		InitializeWorlds();
	}

	protected virtual void InitializeWorlds()
	{
		List<WorldData> allWorlds = Manager.Get<SavedWorldManager>().GetAllWorlds();
		CreateElements(allWorlds);
		deleteButton.gameObject.SetActive(allWorlds.Count != 1);
	}

	public void DestroyElement(string worldDataId)
	{
		int index = elements.FindIndex((GameObject obj) => obj.GetComponent<WorldSelectElement>().GetData().uniqueId == worldDataId);
		UnityEngine.Object.Destroy(elements[index]);
		elements.RemoveAt(index);
	}

	private void InitButtons()
	{
		returnButton.onClick.AddListener(delegate
		{
			if (onReturnPressed != null)
			{
				onReturnPressed();
			}
		});
		renameButton.onClick.AddListener(delegate
		{
			if (onReturnPressed != null)
			{
				onWorldRename(GetSelectedId());
			}
		});
		deleteButton.onClick.AddListener(delegate
		{
			if (onReturnPressed != null)
			{
				onWorldDelete(GetSelectedId());
			}
		});
		playButton.onClick.AddListener(delegate
		{
			if (onReturnPressed != null)
			{
				onWorldPlay(GetSelectedId());
			}
		});
	}

	private void DeselectAll()
	{
		foreach (GameObject element in elements)
		{
			element.GetComponent<WorldSelectElement>().Select(highlight: false);
		}
	}

	private void CreateElements(List<WorldData> worlds)
	{
		elements = new List<GameObject>();
		foreach (WorldData world in worlds)
		{
			CreateElement(world);
		}
		CreateElement(null);
	}

	public virtual string GetSelectedId()
	{
		WorldData selectedData = GetSelectedData();
		if (selectedData != null)
		{
			return selectedData.uniqueId;
		}
		return string.Empty;
	}

	public WorldSelectElement GetSelectedElement()
	{
		foreach (GameObject element in elements)
		{
			WorldData data = element.GetComponent<WorldSelectElement>().GetData();
			if (data != null && data.selected)
			{
				return element.GetComponent<WorldSelectElement>();
			}
		}
		return null;
	}

	public WorldData GetSelectedData()
	{
		WorldSelectElement selectedElement = GetSelectedElement();
		if (selectedElement != null)
		{
			return selectedElement.GetData();
		}
		return null;
	}

	private GameObject CreateElement(WorldData data)
	{
		WorldSelectElement element = UnityEngine.Object.Instantiate(worldPrefabElement.gameObject).GetComponent<WorldSelectElement>();
		element.Init(data, onWorldAdd, delegate(string id)
		{
			DeselectAll();
			onWorldSelected(id);
			element.Select(highlight: true);
		});
		element.gameObject.transform.SetParent(elementsParent.transform);
		element.transform.localScale = Vector3.one;
		elements.Add(element.gameObject);
		return element.gameObject;
	}
}
