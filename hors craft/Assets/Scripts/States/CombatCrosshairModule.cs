// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CombatCrosshairModule
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class CombatCrosshairModule : GameplayModule
	{
		public Image crosshair;

		public Sprite defaultCrosshair;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.WEAPON_EQUIPPED
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			WeaponEquippedContext weaponEquippedContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<WeaponEquippedContext>(Fact.WEAPON_EQUIPPED).FirstOrDefault();
			if (weaponEquippedContext != null)
			{
				SetSprite(weaponEquippedContext.crosshairSprite);
			}
			else
			{
				SetSprite(defaultCrosshair);
			}
		}

		private void SetSprite(Sprite sprite)
		{
			crosshair.sprite = sprite;
			crosshair.rectTransform.sizeDelta = new Vector2(sprite.texture.width, sprite.texture.height);
		}
	}
}
