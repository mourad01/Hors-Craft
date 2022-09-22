// DecompilerFi decompiler from Assembly-CSharp.dll class: ActionsContainer
using System;
using System.Collections.Generic;
using UnityEngine;

public class ActionsContainer : MonoBehaviour
{
	public List<Action> actions = new List<Action>();

	public void LaunchEvent(int i)
	{
		if (i >= 0 && i < actions.Count)
		{
			actions[i]();
		}
		else
		{
			UnityEngine.Debug.LogError("Couldn't play action no: " + i + " in object: " + base.gameObject.name);
		}
	}
}
