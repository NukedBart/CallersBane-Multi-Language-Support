using System;
using System.Collections;
using System.Collections.Generic;
using Gui;
using UnityEngine;

// Token: 0x020001FD RID: 509
public class Limited : AbstractCommListener, ICardListCallback
{
	// Token: 0x06000FF3 RID: 4083 RVA: 0x0006A1C0 File Offset: 0x000683C0
	private void Awake()
	{
		this.allPicks = new EList<Card>();
		this.allPicksList = new GameObject("Card List / All Picks").AddComponent<CardListPopup>();
		this.allPicksList.transform.parent = base.transform;
		this.allPicksList.Init(default(Rect), false, false, this.allPicks, this, null, null, true, false, false, false, null, true).SetUseLockedButton(false).SetOpacity(1f);
		this.allPicksList.enabled = true;
		this.cardsPerRow = App.SceneValues.limited.cardsPerRow;
		this.targetCollectionSize = App.SceneValues.limited.targetCollectionSize;
		this.deckStatsPane = new GameObject("Deck Stats Pane").AddComponent<DeckStatsPane>();
	}

	// Token: 0x06000FF4 RID: 4084 RVA: 0x0006A284 File Offset: 0x00068484
	protected void Start()
	{
		this.regularUI = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		App.Communicator.addListener(this);
		this.gui3d = new Gui3D(Camera.main);
		this._mainCameraPosition = Camera.main.transform.position;
		base.name = "_Limited";
		this.SetupScene();
		App.AudioScript.PlayMusic("Music/Judgement");
		App.ChatUI.Show(false);
		base.StartCoroutine(this.FadeInAfterWait(0.2f));
		App.Communicator.send(new GetCollectionLimitedMessage());
		this.deckStatsPane.Init();
	}

	// Token: 0x06000FF5 RID: 4085 RVA: 0x0006A330 File Offset: 0x00068530
	private IEnumerator FadeInAfterWait(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		App.LobbyMenu.fadeInScene();
		yield break;
	}

	// Token: 0x06000FF6 RID: 4086 RVA: 0x0006A354 File Offset: 0x00068554
	private void SetupScene()
	{
		Camera.main.transparencySortMode = 2;
		Camera.main.nearClipPlane = 0.3f;
		Camera.main.farClipPlane = 1000f;
		this.lightSource = new GameObject("Light");
		this.lightSource.AddComponent<Light>();
		this.lightSource.light.color = new Color(1f, 1f, 1f);
		this.lightSource.transform.position = new Vector3(0f, 0f, 0f);
		this.lightSource.light.intensity = 0.8f;
		this.lightSource.light.type = 1;
		this.lightSource.light.range = 25f;
		this.lightSource.light.shadows = 2;
		this.lastScreenWidth = -9999;
		this.CheckScreenResolutionChanged(true);
		this.inited = true;
		Debug.Log("Init done!");
	}

	// Token: 0x06000FF7 RID: 4087 RVA: 0x0006A460 File Offset: 0x00068660
	private void SetupPositions()
	{
		this.rectSubMenu = App.LobbyMenu.getSubMenuRect(1f);
		float num = this.rectSubMenu.y + this.rectSubMenu.height;
		float num2 = (float)Screen.height * 0.98f - num;
		Vector3 vector = CardView.CardLocalScale();
		float num3 = vector.x / vector.z;
		float num4 = (float)Screen.height * 0.005f;
		float num5 = (float)Screen.height * 0.5f;
		float num6 = num3 * num5;
		float num7 = (float)Screen.width * 0.27f;
		if (num6 > num7)
		{
			num6 = num7;
			num5 = num7 / num3;
		}
		this.rectCard = new Rect(0f, 0f, num6, num5);
		this.rectCard.x = (float)Screen.width - this.rectCard.width - num4;
		this.rectCard.y = num + ((float)Screen.height * 0.9f - num - this.rectCard.height) * 0.59f;
		this.rectCardList = new Rect(num4 + this.rectCard.width * 0.02f, this.rectCard.y - this.rectCard.height * 0.04f, this.rectCard.width * 0.96f, this.rectCard.height * 1.07f);
		this.allPicksList.setRect(this.rectCardList);
	}

