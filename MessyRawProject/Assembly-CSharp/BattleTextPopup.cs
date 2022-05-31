using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000052 RID: 82
public class BattleTextPopup : BattlePopup
{
	// Token: 0x060003AB RID: 939 RVA: 0x000047EA File Offset: 0x000029EA
	public void init(string text)
	{
		base.init();
		this.text = text;
		this.delay = 0.5f;
		this.fontShader = ResourceManager.LoadShader("Scrolls/GUI/3DTextShader");
	}

	// Token: 0x060003AC RID: 940 RVA: 0x0002E068 File Offset: 0x0002C268
	protected override void setup()
	{
		Font font = (Font)ResourceManager.Load("Fonts/HoneyMeadBB_bold");
		this.addText(this.text, font, Color.white, Vector3.zero);
		this.addText(this.text, font, Color.black, new Vector3(0.01f, -0.01f, 0.01f));
	}

	// Token: 0x060003AD RID: 941 RVA: 0x0002E0C4 File Offset: 0x0002C2C4
	private void addText(string text, Font font, Color color, Vector3 offset)
	{
		GameObject gameObject = new GameObject("TextMesh");
		gameObject.name = "bpt_" + text;
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		TextMesh textMesh = gameObject.AddComponent<TextMesh>();
		textMesh.anchor = 7;
		textMesh.lineSpacing = 0.85f;
		textMesh.text = text;
		textMesh.font = font;
		textMesh.fontSize = 26;
		textMesh.characterSize = 2.58f;
		meshRenderer.material = font.material;
		meshRenderer.material.shader = this.fontShader;
		meshRenderer.material.color = color;
		meshRenderer.material.renderQueue = 95500;
		UnityUtil.addChild(base.gameObject, gameObject);
		gameObject.transform.localScale = Vector2.one * 0.04f;
		gameObject.transform.localPosition = offset;
		this.objects.Add(gameObject);
	}

	// Token: 0x060003AE RID: 942 RVA: 0x0002E1AC File Offset: 0x0002C3AC
	protected override void update(float t)
	{
		base.update(t);
		if (t < 0.85f)
		{
			return;
		}
		foreach (GameObject gameObject in this.objects)
		{
			Vector3 vector = gameObject.transform.localScale;
			vector *= 1f + 0.5f * Time.deltaTime;
			gameObject.transform.localScale = vector;
		}
	}

	// Token: 0x0400025E RID: 606
	private List<TextMesh> texts = new List<TextMesh>();

	// Token: 0x0400025F RID: 607
	private Shader fontShader;

	// Token: 0x04000260 RID: 608
	private string text;
}
