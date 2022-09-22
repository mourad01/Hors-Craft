// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.DestroyAfter
using UnityEngine;

namespace Common.Behaviours
{
	public class DestroyAfter : MonoBehaviour
	{
		public float delay = 1f;

		public bool afterAttachedParticlesDuration;

		public bool realtime = true;

		private float activationTime = float.MaxValue;

		private void Awake()
		{
			if (afterAttachedParticlesDuration)
			{
				ParticleSystem componentInChildren = GetComponentInChildren<ParticleSystem>();
				if (componentInChildren != null)
				{
					delay = VersionDependend.GetParticleSystemDuration(componentInChildren);
				}
			}
			CalculateDestroyTime();
		}

		public void CalculateDestroyTime()
		{
			if (realtime)
			{
				activationTime = Time.realtimeSinceStartup + delay;
			}
			else
			{
				activationTime = Time.time + delay;
			}
		}

		private void Update()
		{
			if (!realtime)
			{
				Check(Time.time);
			}
			else
			{
				Check(Time.realtimeSinceStartup);
			}
		}

		private void Check(float time)
		{
			if (time > activationTime)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}
	}
}
