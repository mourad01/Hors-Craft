// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.Engine
using Common.Managers;
using Common.Utils;
using Gameplay;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Uniblocks
{
	public class Engine : MonoBehaviour
	{
		public AnimationCurve chunkSpawnAnimation;

		public AnimationCurve chunkDestroyAnimation;

		public float animationLength;

		public static bool doAnimate = true;

		public static string WorldName;

		public string lWorldName = "TestWorld";

		public string lBlocksPath;

		public MeshingAlgorithm meshingAlgorithm;

		private static string _WorldPath;

		public Vector3 startPlayerPosition = new Vector3(0f, 50f, 0f);

		public float startPlayerYRotation;

		public VehicleController startPlayerVehiclePrefab;

		public Mountable startPlayerMountablePrefab;

		public bool startPlayerNearestInteractiveObject;

		public static Voxel[] Blocks;

		public VoxelPrefabInfo[] lVoxels;

		public Dictionary<Voxel, int> voxelToPriority;

		public Dictionary<Voxel, int> voxelToRarityPriority;

		public List<Voxel> hiddenVoxel = new List<Voxel>();

		public bool raritiesMode;

		public static int HeightRange;

		public static int ChunkSideLength;

		public static int ChunkSpawnDistance;

		public static int ChunkDespawnDistance;

		public int lHeightRange;

		public int lChunkSpawnDistanceInEditor = 80;

		public int lChunkSpawnDistanceInPlayer = 7;

		public int lChunkSideLength;

		public int lChunkDespawnDistance;

		public static float TextureUnit;

		public static float TexturePadding;

		public float lTextureUnit;

		public float lTexturePadding;

		public static int MinTargetFPS;

		public static int MaxTargetFPS;

		public static int MaxChunkSaves;

		public static int MaxChunkDataRequests;

		public int lMinTargetFPS;

		public int lMaxTargetFPS;

		public int lMaxChunkSaves;

		public int lMaxChunkDataRequests;

		public static bool SendCameraLookEvents;

		public static bool SendCursorEvents;

		public static bool EnableMultiplayer;

		public static bool MultiplayerTrackPosition;

		public static bool SaveVoxelData;

		public static bool GenerateMeshes;

		public bool lSendCameraLookEvents;

		public bool lSendCursorEvents;

		public bool lEnableMultiplayer;

		public bool lMultiplayerTrackPosition;

		public bool lSaveVoxelData;

		public bool lGenerateMeshes;

		public static float ChunkTimeout;

		public float lChunkTimeout;

		public static bool EnableChunkTimeout;

		public static int SquaredSideLength;

		public static GameObject UniblocksNetwork;

		public static Engine EngineInstance;

		public static ChunkManager ChunkManagerInstance;

		public static Vector3 ChunkScale;

		public static bool Initialized;

		public static bool EditMode;

		public static TerrainGenerator TerrainGenerator;

		public static IMeshCreator MeshCreator;

		public bool saveMeshRun;

		public static List<ushort> NotDestroyableBlocks;

		public List<ushort> notDistroyableBlocks = new List<ushort>();

		private Dictionary<Voxel, Voxel.Category> toRestoreCategories = new Dictionary<Voxel, Voxel.Category>();

		private static int loadedWorldSeed;

		private static ChunkData chunkPositionToVoxelInfo;

		public static string WorldPath
		{
			get
			{
				return _WorldPath;
			}
			set
			{
				_WorldPath = value;
			}
		}

		public static CommonTerrainGenerator.OtherIDsConfig usefulIDs
		{
			get;
			private set;
		}

		public static int WorldSeed
		{
			get
			{
				if (loadedWorldSeed == 0)
				{
					FilesLoader filesLoader = new FilesLoader();
					if (filesLoader.SeedExists())
					{
						loadedWorldSeed = filesLoader.LoadSeed();
					}
					if (!filesLoader.SeedExists() || loadedWorldSeed == 0)
					{
						loadedWorldSeed = UnityEngine.Random.Range(1, 65535);
					}
				}
				return loadedWorldSeed;
			}
		}

		public void Awake()
		{
			EngineInstance = this;
			ChunkManagerInstance = GetComponent<ChunkManager>();
			NotDestroyableBlocks = notDistroyableBlocks;
			InitUsefulIDs();
			InitHiddenBlocks();
			InitPathAndPrefs();
			InitBlocksArray();
			InitTargetFPS();
			InitVariables();
			InitChunkLoaders();
			CheckInit();
			InitMeshCreator();
			EditorCheck();
			SetVoxelsMaterialName();
			Initialized = true;
			if (Application.isPlaying)
			{
				ChunkManagerInstance.AwakeFormEngine();
			}
		}

		public void InitForRenderingCustomSprites()
		{
			InitBlocksArray();
		}

		public void InitForRenderingBasicBlocks()
		{
			EngineInstance = this;
			ChunkManagerInstance = GetComponent<ChunkManager>();
			NotDestroyableBlocks = notDistroyableBlocks;
			InitUsefulIDs();
			InitHiddenBlocks();
			InitPathAndPrefs();
			InitBlocksArray();
			InitTargetFPS();
			InitVariables();
			InitChunkLoaders();
			CheckInit();
			InitMeshCreator();
			EditorCheck();
			SetVoxelsMaterialName();
			Initialized = true;
		}

		private void InitPathAndPrefs()
		{
			WorldName = lWorldName;
			if (Application.isPlaying && Manager.Contains<SavedWorldManager>())
			{
				UpdateWorldPath();
				WorldPlayerPrefs.get.Load();
			}
		}

		private void InitBlocksArray()
		{
			Dictionary<int, Voxel> dictionary = new Dictionary<int, Voxel>();
			voxelToPriority = new Dictionary<Voxel, int>();
			voxelToRarityPriority = new Dictionary<Voxel, int>();
			toRestoreCategories.Clear();
			int num = 0;
			Vector2[] vTexture = new Vector2[1]
			{
				new Vector2(0f, 0f)
			};
			for (int i = 0; i < lVoxels.Length; i++)
			{
				Voxel component = lVoxels[i].prefab.GetComponent<Voxel>();
				if (lVoxels[i].sprite != null)
				{
					component.voxelSprite = lVoxels[i].sprite;
				}
				if (component.editorOnly)
				{
					component.VTransparency = Transparency.semiTransparent;
					component.VColliderType = ColliderType.none;
					component.VTexture = vTexture;
				}
				if (hiddenVoxel.Contains(component))
				{
					toRestoreCategories.Add(component, component.blockCategory);
					component.blockCategory = Voxel.Category.none;
				}
				dictionary.Add(component.GetUniqueID(), component);
				voxelToPriority[component] = lVoxels[i].priority;
				voxelToRarityPriority[component] = lVoxels[i].rarityPriority;
				num = Mathf.Max(num, component.GetUniqueID());
			}
			Blocks = new Voxel[num + 1];
			for (int j = 0; j < num + 1; j++)
			{
				if (!dictionary.TryGetValue(j, out Blocks[j]))
				{
					Blocks[j] = dictionary[0];
				}
			}
			UberTexturesController component2 = GetComponent<UberTexturesController>();
			if (component2 != null)
			{
				component2.SetupVoxels(dictionary);
			}
			dictionary = null;
		}

		private void InitTargetFPS()
		{
			if (Application.isPlaying && !saveMeshRun && Manager.Contains<ModelManager>())
			{
				if (Manager.Get<ModelManager>().engineSettings.GetEnableEngineSettings())
				{
					MinTargetFPS = Manager.Get<ModelManager>().engineSettings.GetEngineMinTargetFPS();
				}
				else
				{
					MinTargetFPS = lMinTargetFPS;
				}
			}
		}

		private void InitVariables()
		{
			MaxTargetFPS = lMaxTargetFPS;
			MaxChunkSaves = lMaxChunkSaves;
			MaxChunkDataRequests = lMaxChunkDataRequests;
			TextureUnit = lTextureUnit;
			TexturePadding = lTexturePadding;
			EnableMultiplayer = lEnableMultiplayer;
			MultiplayerTrackPosition = lMultiplayerTrackPosition;
			SaveVoxelData = lSaveVoxelData;
			GenerateMeshes = lGenerateMeshes;
			TerrainGenerator = GetComponent<TerrainGenerator>();
			ChunkSpawnDistance = lChunkSpawnDistanceInPlayer;
			HeightRange = lHeightRange;
			ChunkDespawnDistance = lChunkDespawnDistance;
			SendCameraLookEvents = lSendCameraLookEvents;
			SendCursorEvents = lSendCursorEvents;
			ChunkData.SideLength = (ChunkSideLength = lChunkSideLength);
			ChunkData.SquaredSideLength = (SquaredSideLength = lChunkSideLength * lChunkSideLength);
			if (ChunkManagerInstance.tileset3d != null)
			{
				ChunkManagerInstance.tileset3d.GetComponent<Tileset3D>().Init();
			}
		}

		private void InitChunkLoaders()
		{
			if (lChunkTimeout <= 1E-05f)
			{
				EnableChunkTimeout = false;
			}
			else
			{
				EnableChunkTimeout = true;
				ChunkTimeout = lChunkTimeout;
			}
			if (Application.platform == RuntimePlatform.WebGLPlayer)
			{
				lSaveVoxelData = false;
				SaveVoxelData = false;
			}
		}

		private void InitUsefulIDs()
		{
			if (TerrainGenerator == null)
			{
				TerrainGenerator = GetComponent<TerrainGenerator>();
			}
			CommonTerrainGenerator commonTerrainGenerator = TerrainGenerator as CommonTerrainGenerator;
			if (commonTerrainGenerator != null)
			{
				usefulIDs = commonTerrainGenerator.otherIDs;
				return;
			}
			CommonTerrainGenerator.OtherIDsConfig otherIDsConfig = new CommonTerrainGenerator.OtherIDsConfig();
			otherIDsConfig.waterBlockID = 12;
			otherIDsConfig.dirtBlockID = 1;
			otherIDsConfig.stoneBlockID = 5;
			otherIDsConfig.grassBlockID = 2;
			otherIDsConfig.beachBlockID = 101;
			otherIDsConfig.snowBlockID = 41;
			otherIDsConfig.blueprintID = 1118;
			usefulIDs = otherIDsConfig;
		}

		private void CheckInit()
		{
			if (Blocks.Length < 1)
			{
				UnityEngine.Debug.LogError("Uniblocks: The blocks array is empty! Use the Block Editor to update the blocks array.");
				UnityEngine.Debug.Break();
			}
			if (Blocks[0] == null)
			{
				UnityEngine.Debug.LogError("Uniblocks: Cannot find the empty block prefab (id 0)!");
				UnityEngine.Debug.Break();
			}
			else if (Blocks[0].GetComponent<Voxel>() == null)
			{
				UnityEngine.Debug.LogError("Uniblocks: Voxel id 0 does not have the Voxel component attached!");
				UnityEngine.Debug.Break();
			}
			if (ChunkData.SideLength < 1)
			{
				UnityEngine.Debug.LogError("Uniblocks: Chunk side length must be greater than 0!");
				UnityEngine.Debug.Break();
			}
			if (ChunkSpawnDistance < 1)
			{
				ChunkSpawnDistance = 0;
				UnityEngine.Debug.LogWarning("Uniblocks: Chunk spawn distance is 0. No chunks will spawn!");
			}
			if (HeightRange < 0)
			{
				HeightRange = 0;
				UnityEngine.Debug.LogWarning("Uniblocks: Chunk height range can't be a negative number! Setting chunk height range to 0.");
			}
			if (MaxChunkDataRequests < 0)
			{
				MaxChunkDataRequests = 0;
				UnityEngine.Debug.LogWarning("Uniblocks: Max chunk data requests can't be a negative number! Setting max chunk data requests to 0.");
			}
			GetComponent<ChunkManager>().InitMaterials(Blocks);
			GameObject chunkObject = GetComponent<ChunkManager>().ChunkObject;
			int num = chunkObject.GetComponent<Renderer>().sharedMaterials.Length - 1;
			for (ushort num2 = 0; num2 < Blocks.Length; num2 = (ushort)(num2 + 1))
			{
				if (Blocks[num2] != null)
				{
					Voxel component = Blocks[num2].GetComponent<Voxel>();
					if (component.VSubmeshIndex < 0)
					{
						UnityEngine.Debug.LogError("Uniblocks: Voxel " + num2 + " has a material index lower than 0! Material index must be 0 or greater.");
						UnityEngine.Debug.Break();
					}
					if (component.VSubmeshIndex > num)
					{
						UnityEngine.Debug.LogError("Uniblocks: Voxel " + num2 + " uses material index " + component.VSubmeshIndex + ", but the chunk prefab only has " + (num + 1) + " material(s) attached. Set a lower material index or attach more materials to the chunk prefab.");
						UnityEngine.Debug.Break();
					}
				}
			}
			if (QualitySettings.antiAliasing > 0)
			{
				UnityEngine.Debug.LogWarning("Uniblocks: Anti-aliasing is enabled. This may cause seam lines to appear between blocks. If you see lines between blocks, try disabling anti-aliasing, switching to deferred rendering path, or adding some texture padding in the engine settings.");
			}
			if (MinTargetFPS < 20 || MinTargetFPS > 40)
			{
				UnityEngine.Debug.LogWarning("Min target FPS has strange value! It may cause too slow chunks spawning or too little FPS.");
			}
			if (MaxTargetFPS < 30 || MaxTargetFPS > 80)
			{
				UnityEngine.Debug.LogWarning("Max target FPS has strange value! It may cause too fast chunks spawning far away from player or no chunks spawning far away from player.");
			}
		}

		private void InitMeshCreator()
		{
			if (meshingAlgorithm == MeshingAlgorithm.GREEDY)
			{
				if (GetComponent<ChunkMeshCreator>() != null)
				{
					UnityEngine.Object.Destroy(GetComponent<ChunkMeshCreator>());
				}
				GreedyMeshCreator component = GetComponent<GreedyMeshCreator>();
				if (component == null)
				{
					MeshCreator = base.gameObject.AddComponent<GreedyMeshCreator>();
				}
				else
				{
					MeshCreator = component;
				}
			}
			else
			{
				if (GetComponent<GreedyMeshCreator>() != null)
				{
					UnityEngine.Object.Destroy(GetComponent<GreedyMeshCreator>());
				}
				ChunkMeshCreator component2 = GetComponent<ChunkMeshCreator>();
				if (component2 == null)
				{
					MeshCreator = base.gameObject.AddComponent<ChunkMeshCreator>();
				}
				else
				{
					MeshCreator = component2;
				}
			}
			MeshCreator.Init();
			MeshCreator.InitSubmeshArrays(ChunkManagerInstance.ChunkObject.GetComponent<MeshRenderer>().sharedMaterials, ChunkManagerInstance.ChunkObject.GetComponent<Chunk>());
		}

		private void InitHiddenBlocks()
		{
			if (!Application.isPlaying || !Manager.Contains<ModelManager>())
			{
				return;
			}
			int num = (int)(Manager.Get<ModelManager>().configSettings.GetHiddenBlocksValue() * (float)lVoxels.Length);
			if (num == 0)
			{
				return;
			}
			List<ushort> forbiddenIDs = new List<ushort>
			{
				usefulIDs.waterBlockID,
				usefulIDs.dirtBlockID,
				usefulIDs.beachBlockID,
				usefulIDs.stoneBlockID,
				usefulIDs.roadBlockID,
				usefulIDs.roadLineBlockID,
				usefulIDs.snowBlockID,
				usefulIDs.grassBlockID,
				usefulIDs.iceBlockID
			};
			List<Voxel> list = (from lv in lVoxels
				select lv.prefab.GetComponent<Voxel>() into v
				where !forbiddenIDs.Contains(v.GetUniqueID())
				select v).ToList();
			HashSet<int> hashSet = new HashSet<int>();
			DeterministicRNG deterministicRNG = new DeterministicRNG(420);
			for (int i = 0; i < num; i++)
			{
				int b = deterministicRNG.Next(0, list.Count);
				b = Mathf.Min(list.Count - 1, b);
				if (!hashSet.Add(b))
				{
					i--;
				}
				else
				{
					hiddenVoxel.Add(list[b]);
				}
			}
		}

		private void SetVoxelsMaterialName()
		{
			for (int i = 0; i < Blocks.Length; i++)
			{
				if (!(Blocks[i].VMaterial == null))
				{
					Blocks[i].materialName = Blocks[i].VMaterial.name;
				}
			}
		}

		public static void UpdateWorldPath()
		{
			try
			{
				WorldPath = Manager.Get<SavedWorldManager>().GetCurrentPath();
			}
			catch (Exception arg)
			{
				UnityEngine.Debug.LogWarning("Can't access Saved World Manager: " + arg);
			}
		}

		public static void SetWorldName(string worldName)
		{
			WorldName = worldName;
			UpdateWorldPath();
		}

		public static int GetLoadedSeed()
		{
			return loadedWorldSeed;
		}

		public static void ResetWorld()
		{
			loadedWorldSeed = 0;
		}

		private static void CreateWorldDirectory()
		{
			Directory.CreateDirectory(WorldPath);
		}

		public static void StopAllCoorutines()
		{
			EngineInstance.StopAllCoroutines();
		}

		public static void SaveWorld()
		{
			if (!(EngineInstance == null))
			{
				Manager.Get<SavedWorldManager>().CheckSavingPath();
				EngineInstance.StartCoroutine(ChunkDataFiles.SaveAllChunks());
				FilesLoader filesLoader = new FilesLoader();
				filesLoader.SaveSeed(WorldSeed);
			}
		}

		public static void SaveWorldInstant()
		{
			Manager.Get<SavedWorldManager>().CheckSavingPath();
			ChunkDataFiles.SaveAllChunksInstant();
			FilesLoader filesLoader = new FilesLoader();
			filesLoader.SaveSeed(WorldSeed);
		}

		private static void SaveScreenShoot()
		{
		}

		public static GameObject GetVoxelGameObject(ushort voxelId)
		{
			if (voxelId == ushort.MaxValue)
			{
				voxelId = 0;
			}
			return Blocks[voxelId].gameObject;
		}

		public static Voxel GetVoxelType(ushort voxelId)
		{
			if (voxelId >= Blocks.Length)
			{
				return Blocks[0];
			}
			return Blocks[voxelId];
		}

		public static Voxel GetVoxelType(ushort voxelId, Vector3 position)
		{
			if (voxelId >= Blocks.Length)
			{
				return Blocks[0];
			}
			Blocks[voxelId].PrepareVoxel(position);
			return Blocks[voxelId];
		}

		public static VoxelInfo VoxelRaycast(Vector3 origin, Vector3 direction, float range, bool ignoreTransparent)
		{
			int mask = LayerMask.GetMask("Terrain");
			RaycastHit hitInfo = default(RaycastHit);
			if (Physics.Raycast(origin, direction, out hitInfo, range, mask) && hitInfo.collider.GetComponentInParent<Chunk>() != null)
			{
				GameObject gameObject = hitInfo.collider.gameObject;
				Chunk componentInParent = gameObject.GetComponentInParent<Chunk>();
				if (gameObject.GetComponent<Chunk>() == null)
				{
					gameObject = gameObject.transform.parent.gameObject;
				}
				Index index = componentInParent.chunkData.PositionToVoxelIndex(hitInfo.point, hitInfo.normal, returnAdjacent: false);
				if (ignoreTransparent)
				{
					ushort voxel = componentInParent.chunkData.GetVoxel(index.x, index.y, index.z);
					if (GetVoxelType(voxel).VTransparency != 0)
					{
						Vector3 point = hitInfo.point;
						point.y -= 0.5f;
						return VoxelRaycast(point, Vector3.down, range - hitInfo.distance, ignoreTransparent: false);
					}
				}
				return new VoxelInfo(componentInParent.chunkData.PositionToVoxelIndex(hitInfo.point, hitInfo.normal, returnAdjacent: false), componentInParent.chunkData.PositionToVoxelIndex(hitInfo.point, hitInfo.normal, returnAdjacent: true), componentInParent.chunkData);
			}
			return null;
		}

		public static VoxelInfo VoxelRaycast(Ray ray, float range, bool ignoreTransparent)
		{
			return VoxelRaycast(ray.origin, ray.direction, range, ignoreTransparent);
		}

		public static Index PositionToIndex(Vector3 position)
		{
			return new Index(Mathf.FloorToInt((float)Mathf.RoundToInt(position.x / ChunkScale.x) / (float)ChunkData.SideLength), Mathf.FloorToInt((float)Mathf.RoundToInt(position.y / ChunkScale.y) / (float)ChunkData.SideLength), Mathf.FloorToInt((float)Mathf.RoundToInt(position.z / ChunkScale.z) / (float)ChunkData.SideLength));
		}

		public static GameObject PositionToChunk(Vector3 position)
		{
			Index index = PositionToIndex(position);
			return ChunkManager.GetChunk(index);
		}

		public static Chunk PositionToChunkComponent(Vector3 position)
		{
			Index key = PositionToIndex(position);
			Chunk value = null;
			ChunkManager.Chunks.TryGetValue(key, out value);
			return value;
		}

		public static ChunkData PositionToChunkData(Vector3 position)
		{
			Index key = PositionToIndex(position);
			ChunkData value = null;
			ChunkManager.ChunkDatas.TryGetValue(key, out value);
			return value;
		}

		public static VoxelInfo PositionToVoxelInfo(float x, float y, float z)
		{
			return PositionToVoxelInfo(new Vector3(x, y, z));
		}

		public static VoxelInfo PositionToVoxelInfo(Vector3 position)
		{
			Index key = PositionToIndex(position);
			chunkPositionToVoxelInfo = null;
			ChunkManager.ChunkDatas.TryGetValue(key, out chunkPositionToVoxelInfo);
			if (chunkPositionToVoxelInfo != null)
			{
				Index setIndex = chunkPositionToVoxelInfo.PositionToVoxelIndex(position);
				return new VoxelInfo(setIndex, chunkPositionToVoxelInfo);
			}
			return null;
		}

		public static Vector3 VoxelInfoToPosition(VoxelInfo voxelInfo)
		{
			return voxelInfo.chunk.VoxelIndexToPosition(voxelInfo.index);
		}

		public static VoxelInfo VoxelGridRaycast(Ray ray, float distance)
		{
			return VoxelGridRaycast(ray.origin, ray.direction, distance);
		}

		public static VoxelInfo VoxelGridRaycast(Vector3 pos, Vector3 direction, float distance)
		{
			Func<List<VoxelInfo>, bool> endCondition = (List<VoxelInfo> voxelList) => voxelList != null && voxelList.Count > 0 && voxelList[voxelList.Count - 1].GetVoxel() != 0;
			List<VoxelInfo> source = VoxelGridRaycast(pos, direction, distance, endCondition);
			return source.LastOrDefault();
		}

		public static VoxelInfo VoxelGridRaycastIgnoreWater(Vector3 pos, Vector3 direction, float distance)
		{
			Func<List<VoxelInfo>, bool> endCondition = (List<VoxelInfo> voxelList) => voxelList != null && voxelList.Count > 0 && voxelList[voxelList.Count - 1].GetVoxel() != 0 && voxelList[voxelList.Count - 1].GetVoxel() != usefulIDs.waterBlockID;
			List<VoxelInfo> source = VoxelGridRaycast(pos, direction, distance, endCondition);
			return source.LastOrDefault();
		}

		public static VoxelInfo VoxelGridRaycastFor(Vector3 pos, Vector3 direction, float distance, ushort targetVoxel)
		{
			Func<List<VoxelInfo>, bool> endCondition = (List<VoxelInfo> voxelList) => voxelList != null && voxelList.Count > 0 && voxelList[voxelList.Count - 1].GetVoxel() == targetVoxel;
			List<VoxelInfo> source = VoxelGridRaycast(pos, direction, distance, endCondition);
			VoxelInfo voxelInfo = source.LastOrDefault();
			return (voxelInfo == null || voxelInfo.GetVoxel() != targetVoxel) ? null : voxelInfo;
		}

		public static List<VoxelInfo> VoxelGridRaycastAll(Vector3 pos, Vector3 direction, float distance)
		{
			return VoxelGridRaycast(pos, direction, distance, (List<VoxelInfo> vl) => false);
		}

		public static List<VoxelInfo> VoxelGridRaycast(Vector3 pos, Vector3 direction, float distance, Func<List<VoxelInfo>, bool> endCondition)
		{
			if (direction.sqrMagnitude != 1f)
			{
				direction.Normalize();
			}
			int num = Mathf.RoundToInt(pos.x);
			int num2 = Mathf.RoundToInt(pos.y);
			int num3 = Mathf.RoundToInt(pos.z);
			int num4 = (int)Mathf.Sign(direction.x);
			int num5 = (int)Mathf.Sign(direction.y);
			int num6 = (int)Mathf.Sign(direction.z);
			float num7 = (num4 <= 0) ? (Mathf.Floor(pos.x + 0.5f) - 0.5f - pos.x) : (Mathf.Ceil(pos.x - 0.5f) + 0.5f - pos.x);
			float num8 = (num5 <= 0) ? (Mathf.Floor(pos.y + 0.5f) - 0.5f - pos.y) : (Mathf.Ceil(pos.y - 0.5f) + 0.5f - pos.y);
			float num9 = (num6 <= 0) ? (Mathf.Floor(pos.z + 0.5f) - 0.5f - pos.z) : (Mathf.Ceil(pos.z - 0.5f) + 0.5f - pos.z);
			float num10 = num7 / (distance * direction.x);
			float num11 = num8 / (distance * direction.y);
			float num12 = num9 / (distance * direction.z);
			if (float.IsNaN(num10) || float.IsNegativeInfinity(num10))
			{
				num10 = float.PositiveInfinity;
			}
			if (float.IsNaN(num11) || float.IsNegativeInfinity(num11))
			{
				num11 = float.PositiveInfinity;
			}
			if (float.IsNaN(num12) || float.IsNegativeInfinity(num12))
			{
				num12 = float.PositiveInfinity;
			}
			float num13 = 1f / (distance * Mathf.Abs(direction.x));
			float num14 = 1f / (distance * Mathf.Abs(direction.y));
			float num15 = 1f / (distance * Mathf.Abs(direction.z));
			bool flag = false;
			List<VoxelInfo> list = new List<VoxelInfo>();
			VoxelInfo voxelInfo = PositionToVoxelInfo(num, num2, num3);
			if (voxelInfo != null)
			{
				list.Add(voxelInfo);
			}
			while (!endCondition(list) && !flag)
			{
				if (num10 < num11)
				{
					if (num10 < num12)
					{
						num += num4;
						num10 += num13;
					}
					else
					{
						num3 += num6;
						num12 += num15;
					}
				}
				else if (num11 < num12)
				{
					num2 += num5;
					num11 += num14;
				}
				else
				{
					num3 += num6;
					num12 += num15;
				}
				voxelInfo = PositionToVoxelInfo(num, num2, num3);
				if (voxelInfo != null)
				{
					list.Add(voxelInfo);
				}
				if (double.IsNaN(num10) || double.IsNaN(num11) || double.IsNaN(num12))
				{
					flag = true;
				}
				if (num10 > 1f && num11 > 1f && num12 > 1f)
				{
					flag = true;
				}
			}
			return list;
		}

		public static Vector2 GetTextureOffset(ushort voxel, Facing facing)
		{
			return GetTextureOffsetInt(voxel, (int)facing);
		}

		public static Vector2 GetTextureOffsetInt(ushort voxel, int facing)
		{
			Voxel voxelType = GetVoxelType(voxel);
			return GetTextureOffset(voxelType, facing);
		}

		public static Vector2 GetTextureOffset(Voxel voxelType, int facing)
		{
			Vector2[] vTexture = voxelType.VTexture;
			if (vTexture == null || vTexture.Length == 0)
			{
				return new Vector2(0f, 0f);
			}
			if (!voxelType.VCustomSides)
			{
				return vTexture[0];
			}
			if (facing > vTexture.Length - 1)
			{
				return vTexture[vTexture.Length - 1];
			}
			return vTexture[facing];
		}

		public static Texture2D GetTilesetTexture(int textureNumber = 0)
		{
			return ChunkManagerInstance.tileset as Texture2D;
		}

		public static Texture2D GetNormalmapTexture(int textureNumber = 0)
		{
			return ChunkManagerInstance.normalMap as Texture2D;
		}

		public static Texture2D GetLookupTexture(int textureNumber = 0)
		{
			return ChunkManagerInstance.lookup as Texture2D;
		}

		public static Material GetChunkMaterial(int textureNumber = 0)
		{
			return ChunkManagerInstance.ChunkObject.GetComponent<Renderer>().sharedMaterials[textureNumber];
		}

		public static void SetTileset(Texture texture, Texture normalMap = null, Texture lookup = null)
		{
			ChunkManagerInstance.SetTileset(texture, normalMap, lookup);
		}

		private void EditorCheck()
		{
		}
	}
}
