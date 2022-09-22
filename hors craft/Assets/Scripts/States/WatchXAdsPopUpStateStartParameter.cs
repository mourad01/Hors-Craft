// DecompilerFi decompiler from Assembly-CSharp.dll class: States.WatchXAdsPopUpStateStartParameter
using Common.Managers;
using Common.Managers.States;
using System;
using System.Collections.Generic;
using Uniblocks;
using UnityEngine;

namespace States
{
	public class WatchXAdsPopUpStateStartParameter : StartParameter
	{
		public List<Sprite> voxelSpritesToUnlock = new List<Sprite>();

		public AdsCounters type;

		public Voxel.Category blockCategory;

		public StatsManager.AdReason reason;

		public string description = "Watch ad to unlock following textures";

		public string translationKey = "menu.watch.ads";

		public int numberOfAdsNeeded = 1;

		public bool immediatelyAd;

		public bool allowRemoveAdsButton = true;

		public Action<bool> onSuccess;

		public Action<GameObject> configWatchButton;

		public Action<GameObject> configCancelButton;

		public Action<WatchXAdsPopUpStateConnector> updateAction;
	}
}
