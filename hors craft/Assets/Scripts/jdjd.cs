// DecompilerFi decompiler from Assembly-CSharp.dll class: jdjd
using GameUI;
using System;
using UnityEngine;

public class jdjd : CustomSearchBehaviourAbstract
{
	public GameplaySubstateSpriteSwap one;

	public GameplaySubstateSpriteSwap two;

	public GameplaySubstateSpriteSwap three;

	public PatternRepeater pattern;

	public Material patternMaterial;

	public SpawnGameObjectExe popup;

	public GameObject skinListMan;

	public GameObject skinListMan2;

	public GameObject skinListMan3;

	public int iterator = -1;

	public override Action<GameObject, string> GetFunction()
	{
		return Action;
	}

	public void SetChildren(Transform parent)
	{
		parent.position = Vector3.zero;
		for (int i = 0; i < parent.childCount; i++)
		{
			SetChildren(parent.GetChild(i));
		}
	}

	private void Action(GameObject gameObject, string path)
	{
	}
}
