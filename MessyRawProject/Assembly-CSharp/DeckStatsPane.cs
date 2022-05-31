using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020001CB RID: 459
public class DeckStatsPane : MonoBehaviour
{
	// Token: 0x06000E90 RID: 3728 RVA: 0x00061D90 File Offset: 0x0005FF90
	public void Init()
	{
		this.regularUI = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		this.closeButtonSkin = (GUISkin)ResourceManager.Load("_GUISkins/CloseButton");
		this.rect = new Rect((float)(-(float)Screen.height) * 0.05f, (float)Screen.height * 0.255f, (float)Screen.height * 0.42f, (float)Screen.height * 0.58f);
		this.xShow = this.rect.x;
		this.rect.x = (this.xHidden = -this.rect.width);
		this.manaCurveGraph = new Graph(this.regularUI, "Resource cost", "Quantity");
	}

	// Token: 0x06000E91 RID: 3729 RVA: 0x0000BA42 File Offset: 0x00009C42
	public void Toggle()
	{
		base.StopCoroutine("Move");
		base.StartCoroutine("Move", !this.isShowing);
		if (!this.isShowing)
		{
			this.isShowing = true;
		}
	}

	// Token: 0x06000E92 RID: 3730 RVA: 0x00061E50 File Offset: 0x00060050
	private IEnumerator Move(bool moveIn)
	{
		float from = this.rect.x;
		float to = (!moveIn) ? this.xHidden : this.xShow;
		float duration = 0.5f;
		float t = 0f;
		float timeStarted = Time.time;
		while (t <= 1f)
		{
			t = (Time.time - timeStarted) / duration;
			this.rect.x = from + (to - from) * (t * t * (3f - 2f * t));
			yield return null;
		}
		this.rect.x = to;
		if (!moveIn)
		{
			this.isShowing = false;
		}
		yield break;
	}

	// Token: 0x06000E93 RID: 3731 RVA: 0x00061E7C File Offset: 0x0006007C
	public void UpdateGraphs(List<Card> cards)
	{
		GraphData<ResourceType> graphData = new GraphData<ResourceType>();
		this.typeDistribution = new GraphData<ResourceType>();
		this.typeDistribution[3] = new GraphDataSet<ResourceType>();
		this.typeDistribution[4] = new GraphDataSet<ResourceType>();
		this.typeDistribution[2] = new GraphDataSet<ResourceType>();
		this.typeDistribution[1] = new GraphDataSet<ResourceType>();
		graphData[1] = new GraphDataSet<ResourceType>();
		graphData[2] = new GraphDataSet<ResourceType>();
		graphData[3] = new GraphDataSet<ResourceType>();
		graphData[4] = new GraphDataSet<ResourceType>();
		graphData[5] = new GraphDataSet<ResourceType>();
		foreach (Card card in cards)
		{
			GraphDataSet<ResourceType> graphDataSet2;
			GraphDataSet<ResourceType> graphDataSet = graphDataSet2 = graphData[card.getCostTotal()];
			ResourceType resourceType;
			ResourceType key = resourceType = card.getResourceType();
			int num = graphDataSet2[resourceType];
			graphDataSet[key] = num + 1;
			GraphDataSet<ResourceType> graphDataSet4;
			GraphDataSet<ResourceType> graphDataSet3 = graphDataSet4 = this.typeDistribution[(int)card.getPieceKind()];
			ResourceType key2 = resourceType = card.getResourceType();
			num = graphDataSet4[resourceType];
			graphDataSet3[key2] = num + 1;
		}
		this.manaCurveGraph.UpdateData(graphData);
	}

