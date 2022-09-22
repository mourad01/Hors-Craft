// DecompilerFi decompiler from Assembly-CSharp.dll class: States.FragmentComponent
using System;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	[Serializable]
	public class FragmentComponent
	{
		public string fragmentName;

		public string translationKey;

		public string defaultText;

		public int preferredWidth;

		public GameObject prefab;

		public string prefabResource;

		[HideInInspector]
		public GameObject instance;

		[HideInInspector]
		public Button button;
	}
}
