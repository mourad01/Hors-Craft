// DecompilerFi decompiler from Assembly-CSharp.dll class: LimitWorld.Limit
using System;
using System.Runtime.CompilerServices;
using Uniblocks;
using UnityEngine;

namespace LimitWorld
{
	[Serializable]
	public abstract class Limit : ScriptableObject
	{
		[SerializeField]
		protected LimitShape limitShape;

		private bool _inited;

		[Header("EDITOR")]
		public Color GizmosColor;

		public abstract EventTypeLW eventType
		{
			get;
		}

		public bool inited
		{
			get
			{
				if (_inited || Engine.EngineInstance == null)
				{
					return _inited;
				}
				Vector3 startPlayerPosition = Engine.EngineInstance.startPlayerPosition;
				startPlayerPosition.y = 0f;
				limitShape.center = startPlayerPosition;
				initialAction?.Invoke();
				limitShape.Init();
				_inited = true;
				return _inited;
			}
			set
			{
				_inited = value;
			}
		}

		protected virtual Action initialAction
		{
			[CompilerGenerated]
			get
			{
				return null;
			}
		}

		protected virtual bool doChunkCorrection
		{
			[CompilerGenerated]
			get
			{
				return false;
			}
		}

		public abstract bool ProcessEvent(DataLW data);

		public virtual void ResetLimit()
		{
			inited = false;
		}

		public virtual void ReSetup()
		{
			limitShape.NewSize();
			if (!inited)
			{
				inited = inited;
			}
		}

		public bool InBound(Vector3 worldPosition)
		{
			return limitShape.Contains(worldPosition, doChunkCorrection);
		}

		public void DrawGizmos(bool selected)
		{
			if ((object)limitShape != null)
			{
				limitShape.DrawGizos(GizmosColor, doChunkCorrection, selected);
			}
		}
	}
}