	// Token: 0x06000E94 RID: 3732 RVA: 0x00061FC4 File Offset: 0x000601C4
	private void OnGUI()
	{
		if (!this.isShowing)
		{
			return;
		}
		new ScrollsFrame(this.rect).AddNinePatch(ScrollsFrame.Border.LIGHT_CURVED, NinePatch.Patches.CENTER).Draw();
		GUI.skin = this.regularUI;
		for (int i = 0; i < 5; i++)
		{
			ResourceType[] showResourceTypes = null;
			string text = string.Empty;
			Texture2D texture2D = ResourceManager.LoadTexture("ChatUI/white");
			ResourceType resourceType = ResourceType.NONE;
			switch (i)
			{
			case 0:
				text = "All";
				showResourceTypes = new ResourceType[]
				{
					ResourceType.GROWTH,
					ResourceType.ORDER,
					ResourceType.ENERGY,
					ResourceType.DECAY
				};
				break;
			case 1:
				text = "Growth";
				resourceType = ResourceType.GROWTH;
				break;
			case 2:
				text = "Order";
				resourceType = ResourceType.ORDER;
				break;
			case 3:
				text = "Energy";
				resourceType = ResourceType.ENERGY;
				break;
			case 4:
				text = "Decay";
				resourceType = ResourceType.DECAY;
				break;
			}
			if (resourceType.isResource())
			{
				showResourceTypes = new ResourceType[]
				{
					resourceType
				};
			}
			float num = (float)Screen.height * 0.065f;
			float num2 = (float)Screen.height * 0.025f;
			float num3 = this.rect.width - num - num2;
			int fontSize = GUI.skin.button.fontSize;
			GUI.skin.button.fontSize = Screen.height / 50;
			Rect rect;
			rect..ctor(this.rect.x + num + num3 / 5f * (float)i, this.rect.y + (float)Screen.height * 0.023f, num3 / 5f - 5f, (float)Screen.height * 0.035f);
			Color gui = ResourceColor.getGui(resourceType);
			if (resourceType == this.manaCurveGraph.HoveredResource && resourceType.isResource())
			{
				GUI.color = Color.white;
			}
			else
			{
				GUI.color = new Color(gui.r, gui.g, gui.b, gui.a * 0.8f);
			}
			GUI.DrawTexture(new Rect(rect.x, rect.yMax + 3f, rect.width, 4f), texture2D);
			GUI.color = Color.white;
			if (GUI.Button(rect, text))
			{
				App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
				this.manaCurveGraph.SetShowResourceTypes(showResourceTypes);
			}
			GUI.skin.button.fontSize = fontSize;
		}
		float num4 = (float)Screen.height * 0.035f;
		float num5 = (float)Screen.height * 0.1f;
		int fontSize2 = GUI.skin.button.fontSize;
		GUI.skin.button.fontSize = Screen.height / 40;
		if (GUI.Button(new Rect(this.rect.x + (float)Screen.height * 0.015f + this.rect.width / 2f - num5 / 2f, this.rect.yMax - num4 - (float)Screen.height * 0.015f, num5, num4), "Hide"))
		{
			App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
			this.Toggle();
		}
		GUI.skin.button.fontSize = fontSize2;
		if (this.manaCurveGraph != null)
		{
			Rect rect2;
			rect2..ctor(this.rect.x + (float)Screen.height * 0.065f, this.rect.y + (float)Screen.height * 0.025f + this.rect.height * 0.15f, this.rect.width - (float)Screen.height * 0.09f, this.rect.height * 0.35f);
			this.manaCurveGraph.Draw(rect2);
		}
		Rect rect3;
		rect3..ctor(this.rect.x + (float)Screen.height * 0.065f, this.rect.y + (float)Screen.height * 0.025f + this.rect.height * 0.6f, this.rect.width - (float)Screen.height * 0.09f, this.rect.height * 0.25f);
		new TypeDistTable(this.regularUI, rect3, this.typeDistribution).Draw();
	}

	// Token: 0x06000E95 RID: 3733 RVA: 0x0000BA7B File Offset: 0x00009C7B
	public bool IsOpen()
	{
		return this.isShowing;
	}

	// Token: 0x04000B39 RID: 2873
	private Rect rect;

	// Token: 0x04000B3A RID: 2874
	private float xShow;

	// Token: 0x04000B3B RID: 2875
	private float xHidden;

	// Token: 0x04000B3C RID: 2876
	private bool isShowing;

	// Token: 0x04000B3D RID: 2877
	private GUISkin regularUI;

	// Token: 0x04000B3E RID: 2878
	private GUISkin closeButtonSkin;

	// Token: 0x04000B3F RID: 2879
	private GraphData<ResourceType> typeDistribution = new GraphData<ResourceType>();

	// Token: 0x04000B40 RID: 2880
	private Graph manaCurveGraph;
}
