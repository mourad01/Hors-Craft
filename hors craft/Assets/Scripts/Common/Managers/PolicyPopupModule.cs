// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.PolicyPopupModule
using Common.Managers.States;
using Common.Managers.States.UI;
using Common.Model;
using Common.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Managers
{
	public class PolicyPopupModule : ModelModule
	{
		public const int MAX_POPUPS = 5;

		public const string MESSAGE_SHOWN_KEY = "policy.check.shown.";

		private List<string[]> allMultiStates;

		public static bool CanBeOffline()
		{
			if (StartingPolicyShowState.wasInitialPolicyShown)
			{
				return true;
			}
			return false;
		}

		public static bool AllowToStart()
		{
			if (StartingPolicyShowState.wasInitialPolicyShown)
			{
				return true;
			}
			return Manager.Get<AbstractModelManager>().modelDownloaded;
		}

		public static bool WasPolicyShown(int index)
		{
			return PlayerPrefs.GetInt("policy.check.shown." + index, 0) == 1;
		}

		public static void SetPolicyShown(int popupId, string context)
		{
			PlayerPrefs.SetInt("policy.check.shown." + popupId, 1);
			if (!string.IsNullOrEmpty(context))
			{
				PlayerPrefs.SetInt(string.Format("{0}{1}", "policy.check.shown.", context), 1);
			}
			PlayerPrefs.Save();
		}

		public override void FillModelDescription(ModelDescription descriptions)
		{
			for (int i = 0; i < 5; i++)
			{
				descriptions.AddDescription(keyShowCheckbox(i), defaultValue: false);
				descriptions.AddDescription(keyUrl(i), string.Empty);
				descriptions.AddDescription(keyShowInStates(i), string.Empty);
				descriptions.AddDescription(keyMessageKey(i), "policy.popup.message");
				descriptions.AddDescription(keyContex(i), string.Empty);
			}
			descriptions.AddDescription(keyButtonColorAccept(), "#40d672ff");
			descriptions.AddDescription(keyButtonColorMoreInfo(), "#409fd6ff");
			descriptions.AddDescription(keyButtonColorDisabled(), "#aaaaaaff");
			descriptions.AddDescription(keyEnabled(), defaultValue: false);
		}

		public override void OnModelDownloaded()
		{
			GetAllMultiStates(force: true);
		}

		private string keyUrl(int index)
		{
			return "policy.check.url." + index;
		}

		private string keyContex(int index)
		{
			return "policy.check.context." + index;
		}

		private string keyShowCheckbox(int index)
		{
			return "policy.check.show.checkbox." + index;
		}

		private string keyShowInStates(int index)
		{
			return "policy.check.states." + index;
		}

		private string keyMessageKey(int index)
		{
			return "policy.check.message.translation.key." + index;
		}

		private string keyButtonColorAccept()
		{
			return "policy.check.accept.color";
		}

		private string keyButtonColorMoreInfo()
		{
			return "policy.check.moreinfo.color";
		}

		private string keyButtonColorDisabled()
		{
			return "policy.check.disabled.color";
		}

		private string keyEnabled()
		{
			return "policy.check.enabled";
		}

		public Color GetMoreInfoColor()
		{
			return Misc.HexToColor(base.settings.GetString(keyButtonColorMoreInfo()));
		}

		public Color GetAcceptColor()
		{
			return Misc.HexToColor(base.settings.GetString(keyButtonColorAccept()));
		}

		public Color GetDisabledColor()
		{
			return Misc.HexToColor(base.settings.GetString(keyButtonColorDisabled()));
		}

		public string GetContext(int index)
		{
			return base.settings.GetString(keyContex(index));
		}

		public bool ShouldShowCheckbox(int index)
		{
			return base.settings.GetBool(keyShowCheckbox(index));
		}

		public string GetUrl(int index)
		{
			return base.settings.GetString(keyUrl(index));
		}

		public string GetMessageKey(int index)
		{
			return base.settings.GetString(keyMessageKey(index));
		}

		public bool GetEnabled()
		{
			return base.settings.GetBool(keyEnabled());
		}

		public bool ShouldShowInState<T>(UIConnectedState<T> state) where T : UIConnector
		{
			if (!GetEnabled())
			{
				return false;
			}
			string name = state.GetType().Name;
			List<string[]> list = GetAllMultiStates();
			for (int i = 0; i < list.Count; i++)
			{
				string[] array = list[i];
				for (int j = 0; j < array.Length; j++)
				{
					if ((array[j].Equals(name) || array[j].Equals("*")) && !WasPolicyShown(i))
					{
						return true;
					}
				}
			}
			return false;
		}

		public Queue<PolicyPopupData> GetSettingsForState<T>(UIConnectedState<T> state) where T : UIConnector
		{
			Queue<PolicyPopupData> queue = new Queue<PolicyPopupData>();
			string name = state.GetType().Name;
			List<string[]> list = GetAllMultiStates();
			for (int i = 0; i < list.Count; i++)
			{
				string[] array = list[i];
				for (int j = 0; j < array.Length; j++)
				{
					if ((array[j] == name || array[j] == "*") && !WasPolicyShown(i))
					{
						PolicyPopupData policyPopupData = new PolicyPopupData();
						policyPopupData.popupId = i;
						policyPopupData.showCheckbox = ShouldShowCheckbox(i);
						policyPopupData.url = ParseMultiURL(GetUrl(i));
						policyPopupData.messageKey = GetMessageKey(i);
						policyPopupData.context = GetContext(i);
						queue.Enqueue(policyPopupData);
					}
				}
			}
			return queue;
		}

		private string GetStatesToShowIn(int index)
		{
			return base.settings.GetString(keyShowInStates(index), string.Empty);
		}

		private string ParseMultiURL(string url)
		{
			string[] array = url.Split(new char[3]
			{
				' ',
				'\t',
				'\n'
			}, StringSplitOptions.RemoveEmptyEntries);
			return (array.Length < 1) ? string.Empty : array[UnityEngine.Random.Range(0, array.Length)];
		}

		private string[] ParseMultiStates(string states)
		{
			return states.Split(new char[3]
			{
				' ',
				'\t',
				'\n'
			}, StringSplitOptions.RemoveEmptyEntries);
		}

		private List<string[]> GetAllMultiStates(bool force = false)
		{
			if (allMultiStates == null || force)
			{
				allMultiStates = new List<string[]>();
				for (int i = 0; i < 5; i++)
				{
					allMultiStates.Add(ParseMultiStates(GetStatesToShowIn(i)));
				}
			}
			return allMultiStates;
		}
	}
}
