using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x02000130 RID: 304
public class CardListPopup : MonoBehaviour
{
	// Token: 0x060009B5 RID: 2485 RVA: 0x0004B184 File Offset: 0x00049384
	private static string WriteDescription(Card card)
	{
		string text = string.Empty;
		if (card.level > 0)
		{
			string text2 = text;
			text = string.Concat(new object[]
			{
				text2,
				"Tier ",
				card.getTier(),
				", "
			});
		}
		return text + card.getRarityString() + ", " + StringUtil.capitalize(card.getPieceKind().ToString());
	}

	// Token: 0x060009B6 RID: 2486 RVA: 0x0004B1FC File Offset: 0x000493FC
	private void Start()
	{
		this.lobbySkin = (GUISkin)ResourceManager.Load("_GUISkins/Lobby");
		this.cardListPopupSkin = (GUISkin)ResourceManager.Load("_GUISkins/CardListPopup");
		this.cardListPopupGradientSkin = (GUISkin)ResourceManager.Load("_GUISkins/CardListPopupGradient");
		this.cardListPopupBigLabelSkin = (GUISkin)ResourceManager.Load("_GUISkins/CardListPopupBigLabel");
		this.cardListPopupLeftButtonSkin = (GUISkin)ResourceManager.Load("_GUISkins/CardListPopupLeftButton");
		this.cardListPopupTierBoxSkin = (GUISkin)ResourceManager.Load("_GUISkins/CardListPopupTierBox");
		this.sortButtonSelectedStyle = new GUIStyle(this.cardListPopupSkin.button);
		this.sortButtonSelectedStyle.normal = this.sortButtonSelectedStyle.hover;
	}

	// Token: 0x060009B7 RID: 2487 RVA: 0x00008283 File Offset: 0x00006483
	public CardListPopup SetSorter(IComparer<Card> sorter)
	{
		this.collectionSorter = sorter;
		this.refresh();
		return this;
	}

	// Token: 0x060009B8 RID: 2488 RVA: 0x00008293 File Offset: 0x00006493
	public CardListPopup SetToggleText(string text)
	{
		return this.SetToggleTexts(text, text);
	}

	// Token: 0x060009B9 RID: 2489 RVA: 0x0000829D File Offset: 0x0000649D
	public CardListPopup SetToggleTexts(string enabled, string disabled)
	{
		this.labelToggleEnabled = enabled;
		this.labelToggleDisabled = disabled;
		return this;
	}

	// Token: 0x060009BA RID: 2490 RVA: 0x000082AE File Offset: 0x000064AE
	private void OnDestroy()
	{
		if (this.eCards != null)
		{
			this.eCards.onUpdate -= this.onListUpdate;
		}
	}

	// Token: 0x060009BB RID: 2491 RVA: 0x0004B2B4 File Offset: 0x000494B4
	public CardListPopup Init(Rect screenRect, bool showFrame, bool selectable, EList<Card> cards, ICardListCallback callback, GUIContent buttonLeftContent, GUIContent buttonRightContent, bool leftButtonEnabled, bool rightButtonEnabled, bool leftHighlightable, bool rightHighlightable, Texture itemButtonTexture, bool clickableItems)
	{
		this.showFrame = showFrame;
		this.selectable = selectable;
		this.callback = callback;
		this.buttonLeftContent = buttonLeftContent;
		this.buttonRightContent = buttonRightContent;
		this.leftButtonEnabled = leftButtonEnabled;
		this.rightButtonEnabled = rightButtonEnabled;
		this.itemButtonTexture = itemButtonTexture;
		this.leftHighlightable = leftHighlightable;
		this.rightHighlightable = rightHighlightable;
		this.clickableItems = clickableItems;
		if (showFrame)
		{
			this.margins = new Vector4(12f, 12f, 12f, 12f + this.BOTTOM_MARGIN_EXTRA);
		}
		else
		{
			this.margins = new Vector4(0f, 0f, 0f, 0f + this.BOTTOM_MARGIN_EXTRA);
		}
		this.setRect(screenRect);
		this.SetCardList(cards);
		this.SetToggleText("Starter\nscrolls");
		return this;
	}

	// Token: 0x060009BC RID: 2492 RVA: 0x000082D2 File Offset: 0x000064D2
	public void SetCardDescriptionWriter(CardListPopup.CardDescriptionWriter writer)
	{
		this.descriptionWriter = writer;
	}

	// Token: 0x060009BD RID: 2493 RVA: 0x000082DB File Offset: 0x000064DB
	public void SetRightAdjustedCardDescriptionWriter(CardListPopup.CardDescriptionWriter writer)
	{
		this.rightAdjustedDescriptionWriter = writer;
	}

