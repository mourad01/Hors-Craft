// DecompilerFi decompiler from Assembly-CSharp.dll class: FreeRotationFollowCamera
using GameUI;
using System;
using UnityEngine;

[Serializable]
public class FreeRotationFollowCamera
{
	[Serializable]
	public class CameraConfig
	{
		public float xAngle = 24.16f;

		public float startYAngle = 160f;

		public float distance = 5f;

		public float cameraAutoAngleSpeed = 10f;

		public float cameraAutoSmoothSpeed = 5f;

		public float cameraManualAngleSpeed = 40f;

		public float cameraManualSmoothSpeed = 5f;

		public float steadyAfterTouchDuration = 0.25f;

		public float steadyAfterTouchSmoothDuration = 2f;

		public Vector3 lookAtOffset = new Vector3(0f, 0.52f, 0f);

		public float cameraXAngleMin = 12f;

		public float cameraXAngleMax = 60f;
	}

	public Transform pivot;

	public Func<SimpleRepeatButton, SimpleRepeatButton> dragButton;

	private SimpleRepeatButton button;

	public Transform cameraController;

	public bool autoRotationEnabled = true;

	public float perspectiveZoomSpeed = 0.05f;

	public float orthoZoomSpeed = 0.5f;

	private CameraConfig config;

	private float currentYAngle;

	private float yAngleVelocity;

	private float lastPressedTime;

	private float currentXAngle;

	private float xAngleVelocity;

	private Camera cam;

	private float lastTime;

	private bool wasLastPressed;

	private Vector3 lastTouchPos;

	private float lastPressedVelocity;

	private float lastPressedVelocitySign = -1f;

	private float lastPressedXVelocity;

	private float lastPressedXVelocitySign = -1f;

	private Vector3 lastTouchScreenPos;

	private Vector3 destPosition;

	private Quaternion destRotation;

	public FreeRotationFollowCamera(CameraConfig config, Transform pivot, Transform camera, Func<SimpleRepeatButton, SimpleRepeatButton> drag)
	{
		this.config = config;
		this.pivot = pivot;
		cameraController = camera;
		dragButton = drag;
		currentYAngle = config.startYAngle;
		currentXAngle = config.xAngle;
		yAngleVelocity = 0f;
		xAngleVelocity = 0f;
		lastTime = Time.realtimeSinceStartup;
		UpdateUnpressed(1f);
		PrepareDestPositionAndRotation();
		cameraController.transform.position = destPosition;
		cameraController.transform.rotation = destRotation;
		cam = cameraController.GetComponentInChildren<Camera>();
		cam.fieldOfView = 70f;
	}

	public void ChangePivot(Transform pivot)
	{
		this.pivot = pivot;
	}

	public void ChangeConfig(CameraConfig config, bool resetAngle = false)
	{
		this.config = config;
		if (resetAngle)
		{
			currentYAngle = config.startYAngle;
			currentXAngle = config.xAngle;
			yAngleVelocity = 0f;
			xAngleVelocity = 0f;
		}
	}

	public void Update()
	{
		float deltaTime = Time.realtimeSinceStartup - lastTime;
		lastTime = Time.realtimeSinceStartup;
		if (dragButton(button).pressed)
		{
			if (!wasLastPressed)
			{
				StartPress();
			}
			UpdatePressed(deltaTime);
		}
		else
		{
			UpdateUnpressed(deltaTime);
		}
		wasLastPressed = dragButton(button).pressed;
	}

	public void FixedSmoothMove()
	{
		PrepareDestPositionAndRotation();
		SmoothMoveCamera(Time.fixedDeltaTime, (!wasLastPressed) ? config.cameraAutoSmoothSpeed : config.cameraManualSmoothSpeed);
	}

	private void StartPress()
	{
		lastTouchPos = GetTouchScreenPos();
		yAngleVelocity = 0f;
		xAngleVelocity = 0f;
		Vector3 forward = cameraController.transform.position - pivot.position;
		Quaternion quaternion = Quaternion.LookRotation(forward);
		Vector3 eulerAngles = quaternion.eulerAngles;
		currentXAngle = 0f - NormalizeAngle(eulerAngles.x);
		Vector3 eulerAngles2 = quaternion.eulerAngles;
		float num = NormalizeAngle(eulerAngles2.y - 180f);
		Vector3 eulerAngles3 = pivot.transform.rotation.eulerAngles;
		currentYAngle = num - eulerAngles3.y;
	}

