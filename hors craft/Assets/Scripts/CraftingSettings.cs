// DecompilerFi decompiler from Assembly-CSharp.dll class: CraftingSettings
using Common.Managers;
using Common.Model;
using System;
using System.Collections.Generic;
using System.Linq;

public class CraftingSettings : ModelModule
{
	public Action OnModelDownload;

	private string keyCraftingEnabled()
	{
		return "crafting.enabled";
	}

	private string keyBlueprintsEnabled()
	{
		return "blueprints.enabled";
	}

	private string keyBlueprintsButtonEnabled()
	{
		return "blueprints.button.enabled";
	}

	private string keyCategoriesEnabled()
	{
		return "crafting.categories.enabled";
	}

	private string keyResourcePerAdEnabled()
	{
		return "crafting.resources.for.ad.enabled";
	}

	private string keyResourcePerAd(int id)
	{
		return "crafting.resource.per.ad." + id.ToString();
	}

	private string keyResourceOnStart(int id)
	{
		return "crafting.resource.on.start." + id.ToString();
	}

	private string keyBlueprintAdCost(int adNumber)
	{
		return "blueprint.ad.cost." + adNumber.ToString();
	}

	private string keyBlueprintsForFree()
	{
		return "blueprints.free";
	}

	private string keyCraftingForFree()
	{
		return "crafting.free";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyResourcePerAdEnabled(), defaultValue: true);
		descriptions.AddDescription(keyCraftingEnabled(), defaultValue: true);
		descriptions.AddDescription(keyBlueprintsEnabled(), defaultValue: true);
		descriptions.AddDescription(keyCategoriesEnabled(), defaultValue: true);
		descriptions.AddDescription(keyBlueprintsButtonEnabled(), defaultValue: false);
		descriptions.AddDescription(keyBlueprintsForFree(), defaultValue: false);
		descriptions.AddDescription(keyCraftingForFree(), defaultValue: false);
		CraftableList craftableListInstance = Manager.Get<CraftingManager>().GetCraftableListInstance();
		List<CraftableList.ResourceSpawn> resourcesList = craftableListInstance.resourcesList;
		HashSet<int> hashSet = new HashSet<int>(from r in resourcesList
			select r.id);
		foreach (int item in hashSet)
		{
			descriptions.AddDescription(keyResourcePerAd(item), 1);
			descriptions.AddDescription(keyResourceOnStart(item), 1);
		}
	}

	public override void OnModelDownloaded()
	{
		if (OnModelDownload != null)
		{
			OnModelDownload();
		}
	}

	public int GetResourceAmountPerAd(int id)
	{
		return base.settings.GetInt(keyResourcePerAd(id));
	}

	public int GetResourceAmountOnStart(int id)
	{
		return base.settings.GetInt(keyResourceOnStart(id));
	}

	public bool AreCategoriesEnabled()
	{
		return base.settings.GetBool(keyCategoriesEnabled());
	}

	public bool IsCraftingEnabled()
	{
		return base.settings.GetBool(keyCraftingEnabled());
	}

	public bool AreBlueprintsEnabled()
	{
		return base.settings.GetBool(keyBlueprintsEnabled());
	}

	public bool IsBlueprintsButtonEnabled()
	{
		return base.settings.GetBool(keyBlueprintsButtonEnabled());
	}

	public bool IsResourcesPerAdEnabled()
	{
		return base.settings.GetBool(keyResourcePerAdEnabled());
	}

	public bool AreBlueprintsFree()
	{
		return base.settings.GetBool(keyBlueprintsForFree());
	}

	public bool AreCraftingFree()
	{
		return base.settings.GetBool(keyCraftingForFree());
	}

	public int GetBlueprintCost(int amount)
	{
		int i;
		for (i = 1; ModelSettingsHelper.GetIntFromStringSettings(base.settings, keyBlueprintAdCost(i), 999999) < amount; i++)
		{
		}
		return i;
	}
}
