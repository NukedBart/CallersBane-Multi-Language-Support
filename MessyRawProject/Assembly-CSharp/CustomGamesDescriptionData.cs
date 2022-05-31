using System;
using UnityEngine;

// Token: 0x02000181 RID: 385
public class CustomGamesDescriptionData
{
	// Token: 0x06000BF9 RID: 3065 RVA: 0x00009C89 File Offset: 0x00007E89
	public void update(CustomGameInfo c, float width, float deckWidth, float u, GUIStyle title, GUIStyle small, bool swapPlayers)
	{
		this._c = c;
		this._title = title;
		this._small = small;
		this.update(width, deckWidth, u, swapPlayers);
	}

	// Token: 0x06000BFA RID: 3066 RVA: 0x00054CE4 File Offset: 0x00052EE4
	private void update(float width, float deckWidth, float u, bool swapPlayers)
	{
		float num = width * 0.925f;
		this.updateTitleWidth(num);
		this.updateFlavorWidth(num);
		this.desc = new SizedString(((!swapPlayers) ? this._c.descriptionP1 : this._c.descriptionP2).Trim(), num - 34f * u, this._small);
		this.deckP1 = new SizedString(((!swapPlayers) ? this._c.deckP1 : this._c.deckP2).Trim(), deckWidth, this._small);
		this.deckP2 = new SizedString(((!swapPlayers) ? this._c.deckP2 : this._c.deckP1).Trim(), deckWidth, this._small);
		if (!string.IsNullOrEmpty(this._c.bet))
		{
			this.bet = "     Bet: " + this._c.bet;
		}
		else
		{
			this.bet = string.Empty;
		}
		this.inited = true;
	}

	// Token: 0x06000BFB RID: 3067 RVA: 0x00009CAE File Offset: 0x00007EAE
	public void updateTitleWidth(float width)
	{
		this.name = new SizedString(this._c.name, width, this._title);
	}

	// Token: 0x06000BFC RID: 3068 RVA: 0x00009CCD File Offset: 0x00007ECD
	public void updateFlavorWidth(float width)
	{
		this.flavor = new SizedString(this._c.flavor, width, this._small);
	}

	// Token: 0x06000BFD RID: 3069 RVA: 0x00009CEC File Offset: 0x00007EEC
	public CustomGameInfo game()
	{
		return this._c;
	}

	// Token: 0x06000BFE RID: 3070 RVA: 0x00009CF4 File Offset: 0x00007EF4
	public bool isEmpty()
	{
		return !this.inited;
	}

	// Token: 0x06000BFF RID: 3071 RVA: 0x00009CFF File Offset: 0x00007EFF
	public void clear()
	{
		this.inited = false;
	}

	// Token: 0x04000931 RID: 2353
	public SizedString name;

	// Token: 0x04000932 RID: 2354
	public SizedString flavor;

	// Token: 0x04000933 RID: 2355
	public SizedString desc;

	// Token: 0x04000934 RID: 2356
	public SizedString deckP1;

	// Token: 0x04000935 RID: 2357
	public SizedString deckP2;

	// Token: 0x04000936 RID: 2358
	public string bet = string.Empty;

	// Token: 0x04000937 RID: 2359
	private CustomGameInfo _c;

	// Token: 0x04000938 RID: 2360
	public GUIStyle _title;

	// Token: 0x04000939 RID: 2361
	public GUIStyle _small;

	// Token: 0x0400093A RID: 2362
	private bool inited;
}
