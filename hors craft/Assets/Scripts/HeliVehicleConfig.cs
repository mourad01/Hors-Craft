// DecompilerFi decompiler from Assembly-CSharp.dll class: HeliVehicleConfig
using UnityEngine;

[CreateAssetMenu(menuName = "Vehicle/Configs/HeliVehicleConfig")]
public class HeliVehicleConfig : ScriptableObject
{
	public float mass;

	public float drag;

	public float angularDrag;

	public float maxPitchAngle = 30f;

	public float maxRollAngle = 45f;

	public float accelarate = 200f;

	public float gravityPower = 1f;

	public float dumpValue = 0.995f;

	public float forwardSpeed = 1000f;

	public float turnSpeed = 500f;

	public float modelRotationSpeed = 10f;

	public float fovChangeRate = 2f;
}
