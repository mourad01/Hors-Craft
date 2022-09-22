// DecompilerFi decompiler from Assembly-CSharp.dll class: QuestValueGeneratorCustom
public class QuestValueGeneratorCustom : WorldsQuests.QuestValueGenerator
{
	public QuestValueGeneratorCustom(int req_a, int req_b, int price_a, int price_b)
		: base(req_a, req_b, price_a, price_b)
	{
		base.req_a = req_a;
		base.req_b = req_b;
		base.price_a = price_a;
		base.price_b = price_b;
	}
}
