// DecompilerFi decompiler from Assembly-CSharp.dll class: States.EmptyLoadingState
using Common.Managers;
using Common.Managers.States;

namespace States
{
	public class EmptyLoadingState : XCraftUIState<LoadLevelStateConnector>
	{
		public override void StartState(StartParameter parameter)
		{
			LoadingStartParameter loadingStartParameter = parameter as LoadingStartParameter;
			base.StartState(parameter);
			base.connector.Init(Manager.Get<TranslationsManager>().GetText(string.Format("loading.tag.{0}", loadingStartParameter.reason.ToLower(), loadingStartParameter.defaultText), string.Empty));
		}

		public void UpdateProgressState(string text, float value)
		{
			if (base.connector.subtitle != null)
			{
				base.connector.subtitle.text = text;
			}
			base.connector.UpdateLoadProgress(value);
		}
	}
}
