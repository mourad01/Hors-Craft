// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.SurvivalAnalogInput
using GameUI;
using UnityEngine;

namespace Gameplay
{
	public class SurvivalAnalogInput : MonoBehaviour
	{
		private struct CurrentInputInfo
		{
			public TouchPhase phase;

			public Vector3 position;
		}

		private const float rotationSpeedX = 180f;

		private const float rotationSpeedY = 90f;

		private const float smoothSpeed = 5f;

		[HideInInspector]
		public AnalogController analog;

		[HideInInspector]
		public SimpleRepeatButton analogStick;

		private MouseLookController lookController;

		private Vector3 targetEuler;

		private bool touching;

		private bool isActive;

		private bool isRotating;

		public void SetActive(bool active)
		{
			isActive = active;
		}

		public void SetAnalog(AnalogController controller, SimpleRepeatButton stick)
		{
			analog = controller;
			analogStick = stick;
		}

		public bool IsTouching()
		{
			return touching;
		}

		private void Awake()
		{
			touching = false;
			isActive = false;
			lookController = GetComponentInChildren<MouseLookController>();
		}

		public void Reset()
		{
			touching = false;
			isRotating = false;
		}

		private void Update()
		{
			if (isActive && !analogStick.pressed)
			{
				analog.SetGaugeCenterPositon();
			}
			if (!isActive || !analogStick.pressed)
			{
				Reset();
				lookController.SetActive(active: true);
				return;
			}
			lookController.SetActive(active: false);
			UpdateTouch();
			if (isRotating)
			{
				UpdateCurrentRotation();
			}
		}

		private void UpdateTouch()
		{
			if (!touching)
			{
				if (analogStick.pressed)
				{
					PerformTouchStart();
					touching = true;
				}
			}
			else if (analogStick.pressed)
			{
				PerformTouchContinue();
				isRotating = true;
			}
			else
			{
				isRotating = false;
				touching = false;
			}
		}

		private bool IsCorrectTouchDown()
		{
			return analogStick.pressed;
		}

		private bool IsCorrectTouchUp()
		{
			return Input.GetMouseButtonUp(0);
		}

		private void PerformTouchStart()
		{
			targetEuler = base.transform.localEulerAngles;
			analog.UpdateAnalogControllerPosition();
		}

		private void PerformTouchContinue()
		{
			CurrentInputInfo currentInputInfo = ConstructInputInfo();
			Vector2 fingerOffset = (Vector2)currentInputInfo.position - analog.center;
			analog.SetFingerOffset(fingerOffset);
			Vector2 vector = new Vector2(fingerOffset.x / (float)Screen.width, fingerOffset.y / (float)Screen.height);
			float x = vector.x;
			float num = Screen.width;
			Vector2 center = analog.center;
			float min = 0f - (num - center.x) / (float)Screen.width;
			float num2 = Screen.width;
			Vector2 center2 = analog.center;
			float x2 = Mathf.Clamp(x, min, (num2 - center2.x) / (float)Screen.width);
			float y = vector.y;
			Vector2 center3 = analog.center;
			float min2 = 0f - center3.y / (float)Screen.height;
			Vector2 center4 = analog.center;
			Vector2 vector2 = new Vector2(x2, Mathf.Clamp(y, min2, center4.y / (float)Screen.height));
			Vector2 vector3 = new Vector2(vector2.x * 180f, vector2.y * 90f);
			Vector3 b = new Vector3(0f - vector3.y, vector3.x, 0f);
			Vector3 localEulerAngles = lookController.transform.localEulerAngles;
			float x3 = localEulerAngles.x;
			Vector3 localEulerAngles2 = base.transform.localEulerAngles;
			float y2 = localEulerAngles2.y;
			Vector3 localEulerAngles3 = base.transform.localEulerAngles;
			Vector3 a = new Vector3(x3, y2, localEulerAngles3.z);
			RotateTo(a + b);
		}

		private void RotateTo(Vector3 euler)
		{
			euler.x = ClampAngle(euler.x, -90f, 90f);
			targetEuler = euler;
		}

		private float ClampAngle(float angle, float from, float to)
		{
			if (angle > 180f)
			{
				angle -= 360f;
			}
			angle = Mathf.Clamp(angle, from, to);
			if (angle < 0f)
			{
				angle = 360f + angle;
			}
			return angle;
		}

		private void UpdateCurrentRotation()
		{
			Vector3 localEulerAngles = base.transform.localEulerAngles;
			Vector3 localEulerAngles2 = lookController.transform.localEulerAngles;
			Vector2 a = new Vector2(Mathf.Abs(Mathf.DeltaAngle(localEulerAngles2.x, targetEuler.x)), Mathf.Abs(Mathf.DeltaAngle(localEulerAngles.y, targetEuler.y)));
			a *= Time.deltaTime * 5f;
			localEulerAngles.y = Mathf.MoveTowardsAngle(localEulerAngles.y, targetEuler.y, a.y);
			localEulerAngles2.x = Mathf.MoveTowardsAngle(localEulerAngles2.x, targetEuler.x, a.x);
			base.transform.localEulerAngles = localEulerAngles;
			lookController.transform.localEulerAngles = localEulerAngles2;
		}

		private CurrentInputInfo ConstructInputInfo()
		{
			CurrentInputInfo result = default(CurrentInputInfo);
			Touch? touch = FindCurrentTouch();
			if (touch.HasValue)
			{
				result.phase = touch.Value.phase;
				result.position = touch.Value.position;
			}
			return result;
		}

		private Touch? FindCurrentTouch()
		{
			for (int i = 0; i < UnityEngine.Input.touchCount; i++)
			{
				if (UnityEngine.Input.GetTouch(i).fingerId == analogStick.fingerId)
				{
					return UnityEngine.Input.GetTouch(i);
				}
			}
			return null;
		}
	}
}