	// Token: 0x06000FF8 RID: 4088 RVA: 0x0006A5D8 File Offset: 0x000687D8
	private void CheckScreenResolutionChanged(bool forceChange)
	{
		if (forceChange || this.lastScreenWidth != Screen.width || this.lastScreenHeight != Screen.height)
		{
			this.lastScreenWidth = Screen.width;
			this.lastScreenHeight = Screen.height;
			this.OnResolutionChanged();
		}
	}

	// Token: 0x06000FF9 RID: 4089 RVA: 0x0000C99D File Offset: 0x0000AB9D
	public void OnResolutionChanged()
	{
		this.SetupPositions();
	}

	// Token: 0x06000FFA RID: 4090 RVA: 0x0006A628 File Offset: 0x00068828
	private CardView CreateCardView(Card card)
	{
		GameObject gameObject = PrimitiveFactory.createPlane();
		GameObject gameObject2 = PrimitiveFactory.createPlane(false);
		Material material = new Material(ResourceManager.LoadShader("Transparent/Diffuse"));
		material.mainTexture = ResourceManager.LoadTexture("DeckBuilder/scroll_shadow");
		gameObject2.renderer.material = material;
		gameObject2.transform.parent = gameObject.transform;
		gameObject2.transform.localPosition = new Vector3(--0f, -0.1f, 0.4f);
		gameObject2.name = "shadow";
		CardView cardView = gameObject.AddComponent<CardView>();
		cardView.transform.eulerAngles = new Vector3(0f, 180f, 0f);
		cardView.transform.localScale = this.SelectionCardScale();
		cardView.setTooltipEnabled(false);
		cardView.init(null, card, -1);
		gameObject.layer = 4;
		gameObject.name = "Card";
		return cardView;
	}

	// Token: 0x06000FFB RID: 4091 RVA: 0x0006A704 File Offset: 0x00068904
	private void GenerateCard(int typeId, float transparency, List<CardView> list)
	{
		Card card = new Card(1L, CardTypeManager.getInstance().get(typeId), true);
		CardView cardView = this.CreateCardView(card);
		cardView.setTransparency(transparency);
		list.Add(cardView);
	}

	// Token: 0x06000FFC RID: 4092 RVA: 0x0006A73C File Offset: 0x0006893C
	private void SetCardPositions(float rowOffset)
	{
		int num = 0;
		float num2 = this.rectSubMenu.y + this.rectSubMenu.height;
		int num3 = 3;
		foreach (CardView cardView in this.cardSelection)
		{
			int num4 = num % this.cardsPerRow;
			int num5 = num / this.cardsPerRow;
			Vector3 vector = CardView.CardLocalScale();
			float num6 = vector.x / vector.z;
			float num7 = (float)Screen.height / 5f;
			float num8 = num6 * num7;
			float num9 = num8 * 1.15f;
			float num10 = num7 * 1.1f;
			float num11 = num10 * rowOffset;
			Rect dst;
			dst..ctor((float)(Screen.width / 2) + num9 * ((float)num4 - (float)this.cardsPerRow / 2f) + (num9 - num8) / 2f, num2 * 0.4f + (float)Screen.height * 0.57f + num10 * ((float)num5 - (float)num3 / 2f) + num11, num8, num7);
			this.gui3d.DrawObject(dst, cardView.gameObject);
			num++;
		}
	}

