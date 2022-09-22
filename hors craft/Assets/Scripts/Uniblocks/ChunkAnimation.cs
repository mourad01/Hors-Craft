// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.ChunkAnimation
using System;
using System.Collections;
using UnityEngine;

namespace Uniblocks
{
	public class ChunkAnimation : MonoBehaviour, IPoolAble
	{
		private Chunk chunk;

		private MeshFilter meshFilter;

		private MeshRenderer meshRenderer;

		private void Awake()
		{
			meshFilter = base.gameObject.AddComponent<MeshFilter>();
			meshRenderer = base.gameObject.AddComponent<MeshRenderer>();
		}

		public void Init(Chunk chunk)
		{
			this.chunk = chunk;
			UpdateMesh();
			chunk.GetComponent<MeshRenderer>().enabled = false;
			meshRenderer.enabled = true;
		}

		private void UpdateMesh()
		{
			meshFilter.sharedMesh = chunk.GetComponent<MeshFilter>().sharedMesh;
			meshRenderer.sharedMaterials = chunk.GetComponent<MeshRenderer>().sharedMaterials;
			InitChildren();
		}

		private void InitChildren()
		{
			MeshRenderer[] componentsInChildren = chunk.GetComponentsInChildren<MeshRenderer>();
			foreach (MeshRenderer meshRenderer in componentsInChildren)
			{
				Transform transform = meshRenderer.transform;
				if (transform != chunk.transform)
				{
					meshRenderer.enabled = false;
					GameObject gameObject = new GameObject();
					gameObject.AddComponent<MeshFilter>().sharedMesh = transform.GetComponent<MeshFilter>().sharedMesh;
					gameObject.AddComponent<MeshRenderer>().sharedMaterials = meshRenderer.sharedMaterials;
					gameObject.transform.SetParent(base.transform);
					gameObject.transform.localPosition = transform.position - chunk.transform.position;
					gameObject.transform.rotation = transform.rotation;
				}
			}
		}

		private void DestroyChildren()
		{
			IEnumerator enumerator = base.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					UnityEngine.Object.Destroy(transform.gameObject);
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
		}

		public void StartSpawningAnimation()
		{
			StartCoroutine(SpawnAnimation());
		}

		private IEnumerator SpawnAnimation()
		{
			Vector3 targetPosition = ChunkData.IndexToPosition(chunk.chunkData.ChunkIndex);
			Vector3 startPosition = targetPosition + Vector3.down * (targetPosition.y + (float)ChunkData.SideLength);
			base.transform.position = startPosition;
			float distance = targetPosition.y + (float)ChunkData.SideLength;
			float elapsedTime = 0f;
			while (elapsedTime < Engine.EngineInstance.animationLength)
			{
				base.transform.position = startPosition + Vector3.up * distance * Engine.EngineInstance.chunkSpawnAnimation.Evaluate(elapsedTime);
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}
			if (chunk != null)
			{
				MeshRenderer[] componentsInChildren = chunk.GetComponentsInChildren<MeshRenderer>();
				foreach (MeshRenderer meshRenderer in componentsInChildren)
				{
					meshRenderer.enabled = true;
				}
			}
			ChunkManager.AddChunkAnimation(this);
		}

		public void StartDestroyingAnimation()
		{
			StartCoroutine(DestroyAnimation());
		}

		private IEnumerator DestroyAnimation()
		{
			Vector3 startPosition = ChunkData.IndexToPosition(chunk.chunkData.ChunkIndex);
			float distance = startPosition.y + (float)ChunkData.SideLength;
			float elapsedTime = 0f;
			while (elapsedTime < Engine.EngineInstance.animationLength)
			{
				base.transform.position = startPosition + Vector3.down * distance * Engine.EngineInstance.chunkDestroyAnimation.Evaluate(elapsedTime);
				elapsedTime += Time.unscaledDeltaTime;
				yield return null;
			}
			ChunkManager.AddChunkAnimation(this);
		}

		public void Activate()
		{
		}

		public void Deactivate()
		{
			DestroyChildren();
			meshRenderer.enabled = false;
			meshFilter.mesh = null;
			chunk = null;
		}
	}
}