	// Token: 0x060009BE RID: 2494 RVA: 0x000082E4 File Offset: 0x000064E4
	public void SetItemButtonTexture(Texture itemButtonTexture)
	{
		this.itemButtonTexture = itemButtonTexture;
	}

	// Token: 0x060009BF RID: 2495 RVA: 0x000082ED File Offset: 0x000064ED
	public void SetItemButtonsEnabled(bool enabled)
	{
		this.itemButtonEnabled = enabled;
	}

	// Token: 0x060009C0 RID: 2496 RVA: 0x000082F6 File Offset: 0x000064F6
	public Rect getRect()
	{
		return this.screenRect;
	}

	// Token: 0x060009C1 RID: 2497 RVA: 0x000082FE File Offset: 0x000064FE
	private bool hasItemButtons()
	{
		return this.selectable || this.itemButtonTexture != null;
	}

	// Token: 0x060009C2 RID: 2498 RVA: 0x0004B38C File Offset: 0x0004958C
	public void setRect(Rect screenRect)
	{
		this.screenRect = screenRect;
		this.setupPositions();
		this.fieldHeight = 0.052f * (float)Screen.height;
		this.costIconSize = this.fieldHeight;
		this.costIconWidth = this.fieldHeight / 2f;
		this.costIconHeight = this.costIconWidth * 72f / 73f;
		this.cardHeight = this.fieldHeight * 0.72f;
		this.cardWidth = this.cardHeight * 100f / 75f;
		this.itemOffsetX = ((!this.hasItemButtons()) ? 0f : this.fieldHeight);
		this.labelX = this.itemOffsetX + this.cardWidth * 1.45f;
		this.labelsWidth = this.innerRect.width - this.labelX - this.costIconSize - this.scrollBarSize;
		this.maxCharsName = (int)(this.labelsWidth / 12f);
		this.maxCharsRK = (int)(this.labelsWidth / 10f);
		this.searchFieldStyle = ((GUISkin)ResourceManager.Load("_GUISkins/TextEntrySkin")).textField;
	}

	// Token: 0x060009C3 RID: 2499 RVA: 0x0004B4B8 File Offset: 0x000496B8
	private void setupPositions()
	{
		this.outerRect = this.screenRect;
		this.innerBGRect = new Rect(this.outerRect.x + this.margins.x, this.outerRect.y + this.margins.y, this.outerRect.width - (this.margins.x + this.margins.z), this.outerRect.height - (this.margins.y + this.margins.w));
		float num = 0.005f * (float)Screen.width;
		this.innerRect = new Rect(this.innerBGRect.x + num, this.innerBGRect.y + num, this.innerBGRect.width - 2f * num, this.innerBGRect.height - 2f * num);
		float num2 = this.BOTTOM_MARGIN_EXTRA - 0.01f * (float)Screen.height;
		this.buttonLeftRect = new Rect(this.innerRect.x + this.innerRect.width * 0.03f, this.innerBGRect.yMax + num2 * 0.28f, this.innerRect.width * 0.45f, num2);
		this.buttonRightRect = new Rect(this.innerRect.xMax - this.innerRect.width * 0.48f, this.innerBGRect.yMax + num2 * 0.28f, this.innerRect.width * 0.45f, num2);
		this.searchFieldRect = new Rect(this.buttonLeftRect);
		if (this.useSearchField)
		{
			float num3 = num2 * 1.28f;
			this.searchFieldRect.x = this.innerRect.x;
			this.searchFieldRect.width = this.innerRect.width;
			this.searchFieldRect.y = this.searchFieldRect.y - num3;
			this.innerBGRect.height = this.innerBGRect.height - num3;
			this.innerRect.height = this.innerRect.height - num3;
		}
		if (this.useLockedButton)
		{
			Rect full;
			full..ctor(this.searchFieldRect);
			this.searchFieldRect = GeomUtil.cropShare(full, new Rect(0.4f, 0f, 0.6f, 1f));
			this.lockedRect = GeomUtil.cropShare(full, new Rect(0f, 0f, 0.4f, 1f));
		}
	}

	// Token: 0x060009C4 RID: 2500 RVA: 0x0000831A File Offset: 0x0000651A
	public CardListPopup SetUseSearchField(bool use)
	{
		this.useSearchField = use;
		this.setupPositions();
		return this;
	}

	// Token: 0x060009C5 RID: 2501 RVA: 0x0000832A File Offset: 0x0000652A
	public CardListPopup SetUseLockedButton(bool use)
	{
		this.useLockedButton = use;
		this.setupPositions();
		return this;
	}

