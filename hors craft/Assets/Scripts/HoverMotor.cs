// DecompilerFi decompiler from Assembly-CSharp.dll class: HoverMotor
using com.ootii.Cameras;
using UnityEngine;

public class HoverMotor : HoverCar
{
	[Header("References gfx")]
	public GameObject leftHand;

	public GameObject rightHand;

	private Vector3 leftHandRotation;

	private Vector3 rightHandRotation;

	private AngleMotor angleMotor;

	protected override void Start()
	{
		base.Start();
		angleMotor = GetComponentInChildren<AngleMotor>();
	}

	protected override void Awake()
	{
		base.Awake();
		leftHandRotation = leftHand.transform.localEulerAngles;
		rightHandRotation = rightHand.transform.localEulerAngles;
		leftHand.SetActive(value: false);
		rightHand.SetActive(value: false);
	}

	protected override void Update()
	{
		base.Update();
		if (analogInput != null)
		{
			Vector2 vector = analogInput.CalculatePosition();
			MoveHands(vector.x);
		}
	}

	protected override void FixedUpdate()
	{
		base.FixedUpdate();
		if (isInUse && (bool)angleMotor)
		{
			AngleMotor obj = angleMotor;
			Vector2 vector = analogInput.CalculatePosition();
			obj.AngleObject(vector.x, base.velocity);
		}
	}

	public override void VehicleActivate(GameObject player)
	{
		base.VehicleActivate(player);
		leftHand.SetActive(value: true);
		rightHand.SetActive(value: true);
		CameraController.instance.ClearTargetForward();
		CameraController.instance.SetTargetForward(insideCameraPosition.transform.forward, float.MaxValue);
	}

	public override void StopUsing()
	{
		base.StopUsing();
		leftHand.SetActive(value: false);
		rightHand.SetActive(value: false);
	}

	private void MoveHands(float value)
	{
		float num = Quaternion.Angle(initialSteeringWheelRotation, steeringWheel.transform.localRotation);
		if (value > 0f)
		{
			num *= -1f;
		}
		leftHand.transform.localEulerAngles = new Vector3(leftHandRotation.x, leftHandRotation.y, leftHandRotation.z);
		leftHand.transform.Rotate(Vector3.up, num, Space.World);
		rightHand.transform.localEulerAngles = new Vector3(rightHandRotation.x, rightHandRotation.y, rightHandRotation.z);
		rightHand.transform.Rotate(Vector3.up, num, Space.World);
	}
}
