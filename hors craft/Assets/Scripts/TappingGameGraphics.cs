// DecompilerFi decompiler from Assembly-CSharp.dll class: TappingGameGraphics
using States;
using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TappingGameGraphics : MonoBehaviour
{
	public Action callAfterAnimations;

	public virtual void InitGraphics()
	{
	}

	public virtual void OnProgress(float amount)
	{
	}

	public virtual void OnWin()
	{
		callAfterAnimations();
	}

	public virtual void OnLose()
	{
		callAfterAnimations();
	}

	public virtual void OnResetRound()
	{
		callAfterAnimations();
	}

	public virtual void OnFinish()
	{
	}

	public abstract List<GameObject> GetThingsForTutorial(TappingGameStateConnector connector);
}
