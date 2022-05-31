using System;
using Gui;
using UnityEngine;

// Token: 0x020000D8 RID: 216
internal class NumberBox
{
	// Token: 0x0600074F RID: 1871 RVA: 0x00041180 File Offset: 0x0003F380
	public NumberBox(Gui3D gui, Rect rect, Texture2D icon, Color modifierColor, bool flipIconSide)
	{
		this.gui = gui;
		this.outer = rect;
		this.icon = icon;
		this.modifierColor = modifierColor;
		this.flipIconSide = flipIconSide;
		float num = this.outer.height * 1.2f;
		float num2 = num * (float)icon.width / (float)icon.height;
		float num3 = (num2 - this.outer.height) / 2f;
		this.iconRect = new Rect((!flipIconSide) ? (this.outer.x - num3) : (this.outer.xMax - this.outer.height - num3), this.outer.y - (num - this.outer.height) / 2f, num2, num);
	}

	// Token: 0x06000750 RID: 1872 RVA: 0x00006851 File Offset: 0x00004A51
	public void SetSelected(bool selected)
	{
		this.selected = selected;
	}

	// Token: 0x06000751 RID: 1873 RVA: 0x0000685A File Offset: 0x00004A5A
	public void SetAlwaysShowBoth(bool alwaysShowBoth)
	{
		if (this.second == -1)
		{
			this.second = 0;
		}
		this.alwaysShowBoth = alwaysShowBoth;
	}

	// Token: 0x06000752 RID: 1874 RVA: 0x00006876 File Offset: 0x00004A76
	public Rect GetIconRect()
	{
		return this.iconRect;
	}

	// Token: 0x06000753 RID: 1875 RVA: 0x0000687E File Offset: 0x00004A7E
	public void SetFirst(int first)
	{
		this.first = first;
	}

	// Token: 0x06000754 RID: 1876 RVA: 0x00006887 File Offset: 0x00004A87
	public void SetTemporaryModifier(int firstOffset)
	{
		this.firstOffset = firstOffset;
	}

	// Token: 0x06000755 RID: 1877 RVA: 0x00006890 File Offset: 0x00004A90
	public void SetSecond(int second)
	{
		this.second = second;
	}

	// Token: 0x06000756 RID: 1878 RVA: 0x00006899 File Offset: 0x00004A99
	public void SetBoth(int first, int second)
	{
		this.first = first;
		this.second = second;
	}

	// Token: 0x06000757 RID: 1879 RVA: 0x000068A9 File Offset: 0x00004AA9
	public int GetFirst()
	{
		return this.first;
	}

