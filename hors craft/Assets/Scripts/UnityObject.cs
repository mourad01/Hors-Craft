// DecompilerFi decompiler from Assembly-CSharp.dll class: UnityObject
using System;
using System.Collections;
using UnityEngine;

public abstract class UnityObject : MonoBehaviour
{
	private GameObject ourGameObject;

	private Transform ourTransform;

	private Rigidbody ourRigidbody;

	private Collider ourCollider;

	private Animation ourAnimation;

	private Renderer ourRenderer;

	private bool isMoving;

	private Transform ourRightHand;

	private Transform ourLeftHand;

	public GameObject GameObject
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

	public Transform Transform
	{
		get
		{
			if (ourTransform == null)
			{
				ourTransform = base.transform;
			}
			return ourTransform;
		}
	}

	public Vector3 Position
	{
		get
		{
			return Transform.position;
		}
		set
		{
			Transform.position = value;
		}
	}

	public bool IsMoving
	{
		get
		{
			return isMoving;
		}
		set
		{
			isMoving = value;
		}
	}

	public Rigidbody Rigidbody
	{
		get
		{
			if (ourRigidbody == null)
			{
				ourRigidbody = GetComponent<Rigidbody>();
			}
			return ourRigidbody;
		}
	}

	public Collider Collider
	{
		get
		{
			if (ourCollider == null)
			{
				ourCollider = GetComponent<Collider>();
			}
			return ourCollider;
		}
	}

	public Animation Animation
	{
		get
		{
			if (ourAnimation == null)
			{
				ourAnimation = GetComponent<Animation>();
			}
			return ourAnimation;
		}
	}

	public Renderer Renderer
	{
		get
		{
			if (ourRenderer == null)
			{
				Transform transform = Transform.Find("JNT_Root");
				if (transform != null)
				{
					ourRenderer = transform.GetComponent<Renderer>();
				}
			}
			return ourRenderer;
		}
	}

	public Vector3 CenterPoint
	{
		get
		{
			if (Renderer != null)
			{
				return Renderer.bounds.center;
			}
			return Position;
		}
	}

	public Transform RightHand
	{
		get
		{
			if (ourRightHand == null)
			{
				ourRightHand = FindChild(Transform, "JNT_R_Hand");
			}
			if (ourRightHand == null)
			{
				return Transform;
			}
			return ourRightHand;
		}
	}

	public Transform LeftHand
	{
		get
		{
			if (ourLeftHand == null)
			{
				ourLeftHand = FindChild(Transform, "JNT_L_Hand");
			}
			if (ourLeftHand == null)
			{
				return Transform;
			}
			return ourLeftHand;
		}
	}

	private Transform FindChild(Transform currentTransform, string argName)
	{
		if (currentTransform.name == argName)
		{
			return currentTransform;
		}
		if (currentTransform.childCount != 0)
		{
			IEnumerator enumerator = currentTransform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform currentTransform2 = (Transform)enumerator.Current;
					Transform transform = FindChild(currentTransform2, argName);
					if (transform != null)
					{
						return transform;
					}
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
		}
		return null;
	}
}
