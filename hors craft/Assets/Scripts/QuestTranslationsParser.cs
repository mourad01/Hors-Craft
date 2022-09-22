// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestTranslationsParser
using Common.Managers;
using QuestSystems.Adventure;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class QuestTranslationsParser
{
	public enum QuestDialogSequenceState
	{
		inprogress,
		done,
		error
	}

	protected const string KEY_PLAYER = ".player";

	protected const string KEY_IF = ".if";

	protected const string KEY_DEFAULT_AFTERDONE = "deafultAfterDone";

	private TranslationsManager translationsManager;

	public QuestTranslationsParser()
	{
		translationsManager = Manager.Get<TranslationsManager>();
	}

	public QuestDialogSequenceState GetDialogUntil(QuestDataItem quest, out AdventureScreenData dialog)
	{
		int num = 200;
		string[] array = new string[3]
		{
			string.Empty,
			".if",
			".player"
		};
		dialog = new AdventureScreenData();
		int num2 = quest.CurrentLine;
		string empty = string.Empty;
		string empty2 = string.Empty;
		string[] playerOptions = new string[0];
		bool flag = false;
		bool flag2 = false;
		while (num > 0 && !flag)
		{
			for (int i = 0; i < array.Length; i++)
			{
				empty = GetDialogKey(quest, num2, string.Empty);
				empty = $"{empty}{array[i]}";
				if (i == 1)
				{
					empty = $"{empty}.{quest.LastChoosedOption}";
				}
				empty2 = GetTranslatedText(empty);
				if (!string.IsNullOrEmpty(empty2))
				{
					flag = true;
					if (i == 2)
					{
						flag2 = ParseOptionsLine(empty2, out playerOptions);
						dialog.SetOpitions(playerOptions);
					}
					else
					{
						dialog.MainText = empty2;
					}
					if (quest.QuestState == EQuestState.afterDone)
					{
						empty = GetDialogKey(quest, 0, string.Empty);
						empty = string.Format("{0}{1}", empty, ".player");
						empty2 = GetTranslatedText(empty);
						flag2 = ParseOptionsLine(empty2, out playerOptions);
						dialog.SetOpitions(playerOptions);
					}
					num = 0;
					break;
				}
			}
			num2++;
			num--;
		}
		if (flag)
		{
			int updateCount = num2 - quest.CurrentLine;
			quest.UpdateLine(updateCount);
			return flag2 ? QuestDialogSequenceState.done : QuestDialogSequenceState.inprogress;
		}
		return QuestDialogSequenceState.error;
	}

	public bool ParseOptionsLine(string unparsedOptionsLine, out string[] playerOptions)
	{
		List<string> list = new List<string>(unparsedOptionsLine.Split(Environment.NewLine.ToCharArray()));
		for (int num = list.Count - 1; num >= 0; num--)
		{
			if (string.IsNullOrEmpty(list[num]))
			{
				list.RemoveAt(num);
			}
		}
		playerOptions = list.ToArray();
		return (playerOptions == null || playerOptions.Length < 2) ? true : false;
	}

	private bool CheckForOptions(string key, out string[] options, out bool isFinishLine)
	{
		options = null;
		isFinishLine = false;
		string key2 = string.Format("{0}.{1}", key, ".player");
		string translatedText = GetTranslatedText(key2);
		if (!string.IsNullOrEmpty(translatedText))
		{
			options = translatedText.Split(Environment.NewLine.ToCharArray());
			if (options == null)
			{
				return false;
			}
			isFinishLine = ((options.Length == 1) ? true : false);
			return true;
		}
		return false;
	}

	private string GetDialogKey(QuestDataItem quest, int line, string aditionalKey = "")
	{
		string text = quest.QuestName;
		if (quest.QuestState == EQuestState.afterDone)
		{
			text = "deafultAfterDone";
			line = UnityEngine.Random.Range(0, 10);
			if (aditionalKey.Contains(".player"))
			{
				line = 0;
			}
		}
		if (string.IsNullOrEmpty(aditionalKey))
		{
			return $"scene.{text}.{ChooseState(quest.QuestState)}.line.{line}";
		}
		return $"scene.{text}.{ChooseState(quest.QuestState)}.line.{line}.{aditionalKey}";
	}

	private string ChooseState(EQuestState questState)
	{
		switch (questState)
		{
		case EQuestState.notStarted:
			return "start";
		case EQuestState.started:
			return "not_done";
		case EQuestState.notDone:
			return "not_done";
		case EQuestState.partiallyDone:
			return "done_partially";
		case EQuestState.done:
			return "done";
		case EQuestState.afterDone:
			return "afterDone";
		default:
			return "start";
		}
	}

	protected string GetTranslatedText(string key)
	{
		if (translationsManager == null)
		{
			return string.Empty;
		}
		return translationsManager.GetText(key, string.Empty);
	}

	public List<string> GetMarkers(string text)
	{
		List<string> list = new List<string>();
		int num = 0;
		int num2 = 0;
		if (!string.IsNullOrEmpty(text))
		{
			for (int i = 0; i < text.Length; i++)
			{
				num = text.IndexOf('{', i);
				if (num > 0)
				{
					num2 = text.IndexOf('}', num);
					list.Add(text.Substring(num, num2 - num + 1));
				}
				if (num2 > 0)
				{
					i = num2;
				}
				num2 = 0;
				num = 0;
			}
		}
		return list;
	}

	public bool CheckForMarker(List<string> markers, string marker)
	{
		for (int i = 0; i < markers.Count; i++)
		{
			if (markers[i].Contains(marker))
			{
				return true;
			}
		}
		return false;
	}

	public int CheckForItemMarker(List<string> markers)
	{
		int result = -1;
		string empty = string.Empty;
		for (int i = 0; i < markers.Count; i++)
		{
			int num = markers[i].IndexOf('}');
			empty = markers[i].Substring(1, num - 1);
			if (!string.IsNullOrEmpty(empty))
			{
				if (int.TryParse(empty, NumberStyles.Any, CultureInfo.InvariantCulture, out result))
				{
					return result;
				}
				result = -1;
			}
		}
		return result;
	}
}
