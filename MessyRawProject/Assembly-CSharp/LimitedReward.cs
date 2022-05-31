using System;
using System.Collections;
using System.Collections.Generic;
using Gui;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x02000201 RID: 513
public class LimitedReward : AbstractCommListener, ScrollBook.IListener, IOkCallback, iEffect
{
	// Token: 0x0600101A RID: 4122 RVA: 0x0006BA98 File Offset: 0x00069C98
	private void Awake()
	{
		this.cardOverlay = new GameObject("Card Overlay").AddComponent<CardOverlay>();
		this.cardOverlay.Init(this.cardRenderTexture, 5);
		this.cardOverlay.SetHideOverlayOnClick(false);
		this.rewardButtons = new GameObject("Reward Buttons").AddComponent<LimitedRewardButtons>();
		this.rewardButtons.Init(this);
		this.rewardButtons.enabled = false;
	}

	// Token: 0x0600101B RID: 4123 RVA: 0x0000CA2D File Offset: 0x0000AC2D
	public override void OnDestroy()
	{
		base.OnDestroy();
		Object.Destroy(this.scrollBook);
	}

	// Token: 0x0600101C RID: 4124 RVA: 0x0006BB08 File Offset: 0x00069D08
	private void Start()
	{
		this.regularUI = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		this.skinScrollbar = (GUISkin)ResourceManager.Load("_GUISkins/HorizontalSlider");
		App.Communicator.addListener(this);
		this.gui3d = new Gui3D(Camera.main);
		this._mainCameraPosition = Camera.main.transform.position;
		base.name = "_Limited";
		this.SetupScene();
		this.scrollBook = base.gameObject.AddComponent<ScrollBook>();
		this.scrollBook.init(this.rectBook, 880f);
		this.scrollBook.setBoundingRect(this.rectBook);
		this.scrollBook.setListener(this);
		App.ChatUI.Show(false);
		base.StartCoroutine(this.FadeInAfterWait(0.2f));
		App.AudioScript.PlayMusic("Music/Store");
		RewardLimitedMessage waitingReward = App.WaitingReward;
		Reward reward = waitingReward.reward;
		this.commonsToPick = reward.common;
		this.uncommonsToPick = reward.uncommon;
		this.raresToPick = reward.rare;
		this.scrollsToPick = reward.GetTotalScrolls();
		this.goldEarned = reward.gold;
		this.gamesWon = waitingReward.gamesWon;
		this.gamesLeft = waitingReward.gamesLeft;
		this.gamesTotal = waitingReward.gamesTotal;
		this.cardsToPickFrom = new List<Card>(waitingReward.cards);
		this.deckName = waitingReward.deck;
		this.scrollBook.setCards(this.cardsToPickFrom);
		this.ShowNextReward();
	}

	// Token: 0x0600101D RID: 4125 RVA: 0x0006BC94 File Offset: 0x00069E94
	private IEnumerator FadeInAfterWait(float seconds)
	{
		yield return new WaitForSeconds(seconds);
		App.LobbyMenu.fadeInScene();
		yield break;
	}

	// Token: 0x0600101E RID: 4126 RVA: 0x0006BCB8 File Offset: 0x00069EB8
	private void ShowNextReward()
	{
		Log.info(string.Concat(new object[]
		{
			"Remaining rewards, common: ",
			this.commonsToPick,
			", uncommon: ",
			this.uncommonsToPick,
			", rare: ",
			this.raresToPick
		}));
		this.pickingRarityLevel = -1;
		if (this.commonsToPick > 0)
		{
			this.commonsToPick--;
			this.pickingRarityLevel = 0;
		}
		else if (this.uncommonsToPick > 0)
		{
			this.uncommonsToPick--;
			this.pickingRarityLevel = 1;
		}
		else if (this.raresToPick > 0)
		{
			this.raresToPick--;
			this.pickingRarityLevel = 2;
		}
		List<Card> rarityFilteredScrolls = this.getRarityFilteredScrolls(this.pickingRarityLevel);
		if (rarityFilteredScrolls.Count > 0)
		{
			this.scrollBook.setCards(rarityFilteredScrolls);
			this.scrollBook.scrollTo((float)(rarityFilteredScrolls.Count / 2));
		}
		else if (this.pickingRarityLevel == -1)
		{
			this.ShowFinalRewards();
		}
		else
		{
			this.pickingRarityLevel = 2;
			rarityFilteredScrolls = this.getRarityFilteredScrolls(this.pickingRarityLevel);
			this.scrollBook.setCards(rarityFilteredScrolls);
			this.scrollBook.scrollTo((float)(rarityFilteredScrolls.Count / 2));
		}
	}

