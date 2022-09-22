// DecompilerFi decompiler from Assembly-CSharp.dll class: States.VerticalMovementModule
using Gameplay;
using GameUI;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

namespace States
{
	public class VerticalMovementModule : GameplayModule
	{
		public SimpleRepeatArea fallButton;

		public SimpleRepeatArea jumpButton;

		protected override Fact[] listenedFacts
		{
			[CompilerGenerated]
			get
			{
				return new Fact[6]
				{
					Fact.MOVEMENT,
					Fact.MOVE_MODE,
					Fact.UNDERWATER,
					Fact.MOUNTED_MOB,
					Fact.IN_VEHICLE,
					Fact.MCPE_STEERING
				};
			}
		}

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			bool flag = true;
			McpeContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<McpeContext>(Fact.MCPE_STEERING);
			if (factContext != null && factContext.flyInCameraDirection)
			{
				flag = false;
			}
			if (base.gameObject.activeSelf != flag)
			{
				base.gameObject.SetActive(flag);
			}
			if (!flag)
			{
				return;
			}
			MoveModeContext moveModeContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<MoveModeContext>(Fact.MOVE_MODE).FirstOrDefault();
			bool active = MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.UNDERWATER) || (moveModeContext != null && moveModeContext.mode == GlobalSettings.MovingMode.FLYING);
			if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.IN_VEHICLE))
			{
				active = false;
			}
			if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.MOUNTED_MOB))
			{
				InteractiveObjectContext interactiveObjectContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<InteractiveObjectContext>(Fact.MOUNTED_MOB).FirstOrDefault();
				active = (((interactiveObjectContext != null && interactiveObjectContext.obj != null && interactiveObjectContext.obj.GetComponent<Mountable>().isFlying) || MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.UNDERWATER)) ? true : false);
			}
			fallButton.gameObject.SetActive(active);
			jumpButton.gameObject.SetActive(value: true);
			if (changedFacts.Contains(Fact.MOVEMENT) || changedFacts.Contains(Fact.MCPE_STEERING))
			{
				MovementContext movementContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<MovementContext>(Fact.MOVEMENT).FirstOrDefault();
				if (movementContext != null)
				{
					movementContext.setFallButton(() => fallButton.pressed);
					movementContext.setJumpButton(() => jumpButton.pressed);
				}
			}
		}
	}
}
