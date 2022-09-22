// DecompilerFi decompiler from Assembly-CSharp.dll class: Common.Utils.Clipboard
using System;
using System.Reflection;
using UnityEngine;

namespace Common.Utils
{
	[Obsolete("Obsolete: Works only in older versions of Unity. Use Common.Clipboard instead.")]
	public class Clipboard
	{
		private static PropertyInfo m_systemCopyBufferProperty;

		public static string clipBoard
		{
			get
			{
				PropertyInfo systemCopyBufferProperty = GetSystemCopyBufferProperty();
				return (string)systemCopyBufferProperty.GetValue(null, null);
			}
			set
			{
				PropertyInfo systemCopyBufferProperty = GetSystemCopyBufferProperty();
				systemCopyBufferProperty.SetValue(null, value, null);
			}
		}

		private static PropertyInfo GetSystemCopyBufferProperty()
		{
			if (m_systemCopyBufferProperty == null)
			{
				Type typeFromHandle = typeof(GUIUtility);
				m_systemCopyBufferProperty = typeFromHandle.GetProperty("systemCopyBuffer", BindingFlags.Static | BindingFlags.NonPublic);
				if (m_systemCopyBufferProperty == null)
				{
					throw new Exception("Can't access internal member 'GUIUtility.systemCopyBuffer' it may have been removed / renamed");
				}
			}
			return m_systemCopyBufferProperty;
		}
	}
}
