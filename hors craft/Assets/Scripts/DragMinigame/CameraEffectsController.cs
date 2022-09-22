// DecompilerFi decompiler from Assembly-CSharp.dll class: DragMinigame.CameraEffectsController
using com.ootii.Cameras;
using Common.Utils;
using System.Collections;
using UnityEngine;

namespace DragMinigame
{
	public class CameraEffectsController : MonoBehaviour
	{
		[Header("Shake Variables")]
		[Header("Engine shake")]
		[SerializeField]
		private float tachoShakeThreshold = 1f;

		[SerializeField]
		private float speedShakeThreshold = 90f;

		[SerializeField]
		private float maxEngineShakeAmount = 2f;

		[SerializeField]
		private float nitroShakeAmount = 2f;

		private Transform engineShakeTransform;

		private Vector3 engineShakeTransformOriginalPos;

		private float currentEngineShakeAmount;

		private float currentEngineShakeSpeed;

		private bool shakeEngine;

		[Header("Gear Change Shake")]
		[SerializeField]
		private float gearShakeTime;

		[SerializeField]
		private float maxGearShakeAmount;

		[Header("Bump Variables")]
		[Space(7f)]
		[SerializeField]
		private float minBumpAmount = 0.5f;

		[SerializeField]
		private float maxBumpAmount = 1.5f;

		[SerializeField]
		private AnimationCurve bumpCurve;

		[SerializeField]
		private float bumpTime;

		[SerializeField]
		private float nitroAccelerationBumpAmount;

		private float lastPlayerZbump;

		private float lastEnemyZbump;

		private bool bumpPlayer;

		private bool bumpEnemy;

		private float playerBumpAmount;

		private float enemyBumpAmount;

		private float elapsedPlayerBumpTime;

		private float elapsedEnemyBumpTime;

		[SerializeField]
		public Transform followtarget;

		[SerializeField]
		private AnimationCurve nitroEffectCurve;

		[Header("Fov Variables")]
		[Header("Gear Change")]
		[SerializeField]
		private AnimationCurve gearFovCurve;

		[SerializeField]
		private float changeGearFovAmount;

		[SerializeField]
		private float fovTime;

		private float elapsedFovTime;

		private float lastFov;

		private bool fovBump;

		[Header("Breaking")]
		[SerializeField]
		private float targetBreakingFov;

		[SerializeField]
		private float breakingFovSpeed;

		[Header("Nitro")]
		[SerializeField]
		private AnimationCurve nitroFovCurve;

		[SerializeField]
		private float nitroFovAmount;

		private float originalFov;

		private float elapsedNitroTime;

		private bool isNitro;

		[HideInInspector]
		public CameraController cameraController;

		private DragRacingGameManager gameManager;

		private Transform mainCameraRig;

		private Camera mainCamera;

		private float currentSpeed;

		[SerializeField]
		private Transform carRenderer;

		[SerializeField]
		private Transform enemyRenderer;

		[Header("Camera Rotation")]
		[SerializeField]
		private float maxRotation;

		[SerializeField]
		private float maxRotationTime;

		[SerializeField]
		private float minrotationTime;

		private float targetRotationTime;

		private float currentRotationTime;

		private float elapsedRotationTime;

		private float targetZrotation;

		private Quaternion lastRotation;

		private Quaternion targetRotation;

		private bool isRotating;

		public void Init()
		{
			cameraController = CameraController.instance;
			mainCameraRig = cameraController.Transform;
			mainCamera = Camera.main;
			engineShakeTransform = mainCamera.transform;
			gameManager = DragRacingGameManager.dragRacingManagerInstance;
			originalFov = mainCamera.fieldOfView;
		}

