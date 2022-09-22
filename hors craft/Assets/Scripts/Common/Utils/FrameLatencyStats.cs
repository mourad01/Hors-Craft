// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.FrameLatencyStats
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

namespace Common.Utils
{
	public class FrameLatencyStats : MonoBehaviourSingleton<FrameLatencyStats>
	{
		public enum Counter
		{
			INITIALIZATIONS = 0,
			PHYSICS = 1,
			UPDATE = 2,
			RENDER_WHOLE = 3,
			ON_GUI = 4,
			FRAME = 5,
			CULLING = 7,
			RENDER = 8,
			POST_PROCESS = 9
		}

		public class MyStopwatch : Stopwatch
		{
			public bool isActive;

			public double averageDuration
			{
				get;
				private set;
			}

			public double lastDuration
			{
				get;
				private set;
			}

			public double currentDuration => base.Elapsed.TotalMilliseconds;

			public new void Start()
			{
				if (isActive)
				{
					base.Start();
				}
			}

			public new void Restart()
			{
				if (isActive)
				{
					base.Stop();
					Reset();
					base.Start();
				}
			}

			public new void Stop()
			{
				if (isActive)
				{
					base.Stop();
				}
			}

			public new void Reset()
			{
				if (isActive)
				{
					lastDuration = base.Elapsed.TotalMilliseconds;
					averageDuration += (base.Elapsed.TotalMilliseconds - averageDuration) * 0.05000000074505806;
					base.Reset();
				}
			}

			public void StopAndReset()
			{
				if (isActive)
				{
					Stop();
					Reset();
				}
			}
		}

		private Dictionary<int, MyStopwatch> stopwatchDict;

		private bool wasFixedUpdateTriggered;

		private bool wasOnGUITriggered;

		private WaitForEndOfFrame wait = new WaitForEndOfFrame();

		public void SetActiveCounters(List<Counter> counters)
		{
			IEnumerator enumerator = Enum.GetValues(typeof(Counter)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					int num = (int)enumerator.Current;
					if (stopwatchDict.ContainsKey(num))
					{
						stopwatchDict[num].isActive = counters.Contains((Counter)num);
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

		public MyStopwatch GetCounter(Counter counter)
		{
			return stopwatchDict[(int)counter];
		}

		private void Awake()
		{
			InitStopwatches();
			StartCoroutine(FrameCoroutine());
		}

		private void InitStopwatches()
		{
			stopwatchDict = new Dictionary<int, MyStopwatch>();
			IEnumerator enumerator = Enum.GetValues(typeof(Counter)).GetEnumerator();
			try
			{
				while (enumerator.MoveNext())
				{
					int key = (int)enumerator.Current;
					stopwatchDict.Add(key, new MyStopwatch
					{
						isActive = false
					});
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
			stopwatchDict[5].isActive = true;
			stopwatchDict[5].Start();
		}

		private void FixedUpdate()
		{
			if (!wasFixedUpdateTriggered)
			{
				stopwatchDict[0].StopAndReset();
				stopwatchDict[1].Start();
				wasFixedUpdateTriggered = true;
			}
		}

		private void Update()
		{
			wasFixedUpdateTriggered = false;
			wasOnGUITriggered = false;
			stopwatchDict[1].StopAndReset();
			stopwatchDict[2].Start();
			stopwatchDict[5].Restart();
		}

		private void LateUpdate()
		{
			stopwatchDict[2].StopAndReset();
			stopwatchDict[3].Start();
		}

		private void OnPreCull()
		{
			stopwatchDict[7].Start();
		}

		private void OnPreRender()
		{
			stopwatchDict[7].StopAndReset();
			stopwatchDict[8].Start();
		}

		private void OnPostRender()
		{
			stopwatchDict[8].StopAndReset();
			stopwatchDict[9].Start();
		}

		private void OnGUI()
		{
			if (!wasOnGUITriggered)
			{
				stopwatchDict[9].StopAndReset();
				stopwatchDict[3].StopAndReset();
				stopwatchDict[4].Start();
				wasOnGUITriggered = true;
			}
		}

		private IEnumerator FrameCoroutine()
		{
			while (true)
			{
				yield return wait;
				stopwatchDict[4].StopAndReset();
				stopwatchDict[0].Start();
			}
		}

		public string GetLogs()
		{
			StringBuilder stringBuilder = new StringBuilder();
			Counter[] array = Enum.GetValues(typeof(Counter)) as Counter[];
			for (int i = 0; i < array.Length; i++)
			{
				MyStopwatch myStopwatch = stopwatchDict[(int)array[i]];
				if (myStopwatch.isActive)
				{
					double averageDuration = myStopwatch.averageDuration;
					if (array[i] == Counter.FRAME)
					{
						double num = 1000.0 / averageDuration;
						stringBuilder.Append($"{array[i].ToString().ToTitleCase()}: {averageDuration:0.0} ms ({num:0.0} fps)");
					}
					else
					{
						stringBuilder.Append($"{array[i].ToString().ToTitleCase()}: {averageDuration:0.0} ms");
					}
					if (i < array.Length - 1)
					{
						stringBuilder.Append(Environment.NewLine);
					}
				}
			}
			return stringBuilder.ToString();
		}
	}
}
