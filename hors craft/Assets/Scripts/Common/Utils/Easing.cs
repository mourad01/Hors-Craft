// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.Easing
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Utils
{
	public static class Easing
	{
		private delegate float SimpleEaseFunction(float from, float to, float value);

		private delegate float AmplitudeEaseFunction(float from, float to, float value, float amplitude, float duration);

		private static Dictionary<EaseType, SimpleEaseFunction> m_simpleEaseFunctions;

		private static Dictionary<EaseType, AmplitudeEaseFunction> m_amplitudeEaseFunctions;

		private static Dictionary<EaseType, SimpleEaseFunction> simpleEaseFunctions
		{
			get
			{
				if (m_simpleEaseFunctions == null)
				{
					m_simpleEaseFunctions = new Dictionary<EaseType, SimpleEaseFunction>();
					DefineSimpleEaseFunctions();
				}
				return m_simpleEaseFunctions;
			}
		}

		private static Dictionary<EaseType, AmplitudeEaseFunction> amplitudeEaseFunctions
		{
			get
			{
				if (m_amplitudeEaseFunctions == null)
				{
					m_amplitudeEaseFunctions = new Dictionary<EaseType, AmplitudeEaseFunction>();
					DefineComplexEaseFunctions();
				}
				return m_amplitudeEaseFunctions;
			}
		}

		public static float Ease(EaseType easeType, float from, float to, float value)
		{
			return Ease(easeType, from, to, value, clamp: true, 1f, 1f);
		}

		public static float Ease(EaseType easeType, float from, float to, float value, bool clamp)
		{
			return Ease(easeType, from, to, value, clamp, 1f, 1f);
		}

		public static float Ease(EaseType easeType, float from, float to, float value, bool clamp, float amplitude, float amplitudeDuration)
		{
			if (clamp)
			{
				value = Mathf.Clamp(value, 0f, 1f);
			}
			if (value == 0f)
			{
				return from;
			}
			if (value == 1f)
			{
				return to;
			}
			to -= from;
			SimpleEaseFunction value2 = null;
			if (simpleEaseFunctions.TryGetValue(easeType, out value2))
			{
				return value2(from, to, value);
			}
			AmplitudeEaseFunction value3 = null;
			if (amplitudeEaseFunctions.TryGetValue(easeType, out value3))
			{
				return value3(from, to, value, amplitude, amplitudeDuration);
			}
			return to;
		}

		public static Vector2 EaseVector2(EaseType easeType, Vector2 from, Vector2 to, float value, bool clamp = true, float amplitude = 1f, float amplitudeDuration = 1f)
		{
			Vector2 result = default(Vector2);
			result.x = Ease(easeType, from.x, to.x, value, clamp, amplitude, amplitudeDuration);
			result.y = Ease(easeType, from.y, to.y, value, clamp, amplitude, amplitudeDuration);
			return result;
		}

		public static Vector3 EaseVector3(EaseType easeType, Vector3 from, Vector3 to, float value, bool clamp = true, float amplitude = 1f, float amplitudeDuration = 1f)
		{
			Vector3 result = default(Vector3);
			result.x = Ease(easeType, from.x, to.x, value, clamp, amplitude, amplitudeDuration);
			result.y = Ease(easeType, from.y, to.y, value, clamp, amplitude, amplitudeDuration);
			result.z = Ease(easeType, from.z, to.z, value, clamp, amplitude, amplitudeDuration);
			return result;
		}

		public static Color EaseColor(EaseType easeType, Color from, Color to, float value, bool clamp = true, float amplitude = 1f, float amplitudeDuration = 1f)
		{
			Color result = default(Color);
			result.r = Ease(easeType, from.r, to.r, value, clamp, amplitude, amplitudeDuration);
			result.g = Ease(easeType, from.g, to.g, value, clamp, amplitude, amplitudeDuration);
			result.b = Ease(easeType, from.b, to.b, value, clamp, amplitude, amplitudeDuration);
			result.a = Ease(easeType, from.a, to.a, value, clamp, amplitude, amplitudeDuration);
			return result;
		}

		private static void DefineSimpleEaseFunctions()
		{
			m_simpleEaseFunctions.Add(EaseType.Lerp, (float b, float c, float t) => b + t * c);
			m_simpleEaseFunctions.Add(EaseType.InQuad, (float b, float c, float t) => c * t * t + b);
			m_simpleEaseFunctions.Add(EaseType.OutQuad, (float b, float c, float t) => (0f - c) * t * (t - 2f) + b);
			m_simpleEaseFunctions.Add(EaseType.InOutQuad, (float b, float c, float t) => ((t *= 2f) < 1f) ? (c / 2f * t * t + b) : ((0f - c) / 2f * ((t -= 1f) * (t - 2f) - 1f) + b));
			m_simpleEaseFunctions.Add(EaseType.InCubic, (float b, float c, float t) => c * t * t * t + b);
			m_simpleEaseFunctions.Add(EaseType.OutCubic, (float b, float c, float t) => c * ((t -= 1f) * t * t + 1f) + b);
			m_simpleEaseFunctions.Add(EaseType.InOutCubic, (float b, float c, float t) => ((t *= 2f) < 1f) ? (c / 2f * t * t * t + b) : (c / 2f * ((t -= 2f) * t * t + 2f) + b));
			m_simpleEaseFunctions.Add(EaseType.InQuart, (float b, float c, float t) => c * t * t * t + b);
			m_simpleEaseFunctions.Add(EaseType.OutQuart, (float b, float c, float t) => (0f - c) * ((t -= 1f) * t * t * t - 1f) + b);
			m_simpleEaseFunctions.Add(EaseType.InOutQuart, (float b, float c, float t) => ((t *= 2f) < 1f) ? (c / 2f * t * t * t * t + b) : ((0f - c) / 2f * ((t -= 2f) * t * t * t - 2f) + b));
			m_simpleEaseFunctions.Add(EaseType.InQuint, (float b, float c, float t) => c * t * t * t * t * t + b);
			m_simpleEaseFunctions.Add(EaseType.OutQuint, (float b, float c, float t) => c * ((t -= 1f) * t * t * t * t + 1f) + b);
			m_simpleEaseFunctions.Add(EaseType.InOutQuint, (float b, float c, float t) => ((t *= 2f) < 1f) ? (c / 2f * t * t * t * t * t + b) : (c / 2f * ((t -= 2f) * t * t * t * t + 2f) + b));
			m_simpleEaseFunctions.Add(EaseType.InSine, (float b, float c, float t) => (0f - c) * Mathf.Cos(t * ((float)Math.PI / 2f)) + c + b);
			m_simpleEaseFunctions.Add(EaseType.OutSine, (float b, float c, float t) => c * Mathf.Sin(t * ((float)Math.PI / 2f)) + b);
			m_simpleEaseFunctions.Add(EaseType.InOutSine, (float b, float c, float t) => (0f - c) / 2f * (Mathf.Cos((float)Math.PI * t) - 1f) + b);
			m_simpleEaseFunctions.Add(EaseType.InExpo, (float b, float c, float t) => (t != 0f) ? (c * Mathf.Pow(2f, 10f * (t - 1f)) + b) : b);
			m_simpleEaseFunctions.Add(EaseType.OutExpo, (float b, float c, float t) => (t != 1f) ? (c * (0f - Mathf.Pow(2f, -10f * t) + 1f) + b) : (b + c));
			m_simpleEaseFunctions.Add(EaseType.InOutExpo, delegate(float b, float c, float t)
			{
				if (t == 0f)
				{
					return b;
				}
				if (t == 1f)
				{
					return b + c;
				}
				return (t / 2f < 1f) ? (c / 2f * Mathf.Pow(2f, 10f * (t - 1f)) + b) : (c / 2f * (0f - Mathf.Pow(2f, -10f * (t -= 1f)) + 2f) + b);
			});
			m_simpleEaseFunctions.Add(EaseType.InCirc, (float b, float c, float t) => (0f - c) * (Mathf.Sqrt(1f - t * t) - 1f) + b);
			m_simpleEaseFunctions.Add(EaseType.OutCirc, (float b, float c, float t) => c * Mathf.Sqrt(1f - (t -= 1f) * t) + b);
			m_simpleEaseFunctions.Add(EaseType.InOutCirc, (float b, float c, float t) => ((t *= 2f) < 1f) ? ((0f - c) / 2f * (Mathf.Sqrt(1f - t * t) - 1f) + b) : (c / 2f * (Mathf.Sqrt(1f - (t -= 2f) * t) + 1f) + b));
			m_simpleEaseFunctions.Add(EaseType.InElastic, delegate(float b, float c, float t)
			{
				float num6 = 0.3f;
				float num7 = num6 / 4f;
				if (t == 0f)
				{
					return b;
				}
				if (t == 1f)
				{
					return b + c;
				}
				if (c >= Mathf.Abs(c))
				{
					num7 = num6 / ((float)Math.PI * 2f) * Mathf.Asin(c / c);
				}
				return 0f - c * Mathf.Pow(2f, 10f * (t -= 1f)) * Mathf.Sin((t - num7) * ((float)Math.PI * 2f) / num6) + b;
			});
			m_simpleEaseFunctions.Add(EaseType.OutElastic, delegate(float b, float c, float t)
			{
				float num4 = 0.3f;
				float num5 = num4 / 4f;
				if (t == 0f)
				{
					return b;
				}
				if (t == 1f)
				{
					return b + c;
				}
				if (c >= Mathf.Abs(c))
				{
					num5 = num4 / ((float)Math.PI * 2f) * Mathf.Asin(c / c);
				}
				return c * Mathf.Pow(2f, -10f * t) * Mathf.Sin((t - num5) * ((float)Math.PI * 2f) / num4) + c + b;
			});
			m_simpleEaseFunctions.Add(EaseType.InOutElastic, delegate(float b, float c, float t)
			{
				float num2 = 0.450000018f;
				float num3 = num2 / 4f;
				if (t == 0f)
				{
					return b;
				}
				if ((t *= 2f) == 1f)
				{
					return b + c;
				}
				if (c >= Math.Abs(c))
				{
					num3 = num2 / ((float)Math.PI * 2f) * Mathf.Asin(c / c);
				}
				return (t < 1f) ? (-0.5f * (c * Mathf.Pow(2f, 10f * (t -= 1f)) * Mathf.Sin((t - num3) * ((float)Math.PI * 2f) / num2)) + b) : (c * Mathf.Pow(2f, -10f * (t -= 1f)) * Mathf.Sin((t - num3) * ((float)Math.PI * 2f) / num2) * 0.5f + c + b);
			});
			m_simpleEaseFunctions.Add(EaseType.InBack, (float b, float c, float t) => c * t * t * (2.70158f * t - 1.70158f) + b);
			m_simpleEaseFunctions.Add(EaseType.OutBack, (float b, float c, float t) => c * ((t -= 1f) * t * (2.70158f * t + 1.70158f) + 1f) + b);
			m_simpleEaseFunctions.Add(EaseType.InOutBack, delegate(float b, float c, float t)
			{
				float num = 1.70158f;
				return ((t *= 2f) < 1f) ? (c / 2f * (t * t * (((num *= 1.525f) + 1f) * t - num)) + b) : (c / 2f * ((t -= 2f) * t * (((num *= 1.525f) + 1f) * t + num) + 2f) + b);
			});
		}

		private static void DefineComplexEaseFunctions()
		{
			m_amplitudeEaseFunctions.Add(EaseType.OutBounce, delegate(float b, float c, float t, float x, float d)
			{
				if ((double)(t /= d) < 0.36363636363636365)
				{
					return c * (7.5625f * t * t) + b;
				}
				if (t < 0.727272749f)
				{
					return c * (7.5625f * (t -= 0.545454562f) * t + 0.75f) + b;
				}
				return (t < 0.909090936f) ? (c * (7.5625f * (t -= 0.8181818f) * t + 0.9375f) + b) : (c * (7.5625f * (t -= 21f / 22f) * t + 63f / 64f) + b);
			});
			m_amplitudeEaseFunctions.Add(EaseType.InBounce, (float b, float c, float t, float x, float d) => c - m_amplitudeEaseFunctions[EaseType.OutBounce](0f, c, d - t, x, d) + b);
			m_amplitudeEaseFunctions.Add(EaseType.InOutBounce, (float b, float c, float t, float x, float d) => (t < d / 2f) ? (m_amplitudeEaseFunctions[EaseType.InBounce](0f, c, t * 2f, x, d) * 0.5f + b) : (m_amplitudeEaseFunctions[EaseType.OutBounce](0f, c, t * 2f - d, x, d) * 0.5f + c * 0.5f + b));
		}
	}
}
