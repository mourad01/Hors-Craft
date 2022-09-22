// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Stats
using Common.Utils;
using System.Collections;
using UnityEngine;

namespace TsgCommon
{
	public class Stats
	{
		private string app;

		private string homeURL;

		private const string REGISTRATION_TS = "registrationTs";

		public Stats(string app, string homeURL)
		{
			this.app = app;
			this.homeURL = homeURL;
		}

		public IEnumerator TrackLogin()
		{
			WWWForm form = new WWWForm();
			form.AddField("app", app);
			form.AddField("registrationTs", PlayerPrefs.GetInt("registrationTs"));
			form.AddField("playerId", PlayerId.GetId());
			yield return new WWW(homeURL + "stats/login", form);
		}

		public IEnumerator TrackRegistration()
		{
			WWWForm form = new WWWForm();
			form.AddField("app", app);
			yield return new WWW(homeURL + "stats/registration", form);
			UnityEngine.Debug.Log("REGISTRATION: " + homeURL + "stats/registration");
		}

		public IEnumerator OnInit(MonoBehaviour mb)
		{
			if (!PlayerPrefs.HasKey("registrationTs"))
			{
				PlayerPrefs.SetInt("registrationTs", Util.now());
				yield return mb.StartCoroutine(TrackRegistration());
			}
			yield return mb.StartCoroutine(TrackLogin());
		}
	}
}
