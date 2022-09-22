// DecompilerFi decompiler from Assembly-CSharp.dll class: GameUI.MouseLookController
using UnityEngine;

namespace GameUI
{
	public class MouseLookController : MonoBehaviour
	{
		public enum RotationAxes
		{
			MouseXAndY,
			MouseX,
			MouseY
		}

		private struct CurrentInputInfo
		{
			public TouchPhase phase;

			public Vector3 position;
		}

		public SimpleRepeatButton cameraButton;

		public Transform transformToRotateInYAxis;

		public float sensitivityX = 180f;

		public float sensitivityY = 90f;

		public float mouseSensitivityX = 0.5f;

		public float mouseSensitivityY = 0.25f;

		public float smoothSpeed = 2f;

		public int maxXRotationAngle;

		public int minXRotationAngle;

		public AnimationCurve smoothMove = AnimationCurve.Linear(0f, 0f, 1f, 1f);

		public float inertiaDistance = 20f;

		private Vector2 rotationVelocity = Vector2.zero;

		private Vector2 smoothedRotationVelocity = Vector2.zero;

		private Camera mainCamera;

		private Vector3 lastTouchPoint;

		private bool isActive = true;

		public Vector3 direction;

		private void Awake()
		{
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.MOVEMENT, new CameraRotationContext
			{
				setCameraRotationButton = delegate(SimpleRepeatButton cr)
				{
					cameraButton = cr;
				}
			});
		}

		public void SetActive(bool active)
		{
			isActive = active;
		}

		private void Update()
		{
			if (mainCamera == null)
			{
				mainCamera = Camera.main;
			}
			else if (!(cameraButton == null) && isActive)
			{
				UpdateCameraButtonInput();
				Vector3 forward = mainCamera.transform.forward;
				direction = Vector3.Normalize(forward);
				UpdateSmoothedRotation();
			}
		}

		private void UpdateMouseInput()
		{
			rotationVelocity.x = UnityEngine.Input.GetAxis("Mouse X") * mouseSensitivityX;
			rotationVelocity.y = (0f - UnityEngine.Input.GetAxis("Mouse Y")) * mouseSensitivityY;
			Smooth();
		}

		private void UpdateCameraButtonInput()
		{
			if (cameraButton.pressed)
			{
				CurrentInputInfo currentInputInfo = ConstructInputInfo();
				if (currentInputInfo.phase == TouchPhase.Began)
				{
					lastTouchPoint = currentInputInfo.position;
				}
				if (currentInputInfo.phase == TouchPhase.Moved)
				{
					Vector3 position = currentInputInfo.position;
					float num = lastTouchPoint.x - position.x;
					float num2 = lastTouchPoint.y - position.y;
					float num3 = num / (float)Screen.width;
					float num4 = num2 / (float)Screen.height;
					float num5 = (0f - num3) * sensitivityX;
					float num6 = num4 * sensitivityY;
					lastTouchPoint = position;
					rotationVelocity.x += num5;
					rotationVelocity.y += num6;
					Smooth();
				}
			}
		}

		private void Smooth()
		{
			float num = inertiaDistance / (float)Screen.width * sensitivityX;
			float num2 = inertiaDistance / (float)Screen.height * sensitivityY;
			if (Mathf.Abs(rotationVelocity.x) <= num)
			{
				smoothedRotationVelocity.x = smoothMove.Evaluate(Mathf.Abs(rotationVelocity.x) / num) * num * Mathf.Sign(rotationVelocity.x);
			}
			else
			{
				smoothedRotationVelocity.x = rotationVelocity.x;
			}
			if (Mathf.Abs(rotationVelocity.y) <= num2)
			{
				smoothedRotationVelocity.y = smoothMove.Evaluate(Mathf.Abs(rotationVelocity.y) / num2) * num2 * Mathf.Sign(rotationVelocity.y);
			}
			else
			{
				smoothedRotationVelocity.y = rotationVelocity.y;
			}
		}

		private void UpdateSmoothedRotation()
		{
			if (rotationVelocity.magnitude > 0.01f)
			{
				transformToRotateInYAxis.Rotate(0f, smoothedRotationVelocity.x / 0.02f * Time.deltaTime, 0f, Space.World);
				base.transform.Rotate(smoothedRotationVelocity.y / 0.02f * Time.deltaTime, 0f, 0f, Space.Self);
				float num = Vector3.Dot(base.transform.forward, transformToRotateInYAxis.forward);
				if (num < 0f)
				{
					base.transform.Rotate((0f - smoothedRotationVelocity.y / 0.02f) * Time.deltaTime, 0f, 0f, Space.Self);
				}
			}
			rotationVelocity *= smoothSpeed * Time.deltaTime;
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
				Touch touch = UnityEngine.Input.GetTouch(i);
				if (touch.fingerId == cameraButton.fingerId)
				{
					return touch;
				}
			}
			return null;
		}

		private void Start()
		{
			base.transform.rotation = Quaternion.identity;
			if ((bool)GetComponent<Rigidbody>())
			{
				GetComponent<Rigidbody>().freezeRotation = true;
			}
		}
	}
}
