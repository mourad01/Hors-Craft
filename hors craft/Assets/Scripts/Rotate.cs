// DecompilerFi decompiler from Assembly-CSharp.dll class: Rotate
using UnityEngine;

public class Rotate : MonoBehaviour
{
	public float speed = 180f;

	public Space space = Space.Self;

	private void Update()
	{
		base.transform.Rotate(new Vector3(0f, speed * Time.unscaledDeltaTime, 0f), space);
	}
}
