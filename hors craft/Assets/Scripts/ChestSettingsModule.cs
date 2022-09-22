// DecompilerFi decompiler from Assembly-CSharp.dll class: ChestSettingsModule
using Common.Managers;
using Common.Model;
using Uniblocks;

public class ChestSettingsModule : ModelModule
{
	private string keyChestProbability()
	{
		return "chest.spawn.probablity";
	}

	private string keyChestMoneyCount()
	{
		return "chest.currency.count";
	}

	private string keyChestSpawnsResources()
	{
		return "chest.spawns.resources";
	}

	public override void FillModelDescription(ModelDescription descriptions)
	{
		descriptions.AddDescription(keyChestProbability(), 0.0001f);
		descriptions.AddDescription(keyChestMoneyCount(), 10);
		descriptions.AddDescription(keyChestSpawnsResources(), 0);
	}

	public override void OnModelDownloaded()
	{
		if (Engine.EngineInstance != null)
		{
			Engine.EngineInstance.GetComponent<CommonTerrainGenerator>().chests.placeProbability = GetChestProbablity();
		}
	}

	public float GetChestProbablity()
	{
		return base.settings.GetFloat(keyChestProbability(), 0.0001f);
	}

	public int GetChestCoins()
	{
		return base.settings.GetInt(keyChestMoneyCount(), 10);
	}

	public float GetChestSpawnsResources()
	{
		return base.settings.GetInt(keyChestSpawnsResources(), 1);
	}
}
