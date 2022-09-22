// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Behaviours.LensFlareBrightnessPerCameraAngle
using UnityEngine;

namespace Common.Behaviours
{
	[RequireComponent(typeof(LensFlare))]
	public class LensFlareBrightnessPerCameraAngle : MonoBehaviour
	{
		public float brightnessMin;

		public float brightnessMax = 3f;

		[Range(-0.99f, 0.99f)]
		public float minDot = 0.1f;

		[Range(-0.99f, 0.99f)]
		public float maxDot = 0.75f;

		private LensFlare lensFlare;

		private void Awake()
		{
			lensFlare = GetComponent<LensFlare>();
			lensFlare.brightness = (brightnessMin + brightnessMax) / 2f;
		}
	}
}