	// Token: 0x06000FFD RID: 4093 RVA: 0x0006A87C File Offset: 0x00068A7C
	private IEnumerator UpdateCardList()
	{
		this.acceptingInput = false;
		float durationFadeOut = 0.15f;
		float durationMove = 0.3f;
		float durationFadeIn = 0.2f;
		float t = 0f;
		float timeStarted = Time.time;
		Vector3 fromPos = this.lastSelected.transform.position;
		Vector3 fromScale = this.lastSelected.transform.localScale;
		float targetWidth = this.rectCard.width / 1.4f;
		float targetHeight = this.rectCard.height / 1.4f;
		this.gui3d.DrawObject(new Rect(this.rectCardList.x + (this.rectCardList.width - targetWidth) / 2f, this.rectCardList.y + (this.rectCardList.height - targetHeight) / 3f, targetWidth, targetHeight), this.lastSelected.gameObject);
		Vector3 toPos = this.lastSelected.transform.position;
		Vector3 toScale = this.lastSelected.transform.localScale;
		this.lastSelected.transform.localScale = fromScale;
		this.lastSelected.transform.position = fromPos;
		iTween.Stop(this.lastSelected.gameObject);
		iTween.ScaleTo(this.lastSelected.gameObject, toScale, 0.25f);
		iTween.MoveTo(this.lastSelected.gameObject, toPos, 0.25f);
		while (t <= 1f)
		{
			t = (Time.time - timeStarted) / durationFadeOut;
			for (int i = 0; i < this.cardsPerRow; i++)
			{
				if (this.cardSelection[i] != this.lastSelected)
				{
					this.cardSelection[i].setTransparency(this.Smoothstep(1f, 0f, t));
				}
			}
			yield return null;
		}
		for (int j = 0; j < this.cardsPerRow; j++)
		{
			if (this.cardSelection[0] != this.lastSelected)
			{
				Object.Destroy(this.cardSelection[0].gameObject);
			}
			this.cardSelection.RemoveAt(0);
		}
		t = 0f;
		timeStarted = Time.time;
		while (t <= 1f)
		{
			t = (Time.time - timeStarted) / durationFadeOut;
			this.lastSelected.setTransparency(this.Smoothstep(1f, 0f, t));
			yield return null;
		}
		Object.Destroy(this.lastSelected.gameObject);
		t = 0f;
		timeStarted = Time.time;
		while (t <= 1f)
		{
			t = (Time.time - timeStarted) / durationMove;
			this.SetCardPositions(this.Smoothstep(1f, 0f, t));
			for (int k = 0; k < Mathf.Min(this.cardsPerRow, this.cardSelection.Count); k++)
			{
				this.cardSelection[k].setTransparency(this.Smoothstep(0.4f, 1f, t));
			}
			yield return null;
		}
		for (int l = 0; l < Mathf.Min(this.cardsPerRow, this.cardSelection.Count); l++)
		{
			this.cardSelection[l].setTransparency(1f);
		}
		if (this.allPicks.Count < this.targetCollectionSize - 2)
		{
			while (this.pendingCardTypes.Count == 0)
			{
				yield return null;
			}
			for (int m = 0; m < this.cardsPerRow; m++)
			{
				int index = this.pendingCardTypes.Count - this.cardsPerRow + m;
				this.GenerateCard(this.pendingCardTypes[index], 0f, this.cardSelection);
			}
			this.pendingCardTypes.Clear();
			this.SetCardPositions(0f);
			this.lastSelected = null;
			this.acceptingInput = true;
			t = 0f;
			timeStarted = Time.time;
			while (t <= 1f)
			{
				t = (Time.time - timeStarted) / durationFadeIn;
				for (int n = 0; n < this.cardsPerRow; n++)
				{
					this.cardSelection[this.cardSelection.Count - n - 1].setTransparency(this.Smoothstep(0f, 0.4f, t));
				}
				yield return null;
			}
			for (int i2 = 0; i2 < this.cardsPerRow; i2++)
			{
				this.cardSelection[this.cardSelection.Count - i2 - 1].setTransparency(0.4f);
			}
		}
		else
		{
			this.SetCardPositions(0f);
			this.lastSelected = null;
			this.acceptingInput = true;
		}
		yield break;
	}

	// Token: 0x06000FFE RID: 4094 RVA: 0x00006AC7 File Offset: 0x00004CC7
	private float Smoothstep(float from, float to, float t)
	{
		return from + (to - from) * (t * t * (3f - 2f * t));
	}

	// Token: 0x06000FFF RID: 4095 RVA: 0x0000A22E File Offset: 0x0000842E
	protected Vector3 SelectionCardScale()
	{
		return CardView.CardLocalScale(Camera.main.orthographicSize / 4f);
	}