	private void UpdatePressed(float deltaTime)
	{
		Vector3 touchScreenPos = GetTouchScreenPos();
		Vector3 vector = touchScreenPos - lastTouchPos;
		float num = vector.x / Screen.dpi * config.cameraManualAngleSpeed;
		float num2 = vector.y / Screen.dpi * config.cameraManualAngleSpeed;
		currentYAngle += num;
		if ((num2 < 0f && currentXAngle - num2 > config.cameraXAngleMax) || (num2 > 0f && currentXAngle - num2 < config.cameraXAngleMin))
		{
			num2 = 0f;
		}
		currentXAngle -= num2;
		lastTouchPos = touchScreenPos;
		lastPressedTime = Time.realtimeSinceStartup;
		lastPressedVelocity = vector.x;
		lastPressedVelocitySign = Mathf.Sign(yAngleVelocity);
		lastPressedXVelocity = vector.y;
		lastPressedXVelocitySign = Mathf.Sign(xAngleVelocity);
	}

	private Vector3 GetTouchScreenPos()
	{
		Touch[] touches = Input.touches;
		for (int i = 0; i < touches.Length; i++)
		{
			Touch touch = touches[i];
			if (touch.phase != TouchPhase.Canceled && touch.phase != TouchPhase.Ended && touch.fingerId == dragButton(button).fingerIds[0])
			{
				lastTouchScreenPos = touch.position;
				break;
			}
		}
		return lastTouchScreenPos;
	}

	private void UpdateUnpressed(float deltaTime)
	{
		float num = lastPressedTime + config.steadyAfterTouchDuration;
		if (Time.realtimeSinceStartup < num)
		{
			yAngleVelocity += (0f - yAngleVelocity) * Mathf.Min(1f, deltaTime * 2f);
			xAngleVelocity += (0f - xAngleVelocity) * Mathf.Min(1f, deltaTime * 2f);
			return;
		}
		lastPressedVelocity += (0f - lastPressedVelocity) * Mathf.Min(1f, deltaTime * 2f);
		lastPressedXVelocity += (0f - lastPressedXVelocity) * Mathf.Min(1f, deltaTime * 2f);
		float a = Time.realtimeSinceStartup - num;
		float t = Mathf.Min(a, config.steadyAfterTouchSmoothDuration) / config.steadyAfterTouchSmoothDuration;
		yAngleVelocity = Mathf.Lerp(lastPressedVelocity, lastPressedVelocitySign * config.cameraAutoAngleSpeed, t);
		xAngleVelocity = Mathf.Lerp(lastPressedXVelocity, lastPressedXVelocitySign * config.cameraAutoAngleSpeed, t);
	}

	private void PrepareDestPositionAndRotation()
	{
		Vector2 vector = new Vector2(currentXAngle, currentYAngle);
		float distance = config.distance;
		Vector3 point = -pivot.forward * distance;
		Quaternion rhs = Quaternion.AngleAxis(vector.x, pivot.right);
		Quaternion lhs = Quaternion.AngleAxis(vector.y, Vector3.up);
		point = lhs * rhs * point;
		Vector3 a = pivot.position + config.lookAtOffset;
		destPosition = pivot.position + point;
		destRotation = Quaternion.LookRotation(a - cameraController.transform.position);
		Vector3 eulerAngles = destRotation.eulerAngles;
		eulerAngles.x = Mathf.Clamp(NormalizeAngle(eulerAngles.x), config.cameraXAngleMin, config.cameraXAngleMax);
		destRotation = Quaternion.Euler(eulerAngles);
	}

	private void SmoothMoveCamera(float deltaTime, float cameraSpeed)
	{
		cameraController.transform.position = Vector3.Lerp(cameraController.transform.position, destPosition, deltaTime * cameraSpeed);
		Quaternion rotation = cameraController.transform.rotation;
		Vector3 eulerAngles = rotation.eulerAngles;
		float x = eulerAngles.x;
		Vector3 eulerAngles2 = destRotation.eulerAngles;
		float num = Mathf.LerpAngle(x, eulerAngles2.x, deltaTime * cameraSpeed);
		Vector3 eulerAngles3 = rotation.eulerAngles;
		float y = eulerAngles3.y;
		Vector3 eulerAngles4 = destRotation.eulerAngles;
		float num2 = Mathf.LerpAngle(y, eulerAngles4.y, deltaTime * cameraSpeed);
		Transform transform = cameraController.transform;
		float x2 = num;
		float y2 = num2;
		Vector3 eulerAngles5 = rotation.eulerAngles;
		transform.rotation = Quaternion.Euler(x2, y2, eulerAngles5.z);
		cameraController.transform.rotation = destRotation;
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
