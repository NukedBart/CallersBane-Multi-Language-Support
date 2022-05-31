using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001CD RID: 461
public class TypeDistTable
{
	// Token: 0x06000E9C RID: 3740 RVA: 0x0000BA94 File Offset: 0x00009C94
	public TypeDistTable(GUISkin skin, Rect rect, GraphData<ResourceType> data)
	{
		this.rect = rect;
		this.data = data;
		this.skin = skin;
	}

	// Token: 0x06000E9D RID: 3741 RVA: 0x00062568 File Offset: 0x00060768
	public void Draw()
	{
		GUI.skin = this.skin;
		Dictionary<ResourceType, int> dictionary = new Dictionary<ResourceType, int>();
		int fontSize = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = Screen.height / 50;
		float num = this.rect.height / 6f;
		float num2 = this.rect.width / 7f;
		Texture2D texture2D = ResourceManager.LoadTexture("ChatUI/white");
		for (int i = 0; i < 5; i++)
		{
			GUI.color = new Color(1f, 1f, 1f, 0.3f);
			GUI.DrawTexture(new Rect(this.rect.x, this.rect.y + (float)(i + 1) * num, this.rect.width, 1f), texture2D);
			GUI.color = Color.white;
		}
		int num3 = 0;
		foreach (ResourceType resourceType in (ResourceType[])Enum.GetValues(typeof(ResourceType)))
		{
			if (resourceType.isResource())
			{
				float num4 = this.rect.x + num2 * 2f + num2 * (float)resourceType;
				Rect rect;
				rect..ctor(num4, this.rect.y + (float)num3 * num, num2, num);
				GUI.Label(rect, resourceType.ToString().Substring(0, 1));
				int num5 = 2;
				int num6 = 1;
				Color gui = ResourceColor.getGui(resourceType);
				GUI.color = gui;
				Rect rect2;
				rect2..ctor(num4 + (float)num5, this.rect.y + num, num2 - (float)(num5 * 2), 4f * num);
				GUI.color = new Color(gui.r, gui.g, gui.b, gui.a * 0.25f);
				GUI.DrawTexture(new Rect(rect2.x + (float)num6, rect2.y + (float)num6, rect2.width - (float)(num6 * 2), rect2.height - (float)(num6 * 2) + 1f), texture2D);
				GUI.color = Color.white;
			}
		}
		Rect rect3;
		rect3..ctor(this.rect.x + num2 * 2f + num2 * 4f, this.rect.y + (float)num3 * num, num2, num);
		GUI.Label(rect3, "TOTAL");
		num3++;
		TextAnchor alignment = GUI.skin.label.alignment;
		foreach (KeyValuePair<int, GraphDataSet<ResourceType>> keyValuePair in this.data)
		{
			CardType.Kind key = (CardType.Kind)keyValuePair.Key;
			Rect rect4;
			rect4..ctor(this.rect.x, this.rect.y + (float)num3 * num, num2 * 2f, num);
			GUI.skin.label.alignment = 3;
			string text = (key.ToString().Length <= 9) ? (key.ToString() + "S") : (key.ToString().Substring(0, 7) + ".");
			GUI.Label(rect4, text);
			GUI.skin.label.alignment = alignment;
			int num7 = 0;
			foreach (KeyValuePair<ResourceType, int> keyValuePair2 in keyValuePair.Value)
			{
				if (!dictionary.ContainsKey(keyValuePair2.Key))
				{
					dictionary.Add(keyValuePair2.Key, 0);
				}
				Dictionary<ResourceType, int> dictionary3;
				Dictionary<ResourceType, int> dictionary2 = dictionary3 = dictionary;
				ResourceType key2;
				ResourceType resourceType2 = key2 = keyValuePair2.Key;
				int num8 = dictionary3[key2];
				dictionary2[resourceType2] = num8 + keyValuePair2.Value;
				int key3 = (int)keyValuePair2.Key;
				Rect rect5;
				rect5..ctor(this.rect.x + num2 * 2f + num2 * (float)key3, this.rect.y + (float)num3 * num, num2, num);
				GUI.Label(rect5, string.Empty + keyValuePair2.Value);
				num7 += keyValuePair2.Value;
			}
			Rect rect6;
			rect6..ctor(this.rect.x + num2 * 2f + num2 * 4f, this.rect.y + (float)num3 * num, num2, num);
			GUI.Label(rect6, string.Empty + num7);
			num3++;
		}
		int num9 = 0;
		foreach (KeyValuePair<ResourceType, int> keyValuePair3 in dictionary)
		{
			Rect rect7;
			rect7..ctor(this.rect.x + num2 * 2f + num2 * (float)keyValuePair3.Key, this.rect.y + (float)num3 * num, num2, num);
			GUI.Label(rect7, string.Empty + keyValuePair3.Value);
			num9 += keyValuePair3.Value;
		}
		Rect rect8;
		rect8..ctor(this.rect.x, this.rect.y + 5f * num, num2 * 2f, num);
		GUI.skin.label.alignment = 3;
		GUI.Label(rect8, "TOTAL");
		GUI.skin.label.alignment = alignment;
		Rect rect9;
		rect9..ctor(this.rect.x + num2 * 2f + num2 * 4f, this.rect.y + 5f * num, num2, num);
		GUI.Label(rect9, string.Empty + num9);
		GUI.skin.label.fontSize = fontSize;
	}

	// Token: 0x04000B4B RID: 2891
	private Rect rect;

	// Token: 0x04000B4C RID: 2892
	private GraphData<ResourceType> data;

	// Token: 0x04000B4D RID: 2893
	private GUISkin skin;
}
