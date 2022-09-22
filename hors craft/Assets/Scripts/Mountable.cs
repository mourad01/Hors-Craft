// DecompilerFi decompiler from Assembly-CSharp.dll class: Mountable
using com.ootii.Cameras;
using Common.Managers;
using System;
using UnityEngine;

public class Mountable : MonoBehaviour
{
	[Range(0.1f, 5f)]
	public float speedMultiplier = 1.5f;

	[Header("Where will camera be placed when mounted")]
	public Vector3 relativeMountPos = Vector3.up * 2f;

	[Header("Where's the bottom of the animal")]
	public Vector3 relativeBottomPos = Vector3.down;

	public bool isFlying;

	public bool isVehicle;

	public float jumpHightMult = 1f;

	[HideInInspector]
	private AnimalMob mob;

	public virtual bool simulateMovement
	{
		set
		{
			mob.animator.SetBool("walking", value);
		}
	}

	public static event Action<bool> onMounted;

	public virtual void MountVehicle(Transform playerTransform)
	{
	}

	public virtual void MountMob(Transform playerTransform)
	{
		mob.body.velocity = Vector3.zero;
		mob.body.angularVelocity = Vector3.zero;
		mob.mountMode = true;
		mob.ResetLook();
		MountAndPosition(playerTransform);
		Manager.Get<QuestManager>().HandleMobIndicator(mob.gameObject);
	}

	protected void TryToEnableIndicator(bool enable)
	{
		GameObject gameObject = TryToFinIndicator();
		if (gameObject != null)
		{
			gameObject.SetActive(enable);
		}
	}

	protected GameObject TryToFinIndicator()
	{
		PositonPingPong componentInChildren = GetComponentInChildren<PositonPingPong>(includeInactive: true);
		if (componentInChildren != null)
		{
			return componentInChildren.gameObject;
		}
		return null;
	}

	public virtual void MountAndPosition(Transform playerTransform)
	{
		base.transform.SetParent(playerTransform, worldPositionStays: false);
		Vector3 a = (relativeBottomPos + relativeMountPos) / 2f;
		a.Scale(base.transform.localScale);
		base.transform.localPosition = -a;
		base.transform.localRotation = Quaternion.identity;
		if (Mountable.onMounted != null)
		{
			Mountable.onMounted(obj: true);
		}
	}

	public virtual void Move(Transform transform, Vector3 moveDir, float vertical)
	{
	}

	public virtual Vector3 VehicleMoveDirection(Vector3 moveDir, float vertical)
	{
		return Vector3.zero;
	}

	public float GetMountHeight()
	{
		Vector3 vector = relativeMountPos - relativeBottomPos;
		float y = vector.y;
		Vector3 localScale = base.transform.localScale;
		return y * localScale.y;
	}

	public virtual void Unmount()
	{
		base.transform.SetParent(null, worldPositionStays: true);
		mob.mountMode = false;
		simulateMovement = false;
		Manager.Get<QuestManager>().HandleMobIndicator(mob.gameObject);
		if (Mountable.onMounted != null)
		{
			Mountable.onMounted(obj: false);
		}
	}

	public float GetBottomToCenterOffset()
	{
		float num = CalculateMinBoundsY();
		Vector3 position = base.transform.position;
		float y = position.y;
		return y - num;
	}

	public virtual void SetCameraPosition(Transform cameraPos)
	{
	}

	public virtual TrainMountable GetClosestTrain()
	{
		return new TrainMountable();
	}

	private void Awake()
	{
		mob = GetComponent<AnimalMob>();
	}

	private float CalculateMinBoundsY()
	{
		Renderer[] componentsInChildren = GetComponentsInChildren<Renderer>();
		if (componentsInChildren == null || componentsInChildren.Length == 0)
		{
			return 1f;
		}
		float num = float.MaxValue;
		Renderer[] array = componentsInChildren;
		foreach (Renderer renderer in array)
		{
			float a = num;
			Vector3 min = renderer.bounds.min;
			num = Mathf.Min(a, min.y);
		}
		return num;
	}

	protected void SetParent(Transform parent)
	{
		CameraController.instance.Anchor.transform.SetParent(parent);
		CameraController.instance.Anchor.transform.localPosition = Vector3.zero;
		CameraController.instance.Anchor.transform.localRotation = Quaternion.identity;
		CameraController.instance.Anchor.transform.localScale = Vector3.one;
	}
}
