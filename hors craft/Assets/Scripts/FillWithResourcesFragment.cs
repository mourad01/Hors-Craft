// DecompilerFi decompiler from Assembly-CSharp.dll class: FillWithResourcesFragment
using Common.Managers;
using QuestSystems.Adventure;
using States;
using System.Collections.Generic;
using UnityEngine;

public class FillWithResourcesFragment : FillScrollListFragment
{
	public bool isPortalList;

	public bool autoFillAtStart;

	public void Start()
	{
		if (autoFillAtStart)
		{
			Init(null);
		}
	}

	public override void Init(FragmentStartParameter parameter)
	{
		base.Init(parameter);
		List<QuestItemObject> list = new List<QuestItemObject>();
		List<Resource> resourcesList = Singleton<PlayerData>.get.playerItems.GetResourcesList();
		foreach (Resource item in resourcesList)
		{
			if ((isPortalList || !Manager.Get<AdventureQuestManager>().portalDataHolder.IsItemAPortalItem(item.id)) && (!isPortalList || Manager.Get<AdventureQuestManager>().portalDataHolder.IsItemAPortalItem(item.id)))
			{
				QuestItemObject questItemObject = new QuestItemObject();
				questItemObject.SetItem(item);
				list.Add(questItemObject);
			}
		}
		Fill(list);
	}

	public override void Fill<T>(List<T> objects)
	{
		foreach (T @object in objects)
		{
			Resource item = (@object as QuestItemObject).Item;
			if (!RewardBase.silentItems.Contains(item.id))
			{
				RectTransform rectTransform = InstantiateElement();
				QuestItemElement component = rectTransform.GetComponent<QuestItemElement>();
				if (!(component == null))
				{
					component.FillWithInfo($"{item.id}", Manager.Get<CraftingManager>().GetResourceImage(item.id));
					component.SetText(item.count.ToString());
					component.SetItemId(item.id);
				}
			}
		}
	}
}
