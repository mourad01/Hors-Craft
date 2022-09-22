// DecompilerFi decompiler from Assembly-CSharp.dll class: States.TameButtonModule
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class TameButtonModule : GameplayModule
	{
		public Button tameButton;

		public Button followButton;

		public Image followImage;

		public Sprite stopFollowingSprite;

		public Sprite startFollowingSprite;

		protected override Fact[] listenedFacts => new Fact[3]
		{
			Fact.TAME_PANEL_CONFIG,
			Fact.IN_FRONT_OF_TAMEABLE,
			Fact.TOOLKIT_ENABLED
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			if (changedFacts.Contains(Fact.TAME_PANEL_CONFIG))
			{
				OnConfigsChange();
			}
			if (changedFacts.Contains(Fact.IN_FRONT_OF_TAMEABLE))
			{
				OnTamePanelChange();
			}
			if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.TOOLKIT_ENABLED))
			{
				followButton.gameObject.SetActive(value: false);
			}
		}

		private void OnConfigsChange()
		{
			TamePanelContext tamePanelContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<TamePanelContext>(Fact.TAME_PANEL_CONFIG).FirstOrDefault();
			if (tamePanelContext != null)
			{
				SetListenerToButton(tameButton, tamePanelContext.onTame);
				SetListenerToButton(followButton, tamePanelContext.onMoveModeChange);
			}
		}

		private void OnTamePanelChange()
		{
			if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_FRONT_OF_LOVED_ONE) || MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_FRONT_OF_LOVABLE) || MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_FRONT_OF_UNLOVABLE))
			{
				tameButton.transform.parent.gameObject.SetActive(value: false);
				followButton.gameObject.SetActive(value: false);
				return;
			}
			TameContext tameContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<TameContext>(Fact.IN_FRONT_OF_TAMEABLE).FirstOrDefault();
			if (tameContext != null)
			{
				followImage.sprite = ((!tameContext.pettable.following) ? startFollowingSprite : stopFollowingSprite);
			}
			tameButton.transform.parent.gameObject.SetActive(tameContext?.tameButtonAllowed ?? false);
			followButton.gameObject.SetActive(tameContext?.pettable.tamed ?? false);
		}
	}
}
