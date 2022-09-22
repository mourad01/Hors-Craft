// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.Product
using Common.Utils;
using System.Linq;
using UnityEngine;

namespace Cooking
{
	public class Product : CookingBaseObject, IPickable, IUsable
	{
		public Device.UpgradeConfig[] upgradePrefabs;

		public bool canBeOrdered;

		public bool canBePickedUp = true;

		public bool usePrefabPositionAndRotation;

		public float baseWaitTime = 30f;

		private Transform visualizationParent;

		private void Awake()
		{
			visualizationParent = base.transform;
		}

		public GameObject GetGameObject()
		{
			return base.gameObject;
		}

		public Product GetProduct()
		{
			return this;
		}

		public int GetPrice()
		{
			return Price;
		}

		public string GetKey()
		{
			return base.Key;
		}

		public bool Unlocked()
		{
			return base.workController.recipesList.IsProductUnlocked(this);
		}

		public Sprite GetSprite()
		{
			return sprite;
		}

		public void Init(int level)
		{
			SetVisualization(level);
		}

		private void SetVisualization(int upgrade)
		{
			while (visualizationParent.childCount > 0)
			{
				UnityEngine.Object.DestroyImmediate(visualizationParent.GetChild(0).gameObject);
			}
			if (upgradePrefabs.Length != 0 && !(upgradePrefabs[0].prefab == null))
			{
				GameObject prefab = (from up in upgradePrefabs
					where upgrade >= up.minLevel
					orderby up.minLevel descending
					select up).First().prefab;
				GameObject gameObject = Object.Instantiate(prefab, visualizationParent, worldPositionStays: false);
				if (!usePrefabPositionAndRotation)
				{
					gameObject.transform.localScale = Vector3.one;
					Bounds bounds = RenderersBounds.MaximumBounds(gameObject);
					Vector3 localPosition = gameObject.transform.localPosition;
					Vector3 min = bounds.min;
					float y = min.y;
					Vector3 localPosition2 = gameObject.transform.localPosition;
					localPosition.y = 0f - (y - localPosition2.y);
					gameObject.transform.localPosition = localPosition;
				}
			}
		}
	}
}
