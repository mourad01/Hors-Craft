// DecompilerFi decompiler from Assembly-CSharp.dll class: HoverVehicleConfig
using UnityEngine;

[CreateAssetMenu(menuName = "Vehicle/Configs/HoverVehicleConfig")]
public class HoverVehicleConfig : ScriptableObject
{
	public float mass;

	public float drag;

	public float angularDrag;

	public AnimationCurve dumpValueByVelocityAngle;

	public float groundedDrag = 3f;

	public float maxVelocity = 50f;

	public float hoverForce = 1000f;

	public float hoverHeight = 0.9f;

	public float gravityForce = 1000f;

	public float forwardAcceleration = 8000f;

	public float reverseAcceleration = 4000f;

	public float turringMinimum = 1.5f;

	public float turnStrength = 1000f;

	public float steeringWheelMaxAngle = 80f;

	public float frontWheelsMaxAngle = 30f;

	public Vector3 centerOfMass = Vector3.down;
}
