// DecompilerFi decompiler from Assembly-CSharp.dll class: ComputerSizingExtensions
public static class ComputerSizingExtensions
{
	private const int INT_OneKB = 1024;

	public static int KB(this int value)
	{
		return value * 1024;
	}

	public static int MB(this int value)
	{
		return value * 1024 * 1024;
	}

	public static int GB(this int value)
	{
		return value * 1024 * 1024 * 1024;
	}

	public static int TB(this int value)
	{
		return value * 1024 * 1024 * 1024 * 1024;
	}

	public static int PB(this int value)
	{
		return value * 1024 * 1024 * 1024 * 1024 * 1024;
	}

	public static int EB(this int value)
	{
		return value * 1024 * 1024 * 1024 * 1024 * 1024 * 1024;
	}

	public static int ZB(this int value)
	{
		return value * 1024 * 1024 * 1024 * 1024 * 1024 * 1024 * 1024;
	}

	public static int YB(this int value)
	{
		return value * 1024 * 1024 * 1024 * 1024 * 1024 * 1024 * 1024 * 1024;
	}
}
