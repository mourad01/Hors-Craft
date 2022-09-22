// DecompilerFi decompiler from Assembly-CSharp.dll class: AutoRefreshingStock
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public static class AutoRefreshingStock
{
	public class StockItem
	{
		public DateTime lastAddDate;

		public int count;

		public int maxStock;

		public float cooldown;
	}

	private const string PREFIX = "stock.";

	private static Dictionary<string, StockItem> stock = new Dictionary<string, StockItem>();

	private static CultureInfo _cultureInfo;

	public static CultureInfo cultureInfo
	{
		get
		{
			if (_cultureInfo == null)
			{
				_cultureInfo = new CultureInfo("en-US");
			}
			return _cultureInfo;
		}
	}

	public static void RefreshStock()
	{
		foreach (string key in stock.Keys)
		{
			StockItem stockItem = stock[key];
			GetStockCount(key, stockItem.cooldown, stockItem.maxStock);
		}
	}

	public static void InitStockItem(string key, float cooldown, int max, int initialCount = -1)
	{
		if (initialCount == -1)
		{
			initialCount = max;
		}
		GetOrCreateStockItem(key, cooldown, max, initialCount);
	}

	public static void RemoveItem(string key)
	{
		if (stock.ContainsKey(key))
		{
			stock.Remove(key);
		}
		if (PlayerPrefs.HasKey("stock." + key))
		{
			PlayerPrefs.DeleteKey("stock." + key);
		}
	}

	public static bool HasItem(string key)
	{
		return stock.ContainsKey(key);
	}

	public static void UpdateStockItemWithValues(string key, float cooldown, int max, int count = -1)
	{
		StockItem orCreateStockItem = GetOrCreateStockItem(key, cooldown, max, count);
		orCreateStockItem.cooldown = cooldown;
		orCreateStockItem.maxStock = max;
		if (count != -1)
		{
			orCreateStockItem.count = count;
		}
	}

	public static int GetStockCount(string key, float cooldown, int max, int initialCount = -1)
	{
		if (initialCount == -1)
		{
			initialCount = max;
		}
		GetOrCreateStockItem(key, cooldown, max, initialCount);
		return GetStockCount(key);
	}

	public static int GetStockCount(string key)
	{
		StockItem stockItem = GetStockItem(key);
		if (stockItem == null)
		{
			return -1;
		}
		CountStock(stockItem);
		SaveStock(key, stockItem);
		return stockItem.count;
	}

	public static float GetNextItemCooldown(string key, float cooldown, int max, int count = -1)
	{
		if (count == -1)
		{
			count = max;
		}
		GetOrCreateStockItem(key, cooldown, max, count);
		return GetNextItemCooldown(key);
	}

	public static float GetNextItemCooldown(string key)
	{
		StockItem stockItem = GetStockItem(key);
		int stockCount = GetStockCount(key);
		if (stockCount > 0)
		{
			return 0f;
		}
		TimeSpan timeSpan = DateTime.Now - stockItem.lastAddDate;
		float cooldown = stockItem.cooldown;
		return cooldown - (float)timeSpan.TotalSeconds;
	}

	public static void IncrementStockCount(string key, bool allowOverflow = false)
	{
		StockItem stockItem = GetStockItem(key);
		stockItem.count++;
		if (!allowOverflow)
		{
			stockItem.count = Mathf.Min(stockItem.count, stockItem.maxStock);
		}
		SaveStock(key, stockItem);
	}

	public static void DecrementStockCount(string key)
	{
		StockItem stockItem = GetStockItem(key);
		stockItem.count--;
		if (stockItem.count < 0)
		{
			UnityEngine.Debug.LogError("You reduced this item: " + key + " under 0!!!");
		}
		SaveStock(key, stockItem);
	}

	public static void RefillStock(string key)
	{
		StockItem stockItem = GetStockItem(key);
		stockItem.count = stockItem.maxStock;
		stockItem.lastAddDate = DateTime.Now;
		SaveStock(key, stockItem);
	}

	private static StockItem GetOrCreateStockItem(string key, float cooldown, int max, int count)
	{
		if (!stock.ContainsKey(key))
		{
			stock[key] = CreateStockItem(key, cooldown, max, count);
		}
		return stock[key];
	}

	private static StockItem GetStockItem(string key)
	{
		if (!stock.ContainsKey(key))
		{
			UnityEngine.Debug.LogError($"Tried to get stock item {key}, but it wasn't avaible. Init it first");
			return null;
		}
		return stock[key];
	}

	private static void CountStock(StockItem item)
	{
		DateTime lastAddDate = item.lastAddDate;
		if (float.IsNaN(item.cooldown))
		{
			item.lastAddDate = lastAddDate;
			return;
		}
		while (lastAddDate.AddSeconds(item.cooldown) <= DateTime.Now && item.count < item.maxStock)
		{
			item.count = Mathf.Min(item.maxStock, item.count + 1);
			lastAddDate = lastAddDate.AddSeconds(item.cooldown);
		}
		if (item.count < item.maxStock)
		{
			item.lastAddDate = lastAddDate;
		}
		else
		{
			item.lastAddDate = DateTime.Now;
		}
	}

	private static StockItem CreateStockItem(string key, float cooldown, int max, int count)
	{
		if (PlayerPrefs.HasKey("stock." + key))
		{
			return FromString(PlayerPrefs.GetString("stock." + key));
		}
		StockItem stockItem = new StockItem();
		stockItem.lastAddDate = DateTime.Now;
		stockItem.maxStock = max;
		stockItem.cooldown = cooldown;
		stockItem.count = count;
		return stockItem;
	}

	private static void SaveStock(string key, StockItem item)
	{
		PlayerPrefs.SetString("stock." + key, ToString(item));
	}

	private static string ToString(StockItem item)
	{
		return item.maxStock.ToString() + ";" + item.count.ToString() + ";" + item.cooldown.ToString() + ";" + item.lastAddDate.ToString(cultureInfo);
	}

	private static StockItem FromString(string item)
	{
		StockItem stockItem = new StockItem();
		string[] array = item.Split(';');
		stockItem.maxStock = int.Parse(array[0]);
		stockItem.count = int.Parse(array[1]);
		stockItem.cooldown = float.Parse(array[2]);
		stockItem.lastAddDate = DateTime.Parse(array[3], cultureInfo);
		return stockItem;
	}
}
