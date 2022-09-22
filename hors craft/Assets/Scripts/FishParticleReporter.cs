// DecompilerFi decompiler from Assembly-CSharp.dll class: FishParticleReporter
using UnityEngine;

public class FishParticleReporter : MonoBehaviour
{
	private ParticleSystem pSystem;

	private ParticleSystem.Particle[] particles;

	private void Awake()
	{
		Init();
	}

	private void Start()
	{
	}

	private void OnParticleCollision(GameObject other)
	{
		UpdateRotation();
	}

	private void UpdateRotation()
	{
		Init();
		int num = pSystem.GetParticles(particles);
		for (int i = 0; i < num; i++)
		{
			if (particles[i].velocity != Vector3.zero)
			{
				particles[i].rotation3D = Quaternion.LookRotation(particles[i].velocity).eulerAngles;
			}
		}
		pSystem.SetParticles(particles, num);
	}

	private void Init()
	{
		if (pSystem == null)
		{
			pSystem = GetComponent<ParticleSystem>();
			particles = new ParticleSystem.Particle[pSystem.maxParticles];
			pSystem.Emit(5);
			UpdateRotation();
		}
	}
}
