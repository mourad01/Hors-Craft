// DecompilerFi decompiler from Assembly-CSharp.dll class: Cooking.WaveGenerator
using Common.Managers;
using Common.Utils;
using Gameplay;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Cooking
{
	public static class WaveGenerator
	{
		private static int level;

		private static float spawnTimeSum = 0f;

		private static HashSet<Product> usedProductsInThisWave = new HashSet<Product>();

		private static CookingModule _cookingSettings;

		private static CookingModule cookingSettings
		{
			get
			{
				if (_cookingSettings == null)
				{
					_cookingSettings = Manager.Get<ModelManager>().cookingSettings;
				}
				return _cookingSettings;
			}
		}

		private static WorkController workController => Manager.Get<CookingManager>().workController;

		public static Wave GenerateWave(int level)
		{
			spawnTimeSum = 0f;
			usedProductsInThisWave = new HashSet<Product>();
			WaveGenerator.level = level;
			Wave wave = new Wave();
			wave.customers = GenerateCustomers();
			wave.bonusGoal = GenerateBonusGoal(wave.customers.Count);
			wave.timeLimit = CalculateTimeLimit(wave.customers);
			return wave;
		}

		private static BonusGoalInfo GenerateBonusGoal(int customerNumber)
		{
			BonusGoalInfo bonusGoalInfo = new BonusGoalInfo();
			bonusGoalInfo.moneyRequired = CalculateMoneyRequired(customerNumber);
			bonusGoalInfo.bonusMoney = CalculateBonusMoney(bonusGoalInfo.moneyRequired);
			bonusGoalInfo.bonusPrestige = CalculateBonusPrestige(bonusGoalInfo.moneyRequired);
			return bonusGoalInfo;
		}

		private static int CalculateBonusMoney(int moneyRequired)
		{
			float num = cookingSettings.BonusGoalMoneyPercent();
			return Mathf.RoundToInt((float)moneyRequired * num);
		}

		private static float CalculateBonusPrestige(int moneyRequired)
		{
			float num = cookingSettings.BonusGoalPrestigePercent();
			return Mathf.RoundToInt((float)moneyRequired * num);
		}

		private static int CalculateMoneyRequired(int customerNumber)
		{
			int num = cookingSettings.BonusGoalMoneyPerCustomer();
			float num2 = cookingSettings.BonusGoalMoneyPerLevel();
			return Mathf.RoundToInt((float)(num * customerNumber) * 0.75f * (1f + num2 * (float)(level + (level - 1) % 5)));
		}

		private static float CalculateTimeLimit(List<CustomerConfig> customers)
		{
			float num = cookingSettings.WaveTimePerCustomer() * (float)customers.Count;
			float num2 = 1f + cookingSettings.WaveTimeMultiplierPerLevel() * (float)((level - 1) % 5);
			return num * num2;
		}

		private static List<CustomerConfig> GenerateCustomers()
		{
			List<CustomerConfig> list = new List<CustomerConfig>();
			spawnTimeSum = 0f;
			usedProductsInThisWave = new HashSet<Product>();
			float value = CalculateCustomersNumber();
			int num = cookingSettings.MaxCustomersNumber();
			value = Mathf.Clamp(value, 0f, num);
			value = Mathf.RoundToInt(value);
			for (int i = 0; (float)i < value; i++)
			{
				list.Add(GenerateCustomer(i + 1));
			}
			return list;
		}

		private static CustomerConfig GenerateCustomer(int count)
		{
			CustomerConfig customerConfig = new CustomerConfig();
			customerConfig.products = new List<Product>();
			List<Product> avaibleProducts = workController.recipesList.GetAvaibleProducts();
			List<Product> source = (from p in avaibleProducts
				where !workController.workData.WasProductTutorialShown(p.GetKey())
				select p).ToList();
			source = (from p in source
				where !usedProductsInThisWave.Contains(p)
				select p).ToList();
			int num2;
			if (cookingSettings.IsMultipleOrdersEnabled())
			{
				float num = Random.Range(0f, 1f);
				num2 = ((num < cookingSettings.GetThreeOrdersChance() && level >= cookingSettings.GetThreeOrdersAvailableAfter()) ? 3 : ((!(num < cookingSettings.GetThreeOrdersChance() + cookingSettings.GetTwoOrdersChance()) || level < cookingSettings.GetTwoOrdersAvailableAfter()) ? 1 : 2));
			}
			else
			{
				num2 = 1;
			}
			for (int i = 0; i < num2; i++)
			{
				Product item = (source.Count <= 0) ? avaibleProducts.Random() : source.Random();
				usedProductsInThisWave.Add(item);
				customerConfig.products.Add(item);
				if (source.Count > 0)
				{
					break;
				}
			}
			customerConfig.spawnTime = spawnTimeSum + CalculateCustomerSpawnTime(count);
			spawnTimeSum = customerConfig.spawnTime;
			customerConfig.waitTime = CalculateCustomerWaitTime(customerConfig.products);
			return customerConfig;
		}

		private static float CalculateCustomerSpawnTime(int count)
		{
			if (count == 1)
			{
				return 2f;
			}
			float num = cookingSettings.SpawnTimePerCustomer();
			float num2 = SpawnTimeDeviation();
			float num3 = 1f + (float)((level - 1) % 5) * cookingSettings.SpawnTimeMultiModifierPerLevel();
			return (num + num2) * num3;
		}

		private static float SpawnTimeDeviation()
		{
			return (Random.value - 0.5f) * 2f * cookingSettings.SpawnTimeDeviation();
		}

		private static float CalculateCustomersNumber()
		{
			int num = cookingSettings.BaseCustomersNumber();
			float num2 = cookingSettings.CustomersPerLevel();
			return (float)num + (float)(level + (level - 1) % 5) * num2;
		}

		private static float CalculateCustomerWaitTime(List<Product> products)
		{
			float num = 0f;
			float num2 = 0f;
			foreach (Product product in products)
			{
				num += product.baseWaitTime;
			}
			num -= (float)((level - 1) % 5);
			num2 += workController.workPlace.GetPercentUpgradeEffectSummarized(Device.UpgradeEffect.PATIENCE);
			return num * num2;
		}
	}
}
