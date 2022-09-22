// DecompilerFi decompiler from Assembly-CSharp.dll class: Money
public struct Money
{
	public decimal Value
	{
		get;
		private set;
	}

	public Money(decimal value)
	{
		this = default(Money);
		Value = value.Rounded(2);
	}

	public Money(Money value)
	{
		this = default(Money);
		Value = value.Value;
	}

	public static implicit operator Money(decimal value)
	{
		return new Money(value);
	}

	public static implicit operator decimal(Money value)
	{
		return value.Value;
	}
}
