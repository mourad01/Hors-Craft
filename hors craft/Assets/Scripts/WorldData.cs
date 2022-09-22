// DecompilerFi decompiler from Assembly-CSharp.dll class: WorldData
using Common.Managers;
using Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class WorldData
{
	public string name;

	public string uniqueId;

	public bool lastUsed;

	public bool selected;

	public bool savedAtStart;

	public bool screenWasMade;

	public double timestamp;

	public string titleId;

	public string descriptionId;

	public bool resources;

	public string resourcesPath;

	public string resourcesSubfolderPath;

	public string tilesetName;

	public bool isDownloadFinished;

	public bool isDownloaded;

	public bool isPaymentNeeded;

	public HashSet<string> tags = new HashSet<string>();

	public long serverTime;

	public List<SavedWorldManager.Recipe> recepies;

	public Vector3? startPosition;

	public float? startRot;

	public int cost;

	public bool startingWorld;

	public string terrainName;

	public int questLimit = 40;

	public int sortOrder;

	public WorldData(string uniqueId, string name, string resourcesPath = "", bool lastUsed = false, bool selected = false, double timestamp = double.MaxValue, bool resources = false, bool startingWorld = false, string resourcesSubfolderPath = "")
	{
		this.name = name;
		this.lastUsed = lastUsed;
		this.selected = selected;
		this.timestamp = timestamp;
		this.resources = resources;
		savedAtStart = false;
		screenWasMade = false;
		this.uniqueId = uniqueId;
		this.resourcesPath = resourcesPath;
		this.startingWorld = startingWorld;
		if (!string.IsNullOrEmpty(resourcesSubfolderPath))
		{
			resourcesSubfolderPath = "/" + resourcesSubfolderPath;
		}
		this.resourcesSubfolderPath = resourcesSubfolderPath;
		if (string.IsNullOrEmpty(name))
		{
			name = "myWorld";
		}
	}

	public static WorldData CreateDownloadedWorld(string uniqueId, string name, string texturePath, string terrainName, int questLimit)
	{
		WorldData worldData = new WorldData(uniqueId, name, string.Empty, lastUsed: false, selected: false, double.MaxValue, resources: false, startingWorld: false, string.Empty);
		worldData.isDownloaded = true;
		worldData.isDownloadFinished = false;
		worldData.timestamp = 0.0;
		worldData.tilesetName = texturePath;
		worldData.terrainName = terrainName;
		worldData.questLimit = questLimit;
		return worldData;
	}

	public bool CheckDownloadStatus()
	{
		return !isDownloaded || (isDownloaded && isDownloadFinished);
	}

	public void SetAsDownloaded(SavedWorldManager.DownloadWorld world)
	{
		isDownloaded = true;
		isPaymentNeeded = true;
		cost = world.price;
		startPosition = world.GetStartVector();
		startRot = world.start_rot;
		recepies = world.recepies;
		tags = world.GetTags();
		titleId = world.title;
		descriptionId = world.description;
		serverTime = long.Parse(world.timestamp);
		sortOrder = world.sort_order;
	}

	public int[] GetBlocks()
	{
		if (recepies == null || recepies.Count == 0)
		{
			return null;
		}
		List<int> list = new List<int>();
		for (int i = 0; i < recepies.Count; i++)
		{
			if (Manager.Get<CraftingManager>().CraftableIdsContains(recepies[i].id))
			{
				list.Add(Manager.Get<CraftingManager>().GetCraftable(recepies[i].id).id);
				Manager.Get<CraftingManager>().GetCraftable(recepies[i].id).connectedWorldID = uniqueId;
				Manager.Get<CraftingManager>().GetCraftable(recepies[i].id).worldQuestNeeded = recepies[i].quest_needed;
			}
		}
		return list.ToArray();
	}

	public string GetPath()
	{
		string empty = string.Empty;
		if (!resources)
		{
			return $"{Application.persistentDataPath}/Worlds/{uniqueId}";
		}
		return $"{Application.dataPath}/Resources/PregeneratedWorld_{resourcesPath}{resourcesSubfolderPath}";
	}

	public string GetResourcesPath()
	{
		return $"PregeneratedWorld_{resourcesPath}{resourcesSubfolderPath}";
	}

	public string GetImagePath()
	{
		return $"{Application.persistentDataPath}/WorldsScreenshots_{uniqueId}.png";
	}

	public IEnumerator ThumbnailCoorutine(Action<Sprite> spriteConsumer, Action<bool> onSuccess)
	{
		yield return new WaitForEndOfFrame();
		bool result;
		if (!CheckDownloadStatus())
		{
			TryToSetShopImage(spriteConsumer);
			result = false;
		}
		else
		{
			Sprite thumbnailImage = GetThumbnailImage();
			if (thumbnailImage != null)
			{
				spriteConsumer(thumbnailImage);
				result = false;
			}
			else
			{
				TryToSetShopImage(spriteConsumer);
				result = true;
			}
		}
		onSuccess?.Invoke(result);
	}

	public void TryToGetThumbnail(Action<Sprite> spriteConsumer, Action<bool> onSuccess)
	{
		Manager.Get<SavedWorldManager>().StartCoroutine(ThumbnailCoorutine(spriteConsumer, onSuccess));
	}

	private void TryToSetShopImage(Action<Sprite> spriteConsumer)
	{
		Manager.Get<SavedWorldManager>().TryToShopSprite(uniqueId, delegate(Sprite texture)
		{
			spriteConsumer(texture);
		});
	}

	public Sprite GetThumbnailImage()
	{
		Texture2D texture2D = null;
		texture2D = Misc.LoadPNG(GetImagePath());
		if (texture2D == null)
		{
			return null;
		}
		return Sprite.Create(texture2D, new Rect(0f, 0f, texture2D.width, texture2D.height), new Vector2(0.5f, 0.5f));
	}
}
