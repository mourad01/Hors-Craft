// DecompilerFi decompiler from Assembly-CSharp.dll class: Weapons.BowCamera
using com.ootii.Cameras;
using Common.Behaviours.RotationEffects;
using Common.Utils;
using UnityEngine;

namespace Weapons
{
	public class BowCamera : MonoBehaviour
	{
		public LerpRotationEffect aimLerpModifier = new LerpRotationEffect();

		public float cameraFovZoomInDuration;

		public EaseType cameraFovZoomInEaseType;

		public float cameraFovZoomOutDuration;

		public EaseType cameraFovZoomOutEaseType;

		private const float ZOOM_VALUE = 20f;

		private float cameraFovMain;

		private float cameraFovZoom;

		private BowController weapon;

		private bool fovZooming;

		private float targetFov;

		private float zoomStartTime;

		private float zoomDuration;

		private EaseType easeType;

		private Camera mainCamera;

		private RotationEffectsController rotationsController;

		private void Awake()
		{
			weapon = GetComponent<BowController>();
			cameraFovMain = CameraController.instance.MainCamera.fieldOfView;
			cameraFovZoom = cameraFovMain - 20f;
			rotationsController = new RotationEffectsController(timeScaleIndependent: false);
			rotationsController.AddModifier(0, aimLerpModifier);
			FovStartZoomOut();
		}

		private void OnDestroy()
		{
			mainCamera.fieldOfView = cameraFovMain;
		}

		private void FixedUpdate()
		{
			if (mainCamera == null)
			{
				mainCamera = CameraController.instance.MainCamera;
			}
			cameraFovMain = CameraController.instance.MainCamera.fieldOfView;
			cameraFovZoom = cameraFovMain - 20f;
			if (weapon.IsShooting && !fovZooming)
			{
				FovStartZoomIn();
			}
			else if (!weapon.IsShooting && fovZooming)
			{
				FovStartZoomOut();
			}
			if (mainCamera.fieldOfView != targetFov)
			{
				FovUpdateZoom();
			}
		}

		private void FovStartZoomIn()
		{
			zoomStartTime = Time.time;
			zoomDuration = cameraFovZoomInDuration;
			easeType = cameraFovZoomInEaseType;
			fovZooming = true;
			targetFov = cameraFovZoom;
		}

		private void FovStartZoomOut()
		{
			zoomStartTime = Time.time;
			zoomDuration = cameraFovZoomOutDuration;
			easeType = cameraFovZoomOutEaseType;
			fovZooming = false;
			targetFov = cameraFovMain;
		}

		private void FovUpdateZoom()
		{
			float num = Mathf.Clamp((Time.time - zoomStartTime) / zoomDuration, 0f, 1f);
			if (fovZooming)
			{
				num = 1f - num;
			}
			mainCamera.fieldOfView = Easing.Ease(easeType, cameraFovZoom, cameraFovMain, num);
		}
	}
}
