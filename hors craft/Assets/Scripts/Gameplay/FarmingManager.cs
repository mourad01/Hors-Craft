// DecompilerFi decompiler from Assembly-CSharp.dll class: Gameplay.FarmingManager
using Common.Managers;
using Common.Managers.Audio;
using Gameplay.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using Uniblocks;
using UnityEngine;

namespace Gameplay
{
	public class FarmingManager : Manager
	{
		public List<GrowableInfoVoxel> growablesData = new List<GrowableInfoVoxel>();

		public GameObject wateringCanPrefab;

		public AudioClip wateringClip;

		public AudioClip sowingClip;

		public AudioClip harvestClip;

		public Action onBuy;

		private List<ushort> harvestable = new List<ushort>();

		private GrowthManager growth => Manager.Get<GrowthManager>();

		public override void Init()
		{
			harvestable = (from d in growablesData
				let id = d.stages.Last().nextBlockIndex
				select id).ToList();
			InitKeys();
		}

		public void InitKeys()
		{
			foreach (GrowableInfoVoxel growablesDatum in growablesData)
			{
				string settingsKey = growablesDatum.settingsKey;
				MonoBehaviourSingleton<ProgressCounter>.get.CreateOrUpdateCountedItem("harvest." + settingsKey);
				MonoBehaviourSingleton<ProgressCounter>.get.CreateOrUpdateCountedItem("placed." + settingsKey);
			}
			MonoBehaviourSingleton<ProgressCounter>.get.CreateOrUpdateCountedItem("farming.harvested");
			MonoBehaviourSingleton<ProgressCounter>.get.CreateOrUpdateCountedItem("farming.placed");
		}

		public bool CanHarvest(VoxelInfo info)
		{
			return harvestable.Contains(info.GetVoxel());
		}

		public bool TryBuy(VoxelInfo info)
		{
			GrowableInfoVoxel growableInfoVoxel = growablesData.FirstOrDefault((GrowableInfoVoxel d) => d.initialBlockIndex == info.GetVoxel());
			if (growableInfoVoxel == null)
			{
				return false;
			}
			GrowableInfo growableInfo = growth.GetGrowableInfo(GrowableInfoVoxel.GetKey(info));
			if (growableInfo != null)
			{
				string text = Manager.Get<TranslationsManager>().GetText("growth.already.taken", "Can't place it here!");
				Manager.Get<ToastManager>().ShowToast(text);
				return false;
			}
			string settingsKey = growableInfoVoxel.settingsKey;
			if (Manager.Get<AbstractSoftCurrencyManager>().TryToBuySomething(settingsKey))
			{
				if (onBuy != null)
				{
					onBuy();
				}
				Manager.Get<StatsManager>().FarmingPlant(growableInfoVoxel.settingsKey);
				return true;
			}
			string text2 = Manager.Get<TranslationsManager>().GetText("soft.currency.not.enough.message", "Not enough coins!");
			Manager.Get<ToastManager>().ShowToast(text2);
			return false;
		}

		public void Placed(VoxelInfo info)
		{
			GrowableInfo growableInfo = growth.GetGrowableInfo(GrowableInfoVoxel.GetKey(info));
			if (growableInfo == null)
			{
				growableInfo = RegisterGrowableInfo(info);
				if (growableInfo != null)
				{
					MonoBehaviourSingleton<ProgressCounter>.get.Increment("placed." + growableInfo.settingsKey);
					MonoBehaviourSingleton<ProgressCounter>.get.Increment("farming.placed");
				}
			}
			else if (IsRebornFlower(growableInfo, info))
			{
				growth.Unregister(growableInfo);
				Placed(info);
			}
		}

		public void RebuildEarth(VoxelInfo info, VoxelInfo aboveInfo)
		{
			GrowableInfo growableInfo = growth.GetGrowableInfo(GrowableInfoVoxel.GetKey(aboveInfo));
			int stage = 0;
			if (growableInfo == null)
			{
				GrowableInfoVoxel serializedInfo = growablesData.FirstOrDefault((GrowableInfoVoxel d) => BelongsTo(d, info, out stage));
				RegisterGrowableInfo(aboveInfo, serializedInfo, stage);
			}
		}

		public void RebuildPlaced(VoxelInfo info)
		{
			GrowableInfo growableInfo = growth.GetGrowableInfo(GrowableInfoVoxel.GetKey(info));
			GrowthData growthData = growth.GetGrowthData(GrowableInfoVoxel.GetKey(info));
			if (growableInfo == null && growthData != null)
			{
				RegisterGrowableInfo(info);
			}
		}

