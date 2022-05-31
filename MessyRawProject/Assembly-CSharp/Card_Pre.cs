using System;

// Token: 0x020000F0 RID: 240
public class Card_Pre : Card_Fake
{
	// Token: 0x06000807 RID: 2055 RVA: 0x00043948 File Offset: 0x00041B48
	public Card_Pre(int id)
	{
		string name = "AAAAAAAAAAAAAAA" + ((char)(65 + id)).ToString();
		int num = -1000 + id;
		this.id = (long)num;
		this.type = new CardType();
		this.type.id = num;
		this.type.name = name;
		this.type.kind = CardType.Kind.NONE;
		this.type.costDecay = (this.type.costOrder = (this.type.costGrowth = (this.type.costEnergy = 0)));
	}
}