	// Token: 0x060009C6 RID: 2502 RVA: 0x0004B744 File Offset: 0x00049944
	public void SetCardList(EList<Card> cards)
	{
		if (this.eCards == cards)
		{
			return;
		}
		if (this.eCards != null)
		{
			this.eCards.onUpdate -= this.onListUpdate;
		}
		if (cards != null)
		{
			cards.onUpdate += this.onListUpdate;
		}
		this.eCards = cards;
	}

	// Token: 0x060009C7 RID: 2503 RVA: 0x0000833A File Offset: 0x0000653A
	public CardListPopup setOverridedCollectionForFiltering(EList<Card> collection)
	{
		this.overriddenCollection = collection;
		return this;
	}

	// Token: 0x060009C8 RID: 2504 RVA: 0x00008344 File Offset: 0x00006544
	public void onListUpdate(EList<Card> e)
	{
		this.cards = null;
	}

	// Token: 0x060009C9 RID: 2505 RVA: 0x0000834D File Offset: 0x0000654D
	private void refreshIfNeeded()
	{
		if (this.cards == null)
		{
			this.refresh();
		}
	}

	// Token: 0x060009CA RID: 2506 RVA: 0x0004B7A0 File Offset: 0x000499A0
	private void refresh()
	{
		List<Card> list;
		if (this.persistenceSetting)
		{
			list = this.eCards.toList();
		}
		else
		{
			list = Enumerable.ToList<Card>(Enumerable.Where<Card>(this.eCards, (Card c) => c.tradable));
		}
		List<Card> list2 = list;
		if (this.useSearchField)
		{
			CardFilter cardFilter = CardFilter.from(this.searchFieldString);
			if (this.overriddenCollection != null)
			{
				cardFilter.setOverridedCollectionForFiltering(this.overriddenCollection.toList());
			}
			this.cards = cardFilter.getFiltered(list2);
		}
		else
		{
			this.cards = list2;
		}
		if (this.collectionSorter != null)
		{
			this.cards.Sort(this.collectionSorter);
		}
	}

	// Token: 0x060009CB RID: 2507 RVA: 0x0004B864 File Offset: 0x00049A64
	private void Update()
	{
		this.refreshIfNeeded();
		Vector3 vector;
		vector..ctor(Input.mousePosition.x, (float)Screen.height - Input.mousePosition.y);
		bool flag = this.innerRect.Contains(vector);
		bool flag2 = false;
		int num = 0;
		foreach (Card card in this.cards)
		{
			Rect rect;
			rect..ctor(0f, (float)num * this.fieldHeight, this.innerRect.width - this.scrollBarSize, this.fieldHeight);
			if (flag && rect.Contains(vector - new Vector3(this.innerRect.x - this.scrollPos.x, this.innerRect.y - this.scrollPos.y)))
			{
				flag2 = true;
				this.callback.ItemHovered(this, card);
				break;
			}
			num++;
		}
		if (!flag2)
		{
			this.callback.ItemHovered(this, null);
		}
	}

