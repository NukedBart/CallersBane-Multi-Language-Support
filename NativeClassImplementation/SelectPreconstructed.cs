using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020003CA RID: 970
public class SelectPreconstructed : AbstractCommListener
{
	// Token: 0x0600158C RID: 5516 RVA: 0x0000FBC0 File Offset: 0x0000DDC0
	private static string getTitleText(ResourceType resourceType)
	{
		return (resourceType != ResourceType.GROWTH) ? "Starter Deck Awarded" : "Your First Deck";
	}

	// Token: 0x0600158D RID: 5517 RVA: 0x000837B4 File Offset: 0x000819B4
	private void Start()
	{
		App.Communicator.addListener(this);
		base.StartCoroutine(this.initFadeIn(5));
		this.resourceButtons.Add(new SelectPreconstructed.ResourceButton(default(Rect), ResourceType.GROWTH, "Growth units use the power of nature to defeat their foes. Plants, beasts, and the raw brutality of kinsfolk can overwhelm opponents before they have time to organise their ranks.", "Cheap to play and quick to attack, Growth are unparalleled in aggressiveness."));
		this.resourceButtons.Add(new SelectPreconstructed.ResourceButton(default(Rect), ResourceType.ENERGY, "Energy are ruled by machine priests. They don't do subtle. If it's loud, explosive and volatile, they'll bring it to the battlefield.", "Energy wields great power to strike hard. Artillery and and damage spells keep your opponent in check."));
		this.resourceButtons.Add(new SelectPreconstructed.ResourceButton(default(Rect), ResourceType.ORDER, "Military might and tactical positioning are at the root of Order. Will you prioritise honour and hierarchy, or enjoy a more devious game, manipulating troops to give your army the edge?", "Order controls the battlefield with planning, timing, and perfectly executed attacks."));
		this.resourceButtons.Add(new SelectPreconstructed.ResourceButton(default(Rect), ResourceType.DECAY, "Nothing lives long in the marshes of Ilmire; it's a parasitic, gruesome world. Death is just the beginning for this poisonous faction rooted in the dark arts.", "Decay draws on death to prosper. Each death can bolster your power or harm your opponent."));
		RandomUtil.shuffle<SelectPreconstructed.ResourceButton>(this.resourceButtons);
		this.smallStyle = ((GUISkin)ResourceManager.Load("_GUISkins/SelectPrecon")).label;
		this.mediumStyle = new GUIStyle(this.smallStyle);
		this.largeStyle = new GUIStyle(this.smallStyle);
		this.titleStyle = new GUIStyle(this.smallStyle);
		this.titleStyle.alignment = 1;
		this.buttonSkin = (GUISkin)ResourceManager.Load("_GUISkins/Lobby");
		this.bgBlack = ResourceManager.LoadTexture("Login/black");
		this.messageType = App.SceneValues.selectPreconstructed.Pop();
		this.resourceType = SceneValues.SV_SelectPreconstructed.toResource(this.messageType);
		this.TitleText = SelectPreconstructed.getTitleText(this.resourceType);
		this.setupPositions();
	}

	// Token: 0x0600158E RID: 5518 RVA: 0x00083934 File Offset: 0x00081B34
	private IEnumerator initFadeIn(int numFrames)
	{
		DateTime startTime = DateTime.Now;
		for (int i = 0; i < numFrames; i++)
		{
			if ((DateTime.Now - startTime).TotalMilliseconds < 1000.0)
			{
				yield return new WaitForEndOfFrame();
			}
		}
		while (this.color.r < 1f)
		{
			float v = Mth.clamp(this.color.r + 0.1f, 0f, 1f);
			this.color.r = (this.color.g = (this.color.b = v));
			yield return new WaitForFixedUpdate();
		}
		yield return null;
		yield break;
	}

