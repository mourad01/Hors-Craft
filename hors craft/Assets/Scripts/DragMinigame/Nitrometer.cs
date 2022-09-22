// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.Nitrometer
using UnityEngine;
using UnityEngine.UI;

namespace DragMinigame
{
	public class Nitrometer : Boost
	{
		[SerializeField]
		private Image mask;

		public void ShowNitro()
		{
			if (mask != null)
			{
				float fillAmount = Mathf.InverseLerp(fullBoost, 0f, currentBoost);
				mask.fillAmount = fillAmount;
			}
		}
	}
}
