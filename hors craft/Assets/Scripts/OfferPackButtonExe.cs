// DecompilerFi decompiler from Assembly-CSharp.dll class: OfferPackButtonExe
using Common.Managers;
using States;
using UnityEngine;

[CreateAssetMenu(menuName = "Initial Popup Req/Offerpack Button Exec")]
public class OfferPackButtonExe : InitialPopupExecution
{
	public override void Show()
	{
		OfferPackManager offerPackManager = Manager.Get<OfferPackManager>();
		int @int = PlayerPrefs.GetInt("scaledTimeStartup", 0);
		if (offerPackManager.ShouldShowStarterPack(@int))
		{
			Manager.Get<StateMachineManager>().PushState<StarterPackState>();
		}
		else if (offerPackManager.ShouldShowOfferPack(@int))
		{
			Manager.Get<StateMachineManager>().PushState<OfferPackState>();
		}
		MonoBehaviourSingleton<GameplayFacts>.get.RemoveFactContexts<OfferpackStarterTimerContext>(Fact.OFFERPACK_ENABLED);
	}
}
