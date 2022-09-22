// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Test
using System.Collections.Generic;
using UnityEngine;

namespace TsgCommon
{
	public class Test : MonoBehaviour
	{
		private void Start()
		{
		}

		private void Update()
		{
		}

		private void makeSomeFun()
		{
			Datapile datapile = new Datapile("https://world-dev.devs.tensquaregames.com");
			IDatapileTransaction datapileTransaction = datapile.transaction();
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("name", "Microsoft Windows");
			DatapileIdRef value = datapileTransaction.ensure("mobile_test", "os_family", dictionary);
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			dictionary2.Add("name", "Windows 8!");
			dictionary2.Add("os_family_id", value);
			DatapileIdRef value2 = datapileTransaction.ensure("mobile_test", "os", dictionary2);
			Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
			dictionary3.Add("player_id", 1337);
			dictionary3.Add("os_id", value2);
			dictionary3.Add("ts", Util.now());
			datapileTransaction.insert("mobile_test", "login_event", dictionary3);
			datapileTransaction.commit();
			StartCoroutine(datapile.send());
		}

		private void OnCollisionEnter(Collision collision)
		{
			makeSomeFun();
		}
	}
}
