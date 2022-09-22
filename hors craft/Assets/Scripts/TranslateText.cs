// DecompilerFi decompiler from Assembly-CSharp.dll class: TranslateText
using Common.Managers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class TranslateText : MonoBehaviour
{
	public delegate string OnVisitText(string text);

	public enum TransformationMode
	{
		UNTOUCHED,
		TO_UPPER,
		TO_LOWER,
		TO_TITLE_CASE,
		FIRST_LETTER_UPPER
	}

	public string defaultText = string.Empty;

	public string translationKey = string.Empty;

	private TranslationsManager translationsManager;

	private Text foundTextComponent;

	private List<OnVisitText> onVisitTexts = new List<OnVisitText>();

	public TransformationMode transformation;

	private bool textDirty;

	private bool waitsToBeConnected;

	private Text textComponent
	{
		get
		{
			if (foundTextComponent == null)
			{
				foundTextComponent = GetComponent<Text>();
			}
			return foundTextComponent;
		}
	}

	private void Awake()
	{
		translationsManager = Manager.Get<TranslationsManager>();
		RefreshText(translationsManager.GetText(translationKey, string.Empty));
	}

	private void Update()
	{
		CheckIfDownloadIsInProgress();
		CheckIfDownloadIsDone();
		RefreshIfDirty();
	}

	private void CheckIfDownloadIsInProgress()
	{
		if (!waitsToBeConnected && !translationsManager.translationsDownloaded)
		{
			waitsToBeConnected = true;
		}
	}

	private void CheckIfDownloadIsDone()
	{
		if (waitsToBeConnected && translationsManager.translationsDownloaded)
		{
			waitsToBeConnected = false;
			textDirty = true;
		}
	}

	private void RefreshIfDirty()
	{
		if (textDirty)
		{
			textDirty = false;
			RefreshText(translationsManager.GetText(translationKey, string.Empty));
		}
	}

	private void RefreshText(string newText = "")
	{
		if (string.IsNullOrEmpty(newText))
		{
			newText = defaultText;
		}
		foreach (OnVisitText onVisitText in onVisitTexts)
		{
			newText = onVisitText(newText);
		}
		newText = ApplyTransformation(newText);
		SetText(newText);
	}

	private string ApplyTransformation(string text)
	{
		switch (transformation)
		{
		case TransformationMode.TO_UPPER:
			return text.ToUpperInvariant();
		case TransformationMode.TO_LOWER:
			return text.ToLowerInvariant();
		case TransformationMode.TO_TITLE_CASE:
			return text.ToTitleCase();
		case TransformationMode.FIRST_LETTER_UPPER:
			return FirstLetterUpper(text);
		default:
			return text;
		}
	}

	protected virtual void SetText(string text)
	{
		textComponent.text = text;
	}

	public void AddVisitor(OnVisitText visitor)
	{
		onVisitTexts.Add(visitor);
		textDirty = true;
	}

	public void RemoveVisitor(OnVisitText visitor)
	{
		onVisitTexts.Remove(visitor);
		textDirty = true;
	}

	public void ClearVisitors()
	{
		onVisitTexts.Clear();
		textDirty = true;
	}

	public void ForceRefresh()
	{
		textDirty = true;
	}

	private string FirstLetterUpper(string input)
	{
		if (!string.IsNullOrEmpty(input))
		{
			StringBuilder stringBuilder = new StringBuilder(input.First().ToString().ToUpper());
			for (int i = 1; i < input.Length; i++)
			{
				if ((input[i] == '.' || input[i] == '?' || input[i] == '!') && i + 2 < input.Length && input[i + 1] == ' ')
				{
					stringBuilder.Append(". " + char.ToUpper(input[i + 2]));
					i += 2;
				}
				else
				{
					stringBuilder.Append(char.ToLower(input[i]));
				}
			}
			return stringBuilder.ToString();
		}
		return input;
	}

	private void OnValidate()
	{
		if (string.IsNullOrEmpty(translationKey))
		{
			SetText("Define key!");
		}
		else if (string.IsNullOrEmpty(defaultText))
		{
			SetText(translationKey);
		}
		else
		{
			RefreshText(string.Empty);
		}
	}
}
