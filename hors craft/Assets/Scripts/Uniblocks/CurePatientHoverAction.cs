// DecompilerFi decompiler from Assembly-CSharp.dll class: Uniblocks.CurePatientHoverAction
using UnityEngine;

namespace Uniblocks
{
	public class CurePatientHoverAction : HoverAction
	{
		private PlayerGraphic hitMobGraphic;

		private Patient patient;

		private PatientContext patientContext;

		public CurePatientHoverAction(HoverActionsConnector connector)
			: base(connector)
		{
		}

		public override void Update(RaycastHitInfo hitInfo)
		{
			base.Update(hitInfo);
			bool flag = false;
			Collider collider = hitInfo.hit.collider;
			if (IsOurTarget(collider))
			{
				patient = collider.gameObject.GetComponentInParent<Patient>();
				if (patient != null)
				{
					if (patient.gotDisese)
					{
						hitMobGraphic = patient.GetComponentInChildren<PlayerGraphic>();
						if (hitMobGraphic != null)
						{
							flag = true;
						}
					}
					else
					{
						flag = false;
					}
				}
			}
			if (flag)
			{
				CheckContext(patient);
				MonoBehaviourSingleton<GameplayFacts>.get.AddFactIfNotExisting(Fact.IN_FRONT_OF_PATIENT, patientContext);
			}
			else if (patientContext != null)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_PATIENT, patientContext);
				patientContext = null;
			}
		}

		private void CheckContext(Patient patient)
		{
			if (patientContext == null || patientContext.patient != patient)
			{
				MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContext(Fact.IN_FRONT_OF_PATIENT, patientContext);
				patientContext = new PatientContext
				{
					patient = patient.gameObject,
					onCure = OnCure
				};
			}
		}

		private bool IsOurTarget(Collider hit)
		{
			return hit != null && hit.gameObject.layer == 16;
		}

		public void OnCure()
		{
			patient.TryToHeal();
		}
	}
}
