// DecompilerFi decompiler from Assembly-CSharp.dll class: CutSceneAct
using System.Collections.Generic;
using UnityEngine;

public class CutSceneAct : MonoBehaviour
{
	public List<CutScenePart> parts = new List<CutScenePart>();

	[SerializeField]
	protected float actTime = 10f;

	public float ActTime => actTime;

	public int PartsCount
	{
		get
		{
			if (parts == null)
			{
				return 0;
			}
			int num = 0;
			foreach (CutScenePart part in parts)
			{
				num += part.AnimatorsCount;
			}
			return num;
		}
	}

	internal void Init()
	{
		UnityEngine.Debug.Log($"Init act : {GetType()} name: {base.transform.name}");
		foreach (CutScenePart part in parts)
		{
			part.Init();
		}
	}

	public int Run()
	{
		foreach (CutScenePart part in parts)
		{
			UnityEngine.Debug.Log($"Starting part : {part.GetType()} name: {part.partName}");
			part.RunAnimators();
		}
		return parts.Count;
	}

	public void CleanUp()
	{
		foreach (CutScenePart part in parts)
		{
			part.CleanUp();
		}
	}
}
