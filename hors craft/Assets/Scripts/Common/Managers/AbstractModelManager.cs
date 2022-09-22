// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Managers.AbstractModelManager
using Common.Model;
using Common.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Common.Managers
{
	public abstract class AbstractModelManager : Manager
	{
		public SafeModel.Mode modelMode = SafeModel.Mode.TWO_STEPS_SETTINGS;

		public string[] forbiddenToLogFromSafeModel;

		private float assumeModelDownloaded = 14f;

		private float waitTimeForSettings = 30f;

		private float timeWaitedForSettings = 30f;

		protected SafeModel safeModel;

		private List<ModelModule> modules;

		public CommonAdSettingsModule commonAdSettings;

		public PolicyPopupModule policyPopupSettings;

		public CrosspromoSetupModule crosspromoSettings;

		public AllInOneAdRequirementsModule allInOneAdRequirements;

		public RankingsModule rankingsSettings;

		public AchievementsModule achievementsSettings;

		public ModelModuleContext modulesContext
		{
			get;
			private set;
		}

		public bool modelDownloaded
		{
			get;
			private set;
		}

		protected abstract int modelVersion
		{
			get;
		}

		public ModelDescription modelDescription
		{
			get;
			private set;
		}

		public override void Init()
		{
			ConstructModulesAndStartDownload();
		}

		public virtual bool ModelCheck()
		{
			return modelDownloaded;
		}

		protected virtual void ConstructModulesAndStartDownload()
		{
			modules = ConstructModelModules();
			AddDefaultModelModules();
			modulesContext = ConstructContext();
			modelDescription = new ModelDescription(modelVersion);
			foreach (ModelModule module in modules)
			{
				module.FillModelDescription(modelDescription);
				module.AssignIAPContext(modulesContext);
			}
			timeWaitedForSettings = Time.realtimeSinceStartup + waitTimeForSettings;
			StopCoroutinesAndDownload();
		}

		public void StopCoroutinesAndDownload()
		{
			StopAllCoroutines();
			ConnectionInfoManager connectionInfoManager = Manager.Get<ConnectionInfoManager>();
			safeModel = new SafeModel(connectionInfoManager.gameName, connectionInfoManager.homeURL, modelDescription, modelMode, forbiddenToLogFromSafeModel);
			safeModel.StartDownloadFromServer(this, ModelDownloaded);
			if (PolicyPopupModule.CanBeOffline())
			{
				StartCoroutine(AssumeModelDownloaded(assumeModelDownloaded));
			}
			LoadDefaultSettings(safeModel);
			foreach (ModelModule module in modules)
			{
				module.AssignSettings(safeModel.settings);
			}
		}

		public void SetDownloadTimeLimits(float assumeModelDownloaded, float waitTimeForSettings)
		{
			this.assumeModelDownloaded = assumeModelDownloaded;
			this.waitTimeForSettings = waitTimeForSettings;
		}

		private void LoadDefaultSettings(SafeModel safeModel)
		{
			TextAsset textAsset = Resources.Load<TextAsset>("Game/settings");
			if (!(textAsset == null))
			{
				Settings.SerializableSettings serializableSettings = JSONHelper.Deserialize<Settings.SerializableSettings>(textAsset.text);
				if (serializableSettings == null)
				{
					UnityEngine.Debug.LogError("Couldnt deserialize settings.txt");
					return;
				}
				Settings settings = Settings.FromSerializableSettings(serializableSettings);
				safeModel.SaveSettings(settings);
			}
		}

		protected abstract List<ModelModule> ConstructModelModules();

		private ModelModuleContext ConstructContext()
		{
			return new ModelModuleContext();
		}

		public IEnumerator AssumeModelDownloaded(float after)
		{
			yield return new WaitForSecondsRealtime(after);
			if (!modelDownloaded)
			{
				UnityEngine.Debug.Log("Assumed Model Download");
				ModelDownloaded();
			}
		}

		protected virtual void ModelDownloaded()
		{
			modelDownloaded = true;
			foreach (ModelModule module in modules)
			{
				module.OnModelDownloaded();
			}
		}

		private void AddDefaultModelModules()
		{
			modules.Add(commonAdSettings = new CommonAdSettingsModule());
			modules.Add(policyPopupSettings = new PolicyPopupModule());
			modules.Add(crosspromoSettings = new CrosspromoSetupModule());
			modules.Add(allInOneAdRequirements = new AllInOneAdRequirementsModule());
			modules.Add(rankingsSettings = new RankingsModule());
			modules.Add(achievementsSettings = new AchievementsModule());
			//AddIfNotAlreadyAdded(modules, new AdWaterfallOrdersModule());
			AddIfNotAlreadyAdded(modules, new RetentionNotificationsTextsModule());
		}

		private void AddIfNotAlreadyAdded(List<ModelModule> modules, ModelModule module)
		{
			if (modules.Find((ModelModule obj) => obj.GetType() == module.GetType()) == null)
			{
				modules.Add(module);
			}
		}

		public override string ToString()
		{
			return base.ToString();
		}

		public string ToString(string[] forbidden, string[] allowed)
		{
			return safeModel.settings.ToString(forbidden, allowed);
		}

		public bool CheckModelDownloadError(Action<Action> onError)
		{
			bool modelDownloaded = Manager.Get<AbstractModelManager>().modelDownloaded;
			if (Time.realtimeSinceStartup > timeWaitedForSettings && !this.modelDownloaded)
			{
				onError?.Invoke(delegate
				{
					timeWaitedForSettings = Time.realtimeSinceStartup + waitTimeForSettings;
				});
				return true;
			}
			return false;
		}
	}
}