	// Token: 0x0600158F RID: 5519 RVA: 0x00083960 File Offset: 0x00081B60
	private void setupPositions()
	{
		this.lastWidth = Screen.width;
		this.lastHeight = Screen.height;
		this.smallStyle.fontSize = Screen.height / 36;
		this.mediumStyle.fontSize = Screen.height / 32;
		this.largeStyle.fontSize = Screen.height / 24;
		this.titleStyle.fontSize = Screen.height / 16;
		GUIStyle label = this.buttonSkin.label;
		int buttonFontSize = LobbyMenu.getButtonFontSize();
		this.buttonSkin.button.fontSize = buttonFontSize;
		label.fontSize = buttonFontSize;
		float num = (this.resourceButtons.Count <= 3) ? 0.9f : 0.98f;
		if (AspectRatio.now.isWider(AspectRatio._16_10))
		{
			float num2 = 1f + (AspectRatio.now.ratio - AspectRatio._16_10.ratio);
			this.bgRect = this.mockCalc.rAspectH(0f, 0f, 1920f * num2, 1080f * num2);
			this.bgRect.y = this.bgRect.y - (float)Screen.height * (num2 - 1f);
			num = ((this.resourceButtons.Count <= 3) ? 0.8f : 0.9f);
			this.smallStyle.fontSize = (int)(1.2f * (float)this.smallStyle.fontSize);
			this.mediumStyle.fontSize = (int)(1.2f * (float)this.mediumStyle.fontSize);
			this.largeStyle.fontSize = (int)(1.2f * (float)this.largeStyle.fontSize);
		}
		else if (AspectRatio.now.isNarrower(AspectRatio._4_3))
		{
			float num3 = 1f - (AspectRatio._4_3.ratio - AspectRatio.now.ratio);
			this.bgRect = this.mockCalc.rAspectH(0f, 0f, 1920f * num3, 1080f * num3);
		}
		else
		{
			this.bgRect = this.mockCalc.rAspectH(0f, 0f, 1920f, 1080f);
		}
		this.bgRect.x = (float)Screen.width - this.bgRect.width;
		float num4 = this.bgRect.x + 944f * this.bgRect.width / 1920f;
		this.leftRect = new Rect(0f, 0f, num4, (float)Screen.height);
		int num5 = 0;
		float num6 = num4 * num / (float)this.resourceButtons.Count;
		for (int i = 0; i < this.resourceButtons.Count; i++)
		{
			if (this.resourceButtons[i].resource == this.resourceType)
			{
				float num7 = (1f - num) * 0.5f * num4 + num6 * (float)i;
				Rect rect;
				rect..ctor(num7, (float)Screen.height * 0.23f, num6, num6);
				float num8 = 0.08f * (float)Screen.width / num6;
				if (num8 < 1f)
				{
					rect = GeomUtil.scaleCentered(rect, num8);
				}
				rect.height = rect.width * 259f / 222f;
				this.resourceButtons[i].rect = GeomUtil.getCentered(rect, this.leftRect, true, false);
				num5 = i;
				this.chooseBackground(this.resourceButtons[i].resource);
			}
		}
		float num9 = (float)Screen.height * 0.9f;
		this.chooseRect = this.mockCalc.prAspectH(new Vector2(0f, num9), 280f, 72f);
		this.chooseRect.x = (this.leftRect.width - this.chooseRect.width) / 2f;
		float num10 = 0f;
		foreach (SelectPreconstructed.ResourceButton b in this.resourceButtons)
		{
			float num11 = this.calculateTextHeight(b);
			if (num11 > num10)
			{
				num10 = num11;
			}
		}
		Rect rect2 = this.resourceButtons[num5].rect;
		float num12 = rect2.y + rect2.height * 1.2f;
		this.baseTextY = num12 + (this.chooseRect.y - num12 - num10) * 0.25f;
		GUIContent guicontent = new GUIContent(this.TitleText);
		while (this.titleStyle.fontSize > 8 && this.titleStyle.CalcSize(guicontent).x > this.leftRect.width * 0.95f)
		{
			this.titleStyle.fontSize--;
		}
	}

	// Token: 0x06001590 RID: 5520 RVA: 0x00083E6C File Offset: 0x0008206C
	private float calculateTextHeight(SelectPreconstructed.ResourceButton b)
	{
		List<Rect> textfieldRects = this.getTextfieldRects(b, 0f);
		return textfieldRects[textfieldRects.Count - 1].yMax;
	}

	// Token: 0x06001591 RID: 5521 RVA: 0x00083E9C File Offset: 0x0008209C
	private List<Rect> getTextfieldRects(SelectPreconstructed.ResourceButton b, float baseY)
	{
		List<Rect> list = new List<Rect>();
		Rect rect = GeomUtil.scaleCentered(new Rect(0f, baseY, this.leftRect.width, 10f), this.columnWidthMultiplier());
		rect.height = this.largeStyle.CalcHeight(new GUIContent(b.resource.ToString().ToUpper()), rect.width);
		list.Add(rect);
		rect.y += rect.height;
		rect.height = this.smallStyle.CalcHeight(new GUIContent(b.header), rect.width);
		list.Add(rect);
		rect.y += (float)Screen.height * 0.03f + rect.height;
		rect.height = this.mediumStyle.CalcHeight(new GUIContent("STRATEGY"), rect.width);
		list.Add(rect);
		rect.y += rect.height;
		rect.height = this.smallStyle.CalcHeight(new GUIContent(b.desc), rect.width);
		list.Add(rect);
		return list;
	}

