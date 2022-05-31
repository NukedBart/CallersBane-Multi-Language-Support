using System;

// Token: 0x020000F1 RID: 241
public class Card_Post : Card_Fake
{
	// Token: 0x06000808 RID: 2056 RVA: 0x000439E8 File Offset: 0x00041BE8
	public Card_Post(int id)
	{
		char c = (char)(65 + id);
		int num = 2147482647 + id;
		string name = "zzzzzzzzzzzzzzzz" + c.ToString();
		this.id = (long)num;
		this.type = new CardType();
		this.type.id = num;
		this.type.name = name;
		this.type.kind = CardType.Kind.LAST;
		this.type.costDecay = (this.type.costOrder = (this.type.costGrowth = (this.type.costEnergy = 999999)));
	}
}
