// DecompilerFi decompiler from Assembly-CSharp.dll class: PositonPingPong
using UnityEngine;

public class PositonPingPong : MonoBehaviour
{
	public Vector3 offsetFromOrignal;

	public float pingSpeed = 1f;

	private Vector3 orginal;

	private Vector3 top;

	private Vector3 bottom;

	private void Start()
	{
		Init();
	}

	public void Init()
	{
		orginal = base.transform.localPosition;
		top = orginal + offsetFromOrignal;
		bottom = orginal - offsetFromOrignal;
	}

	private void Update()
	{
		base.transform.localPosition = Vector3.Lerp(top, bottom, Mathf.PingPong(Time.time * pingSpeed, 1f));
	}
}
