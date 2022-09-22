// DecompilerFi decompiler from Assembly-CSharp.dll class: RollercoasterEffects
using UnityEngine;
using UnityEngine.UI;

public class RollercoasterEffects : MonoBehaviour
{
	public Image vignette;

	public ParticleSystem particles;

	public Camera cam;

	public float minFov = 50f;

	public float maxFov = 70f;

	public float lifetimeFrom = 2f;

	public float lifetimeTo = 0.6f;

	public float speedFrom = 6f;

	public float speedTo = 18f;

	public float emissionFrom = 20f;

	public float emissionTo = 50f;

	public void UpdateEffects(float factor)
	{
		vignette.color = new Color(1f, 1f, 1f, factor);
		cam.fieldOfView = Mathf.Lerp(minFov, maxFov, factor);
		ParticleSystem.EmissionModule emission = particles.emission;
		if (factor < 0.1f)
		{
			emission.rateOverTime = 0f;
			return;
		}
		ParticleSystem.MainModule main = particles.main;
		emission.rateOverTime = Mathf.Lerp(emissionFrom, emissionTo, factor);
		main.startSpeed = Mathf.Lerp(speedFrom, speedTo, factor);
		main.startLifetime = Mathf.Lerp(lifetimeFrom, lifetimeTo, factor);
	}
}
