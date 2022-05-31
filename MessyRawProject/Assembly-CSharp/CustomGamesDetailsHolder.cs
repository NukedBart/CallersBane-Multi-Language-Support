using System;
using UnityEngine;

// Token: 0x0200017F RID: 383
public class CustomGamesDetailsHolder
{
	// Token: 0x06000BF4 RID: 3060 RVA: 0x00054A14 File Offset: 0x00052C14
	public Rect contentRect()
	{
		return new Rect(0f, 0f, this.frameRect.width * 0.95f, this.descRect.yMax - this.titleRect.y + (float)Screen.height * 0.01f);
	}

	// Token: 0x06000BF5 RID: 3061 RVA: 0x00054A68 File Offset: 0x00052C68
	public static CustomGamesDetailsHolder defaultFromRect(Rect rect, float u, CustomGamesDescriptionData d)
	{
		CustomGamesDetailsHolder customGamesDetailsHolder = new CustomGamesDetailsHolder();
		customGamesDetailsHolder.frameRect = rect;
		customGamesDetailsHolder.betRect = new Rect(0f, 0f, rect.width - 10f * u, rect.height + 10f * u);
		customGamesDetailsHolder.betRect.xMin = customGamesDetailsHolder.betRect.xMax - 170f * u;
		customGamesDetailsHolder.betRect.yMax = customGamesDetailsHolder.betRect.yMin + 40f * u;
		customGamesDetailsHolder.deckButtonRect = customGamesDetailsHolder.betRect;
		CustomGamesDetailsHolder customGamesDetailsHolder2 = customGamesDetailsHolder;
		customGamesDetailsHolder2.deckButtonRect.y = customGamesDetailsHolder2.deckButtonRect.y + (customGamesDetailsHolder.deckButtonRect.height + 10f * u);
		if (d != null)
		{
			float width = customGamesDetailsHolder.betRect.x - 30f * u;
			d.updateTitleWidth(width);
			d.updateFlavorWidth(width);
			customGamesDetailsHolder.titleRect = new Rect(20f * u, 5f * u, d.name.width, d.name.height * 1.5f);
			customGamesDetailsHolder.flavorRect = new Rect(20f * u, d.name.height + 10f * u, d.flavor.width, d.flavor.height);
			customGamesDetailsHolder.descRect = new Rect(40f * u, d.name.height + d.flavor.height + 25f * u, d.desc.width, d.desc.height);
		}
		return customGamesDetailsHolder;
	}

	// Token: 0x04000926 RID: 2342
	public Rect frameRect;

	// Token: 0x04000927 RID: 2343
	public Rect titleRect;

	// Token: 0x04000928 RID: 2344
	public Rect flavorRect;

	// Token: 0x04000929 RID: 2345
	public Rect descRect;

	// Token: 0x0400092A RID: 2346
	public Rect betRect;

	// Token: 0x0400092B RID: 2347
	public Rect deckButtonRect;

	// Token: 0x0400092C RID: 2348
	public CustomGamesDeckHolder deck = new CustomGamesDeckHolder();
}
