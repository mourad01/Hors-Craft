// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.AutoSavingManager
using Common.Managers;
using States;
using Uniblocks;
using UnityEngine;

namespace Gameplay
{
	public class AutoSavingManager : Manager
	{
		public bool disableFrequentSave;

		public float FREQUENT_SAVE_INTERVAL = 10f;

		public float INFREQUENT_SAVE_INTERVAL_AND = 45f;

		public float INFREQUENT_SAVE_INTERVAL_IOS = 90f;

		public float INFREQUENT_SAVE_INTERVAL_EDITOR = 300f;

		private float nextFrequentSaveTime;

		private float nextInfrequentSaveTime;

		private float infrequentSaveInterval => INFREQUENT_SAVE_INTERVAL_AND;

		public override void Init()
		{
			nextFrequentSaveTime = Time.time + FREQUENT_SAVE_INTERVAL;
			nextInfrequentSaveTime = Time.time + infrequentSaveInterval;
		}

		private void Update()
		{
			if (!Manager.Get<StateMachineManager>().IsCurrentStateA<GameplayState>())
			{
				return;
			}
			if (!disableFrequentSave && Time.time > nextFrequentSaveTime)
			{
				Manager.Get<GameCallbacksManager>().FrequentSave();
				nextFrequentSaveTime = Time.time + FREQUENT_SAVE_INTERVAL;
				UnityEngine.Debug.Log("Frequent autosave.");
			}
			if (Time.time > nextInfrequentSaveTime)
			{
				Engine.SaveWorld();
				if (disableFrequentSave)
				{
					Manager.Get<GameCallbacksManager>().FrequentSave();
					UnityEngine.Debug.Log("Frequent autosave.");
				}
				Manager.Get<GameCallbacksManager>().InFrequentSave();
				nextInfrequentSaveTime = Time.time + infrequentSaveInterval;
				UnityEngine.Debug.Log("Infrequent autosave.");
			}
		}
	}
}
