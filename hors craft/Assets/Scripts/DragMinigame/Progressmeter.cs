// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.Progressmeter
using UnityEngine;
using UnityEngine.UI;

namespace DragMinigame
{
	public class Progressmeter : MonoBehaviour
	{
		[SerializeField]
		private Slider testSlider;

		[SerializeField]
		private Text valueText;

		[SerializeField]
		private Image sliderImage;

		[HideInInspector]
		public float maxValue;

		private float currentValue;

		public void ShowProgress(float currentShiftProgress, DragGameConfig.ZoneValues zone)
		{
			if (testSlider != null)
			{
				testSlider.value = currentValue;
			}
			if (testSlider != null)
			{
				valueText.text = currentValue.ToString();
			}
			if (sliderImage != null)
			{
				if ((currentShiftProgress > zone.minAcceptableValue && currentShiftProgress < zone.minPerfectValue) || (currentShiftProgress > zone.maxPerfectValue && currentShiftProgress < zone.maxAcceptableValue))
				{
					sliderImage.color = Color.yellow;
				}
				else if (currentShiftProgress >= zone.minPerfectValue && currentShiftProgress <= zone.maxPerfectValue)
				{
					sliderImage.color = Color.green;
				}
				else
				{
					sliderImage.color = Color.white;
				}
			}
		}

		public virtual void Init(DragGameConfig.ZoneValues zone, float maxValue)
		{
			this.maxValue = maxValue;
			if (testSlider != null)
			{
				testSlider.maxValue = maxValue;
			}
		}

		public float CalculateShiftProgress(float maxLevelProgress, float currentProgress)
		{
			return currentValue = currentProgress * maxValue / maxLevelProgress;
		}
	}
}
