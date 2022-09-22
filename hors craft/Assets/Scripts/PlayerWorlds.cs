// DecompilerFi decompiler from Assembly-CSharp.dll class: PlayerWorlds
using Common.Managers;
using Common.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerWorlds
{
	[Serializable]
	public class BoughtWorld
	{
		public string name;

		public double timestamp;

		public BoughtWorld()
		{
		}

		public BoughtWorld(string name, double timestamp)
		{
			this.name = name;
			this.timestamp = timestamp;
		}

		public override int GetHashCode()
		{
			return name.GetHashCode();
		}
	}

	private const string WORLDS_KEY = "player.worlds.bought";

	private List<BoughtWorld> boughtWorlds;

	private Dictionary<string, BoughtWorld> worldsData;

	public PlayerWorlds()
	{
		boughtWorlds = new List<BoughtWorld>();
		worldsData = new Dictionary<string, BoughtWorld>();
	}

	public static PlayerWorlds LoadFromPlayerPrefs()
	{
		string @string = PlayerPrefs.GetString("player.worlds.bought", string.Empty);
		PlayerWorlds worlds = new PlayerWorlds();
		if (!string.IsNullOrEmpty(@string))
		{
			worlds.boughtWorlds = JSONHelper.Deserialize<List<BoughtWorld>>(@string);
		}
		worlds.boughtWorlds.ForEach(delegate(BoughtWorld world)
		{
			worlds.worldsData[world.name] = world;
		});
		return worlds;
	}

	public bool IsWorldBought(string worldId)
	{
		return worldsData.ContainsKey(worldId);
	}

	public double GetTimeOfBuy(string worldId)
	{
		if (worldsData.ContainsKey(worldId))
		{
			return worldsData[worldId].timestamp;
		}
		return -1.0;
	}

	public void OnWorldBought(string worldId, int cost = 0)
	{
		BoughtWorld item = new BoughtWorld(worldId, Misc.GetTimeStampDouble());
		if (!boughtWorlds.Contains(item) && !worldsData.ContainsKey(worldId))
		{
			boughtWorlds.Add(item);
			worldsData.Add(worldId, boughtWorlds.GetLastItem());
			SaveToPrefs();
			TryToInformManagers(worldId, cost);
		}
	}

	public void TryToInformManagers(string worldBought, int cost)
	{
		if (Manager.Contains<UltimateSoftCurrencyManager>())
		{
			Manager.Get<StatsManager>().OnWorldBought(worldBought, Manager.Get<UltimateSoftCurrencyManager>().GetCurrencyAmount(), cost);
		}
		if (Manager.Contains<OfferPackManager>())
		{
			Manager.Get<OfferPackManager>().OnWorldBought(worldBought);
		}
	}

	public void SaveToPrefs()
	{
		string value = JSONHelper.ToJSON(boughtWorlds);
		PlayerPrefs.SetString("player.worlds.bought", value);
		PlayerPrefs.Save();
	}
}
