// DecompilerFi decompiler from Assembly-CSharp.dll class: States.ProgressFragmentBehaviourCustomTranslationKeys
using Common.Managers;

namespace States
{
	public class ProgressFragmentBehaviourCustomTranslationKeys : ProgressFragmentBehaviour
	{
		public string[] translationKeys;

		protected override void SetStats()
		{
			fragment.ClearStats();
			for (int i = 0; i < trackedProgress.Count; i++)
			{
				int countFor = MonoBehaviourSingleton<ProgressCounter>.get.GetCountFor(trackedProgress[i]);
				string text = Manager.Get<TranslationsManager>().GetText(translationKeys[i], trackedProgress[i].ToUpper().Replace('_', ' ').Replace('.', ' ') + ": ");
				fragment.AddProgressStat(text, countFor.ToString());
			}
		}
	}
}
