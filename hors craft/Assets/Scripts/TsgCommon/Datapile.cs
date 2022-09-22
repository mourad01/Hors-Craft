// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Datapile
using MiniJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TsgCommon
{
	public class Datapile : IDatapile
	{
		private int nextId;

		private string homeUrl;

		public Datapile(string homeUrl)
		{
			this.homeUrl = homeUrl;
			nextId = 1;
			while (PlayerPrefs.HasKey(DatapileUtil.key(nextId)))
			{
				nextId++;
			}
		}

		public int generateId()
		{
			return nextId++;
		}

		public void resetId()
		{
			nextId = 1;
		}

		public IEnumerator send()
		{
			List<object> transactions = new List<object>();
			for (int i = 1; i < nextId; i++)
			{
				string key = DatapileUtil.key(i);
				transactions.Add(PlayerPrefs.GetString(key));
				PlayerPrefs.DeleteKey(key);
				resetId();
			}
			PlayerPrefs.Save();
			string data = Json.Serialize(transactions);
			UnityEngine.Debug.Log("Event Data sending: " + data);
			WWWForm form = new WWWForm();
			form.AddField("data", data);
			yield return new WWW(homeUrl + "/event/track", form);
			UnityEngine.Debug.Log("Event Data Sent");
		}

		public void add(string rows)
		{
			try
			{
				string key = DatapileUtil.key(generateId());
				PlayerPrefs.SetString(key, rows);
				PlayerPrefs.Save();
				rows = null;
			}
			catch (PlayerPrefsException ex)
			{
				UnityEngine.Debug.LogError(ex.Message);
			}
		}

		public IDatapileTransaction transaction()
		{
			return new DatapileTransaction(this);
		}
	}
}
