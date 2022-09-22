// DecompilerFi decompiler from Assembly-CSharp.dll class: ChooseFactionStateConnector
using Common.Managers.States.UI;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ChooseFactionStateConnector : UIConnector
{
	[SerializeField]
	private GameObject uiPrefab;

	[SerializeField]
	private Button eastButton;

	[SerializeField]
	private Button westButton;

	[SerializeField]
	private Faction[] factions;

	private Faction chosenFaction;

	private Action onEastPressed;

	private Action onWestPressed;

	public void Init(Action onEastPressed, Action onWestPressed)
	{
		this.onEastPressed = onEastPressed;
		this.onWestPressed = onWestPressed;
		InitButtons();
	}

	public Faction GetEast()
	{
		return factions[0];
	}

	public Faction GetWest()
	{
		return factions[1];
	}

	private void InitButtons()
	{
		eastButton.onClick.AddListener(delegate
		{
			if (onEastPressed != null)
			{
				onEastPressed();
			}
		});
		westButton.onClick.AddListener(delegate
		{
			if (onWestPressed != null)
			{
				onWestPressed();
			}
		});
	}
}
