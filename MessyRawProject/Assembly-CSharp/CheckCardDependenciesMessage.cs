using System;
using System.Collections.Generic;

// Token: 0x0200029C RID: 668
public class CheckCardDependenciesMessage : Message
{
	// Token: 0x06001251 RID: 4689 RVA: 0x0000D4E8 File Offset: 0x0000B6E8
	public CheckCardDependenciesMessage()
	{
	}

	// Token: 0x06001252 RID: 4690 RVA: 0x0000DD27 File Offset: 0x0000BF27
	public CheckCardDependenciesMessage(long[] cardIds)
	{
		this.cardIds = cardIds;
	}

	// Token: 0x06001253 RID: 4691 RVA: 0x0000DD36 File Offset: 0x0000BF36
	public CheckCardDependenciesMessage(long cardId)
	{
		this.cardIds = new long[]
		{
			cardId
		};
	}

	// Token: 0x06001254 RID: 4692 RVA: 0x00077C7C File Offset: 0x00075E7C
	public string[] GetDeckNames()
	{
		if (this.dependencies != null)
		{
			HashSet<string> hashSet = new HashSet<string>();
			foreach (CardDependency cardDependency in this.dependencies)
			{
				foreach (string text in cardDependency.deckNames)
				{
					hashSet.Add(text);
				}
			}
			string[] array2 = new string[hashSet.Count];
			hashSet.CopyTo(array2);
			return array2;
		}
		return new string[0];
	}

	// Token: 0x04000F2E RID: 3886
	public long[] cardIds;

	// Token: 0x04000F2F RID: 3887
	[ServerToClient]
	public CardDependency[] dependencies;
}
