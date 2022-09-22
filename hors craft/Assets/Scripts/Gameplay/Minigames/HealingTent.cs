// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.Minigames.HealingTent
using com.ootii.Cameras;
using Common.Managers;
using Uniblocks;
using UnityEngine;

namespace Gameplay.Minigames
{
	public class HealingTent : InteractiveObject
	{
		public float healAmount = 100f;

		public float healCooldown = 30f;

		private float cooldown;

		protected override void Awake()
		{
			base.Awake();
			cooldown = 0f;
		}

		private void Update()
		{
			if (cooldown > 0f)
			{
				cooldown -= Time.deltaTime;
			}
		}

		public override void OnUse()
		{
			base.OnUse();
			if (cooldown <= 0f)
			{
				Health componentInChildren = CameraController.instance.Anchor.GetComponentInChildren<Health>();
				componentInChildren.hp += healAmount;
				cooldown = healCooldown;
			}
			else
			{
				string text = Manager.Get<TranslationsManager>().GetText("healing.tent.cooldown", "Next heal in {0}").ToUpper();
				string text2 = text.Replace("{0}", Mathf.RoundToInt(cooldown) + "s");
				Manager.Get<ToastManager>().ShowToast(text2);
			}
		}
	}
}
