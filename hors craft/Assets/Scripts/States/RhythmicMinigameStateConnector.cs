// DecompilerFi decompiler from Assembly-CSharp.dll class: States.RhythmicMinigameStateConnector
using com.ootii.Cameras;
using Common.Managers;
using Common.Managers.States.UI;
using Gameplay.RhythmicMinigame;
using GameUI;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace States
{
	public class RhythmicMinigameStateConnector : UIConnector, RhythmReactor
	{
		public delegate void OnClick();

		public SimpleRepeatButton tapButton;

		public Button returnButton;

		public Animator speaker;

		public Image speakerImage;

		public GameObject bitPrefab;

		public Text getReadyText;

		public GameObject helpInfoPanel;

		public Animator circleBorder;

		public TamePanelController tamePanel;

		public Camera cameraPlayerPreview;

		public OnClick onTapButton;

		public OnClick onReturnButton;

		public Transform background;

		public Transform hpParent;

		private int _hp;

		private Animator getReadyAnimator;

		private float baseMobCameraFov;

		private float baseMainCameraFov;

		private IEnumerator lastFovBumpCoroutine;

		public int hp
		{
			get
			{
				return _hp;
			}
			set
			{
				_hp = value;
				for (int i = 0; i < hpParent.childCount; i++)
				{
					hpParent.GetChild(i).gameObject.SetActive(i < _hp);
				}
			}
		}

		public bool getReadyVisible
		{
			get
			{
				return getReadyText.gameObject.activeSelf;
			}
			set
			{
				getReadyText.gameObject.SetActive(value);
			}
		}

		public bool helpInfoVisible
		{
			get
			{
				return helpInfoPanel.activeSelf;
			}
			set
			{
				helpInfoPanel.SetActive(value);
			}
		}

		public void SetBackGround(Texture2D newImage, Vector3 offset)
		{
			if (!(background == null))
			{
				background.GetComponent<Renderer>().material.SetTexture("_MainTex", newImage);
				Vector3 localPosition = background.transform.localPosition;
				localPosition += offset;
				background.transform.localPosition = localPosition;
			}
		}

		private void Awake()
		{
			_hp = hpParent.childCount;
			bitPrefab.SetActive(value: false);
			tapButton.onFingerDown = delegate
			{
				onTapButton();
			};
			returnButton.onClick.AddListener(delegate
			{
				onReturnButton();
			});
			getReadyAnimator = getReadyText.GetComponent<Animator>();
			baseMobCameraFov = cameraPlayerPreview.fieldOfView;
			baseMainCameraFov = CameraController.instance.MainCamera.fieldOfView;
		}

		public void Update()
		{
		}

		public void OnSpawnBlob(int beatType)
		{
			speaker.SetTrigger("Bump");
			DoFOVBump();
		}

		public void OnCorrectHit(int beatType)
		{
			circleBorder.SetTrigger("correct");
			DoFOVBump();
		}

		public void OnIncorrectHit(int beatType)
		{
			circleBorder.SetTrigger("wrong");
		}

		public void OnBlobFall(int beatType)
		{
			OnIncorrectHit(beatType);
		}

		public void OnCorrectSequence()
		{
		}

		public void OnBeat(int beatIndex)
		{
			if (getReadyText.gameObject.activeInHierarchy)
			{
				getReadyAnimator.SetTrigger("Bump");
			}
		}

		public void OnSuccessScene()
		{
			IEnumerator enumerator = base.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					if (transform != cameraPlayerPreview.transform && transform.name != "Get Ready")
					{
						transform.gameObject.SetActive(value: false);
					}
					else
					{
						transform.gameObject.SetActive(value: true);
					}
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
		}

		public void OnFailureScene()
		{
			IEnumerator enumerator = base.transform.GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					Transform transform = (Transform)enumerator.Current;
					if (transform != cameraPlayerPreview.transform && transform.name != "Get Ready")
					{
						transform.gameObject.SetActive(value: false);
					}
					else
					{
						transform.gameObject.SetActive(value: true);
					}
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
		}

		private void DoFOVBump()
		{
			if (lastFovBumpCoroutine != null)
			{
				StopCoroutine(lastFovBumpCoroutine);
			}
			lastFovBumpCoroutine = FOVBump(Time.realtimeSinceStartup);
			StartCoroutine(lastFovBumpCoroutine);
		}

		private void OnDestroy()
		{
			StopAllCoroutines();
		}

		private IEnumerator FOVBump(float startTime)
		{
			do
			{
				float factor = (Time.realtimeSinceStartup - startTime) / 0.2f;
				cameraPlayerPreview.fieldOfView = baseMobCameraFov - 1f + factor * 1f;
				CameraController.instance.MainCamera.fieldOfView = baseMainCameraFov - 1f + factor * 1f;
				yield return new WaitForEndOfFrame();
			}
			while (Time.realtimeSinceStartup < startTime + 0.2f);
		}

		public void SetTempo(float t)
		{
		}

		public void SetPosition(Camera camera)
		{
		}

		public void OnFinish()
		{
		}

		public void OnBlobInShootZone(int beatType)
		{
		}

		public StatsManager.MinigameType GetGameType()
		{
			return StatsManager.MinigameType.INVALID;
		}
	}
}
