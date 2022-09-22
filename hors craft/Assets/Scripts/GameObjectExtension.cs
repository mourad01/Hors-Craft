// DecompilerFi decompiler from Assembly-CSharp.dll class: GameObjectExtension
using System;
using UnityEngine;

public static class GameObjectExtension
{
	public static T AddComponentWithInit<T>(this GameObject obj, Action<T> onInit) where T : Component
	{
		bool activeSelf = obj.activeSelf;
		obj.SetActive(value: false);
		T val = obj.AddComponent<T>();
		onInit?.Invoke(val);
		obj.SetActive(activeSelf);
		return val;
	}
}
