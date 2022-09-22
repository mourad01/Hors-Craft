// DecompilerFi decompiler from Assembly-CSharp.dll class: TriggerTargetAcquisition
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TriggerTargetAcquisition : MonoBehaviour
{
	[SerializeField]
	private float range;

	private HashSet<GameObject> surroundingObjects = new HashSet<GameObject>();

	private Collider triggerCollider;

	public float setRange
	{
		set
		{
			range = value;
			if (triggerCollider is SphereCollider)
			{
				((SphereCollider)triggerCollider).radius = value;
			}
		}
	}

	public List<GameObject> GetTargets(Func<GameObject, bool> targetRecognisedFunction)
	{
		HashSet<Health> healthUsed = new HashSet<Health>();
		return (from go in surroundingObjects
			let h = (!(go != null)) ? null : go.GetComponentInParent<Health>()
			where h != null && healthUsed.Add(h) && targetRecognisedFunction(go)
			select h.gameObject).ToList();
	}

	private void Awake()
	{
		triggerCollider = (from c in GetComponents<Collider>()
			where c.isTrigger
			select c).FirstOrDefault();
		if (triggerCollider == null)
		{
			triggerCollider = base.gameObject.AddComponent<SphereCollider>();
			triggerCollider.isTrigger = true;
			((SphereCollider)triggerCollider).radius = range;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		if (!other.isTrigger)
		{
			surroundingObjects.Add(other.gameObject);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (!other.isTrigger)
		{
			surroundingObjects.Add(other.gameObject);
		}
	}

	private void OnTriggerExit(Collider other)
	{
		surroundingObjects.Remove(other.gameObject);
	}
}
