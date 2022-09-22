// DecompilerFi decompiler from Assembly-CSharp.dll class: Camera3rdPerson
using System;
using UnityEngine;

public class Camera3rdPerson : MonoBehaviour
{
	public Vector3 cameraPosition = new Vector3(0f, 1.26f, -3.43f);

	public float camSpeed = 5f;

	public float desiredZ = 3.46f;

	public CameraNearbyGroundCheck nearbyCheck;

	private Vector3 moveVector = Vector3.zero;

	private GameObject cameraFollower;

	private bool colliding;

	private bool _setFirst;

	public bool setFirst
	{
		get
		{
			return _setFirst;
		}
		set
		{
			if (value)
			{
				SetFirstPerson();
			}
			else
			{
				SetThirdPerson();
			}
			_setFirst = value;
		}
	}

	private void Start()
	{
		setFirst = false;
	}

	private void LateUpdate()
	{
		if (!setFirst)
		{
			if (!colliding && !nearbyCheck.groundNearby)
			{
				ZoomOut();
			}
			base.transform.localEulerAngles = new Vector3(11.23f, 0f, 0f);
			moveVector = base.transform.localPosition;
			moveVector.y = 1f;
			base.transform.localPosition = moveVector;
		}
	}

	private void SetFirstPerson()
	{
		base.transform.localPosition = Vector3.zero;
		DressupClothesPlacement componentInChildren = PlayerGraphic.GetControlledPlayerInstance().GetComponentInChildren<DressupClothesPlacement>();
		if (componentInChildren != null)
		{
			componentInChildren.HideHat();
			componentInChildren.ToggleHair(newState: false);
			PlayerGraphic.GetControlledPlayerInstance().ToggleHead(newState: false);
		}
		base.transform.Translate(base.transform.localPosition + Vector3.forward / 2f);
	}

	private void SetThirdPerson()
	{
		CreateCamera();
		DressupClothesPlacement componentInChildren = PlayerGraphic.GetControlledPlayerInstance().GetComponentInChildren<DressupClothesPlacement>();
		try
		{
			if (componentInChildren != null)
			{
				componentInChildren.ShowHat();
				componentInChildren.ToggleHair(newState: true);
				PlayerGraphic.GetControlledPlayerInstance().ToggleHead(newState: true);
			}
		}
		catch (Exception arg)
		{
			UnityEngine.Debug.LogError("Error while working on player representation! : " + arg);
		}
	}

	private void ZoomIn()
	{
		Vector3 localPosition = base.transform.localPosition;
		if (localPosition.z < 0f)
		{
			moveVector.x = 0f;
			moveVector.y = 0f;
			moveVector.z = Time.deltaTime * camSpeed;
			base.transform.Translate(moveVector);
		}
	}

	private void ZoomOut()
	{
		Vector3 localPosition = base.transform.localPosition;
		if (localPosition.z > desiredZ)
		{
			moveVector.x = 0f;
			moveVector.y = 0f;
			moveVector.z = (0f - Time.deltaTime) * camSpeed;
			base.transform.Translate(moveVector);
		}
	}

	private void OnTriggerStay(Collider other)
	{
		if (other.gameObject.layer == 30)
		{
			colliding = true;
			ZoomIn();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.gameObject.layer == 30)
		{
			colliding = false;
		}
	}

	private void CreateCamera()
	{
		cameraFollower = GetComponentInParent<Camera>().gameObject;
		cameraFollower.GetComponent<Animator>().enabled = false;
		cameraFollower.transform.localPosition = cameraPosition;
	}
}
