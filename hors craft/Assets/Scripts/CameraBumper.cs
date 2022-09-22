// DecompilerFi decompiler from Assembly-CSharp.dll class: CameraBumper
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
internal class CameraBumper
{
	public enum CollisionType
	{
		None,
		Raycast,
		Collider
	}

	private RaycastHit hit;

	private bool isColliderHit;

	private GameObject ourBumper;

	private DetectionTrigger ourDetectionTrigger;

	[SerializeField]
	private CollisionType collisionType = CollisionType.Raycast;

	[SerializeField]
	private float distanceCheck = 2.5f;

	[SerializeField]
	private float newCameraHeight = 1f;

	[SerializeField]
	private Vector3 offset = Vector3.zero;

	private List<Transform> ourIgnores = new List<Transform>();

	private List<Type> ourIgnoreTypes = new List<Type>();

	private GameObject Bumper
	{
		get
		{
			if (ourBumper == null)
			{
				ourBumper = new GameObject("Bumper");
			}
			return ourBumper;
		}
		set
		{
			ourBumper = value;
		}
	}

	private DetectionTrigger DetectionTrigger
	{
		get
		{
			if (ourDetectionTrigger == null)
			{
				ourDetectionTrigger = (Bumper.AddComponent(typeof(DetectionTrigger)) as DetectionTrigger);
			}
			return ourDetectionTrigger;
		}
	}

	public CollisionType Collision
	{
		get
		{
			return collisionType;
		}
		set
		{
			collisionType = value;
		}
	}

	public float DistanceCheck
	{
		get
		{
			return distanceCheck;
		}
		set
		{
			distanceCheck = value;
		}
	}

	public float NewCameraHeight
	{
		get
		{
			return newCameraHeight;
		}
		set
		{
			newCameraHeight = value;
		}
	}

	public Vector3 Offset
	{
		get
		{
			return offset;
		}
		set
		{
			offset = value;
		}
	}

	public List<Transform> Ignores
	{
		get
		{
			return ourIgnores;
		}
		set
		{
			ourIgnores = value;
		}
	}

	public List<Type> IgnoreTypes
	{
		get
		{
			return ourIgnoreTypes;
		}
		set
		{
			ourIgnoreTypes = value;
		}
	}

	private bool IsBumperHit(Transform argTarget, Transform argCamera)
	{
		switch (collisionType)
		{
		case CollisionType.Collider:
			Bumper.transform.position = argTarget.position + offset + -1f * argTarget.forward;
			DetectionTrigger.Ignores = Ignores;
			DetectionTrigger.IgnoreTypes = IgnoreTypes;
			return DetectionTrigger.IsTripped;
		case CollisionType.Raycast:
		{
			Vector3 direction = argTarget.transform.TransformDirection(-1f * Vector3.forward);
			return Physics.Raycast(argTarget.TransformPoint(offset), direction, out hit, distanceCheck) && hit.transform != argTarget;
		}
		default:
			return false;
		}
	}

	public Vector3 UpdatePosition(Transform argTarget, Transform argCamera, Vector3 argWantedPosition, float argT)
	{
		if (IsBumperHit(argTarget, argCamera))
		{
			Vector3 point = hit.point;
			argWantedPosition.x = point.x;
			Vector3 point2 = hit.point;
			argWantedPosition.z = point2.z;
			Vector3 point3 = hit.point;
			argWantedPosition.y = Mathf.Lerp(point3.y + newCameraHeight, argWantedPosition.y, argT);
		}
		return argWantedPosition;
	}
}
