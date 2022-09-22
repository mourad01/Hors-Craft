// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.RealtimeParticleSystem
using UnityEngine;

namespace Common
{
	[RequireComponent(typeof(ParticleSystem))]
	public class RealtimeParticleSystem : MonoBehaviour
	{
		public bool simulateOnlyIfTimescaleZero = true;

		private ParticleSystem particles;

		private void Awake()
		{
			ParticleSystem[] componentsInChildren = GetComponentsInChildren<ParticleSystem>();
			foreach (ParticleSystem particleSystem in componentsInChildren)
			{
				var main = particleSystem.main;
				main.useUnscaledTime = true;
			}
			base.enabled = false;
		}

		[ContextMenu("Test play")]
		private void Play()
		{
			particles.Play();
		}

		[ContextMenu("Test stop")]
		private void Stop()
		{
			particles.Simulate(0f, withChildren: true, restart: true);
			particles.Stop();
			particles.Clear();
		}

		private void Update()
		{
			if ((!simulateOnlyIfTimescaleZero || Time.timeScale == 0f) && !particles.isStopped)
			{
				particles.Simulate(Time.unscaledDeltaTime, withChildren: true, restart: false);
			}
		}
	}
}
