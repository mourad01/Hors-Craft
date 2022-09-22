// DecompilerFi decompiler from Assembly-CSharp.dll class: OldMoatGames.AnimatedGifPlayer
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace OldMoatGames
{
	[AddComponentMenu("Miscellaneous/Animated GIF Player")]
	public class AnimatedGifPlayer : MonoBehaviour
	{
		public bool Loop = true;

		public bool AutoPlay = true;

		public string FileName = string.Empty;

		public GifPath Path;

		public bool CacheFrames;

		public bool BufferAllFrames;

		public bool UseThreadedDecoder = true;

		public bool OverrideTimeScale;

		public float TimeScale = 1f;

		public Texture2D GifTexture;

		public UnityEvent OnReady = new UnityEvent();

		public UnityEvent OnLoadError = new UnityEvent();

		private GifDecoder _gifDecoder;

		private bool _hasFirstFrameBeenShown;

		[SerializeField]
		private Component _targetComponent;

		[SerializeField]
		private int _targetMaterial;

		private bool _cacheFrames;

		private bool _bufferAllFrames;

		private bool _useThreadedDecoder;

		private float _secondsTillNextFrame;

		private List<GifDecoder.GifFrame> _cachedFrames;

		private Thread _decodeThread;

		private readonly EventWaitHandle _wh = new AutoResetEvent(initialState: false);

		private bool _threadIsCanceled;

		private bool _frameIsReady;

		private readonly object _locker = new object();

		private float _editorPreviousUpdateTime;

		public int Width => (_gifDecoder != null) ? _gifDecoder.GetFrameWidth() : 0;

		public int Height => (_gifDecoder != null) ? _gifDecoder.GetFrameHeight() : 0;

		public Component TargetComponent
		{
			get
			{
				return _targetComponent;
			}
			set
			{
				_targetComponent = value;
			}
		}

		public int TargetMaterialNumber
		{
			get
			{
				return _targetMaterial;
			}
			set
			{
				_targetMaterial = value;
			}
		}

		public GifPlayerState State
		{
			get;
			private set;
		}

		private GifDecoder.GifFrame CurrentFrame
		{
			get;
			set;
		}

		private int CurrentFrameNumber
		{
			get;
			set;
		}

		private void Awake()
		{
		}

		public void Update()
		{
			CheckFrameChange();
		}

		private void OnApplicationQuit()
		{
			EndDecodeThread();
		}

		public void Clear()
		{
			Pause();
			CurrentFrameNumber = 0;
			_hasFirstFrameBeenShown = false;
			_frameIsReady = false;
			EndDecodeThread();
		}

		public void Init()
		{
			_cacheFrames = CacheFrames;
			_bufferAllFrames = BufferAllFrames;
			_useThreadedDecoder = UseThreadedDecoder;
			if (_bufferAllFrames && !_cacheFrames)
			{
				_bufferAllFrames = false;
			}
			if (_cacheFrames)
			{
				_cachedFrames = new List<GifDecoder.GifFrame>();
			}
			_targetComponent = ((!(_targetComponent == null)) ? _targetComponent : GetTargetComponent());
			_gifDecoder = new GifDecoder();
			CurrentFrameNumber = 0;
			_hasFirstFrameBeenShown = false;
			_frameIsReady = false;
			State = GifPlayerState.Disabled;
			StartDecodeThread();
			if (FileName.Length > 0)
			{
				StartCoroutine(Load());
			}
		}

		public void Play()
		{
			if (State != GifPlayerState.Stopped)
			{
				UnityEngine.Debug.LogWarning("Can't play GIF playback. State is: " + State);
			}
			else
			{
				State = GifPlayerState.Playing;
			}
		}

		public void Pause()
		{
			if (State != GifPlayerState.Playing)
			{
				UnityEngine.Debug.LogWarning("Can't pause GIF is not playing. State is: " + State);
			}
			else
			{
				State = GifPlayerState.Stopped;
			}
		}

		public int GetNumberOfFrames()
		{
			return (_gifDecoder != null) ? _gifDecoder.GetFrameCount() : 0;
		}

		private IEnumerator Load()
		{
			if (FileName.Length == 0)
			{
				UnityEngine.Debug.LogWarning("File name not set");
				yield break;
			}
			State = GifPlayerState.Loading;
			string path;
			if (FileName.Substring(0, 4) == "http")
			{
				path = FileName;
			}
			else
			{
				string path2;
				switch (Path)
				{
				case GifPath.StreamingAssetsPath:
					path2 = Application.streamingAssetsPath;
					break;
				case GifPath.PersistentDataPath:
					path2 = Application.persistentDataPath;
					break;
				case GifPath.TemporaryCachePath:
					path2 = Application.temporaryCachePath;
					break;
				default:
					path2 = Application.streamingAssetsPath;
					break;
				}
				path = System.IO.Path.Combine(path2, FileName);
			}
			WWW www = new WWW(path.Replace("#", "%23"));
			try
			{
				yield return www;
				if (!string.IsNullOrEmpty(www.error))
				{
					UnityEngine.Debug.LogWarning("File load error.\n" + www.error + "\nPath:" + WWW.EscapeURL(path, Encoding.Default));
					State = GifPlayerState.Error;
				}
				else
				{
					lock (_locker)
					{
						if (_gifDecoder.Read(new MemoryStream(www.bytes)) == GifDecoder.Status.StatusOk)
						{
							State = GifPlayerState.PreProcessing;
							CreateTargetTexture();
							StartDecoder();
						}
						else
						{
							UnityEngine.Debug.LogWarning("Error loading gif");
							State = GifPlayerState.Error;
							if (OnLoadError != null)
							{
								OnLoadError.Invoke();
							}
						}
					}
				}
			}
			finally
			{
				//base._003C_003E__Finally0();
			}
		}

		private void CreateTargetTexture()
		{
			if (GifTexture != null && _gifDecoder != null && GifTexture.width == _gifDecoder.GetFrameWidth() && GifTexture.height == _gifDecoder.GetFrameHeight())
			{
				return;
			}
			if (_gifDecoder == null || _gifDecoder.GetFrameWidth() == 0 || _gifDecoder.GetFrameWidth() == 0)
			{
				GifTexture = Texture2D.blackTexture;
				return;
			}
			if (GifTexture != null && GifTexture.hideFlags == HideFlags.HideAndDontSave)
			{
				UnityEngine.Object.Destroy(GifTexture);
			}
			GifTexture = CreateTexture(_gifDecoder.GetFrameWidth(), _gifDecoder.GetFrameHeight());
			GifTexture.hideFlags = HideFlags.HideAndDontSave;
		}

		private void SetTexture()
		{
			if (_targetComponent == null)
			{
				return;
			}
			if (_targetComponent is SpriteRenderer)
			{
				SpriteRenderer spriteRenderer = (SpriteRenderer)_targetComponent;
				Vector2 size = spriteRenderer.size;
				Sprite sprite = Sprite.Create(GifTexture, new Rect(0f, 0f, GifTexture.width, GifTexture.height), new Vector2(0.5f, 0.5f), 100f, 0u, SpriteMeshType.FullRect);
				sprite.name = "Gif Player Sprite";
				sprite.hideFlags = HideFlags.HideAndDontSave;
				spriteRenderer.sprite = sprite;
				spriteRenderer.size = size;
			}
			else if (_targetComponent is Renderer)
			{
				Renderer renderer = (Renderer)_targetComponent;
				if (!(renderer.sharedMaterial == null))
				{
					if (renderer.sharedMaterials.Length > 0 && renderer.sharedMaterials.Length > _targetMaterial)
					{
						renderer.sharedMaterials[_targetMaterial].mainTexture = GifTexture;
					}
					else
					{
						renderer.sharedMaterial.mainTexture = GifTexture;
					}
				}
			}
			else if (_targetComponent is RawImage)
			{
				RawImage rawImage = (RawImage)_targetComponent;
				rawImage.texture = GifTexture;
			}
		}

		private Component GetTargetComponent()
		{
			Component[] components = GetComponents<Component>();
			return components.FirstOrDefault((Component component) => component is Renderer || component is RawImage);
		}

		private void SetTargetTexture()
		{
			if (GifTexture == null || GifTexture.width != _gifDecoder.GetFrameWidth() || GifTexture.height != _gifDecoder.GetFrameWidth())
			{
				GifTexture = CreateTexture(_gifDecoder.GetFrameWidth(), _gifDecoder.GetFrameHeight());
			}
			GifTexture.hideFlags = HideFlags.HideAndDontSave;
			if (TargetComponent == null)
			{
				UnityEngine.Debug.LogError("is null");
				return;
			}
			if (TargetComponent is MeshRenderer)
			{
				Renderer renderer = (Renderer)TargetComponent;
				if (renderer.sharedMaterial == null)
				{
					return;
				}
				if (renderer.sharedMaterials.Length > 0 && renderer.sharedMaterials.Length > _targetMaterial)
				{
					renderer.sharedMaterials[_targetMaterial].mainTexture = GifTexture;
				}
				else
				{
					renderer.sharedMaterial.mainTexture = GifTexture;
				}
			}
			if (TargetComponent is SpriteRenderer)
			{
				SpriteRenderer spriteRenderer = (SpriteRenderer)TargetComponent;
				Sprite sprite = Sprite.Create(GifTexture, new Rect(0f, 0f, GifTexture.width, GifTexture.height), new Vector2(0.5f, 0.5f));
				sprite.name = "Gif Player Sprite";
				sprite.hideFlags = HideFlags.HideAndDontSave;
				spriteRenderer.sprite = sprite;
			}
			if (TargetComponent is RawImage)
			{
				UnityEngine.Debug.LogError("is image");
				RawImage rawImage = (RawImage)TargetComponent;
				rawImage.texture = GifTexture;
			}
		}

		private static Texture2D CreateTexture(int width, int height)
		{
			return new Texture2D(width, height, TextureFormat.RGBA32, mipChain: false);
		}

		private void BufferFrames()
		{
			if (_useThreadedDecoder)
			{
				_wh.Set();
			}
			else
			{
				lock (_locker)
				{
					while (true)
					{
						_gifDecoder.ReadNextFrame(loop: false);
						if (_gifDecoder.AllFramesRead)
						{
							break;
						}
						GifDecoder.GifFrame currentFrame = _gifDecoder.GetCurrentFrame();
						AddFrameToCache(currentFrame);
					}
					_frameIsReady = true;
				}
			}
		}

		private void AddFrameToCache(GifDecoder.GifFrame frame)
		{
			byte[] array = new byte[frame.Image.Length];
			Buffer.BlockCopy(frame.Image, 0, array, 0, frame.Image.Length);
			frame.Image = array;
			lock (_cachedFrames)
			{
				_cachedFrames.Add(frame);
			}
		}

		private void StartDecoder()
		{
			if (_bufferAllFrames)
			{
				BufferFrames();
			}
			else
			{
				StartReadFrame();
			}
			State = GifPlayerState.Stopped;
			if (OnReady != null)
			{
				OnReady.Invoke();
			}
			if (AutoPlay)
			{
				Play();
			}
		}

		private void SetNextFrameTime()
		{
			_secondsTillNextFrame = CurrentFrame.Delay;
		}

		private void UpdateFrameTime()
		{
			if (State != GifPlayerState.Playing)
			{
				return;
			}
			if (!Application.isPlaying || OverrideTimeScale)
			{
				if (OverrideTimeScale)
				{
					_secondsTillNextFrame -= (Time.realtimeSinceStartup - _editorPreviousUpdateTime) * TimeScale;
				}
				else
				{
					_secondsTillNextFrame -= Time.realtimeSinceStartup - _editorPreviousUpdateTime;
				}
				_editorPreviousUpdateTime = Time.realtimeSinceStartup;
			}
			else
			{
				_secondsTillNextFrame -= Time.deltaTime;
			}
		}

		private void UpdateFrame()
		{
			if (_gifDecoder.NumberOfFrames > 0 && _gifDecoder.NumberOfFrames == CurrentFrameNumber)
			{
				CurrentFrameNumber = 0;
				if (!Loop)
				{
					Pause();
					return;
				}
			}
			if (_cacheFrames)
			{
				lock (_cachedFrames)
				{
					CurrentFrame = ((_cachedFrames.Count <= CurrentFrameNumber) ? _gifDecoder.GetCurrentFrame() : _cachedFrames[CurrentFrameNumber]);
				}
				if (!_gifDecoder.AllFramesRead)
				{
					StartReadFrame();
				}
			}
			else
			{
				CurrentFrame = _gifDecoder.GetCurrentFrame();
				StartReadFrame();
			}
			UpdateTexture();
			SetNextFrameTime();
			CurrentFrameNumber++;
		}

		private void CheckFrameChange()
		{
			if ((State != GifPlayerState.Playing && _hasFirstFrameBeenShown) || !_frameIsReady)
			{
				return;
			}
			if (!_hasFirstFrameBeenShown)
			{
				SetTexture();
				lock (_locker)
				{
					UpdateFrame();
				}
				_hasFirstFrameBeenShown = true;
				return;
			}
			UpdateFrameTime();
			if (!(_secondsTillNextFrame > 0f))
			{
				lock (_locker)
				{
					UpdateFrame();
				}
			}
		}

		private void UpdateTexture()
		{
			GifTexture.LoadRawTextureData(CurrentFrame.Image);
			GifTexture.Apply();
		}

		private void StartReadFrame()
		{
			_frameIsReady = false;
			if (_useThreadedDecoder)
			{
				_wh.Set();
			}
			else if (!_cacheFrames || !_gifDecoder.AllFramesRead)
			{
				_gifDecoder.ReadNextFrame(!_cacheFrames);
				if (_cacheFrames && !_gifDecoder.AllFramesRead)
				{
					AddFrameToCache(_gifDecoder.GetCurrentFrame());
				}
				_frameIsReady = true;
			}
		}

		private void StartDecodeThread()
		{
			if (_useThreadedDecoder && (_decodeThread == null || !_decodeThread.IsAlive))
			{
				_threadIsCanceled = false;
				_decodeThread = new Thread((ThreadStart)delegate
				{
					FrameDataThread(!_cacheFrames, _bufferAllFrames);
				});
				_decodeThread.Name = "gifDecoder" + _decodeThread.ManagedThreadId;
				_decodeThread.IsBackground = true;
				_decodeThread.Start();
			}
		}

		private void EndDecodeThread()
		{
			if (!_threadIsCanceled)
			{
				_threadIsCanceled = true;
				_wh.Set();
			}
		}

		private void FrameDataThread(bool loopDecoder, bool readAllFrames)
		{
			_wh.WaitOne();
			while (!_threadIsCanceled)
			{
				lock (_locker)
				{
					_gifDecoder.ReadNextFrame(loopDecoder);
					if (_cacheFrames && _gifDecoder.AllFramesRead)
					{
						_frameIsReady = true;
					}
					else
					{
						if (_cacheFrames)
						{
							AddFrameToCache(_gifDecoder.GetCurrentFrame());
						}
						if (!readAllFrames)
						{
							_frameIsReady = true;
							goto IL_00ae;
						}
						if (!_gifDecoder.AllFramesRead)
						{
							continue;
						}
						_frameIsReady = true;
					}
				}
				break;
				IL_00ae:
				_wh.WaitOne();
			}
			_threadIsCanceled = true;
			_wh.Close();
		}
	}
}
