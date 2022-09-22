// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.TalkHoverAction
using Common.Managers;
using Gameplay;
using Gameplay.RhythmicMinigame;
using States;
using UnityEngine;

namespace Uniblocks
{
	public class TalkHoverAction : HoverAction
	{
		private PlayerGraphic hitMobGraphic;

		private InteractiveObjectContext talkContext;

		private InteractiveObjectContext danceContext;

		public TalkHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			bool flag = false;
			Collider collider = hitInfo.hit.collider;
			if (IsTargetTalkative(collider))
			{
				Mob componentInParent = collider.gameObject.GetComponentInParent<Mob>();
				if (componentInParent != null && !IsFighting(componentInParent))
				{
					hitMobGraphic = componentInParent.GetComponentInChildren<PlayerGraphic>();
					if (hitMobGraphic != null)
					{
						flag = ((!(componentInParent is HumanMob) && (bool)componentInParent.GetComponent<Pettable>()) ? ((!Manager.Contains<PetManager>()) ? Manager.Get<MobsManager>().talkWithMobs : Manager.Get<PetManager>().talkWithMobs) : ((!(componentInParent.GetComponent<Patient>() != null) || !componentInParent.GetComponent<Patient>().gotDisese) ? true : false));
					}
				}
			}
			if (flag)
			{
				UpdateInFrontOfTalkable(collider);
			}
			else
			{
				UpdateNoHit();
			}
		}

		private void UpdateInFrontOfTalkable(Collider hit)
		{
			if (IsDifferentThanBefore())
			{
				bool flag = !SymbolsHelper.isWebGL;
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_TALKABLE, talkContext);
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_DANCEABLE, danceContext);
				danceContext = null;
				talkContext = new InteractiveObjectContext
				{
					obj = hitMobGraphic.gameObject,
					useAction = OnTalkClick
				};
				MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_FRONT_OF_TALKABLE, talkContext);
				Danceable componentInParent = hit.gameObject.GetComponentInParent<Danceable>();
				if (flag && componentInParent != null && componentInParent.isDancing)
				{
					danceContext = new InteractiveObjectContext
					{
						obj = hitMobGraphic.gameObject,
						useAction = OnDance
					};
					MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.IN_FRONT_OF_DANCEABLE, danceContext);
				}
			}
		}

		private void UpdateNoHit()
		{
			if (talkContext != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_TALKABLE, talkContext);
				talkContext = null;
			}
			if (danceContext != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_DANCEABLE, danceContext);
				danceContext = null;
			}
		}

		private bool IsDifferentThanBefore()
		{
			return talkContext == null || talkContext.obj != hitMobGraphic.gameObject;
		}

		private void OnTalkClick()
		{
			Manager.Get<StateMachineManager>().PushState<ChatBotState>(new ChatBotStateStartParameter
			{
				botName = ((hitMobGraphic.skinInfo == null) ? null : ((!hitMobGraphic.skinInfo.npcName.IsNOTNullOrEmpty()) ? null : hitMobGraphic.skinInfo.npcName)),
				mobGraphic = hitMobGraphic,
				pettable = hitMobGraphic.GetComponentInParent<Mob>().GetComponentInChildren<Pettable>()
			});
		}

		private void OnDance()
		{
			UsableIndicator componentInParent = hitMobGraphic.GetComponentInParent<UsableIndicator>();
			if (componentInParent != null)
			{
				componentInParent.Interact();
			}
			switch (hitMobGraphic.GetComponentInParent<Danceable>().dancingStyle)
			{
			case DanceConfig.DanceStyle.modern:
				Manager.Get<StateMachineManager>().PushState<RhythmicMinigameState>(new RhythmicMinigameStateStartParameter
				{
					graphicScene = new DanceScene(PlayerGraphic.GetControlledPlayerInstance(), hitMobGraphic, hitMobGraphic.GetComponentInParent<Pettable>())
				});
				break;
			case DanceConfig.DanceStyle.ancient:
				Manager.Get<StateMachineManager>().PushState<RhythmicMinigameState>(new RhythmicMinigameStateStartParameter
				{
					graphicScene = new AncientDanceScene(PlayerGraphic.GetControlledPlayerInstance(), hitMobGraphic, hitMobGraphic.GetComponentInParent<Pettable>())
				});
				break;
			}
		}

		private bool IsTargetTalkative(Collider hit)
		{
			return hit != null && hit.gameObject.layer == 16 && Manager.Get<ModelManager>().chatbotSettings.IsChatBotEnabled() && Manager.Get<ModelManager>().chatbotSettings.IsWebChatBotEnabled();
		}

		private bool IsFighting(Mob mob)
		{
			IFighting fighting = mob as IFighting;
			return fighting != null;
		}
	}
}
