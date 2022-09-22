// DecompilerFi decompiler from Assembly-CSharp.dll class: AnimationCurveSerializer
using System;
using UnityEngine;

[Serializable]
public class AnimationCurveSerializer
{
	[Serializable]
	public class KeyframeData
	{
		public float inTangent;

		public float outTangent;

		public int tangentMode;

		public float time;

		public float value;

		public KeyframeData()
		{
		}

		public KeyframeData(Keyframe keyFrame)
		{
			inTangent = keyFrame.inTangent;
			outTangent = keyFrame.outTangent;
			tangentMode = keyFrame.tangentMode;
			time = keyFrame.time;
			value = keyFrame.value;
		}

		public Keyframe FromData()
		{
			return new Keyframe(time, value, inTangent, outTangent);
		}
	}

	public KeyframeData[] keyframesData;

	public AnimationCurveSerializer()
	{
	}

	public AnimationCurveSerializer(AnimationCurve curve)
	{
		keyframesData = new KeyframeData[curve.keys.Length];
		for (int i = 0; i < curve.keys.Length; i++)
		{
			keyframesData[i] = new KeyframeData(curve.keys[i]);
		}
	}

	public AnimationCurve Deserialize()
	{
		Keyframe[] array = new Keyframe[keyframesData.Length];
		for (int i = 0; i < keyframesData.Length; i++)
		{
			array[i] = keyframesData[i].FromData();
		}
		return new AnimationCurve(array);
	}
}
