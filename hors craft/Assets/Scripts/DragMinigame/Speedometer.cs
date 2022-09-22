// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.Speedometer
using UnityEngine;
using UnityEngine.UI;

namespace DragMinigame
{
	public class Speedometer : MonoBehaviour
	{
		private const float MAX_VALUE = 240f;

		[SerializeField]
		private Text speedText;

		public void ShowSpeed(float currentSpeed)
		{
			if (currentSpeed < 0f)
			{
				currentSpeed = 0f;
			}
			speedText.text = currentSpeed.ToString("0");
		}
	}
}
