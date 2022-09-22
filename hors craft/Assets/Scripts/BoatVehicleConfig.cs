// DecompilerFi decompiler from Assembly-CSharp.dll class: BoatVehicleConfig
using UnityEngine;

[CreateAssetMenu(menuName = "Vehicle/Configs/BoatVehicleConfig")]
public class BoatVehicleConfig : ScriptableObject
{
	public float mass;

	public float drag;

	public float angularDrag;

	public float groundedDrag = 3f;

	public float maxVelocity = 50f;

	public float gravityForce = 1000f;

	public float forwardAcceleration = 8000f;

	public float reverseAcceleration = 4000f;

	public float turnStrength = 1000f;

	public float dumpValue = 0.99f;
}
