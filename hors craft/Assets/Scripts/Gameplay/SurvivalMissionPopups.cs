// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.SurvivalMissionPopups
using Common.Managers;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Gameplay
{
	public class SurvivalMissionPopups : MonoBehaviour
	{
		public GameObject popupPrefab;

		private SurvivalManager survival;

		private void Awake()
		{
			survival = GetComponent<SurvivalManager>();
		}

		public void DayPhaseChanged()
		{
			if (survival.IsCombatTime())
			{
				ShowFightPopup();
			}
			else
			{
				ShowExplorePopup();
			}
		}

		private void ShowExplorePopup()
		{
			string fallback = "It's daylight. Time to build!";
			string text = Manager.Get<TranslationsManager>().GetText("army.popup.explore.title", fallback);
			string[] textForKey = GetTextForKey(GetExplorePopupKey);
			ShowText(text, textForKey);
		}

		private void ShowFightPopup()
		{
			string fallback = "The night has come. Time to fight!";
			string text = Manager.Get<TranslationsManager>().GetText("army.popup.fight.title", fallback);
			string[] textForKey = GetTextForKey(GetFightPopupKey);
			ShowText(text, textForKey);
		}

		private void ShowText(string firstText, string[] missionText)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate(popupPrefab);
			gameObject.transform.SetParent(Manager.Get<CanvasManager>().canvas.transform, worldPositionStays: true);
			gameObject.transform.SetAsFirstSibling();
			RectTransform component = gameObject.GetComponent<RectTransform>();
			RectTransform rectTransform = component;
			Vector2 offsetMin = component.offsetMin;
			rectTransform.offsetMin = new Vector2(0f, offsetMin.y);
			RectTransform rectTransform2 = component;
			Vector2 offsetMax = component.offsetMax;
			rectTransform2.offsetMax = new Vector2(0f, offsetMax.y);
			gameObject.transform.GetChild(0).GetComponentInChildren<Text>().text = firstText;
			if (missionText != null && missionText.Length >= 2 && (!string.IsNullOrEmpty(missionText[0]) || !string.IsNullOrEmpty(missionText[1])))
			{
				gameObject.transform.GetChild(1).GetComponentInChildren<Text>().text = missionText[0];
				gameObject.transform.GetChild(2).GetComponentInChildren<Text>().text = missionText[1];
			}
			else
			{
				gameObject.transform.GetChild(1).GetComponentInChildren<Text>().text = string.Empty;
				gameObject.transform.GetChild(2).GetComponentInChildren<Text>().text = string.Empty;
			}
		}

		private string[] GetTextForKey(Func<int, string> func)
		{
			float fullPassedTime = SurvivalContextsBroadcaster.instance.GetContext<DayTimeContext>().fullPassedTime;
			int arg = (int)fullPassedTime;
			string text = Manager.Get<TranslationsManager>().GetText(func(arg), string.Empty);
			string[] array = new string[2];
			array = text.Split(new string[1]
			{
				"\" "
			}, StringSplitOptions.None);
			if (array.Length >= 2)
			{
				array[0] += "\"";
			}
			return array;
		}

		private string GetFightPopupKey(int i)
		{
			return "army.popup.fight." + i;
		}

		private string GetExplorePopupKey(int i)
		{
			return "army.popup.explore." + i;
		}
	}
}
