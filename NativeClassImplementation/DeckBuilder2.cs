using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeckExtensionMethods;
using Gui;
using JsonFx.Json;
using UnityEngine;

// Token: 0x02000191 RID: 401
public class DeckBuilder2 : AbstractCommListener, ScrollBook.IListener, IOkCallback, ICancelCallback, IOkCancelCallback, IOkStringCallback, IOkStringCancelCallback, IDeckSaveCallback, IDeckCallback
{
	// Token: 0x06000C6D RID: 3181 RVA: 0x000576E4 File Offset: 0x000558E4
	private float getNextZ()
	{
		return this.currentTableCardZ -= 0.05f;
	}

	// Token: 0x06000C6E RID: 3182 RVA: 0x00057708 File Offset: 0x00055908
	private void setSortMode(DeckBuilder2.SortMode mode, bool reverse)
	{
		App.Config.settings.deck_builder.sort_order.value = mode;
		this.collectionSorter.clear();
		if (mode == DeckBuilder2.SortMode.Cost)
		{
			if (reverse)
			{
				this.collectionSorter.byResourceCountDesc();
			}
			else
			{
				this.collectionSorter.byResourceCount();
			}
		}
		this.collectionSorter.byName().byLevel().byTradable();
	}

	// Token: 0x06000C6F RID: 3183 RVA: 0x0005777C File Offset: 0x0005597C
	protected IEnumerator Start()
	{
		this.gui3d = new Gui3D(Camera.main);
		DeckBuilder2._mainCameraPosition = Camera.main.transform.position;
		this.comm = App.Communicator;
		this.comm.addListener(this);
		SceneLoader.OnSceneWillUnload += this.OnSceneWillUnload;
		if (App.SceneValues.deckBuilder != null && App.SceneValues.deckBuilder.isLimitedMode)
		{
			this.mode = ((App.SceneValues.deckBuilder.limitedDeckInfo == null) ? DeckBuilder2.BuilderMode.LIMITED : DeckBuilder2.BuilderMode.VIEW_LIMITED);
			this.tableCardsNeededForDeck = 30;
		}
		if (this.mode == DeckBuilder2.BuilderMode.LIMITED)
		{
			this.loadSaveTableState = false;
			this.comm.send(new GetCollectionLimitedMessage());
		}
		else if (this.mode == DeckBuilder2.BuilderMode.VIEW_LIMITED)
		{
			this.loadSaveTableState = false;
			LimitedDeckInfo ldi = App.SceneValues.deckBuilder.limitedDeckInfo;
			this.comm.send(new DeckCardsLimitedMessage(ldi.name));
		}
		else
		{
			this.comm.send(new LibraryViewMessage());
			this.comm.send(new DeckListMessage());
		}
		bool isDebug = App.SceneValues.deckBuilder == null;
		if (isDebug)
		{
			App.AssetLoader.Init();
			string filename = Application.persistentDataPath + "/db2log.txt";
			string data = FileUtil.readFileContents(filename);
			App.Communicator.setData(data);
		}
		this.sharedShadowMaterial = new Material(ResourceManager.LoadShader("Transparent/Diffuse"));
		this.sharedShadowMaterial.mainTexture = ResourceManager.LoadTexture("DeckBuilder/scroll_shadow");
		this.audioScript = App.AudioScript;
		this.audioScript.StartSimultaneousSilentMusic(DeckBuilder2.musicFiles);
		this.skinScrollbar = (GUISkin)ResourceManager.Load("_GUISkins/HorizontalSlider");
		this.buttonSkin = (GUISkin)ResourceManager.Load("_GUISkins/Lobby");
		this.pulldownSkin = ScriptableObject.CreateInstance<GUISkin>();
		this.pulldownSkin.button = new GUIStyle(this.buttonSkin.button);
		this.pulldownSkin.label = new GUIStyle(this.buttonSkin.label);
		this.pulldownSkin.button.normal.background = ResourceManager.LoadTexture("ChatUI/button_160a");
		GUIStyle button = this.pulldownSkin.button;
		TextAnchor alignment = 3;
		this.pulldownSkin.label.alignment = alignment;
		button.alignment = alignment;
		this.plaqueItemSkin = (GUISkin)ResourceManager.Load("_GUISkins/Plaque");
		this.textFieldSkin = (GUISkin)ResourceManager.Load("_GUISkins/TextEntrySkin");
		this.textFieldSkin.textField.margin.right = 0;
		this.textFieldSkin.textField.padding.right = 0;
		this.expandSkin = (GUISkin)ResourceManager.Load("_GUISkins/DbExpand");
		this.contractSkin = (GUISkin)ResourceManager.Load("_GUISkins/DbContract");
		this.pileSizeSkin = (GUISkin)ResourceManager.Load("_GUISkins/Plaque");
		this.pileSizeSkin.label.alignment = 4;
		this.deckBuilderGuiSkin = ScriptableObject.CreateInstance<GUISkin>();
		this.deckBuilderGuiSkin.box = new GUIStyle(((GUISkin)ResourceManager.Load("_GUISkins/RegularUI")).box);
		this.deckBuilderGuiSkin.label.wordWrap = true;
		this.deckBuilderGuiSkin.label.font = (Font)ResourceManager.Load("Fonts/HoneyMeadBB_bold", typeof(Font));
		this.deckBuilderGuiSkin.label.fontSize = 24;
		this.deckBuilderGuiSkin.label.alignment = 1;
		this.deckBuilderGuiSkin.label.normal.textColor = new Color(1f, 1f, 1f, 1f);
		this.deckBuilderGuiSkin.textField.font = (Font)ResourceManager.Load("Fonts/HoneyMeadBB_bold", typeof(Font));
		this.deckBuilderGuiSkin.textField.fontSize = 20;
		this.deckBuilderGuiSkin.textField.normal.textColor = new Color(1f, 1f, 1f, 1f);
		this.deckBuilderGuiSkin.textArea.fontStyle = 1;
		this.deckBuilderGuiSkin.textArea.normal.textColor = new Color(0.8156863f, 0.63529414f, 0.37254903f, 1f);
		this.deckBuilderGuiSkin.textArea.fontStyle = 1;
		this.deckBuilderGuiSkin.textArea.font = (Font)ResourceManager.Load("Fonts/HoneyMeadBB_bold", typeof(Font));
		this.deckBuilderGuiSkin.textArea.wordWrap = true;
		this.cardOverlay = new GameObject("Card Overlay").AddComponent<CardOverlay>();
		this.cardOverlay.Init(this.cardRenderTexture, 5);
		this.cardOverlay.SetHideOverlayOnClick(true);
		base.name = "_DeckBuilder";
		this.setupScene();
		float yBtnPos = 0.18f * (float)Screen.height;
		float yBtnSpace = 0.054f * (float)Screen.height;
		this.scrollBook = base.gameObject.AddComponent<ScrollBook>();
		this.scrollBook.init(this.rectLeft, 880f);
		this.scrollBook.setBoundingRect(this.rectBook);
		this.scrollBook.setListener(this);
		ResourceType[] resources = new ResourceType[]
		{
			ResourceType.ORDER,
			ResourceType.ENERGY,
			ResourceType.GROWTH,
			ResourceType.DECAY
		};
		this.resourceFilterGroup = new ResourceFilterGroup(0.005f * (float)Screen.width, yBtnPos, yBtnSpace, resources);
		this.sortGroup = new ButtonGroup(false, this.rectRight.x - 0.12f * (float)Screen.height, yBtnPos, yBtnSpace, "Sort");
		this.sortGroup.addItem("Name", true);
		this.sortGroup.addItem("Cost", false);
		DeckBuilder2.SortMode sortEnumValue = App.Config.settings.deck_builder.sort_order.value;
		this.sortGroup.setSelected((int)sortEnumValue);
		this.setSortMode(App.Config.settings.deck_builder.sort_order, false);
		this.updateTableStats();
		App.ChatUI.Show(false);
		base.StartCoroutine(this.fadeInAfterWait(0.2f));
		if (App.SceneValues.deckBuilder != null)
		{
			App.SceneValues.deckBuilder.markRead();
		}
		yield return null;
		yield break;
	}

	// Token: 0x06000C70 RID: 3184 RVA: 0x00057798 File Offset: 0x00055998
	private IEnumerator fadeInAfterWait(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		App.LobbyMenu.fadeInScene();
		yield break;
	}

	// Token: 0x06000C71 RID: 3185 RVA: 0x000577BC File Offset: 0x000559BC
	protected void OnSceneWillUnload(string sceneName)
	{
		if (sceneName != "_DeckBuilderView")
		{
			return;
		}
		SceneValues.SV_DeckBuilder deckBuilder = App.SceneValues.deckBuilder;
		if (this.loadSaveTableState && deckBuilder != null)
		{
			Message message = this.generateDeckSaveMessage(this.loadedDeckName, false);
			deckBuilder.tableState = new MockedDeckCardsMessage((DeckSaveMessage)message);
		}
	}

	// Token: 0x06000C72 RID: 3186 RVA: 0x0000A1DF File Offset: 0x000083DF
	public override void OnDestroy()
	{
		base.OnDestroy();
		App.Communicator.removeListener(this);
		SceneLoader.OnSceneWillUnload -= this.OnSceneWillUnload;
	}

	// Token: 0x06000C73 RID: 3187 RVA: 0x00057818 File Offset: 0x00055A18
	public void handleMessage(DeckCardsMessage m)
	{
		if (m.cards.Length == 0)
		{
			return;
		}
		if (this.mode == DeckBuilder2.BuilderMode.VIEW_LIMITED)
		{
			this.onLibraryReceived(m.cards);
		}
		DepletingMultiMapQuery<long, Vector3> depletingMultiMapQuery = null;
		if (m.metadata != null)
		{
			depletingMultiMapQuery = new DepletingMultiMapQuery<long, Vector3>();
			try
			{
				string text = m.metadata.Replace('\'', '"');
				JsonReader jsonReader = new JsonReader();
				Dictionary<string, object> dictionary = (Dictionary<string, object>)jsonReader.Read(text);
				string text2 = (string)dictionary["pos"];
				foreach (string text3 in text2.Split(new char[]
				{
					'|'
				}))
				{
					string[] array2 = text3.Split(new char[]
					{
						','
					});
					long id = long.Parse(array2[0]);
					int num = int.Parse(array2[1]);
					int num2 = int.Parse(array2[2]);
					Vector3 obj;
					obj..ctor(0.001f * (float)num, 0.001f * (float)num2, this.getNextZ());
					depletingMultiMapQuery.Add(id, obj);
				}
			}
			catch (Exception)
			{
			}
		}
		this.loadDeck(m.deck, m.cards, depletingMultiMapQuery);
	}

	// Token: 0x06000C74 RID: 3188 RVA: 0x00057954 File Offset: 0x00055B54
	public override void handleMessage(Message msg)
	{
		if (msg is DeckListMessage)
		{
			this.decks = new List<DeckInfo>(((DeckListMessage)msg).GetAllDecks());
		}
		if (msg is LibraryViewMessage)
		{
			this.onLibraryReceived(((LibraryViewMessage)msg).cards);
		}
		if (msg is GetCollectionLimitedMessage)
		{
			this.onLibraryReceived(((GetCollectionLimitedMessage)msg).cards);
		}
		if (msg is DeckValidateMessage)
		{
			this.saveDeckInformation = ((DeckValidateMessage)msg).getErrorString(2);
			this.showNameDeck();
		}
		if (msg is LoadLabDeckMessage)
		{
			LoadLabDeckMessage loadLabDeckMessage = (LoadLabDeckMessage)msg;
			this.importDeck(new DeckPortation.Deck(loadLabDeckMessage.types));
		}
		if (msg is OkMessage)
		{
			OkMessage okMessage = (OkMessage)msg;
			if (okMessage.isType(typeof(DeckSaveLimitedMessage)))
			{
				App.Popups.ShowOk(this, "ok-decksavelimited", "Deck saved", "Great work! To begin using this deck, hit the Judgement button in the arena.", "Ok");
			}
		}
		if (msg is FailMessage)
		{
			FailMessage failMessage = (FailMessage)msg;
			if (failMessage.isType(typeof(DeckSaveLimitedMessage)))
			{
				App.Popups.ShowOk(this, "fail-decksavelimited", "Saving deck failed", failMessage.info, "Ok");
			}
		}
	}

	// Token: 0x06000C75 RID: 3189 RVA: 0x00057A90 File Offset: 0x00055C90
	private void onLibraryReceived(IEnumerable<Card> library)
	{
		List<Card> list = new List<Card>();
		foreach (Card card in library)
		{
			if (this.shouldAddCardToLibrary(card))
			{
				list.Add(card);
			}
		}
		this.allCards.Clear();
		this.allCardsDict.Clear();
		Dictionary<ResourceType, int> dictionary = new Dictionary<ResourceType, int>();
		foreach (Card card2 in list)
		{
			this.allCards.Add(card2);
			this.allCardsDict[card2.getId()] = card2;
			ResourceType resource = card2.getCardType().getResource();
			if (dictionary.ContainsKey(resource))
			{
				Dictionary<ResourceType, int> dictionary3;
				Dictionary<ResourceType, int> dictionary2 = dictionary3 = dictionary;
				ResourceType resourceType2;
				ResourceType resourceType = resourceType2 = resource;
				int num = dictionary3[resourceType2];
				dictionary2[resourceType] = num + 1;
			}
			else
			{
				dictionary[resource] = 1;
			}
		}
		foreach (KeyValuePair<ResourceType, int> keyValuePair in dictionary)
		{
			this.resourceFilterGroup.setResourceCount(keyValuePair.Key, keyValuePair.Value);
		}
		this.filterActiveCollection();
		SceneValues.SV_DeckBuilder deckBuilder = App.SceneValues.deckBuilder;
		if (this.loadSaveTableState && deckBuilder != null && deckBuilder.tableState != null)
		{
			this.handleMessage(deckBuilder.tableState);
			deckBuilder.tableState = null;
		}
	}

