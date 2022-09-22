// DecompilerFi decompiler from Assembly-CSharp.dll class: BlueprintListDataProvider
using Common.Managers;
using Gameplay;
using System.Collections.Generic;
using Uniblocks;

public class BlueprintListDataProvider : ScrollableListDataProvider
{
	private List<Craftable> blueprints;

	public override void Init(ScrollableListRawContent content)
	{
		blueprints = (content as BlueprintListRawContent).blueprints;
	}

	public override List<ScrollableListElement> PrepareData()
	{
		List<ScrollableListElement> list = new List<ScrollableListElement>();
		foreach (Craftable blueprint in blueprints)
		{
			BlueprintDataElement blueprintDataElement = ConstructDataElement(blueprint);
			if (blueprintDataElement != null)
			{
				list.Add(blueprintDataElement);
			}
		}
		list.Sort((ScrollableListElement a, ScrollableListElement b) => ((BlueprintDataElement)a).index.CompareTo(((BlueprintDataElement)b).index));
		return list;
	}

	private BlueprintDataElement ConstructDataElement(Craftable craftable)
	{
		ItemConfig.Item item = Manager.Get<ItemConfig>().GetItem(craftable.customCraftableObject.GetComponent<BlueprintCraftableObject>().name);
		if (item == null)
		{
			return null;
		}
		BlueprintDataElement blueprintDataElement = new BlueprintDataElement();
		blueprintDataElement.craftable = craftable;
		blueprintDataElement.blueprintConfigItem = item;
		blueprintDataElement.name = blueprintDataElement.blueprintConfigItem.key;
		blueprintDataElement.icon = craftable.sprite;
		blueprintDataElement.index = Manager.Get<ItemConfig>().items.IndexOf(item);
		return blueprintDataElement;
	}
}