	// Token: 0x060009CC RID: 2508 RVA: 0x0004B9A0 File Offset: 0x00049BA0
	private void OnGUI()
	{
		this.refreshIfNeeded();
		GUI.depth = 15;
		GUI.skin = this.cardListPopupSkin;
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.opacity);
		Rect rect;
		rect..ctor(this.outerRect.x + this.offX, this.outerRect.y, this.outerRect.width, this.outerRect.height);
		Rect rect2;
		rect2..ctor(this.innerBGRect.x + this.offX, this.innerBGRect.y, this.innerBGRect.width, this.innerBGRect.height);
		Rect rect3;
		rect3..ctor(this.innerRect.x + this.offX, this.innerRect.y, this.innerRect.width, this.innerRect.height);
		Rect rect4;
		rect4..ctor(this.buttonLeftRect.x + this.offX, this.buttonLeftRect.y, this.buttonLeftRect.width, this.buttonLeftRect.height);
		Rect rect5;
		rect5..ctor(this.buttonRightRect.x + this.offX, this.buttonRightRect.y, this.buttonRightRect.width, this.buttonRightRect.height);
		Rect rect6;
		rect6..ctor(this.searchFieldRect);
		rect6.x += this.offX;
		Rect full;
		full..ctor(this.lockedRect);
		full.x += this.offX;
		if (this.buttons.Count > 0)
		{
			Rect full2 = rect3;
			rect3.yMin += (float)Screen.height * 0.05f;
			rect2.yMin += (float)Screen.height * 0.05f;
			full2.yMax = rect3.yMin;
			float num = 1f / (float)this.buttons.Count;
			float num2 = 0.005f;
			for (int i = 0; i < this.buttons.Count; i++)
			{
				Rect rect7 = GeomUtil.cropShare(full2, new Rect((float)i * num + num2, 0f, num - 2f * num2, 0.85f));
				CardListPopup.Button button = this.buttons[i];
				GUIStyle guistyle = (this.selectedButton != button) ? GUI.skin.button : this.sortButtonSelectedStyle;
				if (GUI.Button(rect7, button.content(), guistyle))
				{
					if (this.selectedButton == button)
					{
						button.clickAgain();
					}
					else
					{
						button.click();
					}
					this.selectedButton = button;
				}
			}
		}
		if (this.showFrame)
		{
			GUI.Box(rect, string.Empty);
		}
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.opacity * 0.3f);
		GUI.Box(rect2, string.Empty);
		GUI.color = new Color(GUI.color.r, GUI.color.g, GUI.color.b, this.opacity);
		bool flag = false;
		flag |= this.drawSearchfield(rect6);
		if (this.useLockedButton)
		{
			Rect checkboxRect = GeomUtil.cropShare(full, new Rect(0f, 0f, 0.27f, 1f));
			checkboxRect.width = checkboxRect.height;
			bool v = this.persistenceSetting;
			if (new Checkbox(this.labelToggleEnabled, this.labelToggleDisabled, GeomUtil.cropShare(full, new Rect(0.4f, -0.3f, 0.7f, 1.4f)), checkboxRect, this.lobbySkin, 0.95f).SetAlignment(3).Draw(ref v))
			{
				flag = true;
			}
			this.persistenceSetting.set(v);
		}
		if (flag)
		{
			this.refresh();
		}
		this.cardListPopupBigLabelSkin.label.fontSize = (int)(this.fieldHeight / 1.7f);
		this.cardListPopupSkin.label.fontSize = (int)(this.fieldHeight / 2.5f);
		this.scrollPos = GUI.BeginScrollView(rect3, this.scrollPos, new Rect(0f, 0f, this.innerRect.width - 20f, this.fieldHeight * (float)this.cards.Count));
		int num3 = 0;
		Card card = null;
		foreach (Card card2 in this.cards)
		{
			Rect rect8;
			rect8..ctor(this.itemOffsetX + 2f, (float)num3 * this.fieldHeight, this.innerRect.width - this.scrollBarSize - this.itemOffsetX - 2f, this.fieldHeight);
			if (rect8.yMax < this.scrollPos.y || rect8.y > this.scrollPos.y + rect3.height)
			{
				num3++;
				GUI.color = new Color(1f, 1f, 1f, this.opacity);
			}
			else
			{
				if (!card2.tradable)
				{
					GUI.color = new Color(1f, 1f, 1f, this.opacity * 0.5f);
				}
				GUI.skin = this.cardListPopupGradientSkin;
				if (this.clickableItems)
				{
					if (GUI.Button(rect8, string.Empty))
					{
						this.callback.ItemClicked(this, card2);
					}
				}
				else
				{
					GUI.Box(rect8, string.Empty);
				}
				Texture texture = App.AssetLoader.LoadCardImage(card2.getCardImage());
				Rect rect9;
				rect9..ctor(this.itemOffsetX + this.fieldHeight * 0.21f, (float)num3 * this.fieldHeight + (this.fieldHeight - this.cardHeight) * 0.43f, this.cardWidth, this.cardHeight);
				if (texture != null)
				{
					GUI.DrawTexture(rect9, texture);
				}
				if (card2.level > 0)
				{
					GUI.skin = this.cardListPopupTierBoxSkin;
					Color color = GUI.color;
					GUI.color = new Color(1f, 1f, 1f, GUI.color.a);
					GUI.Box(new Rect(rect9.x - 2f, rect9.y - 2f, rect9.width + 4f, rect9.height + 4f), texture);
					GUI.color = color;
				}
				GUI.skin = this.cardListPopupBigLabelSkin;
				string name = card2.getName();
				Vector2 vector = GUI.skin.label.CalcSize(new GUIContent(name));
				Rect rect10;
				rect10..ctor(this.labelX, (float)num3 * this.fieldHeight - 3f + this.fieldHeight * 0.01f, this.labelsWidth, this.cardHeight);
				GUI.Label(rect10, (vector.x >= rect10.width) ? (name.Substring(0, Mathf.Min(name.Length, this.maxCharsName)) + "...") : name);
				GUI.skin = this.cardListPopupSkin;
				string text = (this.descriptionWriter != null) ? this.descriptionWriter(card2) : CardListPopup.DefaultWriter(card2);
				Vector2 vector2 = GUI.skin.label.CalcSize(new GUIContent(text));
				Rect rect11;
				rect11..ctor(this.labelX, (float)num3 * this.fieldHeight - 3f + this.fieldHeight * 0.57f, this.labelsWidth * 0.95f, this.cardHeight);
				GUI.Label(rect11, text);
				if (this.rightAdjustedDescriptionWriter != null)
				{
					TextAnchor alignment = GUI.skin.label.alignment;
					GUI.skin.label.alignment = 2;
					GUI.Label(rect11, this.rightAdjustedDescriptionWriter(card2));
					GUI.skin.label.alignment = alignment;
				}
				this.RenderCost(new Rect(this.labelX + this.labelsWidth + (this.costIconSize - this.costIconWidth) / 2f - 5f, (float)num3 * this.fieldHeight + (this.fieldHeight - this.costIconHeight) / 2f, this.costIconWidth, this.costIconHeight), card2);
				if (this.hasItemButtons())
				{
					GUI.skin = this.cardListPopupLeftButtonSkin;
					Rect rect12;
					rect12..ctor(0f, (float)num3 * this.fieldHeight, this.fieldHeight, this.fieldHeight);
					if (this.itemButtonEnabled && GUI.Button(rect12, string.Empty) && card2.tradable)
					{
						if (this.selectable)
						{
							if (!this.selectedCards.Contains(card2))
							{
								this.selectedCards.Add(card2);
							}
							else
							{
								this.selectedCards.Remove(card2);
							}
						}
						else
						{
							card = card2;
						}
						App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
					}
					if (card2.tradable)
					{
						if (!this.itemButtonEnabled)
						{
							GUI.color = new Color(1f, 1f, 1f, this.opacity * 0.5f);
						}
						if (this.selectable)
						{
							if (this.selectedCards.Contains(card2))
							{
								GUI.DrawTexture(rect12, ResourceManager.LoadTexture("Arena/scroll_browser_button_cb_checked"));
							}
							else
							{
								GUI.DrawTexture(rect12, ResourceManager.LoadTexture("Arena/scroll_browser_button_cb"));
							}
						}
						else if (this.itemButtonTexture != null)
						{
							GUI.DrawTexture(rect12, this.itemButtonTexture);
						}
					}
				}
				if (!card2.tradable)
				{
					GUI.color = new Color(1f, 1f, 1f, this.opacity);
				}
				num3++;
			}
		}
		GUI.EndScrollView();
		if (card != null)
		{
			this.callback.ItemButtonClicked(this, card);
		}
		GUI.skin = this.lobbySkin;
		if (this.buttonLeftContent != null)
		{
			if (!this.leftButtonEnabled)
			{
				GUI.enabled = false;
			}
			if (GUI.Button(rect4, this.buttonLeftContent))
			{
				if (this.selectable)
				{
					this.callback.ButtonClicked(this, ECardListButton.BUTTON_LEFT, new List<Card>(this.selectedCards));
					this.selectedCards.Clear();
				}
				else
				{
					this.callback.ButtonClicked(this, ECardListButton.BUTTON_LEFT);
				}
				App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
			}
			Rect rect13;
			rect13..ctor(rect4.x + rect4.height * 0.01f, rect4.y, rect4.height, rect4.height);
			if (this.leftButtonHighlighted)
			{
				GUI.DrawTexture(rect13, ResourceManager.LoadTexture("Arena/scroll_browser_button_cb_checked"));
			}
			else if (this.leftHighlightable)
			{
				GUI.DrawTexture(rect13, ResourceManager.LoadTexture("Arena/scroll_browser_button_cb"));
			}
			GUI.Label(rect4, this.buttonLeftContent);
			if (!this.leftButtonEnabled)
			{
				GUI.enabled = true;
			}
		}
		if (this.buttonRightContent != null)
		{
			if (!this.rightButtonEnabled)
			{
				GUI.enabled = false;
			}
			if (GUI.Button(rect5, this.buttonRightContent))
			{
				if (this.selectable)
				{
					this.callback.ButtonClicked(this, ECardListButton.BUTTON_RIGHT, new List<Card>(this.selectedCards));
					this.selectedCards.Clear();
				}
				else
				{
					this.callback.ButtonClicked(this, ECardListButton.BUTTON_RIGHT);
				}
				App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
			}
			Rect rect14;
			rect14..ctor(rect5.x + rect5.height * 0.01f, rect5.y, rect5.height, rect5.height);
			if (this.rightButtonHighlighted)
			{
				GUI.DrawTexture(rect14, ResourceManager.LoadTexture("Arena/scroll_browser_button_cb_checked"));
			}
			else if (this.rightHighlightable)
			{
				GUI.DrawTexture(rect14, ResourceManager.LoadTexture("Arena/scroll_browser_button_cb"));
			}
			GUI.Label(rect5, this.buttonRightContent);
			if (!this.rightButtonEnabled)
			{
				GUI.enabled = false;
			}
		}
	}

	// Token: 0x060009CD RID: 2509 RVA: 0x0004C684 File Offset: 0x0004A884
	private bool drawSearchfield(Rect rect)
	{
		if (!this.useSearchField)
		{
			return false;
		}
		string text = this.searchFieldString;
		this.searchFieldString = GUI.TextField(rect, this.searchFieldString, this.searchFieldStyle);
		return this.searchFieldString != text;
	}

	// Token: 0x060009CE RID: 2510 RVA: 0x0004C6CC File Offset: 0x0004A8CC
	private void RenderCost(Rect rect, Card card)
	{
		float num = this.fieldHeight * 0.05f;
		rect.x -= num;
		ResourceType resource = card.getCardType().getResource();
		int cost = card.getCardType().getCost();
		Texture texture = ResourceManager.LoadTexture(resource.battleIconFilename());
		if (texture != null)
		{
			GUI.DrawTexture(rect, texture);
			char[] array = Convert.ToString(cost).ToCharArray();
			for (int i = 0; i < array.Length; i++)
			{
				Rect rect2;
				rect2..ctor(2.5f * num + rect.xMax + 5f - (float)(array.Length - i) * rect.height * 0.6f, rect.y + 1f, rect.height * 0.7f, rect.height);
				Texture texture2 = ResourceManager.LoadTexture("Scrolls/yellow_" + array[i]);
				GUI.DrawTexture(rect2, texture2);
			}
		}
	}

	// Token: 0x060009CF RID: 2511 RVA: 0x0004C7CC File Offset: 0x0004A9CC
	public void SetButtonHighlighted(ECardListButton button, bool highlighted)
	{
		if (button != ECardListButton.BUTTON_LEFT)
		{
			if (button == ECardListButton.BUTTON_RIGHT)
			{
				this.rightButtonHighlighted = highlighted;
			}
		}
		else
		{
			this.leftButtonHighlighted = highlighted;
		}
	}

	// Token: 0x060009D0 RID: 2512 RVA: 0x0004C808 File Offset: 0x0004AA08
	public void SetButtonContent(ECardListButton button, GUIContent content)
	{
		if (button != ECardListButton.BUTTON_LEFT)
		{
			if (button == ECardListButton.BUTTON_RIGHT)
			{
				this.buttonRightContent = content;
			}
		}
		else
		{
			this.buttonLeftContent = content;
		}
	}

	// Token: 0x060009D1 RID: 2513 RVA: 0x0004C844 File Offset: 0x0004AA44
	public void SetButtonEnabled(ECardListButton button, bool enabled)
	{
		if (button != ECardListButton.BUTTON_LEFT)
		{
			if (button == ECardListButton.BUTTON_RIGHT)
			{
				this.rightButtonEnabled = enabled;
			}
		}
		else
		{
			this.leftButtonEnabled = enabled;
		}
	}

	// Token: 0x060009D2 RID: 2514 RVA: 0x00008360 File Offset: 0x00006560
	public CardListPopup SetPersistence(SvBool setting)
	{
		this.persistenceSetting = setting;
		return this;
	}

	// Token: 0x060009D3 RID: 2515 RVA: 0x0000836A File Offset: 0x0000656A
	public void SetOffX(float offX)
	{
		this.offX = offX;
	}

	// Token: 0x060009D4 RID: 2516 RVA: 0x00008373 File Offset: 0x00006573
	public void SetOpacity(float opacity)
	{
		this.opacity = opacity;
	}

	// Token: 0x060009D5 RID: 2517 RVA: 0x0000837C File Offset: 0x0000657C
	public CardListPopup AddButton(CardListPopup.Button button)
	{
		button.cardList = this;
		this.buttons.Add(button);
		return this;
	}

	// Token: 0x060009D6 RID: 2518 RVA: 0x0004C880 File Offset: 0x0004AA80
	public CardListPopup AddButtons(params CardListPopup.Button[] buttons)
	{
		foreach (CardListPopup.Button button in buttons)
		{
			this.AddButton(button);
		}
		return this;
	}

	// Token: 0x060009D7 RID: 2519 RVA: 0x00008392 File Offset: 0x00006592
	public CardListPopup.Button GetButton(int index)
	{
		return this.buttons[index];
	}

	// Token: 0x0400075B RID: 1883
	private GUISkin lobbySkin;

	// Token: 0x0400075C RID: 1884
	private GUISkin cardListPopupSkin;

	// Token: 0x0400075D RID: 1885
	private GUISkin cardListPopupGradientSkin;

	// Token: 0x0400075E RID: 1886
	private GUISkin cardListPopupBigLabelSkin;

	// Token: 0x0400075F RID: 1887
	private GUISkin cardListPopupLeftButtonSkin;

	// Token: 0x04000760 RID: 1888
	private GUISkin cardListPopupTierBoxSkin;

	// Token: 0x04000761 RID: 1889
	private GUIStyle searchFieldStyle;

	// Token: 0x04000762 RID: 1890
	private GUIStyle sortButtonSelectedStyle;

	// Token: 0x04000763 RID: 1891
	private Rect screenRect;

	// Token: 0x04000764 RID: 1892
	private Rect lockedRect;

	// Token: 0x04000765 RID: 1893
	private Rect outerRect;

	// Token: 0x04000766 RID: 1894
	private Rect innerBGRect;

	// Token: 0x04000767 RID: 1895
	private Rect innerRect;

	// Token: 0x04000768 RID: 1896
	private Rect buttonLeftRect;

	// Token: 0x04000769 RID: 1897
	private Rect buttonRightRect;

	// Token: 0x0400076A RID: 1898
	private Rect searchFieldRect;

	// Token: 0x0400076B RID: 1899
	private bool selectable;

	// Token: 0x0400076C RID: 1900
	public Vector2 scrollPos;

	// Token: 0x0400076D RID: 1901
	private float BOTTOM_MARGIN_EXTRA = (float)Screen.height * 0.047f;

	// Token: 0x0400076E RID: 1902
	private Vector4 margins;

	// Token: 0x0400076F RID: 1903
	private EList<Card> eCards;

	// Token: 0x04000770 RID: 1904
	private List<Card> cards;

	// Token: 0x04000771 RID: 1905
	private EList<Card> overriddenCollection;

	// Token: 0x04000772 RID: 1906
	private List<Card> selectedCards = new List<Card>();

	// Token: 0x04000773 RID: 1907
	private IComparer<Card> collectionSorter;

	// Token: 0x04000774 RID: 1908
	private ICardListCallback callback;

	// Token: 0x04000775 RID: 1909
	private GUIContent buttonLeftContent;

	// Token: 0x04000776 RID: 1910
	private GUIContent buttonRightContent;

	// Token: 0x04000777 RID: 1911
	private Texture itemButtonTexture;

	// Token: 0x04000778 RID: 1912
	private bool itemButtonEnabled = true;

	// Token: 0x04000779 RID: 1913
	private bool leftButtonEnabled;

	// Token: 0x0400077A RID: 1914
	private bool rightButtonEnabled;

	// Token: 0x0400077B RID: 1915
	private bool showFrame;

	// Token: 0x0400077C RID: 1916
	private float scrollBarSize = 20f;

	// Token: 0x0400077D RID: 1917
	private bool leftButtonHighlighted;

	// Token: 0x0400077E RID: 1918
	private bool rightButtonHighlighted;

	// Token: 0x0400077F RID: 1919
	private bool leftHighlightable;

	// Token: 0x04000780 RID: 1920
	private bool rightHighlightable;

	// Token: 0x04000781 RID: 1921
	private Texture2D bgBar;

	// Token: 0x04000782 RID: 1922
	private float fieldHeight;

	// Token: 0x04000783 RID: 1923
	private float costIconSize;

	// Token: 0x04000784 RID: 1924
	private float costIconWidth;

	// Token: 0x04000785 RID: 1925
	private float costIconHeight;

	// Token: 0x04000786 RID: 1926
	private float cardHeight;

	// Token: 0x04000787 RID: 1927
	private float cardWidth;

	// Token: 0x04000788 RID: 1928
	private float itemOffsetX;

	// Token: 0x04000789 RID: 1929
	private float labelX;

	// Token: 0x0400078A RID: 1930
	private float labelsWidth;

	// Token: 0x0400078B RID: 1931
	private string labelToggleEnabled;

	// Token: 0x0400078C RID: 1932
	private string labelToggleDisabled;

	// Token: 0x0400078D RID: 1933
	private int maxCharsName;

	// Token: 0x0400078E RID: 1934
	private int maxCharsRK;

	// Token: 0x0400078F RID: 1935
	private float offX;

	// Token: 0x04000790 RID: 1936
	private float opacity;

	// Token: 0x04000791 RID: 1937
	private bool clickableItems;

	// Token: 0x04000792 RID: 1938
	private bool useSearchField = true;

	// Token: 0x04000793 RID: 1939
	private bool useLockedButton = true;

	// Token: 0x04000794 RID: 1940
	private string searchFieldString = string.Empty;

	// Token: 0x04000795 RID: 1941
	private List<CardListPopup.Button> buttons = new List<CardListPopup.Button>();

	// Token: 0x04000796 RID: 1942
	private CardListPopup.Button selectedButton;

	// Token: 0x04000797 RID: 1943
	public static readonly CardListPopup.Button SortByNameButton = new CardListPopup.ButtonCycler().add(new CardListPopup.Button(new GUIContent("Name"), delegate(CardListPopup c)
	{
		c.SetSorter(new DeckSorter().byColor().byNameDesc());
	})).add(new CardListPopup.Button(new GUIContent("Name"), delegate(CardListPopup c)
	{
		c.SetSorter(new DeckSorter().byColor().byName());
	}));

	// Token: 0x04000798 RID: 1944
	public static readonly CardListPopup.Button SortByCostButton = new CardListPopup.ButtonCycler().add(new CardListPopup.Button(new GUIContent("Cost"), delegate(CardListPopup c)
	{
		c.SetSorter(new DeckSorter().byColor().byResourceCountDesc().byName());
	})).add(new CardListPopup.Button(new GUIContent("Cost"), delegate(CardListPopup c)
	{
		c.SetSorter(new DeckSorter().byColor().byResourceCount().byName());
	}));

	// Token: 0x04000799 RID: 1945
	private SvBool persistenceSetting = new SvBool(true);

	// Token: 0x0400079A RID: 1946
	private CardListPopup.CardDescriptionWriter descriptionWriter;

	// Token: 0x0400079B RID: 1947
	private CardListPopup.CardDescriptionWriter rightAdjustedDescriptionWriter;

	// Token: 0x0400079C RID: 1948
	public static CardListPopup.CardDescriptionWriter DefaultWriter = new CardListPopup.CardDescriptionWriter(CardListPopup.WriteDescription);

	// Token: 0x02000131 RID: 305
	public class Button
	{
		// Token: 0x060009DD RID: 2525 RVA: 0x00008412 File Offset: 0x00006612
		public Button(GUIContent content, Action<CardListPopup> action)
		{
			this.set(content, action);
		}

		// Token: 0x060009DE RID: 2526 RVA: 0x00008422 File Offset: 0x00006622
		public GUIContent content()
		{
			return this._content;
		}

		// Token: 0x060009DF RID: 2527 RVA: 0x0000842A File Offset: 0x0000662A
		public Action<CardListPopup> action()
		{
			return this._action;
		}

		// Token: 0x060009E0 RID: 2528 RVA: 0x00008432 File Offset: 0x00006632
		public virtual void click()
		{
			this._action.Invoke(this.cardList);
		}

		// Token: 0x060009E1 RID: 2529 RVA: 0x00008445 File Offset: 0x00006645
		public virtual void clickAgain()
		{
			this.click();
		}

		// Token: 0x060009E2 RID: 2530 RVA: 0x0000844D File Offset: 0x0000664D
		protected void set(GUIContent content, Action<CardListPopup> action)
		{
			this._content = content;
			this._action = action;
		}

		// Token: 0x040007A2 RID: 1954
		private GUIContent _content;

		// Token: 0x040007A3 RID: 1955
		private Action<CardListPopup> _action;

		// Token: 0x040007A4 RID: 1956
		internal CardListPopup cardList;
	}

	// Token: 0x02000132 RID: 306
	public class ButtonCycler : CardListPopup.Button
	{
		// Token: 0x060009E3 RID: 2531 RVA: 0x0000845D File Offset: 0x0000665D
		public ButtonCycler() : base(null, null)
		{
		}

		// Token: 0x060009E4 RID: 2532 RVA: 0x00008472 File Offset: 0x00006672
		public CardListPopup.ButtonCycler add(CardListPopup.Button button)
		{
			if (this.buttons.Count == 0)
			{
				this.set(button);
			}
			this.buttons.Add(button);
			return this;
		}

		// Token: 0x060009E5 RID: 2533 RVA: 0x0004C8B0 File Offset: 0x0004AAB0
		public override void clickAgain()
		{
			this.set(this.buttons[this.index = (this.index + 1) % this.buttons.Count]);
			base.click();
		}

		// Token: 0x060009E6 RID: 2534 RVA: 0x00008498 File Offset: 0x00006698
		public void set(CardListPopup.Button button)
		{
			base.set(button.content(), button.action());
		}

		// Token: 0x040007A5 RID: 1957
		private List<CardListPopup.Button> buttons = new List<CardListPopup.Button>();

		// Token: 0x040007A6 RID: 1958
		private int index;
	}

	// Token: 0x02000133 RID: 307
	// (Invoke) Token: 0x060009E8 RID: 2536
	public delegate string CardDescriptionWriter(Card card);
}
