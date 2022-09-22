// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.FormChanger
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Gameplay
{
	public class FormChanger : MonoBehaviour
	{
		[Serializable]
		public class FormChangingPart
		{
			public string key;

			public GameObject graphic;

			public List<GameObject> items = new List<GameObject>();

			[HideInInspector]
			public GameObject currentlyMountedPart;

			[HideInInspector]
			public int partIndex;
		}

		public List<FormChangingPart> parts;

		private Animator mechAnimator;

		private void Awake()
		{
			mechAnimator = GetComponentInChildren<Animator>();
		}

		public void GenerateRandom()
		{
			parts.ForEach(delegate(FormChangingPart p)
			{
				SetPart(p, UnityEngine.Random.Range(0, p.items.Count));
			});
		}

		public void SetPart(string key, int index)
		{
			FormChangingPart part = parts.FirstOrDefault((FormChangingPart p) => p.key == key);
			SetPart(part, index);
		}

		public void SetPart(FormChangingPart part, int index)
		{
			if (part.currentlyMountedPart != null)
			{
				UnityEngine.Object.DestroyImmediate(part.currentlyMountedPart);
				part.currentlyMountedPart = null;
				part.partIndex = 0;
			}
			index = Mathf.Clamp(index, 0, part.items.Count - 1);
			GameObject gameObject = UnityEngine.Object.Instantiate(part.items[index], part.graphic.transform, worldPositionStays: false);
			gameObject.transform.localPosition = Vector3.zero;
			gameObject.transform.localRotation = Quaternion.identity;
			gameObject.transform.localScale = Vector3.one;
			gameObject.name = gameObject.name.Replace("(Clone)", string.Empty);
			part.currentlyMountedPart = gameObject;
			part.partIndex = index;
			OnFormChange();
		}

		public int GetPartLevel(string partKey)
		{
			return parts.FirstOrDefault((FormChangingPart p) => p.key == partKey)?.partIndex ?? 0;
		}

		public FormChangingPart GetPart(string key)
		{
			return parts.FirstOrDefault((FormChangingPart p) => p.key == key);
		}

		private void OnFormChange()
		{
			IFormChangeListener[] componentsInParent = GetComponentsInParent<IFormChangeListener>();
			IFormChangeListener[] array = componentsInParent;
			foreach (IFormChangeListener formChangeListener in array)
			{
				formChangeListener.OnFormChange();
			}
			if (mechAnimator != null)
			{
				mechAnimator.Rebind();
			}
		}
	}
}
