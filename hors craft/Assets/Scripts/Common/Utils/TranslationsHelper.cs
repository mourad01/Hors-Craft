// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.TranslationsHelper
using Common.Managers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Utils
{
	public static class TranslationsHelper
	{
		public static List<string> GetAllTranslationsWithKey(Func<int, string> key)
		{
			Dictionary<int, string> allTranslationsAndIndicesWithKey = GetAllTranslationsAndIndicesWithKey(key);
			return (from kvp in allTranslationsAndIndicesWithKey
				select kvp.Value).ToList();
		}

		public static Dictionary<int, string> GetAllTranslationsAndIndicesWithKey(Func<int, string> key)
		{
			Dictionary<int, string> dictionary = new Dictionary<int, string>();
			int num = 0;
			string text = Manager.Get<TranslationsManager>().GetText(key(num), string.Empty);
			while (!string.IsNullOrEmpty(text))
			{
				dictionary.Add(num, text);
				num++;
				text = Manager.Get<TranslationsManager>().GetText(key(num), string.Empty);
			}
			return dictionary;
		}
	}
}