		public void UpdateValues(float currentTacho, float currentSpeed, bool isFinished)
		{
			this.currentSpeed = currentSpeed;
			float target = 0f;
			if (gameManager.isBoostOn)
			{
				target = nitroShakeAmount;
			}
			else if (currentTacho >= tachoShakeThreshold || currentSpeed >= speedShakeThreshold)
			{
				target = currentTacho / 8f * maxEngineShakeAmount;
			}
			currentEngineShakeAmount = (currentEngineShakeSpeed = Mathf.MoveTowards(currentEngineShakeSpeed, target, Time.deltaTime * 5f));
		}

		public void UpdateEffects()
		{
			switch (gameManager.gameState)
			{
			case DragGameManager.GameState.Init:
				break;
			case DragGameManager.GameState.Start:
				HandleRandomCameraRotation();
				break;
			case DragGameManager.GameState.Race:
				HandleRandomCameraRotation();
				HandleGearFovBump();
				HandleGearBump();
				HandleEngineShake();
				HandleNitro();
				break;
			case DragGameManager.GameState.Finish:
				HandleBrakingFov();
				HandleEngineShake();
				break;
			}
		}

		public void StartShake()
		{
			shakeEngine = true;
			engineShakeTransformOriginalPos = engineShakeTransform.localPosition;
		}

		private void ShakeGearChange(float prc)
		{
			float amount = maxGearShakeAmount * prc;
			CustomShake(amount, gearShakeTime, 5f);
		}

		private void HandleEngineShake()
		{
			if (shakeEngine)
			{
				Vector3 b = engineShakeTransformOriginalPos + UnityEngine.Random.insideUnitSphere * currentEngineShakeAmount;
				engineShakeTransform.localPosition = Vector3.Lerp(engineShakeTransform.localPosition, b, Time.deltaTime * currentEngineShakeSpeed);
			}
		}

		public void CustomShake(float amount, float time, float speed)
		{
			StartCoroutine(CustomShakeCO(amount, time, speed));
		}

		private IEnumerator CustomShakeCO(float _amount, float _time, float _speed)
		{
			shakeEngine = false;
			Vector3 originalPos = engineShakeTransform.localPosition;
			_amount += currentEngineShakeAmount;
			float elapsedTime = 0f;
			while (elapsedTime < _time)
			{
				Vector3 randomPos = originalPos + UnityEngine.Random.insideUnitSphere * _amount;
				engineShakeTransform.localPosition = Vector3.Lerp(engineShakeTransform.localPosition, randomPos, Time.deltaTime * _speed);
				_amount -= Time.deltaTime;
				elapsedTime += Time.deltaTime;
				yield return null;
			}
			shakeEngine = true;
		}

		public void CameraBump(float prc, bool player)
		{
			if (!bumpPlayer && player)
			{
				playerBumpAmount = minBumpAmount + (maxBumpAmount - minBumpAmount) * prc;
				elapsedPlayerBumpTime = 0f;
				bumpPlayer = true;
				Vector3 localPosition = carRenderer.localPosition;
				lastPlayerZbump = localPosition.z;
			}
			else if (!bumpEnemy && !player)
			{
				enemyBumpAmount = minBumpAmount + (maxBumpAmount - minBumpAmount) * prc;
				elapsedEnemyBumpTime = 0f;
				bumpEnemy = true;
				Vector3 localPosition2 = enemyRenderer.localPosition;
				lastEnemyZbump = localPosition2.z;
			}
		}

		private void HandleGearBump()
		{
			HandleGearBumpFor(carRenderer, playerBumpAmount, ref bumpPlayer, ref elapsedPlayerBumpTime, ref lastPlayerZbump);
			HandleGearBumpFor(enemyRenderer, enemyBumpAmount, ref bumpEnemy, ref elapsedEnemyBumpTime, ref lastEnemyZbump);
		}

		private void HandleGearBumpFor(Transform car, float amount, ref bool bump, ref float elapsedTime, ref float lastZbump)
		{
			if (bump)
			{
				elapsedTime += Time.deltaTime;
				float num = bumpCurve.Evaluate(elapsedTime / bumpTime) * amount;
				car.localPosition = new Vector3(0f, 0f, lastZbump - num);
				if (elapsedTime > bumpTime)
				{
					bump = false;
				}
			}
		}