	// Token: 0x0600101F RID: 4127 RVA: 0x0006BE14 File Offset: 0x0006A014
	private List<Card> getRarityFilteredScrolls(int maxRarity)
	{
		List<Card> list = new List<Card>();
		foreach (Card card in this.cardsToPickFrom)
		{
			if (card.getCardType().rarity <= maxRarity)
			{
				list.Add(card);
			}
		}
		return list;
	}

	// Token: 0x06001020 RID: 4128 RVA: 0x0006BE88 File Offset: 0x0006A088
	private void SetupScene()
	{
		Camera.main.transparencySortMode = 2;
		Camera.main.nearClipPlane = 0.3f;
		Camera.main.farClipPlane = 1000f;
		this.lightSource = new GameObject("Light");
		this.lightSource.AddComponent<Light>();
		this.lightSource.light.color = new Color(0.55f, 0.8f, 1f);
		this.lightSource.transform.position = new Vector3(0f, 0f, 0f);
		this.lightSource.light.intensity = 0.8f;
		this.lightSource.light.type = 1;
		this.lightSource.light.range = 25f;
		this.lightSource.light.shadows = 2;
		this.imBookPlane = PrimitiveFactory.createTexturedPlane("DeckBuilder/spellbook__base", true);
		this.imBookPlane.renderer.material.shader = ResourceManager.LoadShader("Unlit/Transparent");
		this.imLeftBookPlane = PrimitiveFactory.createTexturedPlane("DeckBuilder/spellbook__leftside", true);
		this.imLeftBookPlane.renderer.material.shader = ResourceManager.LoadShader("Unlit/Transparent");
		this.imLeftBookPlane.layer = 12;
		this.imRightBookPlane = PrimitiveFactory.createTexturedPlane("DeckBuilder/spellbook__rightside", true);
		this.imRightBookPlane.renderer.material.shader = ResourceManager.LoadShader("Unlit/Transparent");
		this.imRightBookPlane.layer = 12;
		this.imCoinsPlane = PrimitiveFactory.createTexturedPlane("Limited/goldpile", true);
		this.imCoinsPlane.renderer.material.shader = ResourceManager.LoadShader("Scrolls/Unlit/Transparent");
		this.imCoinsPlane.renderer.enabled = false;
		this.imShadowPlane1 = PrimitiveFactory.createTexturedPlane("Limited/shadow", true);
		this.imShadowPlane1.renderer.material.shader = ResourceManager.LoadShader("Scrolls/Unlit/Transparent");
		this.imShadowPlane1.renderer.enabled = false;
		this.imShadowPlane2 = PrimitiveFactory.createTexturedPlane("Limited/shadow", true);
		this.imShadowPlane2.renderer.material.shader = ResourceManager.LoadShader("Scrolls/Unlit/Transparent");
		this.imShadowPlane2.renderer.enabled = false;
		this.lastScreenWidth = -9999;
		this.CheckScreenResolutionChanged(true);
		this.inited = true;
		Debug.Log("Init done!");
	}

