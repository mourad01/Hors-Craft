// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.VoxelPlacedParticles
using com.ootii.Cameras;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Uniblocks
{
	public class VoxelPlacedParticles : MonoBehaviourSingleton<VoxelPlacedParticles>
	{
		public GameObject particlePrefab;

		public RuntimeAnimatorController controller;

		public float animDuration = 1.5f;

		private List<ParticleSystem> spawnedParticles;

		private List<Vector3> blockedPositions = new List<Vector3>();

		private Camera mainCamera;

		private void Awake()
		{
			InitParticlePool();
		}

		private void InitParticlePool()
		{
			if (!(particlePrefab == null))
			{
				spawnedParticles = new List<ParticleSystem>(3);
				for (int i = 0; i < 3; i++)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate(particlePrefab, base.transform, worldPositionStays: false);
					gameObject.SetActive(value: false);
					spawnedParticles.Add(gameObject.GetComponent<ParticleSystem>());
				}
			}
		}

		public void Play(Vector3 pos, ushort voxelId)
		{
			if (!(particlePrefab == null))
			{
				if (mainCamera == null)
				{
					mainCamera = CameraController.instance.MainCamera;
				}
				if (mainCamera.pixelRect.Contains(mainCamera.WorldToScreenPoint(pos)))
				{
					PlayParticles(pos);
				}
			}
		}

		private void PlayParticles(Vector3 pos)
		{
			ParticleSystem particleSystem = spawnedParticles.FirstOrDefault((ParticleSystem p) => !p.gameObject.activeSelf);
			if (!(particleSystem == null))
			{
				particleSystem.transform.position = pos;
				particleSystem.gameObject.SetActive(value: true);
				particleSystem.Play();
			}
		}

		private void Update()
		{
			if (particlePrefab != null)
			{
				spawnedParticles.ForEach(delegate(ParticleSystem p)
				{
					if (!p.isPlaying)
					{
						p.Stop();
						p.Clear();
						p.gameObject.SetActive(value: false);
					}
				});
			}
		}

		public void PlaceCustomObject(Voxel voxel, Vector3 pos, byte rotation, Action doAfter)
		{
			if (controller == null)
			{
				doAfter();
			}
			else if (!blockedPositions.Contains(pos))
			{
				StartCoroutine(PlaceObject(voxel, pos, rotation, doAfter));
			}
		}

		private IEnumerator PlaceObject(Voxel voxel, Vector3 pos, byte rotation, Action doAfter)
		{
			blockedPositions.Add(pos);
			GameObject fakeObjectParent = new GameObject("CustomObjectAnim");
			fakeObjectParent.transform.position = pos;
			float y = 0f;
			switch (rotation)
			{
			case 0:
				y = 0f;
				break;
			case 1:
				y = 90f;
				break;
			case 2:
				y = 180f;
				break;
			case 3:
				y = 270f;
				break;
			}
			fakeObjectParent.transform.rotation = Quaternion.Euler(0f, y, 0f);
			GameObject fakeObject = new GameObject("Object");
			fakeObject.transform.SetParent(fakeObjectParent.transform, worldPositionStays: false);
			MeshFilter filter = fakeObject.AddComponent<MeshFilter>();
			filter.mesh = voxel.VMesh;
			MeshRenderer renderer = fakeObject.AddComponent<MeshRenderer>();
			renderer.sharedMaterial = voxel.VMaterial;
			Animator anim = fakeObject.AddComponent<Animator>();
			anim.runtimeAnimatorController = controller;
			anim.updateMode = AnimatorUpdateMode.UnscaledTime;
			yield return new WaitForSecondsRealtime(animDuration);
			blockedPositions.Remove(pos);
			doAfter();
			yield return new WaitForSecondsRealtime(0.1f);
			UnityEngine.Object.Destroy(fakeObjectParent);
		}
	}
}
