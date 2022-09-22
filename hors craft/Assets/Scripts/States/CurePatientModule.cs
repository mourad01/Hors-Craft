// DecompilerFi decompiler from Assembly-CSharp.dll class: States.CurePatientModule
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace States
{
	public class CurePatientModule : GameplayModule
	{
		public Button cureButton;

		protected override Fact[] listenedFacts => new Fact[1]
		{
			Fact.IN_FRONT_OF_PATIENT
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			PatientContext patientContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<PatientContext>(Fact.IN_FRONT_OF_PATIENT).FirstOrDefault();
			if (patientContext != null)
			{
				SetListenerToButton(cureButton, patientContext.onCure);
			}
			cureButton.gameObject.SetActive(patientContext != null);
		}
	}
}
