// DecompilerFi decompiler from Assembly-CSharp.dll class: States.UseActionModule
using System.Collections.Generic;
using UnityEngine.UI;

namespace States
{
	public class UseActionModule : GameplayModule
	{
		public Button useButton;

		protected override Fact[] listenedFacts => new Fact[12]
		{
			Fact.MOUNTED_MOB,
			Fact.IN_VEHICLE,
			Fact.INSIDE_INTERACTIVE_OBJECT,
			Fact.IN_FRONT_OF_INTERACTIVE_OBJECT,
			Fact.IN_FRONT_OF_VEHICLE,
			Fact.IN_FRONT_OF_DANCEABLE,
			Fact.IN_FRONT_OF_TALKABLE,
			Fact.IN_FRONT_OF_MOUNTABLE_MOB,
			Fact.IN_FRONT_OF_USABLE_CUTSCENE,
			Fact.IN_FRONT_OF_DOORS,
			Fact.IN_FRONT_OF_SWITCHABLE_VOXEL,
			Fact.CHANGING_CLOTHES
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			List<InteractiveObjectContext> contexts = new List<InteractiveObjectContext>();
			Fact[] listenedFacts = this.listenedFacts;
			foreach (Fact fact in listenedFacts)
			{
				contexts.AddRange(MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<InteractiveObjectContext>(fact));
			}
			if (contexts.Count > 0)
			{
				useButton.gameObject.SetActive(value: true);
				useButton.onClick.RemoveAllListeners();
				useButton.onClick.AddListener(delegate
				{
					contexts[0].useAction();
				});
			}
			else
			{
				useButton.gameObject.SetActive(value: false);
			}
		}
	}
}
