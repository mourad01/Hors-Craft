// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemConfigLevelRewards
using Common.Managers;
using Gameplay;
using System.Collections.Generic;
using System.Linq;

public class ItemConfigLevelRewards : LevelupRewardConfig
{
	public override List<LevelUpRewardItemData> GetRewards(int level)
	{
		List<LevelUpRewardItemData> list = new List<LevelUpRewardItemData>();
		List<ItemConfig.Item> items = Manager.Get<ItemConfig>().items;
		items = (from x in items
			where x.requirements.Any((RequirementWrapper req) => req.requirement is LevelRequirement && req.value == (float)level)
			select x).ToList();
		foreach (ItemConfig.Item item in items)
		{
			if (Manager.Get<ItemConfig>().IsUnlocked(item.key))
			{
				list.Add(new LevelUpRewardItemData
				{
					sprite = item.item.GetSprite(),
					displayName = Manager.Get<TranslationsManager>().GetText(item.translationsKey, item.item.GetName())
				});
			}
		}
		return list;
	}
}
