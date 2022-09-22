// DecompilerFi decompiler from Assembly-CSharp.dll class: GameUI.GrenadeIndicatorController
using Common.Utils;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
	public class GrenadeIndicatorController : MonoBehaviour
	{
		private const float DISTANCE_FROM_MIDDLE = 100f;

		private const float MIN_ANIM_SPEED = 1f;

		private const float MAX_ANIM_SPEED = 4f;

		private Animator alphaAnimator;

		private Transform pivot;

		private Transform grenade;

		private Grenade grenadeScript;

		private Vector2 middlePoint;

		private float range;

		public void Initialize(Transform pivot, Transform grenade, float range)
		{
			alphaAnimator = GetComponent<Animator>();
			this.pivot = pivot;
			this.grenade = grenade;
			grenadeScript = grenade.GetComponent<Grenade>();
			middlePoint = new Vector2((float)Screen.width / 2f, (float)Screen.height / 2f);
			this.range = range;
		}

		private void Update()
		{
			if (grenade == null || pivot == null)
			{
				UnityEngine.Object.Destroy(base.gameObject);
				return;
			}
			UpdatePositionAndRotation();
			UpdateAnimatorSpeed();
			bool enabled = Vector3.Distance(pivot.position, grenade.position) <= range;
			GetComponentInChildren<Image>().enabled = enabled;
		}

		private void UpdatePositionAndRotation()
		{
			float num = CalculateAngle();
			base.transform.rotation = Quaternion.Euler(0f, 0f, 180f - num);
			base.transform.localScale = new Vector3((!(num < 0f)) ? 1f : (-1f), 1f, 1f);
			float num2 = Mathf.Cos((num - 90f) * ((float)Math.PI / 180f)) * 100f;
			float num3 = (0f - Mathf.Sin((num - 90f) * ((float)Math.PI / 180f))) * 100f;
			base.transform.position = new Vector3(middlePoint.x + num2, middlePoint.y + num3, 0f);
		}

		private float CalculateAngle()
		{
			Vector3 position = grenade.position;
			position.y = 0f;
			Vector3 position2 = pivot.position;
			position2.y = 0f;
			Vector3 forward = pivot.forward;
			forward.y = 0f;
			forward.Normalize();
			Vector3 normalized = (position - position2).normalized;
			float f = Mathf.Clamp(Vector3.Dot(forward, normalized), -1f, 1f);
			float num = Mathf.Acos(f) * 57.29578f;
			float num2 = forward.x * (0f - normalized.z) + forward.z * normalized.x;
			if (num2 < 0f)
			{
				num *= -1f;
			}
			return num;
		}

		private void UpdateAnimatorSpeed()
		{
			float flyingTime = grenadeScript.flyingTime;
			float explosionTime = grenadeScript.explosionTime;
			float value = flyingTime / explosionTime;
			float speed = Easing.Ease(EaseType.InCirc, 1f, 4f, value);
			alphaAnimator.speed = speed;
		}
	}
}