	// Token: 0x06000C76 RID: 3190 RVA: 0x00057C58 File Offset: 0x00055E58
	private void setupScene()
	{
		Camera main = Camera.main;
		TransparencySortMode transparencySortMode = 2;
		this.scrollBookCamera.transparencySortMode = transparencySortMode;
		main.transparencySortMode = transparencySortMode;
		Camera main2 = Camera.main;
		float num = 0.3f;
		this.scrollBookCamera.nearClipPlane = num;
		main2.nearClipPlane = num;
		Camera main3 = Camera.main;
		num = 1000f;
		this.scrollBookCamera.farClipPlane = num;
		main3.farClipPlane = num;
		this.deckViewParent = new GameObject("deckViewParent");
		this.deckViewParent.transform.position = new Vector3(0f, 0f, 0f);
		this.imBookPlane = PrimitiveFactory.createTexturedPlane("DeckBuilder/spellbook__base", true);
		this.imLeftBookPlane = PrimitiveFactory.createTexturedPlane("DeckBuilder/spellbook__leftside", true);
		this.imLeftBookPlane.layer = 12;
		this.imRightBookPlane = PrimitiveFactory.createTexturedPlane("DeckBuilder/spellbook__rightside", true);
		this.imRightBookPlane.layer = 12;
		this.unitStand = PrimitiveFactory.createTexturedPlane("DeckBuilder/unitstand", true);
		this.unitStand.renderer.material.color = new Color(1f, 1f, 1f, 0f);
		this.deckStatsPane = new GameObject("Deck Stats Pane").AddComponent<DeckStatsPane>();
		this.deckStatsPane.Init();
		this.lastScreenWidth = -9999;
		this.checkScreenResolutionChanged(true);
		this.inited = true;
		Debug.Log("Init done!");
	}

	// Token: 0x06000C77 RID: 3191 RVA: 0x00057DBC File Offset: 0x00055FBC
	private void checkScreenResolutionChanged(bool forceChange)
	{
		if (forceChange || this.lastScreenWidth != Screen.width || this.lastScreenHeight != Screen.height)
		{
			this.lastScreenWidth = Screen.width;
			this.lastScreenHeight = Screen.height;
			this.onResolutionChanged();
		}
	}

	// Token: 0x06000C78 RID: 3192 RVA: 0x00057E0C File Offset: 0x0005600C
	protected virtual void setupPositions()
	{
		Camera main = Camera.main;
		float orthographicSize = (float)Screen.height * 0.5f;
		this.scrollBookCamera.orthographicSize = orthographicSize;
		main.orthographicSize = orthographicSize;
		if (AspectRatio.now.isWider(AspectRatio._4_3) && AspectRatio.now.isNarrower(AspectRatio._16_9))
		{
			this.buttonIndexOffset = -0.45f;
		}
		this.CardSpacing = 0.0075f * (float)Screen.height;
		this.subMenuRect = App.LobbyMenu.getSubMenuRect(1f);
		float num = this.subMenuRect.y + this.subMenuRect.height;
		float num2 = (float)Screen.height * 0.98f - num;
		float num3 = (float)Screen.height * 0.017f;
		Vector3 vector = CardView.CardLocalScale();
		float num4 = vector.x / vector.z;
		float num5 = (float)Screen.height * 0.005f;
		float num6 = (float)Screen.height * 0.53f;
		float num7 = num4 * num6;
		float num8 = (float)Screen.width * 0.3f;
		if (num7 > num8)
		{
			num7 = num8;
			num6 = num8 / num4;
		}
		this.rectCard = new Rect(0f, 0f, num7, num6);
		this.rectCard.x = (float)Screen.width - this.rectCard.width - num5 * 1.7f;
		float num9 = this.rectCard.x - num5;
		this.rectRight = new Rect(num9, num, (float)Screen.width - num9, num2);
		this.rectLeft = new Rect(0f, num, num9, num2 * 0.4f);
		this.rectBook = new Rect(this.rectLeft);
		this.rectBook.height = this.rectBook.height - 2f * num3;
		this.rectScroll = GeomUtil.scaleCentered(this.rectLeft, 0.3f, 1f);
		this.rectScroll.height = num3;
		this.rectScroll.y = this.rectLeft.yMax - 2f * num3;
		this.rectCard.y = this.rectRight.y + this.rectCard.height * 0.065f;
		float num10 = this.rectLeft.y + this.rectLeft.height;
		this.rectTable = GeomUtil.scaleCentered(new Rect(0f, num10, this.rectRight.x, (float)Screen.height * 0.95f - num10), 0.88f, 0.55f);
		Rect rect = GeomUtil.scaleCentered(this.rectLeft, 0.95f, 0.9f);
		Rect dst;
		dst..ctor(rect);
		dst.width *= 0.133426f;
		Rect dst2;
		dst2..ctor(rect);
		dst2.width *= 0.133426f;
		dst2.x = rect.xMax - dst2.width;
		float num11 = Mathf.Lerp(dst.x, dst.xMax, 0.3f) / (float)Screen.width;
		float num12 = Mathf.Lerp(dst2.x, dst2.xMax, 0.7f) / (float)Screen.width;
		Rect rect2;
		rect2..ctor(num11, 0f, num12 - num11, 1f);
		GUIUtil.setScissorRect(this.scrollBookCamera, rect2);
		UnityCameraClipFix unityCameraClipFix = base.gameObject.AddComponent<UnityCameraClipFix>();
		unityCameraClipFix.init(this.scrollBookCamera, rect2, false);
		this.gui3d.pushTransform();
		this.gui3d.translate(0f, (float)Screen.height * 0.01f);
		this.gui3d.setDepth(920f);
		this.gui3d.DrawObject(rect, this.imBookPlane);
		this.gui3d.setDepth(850f);
		this.gui3d.DrawObject(dst, this.imLeftBookPlane);
		this.gui3d.DrawObject(dst2, this.imRightBookPlane);
		this.gui3d.popTransform();
		this.gui3d.setDepth(0.35f);
		Rect rect3 = this.mock2048.rAspectH(new Rect(0f, 0f, 211.5f, 116f));
		rect3.y = (float)Screen.height * 0.87f;
		rect3 = GeomUtil.getCentered(rect3, this.rectRight, true, false);
		this.gui3d.DrawObject(rect3, this.unitStand);
		this.unitStand.name = "unitstand";
		if (this.scrollBook != null)
		{
			this.scrollBook.setRect(this.rectLeft);
			this.scrollBook.setBoundingRect(this.rectBook);
		}
		this.rectUnit = this.rectCard;
		this.rectUnit.y = this.rectCard.yMax;
		this.rectUnit.yMax = this.rectRight.yMax;
		Rect squareRect = GUIUtil.screen();
		int num13 = (Screen.width - Screen.height) / 2;
		squareRect.xMin += (float)num13;
		squareRect.xMax -= (float)num13;
		this.cardOverlay.SetSquareRect(squareRect);
	}

	// Token: 0x06000C79 RID: 3193 RVA: 0x0000A203 File Offset: 0x00008403
	public void onResolutionChanged()
	{
		this.setupPositions();
	}

	// Token: 0x06000C7A RID: 3194 RVA: 0x0000A20B File Offset: 0x0000840B
	private void stepCollection(int num)
	{
		this.scrollBook.scrollTo(this.scrollBook.scrollPos() + (float)num);
	}

	// Token: 0x06000C7B RID: 3195 RVA: 0x00058338 File Offset: 0x00056538
	private DeckCard createDeckCard(Card card, Vector3 at)
	{
		GameObject gameObject = PrimitiveFactory.createPlane(true);
		ICardView cardView = (ICardView)gameObject.AddComponent(DeckBuilder2.CardViewClass);
		gameObject.transform.position = at;
		gameObject.transform.eulerAngles = new Vector3(0f, 180f, 0f);
		if (DeckBuilder2.CardViewClass == typeof(CardViewCached))
		{
			Vector3 localScale = this.getTableCardScale() * 1.5f;
			localScale.z *= 0.7f;
			gameObject.transform.localScale = localScale;
			CardViewCached cardViewCached = (CardViewCached)cardView;
			cardViewCached.init(card);
		}
		if (DeckBuilder2.CardViewClass == typeof(CardView2))
		{
			Vector3 localScale2 = this.getTableCardScale() * 1.5f;
			localScale2.z *= 0.7f;
			gameObject.transform.localScale = localScale2;
			CardView2 cardView2 = (CardView2)cardView;
			cardView2.init(card);
		}
		if (DeckBuilder2.CardViewClass == typeof(CardView))
		{
			gameObject.transform.localScale = this.getTableCardScale();
			CardView cardView3 = (CardView)cardView;
			cardView3.setTooltipEnabled(false);
			cardView3.setShader("Scrolls/Unlit/Transparent/ZWrite");
			cardView3.init(null, card, -1);
		}
		gameObject.layer = 4;
		gameObject.name = "Card";
		this.onCardEnterView(card, cardView);
		return new DeckCard(cardView);
	}

	// Token: 0x06000C7C RID: 3196 RVA: 0x0005849C File Offset: 0x0005669C
	private void addShadow(GameObject g)
	{
		GameObject gameObject = PrimitiveFactory.createPlane(false);
		gameObject.renderer.sharedMaterial = this.sharedShadowMaterial;
		gameObject.transform.parent = g.transform;
		gameObject.transform.localPosition = new Vector3(--0f, -0.1f, 0.4f);
		gameObject.name = "shadow";
	}

	// Token: 0x06000C7D RID: 3197 RVA: 0x000584FC File Offset: 0x000566FC
	private List<DeckCard> createDeckCards(List<Card> cards, Vector3 at)
	{
		return Enumerable.ToList<DeckCard>(Enumerable.Select<Card, DeckCard>(cards, (Card x) => this.createDeckCard(x, at)));
	}

	// Token: 0x06000C7E RID: 3198 RVA: 0x0000A226 File Offset: 0x00008426
	protected virtual Vector3 getTableCardScale()
	{
		return this.defaultTableCardScale();
	}

	// Token: 0x06000C7F RID: 3199 RVA: 0x0000A22E File Offset: 0x0000842E
	protected Vector3 defaultTableCardScale()
	{
		return CardView.CardLocalScale(Camera.main.orthographicSize / 4f);
	}

	// Token: 0x06000C80 RID: 3200 RVA: 0x00058534 File Offset: 0x00056734
	protected int deleteCard(Card card)
	{
		if (this.activeCards.Contains(card))
		{
			string text = "Can't remove cards from library Scrollbook, only table";
			throw new NotImplementedException(text);
		}
		if (!this.allCards.Remove(card))
		{
			return 0;
		}
		if (this.removeCardFromTable(card))
		{
			return 2;
		}
		return 1;
	}

	// Token: 0x06000C81 RID: 3201 RVA: 0x00004AAC File Offset: 0x00002CAC
	protected virtual bool isFilterAffectingTable()
	{
		return true;
	}

	// Token: 0x06000C82 RID: 3202 RVA: 0x00004AAC File Offset: 0x00002CAC
	protected virtual bool shouldAddCardToLibrary(Card card)
	{
		return true;
	}

	// Token: 0x06000C83 RID: 3203 RVA: 0x0000A245 File Offset: 0x00008445
	private bool filterActiveCollection()
	{
		return this.filterActiveCollection(false);
	}

	// Token: 0x06000C84 RID: 3204 RVA: 0x00058584 File Offset: 0x00056784
	private bool filterActiveCollection(bool forceChanged)
	{
		if (this.allCards.Count == 0)
		{
			Log.info("User does not have ANY scrolls");
			return false;
		}
		List<Card> old = this.activeCards;
		IEnumerable<long> onTable = Enumerable.Select<DeckCard, long>(this.tableCards, (DeckCard dc) => dc.card.getCardInfo().getId());
		IEnumerable<long> inAir = Enumerable.Select<DeckCard, long>(this.draggedCards, (DeckCard dc) => dc.card.getCardInfo().getId());
		IEnumerable<Card> enumerable = Enumerable.Where<Card>(this.allCards, delegate(Card card)
		{
			ResourceType resource = card.getCardType().getResource();
			return this.resourceFilterGroup.isSelected(resource) && !Enumerable.Contains<long>(onTable, card.getId()) && !Enumerable.Contains<long>(inAir, card.getId());
		});
		string s = (!(this.searchString == "Search   ")) ? this.searchString : string.Empty;
		CardFilter cardFilter = CardFilter.from(s);
		this.activeCards = cardFilter.getFiltered(Enumerable.ToList<Card>(enumerable));
		this.sortActiveCollection();
		this.activeTableCardsCount = 0;
		if (this.isFilterAffectingTable())
		{
			HashSet<long> hashSet = new HashSet<long>(Enumerable.Select<DeckCard, long>(cardFilter.getFiltered<DeckCard>(this.tableCards, (DeckCard d) => d.card.getCardInfo()), (DeckCard d) => d.card.getCardInfo().id));
			foreach (DeckCard deckCard in this.tableCards)
			{
				Card cardInfo = deckCard.card.getCardInfo();
				bool flag = !this.hasFilter() || hashSet.Contains(cardInfo.id);
				deckCard.isFiltered = flag;
				deckCard.card.renderAsEnabled(flag, 0.1f);
			}
			this.activeTableCardsCount = hashSet.Count;
		}
		bool flag2 = forceChanged || DeckBuilder2.hasCollectionChanged(old, this.activeCards);
		if (flag2)
		{
			this.scrollBook.setCards(this.activeCards);
			HashSet<int> hashSet2 = new HashSet<int>();
			foreach (Card card2 in this.activeCards)
			{
				hashSet2.Add(card2.typeId);
			}
			this.activeUniqueTypes = hashSet2.Count;
		}
		return flag2;
	}

