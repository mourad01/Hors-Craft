// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.CarPhysics
using System;
using UnityEngine;

namespace DragMinigame
{
	public class CarPhysics : MonoBehaviour
	{
		[Serializable]
		public class CarBodyRotation
		{
			public float angle;

			public float damping;
		}

		public CarBodyRotation StartRotation;

		[SerializeField]
		private Transform RotatingPart;

		[SerializeField]
		private CarBodyRotation DrivingRotation;

		[SerializeField]
		private CarBodyRotation NitroRotation;

		[SerializeField]
		private CarBodyRotation BrakingRotation;

		[SerializeField]
		private Quaternion desiredRotation;

		private CarBodyRotation currentBodyRotation;

		private float elapsed;

		private float currentAngle;

		private float currentDamping;

		public void Init(Transform rotatingPart)
		{
			RotatingPart = rotatingPart;
		}

		private void Update()
		{
			if (desiredRotation.x != 0f)
			{
				elapsed += Time.deltaTime * currentDamping;
				RotatingPart.rotation = Quaternion.Lerp(RotatingPart.rotation, desiredRotation, elapsed);
			}
		}

		private void Updatevalues()
		{
			float angle = currentBodyRotation.angle;
			if (currentAngle != angle)
			{
				elapsed = 0f;
			}
			Quaternion rotation = RotatingPart.rotation;
			float x = rotation.x + angle;
			Quaternion rotation2 = RotatingPart.rotation;
			float y = rotation2.y;
			Quaternion rotation3 = RotatingPart.rotation;
			desiredRotation = Quaternion.Euler(new Vector3(x, y, rotation3.z));
			currentAngle = angle;
			currentDamping = currentBodyRotation.damping;
		}

		public void Rotate(float angle)
		{
			if (currentAngle != angle)
			{
				elapsed = 0f;
			}
			Quaternion rotation = RotatingPart.rotation;
			float x = rotation.x + angle;
			Quaternion rotation2 = RotatingPart.rotation;
			float y = rotation2.y;
			Quaternion rotation3 = RotatingPart.rotation;
			desiredRotation = Quaternion.Euler(new Vector3(x, y, rotation3.z));
			currentAngle = angle;
			currentDamping = 0.1f;
		}

		public void RotateDriving()
		{
			currentBodyRotation = DrivingRotation;
			Updatevalues();
		}

		public void RotateNitro()
		{
			currentBodyRotation = NitroRotation;
			Updatevalues();
		}

		public void RotateBraking()
		{
			currentBodyRotation = BrakingRotation;
			Updatevalues();
		}

		public void RotateStart()
		{
			currentBodyRotation = StartRotation;
			Updatevalues();
		}
	}
}
