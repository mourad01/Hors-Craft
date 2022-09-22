// DecompilerFi decompiler from Assembly-CSharp.dll class: InMobiManager
using Common.Managers;
using UnityEngine;

public class InMobiManager : Manager
{
	[SerializeField]
	private string appId = string.Empty;

	public override void Init()
	{
		UnityEngine.Debug.LogWarning("Using InMobiManager, but INMOBI_ENABLED is not defined");
	}
}
