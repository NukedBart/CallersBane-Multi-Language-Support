using System;
using UnityEngine;

// Token: 0x0200007E RID: 126
public class SpeechBubble : MonoBehaviour
{
	// Token: 0x060004E4 RID: 1252 RVA: 0x00005224 File Offset: 0x00003424
	static SpeechBubble()
	{
		SpeechBubble.FontTextBold.material.color = Color.black;
	}

	// Token: 0x060004E5 RID: 1253 RVA: 0x00035B30 File Offset: 0x00033D30
	private void Awake()
	{
		this.root = new GameObject("speechBubble");
		this.gAll = UnityUtil.addChild(this.root, new GameObject());
		this.gBubble = UnityUtil.addChild(this.gAll, PrimitiveFactory.createTexturedPlane("BattleMode/Flavor/bubble", true));
		this.gAll.transform.localEulerAngles = new Vector3(90f, 180f, 0f);
		this.gAll.transform.localScale = new Vector3(1f, 1f, 0.8f) * 0.1f;
		this.gAll.transform.localPosition = new Vector3(0.6f, 1.1f, 0.15f);
		this.gText = UnityUtil.addChild(this.gAll, CardView.createTextMesh(SpeechBubble.FontTextBold));
		this.gText.transform.localPosition = new Vector3(3.6f, 0f, -3.5f);
		this.gText.transform.localEulerAngles = new Vector3(90f, 180f, 0f);
		this.gText.transform.localScale = Vector3.one * 0.4f;
		this.text = this.gText.GetComponent<TextMesh>();
		this.startTime = Time.time;
		this.lifeTime = 3f;
		this.SetAlpha(0f);
		int renderQueue = 91602;
		UnityUtil.traverse(this.root, delegate(GameObject x)
		{
			if (x.renderer)
			{
				x.renderer.material.renderQueue = renderQueue;
			}
		});
		this.text.renderer.material.renderQueue = renderQueue + 1;
		this.alphaChanged = new FuncValueChangeDetector<float>(new Func<float>(this.calculateAlpha));
	}

	// Token: 0x060004E6 RID: 1254 RVA: 0x00035D08 File Offset: 0x00033F08
	private float calculateAlpha()
	{
		float num = Time.time - this.startTime;
		if (num < 0.2f)
		{
			return 5f * num;
		}
		if (num > this.lifeTime - 0.2f)
		{
			return Mathf.Max(0f, 5f * (this.lifeTime - num));
		}
		return 1f;
	}

	// Token: 0x060004E7 RID: 1255 RVA: 0x00035D68 File Offset: 0x00033F68
	public SpeechBubble init(string t)
	{
		this.root.name = "speechBubble_" + t;
		UnityUtil.addChild(base.gameObject, this.root);
		this.text.renderer.material.color = Color.black;
		this.text.text = t;
		return this;
	}

	// Token: 0x060004E8 RID: 1256 RVA: 0x00035DC4 File Offset: 0x00033FC4
	private void Update()
	{
		if (this.alphaChanged.IsChanged())
		{
			this.SetAlpha(this.calculateAlpha());
		}
		if (!this.isRemoved && Time.time - this.startTime > this.lifeTime)
		{
			Object.Destroy(this);
			this.isRemoved = true;
		}
	}

	// Token: 0x060004E9 RID: 1257 RVA: 0x0000524E File Offset: 0x0000344E
	private void OnDestroy()
	{
		Object.Destroy(this.root);
	}

	// Token: 0x060004EA RID: 1258 RVA: 0x00035E1C File Offset: 0x0003401C
	private void SetAlpha(float alpha)
	{
		Color color = new Color(1f, 1f, 1f, alpha);
		this.gAll.transform.localScale = new Vector3(1f, 1f, 0.8f) * 0.1f;
		UnityUtil.traverse(this.root, delegate(GameObject g)
		{
			this._SetAlphaColor(g, color);
		});
	}

	// Token: 0x060004EB RID: 1259 RVA: 0x00035E98 File Offset: 0x00034098
	private void _SetAlphaColor(GameObject g, Color color)
	{
		Renderer renderer = g.renderer;
		if (renderer == null)
		{
			return;
		}
		if (g == this.gText)
		{
			renderer.material.color = new Color(0f, 0f, 0f, color.a);
		}
		else
		{
			renderer.material.color = color;
		}
	}

	// Token: 0x04000353 RID: 851
	private GameObject root;

	// Token: 0x04000354 RID: 852
	private GameObject gAll;

	// Token: 0x04000355 RID: 853
	private GameObject gBubble;

	// Token: 0x04000356 RID: 854
	private GameObject gText;

	// Token: 0x04000357 RID: 855
	private TextMesh text;

	// Token: 0x04000358 RID: 856
	private float startTime;

	// Token: 0x04000359 RID: 857
	private float lifeTime;

	// Token: 0x0400035A RID: 858
	private IsValueChanged alphaChanged;

	// Token: 0x0400035B RID: 859
	private bool isRemoved;

	// Token: 0x0400035C RID: 860
	private static Font FontTextBold = (Font)ResourceManager.Load("Fonts/HoneyMeadBB_bold");
}
