using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001CE RID: 462
public class Graph
{
	// Token: 0x06000E9E RID: 3742 RVA: 0x00062BBC File Offset: 0x00060DBC
	public Graph(GUISkin skin, string xHeader, string yHeader)
	{
		this.data = new GraphData<ResourceType>();
		this.skin = skin;
		this.xFrequency = 1;
		this.xHeader = xHeader;
		this.yHeader = yHeader;
		this.showResourceTypes = new List<ResourceType>(new ResourceType[]
		{
			ResourceType.GROWTH,
			ResourceType.ORDER,
			ResourceType.ENERGY,
			ResourceType.DECAY
		});
		this.UpdateData(this.data);
	}

	// Token: 0x170000B6 RID: 182
	// (get) Token: 0x06000E9F RID: 3743 RVA: 0x0000BAB1 File Offset: 0x00009CB1
	public ResourceType HoveredResource
	{
		get
		{
			return this.hoveredResource;
		}
	}

	// Token: 0x06000EA0 RID: 3744 RVA: 0x0000BAB9 File Offset: 0x00009CB9
	public void SetShowResourceTypes(ResourceType[] types)
	{
		this.showResourceTypes = new List<ResourceType>(types);
	}

	// Token: 0x06000EA1 RID: 3745 RVA: 0x00062C30 File Offset: 0x00060E30
	public void UpdateData(GraphData<ResourceType> data)
	{
		this.amountPerResource = new Dictionary<ResourceType, int>();
		this.data = data;
		foreach (int num in data.Keys)
		{
			if (num > this.xMax)
			{
				this.xMax = num;
			}
		}
		foreach (GraphDataSet<ResourceType> graphDataSet in data.Values)
		{
			int num2 = 0;
			foreach (KeyValuePair<ResourceType, int> keyValuePair in graphDataSet)
			{
				if (!this.amountPerResource.ContainsKey(keyValuePair.Key))
				{
					this.amountPerResource.Add(keyValuePair.Key, 0);
				}
				Dictionary<ResourceType, int> dictionary2;
				Dictionary<ResourceType, int> dictionary = dictionary2 = this.amountPerResource;
				ResourceType key;
				ResourceType resourceType = key = keyValuePair.Key;
				int num3 = dictionary2[key];
				dictionary[resourceType] = num3 + keyValuePair.Value;
			}
			foreach (int num4 in graphDataSet.Values)
			{
				num2 += num4;
			}
			if (num2 > this.yMax)
			{
				this.yMax = num2;
			}
		}
		this.yFrequency = ((this.yMax >= 6) ? 2 : 1);
	}

