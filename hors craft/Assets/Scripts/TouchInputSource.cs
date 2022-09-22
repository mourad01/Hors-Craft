// DecompilerFi decompiler from Assembly-CSharp.dll class: TouchInputSource
using com.ootii.Input;
using Gameplay;
using GameUI;
using Uniblocks;
using UnityEngine;

public class TouchInputSource : UnityInputSource
{
	private SimpleRepeatButton _cameraButton;

	public float touchSensitivity = 2f;

	private Vector2 oldPosition = Vector2.zero;

	private readonly Vector2 _zeroVector = Vector2.zero;

	private CurrentInputInfo _inputInfo = new CurrentInputInfo();

	public const float SENSITIVITY_MULTIPLIER = 100f;

	public override float ViewX => _IsEnabled ? _inputInfo.position.x : 0f;

	public override float ViewY => _IsEnabled ? _inputInfo.position.y : 0f;

	private void Awake()
	{
		MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.MOVEMENT, new CameraRotationContext
		{
			setCameraRotationButton = delegate(SimpleRepeatButton cr)
			{
				_cameraButton = cr;
			}
		});
	}

	public void Update()
	{
		UpdateCameraButtonInput();
	}

	private void UpdateCameraButtonInput()
	{
		_inputInfo = ConstructInputInfo();
		if (_inputInfo.phase != TouchPhase.Moved)
		{
			_inputInfo.position = _zeroVector;
			oldPosition = _zeroVector;
		}
	}

	private Touch? FindCurrentTouch()
	{
		for (int i = 0; i < UnityEngine.Input.touchCount; i++)
		{
			Touch touch = UnityEngine.Input.GetTouch(i);
			if (touch.fingerId == _cameraButton.fingerId)
			{
				return touch;
			}
		}
		return null;
	}

	private CurrentInputInfo ConstructInputInfo()
	{
		if (_cameraButton != null && _cameraButton.pressed)
		{
			CurrentInputInfo currentInputInfo = new CurrentInputInfo();
			if (UnityEngine.Input.touchCount > 0)
			{
				Touch? touch = FindCurrentTouch();
				if (touch.HasValue)
				{
					if (oldPosition == _zeroVector)
					{
						oldPosition = touch.Value.position;
					}
					currentInputInfo.phase = touch.Value.phase;
					currentInputInfo.position = touch.Value.position - oldPosition;
					oldPosition = touch.Value.position;
				}
			}
			else
			{
				currentInputInfo.position = new Vector2(UnityEngine.Input.GetAxis("Mouse X"), UnityEngine.Input.GetAxis("Mouse Y")) * 10f;
				if (Engine.EditMode)
				{
					if (Input.GetMouseButtonDown(2))
					{
						currentInputInfo.phase = TouchPhase.Began;
					}
					else if (Input.GetMouseButton(2))
					{
						currentInputInfo.phase = TouchPhase.Moved;
					}
				}
				else if (Input.GetMouseButtonDown(0))
				{
					currentInputInfo.phase = TouchPhase.Began;
				}
				else if (Input.GetMouseButton(0))
				{
					currentInputInfo.phase = TouchPhase.Moved;
				}
			}
			Vector3 a = new Vector3(currentInputInfo.position.x / (float)Screen.width, currentInputInfo.position.y / (float)Screen.height, 0f);
			currentInputInfo.position = a * touchSensitivity * 100f;
			return currentInputInfo;
		}
		CurrentInputInfo currentInputInfo2 = new CurrentInputInfo();
		currentInputInfo2.phase = TouchPhase.Canceled;
		currentInputInfo2.position = Vector3.zero;
		return currentInputInfo2;
	}
}