	// Token: 0x06001021 RID: 4129 RVA: 0x0006C100 File Offset: 0x0006A300
	private void SetupPositions()
	{
		Camera main = Camera.main;
		float orthographicSize = (float)Screen.height * 0.5f;
		this.scrollBookCamera.orthographicSize = orthographicSize;
		main.orthographicSize = orthographicSize;
		this.rectSubMenu = App.LobbyMenu.getSubMenuRect(1f);
		float num = this.rectSubMenu.y + this.rectSubMenu.height;
		float num2 = (float)Screen.height * 0.98f - num;
		Vector3 vector = CardView.CardLocalScale();
		float num3 = vector.x / vector.z;
		float num4 = (float)Screen.height * 0.04f;
		float num5 = (float)Screen.height * 0.5f;
		float num6 = num3 * num5;
		this.rectCard = new Rect(0f, 0f, num6, num5);
		this.rectCard.x = (float)(Screen.width / 2) - num6 / 2f;
		this.rectCard.y = num + ((float)Screen.height * 0.9f - num - this.rectCard.height) * 0.59f;
		float num7 = (float)Screen.height * 0.017f;
		this.rectBook = new Rect((float)Screen.height * 0.15f, num + num2 * 0.5f, (float)Screen.width - (float)Screen.height * 0.3f, num2 * 0.4f);
		this.rectBookBound = new Rect(this.rectBook);
		this.rectBookBound.height = this.rectBookBound.height - 2f * num7;
		this.rectScroll = GeomUtil.scaleCentered(this.rectBook, 0.3f, 1f);
		this.rectScroll.height = num7;
		this.rectScroll.y = this.rectBook.yMax - 2f * num7;
		Rect rect = GeomUtil.scaleCentered(this.rectBook, 0.95f, 0.9f);
		Rect dst;
		dst..ctor(rect);
		dst.width *= 0.133426f;
		Rect dst2;
		dst2..ctor(rect);
		dst2.width *= 0.133426f;
		dst2.x = rect.xMax - dst2.width;
		float num8 = Mathf.Lerp(dst.x, dst.xMax, 0.3f) / (float)Screen.width;
		float num9 = Mathf.Lerp(dst2.x, dst2.xMax, 0.7f) / (float)Screen.width;
		Rect rect2;
		rect2..ctor(num8, 0f, num9 - num8, 1f);
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
		this.gui3d.pushTransform();
		this.gui3d.setDepth(890f);
		float num10 = (float)Screen.height * 0.2f;
		float num11 = num10 * 809f / 207f;
		Rect dst3;
		dst3..ctor((float)(Screen.width * 1) / 3f - num11 / 2f, (float)Screen.height * 0.72f - num10 / 2f, num11, num10);
		this.gui3d.DrawObject(dst3, this.imShadowPlane1);
		Rect dst4;
		dst4..ctor((float)(Screen.width * 2) / 3f - num11 / 2f, (float)Screen.height * 0.72f - num10 / 2f, num11, num10);
		this.gui3d.DrawObject(dst4, this.imShadowPlane2);
		this.gui3d.setDepth(880f);
		float num12 = (float)Screen.height * 0.23f;
		float num13 = num12 * 635f / 253f;
		Rect dst5;
		dst5..ctor((float)(Screen.width * 2) / 3f - num13 / 2f, (float)Screen.height * 0.67f - num12 / 2f, num13, num12);
		this.gui3d.DrawObject(dst5, this.imCoinsPlane);
		this.gui3d.popTransform();
		if (this.scrollBook != null)
		{
			this.scrollBook.setRect(this.rectBook);
			this.scrollBook.setBoundingRect(this.rectBookBound);
		}
	}

	// Token: 0x06001022 RID: 4130 RVA: 0x0000CA40 File Offset: 0x0000AC40
	private void StepCollection(int num)
	{
		this.scrollBook.scrollTo(this.scrollBook.scrollPos() + (float)num);
	}

	// Token: 0x06001023 RID: 4131 RVA: 0x0006C5BC File Offset: 0x0006A7BC
	private void CheckScreenResolutionChanged(bool forceChange)
	{
		if (forceChange || this.lastScreenWidth != Screen.width || this.lastScreenHeight != Screen.height)
		{
			this.lastScreenWidth = Screen.width;
			this.lastScreenHeight = Screen.height;
			this.OnResolutionChanged();
		}
	}

	// Token: 0x06001024 RID: 4132 RVA: 0x0000CA5B File Offset: 0x0000AC5B
	public void OnResolutionChanged()
	{
		this.SetupPositions();
	}

	// Token: 0x06001025 RID: 4133 RVA: 0x0000CA63 File Offset: 0x0000AC63
	protected void Update()
	{
		if (!this.inited)
		{
			return;
		}
		this.CheckScreenResolutionChanged(false);
		this.HandleMouseScrollWheel(Input.GetAxis("Mouse ScrollWheel"));
		if (Input.GetKeyDown(13))
		{
			this.PlayGlimmer();
		}
	}

