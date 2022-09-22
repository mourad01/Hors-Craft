// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.ActivateOtherGameObjectAfter
using UnityEngine;

namespace Common.Behaviours
{
	public class ActivateOtherGameObjectAfter : MonoBehaviour
	{
		public enum DelayMode
		{
			FROM_AWAKE,
			FROM_START
		}

		public GameObject otherGameObject;

		public float delay = 1f;

		public bool realtime = true;

		public DelayMode delayMode;

		private float activationTime = float.MaxValue;

		private void Awake()
		{
			if (otherGameObject != null)
			{
				otherGameObject.SetActive(value: false);
			}
			if (delayMode == DelayMode.FROM_AWAKE)
			{
				Activate();
			}
		}

		private void Start()
		{
			if (delayMode == DelayMode.FROM_START)
			{
				Activate();
			}
		}

		private void Activate()
		{
			if (otherGameObject != null)
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
				if (otherGameObject != null)
				{
					otherGameObject.SetActive(value: true);
				}
				base.enabled = false;
			}
		}
	}
}
