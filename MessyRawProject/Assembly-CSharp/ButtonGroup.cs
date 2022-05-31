using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200018C RID: 396
public class ButtonGroup
{
	// Token: 0x06000C4E RID: 3150 RVA: 0x00056EBC File Offset: 0x000550BC
	public ButtonGroup(bool multi, float x, float y, float yspace, string title)
	{
		this.multi = multi;
		this.x = x;
		this.y = y;
		this.yspace = yspace;
		this.title = title;
		this.yoffset = 0f;
		this.iw = 0.12f * (float)Screen.height;
		this.ih = 0.055f * (float)Screen.height;
		if (title != null)
		{
			this.yoffset = this.ih * 0.6f;
			this.titleSkin = (GUISkin)ResourceManager.Load("_GUISkins/PlaqueTitle");
		}
	}

	// Token: 0x06000C4F RID: 3151 RVA: 0x0000A06C File Offset: 0x0000826C
	public void addItem(string name, bool selected)
	{
		this.items.Add(new ButtonGroup.Item(name, selected));
		this.hitCounts.Add(0);
	}

	// Token: 0x06000C50 RID: 3152 RVA: 0x00056F68 File Offset: 0x00055168
	public void setSelected(int index)
	{
		if (index >= this.items.Count)
		{
			return;
		}
		if (index < 0 || !this.multi)
		{
			for (int i = 0; i < this.items.Count; i++)
			{
				this.items[i].selected = false;
			}
		}
		if (index >= 0)
		{
			this.items[index].selected = true;
			List<int> list2;
			List<int> list = list2 = this.hitCounts;
			int num = list2[index];
			list[index] = num + 1;
			this.hitCountsInRow++;
		}
	}

	// Token: 0x06000C51 RID: 3153 RVA: 0x0000A08C File Offset: 0x0000828C
	public bool isSelected(int index)
	{
		return index >= 0 && index < this.items.Count && this.items[index].selected;
	}

	// Token: 0x06000C52 RID: 3154 RVA: 0x0005700C File Offset: 0x0005520C
	public int getFirstSelected()
	{
		for (int i = 0; i < this.items.Count; i++)
		{
			if (this.items[i].selected)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000C53 RID: 3155 RVA: 0x00057050 File Offset: 0x00055250
	public int getSelectedBitSet()
	{
		int num = 0;
		for (int i = 0; i < this.items.Count; i++)
		{
			if (this.isSelected(i))
			{
				num |= 1 << i;
			}
		}
		return num;
	}

	// Token: 0x06000C54 RID: 3156 RVA: 0x00057094 File Offset: 0x00055294
	public bool isInside(Vector2 p)
	{
		for (int i = 0; i < this.items.Count; i++)
		{
			if (this.getRectForItem(i).Contains(p))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000C55 RID: 3157 RVA: 0x0000A0B9 File Offset: 0x000082B9
	public int getHitCountsInRow()
	{
		return this.hitCountsInRow;
	}

	// Token: 0x06000C56 RID: 3158 RVA: 0x0000A0C1 File Offset: 0x000082C1
	public int getHitCount(int index)
	{
		return this.hitCounts[index];
	}

	// Token: 0x06000C57 RID: 3159 RVA: 0x000570D8 File Offset: 0x000552D8
	public void clearHitCounts()
	{
		for (int i = 0; i < this.hitCounts.Count; i++)
		{
			this.hitCounts[i] = 0;
		}
	}

	// Token: 0x06000C58 RID: 3160 RVA: 0x00057110 File Offset: 0x00055310
	public void render()
	{
		int fontSize = GUI.skin.button.fontSize;
		if (this.title != null)
		{
			this.titleSkin.button.fontSize = Screen.height / 52;
			Rect rect;
			rect..ctor(this.x, this.y, this.iw, this.ih * 0.73f);
			GUI.Label(rect, this.title, this.titleSkin.button);
		}
		GUI.skin.button.fontSize = Screen.height / 44;
		for (int i = 0; i < this.items.Count; i++)
		{
			bool flag = this.renderItem(i);
			if (this.multi)
			{
				this.items[i].selected = false;
			}
			if (flag)
			{
				this.setSelected(i);
			}
		}
		GUI.skin.button.fontSize = fontSize;
	}

	// Token: 0x06000C59 RID: 3161 RVA: 0x00057204 File Offset: 0x00055404
	public virtual bool renderItem(int index)
	{
		Rect rectForItem = this.getRectForItem(index);
		ButtonGroup.Item item = this.items[index];
		bool flag = GUI.Toggle(rectForItem, item.selected, item.content, GUI.skin.button);
		item.clicked = (flag && !item.selected);
		return flag;
	}

	// Token: 0x06000C5A RID: 3162 RVA: 0x0000A0CF File Offset: 0x000082CF
	private Rect getRectForItem(int index)
	{
		return new Rect(this.x, this.y + this.yoffset + (float)index * this.yspace, this.iw, this.ih);
	}

	// Token: 0x04000982 RID: 2434
	private bool multi;

	// Token: 0x04000983 RID: 2435
	private string title;

	// Token: 0x04000984 RID: 2436
	protected float x;

	// Token: 0x04000985 RID: 2437
	protected float y;

	// Token: 0x04000986 RID: 2438
	protected float yspace;

	// Token: 0x04000987 RID: 2439
	protected float yoffset;

	// Token: 0x04000988 RID: 2440
	protected List<ButtonGroup.Item> items = new List<ButtonGroup.Item>();

	// Token: 0x04000989 RID: 2441
	protected List<int> hitCounts = new List<int>();

	// Token: 0x0400098A RID: 2442
	protected int hitCountsInRow;

	// Token: 0x0400098B RID: 2443
	private GUISkin titleSkin;

	// Token: 0x0400098C RID: 2444
	protected float iw;

	// Token: 0x0400098D RID: 2445
	protected float ih;

	// Token: 0x0200018D RID: 397
	protected class Item
	{
		// Token: 0x06000C5B RID: 3163 RVA: 0x0000A0FF File Offset: 0x000082FF
		public Item(string text, bool selected)
		{
			this.content = new GUIContent(text);
			this.selected = selected;
		}

		// Token: 0x06000C5C RID: 3164 RVA: 0x0000A11A File Offset: 0x0000831A
		public Item(GUIContent content, bool selected)
		{
			this.content = content;
			this.selected = selected;
		}

		// Token: 0x0400098E RID: 2446
		public GUIContent content;

		// Token: 0x0400098F RID: 2447
		public bool selected;

		// Token: 0x04000990 RID: 2448
		public bool clicked;
	}
}
