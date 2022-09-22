// DecompilerFi decompiler from Assembly-CSharp.dll class: GameSpecificColorChanger
using Common.Managers;
using System;
using UnityEngine;
using UnityEngine.UI;

public class GameSpecificColorChanger : MonoBehaviour
{
	public enum TargetType
	{
		Image,
		Text
	}

	[Serializable]
	public class TargetObjectConfiguration
	{
		public string gameName;

		public Color color;

		public TargetType target;

		public TargetObjectConfiguration()
		{
			gameName = string.Empty;
			color = Color.white;
			target = TargetType.Image;
		}
	}

	public TargetObjectConfiguration[] gameSpecificColors = new TargetObjectConfiguration[1]
	{
		new TargetObjectConfiguration()
	};

	private void Awake()
	{
		UpdateColor();
		UnityEngine.Object.Destroy(this);
	}

	public void UpdateColor()
	{
		string gameName = Manager.Get<ConnectionInfoManager>().gameName;
		if (gameName.IsNullOrEmpty())
		{
			return;
		}
		Image component = GetComponent<Image>();
		Text component2 = GetComponent<Text>();
		if (!component && !component2)
		{
			UnityEngine.Debug.LogError("GameObject doesn't have any target component");
			return;
		}
		TargetObjectConfiguration[] array = gameSpecificColors;
		foreach (TargetObjectConfiguration targetObjectConfiguration in array)
		{
			if (targetObjectConfiguration.gameName == gameName)
			{
				if (targetObjectConfiguration.target == TargetType.Image && (bool)component)
				{
					component.color = targetObjectConfiguration.color;
				}
				else if (targetObjectConfiguration.target == TargetType.Text && (bool)component2)
				{
					component2.color = targetObjectConfiguration.color;
				}
			}
		}
	}
}
