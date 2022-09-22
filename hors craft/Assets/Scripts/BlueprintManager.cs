// DecompilerFi decompiler from Assembly-CSharp.dll class: BlueprintManager
using Common.Managers;
using Common.Utils;
using Gameplay;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;

public class BlueprintManager : Manager, IGameCallbacksListener
{
	private const string BLUEPRINTS_KEY = "blueprints.data";

	public bool trackBuiltBlueprints;

	[HideInInspector]
	public List<ushort> neutralIDList = new List<ushort>
	{
		0,
		12
	};

	private BlueprintBuilder builder;

	private List<PlacedBlueprint> placedBlueprints = new List<PlacedBlueprint>();

	private Dictionary<string, BlueprintData> loadedBlueprintData = new Dictionary<string, BlueprintData>();

	public List<PlacedBlueprintData> builtBlueprints
	{
		get;
		private set;
	}

	public override void Init()
	{
		builder = GetComponent<BlueprintBuilder>();
		placedBlueprints = new List<PlacedBlueprint>();
		Manager.Get<GameCallbacksManager>().RegisterListener(this);
	}

	public void RegisterBlueprint(PlacedBlueprintData blueprint, bool loaded = false)
	{
		PlacedBlueprint placedBlueprint = new PlacedBlueprint();
		placedBlueprint.loaded = loaded;
		placedBlueprint.placedBlueprintData = blueprint;
		placedBlueprint.placedVoxels = new HashSet<Vector3>(blueprint.placedVoxelsList);
		GetBlueprintData(blueprint.dataName);
		placedBlueprints.Add(placedBlueprint);
	}

	public void ShowTutorial()
	{
		GetComponent<BlueprintTutorial>().StartTutorial();
	}

	public IEnumerator LoadBlueprints()
	{
		foreach (PlacedBlueprint placedBlueprint in placedBlueprints)
		{
			if (!placedBlueprint.loaded)
			{
				builder.FixBlueprint(placedBlueprint);
				yield return null;
			}
		}
	}

