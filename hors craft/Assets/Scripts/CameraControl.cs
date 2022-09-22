// DecompilerFi decompiler from Assembly-CSharp.dll class: CameraControl
using System;
using UnityEngine;

[Serializable]
public class CameraControl
{
	[SerializeField]
	private TargetEnum target = TargetEnum.Distance;

	[SerializeField]
	private float stepSize = 1f;

	[SerializeField]
	private MouseCodeEnum mouseCode;

	[SerializeField]
	private KeyCode keyCode;

	public TargetEnum Target
	{
		get
		{
			return target;
		}
		set
		{
			target = value;
		}
	}

	public float StepSize
	{
		get
		{
			return stepSize;
		}
		set
		{
			stepSize = value;
		}
	}

	public MouseCodeEnum MouseCode
	{
		get
		{
			return mouseCode;
		}
		set
		{
			mouseCode = value;
		}
	}

	public KeyCode KeyCode
	{
		get
		{
			return keyCode;
		}
		set
		{
			keyCode = value;
		}
	}

	public bool IsPressed => UnityEngine.Input.GetKey(keyCode);

	public float Value
	{
		get
		{
			float num = 0f;
			switch (mouseCode)
			{
			case MouseCodeEnum.ScrollWheel:
				num = (0f - UnityEngine.Input.GetAxis("Mouse ScrollWheel")) * StepSize * 100f * Time.deltaTime;
				break;
			case MouseCodeEnum.X:
				num = UnityEngine.Input.GetAxis("Mouse X") * StepSize * 100f * Time.deltaTime;
				break;
			case MouseCodeEnum.Y:
				num = UnityEngine.Input.GetAxis("Mouse Y") * StepSize * 100f * Time.deltaTime;
				break;
			}
			if (num == 0f && IsPressed)
			{
				num = StepSize * Time.deltaTime;
			}
			return num;
		}
	}

	public CameraControl()
	{
		stepSize = 1f;
	}

	public CameraControl(TargetEnum argTarget, KeyCode argKeyCode, float argStepSize)
	{
		target = argTarget;
		keyCode = argKeyCode;
		stepSize = argStepSize;
	}

	public CameraControl(TargetEnum argTarget, MouseCodeEnum argMouseCode, float argStepSize)
	{
		target = argTarget;
		mouseCode = argMouseCode;
		stepSize = argStepSize;
	}

	public CameraControl(TargetEnum argTarget, KeyCode argKeyCode, MouseCodeEnum argMouseCode, float argStepSize)
	{
		target = argTarget;
		keyCode = argKeyCode;
		mouseCode = argMouseCode;
		stepSize = argStepSize;
	}
}
