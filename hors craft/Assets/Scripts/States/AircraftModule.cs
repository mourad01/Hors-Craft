// DecompilerFi decompiler from Assembly-CSharp.dll class: States.AircraftModule
using GameUI;
using System.Collections.Generic;
using System.Linq;

namespace States
{
	public class AircraftModule : GameplayModule
	{
		public SimpleRepeatButton thrustButton;

		public SimpleRepeatButton ascendButton;

		public SimpleRepeatButton descendButton;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.MOVEMENT
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			AircraftSteerintContext aircraftSteerintContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<AircraftSteerintContext>(Fact.MOVEMENT).FirstOrDefault();
			if (aircraftSteerintContext != null)
			{
				aircraftSteerintContext.setThrustButton(thrustButton);
				aircraftSteerintContext.setAscendButton(ascendButton);
				aircraftSteerintContext.setDescendButton(descendButton);
			}
		}
	}
}
