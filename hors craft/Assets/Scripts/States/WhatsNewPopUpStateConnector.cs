// DecompilerFi decompiler from Assembly-CSharp.dll class: States.WhatsNewPopUpStateConnector
using Common.Managers;
using Common.Managers.States.UI;
using Gameplay;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class WhatsNewPopUpStateConnector : UIConnector
	{
		[Serializable]
		public struct Feature
		{
			public FeatureType type;

			public Sprite icon;
		}

		public enum FeatureType
		{
			ACHIEVEMENTS,
			CRAFTING,
			FISHING,
			IMPROVE,
			MODE,
			MULTIPLAYER,
			NEWWORLDS,
			PETS,
			CLOTHES
		}

		public delegate void OnClick();

		public Feature[] featuresConfig;

		public GameObject featureElementPrefab;

		public Transform newFeaturesContent;

		public Button returnButton;

		public OnClick onReturnButtonClicked;

		private LayoutElement featureListLayoutElement;

		private float featureListMinHeight = 100f;

		private float featureListMaxHeight = 300f;

		private const string TRANSLATION_KEY = "menu.whatsnew.feature.";

		private int featuresCount;

		private ModelManager modelManager;

		private void Awake()
		{
			returnButton.onClick.AddListener(delegate
			{
				if (onReturnButtonClicked != null)
				{
					onReturnButtonClicked();
				}
			});
			modelManager = Manager.Get<ModelManager>();
			featuresCount = modelManager.whatsNewSettings.GetNewFeaturesCount();
			featureListLayoutElement = GetComponentInChildren<ScrollRect>().GetComponent<LayoutElement>();
			for (int i = 1; i <= featuresCount; i++)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(featureElementPrefab, newFeaturesContent);
				string newFeatureIconType = modelManager.whatsNewSettings.GetNewFeatureIconType(i);
				FeatureType featureType = (FeatureType)Enum.Parse(typeof(FeatureType), newFeatureIconType);
				for (int j = 0; j < featuresConfig.Length; j++)
				{
					if (featuresConfig[j].type == featureType)
					{
						gameObject.GetComponent<FeatureElement>().Init(string.Empty, "menu.whatsnew.feature." + Application.version + "." + i, featuresConfig[j].icon);
						break;
					}
				}
			}
			UpdateFeatureList();
		}

		public void UpdateFeatureList()
		{
			if (newFeaturesContent.childCount > 1)
			{
				featureListLayoutElement.minHeight = featureListMaxHeight;
			}
			else
			{
				featureListLayoutElement.minHeight = featureListMinHeight;
			}
		}
	}
}