	// Token: 0x06001026 RID: 4134 RVA: 0x0006C60C File Offset: 0x0006A80C
	public void ShowFinalRewards()
	{
		this.showFinalRewards = true;
		this.imBookPlane.renderer.enabled = false;
		this.imLeftBookPlane.renderer.enabled = false;
		this.imRightBookPlane.renderer.enabled = false;
		this.imCoinsPlane.renderer.enabled = true;
		this.imShadowPlane1.renderer.enabled = true;
		this.imShadowPlane2.renderer.enabled = true;
		this.scrollBook.setCards(new List<Card>());
		this.scrollBook.setInputEnabled(false);
		int num = 0;
		foreach (Card cardInfo in this.pickedCards)
		{
			GameObject gameObject = PrimitiveFactory.createPlane(false);
			gameObject.name = "Final Reward CardView";
			CardView cardView = gameObject.AddComponent<CardView>();
			cardView.setShader("Scrolls/Unlit/Transparent/ZWrite");
			cardView.init(null, cardInfo, 200);
			cardView.applyHighResTexture();
			cardView.enableShowHelp();
			cardView.setTransparency(0f);
			cardView.transform.parent = base.transform;
			this.finalRewardViews.Add(cardView);
			Vector3 vector = CardView.CardLocalScale();
			float num2 = vector.x / vector.z;
			float num3 = (float)Screen.height * 0.38f;
			float num4 = num2 * num3;
			float num5 = (float)Screen.height * 0.05f;
			float num6 = num5 * Mathf.Max(1f, (float)(this.scrollsToPick - 1) * 0.75f);
			float num7 = num4 - num6;
			Rect dst;
			dst..ctor((float)Screen.width * 0.28f + num7 * ((float)num - (float)this.pickedCards.Count / 2f), (float)Screen.height * 0.4f + num5 / 2f * ((float)num - (float)this.pickedCards.Count / 2f), num4, num3);
			this.gui3d.setDepth(850f - (float)(30 * num));
			this.gui3d.DrawObject(dst, gameObject);
			num++;
		}
		base.StartCoroutine(this.ShowFinalCardViews());
		base.StartCoroutine(this.ShowGlimmer());
	}

	// Token: 0x06001027 RID: 4135 RVA: 0x0006C864 File Offset: 0x0006AA64
	private IEnumerator ShowFinalCardViews()
	{
		float duration = 0.5f;
		float t = 0f;
		float timeStarted = Time.time;
		while (t < 1f)
		{
			t = Mathf.Min((Time.time - timeStarted) / duration, 1f);
			foreach (CardView cv in this.finalRewardViews)
			{
				cv.setTransparency(t);
			}
			this.imCoinsPlane.renderer.material.color = new Color(1f, 1f, 1f, t);
			this.imShadowPlane1.renderer.material.color = new Color(1f, 1f, 1f, t);
			this.imShadowPlane2.renderer.material.color = new Color(1f, 1f, 1f, t);
			this.finalCardAlpha = t;
			yield return null;
		}
		foreach (CardView cv2 in this.finalRewardViews)
		{
			cv2.setTransparency(1f);
		}
		this.imCoinsPlane.renderer.material.color = new Color(1f, 1f, 1f, 1f);
		this.imShadowPlane1.renderer.material.color = new Color(1f, 1f, 1f, 1f);
		this.imShadowPlane2.renderer.material.color = new Color(1f, 1f, 1f, 1f);
		this.finalCardAlpha = 1f;
		this.loopGoldSound = true;
		base.StartCoroutine(this.GoldSound());
		t = 0f;
		timeStarted = Time.time;
		duration = (float)this.goldEarned / 500f;
		while (t < 1f)
		{
			t = Mathf.Min((Time.time - timeStarted) / duration, 1f);
			this.goldEarnedCountup = Mathf.RoundToInt(Mathf.Lerp(0f, (float)this.goldEarned, t));
			yield return null;
		}
		this.loopGoldSound = false;
		yield break;
	}

	// Token: 0x06001028 RID: 4136 RVA: 0x0006C880 File Offset: 0x0006AA80
	private IEnumerator GoldSound()
	{
		App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_coin_tally_loop", true);
		App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_coin_tally_loop", true);
		while (this.loopGoldSound)
		{
			yield return null;
		}
		App.AudioScript.StopSound("Sounds/hyperduck/UI/ui_coin_tally_loop", false);
		App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_coin_tally_end");
		yield break;
	}

	// Token: 0x06001029 RID: 4137 RVA: 0x0006C89C File Offset: 0x0006AA9C
	private IEnumerator ShowGlimmer()
	{
		for (;;)
		{
			float waitDuration = Random.Range(0f, 3f);
			yield return new WaitForSeconds(waitDuration);
			this.PlayGlimmer();
		}
		yield break;
	}

	// Token: 0x0600102A RID: 4138 RVA: 0x0006C8B8 File Offset: 0x0006AAB8
	protected void HandleMouseScrollWheel(float scrollDelta)
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

