// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.GameplaySubstateMechAttackButton
using GameUI;
using States;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay
{
	public class GameplaySubstateMechAttackButton : MonoBehaviour, IGameplaySubstateAction
	{
		public List<Sprite> frameSprites;

		private ShootingModule shootingScript;

		private void Awake()
		{
		}

		public Action<ModuleLoader, GameObject> GetAction()
		{
			return ChangeSprites;
		}

		public void ChangeSprites(ModuleLoader moduleLoader, GameObject other)
		{
			ShootingModule component = other.GetComponent<ShootingModule>();
			if (!(component == null))
			{
				shootingScript = component;
				ChangeSprites();
			}
		}

		public void ChangeSprites()
		{
			if (!(shootingScript == null))
			{
				FormChanger componentInChildren = PlayerGraphic.GetControlledPlayerInstance().GetComponentInChildren<FormChanger>();
				int partLevel = componentInChildren.GetPartLevel("weapon");
				shootingScript.frameImage.sprite = frameSprites[partLevel];
				shootingScript.ammoCountImage.sprite = frameSprites[partLevel];
				ColorController component = shootingScript.frameImage.gameObject.GetComponent<ColorController>();
				if (component != null)
				{
					UnityEngine.Object.Destroy(component);
				}
				shootingScript.frameImage.color = Color.white;
			}
		}
	}
}
