// DecompilerFi decompiler from Assembly-CSharp.dll class: TankWeapon
using System;
using UnityEngine;

public class TankWeapon : WeaponHandler
{
	[Serializable]
	public class AngleRange
	{
		public float minRotation;

		public float maxRotation;

		private float? _normalMax;

		private float? normalHelpMin;

		private float? normalHelpMax;

		private float normalMin => minRotation;

		private float normalMax
		{
			get
			{
				if (!_normalMax.HasValue)
				{
					if (maxRotation < minRotation)
					{
						maxRotation = 360f + maxRotation;
					}
					if (maxRotation > 180f)
					{
						normalHelpMax = maxRotation - 360f;
						normalHelpMin = -180f;
						_normalMax = 180f;
					}
					else
					{
						_normalMax = maxRotation;
					}
				}
				return _normalMax.Value;
			}
		}

		public bool IsInRange(float angle)
		{
			bool flag = angle <= normalMax && angle >= normalMin;
			if (normalHelpMax.HasValue)
			{
				flag = (flag || (angle >= normalHelpMin.Value && angle <= normalHelpMax.Value));
			}
			return flag;
		}

		public override string ToString()
		{
			return $"normal min: {normalMin}; normal max: {normalMax}; help min: {normalHelpMin.GetValueOrDefault(0f)}; help max: {normalHelpMax.GetValueOrDefault(0f)};";
		}
	}

	public GameObject horizontalRotationObject;

	public GameObject verticalRotationObject;

	public float minRotationAngle = -180f;

	public float maxRotationAngle = 180f;

	public AngleRange canShootRange = new AngleRange
	{
		minRotation = -180f,
		maxRotation = 180f
	};

	public float maxRotationVertical = 5f;

	public bool active;

	public float shootForwardFromMuzzleDistance = 1f;

	public float verticalRotationMult = -15f;

	private const float MAX_ROTATION_SPEED = 70f;

	private const float MAX_ROTATION_X_SPEED = 50f;

	public bool canShoot
	{
		get
		{
			Vector3 eulerAngles = horizontalRotationObject.transform.localRotation.eulerAngles;
			float angle = NormalizeAngle(eulerAngles.y);
			return canShootRange.IsInRange(angle);
		}
	}

	protected override void Update()
	{
		base.Update();
		if (active)
		{
			UpdateActiveWeapon();
		}
	}

	public virtual void UpdateActiveWeapon()
	{
		Vector3 forward = base.cam.transform.forward;
		Quaternion lhs = Quaternion.LookRotation(base.cam.transform.forward, Vector3.up);
		if (horizontalRotationObject != null)
		{
			Quaternion to = Quaternion.LookRotation(new Vector3(forward.x, 0f, forward.z).normalized, Vector3.up);
			Vector3 eulerAngles = Quaternion.RotateTowards(horizontalRotationObject.transform.rotation, to, 70f * Time.deltaTime).eulerAngles;
			Vector3 eulerAngles2 = horizontalRotationObject.transform.rotation.eulerAngles;
			horizontalRotationObject.transform.rotation = Quaternion.Euler(new Vector3(eulerAngles2.x, eulerAngles.y, eulerAngles2.z));
			Vector3 eulerAngles3 = horizontalRotationObject.transform.localRotation.eulerAngles;
			if (verticalRotationObject != horizontalRotationObject)
			{
				eulerAngles3.x = 0f;
			}
			eulerAngles3.y = Mathf.Clamp(NormalizeAngle(eulerAngles3.y), minRotationAngle, maxRotationAngle);
			eulerAngles3.z = 0f;
			horizontalRotationObject.transform.localRotation = Quaternion.Euler(eulerAngles3);
		}
		if (verticalRotationObject != null)
		{
			lhs *= Quaternion.Euler(new Vector3(verticalRotationMult, 0f, 0f));
			Vector3 eulerAngles4 = Quaternion.RotateTowards(verticalRotationObject.transform.rotation, lhs, 50f * Time.deltaTime).eulerAngles;
			Vector3 eulerAngles5 = verticalRotationObject.transform.rotation.eulerAngles;
			verticalRotationObject.transform.rotation = Quaternion.Euler(new Vector3(eulerAngles4.x, eulerAngles5.y, eulerAngles5.z));
			Vector3 eulerAngles6 = verticalRotationObject.transform.localRotation.eulerAngles;
			eulerAngles6.x = Mathf.Clamp(NormalizeAngle(eulerAngles6.x), 0f - maxRotationVertical, maxRotationVertical);
			if (horizontalRotationObject != verticalRotationObject)
			{
				eulerAngles6.y = 0f;
			}
			eulerAngles6.z = 0f;
			verticalRotationObject.transform.localRotation = Quaternion.Euler(eulerAngles6);
		}
	}

	protected override void UpdateCrosshair()
	{
		if (active && !(base.crosshairToMove == null))
		{
			Vector3 zero = Vector3.zero;
			Ray ray = new Ray(muzzle.transform.position + muzzle.transform.forward * shootForwardFromMuzzleDistance, muzzle.transform.forward);
			zero = ((!Physics.Raycast(ray, out RaycastHit hitInfo, distance, int.MaxValue, QueryTriggerInteraction.Ignore)) ? (muzzle.transform.position + muzzle.transform.forward * distance) : hitInfo.point);
			Vector3 v = base.cam.WorldToScreenPoint(zero);
			base.crosshairToMove.transform.position = v.XY();
		}
	}

	protected override void ShowReloadCooldown()
	{
		if (active)
		{
			base.ShowReloadCooldown();
		}
	}

	private float NormalizeAngle(float angle)
	{
		float num;
		for (num = angle; num > 180f; num -= 360f)
		{
		}
		for (; num < -180f; num += 360f)
		{
		}
		return num;
	}
}