	// Token: 0x0600102B RID: 4139 RVA: 0x0006C924 File Offset: 0x0006AB24
	protected void OnGUI()
	{
		GUI.depth = 21;
		if (Event.current.type == 7)
		{
			this.gui3d.frameBegin();
			this.OnGUI_draw3D();
			this.gui3d.frameEnd();
		}
		GUI.skin = this.regularUI;
		int fontSize = GUI.skin.label.fontSize;
		Texture2D texture2D = ResourceManager.LoadTexture("Limited/final_judgement");
		float num = this.rectBook.height * 0.8f;
		float num2 = (float)(texture2D.width / texture2D.height) * num;
		GUI.Label(new Rect((float)(Screen.width / 2) - num2 / 2f, this.rectBook.yMin - this.rectBook.height * 1.4f, num2, num), texture2D);
		if (!this.showFinalRewards)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = this.skinScrollbar;
			int num3 = Mathf.Max(1, this.scrollBook.getNumPiles() - 1);
			float num4 = this.scrollBook.scrollPos();
			GUI.SetNextControlName("lrScrollbar");
			float num5 = GUIUtil.HorizontalScrollbar(this.rectScroll, num4, 0.175f * (float)num3, 0f, (float)num3);
			if (!Mth.isClose(num4, num5, 1E-05f))
			{
				this.scrollBook.scrollTo(num5);
			}
			GUI.skin = skin;
			GUI.skin.label.fontSize = Screen.height / 24;
			GUI.Label(new Rect(this.rectBook.x, this.rectBook.yMin - this.rectBook.height * 0.75f, this.rectBook.width, this.rectBook.height * 0.4f), "You won " + this.gamesWon + " matches");
			string text = "Your performance earned you";
			switch (this.gamesWon)
			{
			case 0:
				text = "Your valiant efforts earned you";
				break;
			case 1:
				text = "Your bravery earned you";
				break;
			case 2:
				text = "Well done! You've earned";
				break;
			case 3:
				text = "Skillfully done! You've earned";
				break;
			case 4:
				text = "Masterfully done! You've earned";
				break;
			case 5:
				text = "Your mastery of Calling earns you";
				break;
			}
			GUI.skin.label.fontSize = Screen.height / 24;
			GUI.Label(new Rect(this.rectBook.x, this.rectBook.yMin - this.rectBook.height * 0.6f, this.rectBook.width, this.rectBook.height * 0.4f), string.Concat(new object[]
			{
				text,
				" <color=#ffdd44>",
				this.goldEarned,
				" gold</color> and <color=#ffcc66>",
				this.scrollsToPick,
				" scroll",
				(this.scrollsToPick <= 1) ? string.Empty : "s",
				"</color>"
			}));
			string text2 = "any";
			if (this.pickingRarityLevel == 1)
			{
				text2 = "an uncommon or common";
			}
			else if (this.pickingRarityLevel == 0)
			{
				text2 = "a common";
			}
			GUI.skin.label.fontSize = Screen.height / 32;
			GUI.Label(new Rect(this.rectBook.x, this.rectBook.yMin - this.rectBook.height * 0.2f, this.rectBook.width, this.rectBook.height * 0.2f), string.Concat(new object[]
			{
				"Pick ",
				text2,
				" scroll to keep [",
				this.pickedCards.Count + 1,
				"/",
				this.scrollsToPick,
				"]"
			}));
		}
		else
		{
			GUI.color = new Color(1f, 1f, 1f, this.finalCardAlpha);
			float num6 = (float)Screen.height * 0.25f;
			float num7 = (float)Screen.height * 0.06f;
			int fontSize2 = GUI.skin.button.fontSize;
			GUI.skin.button.fontSize = Screen.height / 24;
			if (GUI.Button(new Rect((float)(Screen.width / 2) - num6 * 1.1f, (float)Screen.height * 0.87f, num6, num7), "Collect reward"))
			{
				App.AudioScript.StopSound("Sounds/hyperduck/UI/ui_coin_tally_loop", false);
				App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_victory");
				GUI.skin.button.fontSize = fontSize2;
				App.Communicator.send(new SelectRewardLimitedMessage(this.deckName, this.pickedCards));
			}
			if (GUI.Button(new Rect((float)(Screen.width / 2) + num6 * 0.1f, (float)Screen.height * 0.87f, num6, num7), "Pick again"))
			{
				App.AudioScript.StopSound("Sounds/hyperduck/UI/ui_coin_tally_loop", false);
				App.AudioScript.PlaySFX("Sounds/hyperduck/UI/ui_button_click");
				GUI.skin.button.fontSize = fontSize2;
				SceneLoader.loadScene("_LimitedReward");
			}
			GUI.skin.button.fontSize = fontSize2;
			GUI.skin.label.fontSize = Screen.height / 6;
			GUI.Label(new Rect((float)(Screen.width * 2) / 3f - (float)Screen.width * 0.2f, (float)Screen.height * 0.39f + (float)Screen.height * 0.007f, (float)Screen.width * 0.4f, (float)Screen.height * 0.3f), "<color=#000000>" + this.goldEarnedCountup + "</color>");
			GUI.Label(new Rect((float)(Screen.width * 2) / 3f - (float)Screen.width * 0.2f, (float)Screen.height * 0.39f, (float)Screen.width * 0.4f, (float)Screen.height * 0.3f), "<color=#ffcc44>" + this.goldEarnedCountup + "</color>");
			GUI.skin.label.fontSize = Screen.height / 14;
			GUI.Label(new Rect((float)(Screen.width * 2) / 3f - (float)Screen.width * 0.2f, (float)Screen.height * 0.315f + (float)Screen.height * 0.004f, (float)Screen.width * 0.4f, (float)Screen.height * 0.3f), "<color=#000000>Gold</color>");
			GUI.Label(new Rect((float)(Screen.width * 2) / 3f - (float)Screen.width * 0.2f, (float)Screen.height * 0.315f, (float)Screen.width * 0.4f, (float)Screen.height * 0.3f), "<color=#ee9922>Gold</color>");
			GUI.color = Color.white;
		}
		GUI.skin.label.fontSize = fontSize;
	}

	// Token: 0x0600102C RID: 4140 RVA: 0x0006D058 File Offset: 0x0006B258
	public void ConfirmScrollChoice(bool confirm)
	{
		if (confirm)
		{
			App.AudioScript.PlaySFX("Sounds/scroll_purchase_01");
			this.rewardButtons.setButtonsEnabled(false);
			switch (this.lastCardClicked.getRarity())
			{
			case 0:
				this.PlayScrollEffect("Scroll_appear_1a_appear");
				this.PlayScrollEffect("Scroll_appear_1_2b_rimshine");
				break;
			case 1:
				this.PlayScrollEffect("Scroll_appear_2a_appear");
				this.PlayScrollEffect("Scroll_appear_1_2b_rimshine");
				break;
			case 2:
				this.PlayScrollEffect("Scroll_appear_3a_appear");
				this.PlayScrollEffect("Scroll_appear_3b_rimshine");
				break;
			}
		}
		else if (!this.cardOverlay.IsHovered())
		{
			this.cardOverlay.Hide();
			this.rewardButtons.enabled = false;
			this.scrollBook.setInputEnabled(true);
		}
	}

	// Token: 0x0600102D RID: 4141 RVA: 0x0006D134 File Offset: 0x0006B334
	public void PlayScrollEffect(string file)
	{
		float num = 0.25f;
		Vector3 vector;
		vector..ctor(0.025f, 0.05f, 0f);
		Log.info("Playing effect: " + file);
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<MeshRenderer>();
		gameObject.name = "BuyEffect_";
		EffectPlayer effectPlayer = gameObject.AddComponent<EffectPlayer>();
		effectPlayer.setMaterialToUse(new Material(ResourceManager.LoadShader("Scrolls/StoreEffect/Unlit/Transparent")));
		effectPlayer.init("BuyEffect/" + file, 1, this, 94000, new Vector3(0.68f * num, 0.4264f * num, 0.5f), false, string.Empty, 0);
		effectPlayer.getAnimPlayer().waitForUpdate();
		gameObject.transform.parent = this.cardOverlay.GetCardView().transform.parent;
		gameObject.transform.position = new Vector3(0f, 0f, 1f) + vector;
		effectPlayer.layer = 8;
		this.effectsPlaying++;
	}

	// Token: 0x0600102E RID: 4142 RVA: 0x0006D240 File Offset: 0x0006B440
	public void PlayGlimmer()
	{
		float num = (1.5f + Random.Range(0f, 2f)) * (float)Screen.height / 30f;
		Vector3 vector;
		vector..ctor((float)(200 + Random.Range(0, 200)), (float)(-300 + Random.Range(0, 200)), 0f);
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<MeshRenderer>();
		gameObject.name = "Glimmer_";
		EffectPlayer effectPlayer = gameObject.AddComponent<EffectPlayer>();
		effectPlayer.setMaterialToUse(new Material(ResourceManager.LoadShader("Scrolls/StoreEffect/Unlit/Transparent")));
		effectPlayer.init("Glimmer", 1, this, 94000, new Vector3(0.409f * num, 0.4264f * num, 0.5f), false, string.Empty, 0);
		effectPlayer.getAnimPlayer().waitForUpdate();
		gameObject.transform.position = base.camera.ScreenToWorldPoint(new Vector3((float)(Screen.width * 2) / 3f - (float)Screen.height * 0.1f + Random.Range(0f, (float)Screen.height * 0.2f), (float)Screen.height * 0.3f + Random.Range(0f, (float)Screen.height * 0.12f), 500f));
	}

	// Token: 0x0600102F RID: 4143 RVA: 0x0006D388 File Offset: 0x0006B588
	protected void OnGUI_draw3D()
	{
		this.gui3d.setDepth(950f);
		Texture2D tex = ResourceManager.LoadTexture("DeckBuilder/bg");
		this.gui3d.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), tex);
		this.gui3d.setDepth(949f);
		float num = this.rectSubMenu.y + this.rectSubMenu.height;
		float num2 = this.rectCard.height * 1.15f;
		Rect rect;
		rect..ctor((float)(Screen.width / 2) - this.rectCard.width * 1.2f, this.rectCard.y - this.rectCard.height * 0.1f, this.rectCard.width * 2.4f, this.rectCard.height * 1.2f);
	}

	// Token: 0x06001030 RID: 4144 RVA: 0x0006D474 File Offset: 0x0006B674
	public override void handleMessage(Message m)
	{
		if (m is OkMessage)
		{
			OkMessage okMessage = (OkMessage)m;
			if (okMessage.isType(typeof(SelectRewardLimitedMessage)))
			{
				App.WaitingReward = null;
				SceneLoader.loadScene("_Lobby");
			}
		}
		else if (m is FailMessage)
		{
			FailMessage failMessage = (FailMessage)m;
			if (failMessage.isType(typeof(SelectRewardLimitedMessage)))
			{
				App.Popups.ShowOk(this, "selectrewardfail", "Notification", "Something went wrong when selecting your rewards. Please try again!", "Ok");
			}
		}
	}

	// Token: 0x06001031 RID: 4145 RVA: 0x0000CA9A File Offset: 0x0000AC9A
	public void PopupOk(string popupType)
	{
		if (popupType == "selectrewardfail")
		{
			SceneLoader.loadScene("_LimitedReward");
		}
	}

	// Token: 0x06001032 RID: 4146 RVA: 0x000028DF File Offset: 0x00000ADF
	public void onCardEnterView(Card card, ICardView view)
	{
	}

	// Token: 0x06001033 RID: 4147 RVA: 0x000028DF File Offset: 0x00000ADF
	public void onStartDragCard(Card card, int cardIndex, GameObject g)
	{
	}

	// Token: 0x06001034 RID: 4148 RVA: 0x000028DF File Offset: 0x00000ADF
	public void onCardHeld(Card card, int cardIndex, GameObject g)
	{
	}

	// Token: 0x06001035 RID: 4149 RVA: 0x0000CAB6 File Offset: 0x0000ACB6
	public void onCardClicked(Card card, int cardIndex, GameObject g)
	{
		this.lastCardClicked = card;
		this.cardOverlay.Show(card);
		this.rewardButtons.enabled = true;
		this.scrollBook.setInputEnabled(false);
		App.AudioScript.PlaySFX("Sounds/hyperduck/DeckBuilder/db_scroll_pickup");
	}

	// Token: 0x06001036 RID: 4150 RVA: 0x000028DF File Offset: 0x00000ADF
	public void onCardDoubleClicked(Card card, int cardIndex, GameObject g)
	{
	}

	// Token: 0x06001037 RID: 4151 RVA: 0x0006D504 File Offset: 0x0006B704
	public void effectAnimDone(EffectPlayer effect, bool loop)
	{
		Object.Destroy(effect.gameObject);
		if (effect.name.Contains("BuyEffect"))
		{
			this.effectsPlaying--;
			if (this.lastCardClicked != null && this.effectsPlaying == 0)
			{
				this.rewardButtons.enabled = false;
				base.StartCoroutine(this.CardOverlayFadeout());
				this.cardsToPickFrom.Remove(this.lastCardClicked);
				this.pickedCards.Add(this.lastCardClicked);
				this.lastCardClicked = null;
				this.ShowNextReward();
			}
		}
	}

	// Token: 0x06001038 RID: 4152 RVA: 0x000028DF File Offset: 0x00000ADF
	public void locator(EffectPlayer effect, AnimLocator loc)
	{
	}

	// Token: 0x06001039 RID: 4153 RVA: 0x0006D5A0 File Offset: 0x0006B7A0
	public IEnumerator CardOverlayFadeout()
	{
		yield return base.StartCoroutine(this.cardOverlay.FadeOutCoroutine(0.4f));
		this.scrollBook.setInputEnabled(true);
		this.rewardButtons.setButtonsEnabled(true);
		yield break;
	}

	// Token: 0x04000C9B RID: 3227
	private const int CMASK_CARD = 4;

	// Token: 0x04000C9C RID: 3228
	private const float ZBackground = 950f;

	// Token: 0x04000C9D RID: 3229
	private const float ZScrollBook = 900f;

	// Token: 0x04000C9E RID: 3230
	private const float ZCardRule = 850f;

	// Token: 0x04000C9F RID: 3231
	private const string ControlName_Scrollbar = "lrScrollbar";

	// Token: 0x04000CA0 RID: 3232
	public RenderTexture cardRenderTexture;

	// Token: 0x04000CA1 RID: 3233
	private bool inited;

	// Token: 0x04000CA2 RID: 3234
	private Vector3 _mainCameraPosition;

	// Token: 0x04000CA3 RID: 3235
	protected Gui3D gui3d;

	// Token: 0x04000CA4 RID: 3236
	public EList<Card> allPicks;

	// Token: 0x04000CA5 RID: 3237
	private GameObject lightSource;

	// Token: 0x04000CA6 RID: 3238
	private GUISkin regularUI;

	// Token: 0x04000CA7 RID: 3239
	[SerializeField]
	private Camera scrollBookCamera;

	// Token: 0x04000CA8 RID: 3240
	private ScrollBook scrollBook;

	// Token: 0x04000CA9 RID: 3241
	private GUISkin skinScrollbar;

	// Token: 0x04000CAA RID: 3242
	private GameObject imBookPlane;

	// Token: 0x04000CAB RID: 3243
	private GameObject imLeftBookPlane;

	// Token: 0x04000CAC RID: 3244
	private GameObject imRightBookPlane;

	// Token: 0x04000CAD RID: 3245
	private GameObject imCoinsPlane;

	// Token: 0x04000CAE RID: 3246
	private GameObject imShadowPlane1;

	// Token: 0x04000CAF RID: 3247
	private GameObject imShadowPlane2;

	// Token: 0x04000CB0 RID: 3248
	protected Rect rectBook;

	// Token: 0x04000CB1 RID: 3249
	protected Rect rectBookBound;

	// Token: 0x04000CB2 RID: 3250
	protected Rect rectScroll;

	// Token: 0x04000CB3 RID: 3251
	protected Rect rectCard;

	// Token: 0x04000CB4 RID: 3252
	protected Rect rectSubMenu;

	// Token: 0x04000CB5 RID: 3253
	private CardOverlay cardOverlay;

	// Token: 0x04000CB6 RID: 3254
	private LimitedRewardButtons rewardButtons;

	// Token: 0x04000CB7 RID: 3255
	private Card lastCardClicked;

	// Token: 0x04000CB8 RID: 3256
	private List<Card> cardsToPickFrom = new List<Card>();

	// Token: 0x04000CB9 RID: 3257
	private List<Card> pickedCards = new List<Card>();

	// Token: 0x04000CBA RID: 3258
	private bool showFinalRewards;

	// Token: 0x04000CBB RID: 3259
	private int gamesWon;

	// Token: 0x04000CBC RID: 3260
	private int gamesLeft;

	// Token: 0x04000CBD RID: 3261
	private int gamesTotal;

	// Token: 0x04000CBE RID: 3262
	private int commonsToPick;

	// Token: 0x04000CBF RID: 3263
	private int uncommonsToPick;

	// Token: 0x04000CC0 RID: 3264
	private int raresToPick;

	// Token: 0x04000CC1 RID: 3265
	private int scrollsToPick;

	// Token: 0x04000CC2 RID: 3266
	private int goldEarned;

	// Token: 0x04000CC3 RID: 3267
	private int goldEarnedCountup;

	// Token: 0x04000CC4 RID: 3268
	private string deckName;

	// Token: 0x04000CC5 RID: 3269
	private int pickingRarityLevel;

	// Token: 0x04000CC6 RID: 3270
	private int effectsPlaying;

	// Token: 0x04000CC7 RID: 3271
	private List<CardView> finalRewardViews = new List<CardView>();

	// Token: 0x04000CC8 RID: 3272
	private int lastScreenWidth;

	// Token: 0x04000CC9 RID: 3273
	private int lastScreenHeight;

	// Token: 0x04000CCA RID: 3274
	private float finalCardAlpha;

	// Token: 0x04000CCB RID: 3275
	private bool loopGoldSound;
}
