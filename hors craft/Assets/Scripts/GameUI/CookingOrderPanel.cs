// DecompilerFi decompiler from Assembly-CSharp.dll class: GameUI.CookingOrderPanel
using Cooking;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
	public class CookingOrderPanel : MonoBehaviour
	{
		public Transform productsPanel;

		public GameObject productPlace;

		public Timer timer
		{
			get;
			private set;
		}

		private void Awake()
		{
			timer = GetComponentInChildren<Timer>();
		}

		public void AddProduct(Action action, Sprite productSprite, string productKey)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(productPlace, productsPanel, worldPositionStays: false);
			gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate
			{
				action();
			});
			CookingOrder componentInChildren = gameObject.GetComponentInChildren<CookingOrder>();
			componentInChildren.productImage.sprite = productSprite;
			componentInChildren.productKey = productKey;
		}

		public void RemoveProduct(Product product)
		{
			CookingOrder[] componentsInChildren = productsPanel.gameObject.GetComponentsInChildren<CookingOrder>();
			int num = 0;
			CookingOrder cookingOrder;
			while (true)
			{
				if (num < componentsInChildren.Length)
				{
					cookingOrder = componentsInChildren[num];
					if (cookingOrder.productKey == product.Key)
					{
						break;
					}
					num++;
					continue;
				}
				return;
			}
			UnityEngine.Object.Destroy(cookingOrder.gameObject);
		}
	}
}
