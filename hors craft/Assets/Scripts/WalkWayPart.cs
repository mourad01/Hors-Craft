// DecompilerFi decompiler from Assembly-CSharp.dll class: WalkWayPart
using UnityEngine;

public class WalkWayPart : MonoBehaviour
{
	public enum PartType
	{
		front,
		endless,
		back
	}

	public PartType partType;

	[Header("Child object settings")]
	public bool visibleInMinigame;

	public Transform childObject;

	public Vector3 positionOffset;

	public virtual void Init()
	{
		if (!(childObject == null))
		{
			childObject.gameObject.SetActive(visibleInMinigame);
			Vector3 localPosition = childObject.transform.localPosition;
			localPosition += positionOffset;
			childObject.transform.localPosition = localPosition;
		}
	}

	public virtual void CleanUp()
	{
		if (!(childObject == null))
		{
			childObject.gameObject.SetActive(value: true);
			Vector3 localPosition = childObject.transform.localPosition;
			localPosition -= positionOffset;
			childObject.transform.localPosition = localPosition;
		}
	}

	public virtual void Show()
	{
	}

	public virtual void Hide()
	{
	}

	public virtual void Move()
	{
	}

	public virtual void Stop()
	{
	}

	public virtual void Pause(float time)
	{
	}

	public virtual void CustomUpdate()
	{
	}
}
