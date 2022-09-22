// DecompilerFi decompiler from Assembly-CSharp.dll class: AdventureScreenData
public class AdventureScreenData
{
	private string mainText;

	private string[] chooseOpitions = new string[0];

	public string MainText
	{
		get
		{
			return mainText;
		}
		set
		{
			mainText = value;
		}
	}

	public int OptionsLength => chooseOpitions.Length;

	public AdventureScreenData()
	{
	}

	public AdventureScreenData(string mainText, string[] options)
	{
		this.mainText = mainText;
		chooseOpitions = options;
	}

	public void SetOpitions(string[] newChooseOpitions)
	{
		if (newChooseOpitions.Length >= 2 && newChooseOpitions[1].Contains("{x}"))
		{
			chooseOpitions = new string[1]
			{
				newChooseOpitions[0]
			};
		}
		else
		{
			chooseOpitions = newChooseOpitions;
		}
	}

	public string GetOption(int index)
	{
		if (index < 0 || chooseOpitions == null || index >= chooseOpitions.Length)
		{
			return string.Empty;
		}
		return chooseOpitions[index];
	}
}
