// DecompilerFi decompiler from Assembly-CSharp.dll class: AngleMotor
using Common.Utils;
using System;
using UnityEngine;

public class AngleMotor : MonoBehaviour
{
	[Serializable]
	private struct TurnRates
	{
		[Range(0f, 1f)]
		public int xTurnRate;

		[Range(0f, 1f)]
		public int yTurnRate;

		[Range(0f, 1f)]
		public int zTurnRate;
	}

	private float goBackToVerticalPositionSpeed = 5f;

	[SerializeField]
	[Range(-1f, 1f)]
	private int turnDirection;

	[SerializeField]
	private TurnRates turnRates;

	[SerializeField]
	private float maxAngle = 30f;

	public void AngleObject(float input, float velocity)
	{
		Vector3 localEulerAngles = base.transform.localEulerAngles;
		if (turnRates.xTurnRate == 1 && (localEulerAngles.x < maxAngle || localEulerAngles.x > 360f - maxAngle))
		{
			base.transform.Rotate((float)turnDirection * input, 0f, 0f);
		}
		else if (turnRates.yTurnRate == 1 && (localEulerAngles.y < maxAngle || localEulerAngles.y > 360f - maxAngle))
		{
			base.transform.Rotate(0f, (float)turnDirection * input, 0f);
		}
		else if (turnRates.zTurnRate == 1 && (localEulerAngles.z < maxAngle || localEulerAngles.z > 360f - maxAngle))
		{
			float num = Easing.Ease(EaseType.OutQuad, 0f, 1f, velocity / 10f);
			float z = maxAngle * num * (0f - input);
			base.transform.localRotation = Quaternion.Slerp(base.transform.localRotation, Quaternion.Euler(0f, 0f, z), Time.fixedDeltaTime * 3f);
		}
	}
}
