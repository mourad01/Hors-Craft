// DecompilerFi decompiler from Assembly-CSharp.dll class: DressupModule
using Common.Managers;
using Common.Model;

public class DressupModule : ModelModule
{
	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyReactionGlamourValue(), 30);
		descriptions.AddDescription(keyReactionGoldValue(), 5);
		descriptions.AddDescription(keyEventGlamourValue(), 1000);
		descriptions.AddDescription(keyEventGoldValue(), 200);
		descriptions.AddDescription(keyAdGoldValue(), 1000);
		for (int i = 0; i < 128; i++)
		{
			descriptions.AddDescription(keyClothesItemBasePrice(i), 10);
			descriptions.AddDescription(keyClothesItemPrestigeRequired(i), 0);
		}
	}

	public override void OnModelDownloaded()
	{
		if (DressupSkinList.instance != null)
		{
			DressupSkinList.instance.ChangeOrder(this);
		}
	}

	private string keyReactionGlamourValue()
	{
		return "dressup.reaction.glamour.value";
	}

	private string keyReactionGoldValue()
	{
		return "dressup.reaction.gold.value";
	}

	private string keyEventGlamourValue()
	{
		return "dressup.event.glamour.value";
	}

	private string keyEventGoldValue()
	{
		return "dressup.event.gold.value";
	}

	private string keyAdGoldValue()
	{
		return "dressup.ad.gold.value";
	}

	private string keyClothesItemBasePrice(int index)
	{
		return "dressup.item.base.price." + index;
	}

	private string keyClothesItemPrestigeRequired(int index)
	{
		return "dressup.item.prestige.required." + index;
	}

	public int GetReactionGlamourValue()
	{
		return base.settings.GetInt(keyReactionGlamourValue(), 30);
	}

	public int GetReactionGoldValue()
	{
		return base.settings.GetInt(keyReactionGoldValue(), 5);
	}

	public int GetEventGlamourValue()
	{
		return base.settings.GetInt(keyEventGlamourValue(), 1000);
	}

	public int GetEventGoldValue()
	{
		return base.settings.GetInt(keyEventGoldValue(), 200);
	}

	public int GetAdGoldValue()
	{
		return base.settings.GetInt(keyAdGoldValue(), 200);
	}

	public int GetClothesItemBasePriceValue(int index)
	{
		return base.settings.GetInt(keyClothesItemBasePrice(index), 0);
	}

	public int GetClothesItemPrestigeRequired(int index)
	{
		return base.settings.GetInt(keyClothesItemPrestigeRequired(index), 0);
	}
}
