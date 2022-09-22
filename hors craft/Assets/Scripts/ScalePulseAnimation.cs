// DecompilerFi decompiler from Assembly-CSharp.dll class: ScalePulseAnimation
using UnityEngine;

public class ScalePulseAnimation : MonoBehaviour
{
	public AnimationCurve animationCurve;

	private float timePassed;

	private void Update()
	{
		timePassed += Time.deltaTime;
		base.transform.localScale = Vector3.one * animationCurve.Evaluate(timePassed);
	}
}
