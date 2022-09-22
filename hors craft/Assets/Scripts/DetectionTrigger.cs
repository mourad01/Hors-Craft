// DecompilerFi decompiler from Assembly-CSharp.dll class: DetectionTrigger
using System;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Triggers/DetectionTrigger")]
public class DetectionTrigger : MonoBehaviour
{
	public enum ColliderEnumType
	{
		Box,
		Capsule,
		Sphere,
		Wheel,
		Mesh
	}

	private GameObject ourGameObject;

	protected Collider ourCollider;

	[SerializeField]
	private ColliderEnumType colliderType = ColliderEnumType.Sphere;

	private Dictionary<int, Transform> ourColliders = new Dictionary<int, Transform>();

	private List<Transform> ourIgnores = new List<Transform>();

	private List<Type> ourIgnoreTypes = new List<Type>();

	private GameObject GameObject
	{
		get
		{
			if (ourGameObject == null)
			{
				ourGameObject = base.gameObject;
			}
			return ourGameObject;
		}
	}

	protected Collider Collider
	{
		get
		{
			if (ourCollider == null)
			{
				ourCollider = GetCollider();
				ourCollider.isTrigger = true;
			}
			return ourCollider;
		}
	}

	public ColliderEnumType ColliderType
	{
		get
		{
			return colliderType;
		}
		set
		{
			colliderType = value;
		}
	}

	public Dictionary<int, Transform> Colliders
	{
		get
		{
			return ourColliders;
		}
		set
		{
			ourColliders = value;
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

	public bool IsTripped
	{
		get
		{
			if (ourColliders.Count == 0)
			{
				return false;
			}
			bool result = false;
			foreach (Transform value in ourColliders.Values)
			{
				if (!Ignores.Contains(value))
				{
					result = true;
				}
			}
			return result;
		}
	}

	public void Awake()
	{
		ourCollider = GetCollider();
	}

	private void OnTriggerEnter(Collider argCollider)
	{
		UnityEngine.Debug.Log(argCollider.transform.GetInstanceID() + " " + argCollider.name);
		ourColliders.Add(argCollider.transform.GetInstanceID(), argCollider.transform);
	}

	private void OnTriggerExit(Collider argCollider)
	{
		ourColliders.Remove(argCollider.transform.GetInstanceID());
	}

	private void OnColliderEnter(Collision argCollider)
	{
		UnityEngine.Debug.Log(argCollider.transform.GetInstanceID() + " " + argCollider.transform.name);
		ourColliders.Add(argCollider.transform.GetInstanceID(), argCollider.transform);
	}

	private void OnColliderExit(Collision argCollider)
	{
		ourColliders.Remove(argCollider.transform.GetInstanceID());
	}

	private Collider GetCollider()
	{
		Collider collider = null;
		switch (colliderType)
		{
		case ColliderEnumType.Box:
			collider = (GetComponent(typeof(BoxCollider)) as BoxCollider);
			if (collider == null)
			{
				collider = (GameObject.AddComponent(typeof(BoxCollider)) as BoxCollider);
			}
			break;
		case ColliderEnumType.Capsule:
			collider = (GetComponent(typeof(CapsuleCollider)) as CapsuleCollider);
			if (collider == null)
			{
				collider = (GameObject.AddComponent(typeof(CapsuleCollider)) as CapsuleCollider);
			}
			break;
		case ColliderEnumType.Sphere:
			collider = (GetComponent(typeof(SphereCollider)) as SphereCollider);
			if (collider == null)
			{
				collider = (GameObject.AddComponent(typeof(SphereCollider)) as SphereCollider);
			}
			break;
		case ColliderEnumType.Wheel:
			collider = (GetComponent(typeof(WheelCollider)) as WheelCollider);
			if (collider == null)
			{
				collider = (GameObject.AddComponent(typeof(WheelCollider)) as WheelCollider);
			}
			break;
		case ColliderEnumType.Mesh:
			collider = (GetComponent(typeof(MeshCollider)) as MeshCollider);
			if (collider == null)
			{
				collider = (GameObject.AddComponent(typeof(MeshCollider)) as MeshCollider);
			}
			break;
		}
		if (collider == null)
		{
			throw new Exception("Trigger Item Has No Collider");
		}
		return collider;
	}
}
