// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.Chunk
using Common.Managers;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace Uniblocks
{
	public class Chunk : MonoBehaviour, IPoolAble
	{
		public ChunkData chunkData;

		public bool Fresh = true;

		public bool EnableTimeout;

		public bool DisableMesh;

		private bool FlaggedToRemove;

		public bool animate;

		public bool FlaggedToUpdate;

		public bool Spawned;

		public GameObject MeshContainer;

		public GameObject ChunkCollider;

		public GameObject meshContainerInstance;

		public Material[] tilesetMaterials;

		public bool spawnFishesParticles = true;

		public GameObject fishParticlesSpawner;

		private GameObject fishSpawnedParticles;

		private bool isSaveTransformManager;

		private bool isPetsManager;

		private bool inited;

		private Stopwatch stopWatch = new Stopwatch();

		private Camera mainCamera;

		private void Awake()
		{
			isSaveTransformManager = Manager.Contains<SaveTransformsManager>();
			isPetsManager = Manager.Contains<PetManager>();
		}

		public Chunk Init(ChunkData data)
		{
			chunkData = data;
			Fresh = true;
			base.transform.position = ChunkData.IndexToPosition(chunkData.ChunkIndex);
			inited = true;
			ChunkManager.RegisterChunk(this);
			animate = (Engine.doAnimate && Time.timeScale > 0.01f && Vector2.Distance(PlayerGraphic.GetControlledPlayerInstance().transform.position.XZ(), base.transform.position.XZ()) > 32f);
			Spawned = true;
			if (isPetsManager)
			{
				Manager.Get<PetManager>().ChunkSpawned(chunkData);
			}
			if (isSaveTransformManager)
			{
				Manager.Get<SaveTransformsManager>().ChunkSpawned(chunkData);
			}
			if (!chunkData.Empty && spawnFishesParticles)
			{
				Vector3 position = base.transform.position;
				if (position.y == 0f)
				{
					TryToSpawnFishes();
				}
			}
			return this;
		}

		public void TryToSpawnFishes()
		{
			Voxel voxelType = Engine.GetVoxelType(Engine.usefulIDs.waterBlockID);
			Material mat = voxelType.VMaterial;
			Material[] sharedMaterials = GetComponent<Renderer>().sharedMaterials;
			bool hasWater = Array.Exists(sharedMaterials, (Material s) => s.Equals(mat));
			chunkData.hasWater = hasWater;
			if (chunkData.hasWater)
			{
				StartCoroutine(SpawnFishes());
			}
		}

		private IEnumerator SpawnFishes()
		{
			yield return new WaitForSeconds(2f);
			GameObject spawnedObj = UnityEngine.Object.Instantiate(fishParticlesSpawner);
			spawnedObj.transform.position = base.transform.position;
			fishSpawnedParticles = spawnedObj;
		}

		private bool AllNeighborsHaveData()
		{
			ChunkData[] neighborChunks = this.chunkData.NeighborChunks;
			foreach (ChunkData chunkData in neighborChunks)
			{
				if (chunkData == null || !chunkData.VoxelsDone)
				{
					return false;
				}
			}
			return true;
		}

		public void FlagToRemove()
		{
			FlaggedToRemove = true;
		}

		public void FlagToUpdate()
		{
			if (!FlaggedToUpdate)
			{
				FlaggedToUpdate = true;
				chunkData.isDirty = true;
			}
		}

		public void TryLateUpdate()
		{
			if (FlaggedToUpdate && !DisableMesh && Engine.GenerateMeshes)
			{
				FlaggedToUpdate = false;
				RebuildMesh();
			}
			if (FlaggedToRemove && ChunkManager.SavesThisFrame < Engine.MaxChunkSaves)
			{
				RemoveChunk();
			}
		}

		private void TriggerEvents()
		{
			for (int i = 0; i < ChunkData.SideLength; i++)
			{
				for (int j = 0; j < ChunkData.SideLength; j++)
				{
					for (int k = 0; k < ChunkData.SideLength; k++)
					{
						ushort num = chunkData.VoxelData[i * ChunkData.SquaredSideLength + j * ChunkData.SideLength + k];
						if (num == 0)
						{
							continue;
						}
						Vector3 position = base.transform.position + new Vector3(k, j, i);
						Voxel voxelType = Engine.GetVoxelType(num, position);
						if (voxelType.hasStartBehaviour)
						{
							VoxelEvents instanceForVoxelId = Singleton<VoxelEventsInstancesCache>.get.GetInstanceForVoxelId(num);
							if (instanceForVoxelId != null)
							{
								instanceForVoxelId.OnBlockRebuilded(chunkData, k, j, i);
							}
						}
					}
				}
			}
		}

		public void RemoveChunk()
		{
			bool flag = false;
			if (Engine.SaveVoxelData)
			{
				if (!ChunkDataFiles.SavingChunks)
				{
					chunkData.SaveData();
					ChunkManager.SavesThisFrame++;
					flag = true;
				}
			}
			else
			{
				flag = true;
			}
			if (flag)
			{
				if (!chunkData.Empty)
				{
					SpawnChunkAnimation().StartDestroyingAnimation();
				}
				if (isSaveTransformManager)
				{
					Manager.Get<SaveTransformsManager>().ChunkDespawned(chunkData);
				}
				if (Manager.Contains<MobsManager>())
				{
					Manager.Get<MobsManager>().ChunkDespawned(chunkData);
				}
				GetComponent<MeshFilter>().mesh = null;
				GetComponent<MeshCollider>().sharedMesh = null;
				if (chunkData != null)
				{
					ChunkManager.chunkDataPool.Release(chunkData);
				}
				ChunkManager.chunkPool.Release(this);
			}
		}

		public ChunkAnimation SpawnChunkAnimation()
		{
			ChunkAnimation chunkAnimation = ChunkManager.GetChunkAnimation();
			chunkAnimation.Init(this);
			return chunkAnimation;
		}

		public void StartSpawnAnimation()
		{
			if (animate)
			{
				animate = false;
				if (Engine.EngineInstance.animationLength > 0f)
				{
					SpawnChunkAnimation().StartSpawningAnimation();
				}
			}
		}

		public void RebuildMesh()
		{
			if (!chunkData.Generate())
			{
				FlagToUpdate();
			}
		}

		public void ChunkComputerWaterCollider()
		{
			chunkData.ChunkComputerWaterCollider();
		}

		public bool WaterCollides(Chunk otherChunk)
		{
			if (chunkData.minMaxXZ.Length < 1)
			{
				UnityEngine.Debug.LogError("Length < 1");
				ChunkComputerWaterCollider();
			}
			Vector3 position = base.transform.position;
			float num = position.x + (float)chunkData.minMaxXZ[0] - 1f;
			Vector3 position2 = base.transform.position;
			float num2 = position2.x + (float)chunkData.minMaxXZ[1] + 1f;
			Vector3 position3 = base.transform.position;
			float num3 = position3.z + (float)chunkData.minMaxXZ[2] - 1f;
			Vector3 position4 = base.transform.position;
			float num4 = position4.z + (float)chunkData.minMaxXZ[3] + 1f;
			Vector3 position5 = otherChunk.transform.position;
			float num5 = position5.x + (float)otherChunk.chunkData.minMaxXZ[0];
			Vector3 position6 = otherChunk.transform.position;
			float num6 = position6.x + (float)otherChunk.chunkData.minMaxXZ[1];
			Vector3 position7 = otherChunk.transform.position;
			float num7 = position7.z + (float)otherChunk.chunkData.minMaxXZ[2];
			Vector3 position8 = otherChunk.transform.position;
			float num8 = position8.z + (float)otherChunk.chunkData.minMaxXZ[3];
			if (num2 >= num5 && num <= num6 && num4 >= num7 && num3 <= num8)
			{
				return true;
			}
			return false;
		}

		public void Activate()
		{
			GetComponent<MeshFilter>().sharedMesh = null;
			GetComponent<MeshCollider>().sharedMesh = null;
		}

		public void Deactivate()
		{
			if (inited)
			{
				ChunkManager.UnregisterChunk(this);
			}
			if (meshContainerInstance != null)
			{
				UnityEngine.Object.Destroy(meshContainerInstance);
			}
			StopAllCoroutines();
			if (fishSpawnedParticles != null)
			{
				UnityEngine.Object.Destroy(fishSpawnedParticles);
			}
			Fresh = false;
			FlaggedToUpdate = false;
			Spawned = false;
			EnableTimeout = false;
			DisableMesh = false;
			FlaggedToRemove = false;
			spawnFishesParticles = false;
			animate = false;
			chunkData = null;
			inited = false;
			IEnumerator enumerator = base.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					if (transform != null)
					{
						UnityEngine.Object.Destroy(transform.gameObject);
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			base.transform.position = new Vector3(1000f, 10000f, 1000f);
			GetComponent<MeshFilter>().sharedMesh = null;
			GetComponent<MeshCollider>().sharedMesh = null;
			ChunkManager.operationsCounter++;
		}
	}
}
