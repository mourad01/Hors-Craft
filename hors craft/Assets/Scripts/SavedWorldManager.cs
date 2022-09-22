// DecompilerFi decompiler from Assembly-CSharp.dll class: SavedWorldManager
using com.ootii.Cameras;
using Common.Managers;
using Common.Utils;
using Gameplay;
using States;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Uniblocks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SavedWorldManager : Manager, IGameCallbacksListener
{
	[Serializable]
	public class DownloadWorld
	{
		public string display_name;

		public string id;

		public string world_cdn;

		public string terrain_generator_file;

		public string shop_image;

		public List<string> textures;

		public List<Recipe> recepies;

		public List<Animal> animals;

		public int price;

		public string start_pos;

		public float start_rot;

		public string blocks;

		public string title;

		public string description;

		public string timestamp;

		public string tags;

		public int starting_world;

		public int quest_limit;

		public int sort_order;

		public bool isStartingWorld => starting_world == 1;

		public Vector3 GetStartVector()
		{
			string[] array = start_pos.Split(',');
			return new Vector3(float.Parse(array[0]), float.Parse(array[1]), float.Parse(array[2]));
		}

		public HashSet<string> GetTags()
		{
			string[] array = tags.Split(',');
			HashSet<string> hashSet = new HashSet<string>();
			for (int i = 0; i < array.Length; i++)
			{
				hashSet.Add(array[i]);
			}
			return hashSet;
		}

		public override string ToString()
		{
			return $"[DownloadWorld] display_name : {display_name} || id : {id}";
		}
	}

	[Serializable]
	public class Animal
	{
		public string name;

		public float weight;

		public int spawn_count_from;

		public int spawn_count_to;

		public float yRot;

		public Animal()
		{
		}

		public Animal(string name, float weight, int spawn_count_from, int spawn_count_to, float yRot)
		{
			this.name = name;
			this.weight = weight;
			this.spawn_count_from = spawn_count_from;
			this.spawn_count_to = spawn_count_to;
			this.yRot = yRot;
		}

		public Animal(MobsManager.MobSpawnConfig mob)
		{
			name = mob.prefab.name;
			weight = mob.weight;
			spawn_count_from = mob.spawnCountFrom;
			spawn_count_to = mob.spawnCountTo;
			yRot = mob.desiredYRotation;
		}

		public MobsManager.MobSpawnConfig createSpawnConfig(GameObject prefab)
		{
			MobsManager.MobSpawnConfig mobSpawnConfig = new MobsManager.MobSpawnConfig();
			mobSpawnConfig.prefab = prefab;
			mobSpawnConfig.weight = weight;
			mobSpawnConfig.spawnCountFrom = spawn_count_from;
			mobSpawnConfig.spawnCountTo = spawn_count_to;
			mobSpawnConfig.desiredYRotation = yRot;
			return mobSpawnConfig;
		}
	}

	[Serializable]
	public class Recipe
	{
		public int id;

		public int quest_needed;

		public Recipe()
		{
		}

		public Recipe(int craftable_id, int questNeeded)
		{
			id = craftable_id;
			quest_needed = questNeeded;
		}
	}

	public const string BASE_WORLD_ID = "xcraft.world1";

	private const string prefsWorldKey = "worlds.data";

	private const string prefsSlotsKey = "worlds.slots.used";

	private const string prefsSlotsCounterKey = "worlds.slots.counter";

	private const string currentWorldIndexKey = "worlds.current";

	[Tooltip("Number of worlds in the resources folder")]
	public int worldCount = 1;

	public List<Vector3> StartingPositions = new List<Vector3>();

	private CommonTerrainData baseTerrainData;

	private Texture baseEngineTileset;

	private Texture baseEngineNormalmap;

	private Texture baseEngineLookup;

	private Dictionary<string, DownloadWorld> worldsToDownload;

	private List<WorldData> worlds;

	private List<WorldData> worldsFromModel;

	private bool canReturnToGame = true;

	public bool restrictWorldSaving;

	public ImagesCache shopImages = new ImagesCache();

	public bool addIosSuffixToPath;

	private Dictionary<string, WorldDownloader> downloaders;

	public bool forceStartPosition;

	private int _currentWorldDataIndex;

	public int currentWorldDataIndex
	{
		get
		{
			int currentWorldDataIndex = _currentWorldDataIndex;
			if (currentWorldDataIndex < 0 || currentWorldDataIndex >= worlds.Count)
			{
				return 0;
			}
			return currentWorldDataIndex;
		}
		set
		{
			if (restrictWorldSaving)
			{
				canReturnToGame = false;
			}
			else
			{
				canReturnToGame = (value >= 0);
			}
			_currentWorldDataIndex = value;
			PlayerPrefs.SetInt("worlds.current", value);
		}
	}

	public void ResetAll()
	{
		PlayerPrefs.DeleteKey("worlds.current");
		PlayerPrefs.DeleteKey("worlds.data");
		PlayerPrefs.Save();
	}

	public override void Init()
	{
		WorldsSettingsModule worldsSettings = Manager.Get<ModelManager>().worldsSettings;
		worldsSettings.OnModelDownload = (Action)Delegate.Combine(worldsSettings.OnModelDownload, new Action(OmModelDownloaded));
		currentWorldDataIndex = PlayerPrefs.GetInt("worlds.current", 0);
	}

	public void OmModelDownloaded()
	{
		if (worlds == null || worlds.Count == 0)
		{
			worlds = LoadWorldsFromPrefs();
		}
		Manager.Get<GameCallbacksManager>().RegisterListener(Manager.Get<SavedWorldManager>());
		if (Manager.Get<ModelManager>().worldsSettings.IsThisGameUltimate())
		{
			Singleton<UltimateCraftModelDownloader>.get.DownloadModel();
		}
	}

	public void CreateWorldDirectory(WorldData data)
	{
		Directory.CreateDirectory(data.GetPath());
	}

	public string GetCurrentPath()
	{
		return worlds[currentWorldDataIndex].GetPath();
	}

	public string GetCurrentResourcesPath()
	{
		return worlds[currentWorldDataIndex].GetResourcesPath();
	}

	public string GetPathForWorldId(string id)
	{
		return GetWorldById(id).GetPath();
	}

	public bool ShouldSaveAtSelect()
	{
		return !worlds[currentWorldDataIndex].savedAtStart;
	}

	public bool WasCurrentWorldDeleted()
	{
		return !canReturnToGame;
	}

	public void TryToShopSprite(string id, Action<Sprite> textureConsumer)
	{
		if (downloaders != null && downloaders.ContainsKey(id))
		{
			Sprite texture = null;
			if (!shopImages.TryToGetImage(id, out texture))
			{
				downloaders[id].DownloadShopImage(delegate(Texture image)
				{
					shopImages.AddImage(id, image);
					textureConsumer(shopImages.GetImage(id));
				});
			}
			else
			{
				textureConsumer(texture);
			}
		}
	}

	public bool CanReturnToGame()
	{
		if (restrictWorldSaving)
		{
			return canReturnToGame = false;
		}
		return canReturnToGame;
	}

	public WorldDownloader DownloadWorldForPlayer(string worldId, Action<bool> onWorldDownloaded)
	{
		if (downloaders != null && !downloaders.ContainsKey(worldId))
		{
			return null;
		}
		int index = GetIndexWorldFromModelById(worldId);
		if (index >= 0)
		{
			if (worldsFromModel[index].CheckDownloadStatus())
			{
				onWorldDownloaded(obj: true);
			}
			else
			{
				downloaders[worldId].DownloadWorldForPlaying(delegate
				{
					worldsFromModel[index].isDownloadFinished = true;
					AddDownloadedWorld(worldsFromModel[index]);
					SaveToPrefs();
					onWorldDownloaded(obj: false);
				});
			}
		}
		return downloaders[worldId];
	}

	public void OnReturnToTitleScreen()
	{
		RemoveCurrentWorld();
		canReturnToGame = true;
	}

	public bool IsWorldReadyToPlay(string worldId)
	{
		if (downloaders != null && downloaders.ContainsKey(worldId))
		{
			return downloaders[worldId].isWorldReadyToPlay || GetWorldById(worldId) != null;
		}
		return true;
	}

	public void ParseAndDownloadable(List<object> worldsData)
	{
		downloaders = new Dictionary<string, WorldDownloader>();
		worldsToDownload = new Dictionary<string, DownloadWorld>();
		worldsFromModel = new List<WorldData>();
		if (worlds != null && worlds.Count != 0)
		{
			foreach (object worldsDatum in worldsData)
			{
				DownloadWorld world = JSONHelper.Deserialize<DownloadWorld>(worldsDatum);
				worldsToDownload[world.id] = world;
				downloaders[world.id] = WorldDownloader.InitializeDownloader(world, OnErrorInitial);
				WorldData worldData = worlds.Find((WorldData data) => data.uniqueId == world.id);
				if (worldData == null)
				{
					WorldData worldData2 = WorldData.CreateDownloadedWorld(world.id, world.display_name, downloaders[world.id].GetTilesetName(), world.terrain_generator_file, world.quest_limit);
					worldData2.SetAsDownloaded(world);
					if (world.isStartingWorld)
					{
						int index = worlds.FindIndex((WorldData obj) => obj.uniqueId.Equals("xcraft.world1"));
						worldData2.uniqueId = "xcraft.world1";
						worldData2.resources = true;
						worlds[index] = worldData2;
						UnityEngine.Debug.LogError(world.GetStartVector() + " " + worldData2.startPosition);
					}
					else
					{
						worldsFromModel.Add(worldData2);
					}
				}
				else
				{
					worldData.SetAsDownloaded(world);
				}
			}
		}
	}

	private void OnErrorInitial(string idFailed)
	{
		if (worlds.Find((WorldData world) => world.uniqueId == idFailed) == null)
		{
			UnityEngine.Debug.LogError("Failed: " + idFailed);
			downloaders.Remove(idFailed);
			worldsFromModel.RemoveAll((WorldData world) => world.uniqueId == idFailed);
		}
	}

	public void DeleteWorld(string id)
	{
		string currentWorldId = worlds[currentWorldDataIndex].uniqueId;
		int num = worlds.FindIndex((WorldData world) => world.uniqueId == id);
		if (num == currentWorldDataIndex)
		{
			RemoveCurrentWorld();
		}
		WorldData data = worlds[num];
		worlds.RemoveAt(num);
		RemoveFilesOfData(data);
		currentWorldDataIndex = worlds.FindIndex((WorldData world) => world.uniqueId == currentWorldId);
		if (currentWorldDataIndex < 0)
		{
			string uniqueId = GetAllWorlds()[0].uniqueId;
			currentWorldDataIndex = GetIndexWorldById(uniqueId);
			Engine.SetWorldName(worlds[currentWorldDataIndex].name);
			Singleton<PlayerData>.get.playerWorlds.OnWorldBought(currentWorldId);
			canReturnToGame = false;
		}
		int numberOfUsedSlots = GetNumberOfUsedSlots();
		PlayerPrefs.SetInt("worlds.slots.used", numberOfUsedSlots - 1);
		SaveToPrefs();
	}

	private void RemoveFilesOfData(WorldData data)
	{
		UnityEngine.Debug.Log(Directory.Exists(data.GetPath()) + " " + data.GetPath());
		UnityEngine.Debug.Log(Directory.Exists(data.GetImagePath()) + " " + data.GetImagePath());
		if (Directory.Exists(data.GetPath()))
		{
			Directory.Delete(data.GetPath(), recursive: true);
		}
		if (File.Exists(data.GetImagePath()))
		{
			File.Delete(data.GetImagePath());
		}
	}

	public string CreateNewWorldIndentifier(string userName, bool increaseWorldCount)
	{
		int num = GetNumberOfUsedSlots();
		if (increaseWorldCount)
		{
			PlayerPrefs.SetInt("worlds.slots.used", num + 1);
			num++;
		}
		IncreaseGlobaWorldsCounter();
		string id = PlayerId.GetId();
		return string.Format("{0}_{1}_{2}_{3}", userName, (num != 0) ? GetGlobaWorldsCounter().ToString() : "pregenerated", id, Misc.GetTimeStampDouble());
	}

	public int GetNumberOfUsedSlots()
	{
		return PlayerPrefs.GetInt("worlds.slots.used", 0);
	}

	public int GetGlobaWorldsCounter()
	{
		return PlayerPrefs.GetInt("worlds.slots.counter", 0);
	}

	public void IncreaseGlobaWorldsCounter()
	{
		PlayerPrefs.SetInt("worlds.slots.counter", GetGlobaWorldsCounter() + 1);
	}

	public bool CheckIfCurrentAndValid(string id)
	{
		int num = worlds.FindIndex((WorldData world) => world.uniqueId == id);
		return currentWorldDataIndex == num && canReturnToGame;
	}

	public bool TryToSelectWorldById(string id)
	{
		UnityEngine.Debug.Log("Trying to select world: " + id);
		int num = worlds.FindIndex((WorldData world) => world.uniqueId == id);
		if (currentWorldDataIndex == num && canReturnToGame)
		{
			return false;
		}
		currentWorldDataIndex = num;
		worlds[num].timestamp = Misc.GetTimeStampDouble();
		worlds[num].selected = true;
		Engine.SetWorldName(worlds[num].name);
		SetAnimals(id);
		SaveToPrefs();
		return true;
	}

	public void SetAnimals(string id)
	{
		MobsContainer mobsContainer = UnityEngine.Object.FindObjectOfType<MobsContainer>();
		if (!(mobsContainer == null))
		{
			if (downloaders != null && downloaders.ContainsKey(id))
			{
				mobsContainer.SetUpManagerForAnimals(downloaders[id].GetWorldsAnimals());
			}
			else
			{
				mobsContainer.SetUpManagerForAnimals(null);
			}
		}
	}

	public void SetTerrainGeneratorFile(string id)
	{
		CommonTerrainData commonTerrainData = null;
		commonTerrainData = baseTerrainData;
		string currentPath = GetCurrentPath();
		string text = worlds[currentWorldDataIndex].terrainName;
		if (string.IsNullOrEmpty(text))
		{
			text = "terrain";
		}
		currentPath = $"{currentPath}/{text}";
		if (File.Exists(currentPath))
		{
			string json = File.ReadAllText(currentPath);
			commonTerrainData = JsonUtility.FromJson<CommonTerrainData>(json);
		}
		CommonTerrainGenerator.ClearCurrentRamp();
		commonTerrainData.ApplyToCommonGenerator(Engine.EngineInstance.GetComponent<CommonTerrainGenerator>());
		Engine.EngineInstance.GetComponent<CommonTerrainGenerator>().chests.placeProbability = Manager.Get<ModelManager>().chestSettings.GetChestProbablity();
		UnityEngine.Debug.Log("chest prop " + Engine.EngineInstance.GetComponent<CommonTerrainGenerator>().chests.placeProbability);
	}

	public WorldData GetCurrentWorld()
	{
		if (worlds == null || worlds.Count == 0)
		{
			worlds = LoadWorldsFromPrefs();
		}
		return worlds[currentWorldDataIndex];
	}

	public WorldData GetWorldById(string id)
	{
		if (worlds == null)
		{
			return null;
		}
		return worlds.Find((WorldData data) => data.uniqueId.Equals(id));
	}

	public int GetIndexWorldById(string id)
	{
		return worlds.FindIndex((WorldData data) => data.uniqueId.Equals(id));
	}

	public WorldData GetWorldFromModelById(string id)
	{
		if (worldsFromModel == null)
		{
			return null;
		}
		return worldsFromModel.Find((WorldData data) => data.uniqueId.Equals(id));
	}

	public int GetIndexWorldFromModelById(string id)
	{
		return worldsFromModel.FindIndex((WorldData data) => data.uniqueId.Equals(id));
	}

	public bool IsWorldFromResources()
	{
		return worlds[currentWorldDataIndex].resources;
	}

	public void CheckSavingPath()
	{
		if (IsWorldFromResources())
		{
			string currentResourcesPath = Manager.Get<SavedWorldManager>().GetCurrentResourcesPath();
			worlds[currentWorldDataIndex].resources = false;
			Engine.UpdateWorldPath();
			string currentPath = Manager.Get<SavedWorldManager>().GetCurrentPath();
			CopyWorldToNewPath(currentResourcesPath, currentPath);
			SaveToPrefs();
		}
	}

	public void CopyWorldToNewPath(string source, string result)
	{
		if (!Directory.Exists(result))
		{
			Directory.CreateDirectory(result);
		}
		UnityEngine.Object[] array = Resources.LoadAll(source);
		UnityEngine.Object[] array2 = array;
		foreach (UnityEngine.Object @object in array2)
		{
			TextAsset textAsset = @object as TextAsset;
			if (textAsset != null)
			{
				File.WriteAllBytes(Path.Combine(result, textAsset.name) + ".txt", textAsset.bytes);
			}
		}
	}

	public void AddDownloadedWorld(WorldData worldData)
	{
		if (worlds.Find((WorldData data) => data.uniqueId.Equals(worldData.uniqueId)) == null)
		{
			worldsFromModel.RemoveAll((WorldData data) => data.uniqueId.Equals(worldData.uniqueId));
			worlds.Add(worldData);
			SaveToPrefs();
		}
	}

	public string AddNewUserWorld(string name)
	{
		string text = CreateNewWorldIndentifier(name, increaseWorldCount: true);
		worlds.Add(new WorldData(text, name, string.Empty, lastUsed: false, selected: false, double.MaxValue, resources: false, startingWorld: false, string.Empty));
		SaveToPrefs();
		CreateWorldDirectory(worlds.GetLastItem());
		WorldsQuests.AddNewDistribution(text);
		return text;
	}

	public void RenameWorld(string id, string newName)
	{
		int num = worlds.FindIndex((WorldData data) => data.uniqueId.Equals(id));
		if (num >= 0)
		{
			worlds[num].name = newName;
			SaveToPrefs();
		}
	}

	public Texture GetCurrentTilesetTexture()
	{
		Texture texture;
		if (Manager.Contains<GameSkinManager>())
		{
			texture = Manager.Get<GameSkinManager>().GetSkinForLevel(currentWorldDataIndex).tileSet;
		}
		else
		{
			string tilesetName = GetCurrentWorld().tilesetName;
			Uri uri = new Uri($"{worlds[currentWorldDataIndex].GetPath()}/{tilesetName}");
			texture = Misc.LoadPNG(Uri.UnescapeDataString(uri.AbsolutePath));
		}
		if (texture == null)
		{
			UnityEngine.Debug.LogWarning("Tileset for given world not found!");
			return baseEngineTileset;
		}
		VoxelSprite.ClearAllSprites();
		texture.filterMode = FilterMode.Point;
		return texture;
	}

	public Texture GetCurrentNormalmapTexture()
	{
		string tilesetName = GetCurrentWorld().tilesetName;
		if (string.IsNullOrEmpty(tilesetName))
		{
			UnityEngine.Debug.LogWarning("Normalmap for given world not found!");
			return baseEngineNormalmap;
		}
		tilesetName = tilesetName.Insert(tilesetName.Length - 4, "_normalmap");
		Uri uri = new Uri($"{worlds[currentWorldDataIndex].GetPath()}/{tilesetName}");
		Texture texture = Misc.LoadPNG(Uri.UnescapeDataString(uri.AbsolutePath));
		if (texture == null)
		{
			UnityEngine.Debug.LogWarning("Normalmap for given world not found!");
			return baseEngineNormalmap;
		}
		texture.filterMode = FilterMode.Bilinear;
		return texture;
	}

	public Texture GetCurrentLookupTexture()
	{
		string tilesetName = GetCurrentWorld().tilesetName;
		if (string.IsNullOrEmpty(tilesetName))
		{
			UnityEngine.Debug.LogWarning("Lookup for given world not found!");
			return baseEngineLookup;
		}
		tilesetName = tilesetName.Insert(tilesetName.Length - 4, "_lookup");
		Uri uri = new Uri($"{worlds[currentWorldDataIndex].GetPath()}/{tilesetName}");
		Texture texture = Misc.LoadPNG(Uri.UnescapeDataString(uri.AbsolutePath));
		if (texture == null)
		{
			UnityEngine.Debug.LogWarning("Lookup for given world not found!");
			return baseEngineLookup;
		}
		texture.filterMode = FilterMode.Point;
		return texture;
	}

	private void SaveToPrefs()
	{
		WorldsList obj = new WorldsList(worlds);
		PlayerPrefs.SetString("worlds.data", JsonUtility.ToJson(obj));
	}

	public List<WorldData> GetAllWorlds()
	{
		if (worlds == null || worlds.Count == 0)
		{
			worlds = LoadWorldsFromPrefs();
		}
		List<WorldData> list = new List<WorldData>(worlds);
		list.Sort((WorldData a, WorldData b) => (int)(b.timestamp - a.timestamp));
		list[0].selected = true;
		list[0].lastUsed = true;
		for (int i = 1; i < worlds.Count; i++)
		{
			list[i].selected = false;
			list[i].lastUsed = false;
		}
		if (worldsFromModel != null && Manager.Get<ModelManager>().worldsSettings.GetWorldsUltimateSelection())
		{
			list.AddRange(worldsFromModel);
		}
		return list;
	}

	private List<WorldData> LoadWorldsFromPrefs()
	{
		string @string = PlayerPrefs.GetString("worlds.data", string.Empty);
		if (string.IsNullOrEmpty(@string))
		{
			List<WorldData> oldWorldFromData = GetOldWorldFromData();
			if (oldWorldFromData != null && oldWorldFromData.Count > 0)
			{
				return oldWorldFromData;
			}
			if (worldCount <= 1)
			{
				return GetDefaultList();
			}
			return GetDefaultListMutliple(worldCount);
		}
		WorldsList worldsList = JsonUtility.FromJson<WorldsList>(@string);
		if (worldsList.data == null || worldsList.data.Count == 0)
		{
			if (worldCount <= 1)
			{
				return GetDefaultList();
			}
			return GetDefaultListMutliple(worldCount);
		}
		return worldsList.data;
	}

	private List<WorldData> GetDefaultList()
	{
		List<WorldData> list = new List<WorldData>();
		string resourcesPath = Manager.Get<ConnectionInfoManager>().gameName + ((!addIosSuffixToPath) ? string.Empty : ".ios");
		list.Add(new WorldData("xcraft.world1", "The World", resourcesPath, lastUsed: true, selected: true, Misc.GetTimeStampDouble(), resources: true, startingWorld: true, string.Empty));
		Singleton<PlayerData>.get.playerWorlds.OnWorldBought("xcraft.world1");
		return list;
	}

	private List<WorldData> GetDefaultListMutliple(int worldCount)
	{
		List<WorldData> list = new List<WorldData>();
		string resourcesPath = Manager.Get<ConnectionInfoManager>().gameName + ((!addIosSuffixToPath) ? string.Empty : ".ios");
		for (int i = 0; i < worldCount; i++)
		{
			string resourcesSubfolderPath = "world." + i;
			WorldData worldData = new WorldData("xcraft.world1" + i, "Pregenerated World" + i, resourcesPath, lastUsed: true, selected: true, Misc.GetTimeStampDouble(), resources: true, startingWorld: false, resourcesSubfolderPath);
			if (StartingPositions.Count > i)
			{
				worldData.startPosition = StartingPositions[i];
			}
			list.Add(worldData);
			Singleton<PlayerData>.get.playerWorlds.OnWorldBought("xcraft.world1" + i);
		}
		return list;
	}

	private WorldData ApplyCustomPositions()
	{
		if (StartingPositions == null)
		{
			return null;
		}
		if (!forceStartPosition)
		{
			return null;
		}
		if (currentWorldDataIndex >= StartingPositions.Count)
		{
			return null;
		}
		WorldData currentWorld = GetCurrentWorld();
		if (currentWorld == null)
		{
			return null;
		}
		currentWorld.startPosition = StartingPositions[currentWorldDataIndex];
		return currentWorld;
	}

	private List<WorldData> GetOldWorldFromData()
	{
		string path = string.Format("{0}/Worlds/{1}", Application.persistentDataPath, "TestWorld");
		string text = string.Format("{0}/Worlds/{1}", Application.persistentDataPath, "xcraft.world1");
		List<WorldData> list = new List<WorldData>();
		DirectoryInfo directoryInfo = new DirectoryInfo(path);
		if (directoryInfo.Exists)
		{
			Directory.CreateDirectory(text);
			FileInfo[] files = directoryInfo.GetFiles();
			FileInfo[] array = files;
			foreach (FileInfo fileInfo in array)
			{
				fileInfo.MoveTo(text + "/" + fileInfo.Name);
			}
			list.Add(new WorldData("xcraft.world1", "The World", string.Empty, lastUsed: true, selected: true, Misc.GetTimeStampDouble(), resources: false, startingWorld: false, string.Empty));
			SaveToPrefs();
			Singleton<PlayerData>.get.playerWorlds.OnWorldBought("xcraft.world1");
		}
		return list;
	}

	public void SaveWorldAndData()
	{
		worlds[currentWorldDataIndex].savedAtStart = true;
		Engine.SaveWorldInstant();
		Manager.Get<GameCallbacksManager>().FrequentSave();
		WorldPlayerPrefs.get.Save();
		SaveToPrefs();
	}

	public void OnPlayWorld(string id)
	{
		canReturnToGame = true;
		if (Manager.Get<ModelManager>().worldsSettings.IsThisGameUltimate())
		{
			Manager.Get<StatsManager>().OnWorldEntered(id, Manager.Get<UltimateSoftCurrencyManager>().GetCurrencyAmount());
		}
		Manager.Get<StatsManager>().CommonWorldEntered(GetWorldCounterFromId(id));
	}

	public int GetWorldCounterFromId(string id)
	{
		string[] array = id.Split('_');
		if (array.Length < 4)
		{
			return 0;
		}
		if (int.TryParse(array[array.Length - 3], NumberStyles.Any, CultureInfo.InvariantCulture, out int result))
		{
			return result;
		}
		return int.MaxValue;
	}

	public void CheckIfMakeScreenShoot()
	{
		StartCoroutine(MakeScreenshot(worlds[currentWorldDataIndex].uniqueId));
	}

	public IEnumerator MakeScreenshot(string name)
	{
		yield return new WaitForSeconds(5f);
		while (!(Time.timeScale > 0f) || !Manager.Get<StateMachineManager>().IsCurrentStateA<GameplayState>())
		{
			yield return new WaitForSeconds(0.1f);
		}
		CreateCamera();
		worlds[currentWorldDataIndex].screenWasMade = true;
		SaveToPrefs();
	}

	public string GetNameOfNewWorld()
	{
		return string.Format("{0} {1}", Manager.Get<TranslationsManager>().GetText("worlds.newname", "My World"), GetNumberOfUsedSlots() + 1);
	}

	private int getWidthOfImage()
	{
		return Mathf.RoundToInt((float)Screen.width * 0.33f);
	}

	private int getHeightOfImage()
	{
		return Mathf.RoundToInt((float)Screen.height * 0.33f);
	}

	private void CreateCamera()
	{
		GameObject gameObject = new GameObject("newCamera");
		Camera camera = gameObject.AddComponent<Camera>();
		camera.depth = -1f;
		camera.CopyFrom(CameraController.instance.MainCamera);
		gameObject.AddComponent<ScreenShotMaker>().TakeScreenshoot(getWidthOfImage(), getHeightOfImage(), worlds[currentWorldDataIndex].GetImagePath());
	}

	private static void RemoveCurrentWorld()
	{
		if (Engine.EngineInstance != null)
		{
			Engine.EngineInstance.StopAllCoroutines();
		}
		Chunk[] array = UnityEngine.Object.FindObjectsOfType<Chunk>();
		foreach (Chunk chunk in array)
		{
			UnityEngine.Object.Destroy(chunk.gameObject);
		}
		ChunkAnimation[] array2 = UnityEngine.Object.FindObjectsOfType<ChunkAnimation>();
		foreach (ChunkAnimation chunkAnimation in array2)
		{
			UnityEngine.Object.Destroy(chunkAnimation.gameObject);
		}
		ChunkManager.ResetPool();
		if (Manager.Contains<MobsManager>())
		{
			Manager.Get<MobsManager>().DespawnAll();
		}
		if (Manager.Contains<SaveTransformsManager>())
		{
			Manager.Get<SaveTransformsManager>().DespawnAll();
		}
		Manager.Get<GameCallbacksManager>().Restart();
	}

	public static void UnloadCraftGameplay()
	{
		Engine.EngineInstance.StopAllCoroutines();
		Chunk[] array = UnityEngine.Object.FindObjectsOfType<Chunk>();
		foreach (Chunk chunk in array)
		{
			UnityEngine.Object.Destroy(chunk.gameObject);
		}
		ChunkAnimation[] array2 = UnityEngine.Object.FindObjectsOfType<ChunkAnimation>();
		foreach (ChunkAnimation chunkAnimation in array2)
		{
			UnityEngine.Object.Destroy(chunkAnimation.gameObject);
		}
		ChunkManager.ResetPool();
		if (Manager.Contains<MobsManager>())
		{
			Manager.Get<MobsManager>().DespawnAll();
		}
		if (Manager.Contains<SaveTransformsManager>())
		{
			Manager.Get<SaveTransformsManager>().DespawnAll();
		}
		Mob[] array3 = UnityEngine.Object.FindObjectsOfType<Mob>();
		foreach (Mob mob in array3)
		{
			UnityEngine.Object.Destroy(mob.gameObject);
		}
		Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
		SceneManager.UnloadScene("Gameplay");
		Manager.Get<GameCallbacksManager>().Restart();
	}

	public static void ResetCurrentWorld(bool shouldSave = false)
	{
		PlayerGraphic controlledPlayerInstance = PlayerGraphic.GetControlledPlayerInstance();
		PlayerController playerController = null;
		if (controlledPlayerInstance != null)
		{
			playerController = controlledPlayerInstance.GetComponentInParent<PlayerController>();
		}
		if (playerController != null)
		{
			playerController.OnWorldReset();
		}
		RemoveCurrentWorld();
		Engine.ResetWorld();
		Manager.Get<StateMachineManager>().PopStatesUntil<GameplayState>();
		SceneManager.UnloadScene("Gameplay");
		Manager.Get<StateMachineManager>().SetState<LoadLevelState>((!shouldSave) ? null : new ShouldSaveParameter(save: true));
	}

	public void OnGameplayStarted()
	{
		if (baseEngineTileset == null)
		{
			baseEngineTileset = Engine.GetTilesetTexture();
		}
		if (baseEngineNormalmap == null)
		{
			baseEngineNormalmap = Engine.GetNormalmapTexture();
		}
		if (baseEngineLookup == null)
		{
			baseEngineLookup = Engine.GetLookupTexture();
		}
		if (baseTerrainData == null)
		{
			baseTerrainData = new CommonTerrainData(Engine.EngineInstance.GetComponent<CommonTerrainGenerator>());
		}
		Engine.SetTileset(Manager.Get<SavedWorldManager>().GetCurrentTilesetTexture(), Manager.Get<SavedWorldManager>().GetCurrentNormalmapTexture(), Manager.Get<SavedWorldManager>().GetCurrentLookupTexture());
		SetTerrainGeneratorFile(GetCurrentWorld().uniqueId);
		SetAnimals(GetCurrentWorld().uniqueId);
		ApplyCustomPositions();
		SetEnginePlayerPosition();
		InformManagersOnChange();
		EnableMovement();
		CheckSavingPath();
		ChunkDataFiles.Init();
	}

	private void InformManagersOnChange()
	{
		if (Manager.Contains<SaveTransformsManager>())
		{
			Manager.Get<SaveTransformsManager>().OnGameplayRestarted();
		}
		if (Manager.Contains<QuestManager>())
		{
			Manager.Get<QuestManager>().OnWorldChange(GetCurrentWorld().uniqueId);
		}
		if (Manager.Get<StatsManager>().TryToGetReporter(out FPSStatReporter reporter))
		{
			reporter.SetAdditionalData(GetCurrentWorld().uniqueId);
		}
	}

	private void SetEnginePlayerPosition()
	{
		WorldData currentWorld = GetCurrentWorld();
		Vector3? startPosition = currentWorld.startPosition;
		if (startPosition.HasValue)
		{
			Engine engineInstance = Engine.EngineInstance;
			Vector3? startPosition2 = GetCurrentWorld().startPosition;
			engineInstance.startPlayerPosition = startPosition2.Value;
		}
		float? startRot = GetCurrentWorld().startRot;
		if (startRot.HasValue)
		{
			Engine engineInstance2 = Engine.EngineInstance;
			float? startRot2 = GetCurrentWorld().startRot;
			engineInstance2.startPlayerYRotation = startRot2.Value;
		}
	}

	private void EnableMovement()
	{
		if (Manager.Get<StateMachineManager>().IsCurrentStateA<GameplayState>())
		{
			(Manager.Get<StateMachineManager>().currentState as GameplayState).EnableUserMovement();
		}
	}

	public void OnGameSavedFrequent()
	{
	}

	public void OnGameSavedInfrequent()
	{
	}

	public void OnGameplayRestarted()
	{
	}
}
