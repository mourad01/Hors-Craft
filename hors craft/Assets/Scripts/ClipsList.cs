// DecompilerFi decompiler from Assembly-CSharp.dll class: ClipsList
using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ClipsList", menuName = "ScriptableObjects/ClipsList", order = 0)]
public class ClipsList : ScriptableObject
{
	[Serializable]
	public class ClipWithWeight
	{
		public AudioClip clip;

		public int weight;
	}

	public List<ClipWithWeight> audioClips;
}
