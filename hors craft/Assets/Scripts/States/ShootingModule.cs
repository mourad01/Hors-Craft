// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ShootingModule
using Common.Utils;
using GameUI;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class ShootingModule : GameplayModule
	{
		public GameObject shootingPanel;

		public SimpleRepeatButton attackButton;

		public Button leftWeapon;

		public Button rightWeapon;

		public Sprite defaultAttackSprite;

		public Image ammoCountImage;

		public Image attackImage;

		public Image frameImage;

		private Action<bool> updateShootButton;

		protected override Fact[] listenedFacts => new Fact[4]
		{
			Fact.SURVIVAL_SHOOTING_CONFIG,
			Fact.SURVIVAL_PHASE,
			Fact.HAS_MORE_THAN_ONE_WEAPON,
			Fact.SURVIVAL_AMMO_COUNT
		};

		private void OnEnable()
		{
			UpdateShootingConfig();
		}

		public override void Init()
		{
			base.Init();
		}

		protected override void Update()
		{
			base.Update();
			if (updateShootButton != null)
			{
				updateShootButton(attackButton.pressed);
			}
		}

		public void ForceButtonUpdate(bool arg)
		{
			if (updateShootButton != null)
			{
				updateShootButton(arg);
			}
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			if (changedFacts.Contains(Fact.SURVIVAL_SHOOTING_CONFIG))
			{
				UpdateShootingConfig();
			}
			if (changedFacts.Contains(Fact.SURVIVAL_PHASE))
			{
				UpdateVisibility();
			}
			if (changedFacts.Contains(Fact.HAS_MORE_THAN_ONE_WEAPON))
			{
				bool active = MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.HAS_MORE_THAN_ONE_WEAPON);
				if (leftWeapon != null)
				{
					leftWeapon.gameObject.SetActive(active);
				}
				if (rightWeapon != null)
				{
					rightWeapon.gameObject.SetActive(active);
				}
			}
			if (changedFacts.Contains(Fact.SURVIVAL_AMMO_COUNT))
			{
				UpdateAmmoCount();
			}
		}

		private void UpdateAmmoCount()
		{
			SurvivalAmmoCountContext survivalAmmoCountContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<SurvivalAmmoCountContext>(Fact.SURVIVAL_AMMO_COUNT).FirstOrDefault();
			if (survivalAmmoCountContext != null)
			{
				float num = 1f - (float)survivalAmmoCountContext.currentAmmo / (float)survivalAmmoCountContext.maxAmmo;
				float a = (!survivalAmmoCountContext.isReloading) ? 0.4f : Easing.Ease(EaseType.OutBack, 0.4f, 0.8f, Mathf.Clamp(num / 0.1f, 0f, 1f));
				ammoCountImage.color = new Color(0.13f, 0.13f, 0.13f, a);
				ammoCountImage.fillAmount = num;
				if (survivalAmmoCountContext.customSprite == null)
				{
					attackImage.sprite = defaultAttackSprite;
				}
				else
				{
					attackImage.sprite = survivalAmmoCountContext.customSprite;
				}
			}
			else
			{
				ammoCountImage.color = new Color(0.13f, 0.13f, 0.13f, 0f);
			}
		}

		private void UpdateShootingConfig()
		{
			ShootingPanelContext shootingPanelContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<ShootingPanelContext>(Fact.SURVIVAL_SHOOTING_CONFIG).FirstOrDefault();
			if (shootingPanelContext != null)
			{
				if (leftWeapon != null)
				{
					SetListenerToButton(leftWeapon, shootingPanelContext.onLeftButton);
				}
				if (rightWeapon != null)
				{
					SetListenerToButton(rightWeapon, shootingPanelContext.onRightButton);
				}
				updateShootButton = shootingPanelContext.onShoot;
			}
			else
			{
				updateShootButton = null;
			}
		}

		private void UpdateVisibility()
		{
			SurvivalPhaseContext survivalPhaseContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<SurvivalPhaseContext>(Fact.SURVIVAL_PHASE).FirstOrDefault();
			if (survivalPhaseContext != null)
			{
				shootingPanel.SetActive(survivalPhaseContext.isCombat);
			}
		}
	}
}
