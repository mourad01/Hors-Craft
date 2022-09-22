// DecompilerFi decompiler from Assembly-CSharp.dll class: FuckedUpSpecialDragonMovement
using UnityEngine;

public class FuckedUpSpecialDragonMovement : MonoBehaviour
{
	public Vector3 endPosition;

	private void Update()
	{
		Vector3 position = base.transform.position;
		if (position.x > -160f)
		{
			base.transform.Translate(Vector3.forward);
		}
		else
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}
}
