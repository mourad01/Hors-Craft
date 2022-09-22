// DecompilerFi decompiler from Assembly-CSharp.dll class: WebGLReq
using UnityEngine;

[CreateAssetMenu(menuName = "Initial Popup Req/Web GL")]
public class WebGLReq : InitialPopupRequirements
{
	public override bool CanBeShown()
	{
		return SymbolsHelper.isWebGL;
	}
}
