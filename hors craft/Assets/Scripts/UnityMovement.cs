// DecompilerFi decompiler from Assembly-CSharp.dll class: UnityMovement
using UnityEngine;

public static class UnityMovement
{
	public static void MoveTowards_NoPhysics(this GameObject go, GameObject destGO, float speed)
	{
		go.transform.MoveTowards_NoPhysics(destGO.transform.position, speed);
	}

	public static void MoveTowards_NoPhysics(this Transform goTrans, Transform destTrans, float speed)
	{
		goTrans.MoveTowards_NoPhysics(destTrans.position, speed);
	}

	public static void MoveTowards_NoPhysics(this GameObject go, Vector3 destV, float speed)
	{
		go.transform.MoveTowards_NoPhysics(destV, speed);
	}

	public static void MoveTowards_NoPhysics(this Transform goTrans, Vector3 destV, float speed)
	{
		goTrans.position = Vector3.MoveTowards(goTrans.position, destV, speed);
	}

	public static void MoveTowardsInterpolate_NoPhysics(this GameObject go, GameObject destGo, float lerpPct)
	{
		go.transform.MoveTowardsInterpolate_NoPhysics(destGo.transform.position, lerpPct);
	}

	public static void MoveTowardsInterpolate_NoPhysics(this GameObject go, Vector3 destV, float lerpPct)
	{
		go.transform.MoveTowardsInterpolate_NoPhysics(destV, lerpPct);
	}

	public static void MoveTowardsInterpolate_NoPhysics(this Transform go, Transform destTrans, float lerpPct)
	{
		go.MoveTowardsInterpolate_NoPhysics(destTrans.position, lerpPct);
	}

	public static void MoveTowardsInterpolate_NoPhysics(this Transform goTrans, Vector3 destV, float lerpPct)
	{
		goTrans.position = Vector3.Lerp(goTrans.transform.position, destV, lerpPct);
	}

	public static void TeleportForward_NoPhysics(this GameObject go, float speed)
	{
		go.transform.TeleportForward_NoPhysics(speed);
	}

	public static void TeleportForward_NoPhysics(this Transform go, float speed)
	{
		go.Translate(Vector3.forward * Time.deltaTime * speed);
	}

	public static void MoveByForcePushing_WithPhysics(this Rigidbody go, Vector3 moveDirection, float force)
	{
		go.AddForce(moveDirection * force);
	}

	public static void MoveByVelocity_WithPhysics(this Rigidbody go, Vector3 movementDirection, float speed)
	{
		go.velocity = movementDirection * speed;
	}

	public static void MoveTowards_WithPhysics(this Rigidbody go, Vector3 movementDirection, float speed)
	{
		go.MovePosition(go.position + movementDirection * speed * Time.deltaTime);
	}
}
