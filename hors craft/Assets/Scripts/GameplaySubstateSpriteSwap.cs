// DecompilerFi decompiler from Assembly-CSharp.dll class: GameplaySubstateSpriteSwap
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameplaySubstateSpriteSwap : MonoBehaviour, IGameplaySubstateAction
{
	[Serializable]
	public struct SwapSpriteData
	{
		public string path;

		public string imageToSwapName;

		public Sprite sprite;

		public void Swap(ModuleLoader module, GameObject other)
		{
			Image imageToSwap = GetImageToSwap(other.transform);
			if (!(imageToSwap == null))
			{
				imageToSwap.sprite = sprite;
			}
		}

		private Image GetImageToSwap(Transform parentTransform)
		{
			Transform transform;
			if (imageToSwapName.Contains("/") || imageToSwapName.Contains("\\"))
			{
				transform = parentTransform.GetChildByPath(imageToSwapName);
			}
			else
			{
				transform = parentTransform.Find(imageToSwapName);
				if (transform == null)
				{
					transform = parentTransform.FindChildRecursively(imageToSwapName);
				}
			}
			if (transform == null)
			{
				return null;
			}
			return transform.GetComponent<Image>();
		}
	}

	public List<SwapSpriteData> swapSpriteData;

	protected Dictionary<string, List<SwapSpriteData>> swapSpriteDataDictionary;

	private void Awake()
	{
		if (this.swapSpriteData == null)
		{
			return;
		}
		swapSpriteDataDictionary = new Dictionary<string, List<SwapSpriteData>>(this.swapSpriteData.Count);
		for (int i = 0; i < this.swapSpriteData.Count; i++)
		{
			Dictionary<string, List<SwapSpriteData>> dictionary = swapSpriteDataDictionary;
			SwapSpriteData swapSpriteData = this.swapSpriteData[i];
			if (!dictionary.ContainsKey(swapSpriteData.path))
			{
				Dictionary<string, List<SwapSpriteData>> dictionary2 = swapSpriteDataDictionary;
				SwapSpriteData swapSpriteData2 = this.swapSpriteData[i];
				dictionary2.Add(swapSpriteData2.path, new List<SwapSpriteData>());
			}
			Dictionary<string, List<SwapSpriteData>> dictionary3 = swapSpriteDataDictionary;
			SwapSpriteData swapSpriteData3 = this.swapSpriteData[i];
			dictionary3[swapSpriteData3.path].Add(this.swapSpriteData[i]);
		}
	}

	public Action<ModuleLoader, GameObject> GetAction()
	{
		return SwapElement;
	}

	public void SwapElement(ModuleLoader moduleLoader, GameObject other)
	{
		if (swapSpriteDataDictionary != null && swapSpriteDataDictionary.ContainsKey(moduleLoader.path))
		{
			List<SwapSpriteData> list = swapSpriteDataDictionary[moduleLoader.path];
			for (int i = 0; i < list.Count; i++)
			{
				list[i].Swap(moduleLoader, other);
			}
		}
	}
}
