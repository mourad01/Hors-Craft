// DecompilerFi decompiler from Assembly-CSharp.dll class: States.SurvivalHUDModule
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class SurvivalHUDModule : GameplayModule
	{
		public GameObject hungerGO;

		public Text hp;

		public Text armor;

		public Text arrows;

		public Text time;

		public Text hungerText;

		public Animator hpAnimator;

		public Animator armorAnimator;

		public Animator arrowsAnimator;

		public GameObject iconSun;

		public GameObject iconNight;

		public Image ammoImage;

		private Sprite defaultAmmoIco;

		private float previousMinutes = -1f;

		private float previousHp = -1f;

		private int previousAmmo = -1;

		private float previousArmor = -1f;

		protected override Fact[] listenedFacts => new Fact[5]
		{
			Fact.SURVIVAL_HEALTH,
			Fact.SURVIVAL_ARMOR,
			Fact.SURVIVAL_AMMO_TYPE,
			Fact.HUNGER,
			Fact.TIME_OF_DAY
		};

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			base.Update();
			if (changedFacts.Contains(Fact.SURVIVAL_HEALTH))
			{
				SurvivalHealth survivalHealth = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<SurvivalHealth>(Fact.SURVIVAL_HEALTH).FirstOrDefault();
				if (survivalHealth != null)
				{
					UpdateHealth(survivalHealth.healthComponent.hp, survivalHealth.healthComponent.maxHp);
				}
				hp.transform.parent.parent.gameObject.SetActive(survivalHealth != null);
			}
			if (changedFacts.Contains(Fact.SURVIVAL_ARMOR))
			{
				SurvivalArmor survivalArmor = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<SurvivalArmor>(Fact.SURVIVAL_ARMOR).FirstOrDefault();
				if (survivalArmor != null)
				{
					if (survivalArmor.armorComponent is ArmorPercentageMod)
					{
						survivalArmor = null;
					}
					else
					{
						UpdateArmor(survivalArmor.armorComponent.armorValue, survivalArmor.armorComponent.maxValue);
					}
				}
				armor.transform.parent.parent.gameObject.SetActive(survivalArmor != null);
			}
			if (changedFacts.Contains(Fact.SURVIVAL_AMMO_TYPE))
			{
				SurvivalAmmoType survivalAmmoType = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<SurvivalAmmoType>(Fact.SURVIVAL_AMMO_TYPE).FirstOrDefault();
				if (survivalAmmoType != null)
				{
					UpdateAmmo(survivalAmmoType.ammoCount, survivalAmmoType.ammoMax, survivalAmmoType.ammoIco, survivalAmmoType.show);
				}
				arrows.transform.parent.parent.gameObject.SetActive(survivalAmmoType != null);
			}
			if (changedFacts.Contains(Fact.HUNGER))
			{
				HungerContext hungerContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<HungerContext>(Fact.HUNGER).FirstOrDefault();
				if (hungerContext != null)
				{
					SetHungerProgress(hungerContext.hunger);
				}
				hungerText.transform.parent.parent.gameObject.SetActive(hungerContext != null);
			}
			if (changedFacts.Contains(Fact.TIME_OF_DAY))
			{
				TimeOfDayContext timeOfDayContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<TimeOfDayContext>(Fact.TIME_OF_DAY).FirstOrDefault();
				if (timeOfDayContext != null)
				{
					UpdateTime(timeOfDayContext.time);
				}
				time.transform.parent.parent.gameObject.SetActive(timeOfDayContext != null);
			}
		}

		private void UpdateTime(float dayTime)
		{
			float num = 24f * dayTime;
			int num2 = (int)num;
			int num3 = (int)((num - (float)num2) * 60f);
			if (previousMinutes != (float)num3)
			{
				time.text = num2.ToString("00") + ":" + num3.ToString("00");
				previousMinutes = num3;
				bool isNight = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<TimeOfDayContext>(Fact.TIME_OF_DAY).FirstOrDefault().isNight;
				iconSun.SetActive(!isNight);
				iconNight.SetActive(isNight);
			}
		}

		private void UpdateHealth(float current, float max)
		{
			if (previousHp != current)
			{
				hp.text = Mathf.Ceil(current) + "/" + Mathf.Ceil(max);
				hpAnimator.SetTrigger("Bump");
				previousHp = current;
			}
		}

		private void UpdateAmmo(int current, int max, Sprite ico, bool show)
		{
			if (defaultAmmoIco == null)
			{
				defaultAmmoIco = ammoImage.sprite;
			}
			if (previousAmmo != current || ammoImage.sprite != ico)
			{
				arrows.text = current.ToString() + ((max <= 0) ? string.Empty : ("/" + max.ToString()));
				ammoImage.sprite = (ico ?? defaultAmmoIco);
				arrowsAnimator.SetTrigger("Bump");
				previousAmmo = current;
			}
			ammoImage.gameObject.SetActive(show);
			arrows.gameObject.SetActive(show);
		}

		private void UpdateArmor(float current, float max)
		{
			if (previousArmor != current)
			{
				armor.text = Mathf.Ceil(current) + "/" + Mathf.Ceil(max);
				armorAnimator.SetTrigger("Bump");
				previousArmor = current;
			}
		}

		private void SetHungerProgress(float progress)
		{
			hungerGO.SetActive(value: true);
			hungerText.text = (progress * 100f).ToString("00") + "/100";
		}
	}
}
