using System;
using UnityEngine;

// Token: 0x0200008A RID: 138
internal class GUIClock
{
	// Token: 0x0600051C RID: 1308 RVA: 0x000372CC File Offset: 0x000354CC
	public GUIClock(GUISkin guiSkin)
	{
		this.guiSkin = guiSkin;
		this.m = new MockupCalc(1920, 1080);
	}

	// Token: 0x0600051D RID: 1309 RVA: 0x0003732C File Offset: 0x0003552C
	public void renderTime(int round, int seconds)
	{
		float num = (float)Screen.height * 0.08f;
		float num2 = num * 164f / 88f;
		GUI.DrawTexture(new Rect((float)(Screen.width / 2) - num2 / 2f, (float)Screen.height * 0.01f, num2, num), ResourceManager.LoadTexture("BattleUI/battlegui_timerbox"));
		float num3 = (float)Screen.height * 0.02f;
		float num4 = num3 * 49f / 19f;
		float num5 = (float)(Screen.width / 2) - num4;
		if (seconds >= 1000)
		{
			num5 -= num4 * 0.15f;
		}
		else if (seconds >= 100)
		{
			num5 -= num4 * 0.075f;
		}
		GUI.DrawTexture(new Rect(num5, (float)Screen.height * 0.042f, num4, num3), ResourceManager.LoadTexture("BattleUI/battlegui_timetext"));
		GUISkin skin = GUI.skin;
		GUI.skin = this.guiSkin;
		GUI.color = new Color(1f, 1f, 1f, 0.75f);
		Rect rect;
		rect..ctor((float)Screen.width * 0.5f - (float)Screen.height * 0.06f, (float)Screen.height * 0.095f, (float)Screen.height * 0.12f, (float)Screen.height * 0.035f);
		GUI.Box(rect, string.Empty);
		GUI.color = Color.white;
		int fontSize = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = Screen.height / 40;
		GUI.Label(rect, "Round " + round);
		GUI.skin.label.fontSize = fontSize;
		GUI.skin = skin;
		if (seconds < 0)
		{
			float num6 = (float)Screen.height * 0.028f;
			float num7 = num6 * 39f / 28f;
			GUI.DrawTexture(new Rect((float)(Screen.width / 2) + num7 * 0.25f, (float)Screen.height * 0.0375f, num7, num6), ResourceManager.LoadTexture("BattleUI/infinity"));
			return;
		}
		if ((float)seconds > this.last)
		{
			this.reset();
		}
		this.last = (float)seconds;
		string text = seconds.ToString();
		int length = text.Length;
		bool flag = true;
		if (seconds < 10 && (int)(8f * Time.time) % 2 == 0)
		{
			flag = false;
		}
		Rect rect2;
		rect2..ctor((float)(Screen.width / 2) + (float)Screen.height * 0.01f, (float)Screen.height * 0.035f, 0f, (float)Screen.height * 0.03f);
		if (seconds >= 1000)
		{
			GUIClock.rescale(ref rect2, 0.7f);
		}
		else if (seconds >= 100)
		{
			GUIClock.rescale(ref rect2, 0.9f);
		}
		Vector2 c = default(Vector2);
		for (int i = 0; i < length; i++)
		{
			int num8 = (int)(text.get_Chars(i) - '0');
			int num9 = this.kerning[num8];
			rect2.width = rect2.height * (float)num9 / 34f;
			if (length == 1)
			{
				rect2.x += rect2.width / 2f;
			}
			if (i == 0)
			{
				c..ctor(rect2.x + rect2.width * 1.05f, rect2.y + rect2.height / 2f);
			}
			if (flag)
			{
				GUI.DrawTexture(rect2, ResourceManager.LoadTexture("BattleMode/Clock/time__n_" + text.get_Chars(i)));
			}
			if (this.resetTime > 0f)
			{
				float num10 = 0.6f;
				float num11 = Time.time - this.resetTime;
				float num12 = num11 / num10;
				Color color = GUI.color;
				GUI.color = new Color(1f, 1f, 1f, 1f - num12);
				Rect rect3 = GeomUtil.scaleAround(rect2, c, 1f + 3f * num12);
				GUI.DrawTexture(rect3, ResourceManager.LoadTexture("BattleMode/Clock/time__n_" + text.get_Chars(i)));
				if (num11 >= num10)
				{
					this.resetTime = -1f;
				}
				GUI.color = color;
			}
			rect2.x += rect2.width * 1.1f;
		}
	}

	// Token: 0x0600051E RID: 1310 RVA: 0x00037794 File Offset: 0x00035994
	private static void rescale(ref Rect r, float s)
	{
		float num = (1f - s) * 0.5f;
		r.y += r.height * num;
		r.x -= r.height * 0.3f;
		r.width *= s;
		r.height *= s;
	}

	// Token: 0x0600051F RID: 1311 RVA: 0x0000545E File Offset: 0x0000365E
	public void reset()
	{
		this.resetTime = Time.time;
	}

	// Token: 0x04000397 RID: 919
	private GUISkin guiSkin;

	// Token: 0x04000398 RID: 920
	private MockupCalc m;

	// Token: 0x04000399 RID: 921
	private float last = 9999999f;

	// Token: 0x0400039A RID: 922
	private float resetTime = -1f;

	// Token: 0x0400039B RID: 923
	private int[] kerning = new int[]
	{
		24,
		14,
		23,
		21,
		23,
		20,
		21,
		22,
		23,
		21
	};
}
