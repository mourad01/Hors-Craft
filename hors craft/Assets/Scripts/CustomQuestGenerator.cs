// DecompilerFi decompiler from Assembly-CSharp.dll class: CustomQuestGenerator
using System;
using System.Collections.Generic;
using UnityEngine;

public class CustomQuestGenerator : MonoBehaviour
{
	[Serializable]
	public class QuestValueGeneratorPair
	{
		[SerializeField]
		protected QuestType questType;

		[Header("Requirements:")]
		[SerializeField]
		protected int recBaseValue;

		[SerializeField]
		protected int recMultiplierValue = 1;

		[Header("Price:")]
		[SerializeField]
		protected int priceBaseValue;

		[SerializeField]
		protected int priceMultiplierValue = 1;

		public QuestType QuestType => questType;

		internal WorldsQuests.QuestValueGenerator GetGenerator()
		{
			return new QuestValueGeneratorCustom(recMultiplierValue, recBaseValue, priceMultiplierValue, priceBaseValue);
		}
	}

	public List<QuestValueGeneratorPair> generatorValues = new List<QuestValueGeneratorPair>();

	public Dictionary<QuestType, WorldsQuests.QuestValueGenerator> GenerateFromFakeModel()
	{
		Dictionary<QuestType, WorldsQuests.QuestValueGenerator> dictionary = new Dictionary<QuestType, WorldsQuests.QuestValueGenerator>();
		foreach (QuestValueGeneratorPair generatorValue in generatorValues)
		{
			dictionary.Add(generatorValue.QuestType, generatorValue.GetGenerator());
		}
		return dictionary;
	}
}
