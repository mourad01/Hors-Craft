// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.VoxelDestroyedParticles
using com.ootii.Cameras;
using System.Collections.Generic;
using UnityEngine;

namespace Uniblocks
{
	public class VoxelDestroyedParticles : MonoBehaviourSingleton<VoxelDestroyedParticles>
	{
		private const int MAX_CACHED_MESHES = 5;

		public GameObject particlePrefab;

		private GameObject spawnedParticlePrefab;

		private ParticleSystem _system;

		private ParticleSystemRenderer _systemRenderer;

		private Dictionary<ushort, Mesh> createdMeshesCache;

		private List<ushort> createdVoxels;

		private Camera mainCamera;

		private ParticleSystem system
		{
			get
			{
				if (_system == null)
				{
					if (spawnedParticlePrefab == null)
					{
						SpawnParticlePrefab();
					}
					_system = spawnedParticlePrefab.GetComponent<ParticleSystem>();
				}
				return _system;
			}
		}

		private ParticleSystemRenderer systemRenderer
		{
			get
			{
				if (_systemRenderer == null)
				{
					if (spawnedParticlePrefab == null)
					{
						SpawnParticlePrefab();
					}
					_systemRenderer = spawnedParticlePrefab.GetComponent<ParticleSystemRenderer>();
				}
				return _systemRenderer;
			}
		}

		private void SpawnParticlePrefab()
		{
			spawnedParticlePrefab = UnityEngine.Object.Instantiate(particlePrefab, Vector3.zero, Quaternion.identity);
		}

		private void Start()
		{
			createdMeshesCache = new Dictionary<ushort, Mesh>();
			createdVoxels = new List<ushort>();
		}

		public void Play(Vector3 pos, ushort voxelId)
		{
			if (mainCamera == null)
			{
				mainCamera = CameraController.instance.MainCamera;
			}
			if (mainCamera.pixelRect.Contains(mainCamera.WorldToScreenPoint(pos)))
			{
				Voxel voxelType = Engine.GetVoxelType(voxelId);
				if (!voxelType.VCustomMesh)
				{
					systemRenderer.transform.position = pos;
					systemRenderer.transform.LookAt(mainCamera.transform.position);
					systemRenderer.sharedMaterial = Engine.GetChunkMaterial(voxelType.VSubmeshIndex);
					systemRenderer.mesh = GetMeshFor(voxelId);
					system.Play();
				}
			}
		}

		private Mesh GetMeshFor(ushort voxelId)
		{
			if (createdMeshesCache.TryGetValue(voxelId, out Mesh value))
			{
				return value;
			}
			value = CreateMesh(voxelId);
			createdVoxels.Add(voxelId);
			createdMeshesCache.Add(voxelId, value);
			if (createdVoxels.Count > 5)
			{
				createdMeshesCache.Remove(createdVoxels[0]);
				createdVoxels.RemoveAt(0);
			}
			return value;
		}

		private Mesh CreateMesh(ushort voxelId)
		{
			Mesh mesh = new Mesh();
			mesh.vertices = new Vector3[4]
			{
				new Vector3(-0.5f, -0.5f, 0f),
				new Vector3(0.5f, -0.5f, 0f),
				new Vector3(0.5f, 0.5f, 0f),
				new Vector3(-0.5f, 0.5f, 0f)
			};
			mesh.triangles = new int[6]
			{
				0,
				1,
				3,
				1,
				2,
				3
			};
			Mesh mesh2 = mesh;
			float pad = Engine.TextureUnit * Engine.TexturePadding;
			Vector2 textureOffset = Engine.GetTextureOffset(voxelId, Facing.forward);
			Vector2[] topUVs = Engine.MeshCreator.GetTopUVs(pad, Engine.TextureUnit, textureOffset, 0);
			if (topUVs.Length == 8)
			{
				mesh2.uv = new Vector2[4]
				{
					topUVs[0],
					topUVs[1],
					topUVs[2],
					topUVs[3]
				};
				mesh2.uv2 = new Vector2[4]
				{
					topUVs[4],
					topUVs[5],
					topUVs[6],
					topUVs[7]
				};
			}
			else
			{
				mesh2.uv = topUVs;
			}
			mesh2.RecalculateNormals();
			return mesh2;
		}
	}
}
