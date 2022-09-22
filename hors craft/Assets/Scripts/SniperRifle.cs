// DecompilerFi decompiler from Assembly-CSharp.dll class: SniperRifle
using com.ootii.Cameras;
using UnityEngine;

public class SniperRifle : MonoBehaviour
{
	public GameObject scope;

	public Camera sniperCamera;

	public GameObject scopeFrame;

	private void Start()
	{
		Camera mainCamera = CameraController.instance.MainCamera;
		if (mainCamera.GetComponent<Skybox>() != null)
		{
			sniperCamera.GetComponent<Skybox>().material = mainCamera.GetComponent<Skybox>().material;
			sniperCamera.farClipPlane = mainCamera.farClipPlane;
		}
	}
}
