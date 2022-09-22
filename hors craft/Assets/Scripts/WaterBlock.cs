// DecompilerFi decompiler from Assembly-CSharp.dll class: WaterBlock
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class WaterBlock : MonoBehaviour
{
	public ScreenOverlay mainCameraOverlay;

	private void FixedUpdate()
	{
		RaycastHit hitInfo = default(RaycastHit);
		if (Physics.Raycast(base.transform.position, Vector3.up, out hitInfo, 1f))
		{
			if (hitInfo.transform.tag == "waterTrigger")
			{
				mainCameraOverlay.intensity = 0.7f;
			}
		}
		else
		{
			mainCameraOverlay.intensity = 0f;
		}
	}
}
