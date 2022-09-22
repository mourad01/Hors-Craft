// DecompilerFi decompiler from Assembly-CSharp.dll class: ItemVInventory.EquipMesh
using System;
using UnityEngine;

namespace ItemVInventory
{
	public abstract class EquipMesh : AbstractEquipmentBehaviour
	{
		[Serializable]
		public class MeshChangeData
		{
			public int level;

			public GameObject prefab;

			public Sprite sprite;
		}

		public MeshChangeData[] prefabsConfig;

		protected Transform prefabHolder;

		protected virtual void Start()
		{
			Array.Sort(prefabsConfig, (MeshChangeData one, MeshChangeData two) => one.level.CompareTo(two.level));
		}

		protected virtual void SetPrefab(int index)
		{
			if (prefabHolder != null)
			{
				for (int num = prefabHolder.childCount - 1; num >= 0; num--)
				{
					UnityEngine.Object.Destroy(prefabHolder.GetChild(num).gameObject);
				}
				GameObject gameObject = UnityEngine.Object.Instantiate(prefabsConfig[index].prefab, prefabHolder);
				gameObject.transform.localPosition = Vector3.zero;
				gameObject.transform.localRotation = Quaternion.identity;
			}
			ItemDefinition componentInParent = base.gameObject.GetComponentInParent<ItemDefinition>();
			if (!(componentInParent == null))
			{
				componentInParent.itemSprite = prefabsConfig[index].sprite;
			}
		}

		public override void Equip(int level)
		{
			if (prefabHolder == null)
			{
				TryGetPrefabHolder(level);
			}
			if (prefabHolder != null)
			{
				SetPrefab(FindMeshIndex(level));
			}
		}

		protected abstract void TryGetPrefabHolder(int level);

		protected virtual int FindMeshIndex(int level)
		{
			int result = 0;
			for (int i = 1; i < prefabsConfig.Length; i++)
			{
				if (level >= prefabsConfig[i].level)
				{
					result = i;
				}
			}
			return result;
		}
	}
}
