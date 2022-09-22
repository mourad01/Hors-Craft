// DecompilerFi decompiler from Assembly-CSharp.dll class: PackageData
public class PackageData
{
	public string imageName;

	public string packageTitle;

	public int currencyCount;

	public string iapIdentifier;

	public string labelText;

	public string iapIOSIdentifier;

	public string fakeCost;

	public int isVideo;

	public bool isCostVideo => (isVideo != 0) ? true : false;

	public PackageData()
	{
	}

	public PackageData(string imageName, string packageTitle, int currencyCount, string iapIdentifier, string labelText, bool isVideo)
	{
		this.imageName = imageName;
		this.packageTitle = packageTitle;
		this.currencyCount = currencyCount;
		this.iapIdentifier = iapIdentifier;
		this.labelText = labelText;
		this.isVideo = (isVideo ? 1 : 0);
	}
}
