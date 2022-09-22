// DecompilerFi decompiler from Assembly-CSharp.dll class: VersionDependend
using UnityEngine;

public static class VersionDependend
{
	public static string applicationBundleIdentifier => Application.identifier;

	public static float GetParticleSystemDuration(ParticleSystem system)
	{
		return system.main.duration;
	}

	public static Transform FindTransform(this Transform t, string childName)
	{
		return t.Find(childName);
	}
}
