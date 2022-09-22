// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.ChunkManager
using Common.Managers;
using Common.Utils;
using Gameplay;
using LimitWorld;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Uniblocks
{
	public class ChunkManager : MonoBehaviour, IGameCallbacksListener
	{
		public static GenericPool<ChunkData> chunkDataPool;

		public static GenericPool<Chunk> chunkPool;

		private static GenericPool<ChunkAnimation> animationPool;

		[Header("Pools")]
		public int chunkDataPoolSize = 2003;

		public int chunkPoolSize = 271;

		public int animationPoolSize = 256;

		public GameObject ChunkObject;

		[Header("Tileset")]
		public Texture tileset;

		public Texture lookup;

		public Texture normalMap;

		public GameObject tileset3d;

		public static Dictionary<Index, ChunkData> ChunkDatas;

		public static Dictionary<Index, Chunk> Chunks;

		private static List<ChunkData> ChunksToDestroy;

		public static int SavesThisFrame;

		public static bool SpawningChunks;

		public static bool StopSpawning;

		public static bool Initialized;

		public static bool DespawnActive = true;

		private bool Done;

		private Index LastRequest;

		private int SpawnQueue;

		private List<int> targetFrameOperations = new List<int>();

		private int minOperationsPerFrame;

		private int maxOperationsPerFrame;

		public static int operationsCounter;

		private readonly WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

		public float sizeOfBlock
		{
			[CompilerGenerated]
			get
			{
				return (float)tileset.width * Engine.TextureUnit;
			}
		}

		public void AwakeFormEngine()
		{
			Done = false;
			SpawningChunks = false;
			Initialized = false;
			Manager.Get<GameCallbacksManager>().RegisterListener(this);
			if (chunkPool == null)
			{
				chunkPool = new GenericPool<Chunk>(CreateChunk, chunkPoolSize);
			}
			if (chunkDataPool == null)
			{
				chunkDataPool = new GenericPool<ChunkData>(() => new ChunkData(), chunkDataPoolSize);
			}
			if (animationPool == null)
			{
				animationPool = new GenericPool<ChunkAnimation>(delegate
				{
					GameObject gameObject = new GameObject("Chunk Animation");
					return gameObject.AddComponent<ChunkAnimation>();
				}, animationPoolSize);
			}
			ChunksToDestroy = new List<ChunkData>();
			ChunkDatas = new Dictionary<Index, ChunkData>(Index.equalityComparer);
			Chunks = new Dictionary<Index, Chunk>(Index.equalityComparer);
		}

		private void Start()
		{
			RecalculateTargetFrameOperations();
			Engine.ChunkScale = ChunkObject.transform.localScale;
			ChunkObject.GetComponent<Chunk>().MeshContainer.transform.localScale = ChunkObject.transform.localScale;
			ChestInteractiveObject.LoadOpenedChestLocation();
		}

		public void Update()
		{
			if (Initialized)
			{
				if (SpawnQueue == 1 && Done)
				{
					SpawnQueue = 0;
					StartSpawnChunks(LastRequest.x, LastRequest.y, LastRequest.z);
				}
				SavesThisFrame = 0;
				operationsCounter = 0;
			}
		}

		public void LateUpdate()
		{
			foreach (Chunk value in Chunks.Values)
			{
				value.TryLateUpdate();
			}
		}

		public static void SpawnChunks(int x, int y, int z)
		{
			Engine.ChunkManagerInstance.TrySpawnChunks(x, y, z);
		}

		private void TrySpawnChunks(int x, int y, int z)
		{
			if (Done)
			{
				StartSpawnChunks(x, y, z);
				return;
			}
			LastRequest = new Index(x, y, z);
			SpawnQueue = 1;
			StopSpawning = true;
		}

		private void StartSpawnChunks(int originX, int originY, int originZ)
		{
			SpawningChunks = true;
			Done = false;
			int num = Engine.ChunkSpawnDistance;
			if (Time.timeScale == 0f)
			{
				num /= 2;
			}
			StartCoroutine(SpawnMissingChunks(originX, originY, originZ, num));
		}

		private IEnumerator SpawnMissingChunks(int originX, int originY, int originZ, int range)
		{
			if (range + 1 != targetFrameOperations.Count)
			{
				RecalculateTargetFrameOperations();
			}
			while (FrameTakesTooLong(0))
			{
				yield return waitForEndOfFrame;
				if (StopSpawning)
				{
					EndSequence();
					yield break;
				}
			}
			ChunksToDestroy.Clear();
			GatherChunksToDestroy(originX, originY, originZ, range);
			if (!ChunkDataFiles.SavingChunks)
			{
				for (int i = 0; i < ChunksToDestroy.Count; i++)
				{
					if (!ChunksToDestroy[i].InPool)
					{
						Chunk chunk = null;
						Chunks.TryGetValue(ChunksToDestroy[i].ChunkIndex, out chunk);
						if (chunk != null)
						{
							chunk.RemoveChunk();
						}
						else
						{
							chunkDataPool.Release(ChunksToDestroy[i]);
						}
						operationsCounter++;
						if (FrameTakesTooLong(1))
						{
							yield return null;
						}
						if (StopSpawning)
						{
							EndSequence();
							yield break;
						}
					}
				}
				ChunksToDestroy.Clear();
			}
			for (int currentLoop = 0; currentLoop <= range; currentLoop++)
			{
				for (int y = originY - currentLoop; y <= originY + currentLoop; y++)
				{
					for (int x = originX - currentLoop; x <= originX + currentLoop; x++)
					{
						for (int z = originZ - currentLoop; z <= originZ + currentLoop; z++)
						{
							float distance = Vector2.Distance(new Vector2(x, z), new Vector2(originX, originZ));
							if (distance <= (float)range && (!MonoBehaviourSingleton<LimitedWorld>.get.active || MonoBehaviourSingleton<LimitedWorld>.get.RaiseEventForResult(new DataLW
							{
								target = new Index(x, y, z)
							}, EventTypeLW.SpawningChunk)))
							{
								Chunks.TryGetValue(new Index(x, y, z), out Chunk value);
								ChunkDatas.TryGetValue(new Index(x, y, z), out ChunkData value2);
								InitChunk(value, value2, x, y, z);
							}
							if (FrameTakesTooLong(currentLoop))
							{
								yield return waitForEndOfFrame;
							}
							if (StopSpawning)
							{
								EndSequence();
								yield break;
							}
						}
					}
				}
			}
			yield return waitForEndOfFrame;
			EndSequence();
		}

		private void GatherChunksToDestroy(int originX, int originY, int originZ, int range)
		{
			if (DespawnActive)
			{
				float num = Engine.ChunkDespawnDistance + range;
				float num2 = num * num;
				foreach (ChunkData value in ChunkDatas.Values)
				{
					if ((float)Mathf.Abs(value.ChunkIndex.y - originY) > num)
					{
						ChunksToDestroy.Add(value);
					}
					else if (new Vector2(value.ChunkIndex.x - originX, value.ChunkIndex.z - originZ).sqrMagnitude > num2)
					{
						ChunksToDestroy.Add(value);
					}
				}
			}
		}

		private void InitChunk(Chunk currentChunk, ChunkData data, int x, int y, int z)
		{
			if (currentChunk == null)
			{
				if (data == null)
				{
					data = chunkDataPool.Take();
					data.Init(new Index(x, y, z));
					operationsCounter++;
				}
			}
			else if (currentChunk.DisableMesh || currentChunk.EnableTimeout)
			{
				currentChunk.DisableMesh = false;
				currentChunk.EnableTimeout = false;
				currentChunk.Fresh = true;
			}
		}

		private void EndSequence()
		{
			SpawningChunks = false;
			Done = true;
			StopSpawning = false;
		}

		private void StartChunkSpawning()
		{
			Done = true;
			SpawningChunks = false;
			Initialized = true;
		}

		public static ChunkAnimation GetChunkAnimation()
		{
			return animationPool.Take();
		}

		public static void AddChunkAnimation(ChunkAnimation chunkAnim)
		{
			animationPool.Release(chunkAnim);
		}

		public void InitMaterials(Voxel[] voxels)
		{
			MeshRenderer component = ChunkObject.GetComponent<MeshRenderer>();
			int num = 0;
			Dictionary<Material, int> dictionary = new Dictionary<Material, int>();
			foreach (Voxel voxel in voxels)
			{
				if (voxel.VMaterial != null && !dictionary.ContainsKey(voxel.VMaterial))
				{
					dictionary.Add(voxel.VMaterial, num);
					num++;
				}
				BlueprintVoxel blueprintVoxel = voxel as BlueprintVoxel;
				if (blueprintVoxel != null)
				{
					if (blueprintVoxel.sheetBlueprintMat != null && !dictionary.ContainsKey(blueprintVoxel.sheetBlueprintMat))
					{
						dictionary.Add(blueprintVoxel.sheetBlueprintMat, num);
						BlueprintVoxel.sheetBlueprintIndex = num;
						num++;
					}
					else if (blueprintVoxel.sheetBlueprintMat != null)
					{
						BlueprintVoxel.sheetBlueprintIndex = dictionary[blueprintVoxel.sheetBlueprintMat];
					}
					if (blueprintVoxel.defaultBlueprintMat != null && !dictionary.ContainsKey(blueprintVoxel.defaultBlueprintMat))
					{
						dictionary.Add(blueprintVoxel.defaultBlueprintMat, num);
						BlueprintVoxel.defaultBlueprintIndex = num;
						num++;
					}
					else if (blueprintVoxel.defaultBlueprintMat != null)
					{
						BlueprintVoxel.defaultBlueprintIndex = dictionary[blueprintVoxel.defaultBlueprintMat];
					}
					if (blueprintVoxel.customMeshMat != null && !dictionary.ContainsKey(blueprintVoxel.customMeshMat))
					{
						dictionary.Add(blueprintVoxel.customMeshMat, num);
						BlueprintVoxel.customMeshIndex = num;
						num++;
					}
					else if (blueprintVoxel.customMeshMat != null)
					{
						BlueprintVoxel.customMeshIndex = dictionary[blueprintVoxel.customMeshMat];
					}
				}
			}
			Material[] array = new Material[dictionary.Count];
			foreach (Material key in dictionary.Keys)
			{
				array[dictionary[key]] = key;
				if (key.name.Contains("Sheet"))
				{
					key.SetTexture("_MainTex", tileset);
				}
				if (lookup != null && key.HasProperty("_Lookup"))
				{
					key.SetTexture("_Lookup", lookup);
				}
				if (normalMap != null && key.HasProperty("_Bump"))
				{
					key.SetTexture("_Bump", normalMap);
				}
			}
			component.sharedMaterials = array;
			foreach (Voxel voxel2 in voxels)
			{
				if (voxel2.VMaterial != null)
				{
					voxel2.VSubmeshIndex = dictionary[voxel2.VMaterial];
				}
			}
		}

		public void SetTileset(Texture texture, Texture normalMap = null, Texture lookup = null)
		{
			tileset = texture;
			Renderer component = ChunkObject.GetComponent<Renderer>();
			for (int i = 0; i < component.sharedMaterials.Length; i++)
			{
				if (component.sharedMaterials[i].name.Contains("Sheet"))
				{
					component.sharedMaterials[i].mainTexture = texture;
				}
				if (lookup != null && component.sharedMaterials[i].HasProperty("_Lookup"))
				{
					component.sharedMaterials[i].SetTexture("_Lookup", lookup);
				}
				if (normalMap != null && component.sharedMaterials[i].HasProperty("_Bump"))
				{
					component.sharedMaterials[i].SetTexture("_Bump", normalMap);
				}
			}
		}

		private void RecalculateTargetFrameOperations()
		{
			minOperationsPerFrame = 1;
			maxOperationsPerFrame = 35;
			targetFrameOperations.Clear();
			for (int i = 0; i <= Engine.ChunkSpawnDistance; i++)
			{
				float value = (float)i / (float)Engine.ChunkSpawnDistance;
				int item = Mathf.RoundToInt(Easing.Ease(EaseType.OutQuad, maxOperationsPerFrame, minOperationsPerFrame, value));
				targetFrameOperations.Add(item);
			}
		}

		private bool FrameTakesTooLong(int loop)
		{
			FrameLatencyStats.MyStopwatch counter = MonoBehaviourSingleton<FrameLatencyStats>.get.GetCounter(FrameLatencyStats.Counter.FRAME);
			float num = 1f;
			num = ((loop < 3) ? 1f : ((counter.currentDuration > counter.averageDuration) ? 0f : ((!ChunkDataFiles.SavingChunks) ? ((float)(counter.averageDuration / counter.lastDuration)) : 0.1f)));
			if ((float)operationsCounter >= (float)targetFrameOperations[loop] * num)
			{
				return true;
			}
			return false;
		}

		public static void RegisterChunk(Chunk chunk)
		{
			Chunks.Add(chunk.chunkData.ChunkIndex, chunk);
		}

		public static void RegisterChunk(ChunkData chunk)
		{
			ChunkDatas.Add(chunk.ChunkIndex, chunk);
		}

		public static void UnregisterChunk(Chunk chunk)
		{
			Chunks.Remove(chunk.chunkData.ChunkIndex);
		}

		public static void UnregisterChunk(ChunkData chunk)
		{
			ChunkDatas.Remove(chunk.ChunkIndex);
		}

		public static GameObject GetChunk(int x, int y, int z)
		{
			return GetChunk(new Index(x, y, z));
		}

		public static GameObject GetChunk(Index index)
		{
			return GetChunkComponent(index)?.gameObject;
		}

		public static Chunk GetChunkComponent(int x, int y, int z)
		{
			return GetChunkComponent(new Index(x, y, z));
		}

		public static Chunk GetChunkComponent(Index index)
		{
			Chunk value = null;
			Chunks.TryGetValue(index, out value);
			return value;
		}

		public GameObject GetChunk()
		{
			return chunkPool.Take().gameObject;
		}

		public static void ResetPool()
		{
			if (chunkDataPool != null && ChunkDatas != null && ChunkDatas.Values.Count > 0)
			{
				foreach (ChunkData item in ChunkDatas.Values.ToList())
				{
					chunkDataPool.Release(item);
				}
			}
			if (chunkPool != null && Chunks != null && Chunks.Values.Count > 0)
			{
				foreach (Chunk item2 in Chunks.Values.ToList())
				{
					chunkPool.Release(item2);
				}
			}
		}

		private Chunk CreateChunk()
		{
			return UnityEngine.Object.Instantiate(ChunkObject, Vector3.zero, Quaternion.identity).GetComponent<Chunk>();
		}

		public void OnGameplayStarted()
		{
			StartChunkSpawning();
		}

		public void OnGameplayRestarted()
		{
		}

		public void OnGameSavedFrequent()
		{
			Resources.UnloadUnusedAssets();
		}

		public void OnGameSavedInfrequent()
		{
		}

		private void OnDestroy()
		{
			if (chunkPool != null)
			{
				chunkPool.Dispose();
				chunkPool = null;
			}
			if (chunkDataPool != null)
			{
				chunkDataPool.Dispose();
				chunkDataPool = null;
			}
			if (animationPool != null)
			{
				animationPool.Dispose();
				animationPool = null;
			}
		}

		public void RebuildChunks()
		{
			foreach (Index key in Chunks.Keys)
			{
				Chunks[key].RebuildMesh();
			}
		}
	}
}
