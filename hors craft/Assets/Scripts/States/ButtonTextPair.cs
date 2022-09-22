// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ButtonTextPair
using System;
using UnityEngine.UI;

namespace States
{
	[Serializable]
	public class ButtonTextPair
	{
		public Button button;

		public Text buttonText;

		public void SetButtonVisibility(bool state)
		{
			button.gameObject.SetActive(state);
		}
	}
}
