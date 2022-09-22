// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.Tachometer
using UnityEngine;
using UnityEngine.UI;

namespace DragMinigame
{
	public class Tachometer : Progressmeter
	{
		[SerializeField]
		private Transform arrow;

		[SerializeField]
		private float minArrowAngle = 117f;

		[SerializeField]
		private float maxArrowAngle = -119f;

		[SerializeField]
		private float minBarAngle = 117f;

		[SerializeField]
		private float maxBarAngle = -119f;

		[SerializeField]
		private Text tachoText;

		[SerializeField]
		private float tachoFor0angle;

		[SerializeField]
		private float fullBarImageTacho;

		[SerializeField]
		private RectTransform greenBarAnchor;

		[SerializeField]
		private RectTransform yellowBarAnchor;

		[SerializeField]
		private RectTransform redBarAnchor;

		private Image redbBarImage;

		private Image yellowBarImage;

		private Image greenBarImage;

		public override void Init(DragGameConfig.ZoneValues zone, float maxValue)
		{
			redbBarImage = redBarAnchor.GetComponentInChildren<Image>();
			greenBarImage = greenBarAnchor.GetComponentInChildren<Image>();
			yellowBarImage = yellowBarAnchor.GetComponentInChildren<Image>();
			base.Init(zone, maxValue);
			float num = Mathf.Abs(minBarAngle - maxBarAngle);
			float num2 = zone.minAcceptableValue - tachoFor0angle;
			float z = (0f - num) * num2 / maxValue;
			yellowBarAnchor.rotation = Quaternion.Euler(new Vector3(0f, 0f, z));
			float fillAmount = (zone.minPerfectValue - zone.minAcceptableValue) / fullBarImageTacho;
			yellowBarImage.fillAmount = fillAmount;
			num2 = zone.minPerfectValue - tachoFor0angle;
			z = (0f - num) * num2 / maxValue;
			greenBarAnchor.rotation = Quaternion.Euler(new Vector3(0f, 0f, z));
			fillAmount = (zone.maxPerfectValue - zone.minPerfectValue) / fullBarImageTacho;
			greenBarImage.fillAmount = fillAmount;
			num2 = zone.maxPerfectValue - tachoFor0angle;
			z = (0f - num) * num2 / maxValue;
			redBarAnchor.rotation = Quaternion.Euler(new Vector3(0f, 0f, z));
			fillAmount = (zone.maxAcceptableValue - zone.maxPerfectValue) / fullBarImageTacho;
			redbBarImage.fillAmount = fillAmount;
		}

		public float GetMaxAngle()
		{
			return maxArrowAngle;
		}

		public void ShowTachoText(float currentTacho)
		{
			if (currentTacho < 0f)
			{
				currentTacho = 0f;
			}
			tachoText.text = (currentTacho * 1000f).ToString("0000");
		}

		public void ShowArrowAngle(float maxTransferSpeed, float currentSpeed)
		{
			if (arrow != null)
			{
				float num = Mathf.Lerp(minArrowAngle, maxArrowAngle, Mathf.InverseLerp(0f, maxTransferSpeed, currentSpeed));
				if (currentSpeed < maxTransferSpeed)
				{
					arrow.eulerAngles = new Vector3(0f, 0f, num);
				}
				else
				{
					arrow.eulerAngles = new Vector3(0f, 0f, num - Random.Range(-1f, 2f));
				}
			}
		}

		public void ShowArrowAngleAtStart(float currentTacho, float startingTacho, float maxTacho)
		{
			if (arrow != null)
			{
				float value = (!(currentTacho < startingTacho)) ? currentTacho : startingTacho;
				float num = Mathf.Lerp(minArrowAngle, maxArrowAngle, Mathf.InverseLerp(0f, maxTacho, value));
				if (currentTacho < startingTacho || currentTacho >= maxTacho)
				{
					arrow.eulerAngles = new Vector3(0f, 0f, num - Random.Range(-1f, 2f));
				}
				else
				{
					arrow.eulerAngles = new Vector3(0f, 0f, num);
				}
			}
		}
	}
}