	// Token: 0x06001000 RID: 4096 RVA: 0x0006A898 File Offset: 0x00068A98
	protected void Update()
	{
		if (!this.inited)
		{
			return;
		}
		this.CheckScreenResolutionChanged(false);
		CardView cardUnderMouse = this.GetCardUnderMouse();
		int num = this.cardSelection.IndexOf(cardUnderMouse);
		if (cardUnderMouse)
		{
			this.ShowCardRule(cardUnderMouse.getCardInfo());
			if (Input.GetMouseButtonDown(0) && num >= 0 && num < this.cardsPerRow && this.acceptingInput)
			{
				this.lastSelected = cardUnderMouse;
				App.AudioScript.PlaySFX("Sounds/hyperduck/DeckBuilder/db_scroll_pickup");
				this.allPicks.Add(cardUnderMouse.getCardInfo());
				this.deckStatsPane.UpdateGraphs(this.allPicks.toList());
				this.allPicksList.scrollPos = new Vector2(0f, float.PositiveInfinity);
				App.Communicator.send(new SelectCardLimitedMessage(cardUnderMouse.getCardInfo().getType()));
				base.StartCoroutine(this.UpdateCardList());
			}
		}
		if (this.lastHovered != cardUnderMouse)
		{
			if (cardUnderMouse != null && num >= 0 && num < this.cardsPerRow)
			{
				iTween.ScaleTo(cardUnderMouse.gameObject, this.SelectionCardScale() * 1.2f, 0.2f);
			}
			int num2 = this.cardSelection.IndexOf(this.lastHovered);
			if (this.lastHovered != null && this.lastHovered != this.lastSelected && num2 >= 0 && num2 < this.cardsPerRow)
			{
				iTween.ScaleTo(this.lastHovered.gameObject, this.SelectionCardScale(), 0.2f);
			}
			this.lastHovered = cardUnderMouse;
		}
	}

	// Token: 0x06001001 RID: 4097 RVA: 0x0006AA4C File Offset: 0x00068C4C
	protected void OnGUI()
	{
		GUI.depth = 21;
		if (Event.current.type == 7)
		{
			this.gui3d.frameBegin();
			this.OnGUI_draw3D();
			this.gui3d.frameEnd();
		}
		Texture2D texture2D = ResourceManager.LoadTexture("Limited/judgement");
		float num = (float)Screen.height * 0.15f;
		float num2 = (float)(texture2D.width / texture2D.height) * num;
		GUI.Label(new Rect((float)(Screen.width / 2) - num2 / 2f, (float)Screen.height * 0.15f, num2, num), texture2D);
		GUI.skin = this.regularUI;
		int fontSize = GUI.skin.label.fontSize;
		GUI.skin.label.fontSize = Screen.height / 30;
		GUI.Label(new Rect(this.rectCardList.x + this.rectCardList.width / 8f, this.rectCardList.yMax - (float)Screen.height * 0.048f, this.rectCardList.width / 4f, (float)Screen.height * 0.05f), this.allPicks.Count + "/" + this.targetCollectionSize);
		int fontSize2 = GUI.skin.button.fontSize;
		if (this.allPicks.Count >= this.targetCollectionSize)
		{
			GUI.skin.label.fontSize = Screen.height / 16;
			GUI.Label(new Rect(0f, (float)Screen.height * 0.475f, (float)Screen.width, (float)Screen.height * 0.15f), "<color=#000000>Well done! Now build\nyour deck.</color>");
			GUI.Label(new Rect(0f, (float)Screen.height * 0.47f, (float)Screen.width, (float)Screen.height * 0.15f), "<color=#ddcc99>Well done! Now build\nyour deck.</color>");
			float num3 = (float)Screen.height * 0.24f;
			float num4 = (float)Screen.height * 0.06f;
			GUI.skin.button.fontSize = Screen.height / 24;
			if (GUI.Button(new Rect((float)(Screen.width / 2) - num3 / 2f, (float)Screen.height * 0.84f, num3, num4), "Build deck"))
			{
				App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
				GUI.skin.button.fontSize = fontSize2;
				App.SceneValues.deckBuilder = new SceneValues.SV_DeckBuilder();
				App.SceneValues.deckBuilder.isLimitedMode = true;
				SceneLoader.loadScene("_DeckBuilderView");
			}
			GUI.skin.button.fontSize = fontSize2;
		}
		GUI.skin.button.fontSize = Screen.height / 40;
		if (GUI.Button(new Rect((float)Screen.height * 0.16f, (float)Screen.height * 0.78f, (float)Screen.height * 0.1f, (float)Screen.height * 0.035f), "Stats") && !this.deckStatsPane.IsOpen())
		{
			App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
			this.deckStatsPane.Toggle();
		}
		GUI.skin.button.fontSize = fontSize2;
		GUI.skin.label.fontSize = fontSize;
	}

