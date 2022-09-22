// DecompilerFi decompiler from Assembly-CSharp.dll class: TutorialCorrectColorBeh
using System;
using UnityEngine;

public class TutorialCorrectColorBeh : CustomSearchBehaviourAbstractPure
{
	private Color tutorialColor = new Color(0f, 0f, 0f, 0.78f);

	public override Action<GameObject, string> GetFunction()
	{
		return Function;
	}

	private void Function(GameObject gameObject, string path)
	{
		GenericTutorial component = gameObject.GetComponent<GenericTutorial>();
		if (component != null)
		{
			component.darkColor = tutorialColor;
		}
		if (string.IsNullOrEmpty(path))
		{
		}
	}
}
