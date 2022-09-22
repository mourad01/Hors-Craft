// DecompilerFi decompiler from Assembly-CSharp.dll class: CameraControllerSpawner
using com.ootii.Cameras;
using Uniblocks;
using UnityEngine;

[RequireComponent(typeof(PlayerMovement))]
public class CameraControllerSpawner : MonoBehaviour
{
	[SerializeField]
	private CameraController.CameraPresets _defaultCameraPreset;

	[SerializeField]
	private CameraController _cameraControllerPrefab;

	private void Awake()
	{
		if (!(CameraController.instance != null))
		{
			CameraController cameraController = UnityEngine.Object.Instantiate(_cameraControllerPrefab);
			cameraController.Anchor = base.transform;
			foreach (CameraMotor motor in cameraController.Motors)
			{
				motor.Anchor = base.transform;
			}
			cameraController.defaultCameraPreset = _defaultCameraPreset;
			cameraController.SetDefaultCameraPreset();
		}
	}

	private void OnDestroy()
	{
		if ((bool)CameraController.instance && (bool)CameraController.instance.gameObject)
		{
			UnityEngine.Object.Destroy(CameraController.instance.gameObject);
		}
	}
}