	// Token: 0x06000C85 RID: 3205 RVA: 0x0005881C File Offset: 0x00056A1C
	private static bool hasCollectionChanged(List<Card> old, List<Card> now)
	{
		if (old == null || old.Count != now.Count)
		{
			return true;
		}
		for (int i = 0; i < now.Count; i++)
		{
			if (now[i].getId() != old[i].getId())
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000C86 RID: 3206 RVA: 0x0000A24E File Offset: 0x0000844E
	private void sortActiveCollection()
	{
		if (this.activeCards.Count <= 1)
		{
			return;
		}
		this.activeCards.Sort(this.collectionSorter);
	}

	// Token: 0x06000C87 RID: 3207 RVA: 0x0005887C File Offset: 0x00056A7C
	private void insertCardSorted(DeckCard card)
	{
		int sortedInsertionIndex = CollectionUtil.getSortedInsertionIndex<Card>(this.activeCards, card.card.getCardInfo(), this.collectionSorter);
		this.activeCards.Insert(sortedInsertionIndex, card.card.getCardInfo());
	}

	// Token: 0x06000C88 RID: 3208 RVA: 0x0000A273 File Offset: 0x00008473
	private void showNameDeck()
	{
		App.Popups.ShowSaveDeck(this, this.loadedDeckName, this.saveDeckInformation);
	}

	// Token: 0x06000C89 RID: 3209 RVA: 0x0000A28C File Offset: 0x0000848C
	private void saveDeck(string name, bool saveAIDeck)
	{
		Log.info("Save Deck");
		this.comm.send(this.generateDeckSaveMessage(name, saveAIDeck));
		this.comm.send(new DeckListMessage());
	}

	// Token: 0x06000C8A RID: 3210 RVA: 0x000588C0 File Offset: 0x00056AC0
	private Message generateDeckSaveMessage(string name, bool saveAIDeck)
	{
		List<DeckCard> list = new List<DeckCard>(this.tableCards);
		list.Sort((DeckCard a, DeckCard b) => -a.t.position.z.CompareTo(b.t.position.z));
		long[] array = new long[list.Count];
		string[] array2 = new string[list.Count];
		for (int i = 0; i < list.Count; i++)
		{
			ICardView card = list[i].card;
			array[i] = card.getCardInfo().getId();
			Vector3 vector = DeckBuilder2.worldToCamera(list[i].t.position);
			int num = (int)(1000f * Mth.clamp(this.getUnitForTableXPos(vector.x), 0f, 1f));
			int num2 = (int)(1000f * Mth.clamp(1f - this.getUnitForTableYPos(vector.y), 0f, 1f));
			array2[i] = string.Concat(new object[]
			{
				card.getCardInfo().getId(),
				",",
				num,
				",",
				num2
			});
		}
		string text = string.Join("|", array2);
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary["pos"] = text;
		string metadata = new JsonWriter().Write(dictionary).Replace('"', '\'');
		if (this.mode == DeckBuilder2.BuilderMode.LIMITED)
		{
			return new DeckSaveLimitedMessage(name, array, metadata);
		}
		if (saveAIDeck)
		{
			return new SaveLabDeckMessage(name, array);
		}
		return new DeckSaveMessage(name, array, metadata);
	}

	// Token: 0x06000C8B RID: 3211 RVA: 0x00058A5C File Offset: 0x00056C5C
	private void deleteDeck(string deckname, bool isAiDeck)
	{
		if (isAiDeck)
		{
			this.comm.send(new DeleteLabDeckMessage(deckname));
		}
		else
		{
			this.comm.send(new DeckDeleteMessage(deckname));
		}
		this.comm.send(new DeckListMessage());
	}

	// Token: 0x06000C8C RID: 3212 RVA: 0x00058AAC File Offset: 0x00056CAC
	private void loadDeck(string deckName, Card[] cards, DepletingMultiMapQuery<long, Vector3> positionMap)
	{
		this.audioScript.PlaySFX("Sounds/hyperduck/DeckBuilder/db_deck_shuffle");
		Object.Destroy(GameObject.Find("guiBlocker"));
		this.loadedDeckName = ((deckName == null) ? string.Empty : deckName);
		foreach (DeckCard deckCard in this.tableCards)
		{
			if (deckCard.t != null && deckCard.t.gameObject != null)
			{
				Object.Destroy(deckCard.t.gameObject);
			}
		}
		this.tableCards.Clear();
		int num = 0;
		foreach (Card card in cards)
		{
			long id = card.getId();
			if (this.allCardsDict.ContainsKey(id))
			{
				DeckCard deckCard2 = this.createDeckCard(this.allCardsDict[id], Vector3.zero);
				this.gui3d.orientPlane(deckCard2.t.gameObject);
				this.tableCards.Add(deckCard2);
			}
			else
			{
				num++;
			}
		}
		if (num > 0)
		{
			App.Popups.ShowOk(this, "missing-scrolls", "Missing scrolls", "Deck is missing " + num + " scrolls.", "Ok");
		}
		this.markTableChanged();
		this.currentTableCardZ = 850f;
		if (positionMap != null)
		{
			for (int j = 0; j < this.tableCards.Count; j++)
			{
				DeckCard deckCard3 = this.tableCards[j];
				long id2 = deckCard3.card.getCardInfo().getId();
				float nextZ = this.getNextZ();
				Vector3 vector = (!positionMap.hasNext(id2)) ? new Vector3(Random.Range(0f, 1f), Random.Range(0f, 1f), nextZ) : positionMap.getNext(id2);
				Vector3 w;
				w..ctor(this.getTableXPos(vector.x), this.getTableYPos(1f - vector.y), vector.z);
				deckCard3.t.position = DeckBuilder2.cameraToWorld(w);
			}
		}
		else
		{
			this.alignTableCards(DeckBuilder2.TableAlignment.Stacked, this.getDefaultSorter());
		}
		this.filterActiveCollection(cards.Length == 0);
		this.updateTableStats();
	}

	// Token: 0x06000C8D RID: 3213 RVA: 0x0000A2BD File Offset: 0x000084BD
	private float getTableXPos(float unit)
	{
		return this.rectTable.x + unit * this.rectTable.width;
	}

	// Token: 0x06000C8E RID: 3214 RVA: 0x0000A2D8 File Offset: 0x000084D8
	private float getTableYPos(float unit)
	{
		return this.rectTable.y + unit * this.rectTable.height;
	}

	// Token: 0x06000C8F RID: 3215 RVA: 0x0000A2F3 File Offset: 0x000084F3
	private float getUnitForTableXPos(float x)
	{
		return (x - this.rectTable.x) / this.rectTable.width;
	}

	// Token: 0x06000C90 RID: 3216 RVA: 0x0000A30E File Offset: 0x0000850E
	private float getUnitForTableYPos(float y)
	{
		return (y - this.rectTable.y) / this.rectTable.height;
	}

	// Token: 0x06000C91 RID: 3217 RVA: 0x00058D3C File Offset: 0x00056F3C
	private void drawShadowLabel(Rect r, string s, GUIStyle style)
	{
		int fontSize = style.fontSize;
		Color textColor = style.normal.textColor;
		style.normal.textColor = Color.black;
		GUI.Label(new Rect(r.x + (float)((fontSize <= 30) ? 1 : 2), r.y + (float)((fontSize <= 30) ? 1 : 2), r.width, r.height), s, style);
		style.normal.textColor = textColor;
		GUI.Label(r, s, style);
	}

	// Token: 0x06000C92 RID: 3218 RVA: 0x00058DCC File Offset: 0x00056FCC
	protected virtual void OnGUI_drawTableGUI()
	{
		float num = (float)Screen.height * 0.92f;
		float y = num + (float)Screen.height * 0.02f;
		GUIPositioner guipositioner = new GUIPositioner(1f, 0f, 1.5f * (float)Screen.height, (float)Screen.height * 0.04f);
		float num2 = -0.22f;
		this.drawShadowLabel(guipositioner.getButtonRect(0.39f + num2, num), "Creatures:", this.deckBuilderGuiSkin.textArea);
		this.drawShadowLabel(guipositioner.getButtonRect(0.48f + num2, num), "Spells:", this.deckBuilderGuiSkin.textArea);
		this.drawShadowLabel(guipositioner.getButtonRect(0.39f + num2, y), "Structures:", this.deckBuilderGuiSkin.textArea);
		this.drawShadowLabel(guipositioner.getButtonRect(0.48f + num2, y), "Ench.:", this.deckBuilderGuiSkin.textArea);
		this.drawShadowLabel(guipositioner.getButtonRect(0.545f + num2, num), "Total scrolls:", this.deckBuilderGuiSkin.textArea);
		this.deckBuilderGuiSkin.textArea.fontSize = Screen.height / 38;
		this.drawShadowLabel(guipositioner.getButtonRect(0.46f + num2, num), string.Empty + this.numCreatures, this.deckBuilderGuiSkin.label);
		this.drawShadowLabel(guipositioner.getButtonRect(0.525f + num2, num), string.Empty + this.numSpells, this.deckBuilderGuiSkin.label);
		this.drawShadowLabel(guipositioner.getButtonRect(0.46f + num2, y), string.Empty + this.numStructures, this.deckBuilderGuiSkin.label);
		this.drawShadowLabel(guipositioner.getButtonRect(0.525f + num2, y), string.Empty + this.numEnchantments, this.deckBuilderGuiSkin.label);
		this.drawShadowLabel(guipositioner.getButtonRect(0.625f + num2, num), string.Concat(new object[]
		{
			string.Empty,
			this.tableCards.Count,
			" / ",
			this.tableCardsNeededForDeck
		}), this.deckBuilderGuiSkin.label);
		if (this.filterStatsAlpha.get() > 0f)
		{
			Color color;
			color..ctor(1f, 1f, 1f, 0.25f * this.filterStatsAlpha.get());
			GUI.color = color;
			Rect buttonRect = guipositioner.getButtonRect(0.67f + num2 + 0.01f, num);
			Rect buttonRect2 = guipositioner.getButtonRect(0.78f + num2 + 0.01f, num);
			Rect r = buttonRect;
			r.y = y;
			Rect r2 = buttonRect2;
			r2.y = y;
			float num3 = (float)Screen.height * 0.01f;
			Rect rect;
			rect..ctor(buttonRect.x - num3, buttonRect.y + 0.3f * num3, buttonRect2.x - buttonRect.x + buttonRect.height * 1.5f, r.yMax - buttonRect.y);
			GUI.Label(rect, string.Empty, this.deckBuilderGuiSkin.box);
			color.a *= 4f;
			GUI.color = color;
			this.drawShadowLabel(buttonRect, "Unique types: ", this.deckBuilderGuiSkin.textArea);
			this.drawShadowLabel(buttonRect2, string.Empty + this.activeUniqueTypes, this.deckBuilderGuiSkin.label);
			this.drawShadowLabel(r, "Table scrolls: ", this.deckBuilderGuiSkin.textArea);
			this.drawShadowLabel(r2, string.Empty + this.activeTableCardsCount, this.deckBuilderGuiSkin.label);
			GUI.color = Color.white;
		}
		float num4 = (float)Screen.height * 0.08f;
		Rect rect2;
		rect2..ctor(this.rectRight.x - num4, (float)Screen.height * 0.925f, num4, num4);
		if (GUI.Button(rect2, ResourceManager.LoadTexture("DeckBuilder/align_stack_cost")))
		{
			this.alignTableCards(DeckBuilder2.TableAlignment.Stacked, this.getDefaultSorter());
		}
		rect2.x -= rect2.width;
		if (GUI.Button(rect2, ResourceManager.LoadTexture("DeckBuilder/align_stack_alphabetical")))
		{
			this.alignTableCards(DeckBuilder2.TableAlignment.Stacked, new DeckSorter().byName().byLevelAscending());
		}
		GUI.skin = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		int fontSize = GUI.skin.button.fontSize;
		GUI.skin.button.fontSize = Screen.height / 40;
		if (GUI.Button(new Rect((float)Screen.height * 0.04f, (float)Screen.height * 0.935f, (float)Screen.height * 0.1f, (float)Screen.height * 0.035f), "Stats"))
		{
			App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
			this.deckStatsPane.Toggle();
		}
		GUI.skin.button.fontSize = fontSize;
	}

	// Token: 0x06000C93 RID: 3219 RVA: 0x000592FC File Offset: 0x000574FC
	private DeckSorter getDefaultSorter()
	{
		DeckSorter deckSorter = new DeckSorter();
		if (this.mode == DeckBuilder2.BuilderMode.NORMAL)
		{
			deckSorter = deckSorter.byColor();
		}
		return deckSorter.byResourceCount().byName().byLevelAscending();
	}

	// Token: 0x06000C94 RID: 3220 RVA: 0x00059334 File Offset: 0x00057534
	public void OnGUI()
	{
		if (!this.inited)
		{
			return;
		}
		GUI.depth = 21;
		GUI.skin = this.deckBuilderGuiSkin;
		if (Event.current.type == 7)
		{
			this.gui3d.frameBegin();
			this.OnGUI_draw3D();
			this.gui3d.frameEnd();
		}
		GUISkin skin = GUI.skin;
		GUI.skin = this.skinScrollbar;
		int num = Mathf.Max(1, this.scrollBook.getNumPiles() - 1);
		float num2 = this.scrollBook.scrollPos();
		GUI.color = new Color(1f, 1f, 1f, this.scrollbarAlpha.get());
		GUI.SetNextControlName("dbScrollbar");
		float num3 = GUIUtil.HorizontalScrollbar(this.rectScroll, num2, 0.175f * (float)num, 0f, (float)num);
		if (!Mth.isClose(num2, num3, 1E-05f))
		{
			this.scrollBook.scrollTo(num3);
			this.scrollbarTicks = 4;
		}
		this.OnGUI_drawExpandButtonAndDigits();
		if (Event.current.type == 6)
		{
			this.handleMouseScrollWheel(Input.GetAxis("Mouse ScrollWheel"));
		}
		if (Event.current.type == null)
		{
			this.handleMouseDown();
		}
		if (Event.current.type == 1)
		{
			this.handleMouseUp();
		}
		if (this.cardRule != null && App.GUI.Blocker(GeomUtil.cropShare(this.rectCard, new Rect(0f, -0.05f, 1f, 0.6f))))
		{
			this.cardOverlay.Show(this.currentCard);
		}
		GUI.color = Color.white;
		GUI.skin = skin;
		float num4 = new GUIPositioner(1f, 0f, 1.5f * (float)Screen.height, (float)Screen.height * 0.04f).getButtonRect(0.39f, 0f).x;
		GUIStyle label = this.deckBuilderGuiSkin.label;
		TextAnchor alignment = 3;
		this.deckBuilderGuiSkin.textArea.alignment = alignment;
		label.alignment = alignment;
		this.deckBuilderGuiSkin.label.fontSize = Screen.height / 20;
		this.deckBuilderGuiSkin.label.fontStyle = 1;
		GUIPositioner guipositioner = new GUIPositioner(1f, 0f, 1.5f * (float)Screen.height, (float)Screen.height * 0.04f);
		float num5 = (!AspectRatio.now.isNarrower(AspectRatio._3_2)) ? 0f : -0.035f;
		num4 = guipositioner.getButtonRect(0.39f + num5, 0f).x;
		this.deckBuilderGuiSkin.label.fontSize = Screen.height / 38;
		this.OnGUI_drawTableGUI();
		this.deckBuilderGuiSkin.textField.fontSize = 20;
		num4 = Math.Min(num4, this.capSearchWidth());
		bool flag = false;
		GUI.skin = this.plaqueItemSkin;
		int firstSelected = this.sortGroup.getFirstSelected();
		this.sortGroup.render();
		int firstSelected2 = this.sortGroup.getFirstSelected();
		if (firstSelected2 != firstSelected)
		{
			flag = true;
			this.setSortMode((DeckBuilder2.SortMode)firstSelected2, false);
		}
		int selectedBitSet = this.resourceFilterGroup.getSelectedBitSet();
		this.resourceFilterGroup.render();
		int selectedBitSet2 = this.resourceFilterGroup.getSelectedBitSet();
		if (selectedBitSet2 != selectedBitSet)
		{
			flag = true;
		}
		Vector2 screenMousePos = GUIUtil.getScreenMousePos();
		bool flag2 = this.sortGroup.isInside(screenMousePos) || this.resourceFilterGroup.isInside(screenMousePos) || App.Popups.IsShowingPopup() || (this._showSearchDropdown && this._searchDropdownBoundingRect.Contains(screenMousePos));
		this.scrollBook.setInputEnabled(!flag2);
		GUI.skin = this.buttonSkin;
		float y = 0.934f * (float)Screen.height;
		GUIPositioner buttonPositioner = LobbyMenu.getButtonPositioner(5.5f, 24f);
		Rect buttonRect = buttonPositioner.getButtonRect(0.3f, 0f, y);
		buttonRect.width *= 1.8f;
		float num6 = buttonRect.width + buttonRect.height * 2f;
		if (num6 >= num4 * 0.95f)
		{
			float num7 = num6 - num4 * 0.95f;
			buttonRect.width -= num7;
		}
		float x = (num4 - buttonRect.width - buttonRect.height * 2f) / 2f;
		buttonRect.x = x;
		GUIStyle button = GUI.skin.button;
		int num8 = Screen.height / 36;
		this.pulldownSkin.label.fontSize = num8;
		num8 = num8;
		this.pulldownSkin.button.fontSize = num8;
		num8 = num8;
		GUI.skin.label.fontSize = num8;
		button.fontSize = num8;
		this.OnGUI_drawTopbarSubmenu();
		GUI.skin = this.buttonSkin;
		GUIPositioner subMenuPositioner = App.LobbyMenu.getSubMenuPositioner(1f, 4);
		Rect buttonRect2 = subMenuPositioner.getButtonRect(3f + this.buttonIndexOffset);
		buttonRect2.width *= 1.5f;
		bool flag3 = this.OnGUI_drawSearchbar(buttonRect2);
		flag = (flag || flag3);
		if (flag)
		{
			this.audioScript.PlaySFX("Sounds/hyperduck/DeckBuilder/db_scroll_filter");
			this.filterActiveCollection();
		}
		GUI.skin = this.deckBuilderGuiSkin;
	}

	// Token: 0x06000C95 RID: 3221 RVA: 0x0000A329 File Offset: 0x00008529
	protected virtual float capSearchWidth()
	{
		return (float)Screen.width;
	}

	// Token: 0x06000C96 RID: 3222 RVA: 0x0000A331 File Offset: 0x00008531
	private void addFilteredCardsToTableAndReSort(int maxAddPerPile)
	{
		this.addFilteredCardsToTable(maxAddPerPile);
		this.alignTableCards(DeckBuilder2.TableAlignment.Stacked, this.getDefaultSorter());
	}

	// Token: 0x06000C97 RID: 3223 RVA: 0x0005989C File Offset: 0x00057A9C
	private void removeFilteredCardsFromTable(int maxRemovePerType)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		for (int i = this.tableCards.Count - 1; i >= 0; i--)
		{
			DeckCard deckCard = this.tableCards[i];
			Card cardInfo = deckCard.card.getCardInfo();
			if (deckCard.isFiltered)
			{
				int num = (!dictionary.ContainsKey(cardInfo.typeId)) ? 0 : dictionary[cardInfo.typeId];
				if (num < maxRemovePerType)
				{
					dictionary[cardInfo.typeId] = num + 1;
					this.tableCards.RemoveAt(i);
					this.markTableChanged();
					Object.Destroy(deckCard.t.gameObject);
				}
			}
		}
		this.filterActiveCollection();
	}

	// Token: 0x06000C98 RID: 3224 RVA: 0x00059964 File Offset: 0x00057B64
	private void addFilteredCardsToTable(int maxPerPile)
	{
		List<Card> list = new List<Card>();
		for (int i = this.scrollBook.getNumPiles() - 1; i >= 0; i--)
		{
			List<Card> cards = this.scrollBook.getCards(i);
			if (cards.Count != 0)
			{
				int num = Mathf.Min(maxPerPile, this.allowStartDraggingCardCount(cards[0]));
				if (num != 0)
				{
					List<Card> list2 = this.scrollBook.popCards(i, num);
					list.AddRange(list2);
				}
			}
		}
		this.putCardsOnTable(this.createDeckCards(list, Vector3.zero));
	}

	// Token: 0x06000C99 RID: 3225 RVA: 0x000599FC File Offset: 0x00057BFC
	private bool OnGUI_drawSearchbar(Rect searchbarRect)
	{
		string text = this.searchString;
		if (GUI.GetNameOfFocusedControl() == "dbSearchfield")
		{
			if (text == "Search   ")
			{
				text = string.Empty;
			}
		}
		else if (text == string.Empty)
		{
			text = "Search   ";
		}
		string text2 = text;
		GUI.SetNextControlName("dbSearchfield");
		text = GUI.TextField(searchbarRect, text, this.textFieldSkin.textField);
		if (this._setFocusState == 1)
		{
			this._setFocusState--;
			int keyboardControl = GUIUtility.keyboardControl;
			TextEditor textEditor = (TextEditor)GUIUtility.GetStateObject(typeof(TextEditor), keyboardControl);
			if (textEditor != null)
			{
				textEditor.SelectNone();
				textEditor.MoveTextEnd();
				textEditor.selectPos = 99999;
			}
		}
		if (this._setFocusState == 2)
		{
			this._setFocusState--;
			GUI.FocusControl("dbSearchfield");
		}
		GUISkin guiskin = (GUISkin)ResourceManager.Load("_GUISkins/CloseButton");
		GUIStyle guistyle = new GUIStyle(guiskin.button);
		guistyle.normal.background = ResourceManager.LoadTexture("ChatUI/dropdown_arrow");
		GUIStyleState hover = guistyle.hover;
		Texture2D background = ResourceManager.LoadTexture("ChatUI/dropdown_arrow_mouseover");
		guistyle.active.background = background;
		hover.background = background;
		Rect translated = GeomUtil.getTranslated(searchbarRect, searchbarRect.width, 0f);
		translated.height = (float)((int)(translated.height * 1.165f));
		translated.width = translated.height;
		if (GUI.Button(translated, string.Empty, guistyle))
		{
			this._showSearchDropdown = true;
		}
		if (this._showSearchDropdown)
		{
			string text3 = this.OnGUI_drawSearchPulldown(searchbarRect);
			if (text3 != null)
			{
				this._showSearchDropdown = false;
				this._setFocusState = 2;
				text = DeckBuilder2._append(text, text3.Substring(0, text3.Length - 1));
			}
		}
		translated.x += translated.width;
		if (GUI.Button(translated, string.Empty, guiskin.button))
		{
			text = string.Empty;
			this._showSearchDropdown = false;
		}
		this.searchString = text;
		return text != text2 && text != "Search   ";
	}

	// Token: 0x06000C9A RID: 3226 RVA: 0x00059C38 File Offset: 0x00057E38
	private string OnGUI_drawSearchPulldown(Rect searchbarRect)
	{
		float itemHeight = searchbarRect.height * 1.2f;
		List<GUIContent> list = new List<GUIContent>();
		list.Add(new GUIContent("   Type"));
		list.Add(new GUIContent("   Description"));
		list.Add(new GUIContent("   Rarity"));
		list.Add(new GUIContent("   Set"));
		list.Add(new GUIContent("   Attack"));
		list.Add(new GUIContent("   Countdown"));
		list.Add(new GUIContent("   Health"));
		List<string> list2 = new List<string>();
		list2.Add("t:");
		list2.Add("d:");
		list2.Add("r:");
		list2.Add("s:");
		list2.Add("ap:");
		list2.Add("cd:");
		list2.Add("hp:");
		if (this is Crafter)
		{
			list.Add(new GUIContent("   # of Scrolls"));
			list2.Add("#:");
		}
		int num = DeckBuilder2.OnGUI_drawButtonList(this.pulldownSkin, searchbarRect, true, itemHeight, list.ToArray(), list2.ToArray(), out this._searchDropdownBoundingRect);
		return (num < 0) ? null : list2[num];
	}

	// Token: 0x06000C9B RID: 3227 RVA: 0x00059D78 File Offset: 0x00057F78
	public static int OnGUI_drawButtonList(GUISkin skin, Rect baseRect, bool expandDown, float itemHeight, GUIContent[] content, string[] rightContent, out Rect boundingRect)
	{
		Rect rect;
		rect..ctor(baseRect);
		rect.height = itemHeight;
		float num = rect.height * (float)content.Length;
		if (expandDown)
		{
			rect.y += baseRect.height;
		}
		else
		{
			rect.y -= num;
		}
		boundingRect..ctor(rect.x, rect.y, rect.width, num);
		GUI.DrawTexture(boundingRect, ResourceManager.LoadTexture("ChatUI/bg_texture"));
		int result = -1;
		GUIStyle guistyle = new GUIStyle(skin.label);
		for (int i = 0; i < content.Length; i++)
		{
			if (GUI.Button(rect, content[i], skin.button))
			{
				result = i;
			}
			GUI.Label(rect, content[i], guistyle);
			if (rightContent != null)
			{
				string text = rightContent[i];
				guistyle.alignment = 5;
				GUI.Label(GeomUtil.getTranslated(rect, (float)(-(float)Screen.width) * 0.01f, 0f), text, guistyle);
				guistyle.alignment = 3;
			}
			rect.y += rect.height;
		}
		return result;
	}

	// Token: 0x06000C9C RID: 3228 RVA: 0x00059EA4 File Offset: 0x000580A4
	private static string _append(string s, string add)
	{
		if (s == "Search   ")
		{
			s = string.Empty;
		}
		s = s.TrimEnd(new char[]
		{
			' '
		});
		int num = s.LastIndexOf(' ') + 1;
		int num2 = s.LastIndexOf(':');
		if (num2 == s.Length - 1 && num - num2 <= 2)
		{
			return s.Substring(0, num) + add + ':';
		}
		if (s.Length > 0)
		{
			s += ' ';
		}
		return s + add + ":";
	}

	// Token: 0x06000C9D RID: 3229 RVA: 0x0000A347 File Offset: 0x00008547
	private void onSaveDeckClicked()
	{
		if (this.mode == DeckBuilder2.BuilderMode.NORMAL)
		{
			this.comm.send(new DeckValidateMessage(this.getTableCards()));
		}
		if (this.mode == DeckBuilder2.BuilderMode.LIMITED)
		{
			this.showNameDeck();
		}
	}

	// Token: 0x06000C9E RID: 3230 RVA: 0x00059F44 File Offset: 0x00058144
	protected virtual void OnGUI_drawTopbarSubmenu()
	{
		GUI.DrawTexture(this.subMenuRect, ResourceManager.LoadTexture("ChatUI/menu_bar_sub"));
		GUIPositioner p = App.LobbyMenu.getSubMenuPositioner(1f, 5, 140f);
		float xIndex = 0f;
		int fontSize = GUI.skin.label.fontSize;
		GUIStyle label = GUI.skin.label;
		int fontSize2 = Screen.height / 40;
		GUI.skin.button.fontSize = fontSize2;
		label.fontSize = fontSize2;
		Func<Rect> func = () => p.getButtonRect((xIndex += 1f) + this.buttonIndexOffset - 1f);
		if (this.mode != DeckBuilder2.BuilderMode.VIEW_LIMITED && LobbyMenu.drawButton(func.Invoke(), "Save Deck"))
		{
			this.onSaveDeckClicked();
		}
		if (this.mode == DeckBuilder2.BuilderMode.NORMAL && LobbyMenu.drawButton(func.Invoke(), "Load Deck") && this.decks != null)
		{
			this.showDeckSelector();
		}
		if (LobbyMenu.drawButton(func.Invoke(), "Clear Table"))
		{
			this.clearTable();
			this.scrollBook.setCards(this.activeCards);
		}
		GUI.enabled = true;
		if (LobbyMenu.drawButton(func.Invoke(), "Add Scrolls"))
		{
			long num = TimeUtil.CurrentTimeMillis();
			this.addFilteredCardsToTableAndReSort((this.mode != DeckBuilder2.BuilderMode.LIMITED) ? 1 : 100);
			Log.error("Time: " + (TimeUtil.CurrentTimeMillis() - num));
		}
		GUI.enabled = true;
		GUIStyle label2 = GUI.skin.label;
		fontSize2 = fontSize;
		GUI.skin.button.fontSize = fontSize2;
		label2.fontSize = fontSize2;
	}

	// Token: 0x06000C9F RID: 3231 RVA: 0x0000A37D File Offset: 0x0000857D
	private Card[] getTableCards()
	{
		return Enumerable.ToArray<Card>(Enumerable.Select<DeckCard, Card>(this.tableCards, (DeckCard c) => c.card.getCardInfo()));
	}

	// Token: 0x06000CA0 RID: 3232 RVA: 0x0005A0E8 File Offset: 0x000582E8
	protected virtual void handleMouseScrollWheel(float scrollDelta)
	{
		if (Mathf.Abs(scrollDelta) < 0.001f)
		{
			return;
		}
		float num = 0.2f * (float)(this.scrollBook.getNumPiles() - 1) * scrollDelta;
		if (Mathf.Abs(num) < 1f)
		{
			num = Mth.sign(num);
		}
		this.scrollBook.scrollTo(this.scrollBook.scrollPos() - Mathf.Round(num));
	}

	// Token: 0x06000CA1 RID: 3233 RVA: 0x000028DF File Offset: 0x00000ADF
	protected virtual void handleMouseDown()
	{
	}

	// Token: 0x06000CA2 RID: 3234 RVA: 0x0005A154 File Offset: 0x00058354
	protected virtual void handleMouseUp()
	{
		Vector2 screenMousePos = GUIUtil.getScreenMousePos();
		if (this._showSearchDropdown && !this._searchDropdownBoundingRect.Contains(screenMousePos))
		{
			this._showSearchDropdown = false;
		}
	}

	// Token: 0x06000CA3 RID: 3235 RVA: 0x0005A18C File Offset: 0x0005838C
	protected virtual void OnGUI_draw3D()
	{
		this.gui3d.setDepth(950f);
		Texture2D tex = ResourceManager.LoadTexture("DeckBuilder/bg");
		this.gui3d.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), tex);
		if (this.mode == DeckBuilder2.BuilderMode.LIMITED)
		{
			this.gui3d.GetLastMaterial().color = new Color(0.55f, 0.8f, 1f);
		}
		this.gui3d.setDepth(940f);
		new ScrollsFrame(this.rectLeft).SetGui(this.gui3d).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.CENTER).Draw();
		this.gui3d.setDepth(0.5f);
		new ScrollsFrame(this.rectRight).SetGui(this.gui3d).AddNinePatch(ScrollsFrame.Border.DARK_CURVED, NinePatch.Patches.CENTER).Draw();
	}

	// Token: 0x06000CA4 RID: 3236 RVA: 0x0005A274 File Offset: 0x00058474
	private void _drawDigit(int count, Vector2 pos, float scale, float xo)
	{
		Rect rect = this.mock2048.rAspectH(0f, 0f, scale * 56f, scale * 56f);
		rect.center = pos;
		rect.x += this.mock2048.X(-2f + xo);
		this.pileSizeSkin.label.fontSize = (int)(0.9f * rect.height);
		string text = count.ToString();
		GUI.Label(rect, text, this.pileSizeSkin.label);
	}

	// Token: 0x06000CA5 RID: 3237 RVA: 0x0005A308 File Offset: 0x00058508
	private void OnGUI_drawExpandButtonAndDigits()
	{
		Rect rect = GeomUtil.scaleCentered(this.mock2048.rAspectH(0f, 0f, 167f, 101f), 0.75f, 0.6f);
		rect.center = new Vector2(this.rectLeft.center.x, this.rectBook.yMax - rect.height);
		bool flag = this.scrollBook.isExpanded();
		Rect rect2;
		rect2..ctor(rect);
		Rect r;
		r..ctor(-100f, -100f, 1f, 1f);
		int num = 0;
		foreach (ScrollBook.VisibleCardData visibleCardData in this.scrollBook.getVisibleCardsData())
		{
			Vector3 vector = DeckBuilder2.worldToCamera(visibleCardData.pos);
			vector.x = Mathf.Round(vector.x);
			if (GeomUtil.scaleCentered(this.rectBook, 0.75f).Contains(vector))
			{
				rect2.center = new Vector2(vector.x, this.rectBook.yMax - rect.height);
				float num2 = 0.75f;
				if (visibleCardData.isCenter)
				{
					r = rect2;
					num = visibleCardData.count;
					num2 = 1f;
				}
				if (!flag)
				{
					Color color = GUI.color;
					Texture2D texture2D = ResourceManager.LoadTexture("DeckBuilder/expand/numbersdisplay");
					GUI.color = ColorUtil.GetWithAlpha(color, color.a * 0.75f);
					GUI.DrawTexture(GeomUtil.scaleCentered(rect2, num2), texture2D);
					GUI.color = color;
					this._drawDigit(visibleCardData.count, rect2.center, num2, 0f);
				}
			}
		}
		GUISkin guiskin = this.expandSkin;
		if (flag)
		{
			r.center = rect.center;
			guiskin = this.contractSkin;
		}
		GUI.SetNextControlName("dbExpand");
		if (GUI.Button(GeomUtil.scaleCentered(r, 1.2f), string.Empty, guiskin.button))
		{
			this.expandOrContractPile();
		}
		if (flag)
		{
			if (num > 0)
			{
				this._drawDigit(this.scrollBook.getNumPiles(), r.center, 1f, 0f);
			}
		}
		else
		{
			this._drawDigit(num, r.center, 1f, 0.5f);
		}
	}

	// Token: 0x06000CA6 RID: 3238 RVA: 0x0000A3AC File Offset: 0x000085AC
	public virtual void clearTable()
	{
		this.loadDeck(null, new Card[0], null);
	}

	// Token: 0x06000CA7 RID: 3239 RVA: 0x0000A3BC File Offset: 0x000085BC
	private void markTableChanged()
	{
		this.tableChanged = true;
	}

	// Token: 0x06000CA8 RID: 3240 RVA: 0x0005A5A0 File Offset: 0x000587A0
	private void updateTableStats()
	{
		if (!this.tableChanged)
		{
			return;
		}
		this.tableChanged = false;
		this.resourceCardsOnTable.Clear();
		this.resourceCosts.Clear();
		this.numCreatures = (this.numSpells = (this.numStructures = (this.numEnchantments = 0)));
		List<Card> list = new List<Card>();
		for (int i = 0; i < this.tableCards.Count; i++)
		{
			ICardView card = this.tableCards[i].card;
			Card cardInfo = card.getCardInfo();
			list.Add(cardInfo);
			CardType.Kind pieceKind = cardInfo.getPieceKind();
			if (pieceKind == CardType.Kind.SPELL)
			{
				this.numSpells++;
			}
			if (pieceKind == CardType.Kind.CREATURE)
			{
				this.numCreatures++;
			}
			if (pieceKind == CardType.Kind.STRUCTURE)
			{
				this.numStructures++;
			}
			if (pieceKind == CardType.Kind.ENCHANTMENT)
			{
				this.numEnchantments++;
			}
			ResourceType resource = cardInfo.getCardType().getResource();
			int num;
			this.resourceCardsOnTable.TryGetValue(resource, ref num);
			num = (this.resourceCardsOnTable[resource] = num + 1);
			int costTotal = cardInfo.getCostTotal();
			List<int> list2;
			if (!this.resourceCosts.TryGetValue(resource, ref list2))
			{
				list2 = new List<int>();
				this.resourceCosts[resource] = list2;
			}
			if (costTotal >= list2.Count)
			{
				CollectionUtil.extendList<int>(list2, costTotal + 1);
			}
			List<int> list4;
			List<int> list3 = list4 = list2;
			int num3;
			int num2 = num3 = costTotal;
			num3 = list4[num3];
			list3[num2] = num3 + 1;
		}
		this.deckStatsPane.UpdateGraphs(list);
		int num4 = 0;
		ResourceType resourceType = ResourceType.NONE;
		foreach (KeyValuePair<ResourceType, int> keyValuePair in this.resourceCardsOnTable)
		{
			if (keyValuePair.Value > num4)
			{
				num4 = keyValuePair.Value;
				resourceType = keyValuePair.Key;
			}
		}
		int num5 = 0;
		switch (resourceType)
		{
		case ResourceType.GROWTH:
			num5 = 1;
			break;
		case ResourceType.ENERGY:
			num5 = 2;
			break;
		case ResourceType.ORDER:
			num5 = 0;
			break;
		case ResourceType.DECAY:
			num5 = 3;
			break;
		}
		if (num5 != this.currentlyPlayingIndex)
		{
			this.musicStarted = true;
			base.StartCoroutine(this.PlayGivenMusicSoon(num5));
			this.currentlyPlayingIndex = num5;
		}
		if (!this.musicStarted)
		{
			this.musicStarted = true;
			base.StartCoroutine(this.PlayGivenMusicSoon(0));
		}
	}

	// Token: 0x06000CA9 RID: 3241 RVA: 0x0005A844 File Offset: 0x00058A44
	private IEnumerator PlayGivenMusicSoon(int index)
	{
		yield return new WaitForSeconds(1.6f);
		this.audioScript.FadeInSimultaneousTrack(index);
		yield break;
	}

	// Token: 0x06000CAA RID: 3242 RVA: 0x0005A870 File Offset: 0x00058A70
	private void updateCardRule()
	{
		if (!Input.GetMouseButtonDown(0))
		{
			return;
		}
		if (App.Popups.IsShowingPopup() || App.ChatUI.IsHovered())
		{
			return;
		}
		int num = 16;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit raycastHit;
		if (Physics.Raycast(ray, ref raycastHit, 2000f, num))
		{
			GameObject gameObject = raycastHit.collider.gameObject;
			if (gameObject != this.cardRule && gameObject.name == "Card")
			{
				this._showCardRule(((ICardView)gameObject.GetComponent(DeckBuilder2.CardViewClass)).getCardInfo());
				this.showScrollBookCardRule = false;
				return;
			}
		}
	}

	// Token: 0x06000CAB RID: 3243 RVA: 0x0000A3C5 File Offset: 0x000085C5
	protected void showCardRule(Card card)
	{
		this._showCardRule(card);
		this.showScrollBookCardRule = false;
	}

	// Token: 0x06000CAC RID: 3244 RVA: 0x0005A924 File Offset: 0x00058B24
	private void _showCardRule(Card card)
	{
		if (card.getId() == this.lastCardRuleId && card.level == this.lastCardRuleLevel)
		{
			return;
		}
		this.lastCardRuleId = card.getId();
		this.lastCardRuleLevel = card.level;
		if (this.cardRule != null)
		{
			Object.Destroy(this.cardRule);
		}
		this.currentCard = card;
		this.cardRule = PrimitiveFactory.createPlane(false);
		this.cardRule.name = "CardRule";
		CardView cardView = this.cardRule.AddComponent<CardView>();
		cardView.init(null, card, 200);
		cardView.applyHighResTexture();
		cardView.enableShowStats();
		cardView.enableShowHelp();
		this.onCardEnterView(card, cardView);
		this.gui3d.DrawObject(this.rectCard, this.cardRule);
		if (this.unit != null)
		{
			this.pendingUnitCard = card;
			return;
		}
		this.showCardUnit(card);
	}

	// Token: 0x06000CAD RID: 3245 RVA: 0x0005AA1C File Offset: 0x00058C1C
	protected virtual void showCardUnit(Card card)
	{
		if (card.getType() == this.lastCardUnitType)
		{
			return;
		}
		this.lastCardUnitType = card.getType();
		if (this.unit != null)
		{
			Object.Destroy(this.unit.gameObject);
			Object.Destroy(this.unitPlane);
			this.unit = null;
			this.unitPlane = null;
		}
		if (card.getPieceKind().isUnit())
		{
			this.unitPlane = new GameObject();
			this.unitPlane.AddComponent<MeshRenderer>();
			this.unitPlane.name = "UnitPlane";
			this.unitPlane.transform.parent = base.gameObject.transform.parent;
			this.unit = this.unitPlane.AddComponent<Unit>();
			this.unit.overriddenRenderQueue = 93000;
			this.unit.initForDeckbuilder(this.comm.GetCardDownloadURL(), card.getCardType());
			this.unit.transform.localPosition = GeomUtil.getTranslated(this.unitStand.transform.localPosition, 0f, (float)Screen.height * 0.023f, 0f);
			this.unit.setBaseScale(0.118f * (float)Screen.height);
			this.setUnitAlpha(0f);
		}
	}

	// Token: 0x06000CAE RID: 3246 RVA: 0x0000A3D5 File Offset: 0x000085D5
	protected Unit getUnit()
	{
		return this.unit;
	}

	// Token: 0x06000CAF RID: 3247 RVA: 0x0000A3DD File Offset: 0x000085DD
	protected void playAttackAnimation()
	{
		if (this.unit == null)
		{
			return;
		}
		this.unit.unitAttack();
	}

	// Token: 0x06000CB0 RID: 3248 RVA: 0x0005AB74 File Offset: 0x00058D74
	private void setUnitAlpha(float a)
	{
		a = Mth.clamp(a, 0f, 1f);
		if (this.unit != null)
		{
			this.unit.setAlpha(a);
		}
		if (this.unitStand != null)
		{
			this.unitStand.renderer.material.color = new Color(1f, 1f, 1f, a);
		}
	}

	// Token: 0x06000CB1 RID: 3249 RVA: 0x0005ABEC File Offset: 0x00058DEC
	protected void FixedUpdate()
	{
		if (!this.inited)
		{
			return;
		}
		if (this.pendingUnitCard != null)
		{
			float num = (!(this.unit != null)) ? 0f : this.unit.getAlpha();
			if (num > 0.01f)
			{
				this.setUnitAlpha(num - 0.2f);
			}
			else
			{
				this.showCardUnit(this.pendingUnitCard);
				this.pendingUnitCard = null;
			}
		}
		else if (this.unit != null)
		{
			float alpha = this.unit.getAlpha();
			if (alpha < 0.99f && this.unit.isPartiallyLoaded())
			{
				this.setUnitAlpha(alpha + 0.2f);
			}
		}
		if (this.showScrollBookCardRule)
		{
			Card centerCard = this.scrollBook.getCenterCard();
			if (centerCard != null && this.scrollbarTicks < 0)
			{
				this._showCardRule(centerCard);
			}
		}
		this.scrollbarTicks--;
		this.scrollbarAlpha.update(this.showScrollbar);
		this.filterStatsAlpha.update(this.hasFilter());
	}

	// Token: 0x06000CB2 RID: 3250 RVA: 0x0005AD14 File Offset: 0x00058F14
	private static Vector2 getMouseOffset(Vector3 relativeTo)
	{
		Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 0f));
		return new Vector2(vector.x - relativeTo.x, vector.y - relativeTo.y);
	}

	// Token: 0x06000CB3 RID: 3251 RVA: 0x0005AD74 File Offset: 0x00058F74
	protected void handleKeyboard()
	{
		bool flag = Input.GetKey(306) || Input.GetKey(305);
		if (flag && Input.GetKeyDown(13))
		{
			this.addFilteredCardsToTableAndReSort(1);
		}
		if (flag && this.isFilterAffectingTable() && Input.GetKeyDown(127))
		{
			this.removeFilteredCardsFromTable(1);
		}
		if (flag && Input.GetKeyDown(115))
		{
			this.onSaveDeckClicked();
		}
		if (GUIUtility.keyboardControl == 0)
		{
			if (Input.GetKeyDown(276))
			{
				this.scrollBook.scrollTo((float)((int)(this.scrollBook.scrollPos() - 1f)));
			}
			if (Input.GetKeyDown(275))
			{
				this.scrollBook.scrollTo((float)((int)(this.scrollBook.scrollPos() + 1f)));
			}
		}
	}

	// Token: 0x06000CB4 RID: 3252 RVA: 0x0005AE58 File Offset: 0x00059058
	protected void handleMouse()
	{
		if (App.Popups.IsShowingPopup())
		{
			return;
		}
		Vector2 screenMousePos = GUIUtil.getScreenMousePos();
		if (Input.GetMouseButtonDown(0))
		{
			this._hasHeld = false;
			this.startMouseDownPos = screenMousePos;
			this.startMouseDownTime = Time.time;
			this.startMouseDownCard = this.getTableCardUnderMouse();
			if (this.startMouseDownCard != null)
			{
				this.beginDragCards(new List<DeckCard>(new DeckCard[]
				{
					this.startMouseDownCard
				}), false);
			}
		}
		if (Input.GetMouseButtonUp(0) && this.draggedCards.Count > 0)
		{
			List<DeckCard> list = new List<DeckCard>(this.draggedCards);
			this.draggedCards.Clear();
			this.showScrollbar = true;
			this._tweening = false;
			bool flag = this.rectLeft.Contains(screenMousePos) && this.rectLeft.Contains(DeckBuilder2.worldToCamera(list[0].t.position));
			if (flag)
			{
				list.ForEach(new Action<DeckCard>(this.removeCardFromTable));
				this.filterActiveCollection(true);
			}
			else
			{
				this.putCardsOnTable(list);
			}
		}
		if (Input.GetMouseButton(0))
		{
			this.handleMouseButton(screenMousePos, Input.GetMouseButtonDown(0));
		}
	}

	// Token: 0x06000CB5 RID: 3253 RVA: 0x0000A3FC File Offset: 0x000085FC
	protected void Update()
	{
		if (!this.inited)
		{
			return;
		}
		this.checkScreenResolutionChanged(false);
		this.handleKeyboard();
		this.handleMouse();
		this.updateTableStats();
		this.updateCardRule();
	}

	// Token: 0x06000CB6 RID: 3254 RVA: 0x0005AF8C File Offset: 0x0005918C
	private DeckCard getTableCardUnderMouse()
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
		return this.tableCards.Find((DeckCard x) => x.t == hit.collider.transform);
	}

	// Token: 0x06000CB7 RID: 3255 RVA: 0x0005AFF4 File Offset: 0x000591F4
	private List<DeckCard> getTableCards(int cardType)
	{
		return Enumerable.ToList<DeckCard>(Enumerable.Where<DeckCard>(this.tableCards, (DeckCard x) => cardType == x.card.getCardInfo().getType()));
	}

	// Token: 0x06000CB8 RID: 3256 RVA: 0x0000A429 File Offset: 0x00008629
	private void handleMouseButton(Vector2 mouse, bool clicked)
	{
		this.handleMouseButtonHeld(mouse);
		this.updateDraggedCardsPos();
		if (clicked && this.rectUnit.Contains(mouse))
		{
			this.playAttackAnimation();
		}
	}

	// Token: 0x06000CB9 RID: 3257 RVA: 0x0005B02C File Offset: 0x0005922C
	private void handleMouseButtonHeld(Vector2 mouse)
	{
		if (this._hasHeld)
		{
			return;
		}
		if (Time.time - this.startMouseDownTime < 0.4f)
		{
			return;
		}
		this._hasHeld = true;
		if ((mouse - this.startMouseDownPos).magnitude >= 0.01f * (float)Screen.width)
		{
			return;
		}
		this.startTweenTablePile();
	}

	// Token: 0x06000CBA RID: 3258 RVA: 0x0005B090 File Offset: 0x00059290
	private void startTweenTablePile()
	{
		DeckCard card = this.getTableCardUnderMouse();
		if (card == null || card != this.startMouseDownCard)
		{
			return;
		}
		List<DeckCard> list = this.getTableCards(card.card.getCardInfo().getType());
		float num = Mathf.Max(Enumerable.ToArray<float>(Enumerable.Select<DeckCard, float>(list, (DeckCard c) => Mathf.Abs(card.t.position.x - c.t.position.x) + Mathf.Abs(card.t.position.y - c.t.position.y))));
		float num2 = Mathf.Max(Enumerable.ToArray<float>(Enumerable.Select<DeckCard, float>(list, (DeckCard c) => c.t.transform.position.y)));
		list[list.IndexOf(card)] = list[list.Count - 1];
		list[list.Count - 1] = card;
		bool flag = num2 == card.t.position.y;
		if (flag && num < ((float)list.Count - 0.5f) * this.CardSpacing)
		{
			this.beginDragCards(list, false);
			return;
		}
		this._tweening = true;
		this.beginDragCards(list, false);
		for (int i = 0; i < list.Count - 1; i++)
		{
			Vector3 positionForDraggedCard = this.getPositionForDraggedCard(i);
			positionForDraggedCard.z = card.t.position.z + 0.1f * (float)(3 - i);
			iTween.MoveTo(list[i].t.gameObject, iTween.Hash(new object[]
			{
				"position",
				positionForDraggedCard,
				"time",
				0.2f,
				"oncomplete",
				"onTweenComplete",
				"oncompletetarget",
				base.gameObject
			}));
		}
	}

	// Token: 0x06000CBB RID: 3259 RVA: 0x0000A455 File Offset: 0x00008655
	private void onTweenComplete()
	{
		this._tweening = false;
	}

	// Token: 0x06000CBC RID: 3260 RVA: 0x0000A45E File Offset: 0x0000865E
	protected static Vector2 worldToCamera(Vector2 w)
	{
		return GeomUtil.v3tov2(DeckBuilder2.worldToCamera(GeomUtil.v2tov3(w)));
	}

	// Token: 0x06000CBD RID: 3261 RVA: 0x0005B284 File Offset: 0x00059484
	protected static Vector3 worldToCamera(Vector3 w)
	{
		Vector3 mainCameraPosition = DeckBuilder2._mainCameraPosition;
		return new Vector3(w.x + (float)(Screen.width / 2) - mainCameraPosition.x, (float)(Screen.height / 2) - w.y - mainCameraPosition.y, w.z);
	}

	// Token: 0x06000CBE RID: 3262 RVA: 0x0000A470 File Offset: 0x00008670
	protected static Vector2 cameraToWorld(Vector2 w)
	{
		return GeomUtil.v3tov2(DeckBuilder2.cameraToWorld(GeomUtil.v2tov3(w)));
	}

	// Token: 0x06000CBF RID: 3263 RVA: 0x0005B2D4 File Offset: 0x000594D4
	protected static Vector3 cameraToWorld(Vector3 w)
	{
		Vector3 mainCameraPosition = DeckBuilder2._mainCameraPosition;
		return new Vector3(w.x - (float)(Screen.width / 2) + mainCameraPosition.x, -w.y + 0.5f * (float)Screen.height + mainCameraPosition.y, w.z);
	}

	// Token: 0x06000CC0 RID: 3264 RVA: 0x0005B328 File Offset: 0x00059528
	private void updateDraggedCardsPos()
	{
		if (this.draggedCards.Count == 0 || this._tweening)
		{
			return;
		}
		for (int i = 0; i < this.draggedCards.Count; i++)
		{
			this.draggedCards[i].t.position = this.getPositionForDraggedCard(i);
		}
		this.showScrollbar = !this.isDraggedCardsOverlapping(this.rectScroll);
	}

	// Token: 0x06000CC1 RID: 3265 RVA: 0x0000A482 File Offset: 0x00008682
	protected bool isDraggedCardsOverlapping(Rect rect)
	{
		return this.draggedCards.Count != 0 && DeckBuilder2.isCardOverlapping(this.draggedCards[0].card, rect);
	}

	// Token: 0x06000CC2 RID: 3266 RVA: 0x0000A4AD File Offset: 0x000086AD
	protected static bool isCardOverlapping(ICardView card, Rect rect)
	{
		return DeckBuilder2.isPlaneOverlapping(card.getTransform(), rect);
	}

	// Token: 0x06000CC3 RID: 3267 RVA: 0x0005B3A0 File Offset: 0x000595A0
	protected static bool isPlaneOverlapping(Transform planeTransform, Rect rect)
	{
		float num = planeTransform.localScale.x * 5f;
		float num2 = planeTransform.localScale.z * 5f;
		Rect r;
		r..ctor(planeTransform.position.x - num, planeTransform.position.y + num2, num + num, num2 + num2);
		Rect translated = GeomUtil.getTranslated(r, (float)(Screen.width / 2), (float)(Screen.height / 2));
		translated.y = (float)Screen.height - translated.y;
		return GeomUtil.overlapsOrTouches(translated, rect);
	}

	// Token: 0x06000CC4 RID: 3268 RVA: 0x0005B440 File Offset: 0x00059640
	private Vector3 getPositionForDraggedCard(int index)
	{
		Vector3 positionForStackedCard = this.getPositionForStackedCard(GUIUtil.getScreenMousePos(), index, this.draggedCards.Count, 2f);
		return new Vector3(positionForStackedCard.x - this.startMouseDownOffset.x, positionForStackedCard.y - this.startMouseDownOffset.y, positionForStackedCard.z);
	}

	// Token: 0x06000CC5 RID: 3269 RVA: 0x0000A4BB File Offset: 0x000086BB
	private Vector3 getPositionForStackedCard(Vector2 pos, int index, int numStacked)
	{
		return this.getPositionForStackedCard(pos, index, numStacked, this.currentTableCardZ);
	}

	// Token: 0x06000CC6 RID: 3270 RVA: 0x0005B49C File Offset: 0x0005969C
	private Vector3 getPositionForStackedCard(Vector2 pos, int index, int numStacked, float zBase)
	{
		Vector3 pos2 = this.gui3d.getPosition(pos.x, pos.y + (float)(numStacked - index - 1) * this.CardSpacing).pos;
		return new Vector3(pos2.x, pos2.y, zBase - 0.1f * (float)index);
	}

	// Token: 0x06000CC7 RID: 3271 RVA: 0x0000A4CC File Offset: 0x000086CC
	protected virtual bool allowAddCardToTable(Card card)
	{
		return this.mode == DeckBuilder2.BuilderMode.LIMITED || this.getNumCardsOnTable(card.getType()) < 3;
	}

	// Token: 0x06000CC8 RID: 3272 RVA: 0x0005B4F4 File Offset: 0x000596F4
	protected int getNumCardsOnTable(int ofType)
	{
		return Enumerable.Count<DeckCard>(this.tableCards, (DeckCard c) => c.card.getCardInfo().getType() == ofType);
	}

	// Token: 0x06000CC9 RID: 3273 RVA: 0x0000A4EB File Offset: 0x000086EB
	private void removeCardFromTable(DeckCard card)
	{
		if (this.tableCards.Remove(card))
		{
			this.addCardToScrollBook(card);
			this.markTableChanged();
		}
		Object.Destroy(card.t.gameObject);
	}

	// Token: 0x06000CCA RID: 3274 RVA: 0x0005B528 File Offset: 0x00059728
	protected bool removeCardFromTable(Card card)
	{
		for (int i = 0; i < this.tableCards.Count; i++)
		{
			DeckCard deckCard = this.tableCards[i];
			if (deckCard.card.getCardInfo().getId() == card.getId())
			{
				this.tableCards.RemoveAt(i);
				Object.Destroy(deckCard.t.gameObject);
				this.markTableChanged();
				return true;
			}
		}
		return false;
	}

	// Token: 0x06000CCB RID: 3275 RVA: 0x0005B5A0 File Offset: 0x000597A0
	protected virtual void putCardsOnTable(List<DeckCard> cards)
	{
		this.sw.start();
		foreach (DeckCard card in cards)
		{
			this.putCardOnTable(card);
		}
		if (this.draggedSourceIsScrollBook)
		{
			this.filterActiveCollection();
		}
		this.sw.stop();
		this.sw.printEvery(1);
	}

	// Token: 0x06000CCC RID: 3276 RVA: 0x0005B62C File Offset: 0x0005982C
	private bool putCardOnTable(DeckCard card)
	{
		if (this.tableCards.IndexOf(card) >= 0)
		{
			this._dropOnTable(card);
			return true;
		}
		if (Enumerable.Any<DeckCard>(this.tableCards, (DeckCard c) => c.card.getCardInfo().getId() == card.card.getCardInfo().getId()))
		{
			Object.Destroy(card.card.getTransform().gameObject);
			return false;
		}
		if (!this.allowAddCardToTable(card.card.getCardInfo()))
		{
			this.addCardToScrollBook(card);
			Object.Destroy(card.card.getTransform().gameObject);
			return false;
		}
		this._dropOnTable(card);
		this.tableCards.Add(card);
		this.markTableChanged();
		return true;
	}

	// Token: 0x06000CCD RID: 3277 RVA: 0x0005B70C File Offset: 0x0005990C
	private static void tweenDropOnTable(Transform transform, float z)
	{
		Vector3 position = transform.position;
		position.z = z;
		iTween.MoveTo(transform.gameObject, iTween.Hash(new object[]
		{
			"position",
			position,
			"time",
			0.3f,
			"easetype",
			iTween.EaseType.easeOutExpo
		}));
	}

	// Token: 0x06000CCE RID: 3278 RVA: 0x0005B778 File Offset: 0x00059978
	private void _dropOnTable(DeckCard card)
	{
		DeckBuilder2.tweenDropOnTable(card.card.getTransform(), this.getNextZ());
		Hashtable dropOnTableMoveToTween = this.getDropOnTableMoveToTween(card.card);
		if (dropOnTableMoveToTween != null)
		{
			iTween.Stop(card.t.gameObject);
			iTween.MoveTo(card.t.gameObject, dropOnTableMoveToTween);
		}
		this.audioScript.PlaySFX("Sounds/hyperduck/DeckBuilder/db_scroll_pickup");
		if (this.scrollBook.isExpanded() && this.scrollBook.getNumPiles() == 0)
		{
			this.expandOrContractPile();
		}
	}

	// Token: 0x06000CCF RID: 3279 RVA: 0x0005B808 File Offset: 0x00059A08
	protected virtual Hashtable getDropOnTableMoveToTween(ICardView card)
	{
		Vector3 w = GeomUtil.clamp(this.rectTable, DeckBuilder2.worldToCamera(card.getTransform().position));
		Vector3 vector = DeckBuilder2.cameraToWorld(w);
		if ((card.getTransform().position - vector).magnitude > 1f)
		{
			float num = 6.2831855f * RandomUtil.random();
			vector.x += 0.8f * Mathf.Cos(num) * this.CardSpacing;
			vector.y += 1.2f * Mathf.Sin(num) * this.CardSpacing;
			vector.z = this.currentTableCardZ;
			return iTween.Hash(new object[]
			{
				"position",
				vector,
				"time",
				0.1f,
				"easetype",
				iTween.EaseType.easeOutCubic
			});
		}
		return null;
	}

	// Token: 0x06000CD0 RID: 3280 RVA: 0x0005B8FC File Offset: 0x00059AFC
	private bool hasFilter()
	{
		string text = this.searchString.Trim();
		return text.Length > 0 && this.searchString != "Search   ";
	}

	// Token: 0x06000CD1 RID: 3281 RVA: 0x000028DF File Offset: 0x00000ADF
	protected virtual void addCardToScrollBook(DeckCard card)
	{
	}

	// Token: 0x06000CD2 RID: 3282 RVA: 0x0005B934 File Offset: 0x00059B34
	private void alignTableCards(DeckBuilder2.TableAlignment alignment, DeckSorter sorter)
	{
		if (this.tableCards.Count == 0)
		{
			return;
		}
		this.currentTableCardZ = 850f;
		List<DeckCard> list = new List<DeckCard>(this.tableCards);
		list.Sort(sorter);
		List<Card> cards = Enumerable.ToList<Card>(Enumerable.Select<DeckCard, Card>(list, (DeckCard d) => d.card.getCardInfo()));
		int numTypes = cards.getNumTypes();
		int num = (numTypes <= 5) ? 1 : 2;
		bool flag = numTypes % num != 0;
		int num2 = numTypes / num;
		int num3 = (!flag) ? num2 : (num2 + 1);
		int num4 = -1;
		int num5 = 0;
		for (int i = 0; i < this.tableCards.Count; i++)
		{
			int typeIndexForCardIndex = cards.getTypeIndexForCardIndex(i);
			if (typeIndexForCardIndex != num4)
			{
				num4 = typeIndexForCardIndex;
				num5 = cards.getNumOf(typeIndexForCardIndex);
			}
			int typeSubIndexForCardIndex = cards.getTypeSubIndexForCardIndex(i);
			int num6 = (typeIndexForCardIndex < num3) ? 0 : 1;
			int num7 = (num6 != 0) ? num2 : num3;
			int num8 = (num6 != 0) ? (typeIndexForCardIndex - num3) : typeIndexForCardIndex;
			ICardView card = list[i].card;
			float num9 = (num7 <= 4) ? ((1f + (float)num8) / (float)(num7 + 1)) : ((float)num8 / ((float)num7 - 1f));
			float tableXPos = this.getTableXPos(num9);
			float tableYPos = this.getTableYPos(0.1f + 0.85f * (float)num6);
			Vector3 positionForStackedCard = this.getPositionForStackedCard(new Vector2(tableXPos, tableYPos), typeSubIndexForCardIndex, num5 - 3, this.getNextZ());
			iTween.Stop(card.getTransform().gameObject);
			iTween.MoveTo(card.getTransform().gameObject, iTween.Hash(new object[]
			{
				"position",
				positionForStackedCard,
				"time",
				0.2f,
				"delay",
				0.01f * (float)i,
				"easetype",
				iTween.EaseType.easeOutCubic
			}));
			iTween.RotateTo(card.getTransform().gameObject, Gui3D.getFaceCameraRotation(), 0.2f);
			if (positionForStackedCard.z < this.currentTableCardZ)
			{
				this.currentTableCardZ = positionForStackedCard.z;
			}
		}
	}

	// Token: 0x06000CD3 RID: 3283 RVA: 0x0005BB90 File Offset: 0x00059D90
	protected virtual void beginDragCards(List<DeckCard> cards, bool sourceIsScrollBook)
	{
		this.draggedCards.Clear();
		if (cards.Count == 0)
		{
			return;
		}
		this.draggedCards.AddRange(cards);
		this.draggedSourceIsScrollBook = sourceIsScrollBook;
		this.startMouseDownOffset = DeckBuilder2.getMouseOffset(this.draggedCards[cards.Count - 1].t.position);
		foreach (DeckCard deckCard in this.draggedCards)
		{
			if (!this._tweening)
			{
				this.gui3d.orientPlane(deckCard.t.gameObject);
				iTween.Stop(deckCard.card.getTransform().gameObject);
			}
			else
			{
				Vector3 position = deckCard.t.position;
				this.gui3d.orientPlane(deckCard.t.gameObject);
				deckCard.t.position = position;
			}
		}
		this.updateDraggedCardsPos();
		this.audioScript.PlaySFX("Sounds/hyperduck/DeckBuilder/db_scroll_select");
	}

	// Token: 0x06000CD4 RID: 3284 RVA: 0x0000A51B File Offset: 0x0000871B
	public void expandOrContractPile()
	{
		if (!this.scrollBook.isExpanded())
		{
			this.scrollBook.expand();
		}
		else
		{
			this.scrollBook.contract();
		}
	}

	// Token: 0x06000CD5 RID: 3285 RVA: 0x0000A548 File Offset: 0x00008748
	private void showDeckSelector()
	{
		App.Popups.ShowDeckSelector(this, this, this.decks, true, true, "_import");
	}

	// Token: 0x06000CD6 RID: 3286 RVA: 0x0005BCB4 File Offset: 0x00059EB4
	public void PopupCancel(string popupType)
	{
		if ("confirmdelete" == popupType || "confirmdeleteAI" == popupType)
		{
			this.showDeckSelector();
		}
		if (popupType == "deck-export")
		{
			App.Popups.ShowSaveDeck(this, this.loadedDeckName, this.saveDeckInformation);
		}
	}

	// Token: 0x06000CD7 RID: 3287 RVA: 0x0005BD10 File Offset: 0x00059F10
	public void PopupExport(string popupType, string choice)
	{
		this.loadedDeckName = choice;
		string initialEntryText = DeckPortation.fromList(choice, App.MyProfile.ProfileInfo.name, this.getTableCards()).toJson();
		App.Popups.ShowTextArea(this, "deck-export", "Export deck", string.Empty, "Copy to Clipboard", "Back", initialEntryText, false, false);
		App.Popups.ShowOverlay();
	}

	// Token: 0x06000CD8 RID: 3288 RVA: 0x0000A563 File Offset: 0x00008763
	public void PopupSaveAIDeck(string popupType, string choice)
	{
		this.saveDeck(choice, true);
	}

	// Token: 0x06000CD9 RID: 3289 RVA: 0x0005BD78 File Offset: 0x00059F78
	public void PopupOk(string popupType, string choice)
	{
		if (popupType == "savedeck")
		{
			this.saveDeck(choice, false);
			this.loadedDeckName = choice;
			if (this.mode == DeckBuilder2.BuilderMode.LIMITED)
			{
				App.Popups.ShowOverlay();
			}
		}
		if (popupType == "deck-export")
		{
			SystemUtil.copyToClipboard(choice);
		}
		if (popupType == "deck-import")
		{
			DeckPortation.Deck deck = (choice.TrimStart(new char[0]).IndexOf("{") == 0) ? DeckPortation.fromJson(choice) : DeckPortation.fromScrollsGuide("sg", App.MyProfile.ProfileInfo.name, choice);
			if (deck == null)
			{
				return;
			}
			this.importDeck(deck);
		}
	}

	// Token: 0x06000CDA RID: 3290 RVA: 0x0005BE30 File Offset: 0x0005A030
	private void importDeck(DeckPortation.Deck d)
	{
		List<int> list = new List<int>();
		List<Card> cards = d.getCards(this.allCards, list);
		StringBuilder stringBuilder = new StringBuilder();
		if (list.Count > 0)
		{
			Dictionary<int, int> dictionary = CollectionUtil.countInstances<int>(list);
			List<CardType> list2 = Enumerable.ToList<CardType>(Enumerable.Where<CardType>(Enumerable.Select<int, CardType>(dictionary.Keys, (int t) => CardTypeManager.getInstance().get(t)), (CardType ct) => ct != null && ct.isValid()));
			list2.Sort((CardType a, CardType b) => a.name.CompareTo(b.name));
			int num = 0;
			foreach (CardType cardType in list2)
			{
				int num2 = dictionary[cardType.id];
				num += num2;
				stringBuilder.AppendLine(num2 + " x " + cardType.name);
			}
			if (list.Count > num)
			{
				stringBuilder.Insert(0, list.Count - num + " x invalid type id");
			}
			App.Popups.ShowTextArea(this, "deck-import-missing", "Missing Scrolls", string.Empty, "Ok", null, stringBuilder.ToString(), false, true);
		}
		this.clearTable();
		this.putCardsOnTable(this.createDeckCards(cards, Vector3.zero));
		this.alignTableCards(DeckBuilder2.TableAlignment.Stacked, this.getDefaultSorter());
	}

	// Token: 0x06000CDB RID: 3291 RVA: 0x0005BFD8 File Offset: 0x0005A1D8
	public void PopupOk(string popupType)
	{
		if (popupType == "deckselector")
		{
			App.Popups.ShowTextArea(this, "deck-import", "Import deck", "Paste the encoded deck string.", "Ok", "Cancel", string.Empty, true, true);
		}
		if (popupType == "confirmdelete")
		{
			this.deleteDeck(this.deckDeleteName, false);
			this.showDeckSelector();
		}
		if (popupType == "confirmdeleteAI")
		{
			this.deleteDeck(this.deckDeleteName, true);
			this.showDeckSelector();
		}
		if (popupType == "ok-decksavelimited")
		{
			SceneLoader.loadScene("_Lobby");
		}
	}

	// Token: 0x06000CDC RID: 3292 RVA: 0x0000A56D File Offset: 0x0000876D
	public void PopupDeckChosen(DeckInfo deck)
	{
		if (deck is LabEntryInfo)
		{
			this.comm.send(new LoadLabDeckMessage());
		}
		else
		{
			this.comm.send(new DeckCardsMessage(deck.name));
		}
	}

	// Token: 0x06000CDD RID: 3293 RVA: 0x0005C080 File Offset: 0x0005A280
	public void PopupDeckDeleted(DeckInfo deck)
	{
		this.deckDeleteName = deck.name;
		if (deck is LabEntryInfo)
		{
			App.Popups.ShowOkCancel(this, "confirmdeleteAI", "Withdraw AI deck", "Are you sure you want to withdraw your AI deck from competing?", "Withdraw", "Cancel");
		}
		else
		{
			App.Popups.ShowOkCancel(this, "confirmdelete", "Delete deck", "Really delete selected deck?", "Delete", "Cancel");
		}
	}

	// Token: 0x06000CDE RID: 3294 RVA: 0x0000A5A7 File Offset: 0x000087A7
	protected virtual int allowStartDraggingCardCount(Card card)
	{
		if (this.mode == DeckBuilder2.BuilderMode.LIMITED)
		{
			return int.MaxValue;
		}
		return 3 - this.getNumCardsOnTable(card.getType());
	}

	// Token: 0x06000CDF RID: 3295 RVA: 0x0005C0F4 File Offset: 0x0005A2F4
	private void popScrollbookCards(Card card, int cardIndex, int maxCount, Vector3 p)
	{
		int num = this.allowStartDraggingCardCount(card);
		if (num <= 0)
		{
			return;
		}
		maxCount = Math.Min(maxCount, num);
		List<Card> list = this.scrollBook.popCards(cardIndex, maxCount);
		list.Reverse();
		this.beginDragCards(this.createDeckCards(list, p), true);
	}

	// Token: 0x06000CE0 RID: 3296 RVA: 0x000028DF File Offset: 0x00000ADF
	public virtual void onCardEnterView(Card card, ICardView view)
	{
	}

	// Token: 0x06000CE1 RID: 3297 RVA: 0x0000A5C9 File Offset: 0x000087C9
	public void onStartDragCard(Card card, int cardIndex, GameObject g)
	{
		this.popScrollbookCards(card, cardIndex, 1, g.transform.position);
	}

	// Token: 0x06000CE2 RID: 3298 RVA: 0x0000A5DF File Offset: 0x000087DF
	public void onCardClicked(Card card, int cardIndex, GameObject g)
	{
		this.showScrollBookCardRule = true;
	}

	// Token: 0x06000CE3 RID: 3299 RVA: 0x0000A5E8 File Offset: 0x000087E8
	public void onCardHeld(Card card, int cardIndex, GameObject g)
	{
		this.popScrollbookCards(card, cardIndex, this.allowStartDraggingCardCount(card), g.transform.position);
	}

	// Token: 0x06000CE4 RID: 3300 RVA: 0x0000A604 File Offset: 0x00008804
	public void onCardDoubleClicked(Card card, int cardIndex, GameObject g)
	{
		this.expandOrContractPile();
	}

	// Token: 0x04000995 RID: 2453
	protected const int MaxCardsOfSameType = 3;

	// Token: 0x04000996 RID: 2454
	private const string ControlName_Search = "dbSearchfield";

	// Token: 0x04000997 RID: 2455
	private const string ControlName_Scrollbar = "dbScrollbar";

	// Token: 0x04000998 RID: 2456
	private const string ControlName_ExpandButton = "dbExpand";

	// Token: 0x04000999 RID: 2457
	protected const float ZTableCardBase = 850f;

	// Token: 0x0400099A RID: 2458
	protected const float ZScrollBook = 900f;

	// Token: 0x0400099B RID: 2459
	protected const float ZBackground = 950f;

	// Token: 0x0400099C RID: 2460
	private const string emptySearchFieldString = "Search   ";

	// Token: 0x0400099D RID: 2461
	private const int CMASK_CARD = 4;

	// Token: 0x0400099E RID: 2462
	private const int CMASK_OTHER = 5;

	// Token: 0x0400099F RID: 2463
	private const string IdConfirmDelete = "confirmdelete";

	// Token: 0x040009A0 RID: 2464
	private const string IdConfirmDeleteAI = "confirmdeleteAI";

	// Token: 0x040009A1 RID: 2465
	private GameObject deckViewParent;

	// Token: 0x040009A2 RID: 2466
	private static readonly Type CardViewClass = typeof(CardView);

	// Token: 0x040009A3 RID: 2467
	private GUISkin deckBuilderGuiSkin;

	// Token: 0x040009A4 RID: 2468
	private GUISkin buttonSkin;

	// Token: 0x040009A5 RID: 2469
	private GUISkin pulldownSkin;

	// Token: 0x040009A6 RID: 2470
	private GUISkin plaqueItemSkin;

	// Token: 0x040009A7 RID: 2471
	private GUISkin textFieldSkin;

	// Token: 0x040009A8 RID: 2472
	private GUISkin expandSkin;

	// Token: 0x040009A9 RID: 2473
	private GUISkin contractSkin;

	// Token: 0x040009AA RID: 2474
	private GUISkin pileSizeSkin;

	// Token: 0x040009AB RID: 2475
	private readonly List<Card> allCards = new List<Card>();

	// Token: 0x040009AC RID: 2476
	private readonly Dictionary<long, Card> allCardsDict = new Dictionary<long, Card>();

	// Token: 0x040009AD RID: 2477
	private List<Card> activeCards = new List<Card>();

	// Token: 0x040009AE RID: 2478
	private DeckSorter collectionSorter = new DeckSorter();

	// Token: 0x040009AF RID: 2479
	protected readonly List<DeckCard> tableCards = new List<DeckCard>();

	// Token: 0x040009B0 RID: 2480
	private int tableCardsNeededForDeck = 50;

	// Token: 0x040009B1 RID: 2481
	private List<DeckInfo> decks;

	// Token: 0x040009B2 RID: 2482
	private CardOverlay cardOverlay;

	// Token: 0x040009B3 RID: 2483
	[SerializeField]
	private RenderTexture cardRenderTexture;

	// Token: 0x040009B4 RID: 2484
	private float CardSpacing;

	// Token: 0x040009B5 RID: 2485
	protected float currentTableCardZ = 850f;

	// Token: 0x040009B6 RID: 2486
	private float buttonIndexOffset;

	// Token: 0x040009B7 RID: 2487
	private MockupCalc mock2048 = new MockupCalc(2048, 1536);

	// Token: 0x040009B8 RID: 2488
	private int numCreatures;

	// Token: 0x040009B9 RID: 2489
	private int numStructures;

	// Token: 0x040009BA RID: 2490
	private int numSpells;

	// Token: 0x040009BB RID: 2491
	private int numEnchantments;

	// Token: 0x040009BC RID: 2492
	private string loadedDeckName = string.Empty;

	// Token: 0x040009BD RID: 2493
	private string searchString = string.Empty;

	// Token: 0x040009BE RID: 2494
	protected ScrollBook scrollBook;

	// Token: 0x040009BF RID: 2495
	private int scrollbarTicks;

	// Token: 0x040009C0 RID: 2496
	private FloatFade scrollbarAlpha = new UnitFade(1f, 0.2f, 0.2f);

	// Token: 0x040009C1 RID: 2497
	private FloatFade filterStatsAlpha = new UnitFade(0f, 0.1f, 0.1f);

	// Token: 0x040009C2 RID: 2498
	private bool showScrollbar = true;

	// Token: 0x040009C3 RID: 2499
	private bool showScrollBookCardRule = true;

	// Token: 0x040009C4 RID: 2500
	private DeckStatsPane deckStatsPane;

	// Token: 0x040009C5 RID: 2501
	private Communicator comm;

	// Token: 0x040009C6 RID: 2502
	private ButtonGroup sortGroup;

	// Token: 0x040009C7 RID: 2503
	private ResourceFilterGroup resourceFilterGroup;

	// Token: 0x040009C8 RID: 2504
	[SerializeField]
	private Camera scrollBookCamera;

	// Token: 0x040009C9 RID: 2505
	private static Vector3 _mainCameraPosition;

	// Token: 0x040009CA RID: 2506
	private AudioScript audioScript;

	// Token: 0x040009CB RID: 2507
	private Material sharedShadowMaterial;

	// Token: 0x040009CC RID: 2508
	protected Gui3D gui3d;

	// Token: 0x040009CD RID: 2509
	protected bool loadSaveTableState = true;

	// Token: 0x040009CE RID: 2510
	private DeckBuilder2.BuilderMode mode;

	// Token: 0x040009CF RID: 2511
	private static string[] musicFiles = new string[]
	{
		"Music/Deckbuilder_Neutral",
		"Music/Deckbuilder_Growth",
		"Music/Deckbuilder_Energy",
		"Music/Deckbuilder_Decay"
	};

	// Token: 0x040009D0 RID: 2512
	private int currentlyPlayingIndex;

	// Token: 0x040009D1 RID: 2513
	private bool musicStarted;

	// Token: 0x040009D2 RID: 2514
	private GUISkin skinScrollbar;

	// Token: 0x040009D3 RID: 2515
	private bool inited;

	// Token: 0x040009D4 RID: 2516
	private int lastScreenWidth;

	// Token: 0x040009D5 RID: 2517
	private int lastScreenHeight;

	// Token: 0x040009D6 RID: 2518
	private GameObject imBookPlane;

	// Token: 0x040009D7 RID: 2519
	private GameObject imLeftBookPlane;

	// Token: 0x040009D8 RID: 2520
	private GameObject imRightBookPlane;

	// Token: 0x040009D9 RID: 2521
	protected Rect rectLeft;

	// Token: 0x040009DA RID: 2522
	protected Rect rectBook;

	// Token: 0x040009DB RID: 2523
	protected Rect rectScroll;

	// Token: 0x040009DC RID: 2524
	protected Rect rectTable;

	// Token: 0x040009DD RID: 2525
	protected Rect rectRight;

	// Token: 0x040009DE RID: 2526
	protected Rect rectCard;

	// Token: 0x040009DF RID: 2527
	protected Rect rectUnit;

	// Token: 0x040009E0 RID: 2528
	protected Rect subMenuRect;

	// Token: 0x040009E1 RID: 2529
	private int activeTableCardsCount;

	// Token: 0x040009E2 RID: 2530
	private int activeUniqueTypes;

	// Token: 0x040009E3 RID: 2531
	private string saveDeckInformation;

	// Token: 0x040009E4 RID: 2532
	private bool _showSearchDropdown;

	// Token: 0x040009E5 RID: 2533
	private Rect _searchDropdownBoundingRect;

	// Token: 0x040009E6 RID: 2534
	private int _setFocusState = 2;

	// Token: 0x040009E7 RID: 2535
	private Dictionary<ResourceType, int> resourceCardsOnTable = new Dictionary<ResourceType, int>();

	// Token: 0x040009E8 RID: 2536
	private Dictionary<ResourceType, List<int>> resourceCosts = new Dictionary<ResourceType, List<int>>();

	// Token: 0x040009E9 RID: 2537
	private bool tableChanged = true;

	// Token: 0x040009EA RID: 2538
	private GameObject cardRule;

	// Token: 0x040009EB RID: 2539
	protected Card currentCard;

	// Token: 0x040009EC RID: 2540
	private long lastCardRuleId = -1L;

	// Token: 0x040009ED RID: 2541
	private int lastCardUnitType = -1;

	// Token: 0x040009EE RID: 2542
	private int lastCardRuleLevel = -1;

	// Token: 0x040009EF RID: 2543
	private GameObject unitPlane;

	// Token: 0x040009F0 RID: 2544
	private GameObject unitStand;

	// Token: 0x040009F1 RID: 2545
	private Unit unit;

	// Token: 0x040009F2 RID: 2546
	private Card pendingUnitCard;

	// Token: 0x040009F3 RID: 2547
	private float startMouseDownTime;

	// Token: 0x040009F4 RID: 2548
	private DeckCard startMouseDownCard;

	// Token: 0x040009F5 RID: 2549
	private Vector2 startMouseDownPos = default(Vector2);

	// Token: 0x040009F6 RID: 2550
	private Vector2 startMouseDownOffset = default(Vector2);

	// Token: 0x040009F7 RID: 2551
	private bool _hasHeld;

	// Token: 0x040009F8 RID: 2552
	private bool _tweening;

	// Token: 0x040009F9 RID: 2553
	private StopWatch sw = new StopWatch();

	// Token: 0x040009FA RID: 2554
	private List<DeckCard> draggedCards = new List<DeckCard>();

	// Token: 0x040009FB RID: 2555
	private bool draggedSourceIsScrollBook;

	// Token: 0x040009FC RID: 2556
	private string deckDeleteName;

	// Token: 0x02000192 RID: 402
	private enum BuilderMode
	{
		// Token: 0x04000A09 RID: 2569
		NORMAL,
		// Token: 0x04000A0A RID: 2570
		LIMITED,
		// Token: 0x04000A0B RID: 2571
		VIEW_LIMITED
	}

	// Token: 0x02000193 RID: 403
	public enum SortMode
	{
		// Token: 0x04000A0D RID: 2573
		Name,
		// Token: 0x04000A0E RID: 2574
		Cost
	}

	// Token: 0x02000194 RID: 404
	private enum TableAlignment
	{
		// Token: 0x04000A10 RID: 2576
		Stacked
	}
}
