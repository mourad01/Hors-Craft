// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.IsometricCamera
using Common.Utils;
using GameUI;
using System;
using UnityEngine;

namespace Uniblocks
{
	public class IsometricCamera : MonoBehaviour
	{
		[HideInInspector]
		public SimpleRepeatButton dragButton;

		[HideInInspector]
		public Transform pivot;

		public bool automaticMovement;

		private AnimationCurve cameraMovement;

		public float cameraAngleSpeed = 10f;

		public float cameraSpeed = 0.5f;

		public float angle = 60f;

		private Vector3 destPosition;

		private Vector3 lastTouchPos;

		private bool wasLastPressed;

		private float currentYAngle;

		private float groundDistance;

		private float modifiedHeight;

		private const float MIN_HEIGHT = 6f;

		private const float TIME_STEP = 0.02f;

		private float timer;

		private float deltaTime;

		private float previousTime;

		private float automaticTimer;

		private Vector3 lastTouchScreenPos;

		private void Awake()
		{
			destPosition = base.transform.position;
		}

		public void Init(SimpleRepeatButton dragButton, Transform pivot, float objectHeight = 16f)
		{
			this.dragButton = dragButton;
			this.pivot = pivot;
			modifiedHeight = objectHeight * 0.7f + 6f;
			groundDistance = modifiedHeight * Mathf.Tan((float)Math.PI / 180f * (90f - angle));
			timer = 0f;
			deltaTime = 0f;
			previousTime = Time.realtimeSinceStartup;
			base.transform.position = pivot.position;
		}

		public void Init(AnimationCurve animationCurve, Transform pivot, float objectHeight = 16f)
		{
			cameraMovement = animationCurve;
			automaticMovement = true;
			this.pivot = pivot;
			modifiedHeight = objectHeight * 0.7f + 6f;
			groundDistance = modifiedHeight * Mathf.Tan((float)Math.PI / 180f * (90f - angle));
			timer = 0f;
			deltaTime = 0f;
			previousTime = Time.realtimeSinceStartup;
			base.transform.position = pivot.position;
		}

		private void Update()
		{
			deltaTime = Time.realtimeSinceStartup - previousTime;
			timer += deltaTime;
			automaticTimer += deltaTime;
			previousTime = Time.realtimeSinceStartup;
			if (automaticMovement)
			{
				UpdateAutomatic();
			}
			else
			{
				if (dragButton.pressed)
				{
					if (!wasLastPressed)
					{
						StartPress();
					}
					UpdatePressed();
				}
				wasLastPressed = dragButton.pressed;
			}
			UpdateDestPosition();
			while (timer >= 0.02f)
			{
				UpdatePositionAndRotation();
				timer -= 0.02f;
			}
		}

		private void UpdatePositionAndRotation()
		{
			Vector3 normalized = (pivot.position - base.transform.position).normalized;
			Quaternion quaternion = Quaternion.LookRotation(normalized);
			if (automaticMovement)
			{
				base.transform.rotation = quaternion;
			}
			else
			{
				float f = Quaternion.Dot(base.transform.rotation, quaternion);
				float maxDegreesDelta = Easing.Ease(EaseType.InQuint, 8f, 0.2f, Mathf.Abs(f));
				base.transform.rotation = Quaternion.RotateTowards(base.transform.rotation, quaternion, maxDegreesDelta);
			}
			SmoothMoveCamera(0.05f, cameraSpeed);
		}

		private void UpdateAutomatic()
		{
			currentYAngle = cameraMovement.Evaluate(automaticTimer);
		}

		private void StartPress()
		{
			lastTouchPos = GetTouchScreenPos();
		}

		private void UpdateDestPosition()
		{
			destPosition = pivot.position;
			destPosition.x += Mathf.Sin((float)Math.PI / 180f * currentYAngle) * groundDistance;
			destPosition.z -= Mathf.Cos((float)Math.PI / 180f * currentYAngle) * groundDistance;
			destPosition.y += modifiedHeight;
		}

		private void UpdatePressed()
		{
			Vector3 touchScreenPos = GetTouchScreenPos();
			Vector3 vector = touchScreenPos - lastTouchPos;
			lastTouchPos = touchScreenPos;
			float num = vector.x / Screen.dpi * cameraAngleSpeed;
			currentYAngle -= num;
		}

		private void SmoothMoveCamera(float deltaTime, float cameraSpeed)
		{
			base.transform.position = Vector3.Lerp(base.transform.position, destPosition, deltaTime * cameraSpeed);
		}

		private Vector3 GetTouchScreenPos()
		{
			Touch[] touches = Input.touches;
			for (int i = 0; i < touches.Length; i++)
			{
				Touch touch = touches[i];
				if (touch.phase != TouchPhase.Canceled && touch.phase != TouchPhase.Ended && touch.fingerId == dragButton.fingerIds[0])
				{
					lastTouchScreenPos = touch.position;
					break;
				}
			}
			return lastTouchScreenPos;
		}
	}
}