	// Token: 0x06000758 RID: 1880 RVA: 0x00041258 File Offset: 0x0003F458
	public void Draw()
	{
		Rect dst = this.outer;
		Rect dst2 = this.iconRect;
		float num = (!(this.resourceCounterGo != null)) ? -1f : this.resourceCounterGo.renderer.material.GetFloat("_Lerp");
		this.gui.DrawTexture(dst, ResourceManager.LoadTexture("BattleUI/battlegui_resourcebox"));
		if (num >= 0f)
		{
			this.gui.GetLastMaterial().SetFloat("_Lerp", num);
		}
		if (this.selected)
		{
			this.selectedAlpha += Time.deltaTime / 0.1f;
		}
		else
		{
			this.selectedAlpha -= Time.deltaTime / 0.1f;
		}
		this.selectedAlpha = Mathf.Clamp01(this.selectedAlpha);
		if (this.selectedAlpha > 0f)
		{
			Color color = this.gui.GetColor();
			this.gui.SetColor(new Color(1f, 1f, 1f, this.selectedAlpha));
			this.gui.DrawTexture(dst, ResourceManager.LoadTexture("BattleUI/battlegui_resourcebox_highlight"));
			if (num >= 0f)
			{
				this.gui.GetLastMaterial().SetFloat("_Lerp", num);
			}
			this.gui.SetColor(color);
		}
		if (this.resourceCounterGo != null)
		{
			this.gui.DrawTexture(dst2, this.icon);
			if (num >= 0f)
			{
				this.gui.GetLastMaterial().SetFloat("_Lerp", num);
			}
		}
		else
		{
			this.gui.DrawTexture(dst2, this.icon);
		}
		char[] array = Convert.ToString(this.first + this.firstOffset).ToCharArray();
		char[] array2 = Convert.ToString(this.second).ToCharArray();
		char[] array3 = Convert.ToString(this.firstOffset).ToCharArray();
		float height = dst.height;
		float num2 = dst.height / 2.5f;
		float num3 = dst.height * 0.6f;
		if (this.alwaysShowBoth)
		{
			num2 *= 0.85f;
			num3 *= 0.85f;
		}
		else
		{
			float num4 = (array2.Length < 2) ? 1f : 0.85f;
			num2 *= num4;
			num3 *= num4;
		}
		bool flag = this.second >= 0 || this.alwaysShowBoth;
		float num5 = (!this.flipIconSide) ? (dst.x + height) : dst.x;
		float num6 = dst.width - height;
		float num7 = (!flag) ? (num6 / 2f) : (num6 / 4f);
		float num8 = num6 / 2f;
		float num9 = num6 * 3f / 4f;
		float num10 = -num6 / 4f;
		float num11 = dst.y + (dst.height - num3) / 2f;
		float num12 = dst.height * 0.05f;
		if (array.Length > array2.Length)
		{
			num7 += num2 * 0.5f;
			num8 += num2 * 0.5f;
			num9 += num2 * 0.5f;
		}
		if (array2.Length > array.Length)
		{
			num7 -= num2 * 0.5f;
			num8 -= num2 * 0.5f;
			num9 -= num2 * 0.5f;
		}
		string text = (!this.alwaysShowBoth) ? "BattleUI/battlegui_number_" : "BattleUI/battlegui_number_d_";
		for (int i = 0; i < array.Length; i++)
		{
			float num13 = ((float)(-(float)array.Length) / 2f + (float)i) * num2;
			Rect dst3;
			dst3..ctor(num5 + num7 + num13, num11 + ((!this.alwaysShowBoth) ? 0f : (-num12)), num2, num3);
			Texture2D tex = ResourceManager.LoadTexture(text + array[i]);
			this.gui.DrawTexture(dst3, tex);
		}
		if (this.firstOffset != 0)
		{
			this.gui.DrawTexture(this.iconRect, ResourceManager.LoadTexture("BattleUI/battlegui_icon_overlay"));
			this.gui.GetLastMaterial().color = new Color(1f, 1f, 1f, 0.7f);
			for (int j = 0; j < array3.Length; j++)
			{
				float num14 = ((float)(-(float)array3.Length) / 2f + (float)j) * num2;
				Rect dst4;
				dst4..ctor(num5 + num10 + num14, num11 + ((!this.alwaysShowBoth) ? 0f : (-num12)), num2, num3);
				Texture2D tex2 = ResourceManager.LoadTexture(text + array3[j]);
				this.gui.DrawTexture(dst4, tex2);
				this.gui.GetLastMaterial().color = this.modifierColor;
			}
		}
		if (flag)
		{
			Texture2D tex3 = ResourceManager.LoadTexture("BattleUI/battlegui_number_slash");
			this.gui.DrawTexture(new Rect(num5 + num8 - num2 / 2f, num11, num2, num3), tex3);
			for (int k = 0; k < array2.Length; k++)
			{
				float num15 = ((float)(-(float)array.Length) / 2f + (float)k) * num2;
				Rect dst5;
				dst5..ctor(num5 + num9 + num15, num11 + ((!this.alwaysShowBoth) ? 0f : num12), num2, num3);
				Texture2D tex4 = ResourceManager.LoadTexture(text + array2[k]);
				this.gui.DrawTexture(dst5, tex4);
			}
		}
	}

	// Token: 0x04000569 RID: 1385
	private Gui3D gui;

	// Token: 0x0400056A RID: 1386
	private Rect outer;

	// Token: 0x0400056B RID: 1387
	private Rect iconRect;

	// Token: 0x0400056C RID: 1388
	private Texture2D icon;

	// Token: 0x0400056D RID: 1389
	private bool flipIconSide;

	// Token: 0x0400056E RID: 1390
	private bool alwaysShowBoth;

	// Token: 0x0400056F RID: 1391
	private bool selected;

	// Token: 0x04000570 RID: 1392
	private float selectedAlpha;

	// Token: 0x04000571 RID: 1393
	private int first;

	// Token: 0x04000572 RID: 1394
	private int second = -1;

	// Token: 0x04000573 RID: 1395
	private int firstOffset;

	// Token: 0x04000574 RID: 1396
	private Color modifierColor;

	// Token: 0x04000575 RID: 1397
	public GameObject resourceCounterGo;
}
