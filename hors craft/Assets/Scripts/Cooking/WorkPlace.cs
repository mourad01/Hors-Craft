// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.WorkPlace
using Common.Managers;
using Common.Utils;
using Gameplay;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cooking
{
	public class WorkPlace : MonoBehaviour
	{
		[Serializable]
		public class CustomerPlace
		{
			public Transform place;

			public Transform moneyPlace;

			public Transform workerPlace;

			public Transform orderPanelPlace;

			[HideInInspector]
			public bool isFree = true;

			[HideInInspector]
			public Customer customer;
		}

		public CustomerPlace[] customerPlaces;

		public List<Device> devices
		{
			get;
			private set;
		}

		private void Awake()
		{
			devices = base.gameObject.GetComponentsInChildren<Device>().ToList();
			CustomerPlace[] array = customerPlaces;
			foreach (CustomerPlace customerPlace in array)
			{
				customerPlace.isFree = true;
			}
		}

		public void InitDevices()
		{
			foreach (Device device in devices)
			{
				device.gameObject.SetActive(device.Unlocked());
				device.Init();
			}
		}

		public List<T> GetDevicesOfType<T>() where T : Device
		{
			List<T> list = new List<T>();
			for (int i = 0; i < devices.Count; i++)
			{
				if (devices[i] is T)
				{
					list.Add(devices[i] as T);
				}
			}
			return list;
		}

		public bool AnySpotFree()
		{
			return customerPlaces.Any((CustomerPlace cp) => cp.isFree);
		}

		public CustomerPlace GetFreeCustomerPlace()
		{
			if (!AnySpotFree())
			{
				return null;
			}
			return (from cp in customerPlaces
				where cp.isFree
				select cp).Random();
		}

		public float GetUpgradeEffectSummarized(Device.UpgradeEffect effect)
		{
			Device[] source = (from d in devices
				where d.upgradeEffects.Contains(effect)
				select d).ToArray();
			return source.Sum((Device d) => GetUpgradeValue(d, effect));
		}

		public float GetPercentUpgradeEffectSummarized(Device.UpgradeEffect effect)
		{
			Device[] source = (from d in devices
				where d.upgradeEffects.Contains(effect)
				select d).ToArray();
			return 1f + source.Sum((Device d) => GetUpgradeValue(d, effect) - 1f);
		}

		private float GetUpgradeValue(Device device, Device.UpgradeEffect effect)
		{
			return Manager.Get<ModelManager>().cookingSettings.GetUpgradeValue(device.Key, UpgradeName(effect), device.GetUpgradeLevel());
		}

		private string UpgradeName(Device.UpgradeEffect effect)
		{
			return Misc.CustomNameToKey(effect.ToString());
		}
	}
}
