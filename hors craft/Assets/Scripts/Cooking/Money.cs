// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.Money
using Common.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Cooking
{
	public class Money : MonoBehaviour
	{
		public float duration = 2f;

		public float screenPercentTravel = 0.3f;

		public float maxScale = 1.5f;

		private float timer;

		private Vector3 initialPosition;

		private Vector3 targetPosition;

		private Text text;

		private void Start()
		{
			initialPosition = base.transform.position;
			targetPosition = initialPosition + new Vector3(0f, (float)Screen.height * screenPercentTravel, 0f);
			text = GetComponentInChildren<Text>();
		}

		private void Update()
		{
			float progress = timer / duration;
			UpdateColor(progress);
			UpdatePosition(progress);
			UpdateScale(progress);
			timer += Time.unscaledDeltaTime;
		}

		private void UpdateColor(float progress)
		{
			Color color = text.color;
			color.a = Easing.Ease(EaseType.InQuart, 1f, 0f, progress);
			text.color = color;
		}

		private void UpdatePosition(float progress)
		{
			base.transform.position = Easing.EaseVector3(EaseType.OutQuart, initialPosition, targetPosition, progress);
		}

		private void UpdateScale(float progress)
		{
			float num = 0.25f;
			float num2 = 1f;
			num2 = ((!(progress <= num)) ? Easing.Ease(EaseType.OutCirc, maxScale, 0.8f, (progress - num) / (1f - num)) : Easing.Ease(EaseType.OutQuint, 1f, maxScale, progress / num));
			base.transform.localScale = Vector3.one * num2;
		}
	}
}