		public void NitroFov()
		{
			if (!isNitro)
			{
				elapsedNitroTime = 0f;
				isNitro = true;
			}
		}

		public void FovBump()
		{
			if (!fovBump)
			{
				elapsedFovTime = 0f;
				lastFov = mainCamera.fieldOfView;
				fovBump = true;
			}
		}

		private void HandleBrakingFov()
		{
			float value = Mathf.InverseLerp(200f, 0f, currentSpeed);
			mainCamera.fieldOfView = Easing.Ease(EaseType.OutBack, lastFov, targetBreakingFov, value);
		}

		private void HandleNitroFov()
		{
			float num = nitroFovCurve.Evaluate(elapsedNitroTime / gameManager.GetBoostTime()) * nitroFovAmount;
			float fieldOfView = originalFov + num;
			mainCamera.fieldOfView = fieldOfView;
		}

		private void HandleGearFovBump()
		{
			if (fovBump)
			{
				elapsedFovTime += Time.deltaTime;
				float num = gearFovCurve.Evaluate(elapsedFovTime / fovTime) * changeGearFovAmount;
				float fieldOfView = lastFov + num;
				mainCamera.fieldOfView = fieldOfView;
				if (elapsedFovTime > fovTime)
				{
					fovBump = false;
				}
			}
		}

		public void ActivateTransitionCamera(string transitionName, int startIndex = -1)
		{
			TransitionMotor motor = cameraController.GetMotor<TransitionMotor>(transitionName);
			if (startIndex >= 0)
			{
				motor.StartMotorIndex = startIndex;
			}
			cameraController.ActivateMotor(motor);
		}

		public string GetActiveMotorName()
		{
			return cameraController.ActiveMotor.Name;
		}

		public void ChangeGear(float prc, bool isPlayer)
		{
			CameraBump(prc, isPlayer);
			if (prc <= 0f)
			{
				FovBump();
			}
			else if (isPlayer)
			{
				ShakeGearChange(prc);
			}
		}

		private void HandleNitro()
		{
			if (isNitro && !fovBump && !bumpPlayer)
			{
				elapsedNitroTime += Time.deltaTime;
				HandleNitroFov();
				HandleNitroAccelerationEffect();
				if (elapsedNitroTime >= gameManager.GetBoostTime())
				{
					isNitro = false;
				}
			}
		}

		private void HandleNitroAccelerationEffect()
		{
			if (!bumpPlayer)
			{
				float num = nitroEffectCurve.Evaluate(elapsedNitroTime / gameManager.GetBoostTime());
				Transform transform = carRenderer;
				Vector3 localPosition = carRenderer.localPosition;
				float x = localPosition.x;
				Vector3 localPosition2 = carRenderer.localPosition;
				transform.localPosition = new Vector3(x, localPosition2.y, num * nitroAccelerationBumpAmount);
			}
		}

		private void HandleRandomCameraRotation()
		{
			if (isRotating)
			{
				elapsedRotationTime += Time.deltaTime;
				mainCamera.transform.localRotation = Quaternion.Slerp(lastRotation, targetRotation, elapsedRotationTime / targetRotationTime);
				if (elapsedRotationTime > targetRotationTime)
				{
					isRotating = false;
				}
			}
			else
			{
				isRotating = true;
				elapsedRotationTime = 0f;
				targetRotationTime = UnityEngine.Random.Range(minrotationTime, maxRotationTime);
				targetZrotation = UnityEngine.Random.Range(0f, Mathf.Sign(targetZrotation) * (0f - maxRotation));
				lastRotation = mainCamera.transform.localRotation;
				targetRotation = Quaternion.Euler(0f, 0f, targetZrotation);
			}
		}
	}
}
