using System;

// Token: 0x02000108 RID: 264
public class KeywordDescription
{
	// Token: 0x0600087E RID: 2174 RVA: 0x000076C1 File Offset: 0x000058C1
	private KeywordDescription(string keyword, string description, KeywordDescription.Type type)
	{
		this.keyword = keyword;
		this.description = description;
		this.type = type;
	}

	// Token: 0x0600087F RID: 2175 RVA: 0x000076DE File Offset: 0x000058DE
	public static KeywordDescription fromWord(string keyword, string description)
	{
		return new KeywordDescription(keyword, description, KeywordDescription.Type.Word);
	}

	// Token: 0x06000880 RID: 2176 RVA: 0x000076E8 File Offset: 0x000058E8
	public static KeywordDescription fromCardReference(string keyword, string description)
	{
		return new KeywordDescription(keyword, description, KeywordDescription.Type.CardRef);
	}

	// Token: 0x04000643 RID: 1603
	public readonly string keyword;

	// Token: 0x04000644 RID: 1604
	public readonly string description;

	// Token: 0x04000645 RID: 1605
	private readonly KeywordDescription.Type type;

	// Token: 0x02000109 RID: 265
	private enum Type
	{
		// Token: 0x04000647 RID: 1607
		Word,
		// Token: 0x04000648 RID: 1608
		CardRef
	}
}
