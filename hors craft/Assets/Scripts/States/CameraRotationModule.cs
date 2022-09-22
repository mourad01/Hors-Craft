// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CameraRotationModule
using GameUI;
using System.Collections.Generic;
using System.Linq;

namespace States
{
	public class CameraRotationModule : GameplayModule
	{
		public SimpleRepeatButton rotateButton;

		protected override Fact[] listenedFacts => new Fact[3]
		{
			Fact.MOVEMENT,
			Fact.MCPE_STEERING,
			Fact.IN_BLUEPRINT_RANGE
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			if (MonoBehaviourSingleton<GameplayFacts>.get.FactExists(Fact.MCPE_STEERING))
			{
				if (base.gameObject.activeSelf)
				{
					base.gameObject.SetActive(value: false);
				}
				return;
			}
			if (!base.gameObject.activeSelf)
			{
				base.gameObject.SetActive(value: true);
			}
			MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<CameraRotationContext>(Fact.MOVEMENT).FirstOrDefault()?.setCameraRotationButton(rotateButton);
			MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<BlueprintFillContext>(Fact.IN_BLUEPRINT_RANGE).FirstOrDefault()?.setFillVoxelButton(rotateButton);
		}
	}
}
