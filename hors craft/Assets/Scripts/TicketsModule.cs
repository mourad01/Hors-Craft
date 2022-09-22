// DecompilerFi decompiler from Assembly-CSharp.dll class: TicketsModule
using Common.Managers;
using Common.Model;

public class TicketsModule : ModelModule
{
	private string keyBlueprintUnlockPrice()
	{
		return "tickets.per.blueprint.unlock";
	}

	private string keyFillBlueprintPrice()
	{
		return "tickets.per.blueprint.fill";
	}

	private string keyTicketsForAd()
	{
		return "tickets.per.ad";
	}

	private string keyTicketsForEntrance()
	{
		return "tickets.entrance.fee";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyBlueprintUnlockPrice(), 5);
		descriptions.AddDescription(keyFillBlueprintPrice(), 15);
		descriptions.AddDescription(keyTicketsForAd(), 20);
		descriptions.AddDescription(keyTicketsForEntrance(), 1);
	}

	public override void OnModelDownloaded()
	{
	}

	public int GetBlueprintUnlockPrice()
	{
		return base.settings.GetInt(keyBlueprintUnlockPrice(), 5);
	}

	public int GetFillBlueprintPrice()
	{
		return base.settings.GetInt(keyFillBlueprintPrice(), 15);
	}

	public int GetTicketsForAd()
	{
		return base.settings.GetInt(keyTicketsForAd(), 20);
	}

	public int GetTicketsForEntrance()
	{
		return base.settings.GetInt(keyTicketsForEntrance(), 1);
	}
}
