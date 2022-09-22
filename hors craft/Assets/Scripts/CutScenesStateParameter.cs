// DecompilerFi decompiler from Assembly-CSharp.dll class: CutScenesStateParameter
using Common.Managers.States;
using System;

public class CutScenesStateParameter : StartParameter
{
	public Action onInitEnded
	{
		get;
		private set;
	}

	public CutScenesStateParameter(Action onInitEnded)
	{
		this.onInitEnded = onInitEnded;
	}
}
