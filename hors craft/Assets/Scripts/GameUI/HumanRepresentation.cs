// DecompilerFi decompiler from Assembly-CSharp.dll class: GameUI.HumanRepresentation
using Common.Utils;
using System;
using UnityEngine;

namespace GameUI
{
	public class HumanRepresentation
	{
		public PlayerGraphic graphic;

		public Pettable pettable;

		private Transform oldParent;

		private Vector3 oldPosition;

		private Vector3 oldRotation;

		private int oldLayer;

		private Animator animator;

		private GameObject fakeParent;

		public HumanRepresentation(PlayerGraphic graphic, Pettable pettable = null)
		{
			this.graphic = graphic;
			this.pettable = pettable;
			if (graphic != null)
			{
				oldParent = graphic.graphicRepresentation.transform.parent;
				oldPosition = graphic.graphicRepresentation.transform.localPosition;
				oldRotation = graphic.graphicRepresentation.transform.localEulerAngles;
			}
		}

		public void Show()
		{
			fakeParent.SetActive(value: true);
		}

		public void UIModeOn(Action<GameObject> setMobRepresentationPlace, bool setLayerToClothes = true)
		{
			fakeParent = new GameObject();
			if (graphic != null)
			{
				graphic.graphicRepresentation.transform.SetParent(fakeParent.transform, worldPositionStays: false);
				graphic.graphicRepresentation.transform.localEulerAngles = Vector3.zero;
				graphic.graphicRepresentation.transform.localPosition = CalculatePosition();
				graphic.graphicRepresentation.SetActive(value: true);
				fakeParent.transform.localScale = new Vector3(graphic.scaleFactor, graphic.scaleFactor, graphic.scaleFactor);
				graphic.graphicRepresentation.transform.localScale = Vector3.one;
				oldLayer = graphic.graphicRepresentation.layer;
				if (setLayerToClothes)
				{
					graphic.graphicRepresentation.SetLayerRecursively(LayerMask.NameToLayer("ClothesPreview"));
				}
				setMobRepresentationPlace(fakeParent);
			}
		}

		public void Hide()
		{
			fakeParent.SetActive(value: false);
		}

		public void UIModeOff()
		{
			if (graphic != null)
			{
				graphic.graphicRepresentation.transform.SetParent(oldParent, worldPositionStays: false);
				graphic.graphicRepresentation.transform.localEulerAngles = oldRotation;
				graphic.graphicRepresentation.transform.localPosition = oldPosition;
				graphic.graphicRepresentation.transform.localScale = new Vector3(graphic.scaleFactor, graphic.scaleFactor, graphic.scaleFactor);
				graphic.graphicRepresentation.SetLayerRecursively(oldLayer);
				UnityEngine.Object.Destroy(fakeParent);
				graphic = null;
				pettable = null;
			}
		}

		public void HideHead()
		{
			if (graphic != null)
			{
				graphic.head.SetActive(value: false);
			}
		}

		public void ShowHead()
		{
			if (graphic != null)
			{
				graphic.head.SetActive(value: true);
			}
		}

		private Vector3 CalculatePosition()
		{
			return -RenderersBounds.MiddlePoint(graphic.graphicRepresentation);
		}
	}
}
