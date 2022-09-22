// DecompilerFi decompiler from Assembly-CSharp.dll class: GameUI.PetRepresentation
using Common.Utils;
using System;
using UnityEngine;

namespace GameUI
{
	public class PetRepresentation
	{
		public GameObject petObject;

		private GameObject fakeParent;

		public PetRepresentation(GameObject petObject)
		{
			this.petObject = petObject;
		}

		public void Show()
		{
			fakeParent.SetActive(value: true);
		}

		public void UIModeOn(Action<GameObject> setMobRepresentationPlace)
		{
			fakeParent = new GameObject();
			petObject.transform.parent = fakeParent.transform;
			petObject.transform.eulerAngles = Vector3.zero;
			petObject.transform.localPosition = CalculatePosition();
			petObject.GetComponent<AnimalMob>().enabled = false;
			SaveTransform component = petObject.GetComponent<SaveTransform>();
			if (component != null)
			{
				component.enabled = false;
			}
			petObject.SetActive(value: true);
			petObject.SetLayerRecursively(LayerMask.NameToLayer("ClothesPreview"));
			setMobRepresentationPlace(fakeParent);
		}

		public void Hide()
		{
			fakeParent.SetActive(value: false);
		}

		public void UIModeOff()
		{
			UnityEngine.Object.Destroy(fakeParent);
			petObject = null;
		}

		private Vector3 CalculatePosition()
		{
			return -RenderersBounds.MiddlePoint(petObject);
		}
	}
}
