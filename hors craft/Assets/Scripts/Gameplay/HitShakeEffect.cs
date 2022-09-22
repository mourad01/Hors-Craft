// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.HitShakeEffect
using UnityEngine;

namespace Gameplay
{
	public class HitShakeEffect : MonoBehaviour
	{
		[Range(0f, 3f)]
		public float shakeDuration = 0.5f;

		private Quaternion initialRotation;

		private float startTime;

		private void Awake()
		{
			startTime = -1f;
		}

		public void StartShake()
		{
			startTime = Time.realtimeSinceStartup;
			initialRotation = base.transform.localRotation;
			ref Quaternion reference = ref initialRotation;
			Vector3 eulerAngles = initialRotation.eulerAngles;
			reference.eulerAngles = new Vector3(Clamp(eulerAngles.x), 0f, 0f);
		}

		private float Clamp(float x)
		{
			if (x >= 90f && x <= 180f)
			{
				return 89f;
			}
			if (x >= 180f && x <= 270f)
			{
				return 269f;
			}
			return x;
		}

		private void Update()
		{
			if (startTime > Time.realtimeSinceStartup - shakeDuration)
			{
				base.transform.localRotation = GetCurrentRotation();
			}
		}

		private Quaternion GetCurrentRotation()
		{
			float num = CalculateCurrentFactor();
			if (num == 0f)
			{
				return initialRotation;
			}
			if (num == 1f)
			{
				return initialRotation;
			}
			float num2 = 1f - num;
			return Quaternion.Slerp(initialRotation, UnityEngine.Random.rotation, num2 * 0.07f);
		}

		private float CalculateCurrentFactor()
		{
			if (shakeDuration == 0f)
			{
				return 1f;
			}
			return Mathf.Clamp((Time.realtimeSinceStartup - startTime) / shakeDuration, 0f, 1f);
		}
	}
}
