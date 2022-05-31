using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200018E RID: 398
public class ResourceFilterGroup : ButtonGroup
{
	// Token: 0x06000C5D RID: 3165 RVA: 0x0005725C File Offset: 0x0005545C
	public ResourceFilterGroup(float x, float y, float yspace, ICollection<ResourceType> resources) : base(true, x, y, yspace, "Filter")
	{
		foreach (ResourceType type in resources)
		{
			this.addItem(type);
		}
	}

	// Token: 0x06000C5E RID: 3166 RVA: 0x0000A130 File Offset: 0x00008330
	private void addItem(ResourceType type)
	{
		base.addItem(string.Empty, true);
		this.counts.Add(string.Empty);
		this.resources.Add(type);
	}

	// Token: 0x06000C5F RID: 3167 RVA: 0x000572D4 File Offset: 0x000554D4
	public void setResourceCount(ResourceType type, int count)
	{
		int index = this.getIndex(type);
		if (index >= 0)
		{
			this.counts[index] = count.ToString();
		}
	}

	// Token: 0x06000C60 RID: 3168 RVA: 0x00057304 File Offset: 0x00055504
	public bool isSelected(ResourceType type)
	{
		int index = this.getIndex(type);
		return index >= 0 && base.isSelected(index);
	}

	// Token: 0x06000C61 RID: 3169 RVA: 0x0005732C File Offset: 0x0005552C
	private int getIndex(ResourceType type)
	{
		for (int i = 0; i < this.resources.Count; i++)
		{
			if (this.resources[i] == type)
			{
				return i;
			}
		}
		return -1;
	}

	// Token: 0x06000C62 RID: 3170 RVA: 0x0005736C File Offset: 0x0005556C
	public override bool renderItem(int index)
	{
		bool flag = base.renderItem(index);
		float num = this.y + this.yoffset + (float)index * this.yspace;
		float num2 = 0.027f * this.ih;
		Rect r;
		r..ctor(this.x + 8f * num2, num, 32f * num2, 34f * num2);
		Color color = GUI.color;
		GUI.color = ((!flag) ? new Color(1f, 1f, 1f, 0.5f) : Color.white);
		int fontSize = GUI.skin.label.fontSize;
		GUI.Label(GeomUtil.getTranslated(r, 0f, 2f * num2), ResourceManager.LoadTexture(this.resources[index].battleIconFilename()));
		GUI.color = color;
		GUI.skin.label.fontSize = GUIUtil.getFittingFontSize(this.counts[index], GUI.skin.label, 32f * num2);
		GUI.Label(GeomUtil.getTranslated(r, 30f * num2, 0.5f * num2), this.counts[index]);
		GUI.skin.label.fontSize = fontSize;
		return flag;
	}

	// Token: 0x04000991 RID: 2449
	private List<ResourceType> resources = new List<ResourceType>();

	// Token: 0x04000992 RID: 2450
	private List<string> counts = new List<string>();
}
