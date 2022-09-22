// DecompilerFi decompiler from Assembly-CSharp.dll class: AditionalBlocksItemData
using Common.Managers;
using States;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Serialization;
using UnityEngine;

[XmlInclude(typeof(AditionalBlocksItemData))]
public class AditionalBlocksItemData : PackItemData
{
	public int grantCount = -1;

	public AditionalBlocksItemData()
	{
		type = PackItemType.AditionalBlocks;
	}

	public override void FillWithRandom()
	{
		grantCount = UnityEngine.Random.Range(0, 200);
	}

	public override Type GetConnectedType()
	{
		return typeof(AditionalBlocksItem);
	}

	public override void GrantItem()
	{
		if (grantCount <= 0)
		{
			return;
		}
		List<Resource> resourcesList = Manager.Get<CraftingManager>().GetResourcesList();
		for (int i = 0; i < resourcesList.Count; i++)
		{
			Singleton<PlayerData>.get.playerItems.AddToResources(resourcesList[i].id, grantCount);
		}
		if (!Manager.Get<StateMachineManager>().ContainsState(typeof(PauseState)))
		{
			return;
		}
		PauseState pauseState = Manager.Get<StateMachineManager>().GetStateInstance(typeof(PauseState)) as PauseState;
		if (!(pauseState == null) && (bool)(Manager.Get<StateMachineManager>().currentState as PauseState) && !(pauseState.getCurrentFragment.instance == null))
		{
			CraftingFragment craftingFragment = pauseState.getCurrentFragment.instance.GetComponent<Fragment>() as CraftingFragment;
			if (!(craftingFragment == null))
			{
				craftingFragment.UpdateFragment();
			}
		}
	}

	public override bool IsValid()
	{
		throw new NotImplementedException();
	}

	public override string ToParsable()
	{
		return $"{type}:{grantCount}";
	}

	public override void TryParseData(string data)
	{
		int.TryParse(data, NumberStyles.Any, CultureInfo.InvariantCulture, out grantCount);
	}
}
