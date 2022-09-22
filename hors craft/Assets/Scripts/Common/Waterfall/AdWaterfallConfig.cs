// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Waterfall.AdWaterfallConfig
using Common.MiniJSON;
using Common.Model;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

//namespace Common.Waterfall
//{
	/*public class AdWaterfallConfig
	{
		private AdWaterfallSettings waterfallSettings;

		private Settings modelSettings;

		private const int IV_SIZE = 16;

		private const string DECRYPT_PASS = "fwPZXRSiEMWCOpMONapilsqz0sc0G5H1";

		public AdWaterfallConfig(Settings modelSettings, AdWaterfallStepDefinition[] steps)
		{
			waterfallSettings = new AdWaterfallSettings(steps);
			this.modelSettings = modelSettings;
		}

		private string BytesToString(byte[] array)
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(BitConverter.ToString(array));
			stringBuilder.Append(" Count: " + array.Length);
			return stringBuilder.ToString();
		}

		public void Deserialize()
		{
			string text = modelSettings.GetString("waterfall.serialized.config", string.Empty);
			if (string.IsNullOrEmpty(text))
			{
				waterfallSettings.ReadSettingsFrom(modelSettings);
				return;
			}
			if (!text.StartsWith("{"))
			{
				try
				{
					UnityEngine.Debug.Log(text);
					byte[] array = Convert.FromBase64String(text);
					byte[] array2 = new byte[16];
					byte[] array3 = new byte[array.Length - 16];
					Array.Copy(array, array2, 16);
					Array.Copy(array, 16, array3, 0, array3.Length);
					text = Decrypt(array2, array3);
				}
				catch (Exception arg)
				{
					UnityEngine.Debug.LogError("Waterfall decryption exception: " + arg);
				}
			}
			if (!string.IsNullOrEmpty(text))
			{
				object obj = null;
				try
				{
					obj = Json.Deserialize(text);
				}
				catch (Exception arg2)
				{
					UnityEngine.Debug.LogError("Waterfall deserialization exception: " + arg2);
				}
				if (obj == null)
				{
					UnityEngine.Debug.LogError("Server sent invalid waterfall config! " + text);
				}
				else
				{
					waterfallSettings.ReadSettingsFrom(obj);
				}
			}
		}

		public void Execute()
		{
			waterfallSettings.Execute();
		}

		public bool IsVerbose()
		{
			return waterfallSettings.IsVerbose();
		}

		public bool IsRewardedWithInterstitial()
		{
			return waterfallSettings.IsRewardedWithInterstitial();
		}

		private string Decrypt(byte[] ivBytes, byte[] cipherBytes)
		{
			Aes aes = Aes.Create();
			aes.Mode = CipherMode.CBC;
			byte[] array = aes.Key = Encoding.ASCII.GetBytes("fwPZXRSiEMWCOpMONapilsqz0sc0G5H1");
			aes.IV = ivBytes;
			MemoryStream memoryStream = new MemoryStream();
			ICryptoTransform transform = aes.CreateDecryptor();
			CryptoStream cryptoStream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write);
			string empty = string.Empty;
			try
			{
				cryptoStream.Write(cipherBytes, 0, cipherBytes.Length);
				cryptoStream.FlushFinalBlock();
				byte[] array2 = memoryStream.ToArray();
				return Encoding.UTF8.GetString(array2, 0, array2.Length);
			}
			finally
			{
				memoryStream.Close();
				cryptoStream.Close();
			}
		}
	}
}*/
