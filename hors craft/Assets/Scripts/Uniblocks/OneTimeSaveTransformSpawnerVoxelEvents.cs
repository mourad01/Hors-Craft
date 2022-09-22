// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.OneTimeSaveTransformSpawnerVoxelEvents
using System.Collections;
using UnityEngine;

namespace Uniblocks
{
	public class OneTimeSaveTransformSpawnerVoxelEvents : DefaultVoxelEvents, ISpawnableVoxelEvent
	{
		public Vector3 spawnPositionOffset = Vector3.zero;

		public GameObject prefabToSpawn;

		public override void OnBlockRebuilded(ChunkData chunk, int x, int y, int z)
		{
			VoxelInfo voxelInfo = chunk.GetVoxelInfo(x, y, z);
			if (!CheckIndentifier(voxelInfo))
			{
				SavePropableSpawn(voxelInfo);
				StartCoroutine(SpawnPrefab(voxelInfo, 5f));
			}
		}

		protected virtual void AfterSpawn(VoxelInfo info, GameObject spawned)
		{
		}

		private void SavePropableSpawn(VoxelInfo info)
		{
			PlayerPrefs.SetInt(GetIdentifier(info), 1);
			PlayerPrefs.Save();
		}

		private string GetIdentifier(VoxelInfo info)
		{
			return $"oneTimeSpawn.{info.index.ToString()}";
		}

		private bool CheckIndentifier(VoxelInfo info)
		{
			return PlayerPrefs.GetInt(GetIdentifier(info), 0) == 1;
		}

		private IEnumerator SpawnPrefab(VoxelInfo info, float spawnAfter)
		{
			yield return new WaitForSeconds(spawnAfter);
			if (prefabToSpawn != null && info.chunk != null)
			{
				UnityEngine.Debug.Log("spawning: " + prefabToSpawn.transform.name);
				Index index = new Index(info.index.x, info.index.y, info.index.z);
				Vector3 vector = info.chunk.VoxelIndexToPosition(index);
				UnityEngine.Debug.Log(Physics.Raycast(vector, Vector3.down, out RaycastHit _, 5f));
				vector += spawnPositionOffset;
				GameObject gameObject = Object.Instantiate(prefabToSpawn, vector, Quaternion.identity);
				gameObject.transform.eulerAngles = Vector3.up * info.GetVoxelFinalRotationInDegrees();
				if (gameObject.GetComponentInChildren<VehicleController>() != null)
				{
					SaveTransform component = gameObject.GetComponent<SaveTransform>();
					if (component != null)
					{
						UnityEngine.Object.Destroy(component);
					}
				}
				if (gameObject.GetComponentInChildren<HeliController>() != null)
				{
					HeliController componentInChildren = gameObject.GetComponentInChildren<HeliController>();
					componentInChildren.rigidBody.freezeRotation = false;
					componentInChildren.rigidBody.useGravity = false;
				}
				AfterSpawn(info, gameObject);
			}
			yield return new WaitForSeconds(0.1f);
			if (info.chunk != null)
			{
				info.SetVoxel(0, updateMesh: false, 0);
			}
		}

		public GameObject[] GetPrefabs()
		{
			return new GameObject[1]
			{
				prefabToSpawn
			};
		}
	}
}