	// Token: 0x06000EA2 RID: 3746 RVA: 0x00062E30 File Offset: 0x00061030
	public void Draw(Rect rect)
	{
		GUI.skin = this.skin;
		int fontSize = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = Screen.height / 50;
		float num = (float)Screen.height * 0.002f;
		float num2 = (float)Screen.height * 0.02f;
		Texture2D texture2D = ResourceManager.LoadTexture("ChatUI/white");
		TextAnchor alignment = GUI.skin.label.alignment;
		GUI.skin.label.alignment = 3;
		GUI.Label(new Rect(rect.x, rect.y - (float)Screen.height * 0.03f, rect.width, (float)Screen.height * 0.03f), this.yHeader);
		GUI.skin.label.alignment = 4;
		GUI.Label(new Rect(rect.x + num2, rect.yMax, rect.width - num2, (float)Screen.height * 0.03f), this.xHeader);
		GUI.skin.label.alignment = alignment;
		int num3 = (this.xMax - this.xMin) / this.xFrequency;
		float num4 = (rect.width - num2 * 1.5f) / (float)(num3 + 1);
		int num5 = (this.yMax - this.yMin) / this.yFrequency;
		float num6 = (rect.height - num2) / ((float)num5 + 0.5f);
		GUI.skin.label.alignment = 1;
		for (int i = 1; i <= this.yMax / this.yFrequency; i++)
		{
			float num7 = (float)GUI.skin.label.fontSize;
			GUI.color = new Color(1f, 1f, 1f, 0.3f);
			GUI.DrawTexture(new Rect(rect.x + num2, rect.yMax - num2 - num6 * (float)i, rect.width - num2, 1f), texture2D);
			GUI.color = Color.white;
			GUI.Label(new Rect(rect.x, rect.yMax - num2 - num - num6 * (float)i - num7 / 2f, num2, num6), string.Empty + i * this.yFrequency);
		}
		bool flag = false;
		GUI.skin.label.alignment = 4;
		int num8 = 0;
		for (int j = this.xMin; j <= this.xMax; j += this.xFrequency)
		{
			float num9 = rect.x + num2 + num4 * ((float)num8 + 0.2f);
			float num10 = Mathf.Min(num4 * 0.8f, rect.width / 5f);
			GUI.Label(new Rect(num9 + (num4 - num10) / 2f, rect.yMax - num2 + num, num10, num2 - num), string.Empty + j);
			if (this.data.ContainsKey(j))
			{
				List<Graph.GraphPart> list = new List<Graph.GraphPart>();
				foreach (KeyValuePair<ResourceType, int> keyValuePair in this.data[j])
				{
					if (this.showResourceTypes.Contains(keyValuePair.Key))
					{
						float height = num6 * (float)keyValuePair.Value / (float)this.yFrequency;
						Color gui = ResourceColor.getGui(keyValuePair.Key);
						list.Add(new Graph.GraphPart
						{
							color = gui,
							height = height,
							type = keyValuePair.Key,
							frequencies = this.amountPerResource
						});
					}
				}
				list.Sort();
				float num11 = 0f;
				for (int k = list.Count - 1; k >= 0; k--)
				{
					Graph.GraphPart graphPart = list[k];
					float num12 = num11 + graphPart.height;
					int num13 = 1;
					Rect rect2;
					rect2..ctor(num9 + (num4 - num10) / 2f, rect.yMax - num2 - num12, num10, graphPart.height);
					GUI.color = new Color(graphPart.color.r * 0.3f, graphPart.color.g * 0.3f, graphPart.color.b * 0.3f, graphPart.color.a);
					GUI.DrawTexture(rect2, texture2D);
					bool flag2 = rect2.Contains(GUIUtil.getScreenMousePos());
					if (flag2)
					{
						flag = true;
					}
					if (flag2 || graphPart.type == this.hoveredResource)
					{
						this.hoveredResource = graphPart.type;
						GUI.color = new Color(0.9f, 0.9f, 0.9f, 1f);
					}
					else
					{
						GUI.color = new Color(graphPart.color.r * 0.8f, graphPart.color.g * 0.8f, graphPart.color.b * 0.8f, graphPart.color.a);
					}
					GUI.DrawTexture(new Rect(rect2.x + (float)num13, rect2.y + (float)num13, num10 - (float)(2 * num13), graphPart.height - (float)num13), texture2D);
					GUI.color = Color.white;
					num11 += graphPart.height;
				}
			}
			num8++;
		}
		if (!flag)
		{
			this.hoveredResource = ResourceType.NONE;
		}
		GUI.color = new Color(0.8f, 0.7f, 0.6f, 1f);
		GUI.DrawTexture(new Rect(rect.x + num2, rect.y, num, rect.height), texture2D);
		GUI.DrawTexture(new Rect(rect.x, rect.yMax - num - num2, rect.width, num), texture2D);
		GUI.color = Color.white;
		GUI.skin.label.alignment = alignment;
		GUI.skin.label.fontSize = fontSize;
	}

	// Token: 0x04000B4E RID: 2894
	private GraphData<ResourceType> data;

	// Token: 0x04000B4F RID: 2895
	private GUISkin skin;

	// Token: 0x04000B50 RID: 2896
	private int xMin = 1;

	// Token: 0x04000B51 RID: 2897
	private int yMin;

	// Token: 0x04000B52 RID: 2898
	private int xMax;

	// Token: 0x04000B53 RID: 2899
	private int yMax;

	// Token: 0x04000B54 RID: 2900
	private int xFrequency;

	// Token: 0x04000B55 RID: 2901
	private int yFrequency;

	// Token: 0x04000B56 RID: 2902
	private string xHeader;

	// Token: 0x04000B57 RID: 2903
	private string yHeader;

	// Token: 0x04000B58 RID: 2904
	private List<ResourceType> showResourceTypes;

	// Token: 0x04000B59 RID: 2905
	private Dictionary<ResourceType, int> amountPerResource;

	// Token: 0x04000B5A RID: 2906
	private ResourceType hoveredResource = ResourceType.NONE;

	// Token: 0x020001CF RID: 463
	private class GraphPart : IComparable
	{
		// Token: 0x06000EA4 RID: 3748 RVA: 0x0000BAC7 File Offset: 0x00009CC7
		public int CompareTo(object other)
		{
			return this.frequencies[this.type] - this.frequencies[((Graph.GraphPart)other).type];
		}

		// Token: 0x04000B5B RID: 2907
		public ResourceType type;

		// Token: 0x04000B5C RID: 2908
		public Color color;

		// Token: 0x04000B5D RID: 2909
		public float height;

		// Token: 0x04000B5E RID: 2910
		public Dictionary<ResourceType, int> frequencies;
	}
}
