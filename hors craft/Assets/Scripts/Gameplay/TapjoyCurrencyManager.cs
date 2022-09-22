// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.TapjoyCurrencyManager
using Common.Managers;
using Common.Utils;
using System;
using System.Collections;
using TapjoyUnity;
using Uniblocks;
using UnityEngine;

namespace Gameplay
{
	public class TapjoyCurrencyManager : Manager
	{
		public float askForCurrencyInterval = 1f;

		private string lastBalanceId = "tapjoy.last.balance.value";

		private int lastBalance
		{
			get
			{
				return PlayerPrefs.GetInt(lastBalanceId, 0);
			}
			set
			{
				PlayerPrefs.SetInt(lastBalanceId, value);
			}
		}

		public override void Init()
		{
			Tapjoy.OnGetCurrencyBalanceResponse += CurrencyBalanceResponse;
			StartCoroutine(AskTapjoyForBalance());
		}

		private IEnumerator AskTapjoyForBalance()
		{
			while (true)
			{
				if (Manager.Get<ModelManager>().modelDownloaded)
				{
					UnityEngine.Debug.Log("asking for currency balance...");
					Tapjoy.GetCurrencyBalance();
				}
				yield return Enumerators.WaitForRealSeconds(askForCurrencyInterval);
			}
		}

		private void CurrencyBalanceResponse(string currencyName, int balance)
		{
			UnityEngine.Debug.Log("got new currency balance! " + balance);
			int lastBalance = this.lastBalance;
			if (balance < lastBalance)
			{
				this.lastBalance = balance;
			}
			else if (balance > lastBalance)
			{
				int num = balance - lastBalance;
				this.lastBalance = balance;
				int num2 = 0;
				IEnumerator enumerator = Enum.GetValues(typeof(Voxel.Category)).GetEnumerator();
				try
				{
					while (enumerator.MoveNext())
					{
						Voxel.Category category = (Voxel.Category)enumerator.Current;
						num2 += Manager.Get<ModelManager>().blocksUnlocking.GetNoOfBlocksToUnlockPerAd(category);
					}
				}
				finally
				{
					IDisposable disposable;
					if ((disposable = (enumerator as IDisposable)) != null)
					{
						disposable.Dispose();
					}
				}
				int num3 = num / num2;
				int @int = PlayerPrefs.GetInt("numberOfWatchedRewardedAds");
				@int += num3;
				PlayerPrefs.SetInt("numberOfWatchedRewardedAds", @int);
				UnityEngine.Debug.Log("Tapjoy balance changed! Watched Ads before: " + (@int - num3) + " after: " + @int + " balance diff: " + num);
			}
		}
	}
}
