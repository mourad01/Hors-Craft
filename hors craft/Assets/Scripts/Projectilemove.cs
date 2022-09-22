// DecompilerFi decompiler from Assembly-CSharp.dll class: Projectilemove
using UnityEngine;

public class Projectilemove : MonoBehaviour
{
	public float speed = 10f;

	public ParticleSystem[] systems;

	[HideInInspector]
	public float stopEmittingAfterMeters = float.MaxValue;

	private float meters;

	private void Update()
	{
		float num = speed * Time.deltaTime;
		base.transform.Translate(Vector3.forward * num);
		meters += num;
		if (meters >= stopEmittingAfterMeters)
		{
			ParticleSystem[] array = systems;
			foreach (ParticleSystem particleSystem in array)
			{
				var _temp_val_20690 = particleSystem.emission; _temp_val_20690.enabled = false;
			}
			if (GetComponent<TrailRenderer>() != null)
			{
				GetComponent<TrailRenderer>().enabled = false;
			}
		}
	}
}
