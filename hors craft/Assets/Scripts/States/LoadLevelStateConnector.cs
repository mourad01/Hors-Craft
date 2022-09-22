// DecompilerFi decompiler from Assembly-CSharp.dll class: States.LoadLevelStateConnector
using Common.Managers;
using Common.Managers.States.UI;
using GameUI;
using Uniblocks;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class LoadLevelStateConnector : UIConnector
	{
		public Image progressBar;

		public Text subtitle;

		public Image backgroundImage;

		public Text titleText;

		public GameObject cubePrefab;

		private Sprite sprite;

		private void Awake()
		{
			if (subtitle != null)
			{
				subtitle.gameObject.SetActive(value: false);
			}
			sprite = Resources.Load<Sprite>("TitleScreen&LoadingResources/_Titlescreen");
			UpdateLoadProgress(0f);
			SetBackgroundColor();
		}

		private void SetBackgroundColor()
		{
			ColorManager colorManager = Manager.Get<ColorManager>();
			if (sprite != null)
			{
				backgroundImage.color = Color.white;
				backgroundImage.sprite = sprite;
			}
			else
			{
				backgroundImage.color = colorManager.GetColorForCategory(ColorManager.ColorCategory.MAIN_COLOR);
			}
		}

		public void InitCube(Voxel loadingBlock)
		{
			LoadingCube instance = LoadingCube.instance;
			if (cubePrefab != null)
			{
				if (instance != null)
				{
					instance.gameObject.SetActive(value: true);
					instance.InitCube(loadingBlock);
				}
				else
				{
					GameObject gameObject = Object.Instantiate(cubePrefab, Vector3.zero, Quaternion.identity);
					LoadingCube component = gameObject.GetComponent<LoadingCube>();
					component.InitCube(loadingBlock);
				}
			}
		}

		public void DestroyCube()
		{
			LoadingCube instance = LoadingCube.instance;
			if (instance.gameObject != null)
			{
				UnityEngine.Object.Destroy(instance.gameObject);
			}
		}

		public void Init(string subtitleText)
		{
			if (subtitle != null)
			{
				subtitle.gameObject.SetActive(value: true);
				subtitle.text = subtitleText;
			}
			UpdateLoadProgress(0f);
		}

		public void UpdateLoadProgress(float value)
		{
			if (progressBar != null)
			{
				progressBar.fillAmount = value;
			}
		}

		private void OnDestroy()
		{
			if (sprite != null && backgroundImage.sprite == sprite)
			{
				backgroundImage.sprite = null;
				Resources.UnloadAsset(sprite);
				sprite = null;
			}
		}
	}
}
