// DecompilerFi decompiler from Assembly-CSharp.dll class: AmbientColorBehaviur1
using System;
using UnityEngine;

public class AmbientColorBehaviur1 : CustomSearchBehaviourAbstract
{
	public Color Night = new Color(0.38f, 0.392f, 0.467f);

	public Color StartNight = new Color(0.627f, 0.357f, 0.149f);

	public Color Day = new Color(0.573f, 0.51f, 0.255f);

	public Color StartDay = new Color(0.396f, 0.431f, 0.341f);

	public Color LightColor = new Color(0.957f, 0.957f, 0.957f);

	public override Action<GameObject, string> GetFunction()
	{
		return Action;
	}

	private void Action(GameObject gameObject, string path)
	{
	}
}
