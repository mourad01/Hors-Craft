// DecompilerFi decompiler from Assembly-CSharp.dll class: States.MountModule
using System.Collections.Generic;
using System.Linq;
using UnityEngine.UI;

namespace States
{
	public class MountModule : GameplayModule
	{
		public Button mountButton;

		protected override Fact[] listenedFacts => new Fact[2]
		{
			Fact.IN_FRONT_OF_MOUNTABLE_MOB,
			Fact.MOUNTED_MOB
		};

		public override void Init()
		{
			base.Init();
		}

		public override void OnFactsChanged(HashSet<Fact> changedFacts)
		{
			InteractiveObjectContext interactiveObjectContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<InteractiveObjectContext>(Fact.MOUNTED_MOB).FirstOrDefault();
			if (interactiveObjectContext == null)
			{
				interactiveObjectContext = MonoBehaviourSingleton<GameplayFacts>.get.GetFactContexts<InteractiveObjectContext>(Fact.IN_FRONT_OF_MOUNTABLE_MOB).FirstOrDefault();
			}
			if (interactiveObjectContext != null)
			{
				SetListenerToButton(mountButton, interactiveObjectContext.useAction);
			}
			mountButton.gameObject.SetActive(interactiveObjectContext != null);
		}
	}
}