	// Token: 0x06001592 RID: 5522 RVA: 0x00083FE0 File Offset: 0x000821E0
	private void FixedUpdate()
	{
		Vector2 screenMousePos = GUIUtil.getScreenMousePos();
		foreach (SelectPreconstructed.ResourceButton resourceButton in this.resourceButtons)
		{
			if (resourceButton == this.selectedButton)
			{
				resourceButton.bgAlpha += 0.15f;
			}
			else
			{
				resourceButton.bgAlpha -= 0.1f;
			}
			resourceButton.bgAlpha = Mth.clamp(resourceButton.bgAlpha, 0f, 1f);
			resourceButton.updateScale(resourceButton == this.selectedButton);
		}
		if (Screen.width != this.lastWidth || Screen.height != this.lastHeight)
		{
			this.setupPositions();
		}
	}

	// Token: 0x06001593 RID: 5523 RVA: 0x0000FBD7 File Offset: 0x0000DDD7
	private float columnWidthMultiplier()
	{
		return (!AspectRatio.now.isNarrower(AspectRatio._3_2)) ? 0.7f : 0.82f;
	}

	// Token: 0x06001594 RID: 5524 RVA: 0x000840C0 File Offset: 0x000822C0
	private void OnGUI()
	{
		GUI.depth = 5;
		GUI.color = Color.black;
		GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), this.bgBlack);
		GUI.color = this.color;
		this.drawBackgrounds();
		Rect rect;
		rect..ctor(0f, (float)Screen.height * 0.04f, this.leftRect.width, (float)Screen.height * 0.1f);
		rect.height = this.titleStyle.CalcHeight(new GUIContent(this.TitleText), rect.width);
		GUI.Label(rect, this.TitleText, this.titleStyle);
		Rect rect2 = GeomUtil.scaleCentered(new Rect(0f, (float)Screen.height * 0.1f, this.leftRect.width, 10f), this.columnWidthMultiplier());
		rect2.height = this.smallStyle.CalcHeight(new GUIContent(string.Empty), rect2.width);
		float num = Mathf.Lerp(rect.yMax, this.resourceButtons[0].rect.y, 0.42f);
		rect2.y = num - rect2.height / 2f;
		GUI.Label(rect2, string.Empty, this.smallStyle);
		foreach (SelectPreconstructed.ResourceButton resourceButton in this.resourceButtons)
		{
			if (resourceButton.OnGUI(resourceButton == this.selectedButton))
			{
				this.chooseBackground(resourceButton.resource);
			}
		}
		if (this.selectedButton != null)
		{
			List<Rect> textfieldRects = this.getTextfieldRects(this.selectedButton, this.baseTextY);
			if (textfieldRects.Count == 4)
			{
				this.largeStyle.normal.textColor = ResourceColor.get(this.selectedButton.resource);
				GUI.Label(textfieldRects[0], this.selectedButton.resource.ToString().ToUpper(), this.largeStyle);
				GUI.Label(textfieldRects[1], this.selectedButton.header, this.smallStyle);
				this.largeStyle.normal.textColor = ColorUtil.FromHex24(15248461u);
				GUI.Label(textfieldRects[2], "STRATEGY", this.mediumStyle);
				GUI.Label(textfieldRects[3], this.selectedButton.desc, this.smallStyle);
				string text = StringUtil.capitalize(this.selectedButton.resource.ToString());
				if (LobbyMenu.drawButton(this.chooseRect, "Ok, thanks!", this.buttonSkin))
				{
					App.Communicator.send(new RemoveMessageMessage(this.messageType));
					SceneLoader.loadScene("_HomeScreen");
				}
			}
		}
	}

	// Token: 0x06001595 RID: 5525 RVA: 0x000843C4 File Offset: 0x000825C4
	private void drawBackgrounds()
	{
		Color c = GUI.color;
		foreach (SelectPreconstructed.ResourceButton resourceButton in this.resourceButtons)
		{
			string filename = "ChooseStartDeck/choose_bg_" + resourceButton.resource.ToString().ToLower();
			Texture2D texture2D = ResourceManager.LoadTexture(filename);
			if (resourceButton.bgAlpha > 0f)
			{
				GUI.color = ColorUtil.GetWithAlpha(c, resourceButton.bgAlpha);
				GUI.DrawTexture(this.bgRect, texture2D);
			}
		}
		GUI.color = c;
	}

	// Token: 0x06001596 RID: 5526 RVA: 0x0008447C File Offset: 0x0008267C
	private void chooseBackground(ResourceType resource)
	{
		foreach (SelectPreconstructed.ResourceButton resourceButton in this.resourceButtons)
		{
			if (resourceButton.resource == resource)
			{
				this.selectedButton = resourceButton;
			}
		}
	}

	// Token: 0x06001597 RID: 5527 RVA: 0x000844E4 File Offset: 0x000826E4
	internal static GUIStyle createButtonSkin(ResourceType resource)
	{
		GUIStyle guistyle = new GUIStyle();
		string text = resource.ToString().ToLower();
		guistyle.normal.background = ResourceManager.LoadTexture("ChooseStartDeck/deck_" + text + "_2");
		guistyle.hover.background = ResourceManager.LoadTexture("ChooseStartDeck/deck_" + text + "_mouseover");
		guistyle.active.background = ResourceManager.LoadTexture("ChooseStartDeck/deck_" + text + "_mousedown");
		return guistyle;
	}

	// Token: 0x040012C3 RID: 4803
	private const string HeaderText = "";

	// Token: 0x040012C4 RID: 4804
	private Texture2D bgTexture;

	// Token: 0x040012C5 RID: 4805
	private Rect bgRect;

	// Token: 0x040012C6 RID: 4806
	private Rect leftRect;

	// Token: 0x040012C7 RID: 4807
	private Rect chooseRect;

	// Token: 0x040012C8 RID: 4808
	private MockupCalc mockCalc = new MockupCalc(1920, 1080);

	// Token: 0x040012C9 RID: 4809
	private GUIStyle titleStyle;

	// Token: 0x040012CA RID: 4810
	private GUIStyle smallStyle;

	// Token: 0x040012CB RID: 4811
	private GUIStyle mediumStyle;

	// Token: 0x040012CC RID: 4812
	private GUIStyle largeStyle;

	// Token: 0x040012CD RID: 4813
	private GUISkin buttonSkin;

	// Token: 0x040012CE RID: 4814
	private SelectPreconstructed.ResourceButton selectedButton;

	// Token: 0x040012CF RID: 4815
	private Texture2D bgBlack;

	// Token: 0x040012D0 RID: 4816
	private float baseTextY;

	// Token: 0x040012D1 RID: 4817
	private string TitleText = string.Empty;

	// Token: 0x040012D2 RID: 4818
	private MessageMessage.Type messageType;

	// Token: 0x040012D3 RID: 4819
	private ResourceType resourceType = ResourceType.NONE;

	// Token: 0x040012D4 RID: 4820
	private List<SelectPreconstructed.ResourceButton> resourceButtons = new List<SelectPreconstructed.ResourceButton>();

	// Token: 0x040012D5 RID: 4821
	private Color color = Color.black;

	// Token: 0x040012D6 RID: 4822
	private int lastWidth = -999;

	// Token: 0x040012D7 RID: 4823
	private int lastHeight = -999;

	// Token: 0x020003CB RID: 971
	private class ResourceButton
	{
		// Token: 0x06001598 RID: 5528 RVA: 0x0008456C File Offset: 0x0008276C
		public ResourceButton(Rect rect, ResourceType resource, string header, string desc)
		{
			this.rect = rect;
			this.resource = resource;
			this.guiStyle = SelectPreconstructed.createButtonSkin(resource);
			this.header = header;
			this.desc = desc;
			this.selectedGuiStyle = SelectPreconstructed.createButtonSkin(resource);
			GUIStyleState hover = this.selectedGuiStyle.hover;
			Texture2D background = this.selectedGuiStyle.normal.background;
			this.selectedGuiStyle.active.background = background;
			hover.background = background;
		}

		// Token: 0x06001599 RID: 5529 RVA: 0x000845F4 File Offset: 0x000827F4
		public bool OnGUI(bool isSelected)
		{
			Rect rect = GeomUtil.scaleCentered(this.rect, this.scale);
			Rect rect2 = GeomUtil.scaleCentered(this.rect, this.scale * 0.75f);
			rect2.height = rect2.width;
			rect.x -= rect.width * 0.6f;
			rect2.x += rect.width * 0.5f;
			rect2.y += rect.height * 0.1f;
			GUI.DrawTexture(rect2, ResourceManager.LoadTexture(this.resource.guiIconFilename()));
			return GUI.Button(rect, string.Empty, (!isSelected) ? this.guiStyle : this.selectedGuiStyle);
		}

		// Token: 0x0600159A RID: 5530 RVA: 0x000846C4 File Offset: 0x000828C4
		public void updateScale(bool selected)
		{
			float num = (!selected) ? -0.065f : 0.125f;
			this.scale = Mth.clamp(this.scale + num, 1f, 1.18f);
		}

		// Token: 0x040012D8 RID: 4824
		public Rect rect;

		// Token: 0x040012D9 RID: 4825
		public ResourceType resource;

		// Token: 0x040012DA RID: 4826
		public string header;

		// Token: 0x040012DB RID: 4827
		public string desc;

		// Token: 0x040012DC RID: 4828
		public float bgAlpha;

		// Token: 0x040012DD RID: 4829
		private float scale = 1f;

		// Token: 0x040012DE RID: 4830
		private GUIStyle guiStyle;

		// Token: 0x040012DF RID: 4831
		private GUIStyle selectedGuiStyle;
	}
}