		private GrowableInfo RegisterGrowableInfo(VoxelInfo info, GrowableInfoVoxel serializedInfo = null, int stage = 0)
		{
			if (serializedInfo == null)
			{
				serializedInfo = growablesData.FirstOrDefault((GrowableInfoVoxel d) => BelongsTo(d, info, out stage));
			}
			if (serializedInfo == null)
			{
				UnityEngine.Debug.LogError("Couldn't find serialized farming info for voxel " + info.GetVoxel());
				return null;
			}
			GrowableInfo growableInfo = new GrowableInfoVoxel(serializedInfo, info);
			growth.Register(growableInfo, serializedInfo.settingsKey, stage);
			return growableInfo;
		}

		private bool BelongsTo(GrowableInfoVoxel growableInfo, VoxelInfo info, out int stage)
		{
			stage = 0;
			if (growableInfo.initialBlockIndex == info.GetVoxel())
			{
				return true;
			}
			for (int i = 0; i < growableInfo.stages.Length; i++)
			{
				if (growableInfo.stages[i].nextBlockIndex == info.GetVoxel())
				{
					stage = i + 1;
					return true;
				}
			}
			return false;
		}

		private bool IsRebornFlower(GrowableInfo growableInfo, VoxelInfo info)
		{
			return growablesData.Any((GrowableInfoVoxel d) => d.initialBlockIndex == info.GetVoxel() && growableInfo.CurrentStageNumber() > growableInfo.growStages.Length);
		}

		public void Destroyed(VoxelInfo info)
		{
			string key = GrowableInfoVoxel.GetKey(info);
			GrowableInfo growableInfo = growth.GetGrowableInfo(key);
			if (growableInfo != null)
			{
				int softPriceFor = Manager.Get<ModelManager>().currencySettings.GetSoftPriceFor(growableInfo.settingsKey);
				Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(softPriceFor);
				growth.Unregister(growableInfo);
			}
		}

		public void Harvest(VoxelInfo info)
		{
			GrowableInfoVoxel growableInfoVoxel = growablesData.FirstOrDefault((GrowableInfoVoxel gr) => gr.stages.LastOrDefault().nextBlockIndex == info.GetVoxel());
			int softRewardFor = Manager.Get<ModelManager>().currencySettings.GetSoftRewardFor(growableInfoVoxel.settingsKey);
			Manager.Get<AbstractSoftCurrencyManager>().OnCurrencyAmountChange(softRewardFor);
			info.SetVoxel(0, updateMesh: true, 0);
			growth.ForceGrow(GrowableInfoVoxel.GetKey(info));
			MonoBehaviourSingleton<ProgressCounter>.get.Increment("harvest." + growableInfoVoxel.settingsKey);
			MonoBehaviourSingleton<ProgressCounter>.get.Increment("farming.harvested");
			MonoBehaviourSingleton<GameplayFacts>.get.AddFact(Fact.SOFT_CURRENCY_CHANGED, new ItemToPocketContext
			{
				icon = Engine.GetVoxelType(growableInfoVoxel.initialBlockIndex).voxelSprite,
				startPosition = Engine.VoxelInfoToPosition(info)
			});
			PlayClip(harvestClip);
			Manager.Get<StatsManager>().FarmingHarvest();
		}

		public void Water(VoxelInfo info)
		{
			Index index = new Index(info.index.x, info.index.y + 1, info.index.z);
			info = info.chunk.GetVoxelInfo(index);
			string key = GrowableInfoVoxel.GetKey(info);
			GrowableInfoVoxel growableInfoVoxel = growth.GetGrowableInfo(key) as GrowableInfoVoxel;
			if (growableInfoVoxel.CurrentStageNumber() == 0)
			{
				growth.GrowOnce(key);
			}
			Manager.Get<StatsManager>().FarmingWater();
		}

		public void Sow(VoxelInfo info)
		{
			Index index = new Index(info.index.x, info.index.y + 1, info.index.z);
			VoxelInfo voxelInfo = info.chunk.GetVoxelInfo(index);
			string key = GrowableInfoVoxel.GetKey(voxelInfo);
			GrowthData growthData = growth.GetGrowthData(key);
			GrowableInfoVoxel growableInfoVoxel = growablesData.FirstOrDefault((GrowableInfoVoxel d) => d.settingsKey == growthData.id);
			ushort initialBlockIndex = growableInfoVoxel.initialBlockIndex;
			info.SetVoxel(initialBlockIndex, updateMesh: true, 0);
			GrowableInfo growableInfo = growth.GetGrowableInfo(key);
			if (growableInfo != null)
			{
				growth.Unregister(growableInfo);
				growth.Register(growableInfo, growableInfoVoxel.settingsKey);
			}
			else
			{
				growth.Clear(key);
			}
			PlayClip(sowingClip);
			Manager.Get<StatsManager>().FarmingSow();
		}

		public void PlayClip(AudioClip clip)
		{
			Sound sound = new Sound();
			sound.clip = clip;
			sound.mixerGroup = Manager.Get<MixersManager>().uiMixerGroup;
			Sound sound2 = sound;
			sound2.Play();
		}
	}
}
