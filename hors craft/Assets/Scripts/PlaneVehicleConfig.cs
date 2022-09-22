// DecompilerFi decompiler from Assembly-CSharp.dll class: PlaneVehicleConfig
using UnityEngine;

[CreateAssetMenu(menuName = "Vehicle/Configs/PlaneVehicleConfig")]
public class PlaneVehicleConfig : ScriptableObject
{
	public float mass;

	public float drag;

	public float angularDrag;

	public float pitchSpeed = 100f;

	public float yawSpeed = 100f;

	public float rollSpeed = 100f;

	public float liftFactor = 0.1f;

	public float accelarate = 20f;

	public float maximalThrust = 200f;

	public float gravityPower = 1f;

	public float dumpValue = 0.995f;

	public float returnToZeroAngle = 0.3f;

	public float maximumVelocity = 200f;

	public bool analogStick = true;

	public bool invertControls;

	public float groundDistanceCheck = 1.5f;

	public float obstacleUpCheck = 1.5f;

	public float obstacleForwardCheck = 1.5f;

	public float goBackPower = 200f;

	public bool ignoreRaycastChecks;

	public float hoverForce;

	public float hoverHeight;

	public float gravityForce;

	public float currentAngleValue;

	public float angleDumpValue = 5f;

	public float epsilon = 0.7f;
}
