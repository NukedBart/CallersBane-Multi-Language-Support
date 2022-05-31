using System;
using UnityEngine;

// Token: 0x0200017E RID: 382
public class CustomGamesGuiHolder
{
	// Token: 0x06000BF1 RID: 3057 RVA: 0x00009C46 File Offset: 0x00007E46
	public float u(float v)
	{
		return this._u * v;
	}

	// Token: 0x06000BF2 RID: 3058 RVA: 0x00009C50 File Offset: 0x00007E50
	public float rowWidth()
	{
		return this.gamesListRectInner.width * 0.95f;
	}

	// Token: 0x04000919 RID: 2329
	public float labelFontSize;

	// Token: 0x0400091A RID: 2330
	public CustomGamesDetailsHolder details = new CustomGamesDetailsHolder();

	// Token: 0x0400091B RID: 2331
	public Rect gamesListRectInner;

	// Token: 0x0400091C RID: 2332
	public Rect fullRect;

	// Token: 0x0400091D RID: 2333
	public Rect leftRect;

	// Token: 0x0400091E RID: 2334
	public Rect rightRect;

	// Token: 0x0400091F RID: 2335
	public Rect inputNameRect;

	// Token: 0x04000920 RID: 2336
	public Rect inputRect;

	// Token: 0x04000921 RID: 2337
	public Rect searchInputRect;

	// Token: 0x04000922 RID: 2338
	public Rect detailsRect;

	// Token: 0x04000923 RID: 2339
	public Rect buttonRect;

	// Token: 0x04000924 RID: 2340
	public float buttonHeight;

	// Token: 0x04000925 RID: 2341
	public float _u;
}
