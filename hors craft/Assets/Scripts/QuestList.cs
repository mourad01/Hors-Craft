// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestList
using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestList : MonoBehaviour
{
	public List<Quest> quests;

	public Sprite[] images = new Sprite[Enum.GetValues(typeof(QuestType)).Length];
}
