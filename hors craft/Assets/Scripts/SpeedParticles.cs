// DecompilerFi decompiler from Assembly-CSharp.dll class: SpeedParticles
using UnityEngine;

public class SpeedParticles : MonoBehaviour
{
	private HoverCar car;

	private ParticleSystem particles;

	public float lifetimeFrom = 2f;

	public float lifetimeTo = 0.6f;

	public float speedFrom = 6f;

	public float speedTo = 18f;

	public float emissionFrom = 20f;

	public float emissionTo = 50f;

	private void Awake()
	{
		car = GetComponentInParent<HoverCar>();
		particles = GetComponent<ParticleSystem>();
	}

	private void Update()
	{
		if (!(car == null))
		{
			float num = Mathf.Clamp01((car.velocity - 2f) / 10f);
			ParticleSystem.EmissionModule emission = particles.emission;
			if (num == 0f)
			{
				emission.rate = 0f;
				return;
			}
			emission.rate = Mathf.Lerp(emissionFrom, emissionTo, num);
			particles.startSpeed = Mathf.Lerp(speedFrom, speedTo, num);
			particles.startLifetime = Mathf.Lerp(lifetimeFrom, lifetimeTo, num);
		}
	}
}
