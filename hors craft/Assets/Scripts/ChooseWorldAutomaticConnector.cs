// DecompilerFi decompiler from Assembly-CSharp.dll class: ChooseWorldAutomaticConnector
using Common.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;

public class ChooseWorldAutomaticConnector : ChooseWorldConnector
{
	private List<WorldData> worldsSorted = new List<WorldData>();

	public override void Init(Action onReturnPressed, Action<string> onWorldSelected, Action onWorldAdd, Action<string> onWorldPlay, Action<string> onWorldDelete, Action<string> onWorldRename)
	{
		fakeLoading.SetActive(value: false);
		InitializeWorlds();
		onWorldPlay(GetSelectedId());
	}

	protected override void InitializeWorlds()
	{
		worldsSorted = Manager.Get<SavedWorldManager>().GetAllWorlds();
		worldsSorted.Sort((WorldData x, WorldData y) => x.uniqueId.CompareTo(y.uniqueId));
		UnityEngine.Debug.Log(worldsSorted);
	}

	public override string GetSelectedId()
	{
		WorldData currentWorld = Manager.Get<SavedWorldManager>().GetCurrentWorld();
		int num = -1;
		for (int i = 0; i < worldsSorted.Count; i++)
		{
			if (string.Equals(worldsSorted[i].uniqueId, currentWorld.uniqueId))
			{
				num = i;
				break;
			}
		}
		if (num + 1 >= worldsSorted.Count || num == -1)
		{
			UnityEngine.Debug.LogError("Cannot load level! Not enough levels!");
			return (worldsSorted.Count < 1) ? string.Empty : worldsSorted[0].uniqueId;
		}
		return Manager.Get<SavedWorldManager>().GetWorldById(worldsSorted[num + 1].uniqueId).uniqueId;
	}
}
