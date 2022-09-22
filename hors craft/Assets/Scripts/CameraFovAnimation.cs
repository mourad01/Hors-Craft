// DecompilerFi decompiler from Assembly-CSharp.dll class: CameraFovAnimation
using com.ootii.Cameras;
using Common.Utils;
using UnityEngine;

public class CameraFovAnimation : MonoBehaviour
{
	[Range(0f, 3f)]
	public float duration = 0.25f;

	public float strength = 5f;

	private float time;

	private float fov = 70f;

	private bool animating;

	public void StartAnimation(float recoil, float duration)
	{
		this.duration = duration;
		strength = recoil;
		time = 0f;
		if (!animating)
		{
			fov = CameraController.instance.MainCamera.fieldOfView;
		}
		animating = true;
		CameraController.instance.MainCamera.fieldOfView = fov + strength;
	}

	private void Update()
	{
		if (animating)
		{
			float value = time / duration;
			CameraController.instance.MainCamera.fieldOfView = Easing.Ease(EaseType.OutSine, fov + strength, fov, value);
			time = Mathf.Min(time + Time.deltaTime, duration);
			if (time >= duration)
			{
				animating = false;
				CameraController.instance.MainCamera.fieldOfView = fov;
			}
		}
	}
}
