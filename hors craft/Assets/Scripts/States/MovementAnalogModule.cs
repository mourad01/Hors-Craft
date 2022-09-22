// DecompilerFi decompiler from Assembly-CSharp.dll class: States.MovementAnalogModule
using GameUI;
using System.Collections.Generic;
using System.Linq;

namespace States
{
	public class MovementAnalogModule : GameplayModule
	{
		public AnalogController analog;

		public SimpleRepeatButton analogButton;

		protected override Fact[] listenedFacts => new Fact[2]
		{
			Fact.MAIN_ANALOG,
			Fact.MCPE_STEERING
		};

		public override void Init()
		{
			base.Init();
		}

		public bool IsEnabled()
		{
			bool result = false;
			McpeContext factContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContext<McpeContext>(Fact.MCPE_STEERING);
			if (factContext != null && !factContext.flyInCameraDirection)
			{
				result = true;
			}
			return result;
		}

		public override void OnFactsChanged(HashSet<Fact> facts)
		{
			bool flag = IsEnabled();
			if (base.gameObject.activeSelf != flag)
			{
				base.gameObject.SetActive(flag);
			}
			if (flag)
			{
				SetAnalogController();
			}
		}

		private void SetAnalogController()
		{
			MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<AnalogInputContext>(Fact.MAIN_ANALOG).FirstOrDefault()?.setAnalogController(analog, analogButton);
		}
	}
}