	// Token: 0x06001002 RID: 4098 RVA: 0x0006AD9C File Offset: 0x00068F9C
	protected void OnGUI_draw3D()
	{
		this.gui3d.setDepth(950f);
		Texture2D tex = ResourceManager.LoadTexture("DeckBuilder/bg");
		this.gui3d.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), tex);
		this.gui3d.GetLastMaterial().color = new Color(0.55f, 0.8f, 1f);
		this.gui3d.setDepth(949f);
		float num = this.rectSubMenu.y + this.rectSubMenu.height;
		float num2 = this.rectCard.height * 1.15f;
		Rect rect;
		rect..ctor((float)(-(float)Screen.height) * 0.05f, this.rectCard.y - (num2 - this.rectCard.height) / 2f - (float)Screen.height * 0.01f, this.rectCard.width + (float)Screen.height * 0.08f, num2);
		new ScrollsFrame(rect).SetGui(this.gui3d).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.CENTER).Draw();
		Rect rect2 = rect;
		rect2.x = (float)Screen.width - rect2.width + (float)Screen.height * 0.05f;
		new ScrollsFrame(rect2).SetGui(this.gui3d).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.CENTER).Draw();
		this.gui3d.setDepth(0.4f);
	}

	// Token: 0x06001003 RID: 4099 RVA: 0x0006AF14 File Offset: 0x00069114
	private CardView GetCardUnderMouse()
	{
		if (App.ChatUI.IsHovered())
		{
			return null;
		}
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit hit;
		if (!Physics.Raycast(ray, ref hit, 2000f, 16))
		{
			return null;
		}
		return this.cardSelection.Find((CardView x) => x.transform == hit.collider.transform);
	}

	// Token: 0x06001004 RID: 4100 RVA: 0x0006AF7C File Offset: 0x0006917C
	private void ShowCardRule(Card card)
	{
		if ((long)card.getType() == this.lastCardRuleTypeId)
		{
			return;
		}
		this.lastCardRuleTypeId = (long)card.getType();
		if (this.cardRule != null)
		{
			Object.Destroy(this.cardRule);
		}
		this.cardRule = PrimitiveFactory.createPlane(false);
		this.cardRule.name = "CardRule";
		CardView cardView = this.cardRule.AddComponent<CardView>();
		cardView.init(null, card, 200);
		cardView.applyHighResTexture();
		cardView.enableShowHelp();
		this.gui3d.DrawObject(this.rectCard, this.cardRule);
	}

	// Token: 0x06001005 RID: 4101 RVA: 0x0006B020 File Offset: 0x00069220
	public override void handleMessage(Message m)
	{
		if (m is GetCollectionLimitedMessage)
		{
			GetCollectionLimitedMessage getCollectionLimitedMessage = (GetCollectionLimitedMessage)m;
			foreach (Card item in getCollectionLimitedMessage.cards)
			{
				this.allPicks.Add(item);
			}
			if (this.allPicks.Count > 0)
			{
				this.ShowCardRule(this.allPicks[this.allPicks.Count - 1]);
			}
			this.deckStatsPane.UpdateGraphs(this.allPicks.toList());
		}
		if (m is NextCardsLimitedMessage)
		{
			NextCardsLimitedMessage nextCardsLimitedMessage = (NextCardsLimitedMessage)m;
			int num = 0;
			int num2 = 0;
			if (this.cardSelection.Count == 0)
			{
				foreach (int typeId in nextCardsLimitedMessage.cardTypes)
				{
					if (num2 >= this.targetCollectionSize - this.allPicks.Count)
					{
						break;
					}
					this.GenerateCard(typeId, (num >= this.cardsPerRow) ? 0.4f : 1f, this.cardSelection);
					num++;
					if (num % this.cardsPerRow == 0)
					{
						num2++;
					}
				}
				this.SetCardPositions(0f);
			}
			else
			{
				foreach (int num3 in nextCardsLimitedMessage.cardTypes)
				{
					this.pendingCardTypes.Add(num3);
				}
			}
			if (this.cardRule == null)
			{
				this.ShowCardRule(this.cardSelection[0].getCardInfo());
			}
		}
	}

	// Token: 0x06001006 RID: 4102 RVA: 0x000028DF File Offset: 0x00000ADF
	public void ButtonClicked(CardListPopup popup, ECardListButton button)
	{
	}

	// Token: 0x06001007 RID: 4103 RVA: 0x000028DF File Offset: 0x00000ADF
	public void ButtonClicked(CardListPopup popup, ECardListButton button, List<Card> selectedCards)
	{
	}

	// Token: 0x06001008 RID: 4104 RVA: 0x0006B1D4 File Offset: 0x000693D4
	public void ItemHovered(CardListPopup popup, Card card)
	{
		if (this.deckStatsPane.IsOpen())
		{
			return;
		}
		if (card != null && (!(this.cardRule != null) || this.cardRule.GetComponent<CardView>().getCardInfo().getType() != card.getType()))
		{
			this.ShowCardRule(card);
		}
	}

	// Token: 0x06001009 RID: 4105 RVA: 0x0000C9A5 File Offset: 0x0000ABA5
	public void ItemClicked(CardListPopup popup, Card card)
	{
		if (this.deckStatsPane.IsOpen())
		{
			return;
		}
		if (card != null)
		{
			this.ShowCardRule(card);
		}
	}

	// Token: 0x0600100A RID: 4106 RVA: 0x000028DF File Offset: 0x00000ADF
	public void ItemButtonClicked(CardListPopup popup, Card card)
	{
	}

	// Token: 0x04000C64 RID: 3172
	private const float CARD_PARTIAL_TRANSPARENCY = 0.4f;

	// Token: 0x04000C65 RID: 3173
	private const int CMASK_CARD = 4;

	// Token: 0x04000C66 RID: 3174
	private const float ZBackground = 950f;

	// Token: 0x04000C67 RID: 3175
	private const float ZCards = 0.4f;

	// Token: 0x04000C68 RID: 3176
	private readonly List<CardView> cardSelection = new List<CardView>();

	// Token: 0x04000C69 RID: 3177
	private readonly List<CardView> fadingOut = new List<CardView>();

	// Token: 0x04000C6A RID: 3178
	private readonly List<CardView> fadingIn = new List<CardView>();

	// Token: 0x04000C6B RID: 3179
	private readonly List<int> pendingCardTypes = new List<int>();

	// Token: 0x04000C6C RID: 3180
	private bool inited;

	// Token: 0x04000C6D RID: 3181
	private Vector3 _mainCameraPosition;

	// Token: 0x04000C6E RID: 3182
	protected Gui3D gui3d;

	// Token: 0x04000C6F RID: 3183
	public EList<Card> allPicks;

	// Token: 0x04000C70 RID: 3184
	private CardListPopup allPicksList;

	// Token: 0x04000C71 RID: 3185
	private CardView lastSelected;

	// Token: 0x04000C72 RID: 3186
	private int cardsPerRow;

	// Token: 0x04000C73 RID: 3187
	private int targetCollectionSize;

	// Token: 0x04000C74 RID: 3188
	private GameObject lightSource;

	// Token: 0x04000C75 RID: 3189
	private GUISkin regularUI;

	// Token: 0x04000C76 RID: 3190
	private DeckStatsPane deckStatsPane;

	// Token: 0x04000C77 RID: 3191
	protected Rect rectCard;

	// Token: 0x04000C78 RID: 3192
	protected Rect rectSubMenu;

	// Token: 0x04000C79 RID: 3193
	protected Rect rectCardList;

	// Token: 0x04000C7A RID: 3194
	private int lastScreenWidth;

	// Token: 0x04000C7B RID: 3195
	private int lastScreenHeight;

	// Token: 0x04000C7C RID: 3196
	private bool acceptingInput = true;

	// Token: 0x04000C7D RID: 3197
	private CardView lastHovered;

	// Token: 0x04000C7E RID: 3198
	private GameObject cardRule;

	// Token: 0x04000C7F RID: 3199
	private long lastCardRuleTypeId = -1L;
}
