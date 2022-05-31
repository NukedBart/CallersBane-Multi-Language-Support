using System;

// Token: 0x02000256 RID: 598
public static class ResourceTypeExtensions
{
	// Token: 0x060011D0 RID: 4560 RVA: 0x0000D8A0 File Offset: 0x0000BAA0
	public static bool isResource(this ResourceType type)
	{
		return type != ResourceType.SPECIAL && type != ResourceType.NONE && !type.isCards();
	}

	// Token: 0x060011D1 RID: 4561 RVA: 0x000053B0 File Offset: 0x000035B0
	public static bool isCards(this ResourceType type)
	{
		return type == ResourceType.CARDS;
	}

	// Token: 0x060011D2 RID: 4562 RVA: 0x0000D8BC File Offset: 0x0000BABC
	public static string battleIconFilename(this ResourceType type)
	{
		return "BattleUI/battlegui_icon_" + type.ToString().ToLower();
	}

	// Token: 0x060011D3 RID: 4563 RVA: 0x0000D8D8 File Offset: 0x0000BAD8
	public static string guiIconFilename(this ResourceType type)
	{
		return "Icons/Resources/256_" + type.ToString().ToLower();
	}
}