	public void BuiltBlueprint(PlacedBlueprint blueprint)
	{
		if (trackBuiltBlueprints)
		{
			Manager.Get<ItemConfig>()?.OnItemClaimed(blueprint.placedBlueprintData.dataName);
			string key = $"{blueprint.placedBlueprintData.dataName}.built";
			if (!AutoRefreshingStock.HasItem(key))
			{
				AutoRefreshingStock.InitStockItem(key, float.NaN, 0);
			}
			AutoRefreshingStock.IncrementStockCount(key);
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.EVENT_POSITION, new PositionSignalContext
			{
				position = blueprint.placedBlueprintData.position
			});
			blueprint.placedBlueprintData.placedVoxelsList = new List<Vector3>();
			blueprint.placedBlueprintData.built = true;
			builtBlueprints.Add(blueprint.placedBlueprintData);
			RemovePlacedBlueprint(blueprint, nullData: false);
		}
		else
		{
			RemovePlacedBlueprint(blueprint);
		}
	}

	public void FillVoxelInPosition(Vector3 worldPosition, bool manually = true)
	{
		builder.FillVoxelInPosition(worldPosition, manually);
	}

	public void DeleteBlueprint(Vector3 worldPosition)
	{
		PlacedBlueprint placedBlueprint = GetPlacedBlueprint(worldPosition, accurate: true);
		PlacedBlueprintData placedBlueprintData;
		if (placedBlueprint == null)
		{
			placedBlueprintData = GetBuiltBlueprint(worldPosition);
			OnBeforeDestroy(placedBlueprintData);
			RemoveBuiltBlueprint(placedBlueprintData);
		}
		else
		{
			placedBlueprintData = placedBlueprint.placedBlueprintData;
			OnBeforeDestroy(placedBlueprintData);
			RemovePlacedBlueprint(placedBlueprint);
		}
		builder.DeleteBlueprint(placedBlueprintData);
		Manager.Get<GameCallbacksManager>().FrequentSave();
		Engine.SaveWorld();
	}

	private void OnBeforeDestroy(PlacedBlueprintData data)
	{
		string key = $"{data.dataName}.built";
		if (AutoRefreshingStock.HasItem(key))
		{
			AutoRefreshingStock.DecrementStockCount(key);
		}
		Singleton<PlayerData>.get.playerItems.AddCraftable(data.craftableId, 1);
	}

	public void InstantFillAllBlueprintAccurate(Vector3 worldPosition)
	{
		builder.InstantFillAllBlueprintAccurate(worldPosition);
	}

	public Vector3 GetDefaultBlockToBuild(Vector3 worldPosition)
	{
		return builder.GetDefaultBlockToBuild(worldPosition);
	}

	private void RemovePlacedBlueprint(PlacedBlueprint placedBlueprint, bool nullData = true)
	{
		string dataName = placedBlueprint.placedBlueprintData.dataName;
		if (placedBlueprints.FirstOrDefault((PlacedBlueprint pb) => pb.placedBlueprintData.dataName.Equals(dataName)) == null && loadedBlueprintData.ContainsKey(dataName))
		{
			loadedBlueprintData.Remove(dataName);
		}
		if (nullData)
		{
			placedBlueprint.placedBlueprintData = null;
		}
		placedBlueprints.Remove(placedBlueprint);
	}

	private void RemoveBuiltBlueprint(PlacedBlueprintData data)
	{
		builtBlueprints.Remove(data);
	}

	public bool IsWithinBlueprintTriggerRange(Vector3 worldPosition, int triggerRange = 7)
	{
		return placedBlueprints.FirstOrDefault((PlacedBlueprint pb) => pb.InBounds(worldPosition, triggerRange)) != null;
	}

	public ushort GetRealVoxelTypeFromWorldPosition(PlacedBlueprintData placedBlueprintData, Vector3 worldPosition)
	{
		Vector3 localPosition = placedBlueprintData.GetLocalPosition(worldPosition);
		return Engine.GetVoxelType(GetBlueprintData(placedBlueprintData.dataName).GetVoxel((int)localPosition.x, (int)localPosition.y, (int)localPosition.z)).GetUniqueID();
	}

	public ushort GetRealVoxelTypeFromWorldPosition(PlacedBlueprintData placedBlueprintData, Vector3 worldPosition, out byte rotation)
	{
		Vector3 localPosition = placedBlueprintData.GetLocalPosition(worldPosition);
		return Engine.GetVoxelType(GetBlueprintData(placedBlueprintData.dataName).GetVoxel((int)localPosition.x, (int)localPosition.y, (int)localPosition.z, out rotation)).GetUniqueID();
	}

	public ushort GetRealVoxelTypeFromLocalPosition(PlacedBlueprintData placedBlueprintData, Vector3 LocalPosition, out byte rotation)
	{
		return Engine.GetVoxelType(GetBlueprintData(placedBlueprintData.dataName).GetVoxel((int)LocalPosition.x, (int)LocalPosition.y, (int)LocalPosition.z, out rotation)).GetUniqueID();
	}

	public Voxel GetRealVoxel(Vector3 worldPosition, out byte rotation)
	{
		PlacedBlueprint placedBlueprint = GetPlacedBlueprint(worldPosition, accurate: true);
		if (placedBlueprint == null)
		{
			rotation = 0;
			return Engine.GetVoxelType(0);
		}
		PlacedBlueprintData placedBlueprintData = placedBlueprint.placedBlueprintData;
		Vector3 localPosition = placedBlueprintData.GetLocalPosition(worldPosition);
		return Engine.GetVoxelType(GetBlueprintData(placedBlueprintData.dataName).GetVoxel((int)localPosition.x, (int)localPosition.y, (int)localPosition.z, out rotation));
	}

	public bool CheckBlueprintIntersection(Vector3 worldMin, Vector3 worldMax)
	{
		foreach (PlacedBlueprint placedBlueprint in placedBlueprints)
		{
			Vector3 localPosition = placedBlueprint.placedBlueprintData.GetLocalPosition(worldMin);
			Vector3 localPosition2 = placedBlueprint.placedBlueprintData.GetLocalPosition(worldMax);
			if ((!(localPosition.x < placedBlueprint.placedBlueprintData.min.x) || !(localPosition2.x < placedBlueprint.placedBlueprintData.min.x)) && (!(localPosition.x > placedBlueprint.placedBlueprintData.max.x) || !(localPosition2.x > placedBlueprint.placedBlueprintData.max.x)) && (!(localPosition.y < placedBlueprint.placedBlueprintData.min.y) || !(localPosition2.y < placedBlueprint.placedBlueprintData.min.y)) && (!(localPosition.y > placedBlueprint.placedBlueprintData.max.y) || !(localPosition2.y > placedBlueprint.placedBlueprintData.max.y)) && (!(localPosition.z < placedBlueprint.placedBlueprintData.min.z) || !(localPosition2.z < placedBlueprint.placedBlueprintData.min.z)) && (!(localPosition.z > placedBlueprint.placedBlueprintData.max.z) || !(localPosition2.z > placedBlueprint.placedBlueprintData.max.z)))
			{
				return true;
			}
		}
		return false;
	}

	public bool IsBlueprint(Vector3 worldPosition)
	{
		if (trackBuiltBlueprints)
		{
			return GetPlacedBlueprint(worldPosition, accurate: true, canBeNeutral: false) != null || GetBuiltBlueprint(worldPosition) != null;
		}
		return GetPlacedBlueprint(worldPosition, accurate: true, canBeNeutral: false) != null;
	}

	public PlacedBlueprint GetPlacedBlueprint(Vector3 worldPosition, bool accurate, bool canBeNeutral = true)
	{
		if (placedBlueprints.IsNullOrEmpty())
		{
			return null;
		}
		if (accurate)
		{
			Func<PlacedBlueprint, bool> predicate = delegate(PlacedBlueprint b)
			{
				if (b.InBounds(worldPosition))
				{
					if (canBeNeutral)
					{
						return true;
					}
					ushort realVoxelTypeFromWorldPosition = GetRealVoxelTypeFromWorldPosition(b.placedBlueprintData, worldPosition);
					return realVoxelTypeFromWorldPosition != 0 && realVoxelTypeFromWorldPosition != 12;
				}
				return false;
			};
			return placedBlueprints.FirstOrDefault(predicate);
		}
		List<PlacedBlueprint> list = (from pb in placedBlueprints
			where pb.InBounds(worldPosition, 7)
			select pb).ToList();
		if (list.IsNullOrEmpty())
		{
			return null;
		}
		return (from b in list
			orderby Vector3.Distance(b.placedBlueprintData.position, worldPosition)
			select b).First();
	}

	public PlacedBlueprintData GetBuiltBlueprint(Vector3 worldPosition)
	{
		if (builtBlueprints.IsNullOrEmpty())
		{
			return null;
		}
		Func<PlacedBlueprintData, bool> predicate = delegate(PlacedBlueprintData b)
		{
			if (!b.InBounds(worldPosition))
			{
				return false;
			}
			ushort realVoxelTypeFromWorldPosition = GetRealVoxelTypeFromWorldPosition(b, worldPosition);
			return realVoxelTypeFromWorldPosition != 0 && realVoxelTypeFromWorldPosition != 12;
		};
		return builtBlueprints.FirstOrDefault(predicate);
	}

	public BlueprintData GetBlueprintData(string name)
	{
		if (!loadedBlueprintData.ContainsKey(name))
		{
			loadedBlueprintData[name] = BlueprintDataFiles.ReadDataFromResources(name);
		}
		return loadedBlueprintData[name];
	}

	public void OnGameplayStarted()
	{
		if (WorldPlayerPrefs.get.HasString("blueprints.data"))
		{
			string @string = WorldPlayerPrefs.get.GetString("blueprints.data");
			placedBlueprints = new List<PlacedBlueprint>();
			builtBlueprints = new List<PlacedBlueprintData>();
			List<PlacedBlueprintData> list = JSONHelper.Deserialize<List<PlacedBlueprintData>>(@string);
			foreach (PlacedBlueprintData item in list)
			{
				if (item.built)
				{
					builtBlueprints.Add(item);
				}
				else if (item.blocksToFillInBlueprint > 0)
				{
					RegisterBlueprint(item);
				}
			}
		}
		else
		{
			if (placedBlueprints == null)
			{
				placedBlueprints = new List<PlacedBlueprint>();
			}
			builtBlueprints = new List<PlacedBlueprintData>();
			loadedBlueprintData = new Dictionary<string, BlueprintData>();
		}
	}

	public void OnGameplayRestarted()
	{
		placedBlueprints = new List<PlacedBlueprint>();
		builtBlueprints = new List<PlacedBlueprintData>();
		loadedBlueprintData = new Dictionary<string, BlueprintData>();
	}

	public void OnGameSavedFrequent()
	{
		foreach (PlacedBlueprint placedBlueprint in placedBlueprints)
		{
			placedBlueprint.placedBlueprintData.placedVoxelsList = new List<Vector3>(placedBlueprint.placedVoxels);
		}
		List<PlacedBlueprintData> list = (from b in placedBlueprints
			select b.placedBlueprintData).ToList();
		list.AddRange(builtBlueprints);
		string value = JSONHelper.ToJSON(list);
		WorldPlayerPrefs.get.SetString("blueprints.data", value);
	}

	public void OnGameSavedInfrequent()
	{
	}
}
