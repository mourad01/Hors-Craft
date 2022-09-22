// DecompilerFi decompiler from Assembly-CSharp.dll class: IsometricPlaceablePrefabOnWater
using UnityEngine;

public class IsometricPlaceablePrefabOnWater : IsometricPlaceableBoat
{
	protected override void PlacePlayer()
	{
		if (!(playerTransform.parent != null))
		{
			Vector3 size = GetBounds().size;
			Transform transform = objInstance.transform;
			playerTransform.position = transform.position + transform.forward * (0f - size.z) + transform.up * size.y;
			RotateTowardsObj();
		}
	}
}
