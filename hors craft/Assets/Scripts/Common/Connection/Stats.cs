// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Connection.Stats
using System.Collections;
using UnityEngine;

namespace Common.Connection
{
	public class Stats
	{
		private string gamename;

		private string homeURL;

		private const string REGISTRATION_TS = "registrationTs";

		public Stats(string gamename, string homeURL)
		{
			this.gamename = gamename;
			this.homeURL = homeURL;
		}

		private WWWForm CreateCommonWWWForm()
		{
			WWWForm wWWForm = FormFactory.CreateBasicWWWForm();
			wWWForm.AddField("device_model", SystemInfo.deviceModel);
			wWWForm.AddField("graphic_model", SystemInfo.graphicsDeviceName);
			return wWWForm;
		}

		private IEnumerator TrackLogin()
		{
			yield return new WWW(form: CreateCommonWWWForm(), url: homeURL + "stats/login");
		}

		private IEnumerator TrackRegistration()
		{
			yield return new WWW(form: CreateCommonWWWForm(), url: homeURL + "stats/registration");
		}

		public IEnumerator OnInit(MonoBehaviour coroutinesProvider)
		{
			if (!PlayerPrefs.HasKey("registrationTs"))
			{
				PlayerPrefs.SetInt("registrationTs", Util.now());
				yield return coroutinesProvider.StartCoroutine(TrackRegistration());
			}
			yield return coroutinesProvider.StartCoroutine(TrackLogin());
		}

		public IEnumerator TrackStatsQueue(string queueJson)
		{
			WWWForm form = CreateCommonWWWForm();
			form.AddField("stats", queueJson);
			yield return new WWW(homeURL + "stats/trackMultipleStats", form);
		}
	}
}
