// DecompilerFi decompiler from Assembly-CSharp.dll class: WaitForWorldLoad
using System.Collections;
using UnityEngine;

public class WaitForWorldLoad : MonoBehaviour
{
	public float distToGround = 3f;

	public int times;

	private void OnDisable()
	{
		StopAllCoroutines();
	}

	private void OnEnable()
	{
		StartCoroutine(CheckIfLoadedUpdate());
	}

	private IEnumerator CheckIfLoadedUpdate()
	{
		yield return new WaitForSecondsRealtime(1f);
		if (IsGrounded())
		{
			GetComponent<CharacterController>().enabled = true;
			StopAllCoroutines();
		}
		else
		{
			times++;
			StartCoroutine(CheckIfLoadedUpdate());
		}
	}

	private bool IsGrounded()
	{
		UnityEngine.Debug.DrawRay(base.transform.position, -Vector3.up * distToGround, Color.red, 0.5f);
		return Physics.Raycast(base.transform.position, -Vector3.up, distToGround + 0.1f);
	}
}
