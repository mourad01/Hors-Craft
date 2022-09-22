// DecompilerFi decompiler from Assembly-CSharp.dll class: PlayerController
using com.ootii.Cameras;
using Uniblocks;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
	[HideInInspector]
	public bool isInVehicle;

	public bool showAdPopupOnDeath;

	[HideInInspector]
	public bool isInFlyingVehicle;

	private PlayerMovement playerMovement;

	private void Awake()
	{
		playerMovement = GetComponent<PlayerMovement>();
	}

	private void Update()
	{
		playerMovement.GetUnderwaterStatus();
	}

	public void OnWorldReset()
	{
		if (isInVehicle)
		{
			Transform parent = base.transform.parent;
			base.transform.SetParent(null);
			base.transform.rotation = Quaternion.identity;
			VehicleController componentInChildren = parent.root.GetComponentInChildren<VehicleController>();
			if (componentInChildren != null)
			{
				componentInChildren.StopUsing();
				componentInChildren.Dispose();
			}
		}
		UnityEngine.Object.Destroy(base.gameObject);
	}

	public void EnablePlayerVehicleScripts(bool enable)
	{
		GetComponent<CharacterController>().enabled = enable;
		GetComponent<CharacterMotor>().enabled = enable;
		GetComponent<PlayerMovement>().enabled = enable;
		Animator component = CameraController.instance.MainCamera.GetComponent<Animator>();
		component.SetBool("bob", value: false);
		GameObject mainBody = PlayerGraphic.GetControlledPlayerInstance().mainBody;
		if (mainBody != null)
		{
			mainBody.gameObject.SetActive(enable);
		}
	}
}
