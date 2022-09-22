// DecompilerFi decompiler from Assembly-CSharp.dll class: SaveTransformsManager
using Common.Managers;
using Gameplay;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

public class SaveTransformsManager : AbstractSaveTransformManager, IGameCallbacksListener
{
	private List<GameObject> mobs;

	public override void Init()
	{
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
	}

	public void OnGameSavedFrequent()
	{
		SaveAll();
	}

	public void OnGameSavedInfrequent()
	{
	}

	public void OnGameplayStarted()
	{
		LoadAll();
	}

	public void OnGameplayRestarted()
	{
		DeleteAll();
	}

	protected override void PrepareDictionary()
	{
		base.PrepareDictionary();
		mobs = Manager.Get<MobsManager>().GetMobList();
		for (int i = 0; i < mobs.Count; i++)
		{
			name2Prefab.AddIfNotExists(mobs[i].GetComponent<AbstractSaveTransform>().PrefabName, mobs[i]);
		}
	}

	protected override void TryActivate()
	{
		int num = 0;
		while (num < toActivate.Count)
		{
			if (toActivate[num] == null)
			{
				toActivate.RemoveAt(num);
				continue;
			}
			GameObject gameObject = Engine.PositionToChunk(toActivate[num].transform.position);
			if (gameObject == null)
			{
				AbstractSaveTransform component = toActivate[num].GetComponent<AbstractSaveTransform>();
				if (component == null)
				{
					UnityEngine.Debug.LogError("Assign SaveTransform on object instantiate: " + toActivate[num].name);
				}
				else
				{
					component.RegisterToFreezed();
				}
				toActivate.RemoveAt(num);
			}
			else
			{
				Chunk component2 = gameObject.GetComponent<Chunk>();
				if (component2 != null && component2.Spawned)
				{
					toActivate[num].SetActive(value: true);
					toActivate.RemoveAt(num);
				}
				else
				{
					num++;
				}
			}
		}
	}
}
