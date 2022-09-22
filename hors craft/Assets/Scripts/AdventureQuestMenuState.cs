// DecompilerFi decompiler from Assembly-CSharp.dll class: AdventureQuestMenuState
using Common.Behaviours;
using Common.Managers.States;
using States;
using System.Linq;

public class AdventureQuestMenuState : PauseState
{
	public override void StartState(StartParameter parameter)
	{
		base.StartState(parameter);
		fragments.First((FragmentComponent frag) => frag.fragmentName == "Blocks").button.transform.parent.gameObject.SetActive(MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper);
		fragments.First((FragmentComponent frag) => frag.fragmentName == "Blueprints").button.transform.parent.gameObject.SetActive(MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper);
		fragments.First((FragmentComponent frag) => frag.fragmentName == "Crafting").button.transform.parent.gameObject.SetActive(MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper);
		fragments.First((FragmentComponent frag) => frag.fragmentName == "Settings").button.transform.parent.gameObject.SetActive(MonoBehaviourSingleton<DeveloperModeBehaviour>.get.isDeveloper);
	}

	public override void FinishState()
	{
		base.FinishState();
	}

	public override void UpdateState()
	{
		base.UpdateState();
	}
}
