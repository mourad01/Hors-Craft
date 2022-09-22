// DecompilerFi decompiler from Assembly-CSharp.dll class: TsgCommon.Crosspromo.CrosspromoIconCreator
using OldMoatGames;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace TsgCommon.Crosspromo
{
	internal class CrosspromoIconCreator
	{
		private const string PREFAB_PATH_IN_RESOURCES = "Crosspromo/Prefab";

		private const string MULTI_PREFAB_PATH_IN_RESOURCES = "Crosspromo/TitleCrosspromoPrefab";

		private GameObject spawnedIconGO;

		private static GameObject prefab;

		private static GameObject Prefab
		{
			get
			{
				if (prefab == null)
				{
					prefab = GetPrefab();
				}
				return prefab;
			}
		}

		public CrosspromoIconCreator(Canvas canvas, Vector2 percentPosOnScreen, string headerText, Texture2D texture, CrosspromoClickCallback onClick, bool xButtonEnabled, CrosspromoClickCallback onClickX, int animator, string gifUrl = "")
		{
			if (Spawn(canvas, percentPosOnScreen, headerText, onClick, xButtonEnabled, onClickX, animator))
			{
				if (!gifUrl.IsNullOrEmpty())
				{
					SetGif(gifUrl, xButtonEnabled);
				}
				else
				{
					SetTexture(texture);
				}
			}
		}

		private static GameObject GetPrefab()
		{
			GameObject gameObject = Resources.Load<GameObject>("Crosspromo/Prefab");
			if (gameObject == null)
			{
				UnityEngine.Debug.LogWarning("Couldn't find crosspromo icon prefab!");
			}
			return gameObject;
		}

		public void Dispose()
		{
			if (spawnedIconGO != null)
			{
				UnityEngine.Object.Destroy(spawnedIconGO);
			}
		}

		private bool Spawn(Canvas canvas, Vector2 percentPosOnScreen, string headerText, CrosspromoClickCallback onClick, bool xButtonEnabled, CrosspromoClickCallback onClickX, int animator)
		{
			if (Prefab == null || MonoBehaviourSingleton<CrosspromoController>.get.containerSprite == null)
			{
				return false;
			}
			spawnedIconGO = UnityEngine.Object.Instantiate(Prefab, canvas.transform);
			spawnedIconGO.GetComponent<CrosspromoIconDragger>().onClick = onClick;
			spawnedIconGO.GetComponent<CrosspromoIconDragger>().SetAnimator(animator);
			spawnedIconGO.GetComponentInChildren<Text>().text = headerText;
			Button componentInChildren = spawnedIconGO.GetComponentInChildren<Button>();
			if (componentInChildren != null)
			{
				componentInChildren.gameObject.SetActive(xButtonEnabled);
				componentInChildren.onClick.AddListener(delegate
				{
					onClickX();
				});
			}
			Vector2 vector = Vector2.one * Mathf.Max(Screen.width, Screen.height) * MonoBehaviourSingleton<CrosspromoController>.get.iconPercentSize / canvas.scaleFactor;
			RectTransform component = spawnedIconGO.GetComponent<RectTransform>();
			RectTransform rectTransform = component;
			float x = vector.x;
			Vector2 sizeDelta = component.sizeDelta;
			float x2 = x / sizeDelta.x;
			float y = vector.y;
			Vector2 sizeDelta2 = component.sizeDelta;
			rectTransform.localScale = new Vector2(x2, y / sizeDelta2.y);
			if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
			{
				UIUtils.FitOnScreen(component, percentPosOnScreen);
			}
			else
			{
				UIUtils.FitOnScreen(component, canvas.GetComponent<RectTransform>(), percentPosOnScreen);
			}
			return true;
		}

		private void SetTexture(Texture2D texture)
		{
			if (spawnedIconGO != null)
			{
				Transform transform = spawnedIconGO.transform.FindChildRecursively("Icon");
				if (transform == null)
				{
					UnityEngine.Debug.LogWarning("Couldn't find 'Icon' go inside crosspromo prefab!");
					return;
				}
				Image componentInChildren = transform.GetComponentInChildren<Image>();
				Rect rect = new Rect(0f, 0f, texture.width, texture.height);
				componentInChildren.sprite = Sprite.Create(texture, rect, Vector2.zero, 100f, 0u, SpriteMeshType.FullRect);
			}
		}

		private void SetGif(string gifUrl, bool xButtonEnabled)
		{
			AnimatedGifPlayer gifPlayer = spawnedIconGO.AddComponent<AnimatedGifPlayer>();
			gifPlayer.FileName = gifUrl;
			gifPlayer.UseThreadedDecoder = true;
			IEnumerator enumerator = spawnedIconGO.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					transform.gameObject.SetActive(value: false);
				}
			}
			finally
			{
				IDisposable disposable;
				if ((disposable = (enumerator as IDisposable)) != null)
				{
					disposable.Dispose();
				}
			}
			gifPlayer.OnReady.AddListener(delegate
			{
				if (spawnedIconGO != null)
				{
					Transform transform2 = spawnedIconGO.transform.FindChildRecursively("Icon");
					if (transform2 != null)
					{
						transform2.gameObject.SetActive(value: false);
					}
					Transform transform3 = spawnedIconGO.transform.FindChildRecursively("GifIcon");
					if (transform3 == null)
					{
						UnityEngine.Debug.LogWarning("Couldn't find 'GifIcon' go inside crosspromo prefab!");
					}
					else
					{
						transform3.gameObject.SetActive(value: true);
						RawImage componentInChildren = transform3.GetComponentInChildren<RawImage>();
						gifPlayer.TargetComponent = componentInChildren;
						gifPlayer.OverrideTimeScale = true;
						IEnumerator enumerator2 = spawnedIconGO.transform.GetEnumerator();
						try
						{
							while (enumerator2.MoveNext())
							{
								Transform transform4 = (Transform)enumerator2.Current;
								transform4.gameObject.SetActive(value: true);
							}
						}
						finally
						{
							IDisposable disposable2;
							if ((disposable2 = (enumerator2 as IDisposable)) != null)
							{
								disposable2.Dispose();
							}
						}
						Button componentInChildren2 = spawnedIconGO.GetComponentInChildren<Button>();
						componentInChildren2.gameObject.SetActive(xButtonEnabled);
					}
				}
			});
			gifPlayer.OnLoadError.AddListener(delegate
			{
				UnityEngine.Debug.LogError("ERROR DECODING CROSSPROMO GIF!");
			});
			gifPlayer.OnLoadError.AddListener(Dispose);
			gifPlayer.Init();
		}
	}
}
