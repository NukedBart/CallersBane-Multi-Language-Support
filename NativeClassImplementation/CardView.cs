using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x0200011D RID: 285
public class CardView : AbstractCardView, IColorSettable, GetCardStatsMessage.ICardStatsReceiver, iEffect
{
	// Token: 0x060008EC RID: 2284 RVA: 0x00045D80 File Offset: 0x00043F80
	static CardView()
	{
		CardView.FontTitle = (Font)ResourceManager.Load("Fonts/dwarvenaxebb");
		CardView.FontText = (Font)ResourceManager.Load("Fonts/HoneyMeadBB");
		CardView.FontTextBold = (Font)ResourceManager.Load("Fonts/HoneyMeadBB_bold");
		CardView.FontTextBoldItalic = (Font)ResourceManager.Load("Fonts/HoneyMeadBB_boldital");
	}

	// Token: 0x060008ED RID: 2285 RVA: 0x00045E58 File Offset: 0x00044058
	public static GameObject createTextMesh(Font font)
	{
		GameObject gameObject = new GameObject("TextHolder");
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		TextMesh textMesh = gameObject.AddComponent<TextMesh>();
		textMesh.lineSpacing = 0.85f;
		textMesh.fontSize = 24;
		textMesh.characterSize = 2.58f;
		gameObject.name = "3DText_tier";
		textMesh.font = CardView.FontTextBold;
		meshRenderer.material = textMesh.font.material;
		meshRenderer.material.shader = ResourceManager.LoadShader(CardView.fontShader);
		gameObject.renderer.material.color = CardView.cardTextFontColor;
		return gameObject;
	}

	// Token: 0x060008EE RID: 2286 RVA: 0x00007BBD File Offset: 0x00005DBD
	public CardView setType(CardView.Type type)
	{
		this.type = type;
		return this;
	}

	// Token: 0x060008EF RID: 2287 RVA: 0x00007BC7 File Offset: 0x00005DC7
	private void Start()
	{
		App.GlobalMessageHandler.registerCardStatsListener(this);
	}

	// Token: 0x060008F0 RID: 2288 RVA: 0x00007BD4 File Offset: 0x00005DD4
	private void OnDestroy()
	{
		App.GlobalMessageHandler.unregisterCardStatsListener(this);
	}

	// Token: 0x060008F1 RID: 2289 RVA: 0x00007BE1 File Offset: 0x00005DE1
	public void init(iCardRule callBackTarget, Unit unit, int rendQ)
	{
		this.init(callBackTarget, unit.getCard(), rendQ, unit.getBuffs(), unit.getTilePosition());
	}

	// Token: 0x060008F2 RID: 2290 RVA: 0x00007BFD File Offset: 0x00005DFD
	public void init(iCardRule callBackTarget, Card cardInfo, int rendQ)
	{
		this.init(callBackTarget, cardInfo, rendQ, null, new TilePosition());
	}

	// Token: 0x060008F3 RID: 2291 RVA: 0x00045EF0 File Offset: 0x000440F0
	public void init(iCardRule callBackTarget, Card card, int rendQ, List<EnchantmentInfo> buffArr, TilePosition pos)
	{
		if (!Directory.Exists(CardView.RESOURCE_DIRECTORY))
		{
			Directory.CreateDirectory(CardView.RESOURCE_DIRECTORY);
		}
		this.card = card;
		this.rayCastCamera = Camera.main;
		this.useRendQ = (rendQ >= 0);
		this.rendQ = rendQ;
		this.currentLayer = base.gameObject.layer;
		this.callBackTarget = callBackTarget;
		base.tag = "blinkable_card";
		this.pos = pos;
		this.buffs = (buffArr ?? new List<EnchantmentInfo>());
		if (this.nextFoilTimeStamp < 0L)
		{
			this.nextFoilTimeStamp = TimeUtil.CurrentTimeMillis() + (long)Random.Range(0, 5000);
		}
		this.firstPageText = new GameObject("FirstPageText");
		this.firstPageText.layer = base.gameObject.layer;
		UnityUtil.addChild(base.gameObject, this.firstPageText);
		this.pages.Add(this.firstPageText);
		ResourceType resource = card.getCardType().getResource();
		this.glowColor = ResourceColor.get(resource);
		this.audioScript = App.AudioScript;
		this.isInited = true;
		this.createLevelGraphics();
		this.createTexts();
		base.renderer.material = this.createMaterial();
		this.cardImage = PrimitiveFactory.createPlane(false);
		this.cardImage.name = "Image_Plane";
		UnityUtil.addChild(base.gameObject, this.cardImage);
		this.cardImage.transform.localScale = new Vector3(0.752f, 1f, 0.335f);
		this.cardImage.transform.localPosition = new Vector3(0f, -0.001f, -1.3825f);
		this.cardImage.renderer.material = this.createMaterial();
		this.icoBG = PrimitiveFactory.createPlane(false);
		this.icoBG.name = "Resource_Icon_BG";
		UnityUtil.addChild(base.gameObject, this.icoBG);
		this.icoBG.transform.localPosition = new Vector3(-0.05f, 0.015f, -4.79f);
		this.icoBG.transform.localScale = new Vector3(0.316f, 1f, 0.118f);
		this.ico = PrimitiveFactory.createPlane(false);
		this.ico.name = "Resource_Icon";
		this.ico.tag = "blinkable_cost";
		UnityUtil.addChild(base.gameObject, this.ico);
		this.ico.transform.localPosition = new Vector3(0.45f, 0.02f, -4.8f);
		this.ico.transform.localScale = new Vector3(0.14f, 1f, 0.0756f);
		this.ico.renderer.material = new Material(ResourceManager.LoadShader(this.shader));
		this.icoBG.renderer.material = new Material(ResourceManager.LoadShader(this.shader));
		this.icoBG.renderer.material.mainTexture = ResourceManager.LoadTexture("Scrolls/NewGraphics/scrolls__resource_cost");
		this.statsBG = PrimitiveFactory.createPlane(false);
		this.statsBG.name = "Stats_BG";
		UnityUtil.addChild(base.gameObject, this.statsBG);
		this.statsBG.transform.localScale = new Vector3(0.754f, 1f, 0.124f);
		this.statsBG.transform.localPosition = new Vector3(-0.05f, 0.009f, 0.435f);
		this.statsBG.renderer.material = new Material(ResourceManager.LoadShader(this.shader));
		this.statsBG.renderer.material.mainTexture = ResourceManager.LoadTexture("Scrolls/NewGraphics/scrolls__statsbar");
		this.renderQueueOffsets.Add(this.cardImage.renderer, -10);
		this.renderQueueOffsets.Add(base.renderer, -9);
		this.renderQueueOffsets.Add(this.icoBG.renderer, -7);
		this.renderQueueOffsets.Add(this.ico.renderer, -6);
		this.renderQueueOffsets.Add(this.statsBG.renderer, -5);
		this.card = null;
		this.updateGraphics(card);
		this.colorChanger.Initialize(base.gameObject);
		this.setRenderQueue(rendQ);
		this.highResFetcher = new TextureFetcher(this, new TextureFetcher.onFetched(this.onFetchedCardImage));
	}

	// Token: 0x060008F4 RID: 2292 RVA: 0x00007C0E File Offset: 0x00005E0E
	public int profileId()
	{
		return this._profileId;
	}

	// Token: 0x060008F5 RID: 2293 RVA: 0x00007C16 File Offset: 0x00005E16
	public void setProfileId(int profileId)
	{
		this._profileId = profileId;
	}

	// Token: 0x060008F6 RID: 2294 RVA: 0x00007C1F File Offset: 0x00005E1F
	public void overrideCost(int cost)
	{
		this.overriddenCost = new int?(cost);
	}

	// Token: 0x060008F7 RID: 2295 RVA: 0x00007C2D File Offset: 0x00005E2D
	public int getCostOrOverridden()
	{
		return (this.overriddenCost == null) ? this.card.getCostTotal() : this.overriddenCost.Value;
	}

	// Token: 0x060008F8 RID: 2296 RVA: 0x00007C5A File Offset: 0x00005E5A
	public void setTooltipEnabled(bool enabled)
	{
		this.disableTooltip = !enabled;
	}

	// Token: 0x060008F9 RID: 2297 RVA: 0x00007C66 File Offset: 0x00005E66
	public void setStartPos(Vector3 startPos)
	{
		this.startPos = startPos;
	}

	// Token: 0x060008FA RID: 2298 RVA: 0x00007C6F File Offset: 0x00005E6F
	public Vector3 getStartPos()
	{
		return this.startPos;
	}

	// Token: 0x060008FB RID: 2299 RVA: 0x00007C77 File Offset: 0x00005E77
	public string sortKey()
	{
		return this.card.getName() + this.card.getId();
	}

	// Token: 0x060008FC RID: 2300 RVA: 0x00007C99 File Offset: 0x00005E99
	public void doneMoving()
	{
		this.cardMoving = false;
	}

	// Token: 0x060008FD RID: 2301 RVA: 0x00046380 File Offset: 0x00044580
	public bool isClicked(Vector2 mousePos, bool mousePressed)
	{
		if (!mousePressed || this.rayCastCamera == null)
		{
			return false;
		}
		RaycastHit raycastHit = default(RaycastHit);
		Ray ray = this.rayCastCamera.ScreenPointToRay(mousePos);
		return Physics.Raycast(ray, ref raycastHit, this.rayCastCamera.farClipPlane, 1 << this.currentLayer) && UnityUtil.hasParent(raycastHit.collider.gameObject.transform, base.transform);
	}

	// Token: 0x060008FE RID: 2302 RVA: 0x00046404 File Offset: 0x00044604
	public void onSelect()
	{
		if (this.cardGrayedOut)
		{
			return;
		}
		if (this.effectPlayers.Count != 0)
		{
			Log.info("We already had EffectPlayers!");
			this.deselect();
		}
		this.selected = true;
		GameObject gameObject = new GameObject("selected-player");
		UnityUtil.addChild(base.gameObject, gameObject);
		gameObject.transform.localScale = base.gameObject.transform.localScale;
		gameObject.transform.localPosition = new Vector3(-0.07f, -0.17f, -0.44f);
		gameObject.transform.localEulerAngles = new Vector3(90f, 180f, 0f);
		string text = this.card.getRarityString().ToLower();
		string name = "scroll_selected_" + text + "_back";
		this.effectGlow = this.setupAnim(gameObject.transform, name);
		this.effectGlow.transform.Translate(0f, 0f, 1f);
		this.effectGlow.renderer.material.color = ColorUtil.GetWithAlpha(this.glowColor, 0f);
		this.effectPlayers.Add(this.effectGlow.gameObject);
		name = "scroll_selected_" + text + "_front";
		this.effectWhite = this.setupAnim(gameObject.transform, name);
		this.effectWhite.renderer.material.color = new Color(1f, 1f, 1f, 0f);
		this.effectPlayers.Add(this.effectWhite.gameObject);
		this.effectPlayers.Add(gameObject);
	}

	// Token: 0x060008FF RID: 2303 RVA: 0x000465B8 File Offset: 0x000447B8
	private EffectPlayer setupAnim(Transform parent, string name)
	{
		GameObject gameObject = new GameObject("player-" + name);
		gameObject.AddComponent<MeshRenderer>();
		UnityUtil.addChild(parent.gameObject, gameObject);
		Vector3 baseScale;
		baseScale..ctor(1.5857f, 1.6463f, 1f);
		EffectPlayer effectPlayer = gameObject.AddComponent<EffectPlayer>();
		effectPlayer.renderer.material = this.createMaterial();
		effectPlayer.init("battlegui-cardglow_cast_target", 1, this, this.getRenderQueue(-5), baseScale, 0);
		effectPlayer.layer = 9;
		this.renderQueueOffsets.Add(effectPlayer.renderer, -5);
		effectPlayer.getAnimPlayer().setAnimationId(name);
		return effectPlayer;
	}

	// Token: 0x06000900 RID: 2304 RVA: 0x00046658 File Offset: 0x00044858
	private EffectPlayer setupFoilAnim(Transform parent, string name, string location, string animName)
	{
		GameObject gameObject = new GameObject(name);
		gameObject.AddComponent<MeshRenderer>();
		UnityUtil.addChild(parent.gameObject, gameObject);
		Vector3 baseScale;
		baseScale..ctor(1.5857f, 1.6463f, 1f);
		EffectPlayer effectPlayer = gameObject.AddComponent<EffectPlayer>();
		effectPlayer.setMaterialToUse(new Material(ResourceManager.LoadShader("Scrolls/StoreEffect/Unlit/Transparent")));
		effectPlayer.scaleTransform = base.transform;
		effectPlayer.init(location, 1, this, (!this.useRendQ) ? -1 : this.getRenderQueue(-8), baseScale, false, string.Empty, 0);
		effectPlayer.layer = this.currentLayer;
		this.renderQueueOffsets.Add(effectPlayer.renderer, -8);
		effectPlayer.getAnimPlayer().setAnimationId(animName);
		return effectPlayer;
	}

	// Token: 0x06000901 RID: 2305 RVA: 0x00046718 File Offset: 0x00044918
	private void disableFoilRendering()
	{
		foreach (EffectPlayer effectPlayer in base.transform.GetComponentsInChildren<EffectPlayer>())
		{
			effectPlayer.enabled = false;
		}
	}

	// Token: 0x06000902 RID: 2306 RVA: 0x00046750 File Offset: 0x00044950
	private void enableFoilRendering(bool enable)
	{
		this.foilRainbowDisabled = !enable;
		if (this.hasVisuals() && this.card.isAtleastTier(2))
		{
			this.foilPlate.renderer.enabled = true;
			if (this.foilAnim != null)
			{
				this.foilAnim.renderer.enabled = true;
			}
		}
		else
		{
			this.foilPlate.renderer.enabled = false;
		}
		if (!enable)
		{
			this.foilPlate.renderer.enabled = false;
			if (this.foilAnim != null)
			{
				this.foilAnim.renderer.enabled = false;
			}
		}
	}

	// Token: 0x06000903 RID: 2307 RVA: 0x00007CA2 File Offset: 0x00005EA2
	public void onDeselect()
	{
		this.selected = false;
	}

	// Token: 0x06000904 RID: 2308 RVA: 0x00046808 File Offset: 0x00044A08
	private void deselect()
	{
		foreach (GameObject gameObject in this.effectPlayers)
		{
			if (gameObject.renderer)
			{
				this.renderQueueOffsets.Remove(gameObject.renderer);
			}
			Object.Destroy(gameObject);
		}
		this.effectPlayers.Clear();
		this.effectGlow = (this.effectWhite = null);
		this.glowAlpha = 0f;
	}

	// Token: 0x06000905 RID: 2309 RVA: 0x000468AC File Offset: 0x00044AAC
	public override void setLocked(bool locked, bool useLargeLock)
	{
		if (locked == this.locked && useLargeLock == this.largeLock)
		{
			return;
		}
		if (!locked || useLargeLock != this.largeLock)
		{
			Object.Destroy(this.lockGo);
		}
		if (locked)
		{
			this.lockGo = PrimitiveFactory.createTexturedPlane("Store/lock_outline", true);
			UnityUtil.addChild(base.gameObject, this.lockGo);
			if (useLargeLock)
			{
				this.lockGo.transform.localScale = new Vector3(0.5f, 0.3f, 0.3f) * 1.2f;
				this.lockGo.transform.localPosition = new Vector3(0f, 0.1f, 0f);
			}
			else
			{
				this.lockGo.transform.localScale = new Vector3(0.5f, 0.3f, 0.3f) * 0.5f;
				this.lockGo.transform.localPosition = new Vector3(-2.35f, 0.1f, -3.72f);
			}
			this.lockGo.renderer.material.renderQueue = this.getRenderQueue(0);
			this.renderQueueOffsets.Add(this.lockGo.renderer, 0);
		}
		this.locked = locked;
		this.largeLock = useLargeLock;
	}

	// Token: 0x06000906 RID: 2310 RVA: 0x00046A08 File Offset: 0x00044C08
	public void animateOpCast(CardView.OpCastInfo vals)
	{
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"x",
			2.43f,
			"y",
			5.53f,
			"z",
			3f * (float)vals.xMod,
			"time",
			0.6f,
			"onComplete",
			"animateOpCastComplete",
			"onCompleteTarget",
			base.gameObject,
			"easetype",
			iTween.EaseType.easeInExpo
		}));
		iTween.ScaleTo(base.gameObject, iTween.Hash(new object[]
		{
			"x",
			base.transform.localScale.x * 0.3f,
			"z",
			base.transform.localScale.z * 0.3f,
			"time",
			0.6f,
			"easetype",
			iTween.EaseType.easeInExpo
		}));
		this.playCastSound();
		vals.callback.effectDone();
		vals.callback.animateHistory(vals.xMod == -1);
	}

	// Token: 0x06000907 RID: 2311 RVA: 0x00007CAB File Offset: 0x00005EAB
	public void setStartFlying()
	{
		this._isFlying = true;
	}

	// Token: 0x06000908 RID: 2312 RVA: 0x00007CB4 File Offset: 0x00005EB4
	private void animateOpCastComplete()
	{
		this._isFlying = false;
	}

	// Token: 0x06000909 RID: 2313 RVA: 0x00007CBD File Offset: 0x00005EBD
	public bool isFlying()
	{
		return this._isFlying;
	}

	// Token: 0x0600090A RID: 2314 RVA: 0x00046B80 File Offset: 0x00044D80
	private void playCastSound()
	{
		CardType cardType = this.getCardType();
		TagSoundReader.Snd snd = cardType.Sound().get("sound_cast");
		if (!cardType.kind.isSorcery() && snd.name == null)
		{
			return;
		}
		if (snd.name == null)
		{
			string text = "0";
			int cost = cardType.getCost();
			if (cost >= 4)
			{
				text = "1";
			}
			if (cost >= 6)
			{
				text = "2";
			}
			string text2 = cardType.kind.ToString();
			string name = "Sounds/hyperduck/cast_" + (cardType.getResource() + "_" + text).ToLower();
			snd = new TagSoundReader.Snd(name);
		}
		this.playSound(snd);
	}

	// Token: 0x0600090B RID: 2315 RVA: 0x00046C3C File Offset: 0x00044E3C
	private void playSound(string soundName, float volume, float pitch, float delaySeconds)
	{
		if (delaySeconds <= 0f)
		{
			this.audioScript.PlaySFX(soundName, volume, pitch);
		}
		else
		{
			base.StartCoroutine(EnumeratorUtil.chain(new IEnumerator[]
			{
				EnumeratorUtil.Func(new WaitForSeconds(delaySeconds)),
				EnumeratorUtil.Func(delegate()
				{
					this.audioScript.PlaySFX(soundName, volume, pitch);
				})
			}));
		}
	}

	// Token: 0x0600090C RID: 2316 RVA: 0x00007CC5 File Offset: 0x00005EC5
	private void playSound(TagSoundReader.Snd snd)
	{
		this.playSound(snd.name, snd.volume, snd.getPitch(), snd.delay);
	}

	// Token: 0x0600090D RID: 2317 RVA: 0x00007CE5 File Offset: 0x00005EE5
	public long getCardId()
	{
		return this.card.getId();
	}

	// Token: 0x0600090E RID: 2318 RVA: 0x00046CD0 File Offset: 0x00044ED0
	private void FixedUpdate()
	{
		if (this.fadeModifier < 0f)
		{
			this.setTransparency(this.alpha - 0.025f);
		}
		if (this.fadeModifier > 0f)
		{
			this.setTransparency(this.alpha + 0.025f);
		}
		if (this.alpha <= 0f && this.callBackTarget != null)
		{
			this.callBackTarget.HideCardView();
		}
		if (this.alpha >= 1f)
		{
			this.fadeModifier = 0f;
		}
		if (this.effectGlow != null)
		{
			float num = this.glowAlpha;
			this.glowAlpha = Mth.clamp(this.glowAlpha + ((!this.selected) ? -0.25f : 0.25f), 0f, 1f);
			if (num != this.glowAlpha)
			{
				Color withAlpha = ColorUtil.GetWithAlpha(this.glowColor, this.glowAlpha);
				this.effectGlow.renderer.material.color = withAlpha;
				this.effectWhite.renderer.material.color = new Color(1f, 1f, 1f, this.glowAlpha);
			}
			if (this.glowAlpha == 0f)
			{
				this.deselect();
			}
		}
		if (this.isFading)
		{
			float num2 = (Time.time - this.fadeStartTime) / this.fadeDuration;
			if (num2 < 1f)
			{
				this.setColor(Color.Lerp(this.fadeSrcColor, this.fadeDstColor, num2));
			}
			else
			{
				this.setColor(this.fadeDstColor);
				this.isFading = false;
			}
		}
	}

	// Token: 0x0600090F RID: 2319 RVA: 0x00007CF2 File Offset: 0x00005EF2
	private bool hasVisuals()
	{
		return App.Config.settings.preferences.tier_visuals;
	}

	// Token: 0x06000910 RID: 2320 RVA: 0x00046E84 File Offset: 0x00045084
	private void Update()
	{
		if (this.doHandleInput)
		{
			this.handleMouseOver();
		}
		if (!this.foilRainbowDisabled && this.runFoilRainbow && this.hasVisuals() && this.card.isAtleastTier(3))
		{
			this.runFoilEffect();
			this.runFoilRainbow = false;
		}
		long num = TimeUtil.CurrentTimeMillis();
		if (num >= this.nextFoilTimeStamp)
		{
			if (!this.foilRainbowDisabled && this.hasVisuals() && this.card.isAtleastTier(3))
			{
				this.runFoilTwinkle();
			}
			this.nextFoilTimeStamp = num + (long)Random.Range(5000, 10000);
		}
		if (!this.disableTooltip && this.tooltip != string.Empty)
		{
			App.Tooltip.setText(this.tooltip);
		}
	}

	// Token: 0x06000911 RID: 2321 RVA: 0x00007D0D File Offset: 0x00005F0D
	private bool handleMouseOver()
	{
		return this.handleMouseOver(Input.mousePosition, Input.GetMouseButtonDown(0));
	}

	// Token: 0x06000912 RID: 2322 RVA: 0x00046F6C File Offset: 0x0004516C
	public bool handleMouseOver(Vector2 mousePosition, bool mousePressed)
	{
		if (this.disableTooltip)
		{
			return false;
		}
		if (this.rayCastCamera == null)
		{
			return false;
		}
		mousePressed = (mousePressed && this.doHandleClicks);
		Ray ray = this.rayCastCamera.ScreenPointToRay(mousePosition);
		RaycastHit[] array = Physics.RaycastAll(ray, 100f, 1 << base.gameObject.layer);
		string text = string.Empty;
		bool flag = false;
		try
		{
			foreach (RaycastHit hit in array)
			{
				if (this.helpOverlay != null && !flag)
				{
					if (this.handleHelpOverlayHit(hit, mousePressed))
					{
						flag = true;
					}
				}
				else if (!(base.transform != hit.collider.gameObject.transform.parent) || !(base.transform != hit.collider.gameObject.transform) || UnityUtil.hasParent(hit.collider.gameObject.transform, base.transform))
				{
					string name = hit.collider.gameObject.name;
					if (name.StartsWith("Buff_") && this.buffs.Count > 0)
					{
						string[] array3 = name.Split(new char[]
						{
							'_'
						});
						int num = Convert.ToInt32(array3[1]);
						EnchantmentInfo enchantmentInfo = this.buffs[num];
						text = enchantmentInfo.name;
						CardType cardType = CardTypeManager.getInstance().get(enchantmentInfo.name);
						if (cardType != null)
						{
							foreach (PassiveAbility passiveAbility in cardType.passiveRules)
							{
								text = text + "\n* " + passiveAbility.displayName;
							}
						}
						if (enchantmentInfo.description != string.Empty)
						{
							text = text + '\n' + enchantmentInfo.getUntaggedDescription();
						}
					}
					if (mousePressed && name == "NextPageArrow")
					{
						flag = true;
						this.pages.Next();
						if (!this.hasFetchedStats && (this.pages.Current().g == this.statsBoard || this.pages.Current().g == this.historyBoard))
						{
							App.Communicator.send(new GetCardStatsMessage(this.card.getId()));
						}
					}
					if (mousePressed && !flag && name == "FirstPageText" && this.canShowHelp())
					{
						flag = true;
						this.infoButtonClicked();
					}
					if (name == "Trigger_Ability_Button")
					{
						foreach (ActiveAbility activeAbility in this.card.getActiveAbilities())
						{
							if (activeAbility.isTriggered())
							{
								text = activeAbility.description;
								if (mousePressed && this.callBackTarget != null)
								{
									this.callBackTarget.ActivateTriggeredAbility(activeAbility.id, this.pos);
									this.callBackTarget.HideCardView();
								}
							}
						}
						return true;
					}
					if (text == string.Empty && name.StartsWith("3dPassive_"))
					{
						string text2 = name.Substring(10);
						int num2 = int.Parse(text2);
						PassiveAbility passiveAbility2 = this.card.getPassiveAbilities()[num2];
						text = passiveAbility2.description;
					}
				}
			}
		}
		catch (Exception)
		{
		}
		this.tooltip = text;
		return flag;
	}

	// Token: 0x06000913 RID: 2323 RVA: 0x00007D25 File Offset: 0x00005F25
	private bool handleHelpOverlayHit(RaycastHit hit, bool mousePressed)
	{
		if (!mousePressed)
		{
			return false;
		}
		if (hit.collider.name != "help_overlay")
		{
			return false;
		}
		this.infoButtonClicked();
		return true;
	}

	// Token: 0x06000914 RID: 2324 RVA: 0x00007D53 File Offset: 0x00005F53
	private void infoButtonClicked()
	{
		if (this.helpOverlay == null)
		{
			this.createHelpOverlay();
		}
		else
		{
			this.destroyHelpOverlay();
		}
	}

	// Token: 0x06000915 RID: 2325 RVA: 0x00007D77 File Offset: 0x00005F77
	private void destroyHelpOverlay()
	{
		if (this.helpOverlay == null)
		{
			return;
		}
		Object.Destroy(this.helpOverlay);
		this.helpOverlay = null;
	}

	// Token: 0x06000916 RID: 2326 RVA: 0x00007D9D File Offset: 0x00005F9D
	private bool canShowTooltip()
	{
		return !this.disableTooltip && this.helpOverlay == null;
	}

	// Token: 0x06000917 RID: 2327 RVA: 0x00007DB9 File Offset: 0x00005FB9
	private bool canShowHelp()
	{
		return this.card.getKeywords().Length > 0;
	}

	// Token: 0x06000918 RID: 2328 RVA: 0x00047360 File Offset: 0x00045560
	private void createHelpOverlay()
	{
		if (!this.canShowHelp())
		{
			return;
		}
		int renderQueue = this.getRenderQueue(-1);
		string filename = "Scrolls/NewNewGraphics/scroll_clean_bg_" + this.card.getRarity();
		this.helpOverlay = PrimitiveFactory.createPlane(true);
		this.helpOverlay.name = "help_overlay";
		this.helpOverlay.layer = base.gameObject.layer;
		this.helpOverlay.renderer.material = this.createMaterial();
		this.helpOverlay.renderer.material.mainTexture = ResourceManager.LoadTexture(filename);
		this.registerRenderQueueOffset(this.helpOverlay.renderer, -2);
		UnityUtil.addChild(base.gameObject, this.helpOverlay);
		UnityUtil.refreshBoxCollider(this.helpOverlay);
		GameObject gameObject = this.createTitleText(this.helpOverlay);
		gameObject.name = string.Empty;
		gameObject.GetComponent<TextMesh>().text = this.card.getName();
		gameObject.renderer.material.renderQueue = renderQueue;
		List<KeywordDescription> list = new List<KeywordDescription>(this.card.getKeywords());
		int num = 0;
		Vector3 vector;
		vector..ctor(0.095f, 0.06f, 1f);
		Vector3 scale = vector;
		for (int i = 0; i < list.Count; i++)
		{
			KeywordDescription keywordDescription = list[i];
			float num2 = 3.3f;
			float num3 = -3.246f + 0.78f * (float)i + 0.32f * (float)num;
			GameObject gameObject2 = this.createText(this.helpOverlay, CardView.FontTextBold, "keyword_" + i, 26, 2.58f, vector);
			gameObject2.GetComponent<TextMesh>().text = keywordDescription.keyword.ToUpper();
			gameObject2.transform.localPosition = new Vector3(num2, 0.01f, num3 + 0.015f);
			gameObject2.renderer.material.renderQueue = renderQueue;
			string text = CardView.wrap(keywordDescription.description);
			num += StringUtil.countLines(text);
			GameObject gameObject3 = this.createText(this.helpOverlay, CardView.FontTextBold, "description_" + i, 24, 2.58f, scale);
			gameObject3.GetComponent<TextMesh>().text = text;
			gameObject3.transform.localPosition = new Vector3(num2, 0.01f, num3 + 0.42f);
			gameObject3.renderer.material.renderQueue = renderQueue;
		}
		UnityUtil.setLayerRecursively(this.helpOverlay, this.currentLayer);
	}

	// Token: 0x06000919 RID: 2329 RVA: 0x00007DCB File Offset: 0x00005FCB
	public void fadeOut()
	{
		this.fadeModifier = -1f;
	}

	// Token: 0x0600091A RID: 2330 RVA: 0x00007DD8 File Offset: 0x00005FD8
	public void fadeIn()
	{
		this.fadeModifier = 1f;
	}

	// Token: 0x0600091B RID: 2331 RVA: 0x000475F8 File Offset: 0x000457F8
	public void setTransparency(float alpha)
	{
		if (this.alpha == alpha)
		{
			return;
		}
		this.colorChanger.SetColor(ColorUtil.GetWithAlpha(this.baseColor, alpha));
		if (alpha < 0.01f && this.alpha >= 0.01f)
		{
			this.enableFoilRendering(false);
		}
		if (alpha > 0.01f && this.alpha <= 0.01f)
		{
			this.enableFoilRendering(true);
		}
		this.alpha = alpha;
	}

	// Token: 0x0600091C RID: 2332 RVA: 0x00047674 File Offset: 0x00045874
	public void setDiffuseColor(Color color)
	{
		color.a = this.colorChanger.GetColor().a;
		this.colorChanger.SetColor(color);
	}

	// Token: 0x0600091D RID: 2333 RVA: 0x00007DE5 File Offset: 0x00005FE5
	public void setColor(Color color)
	{
		this.colorChanger.SetColor(color);
	}

	// Token: 0x0600091E RID: 2334 RVA: 0x00007DF3 File Offset: 0x00005FF3
	public void setColor(Color color, float fadeTime)
	{
		this.isFading = true;
		this.fadeSrcColor = this.colorChanger.GetColor();
		this.fadeDstColor = color;
		this.fadeStartTime = Time.time;
		this.fadeDuration = fadeTime;
	}

	// Token: 0x0600091F RID: 2335 RVA: 0x000476A8 File Offset: 0x000458A8
	public void setRenderQueue(int renderQueue)
	{
		if (!this.useRendQ)
		{
			return;
		}
		this.rendQ = renderQueue;
		foreach (KeyValuePair<Renderer, int> keyValuePair in this.renderQueueOffsets)
		{
			if (keyValuePair.Key.material != null)
			{
				keyValuePair.Key.material.renderQueue = this.getRenderQueue(keyValuePair.Value);
				keyValuePair.Key.material = keyValuePair.Key.material;
			}
		}
	}

	// Token: 0x06000920 RID: 2336 RVA: 0x0004775C File Offset: 0x0004595C
	private void setRenderQueueIfNotSet(Renderer renderer, int offset)
	{
		if (renderer.material == null)
		{
			return;
		}
		int renderQueue = this.getRenderQueue(offset);
		if (renderer.material.renderQueue != renderQueue)
		{
			renderer.material.renderQueue = renderQueue;
			renderer.material = renderer.material;
		}
	}

	// Token: 0x06000921 RID: 2337 RVA: 0x00007E26 File Offset: 0x00006026
	private void registerRenderQueueOffset(Renderer renderer, int offset)
	{
		this.renderQueueOffsets.Add(renderer, offset);
		this.setRenderQueueIfNotSet(renderer, offset);
	}

	// Token: 0x06000922 RID: 2338 RVA: 0x00007E3D File Offset: 0x0000603D
	private int getRenderQueue(int offset)
	{
		return 92000 + this.rendQ * 10 + offset;
	}

	// Token: 0x06000923 RID: 2339 RVA: 0x00007E50 File Offset: 0x00006050
	public void setLayer(int newLayer)
	{
		this.currentLayer = newLayer;
		this.resetLayerWithAllChildren();
	}

	// Token: 0x06000924 RID: 2340 RVA: 0x000477AC File Offset: 0x000459AC
	public void resetLayerWithAllChildren()
	{
		UnityUtil.setLayerRecursively(base.gameObject, this.currentLayer);
		foreach (EffectPlayer effectPlayer in base.GetComponents<EffectPlayer>())
		{
			effectPlayer.layer = this.currentLayer;
		}
	}

	// Token: 0x06000925 RID: 2341 RVA: 0x000477F8 File Offset: 0x000459F8
	private void createLevelGraphics()
	{
		this.foilPlate = PrimitiveFactory.createPlane(false);
		this.foilPlate.name = "foilPlate";
		UnityUtil.addChild(base.gameObject, this.foilPlate);
		this.foilPlate.transform.localPosition = new Vector3(0f, 0.005f, 0f);
		this.foilPlate.renderer.material = this.createMaterial();
		Texture2D mainTexture = (!this.card.isAtleastTier(3)) ? ResourceManager.LoadTexture("Scrolls/Foil/crafting_3_3") : ResourceManager.LoadTexture("Scrolls/Foil/crafting_3_2");
		this.foilPlate.renderer.material.mainTexture = mainTexture;
		this.foilPlate.renderer.enabled = false;
		this.renderQueueOffsets.Add(this.foilPlate.renderer, -9);
	}

	// Token: 0x06000926 RID: 2342 RVA: 0x00007E5F File Offset: 0x0000605F
	public void setShader(string shaderName)
	{
		if (this.isInited)
		{
			throw new InvalidOperationException("Can't set shader after initializing");
		}
		this.shader = shaderName;
	}

	// Token: 0x06000927 RID: 2343 RVA: 0x00007E7E File Offset: 0x0000607E
	private Material createMaterial()
	{
		return CardView.createMaterial(this.shader);
	}

	// Token: 0x06000928 RID: 2344 RVA: 0x00007E8B File Offset: 0x0000608B
	private static Material createMaterial(string shaderName)
	{
		return new Material(ResourceManager.LoadShader(shaderName));
	}

	// Token: 0x06000929 RID: 2345 RVA: 0x00007E98 File Offset: 0x00006098
	public void setRaycastCamera(Camera raycastCamera)
	{
		this.rayCastCamera = raycastCamera;
	}

	// Token: 0x0600092A RID: 2346 RVA: 0x000478D8 File Offset: 0x00045AD8
	private void runFoilEffect()
	{
		if (this.foilRainbow != null)
		{
			return;
		}
		this.foilRainbow = new GameObject("Foil-Card-Rainbow");
		UnityUtil.addChild(base.gameObject, this.foilRainbow);
		this.foilRainbow.transform.localScale = base.gameObject.transform.localScale;
		this.foilRainbow.transform.localPosition = new Vector3(-0.07f, 0.015f, -0.44f);
		this.foilRainbow.transform.localEulerAngles = new Vector3(90f, 180f, 0f);
		string name = "scroll_foil_bling";
		this.foilAnim = this.setupFoilAnim(this.foilRainbow.transform, name, "FoilShine/foil_rainbow", "foil_common");
	}

	// Token: 0x0600092B RID: 2347 RVA: 0x000479AC File Offset: 0x00045BAC
	private void runFoilTwinkle()
	{
		if (this.foilTwinkle != null)
		{
			return;
		}
		if (this.alpha < 0.01f)
		{
			return;
		}
		this.foilTwinkle = new GameObject("Foil-Card-Twinkle");
		UnityUtil.addChild(base.gameObject, this.foilTwinkle);
		this.foilTwinkle.transform.localScale = base.gameObject.transform.localScale;
		this.foilTwinkle.transform.localPosition = new Vector3(-0.07f, 0.02f, -0.44f);
		this.foilTwinkle.transform.localEulerAngles = new Vector3(90f, 180f, 0f);
		string name = "scroll_foil_twinkle";
		this.setupFoilAnim(this.foilTwinkle.transform, name, "FoilShine/foil_edge_twinkle", "edge_bling");
	}

	// Token: 0x0600092C RID: 2348 RVA: 0x00007EA1 File Offset: 0x000060A1
	public void setGrayedOutCost(bool grayedOut)
	{
		this.costGrayout = grayedOut;
		this.internalCostGrayout();
	}

	// Token: 0x0600092D RID: 2349 RVA: 0x00047A8C File Offset: 0x00045C8C
	private void internalCostGrayout()
	{
		Color color = (!this.cardGrayedOut) ? Color.white : new Color(0.8f, 0.8f, 0.8f);
		if (this.costGrayout || this.cardGrayedOut)
		{
			foreach (GameObject gameObject in this.gosNumCost)
			{
				gameObject.renderer.material.color = new Color(0.65f, 0.55f, 0.55f) * color;
			}
			this.ico.renderer.material.color = new Color(0.5f, 0.4f, 0.4f) * color;
		}
		else
		{
			foreach (GameObject gameObject2 in this.gosNumCost)
			{
				gameObject2.renderer.material.color = Color.white * color;
			}
			this.ico.renderer.material.color = Color.white * color;
		}
	}

	// Token: 0x0600092E RID: 2350 RVA: 0x00047BFC File Offset: 0x00045DFC
	public void setGrayedOut(bool grayedOut)
	{
		this.onDeselect();
		this.cardGrayedOut = grayedOut;
		if (grayedOut)
		{
			this.colorChanger.SetColor(new Color(0.7f, 0.7f, 0.7f));
		}
		else
		{
			this.colorChanger.SetColor(Color.white);
		}
		this.internalCostGrayout();
	}

	// Token: 0x0600092F RID: 2351 RVA: 0x00047C58 File Offset: 0x00045E58
	private void createText_Buffs()
	{
		this.destroyAndClear(this.gosBuffs);
		float num = 1.25f;
		if (this.buffs.Count > 1)
		{
			num = Mathf.Min(num, 6.25f / (float)(this.buffs.Count - 1));
		}
		for (int i = 0; i < this.buffs.Count; i++)
		{
			EnchantmentInfo enchantmentInfo = this.buffs[i];
			float num2 = -1f + (float)i * 0.01f;
			GameObject gameObject = PrimitiveFactory.createPlane(true);
			gameObject.name = "Buff_" + i;
			Material material = new Material(ResourceManager.LoadShader(this.shader));
			material.mainTexture = ResourceManager.LoadTexture("Scrolls/NewGraphics/buffdebuff_" + enchantmentInfo.type.ToString().ToLower());
			this.renderQueueOffsets.Add(gameObject.renderer, -10);
			gameObject.renderer.material = material;
			UnityUtil.addChild(base.gameObject, gameObject);
			gameObject.transform.localPosition = new Vector3(5.86f, num2 - 0.01f, -3.3f + (float)i * num);
			gameObject.transform.localScale = new Vector3(0.316f, 1f, 0.118f);
			this.gosBuffs.Add(gameObject);
			GameObject gameObject2 = new GameObject("TextMesh");
			this.textsArr.Add(gameObject2);
			MeshRenderer meshRenderer = gameObject2.AddComponent<MeshRenderer>();
			TextMesh textMesh = gameObject2.AddComponent<TextMesh>();
			textMesh.lineSpacing = 0.85f;
			textMesh.fontSize = 26;
			textMesh.characterSize = 2.58f;
			gameObject2.name = "Buff_Text_" + i;
			textMesh.text = enchantmentInfo.name;
			Font fontTextBold = CardView.FontTextBold;
			textMesh.font = fontTextBold;
			meshRenderer.material = fontTextBold.material;
			meshRenderer.material.shader = ResourceManager.LoadShader(CardView.fontShader);
			this.renderQueueOffsets.Add(meshRenderer, -10);
			UnityUtil.addChild(base.gameObject, gameObject2);
			gameObject2.transform.localScale = new Vector3(0.095f, 0.06f, 1f);
			gameObject2.transform.localEulerAngles = new Vector3(90f, 90f, 270f);
			gameObject2.transform.localPosition = new Vector3(6.99f, num2 - 0.005f, -3.63f + (float)i * num);
			gameObject2.renderer.material.color = CardView.cardTextFontColor;
			this.gosBuffs.Add(gameObject2);
			GameObject gameObject3 = new GameObject("TextMesh");
			this.textsArr.Add(gameObject3);
			MeshRenderer meshRenderer2 = gameObject3.AddComponent<MeshRenderer>();
			TextMesh textMesh2 = gameObject3.AddComponent<TextMesh>();
			textMesh2.lineSpacing = 0.85f;
			textMesh2.fontSize = 26;
			textMesh2.characterSize = 1.95f;
			gameObject3.name = "Buff_Type_" + i;
			textMesh2.text = enchantmentInfo.type.getName();
			Font fontTextBold2 = CardView.FontTextBold;
			textMesh2.font = fontTextBold2;
			meshRenderer2.material = fontTextBold2.material;
			meshRenderer2.material.shader = ResourceManager.LoadShader(CardView.fontShader);
			this.renderQueueOffsets.Add(meshRenderer2, -10);
			UnityUtil.addChild(base.gameObject, gameObject3);
			gameObject3.transform.localScale = new Vector3(0.095f, 0.06f, 1f);
			gameObject3.transform.localEulerAngles = new Vector3(90f, 90f, 270f);
			gameObject3.transform.localPosition = new Vector3(6.81f, num2 - 0.005f, -3.28f + (float)i * num);
			gameObject3.renderer.material.color = CardView.cardTextFontColor;
			this.gosBuffs.Add(gameObject3);
		}
	}

	// Token: 0x06000930 RID: 2352 RVA: 0x0004805C File Offset: 0x0004625C
	private void createText_PassiveAbilities()
	{
		foreach (GameObject go in this.gosPassiveAbilities)
		{
			this.colorChanger.Remove(go);
		}
		this.destroyAndClear(this.gosPassiveAbilities);
		for (int i = 0; i < this.card.getPassiveAbilities().Length; i++)
		{
			GameObject gameObject = new GameObject("TextMesh");
			this.textsArr.Add(gameObject);
			MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
			TextMesh textMesh = gameObject.AddComponent<TextMesh>();
			textMesh.lineSpacing = 0.85f;
			textMesh.fontSize = 24;
			textMesh.characterSize = 2.58f;
			textMesh.richText = true;
			gameObject.name = "3dPassive_" + i.ToString();
			textMesh.text = "* " + this.card.getPassiveAbilities()[i].displayName;
			Font fontTextBoldItalic = CardView.FontTextBoldItalic;
			textMesh.font = fontTextBoldItalic;
			meshRenderer.material = fontTextBoldItalic.material;
			meshRenderer.material.shader = ResourceManager.LoadShader(CardView.fontShaderColored);
			this.renderQueueOffsets.Add(meshRenderer, -7);
			UnityUtil.addChild(this.firstPageText, gameObject);
			gameObject.transform.localScale = new Vector3(0.095f, 0.06f, 1f);
			gameObject.transform.localEulerAngles = new Vector3(90f, 90f, 270f);
			gameObject.transform.localPosition = new Vector3(3.3f, 0.01f, 1f + 0.22f * (float)i);
			gameObject.renderer.material.color = CardView.fontColor;
			this.gosPassiveAbilities.Add(gameObject);
		}
		foreach (GameObject go2 in this.gosPassiveAbilities)
		{
			this.colorChanger.Add(go2);
		}
	}

	// Token: 0x06000931 RID: 2353 RVA: 0x00048298 File Offset: 0x00046498
	private void createText_ActiveAbilities()
	{
		this.destroyAndClear(this.gosActiveAbilities);
		foreach (ActiveAbility activeAbility in this.card.getActiveAbilities())
		{
			if (activeAbility.isTriggered())
			{
				GameObject gameObject = PrimitiveFactory.createPlane(true);
				gameObject.collider.isTrigger = true;
				gameObject.name = "Trigger_Ability_Button";
				UnityUtil.addChild(base.gameObject, gameObject);
				gameObject.transform.localScale = new Vector3(0.566f, 1f, 0.105f);
				gameObject.transform.localPosition = new Vector3(-0.07f, 0.001f, 3.92f);
				Material material = new Material(ResourceManager.LoadShader(this.shader));
				this.renderQueueOffsets.Add(gameObject.renderer, -7);
				material.mainTexture = ResourceManager.LoadTexture("Scrolls/NewGraphics/scrolls__activability");
				gameObject.renderer.material = material;
				GameObject gameObject2 = new GameObject("TextMesh");
				this.textsArr.Add(gameObject2);
				MeshRenderer meshRenderer = gameObject2.AddComponent<MeshRenderer>();
				TextMesh textMesh = gameObject2.AddComponent<TextMesh>();
				textMesh.lineSpacing = 0.85f;
				textMesh.fontSize = 24;
				textMesh.characterSize = 2.58f;
				gameObject2.name = "3D Text - triggered abil";
				textMesh.text = activeAbility.name;
				textMesh.anchor = 1;
				textMesh.alignment = 1;
				Font fontTitle = CardView.FontTitle;
				textMesh.font = fontTitle;
				fontTitle.material.color = Color.white;
				meshRenderer.material = fontTitle.material;
				meshRenderer.material.shader = ResourceManager.LoadShader(CardView.fontShader);
				this.renderQueueOffsets.Add(meshRenderer, -7);
				UnityUtil.addChild(base.gameObject, gameObject2);
				gameObject2.transform.localScale = new Vector3(0.095f, 0.06f, 1f);
				gameObject2.transform.localEulerAngles = new Vector3(90f, 90f, 270f);
				gameObject2.transform.localPosition = new Vector3(0f, 0.01f, 3.72f);
				gameObject2.renderer.material.color = new Color(1f, 1f, 1f);
				this.gosActiveAbilities.Add(gameObject2);
				this.gosActiveAbilities.Add(gameObject);
			}
		}
	}

	// Token: 0x06000932 RID: 2354 RVA: 0x00048508 File Offset: 0x00046708
	private GameObject createText(GameObject parent, Font font, string name, int fontSize, float characterSize, Vector3 scale)
	{
		GameObject gameObject = new GameObject("TextMesh");
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		TextMesh textMesh = gameObject.AddComponent<TextMesh>();
		textMesh.font = font;
		textMesh.fontSize = fontSize;
		textMesh.characterSize = characterSize;
		textMesh.lineSpacing = 0.85f;
		gameObject.name = name;
		font.material.color = Color.black;
		meshRenderer.material = font.material;
		meshRenderer.material.shader = ResourceManager.LoadShader(CardView.fontShader);
		this.renderQueueOffsets.Add(meshRenderer, -4);
		UnityUtil.addChild(parent, gameObject);
		gameObject.transform.localScale = scale;
		gameObject.transform.localEulerAngles = new Vector3(90f, 90f, 270f);
		gameObject.renderer.material.color = CardView.cardTextFontColor;
		return gameObject;
	}

	// Token: 0x06000933 RID: 2355 RVA: 0x000485E0 File Offset: 0x000467E0
	private GameObject createTitleText(GameObject parent)
	{
		GameObject gameObject = this.createText(parent, CardView.FontTitle, "3DText_title", 42, 0.74f, new Vector3(0.3f, 0.2f, 1f));
		gameObject.GetComponent<TextMesh>().anchor = 1;
		gameObject.transform.localPosition = new Vector3(0f, 0.01f, -4.246f);
		return gameObject;
	}

	// Token: 0x06000934 RID: 2356 RVA: 0x00048648 File Offset: 0x00046848
	private void createTexts()
	{
		if (!this.card.isValid())
		{
			return;
		}
		this.textsArr.Add(this.createTitleText(base.gameObject));
		if (!this.showTexts)
		{
			return;
		}
		GameObject gameObject = new GameObject("TextMesh");
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		TextMesh textMesh = gameObject.AddComponent<TextMesh>();
		textMesh.lineSpacing = 0.85f;
		textMesh.fontSize = 24;
		textMesh.characterSize = 2.58f;
		gameObject.name = "3DText_tier";
		Font fontTextBold = CardView.FontTextBold;
		textMesh.font = fontTextBold;
		fontTextBold.material.color = Color.black;
		meshRenderer.material = fontTextBold.material;
		meshRenderer.material.shader = ResourceManager.LoadShader(CardView.fontShader);
		gameObject.renderer.material.color = CardView.cardTextFontColor;
		this.renderQueueOffsets.Add(meshRenderer, -4);
		this.textsArr.Add(gameObject);
		UnityUtil.addChild(base.gameObject, gameObject);
		gameObject.transform.localScale = new Vector3(0.095f, 0.06f, 1f);
		gameObject.transform.localEulerAngles = new Vector3(90f, 90f, 270f);
		gameObject.transform.localPosition = new Vector3(-3.1f, 0.01f, -4.2f);
		GameObject gameObject2 = new GameObject("TextMesh");
		this.textsArr.Add(gameObject2);
		MeshRenderer meshRenderer2 = gameObject2.AddComponent<MeshRenderer>();
		TextMesh textMesh2 = gameObject2.AddComponent<TextMesh>();
		textMesh2.lineSpacing = 0.85f;
		textMesh2.fontSize = 24;
		textMesh2.characterSize = 2.58f;
		gameObject2.name = "3DText_desc";
		Font fontTextBold2 = CardView.FontTextBold;
		textMesh2.font = fontTextBold2;
		fontTextBold2.material.color = Color.black;
		meshRenderer2.material = fontTextBold2.material;
		meshRenderer2.material.shader = ResourceManager.LoadShader(CardView.fontShaderColored);
		meshRenderer2.material = meshRenderer2.material;
		this.renderQueueOffsets.Add(meshRenderer2, -4);
		UnityUtil.addChild(this.firstPageText, gameObject2);
		gameObject2.transform.localScale = new Vector3(0.095f, 0.06f, 1f);
		gameObject2.transform.localEulerAngles = new Vector3(90f, 90f, 270f);
		gameObject2.renderer.material.color = CardView.cardTextFontColor;
		GameObject gameObject3 = new GameObject("TextMesh");
		this.textsArr.Add(gameObject3);
		MeshRenderer meshRenderer3 = gameObject3.AddComponent<MeshRenderer>();
		TextMesh textMesh3 = gameObject3.AddComponent<TextMesh>();
		gameObject3.renderer.material = new Material(ResourceManager.LoadShader(this.shader));
		textMesh3.lineSpacing = 0.85f;
		textMesh3.fontSize = 24;
		textMesh3.characterSize = 2.58f;
		gameObject3.name = "3DText_flavor";
		textMesh3.anchor = 1;
		textMesh3.alignment = 1;
		Font fontTextBoldItalic = CardView.FontTextBoldItalic;
		textMesh3.font = fontTextBoldItalic;
		fontTextBoldItalic.material.color = Color.black;
		meshRenderer3.material = fontTextBoldItalic.material;
		meshRenderer3.material.shader = ResourceManager.LoadShader(CardView.fontShader);
		this.renderQueueOffsets.Add(meshRenderer3, -4);
		UnityUtil.addChild(this.firstPageText, gameObject3);
		gameObject3.transform.localScale = new Vector3(0.095f, 0.06f, 1f);
		gameObject3.transform.localEulerAngles = new Vector3(90f, 90f, 270f);
		gameObject3.renderer.material.color = CardView.cardTextFontColor;
		this.createText_ActiveAbilities();
		GameObject gameObject4 = new GameObject("TextMesh");
		this.textsArr.Add(gameObject4);
		MeshRenderer meshRenderer4 = gameObject4.AddComponent<MeshRenderer>();
		TextMesh textMesh4 = gameObject4.AddComponent<TextMesh>();
		textMesh4.anchor = 1;
		textMesh4.alignment = 1;
		textMesh4.lineSpacing = 0.85f;
		textMesh4.fontSize = 28;
		textMesh4.characterSize = 0.74f;
		gameObject4.name = "3DText_pieceType";
		Font fontTitle = CardView.FontTitle;
		textMesh4.font = fontTitle;
		fontTitle.material.color = Color.black;
		meshRenderer4.material = fontTitle.material;
		meshRenderer4.material.shader = ResourceManager.LoadShader(CardView.fontShader);
		this.renderQueueOffsets.Add(meshRenderer4, -4);
		UnityUtil.addChild(base.gameObject, gameObject4);
		gameObject4.transform.localScale = new Vector3(0.3f, 0.2f, 1f);
		gameObject4.transform.localEulerAngles = new Vector3(90f, 90f, 270f);
		gameObject4.transform.localPosition = new Vector3(0f, 0.01f, -3.67f);
		gameObject4.renderer.material.color = CardView.cardTextFontColor;
	}

	// Token: 0x06000935 RID: 2357 RVA: 0x00048B44 File Offset: 0x00046D44
	private static void fontMaterialsTest(MeshRenderer renderer, Font font)
	{
		Material[] array = new Material[]
		{
			new Material(font.material),
			new Material(font.material)
		};
		array[1].shader = Shader.Find("Scrolls/Unlit/Color");
		array[0].shader = ResourceManager.LoadShader(CardView.fontShaderColored);
		foreach (Material material in array)
		{
			material.mainTexture = font.material.mainTexture;
		}
		renderer.materials = array;
	}

	// Token: 0x06000936 RID: 2358 RVA: 0x00007EB0 File Offset: 0x000060B0
	private string resources()
	{
		return this.card.getCardImage();
	}

	// Token: 0x06000937 RID: 2359 RVA: 0x00048BCC File Offset: 0x00046DCC
	private void destroyAndClear(List<GameObject> gameObjects)
	{
		foreach (GameObject gameObject in gameObjects)
		{
			this.renderQueueOffsets.Remove(gameObject.renderer);
			Object.Destroy(gameObject);
		}
		gameObjects.Clear();
	}

	// Token: 0x06000938 RID: 2360 RVA: 0x00048C38 File Offset: 0x00046E38
	private static void updateDigits(List<GameObject> gameObjects, char[] withNums)
	{
		for (int i = 0; i < gameObjects.Count; i++)
		{
			Texture2D texture2D = ResourceManager.LoadTexture("Scrolls/NewGraphics/Numbers/scroll_numbers__" + withNums[i]);
			float num = (float)(texture2D.width / texture2D.height) * 0.05f;
			gameObjects[i].renderer.material.mainTexture = texture2D;
			gameObjects[i].transform.localScale = new Vector3(num * 1.5686f, 1f, 0.044625778f);
		}
	}

	// Token: 0x06000939 RID: 2361 RVA: 0x00048CC8 File Offset: 0x00046EC8
	private static Vector3 getCostScale(int len)
	{
		float num = 1f - 0.2f * (float)len;
		return new Vector3(num * 0.1f, 1f, num * 0.08f);
	}

	// Token: 0x0600093A RID: 2362 RVA: 0x00007EBD File Offset: 0x000060BD
	private static Vector3 getCostPos(int len, int i)
	{
		if (len == 1)
		{
			return new Vector3(-0.55f, 0.06f, -4.78f);
		}
		return new Vector3(-0.3f - 0.55f * (float)i, 0.03f, -4.78f);
	}

	// Token: 0x0600093B RID: 2363 RVA: 0x00048CFC File Offset: 0x00046EFC
	private void createGraphics_Cost()
	{
		int num = Mathf.Min(this.getCostOrOverridden(), 99);
		char[] array = Convert.ToString(num).ToCharArray();
		int len = array.Length;
		if (array.Length != this.gosNumCost.Count)
		{
			this.destroyAndClear(this.gosNumCost);
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = PrimitiveFactory.createPlane(false);
				gameObject.name = "Resource_Cost_Number";
				gameObject.tag = "blinkable_cost";
				UnityUtil.addChild(base.gameObject, gameObject);
				gameObject.transform.localPosition = CardView.getCostPos(len, i);
				gameObject.transform.localScale = CardView.getCostScale(len);
				this.renderQueueOffsets.Add(gameObject.renderer, -5);
				gameObject.renderer.material = new Material(ResourceManager.LoadShader(this.shader));
				this.numbersArr.Add(gameObject);
				this.gosNumCost.Add(gameObject);
			}
		}
		for (int j = 0; j < this.gosNumCost.Count; j++)
		{
			this.gosNumCost[j].renderer.material.mainTexture = ResourceManager.LoadTexture("Scrolls/yellow_" + array[j]);
		}
		this.lastCost = num;
	}

	// Token: 0x0600093C RID: 2364 RVA: 0x00048E54 File Offset: 0x00047054
	private void createGraphics_AttackPower()
	{
		char[] array = Convert.ToString(this.card.getAttackPower()).ToCharArray();
		if (array.Length != this.gosNumAttackPower.Count)
		{
			this.destroyAndClear(this.gosNumAttackPower);
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = PrimitiveFactory.createPlane(false);
				gameObject.name = "Attack_Number";
				gameObject.tag = "blinkable_attack";
				UnityUtil.addChild(base.gameObject, gameObject);
				gameObject.transform.localPosition = new Vector3(1.88f - (float)(array.Length - 1) * 0.03f - (float)i * 0.35f, 0.01f, 0.4f);
				this.renderQueueOffsets.Add(gameObject.renderer, -4);
				gameObject.renderer.material = new Material(ResourceManager.LoadShader(this.shader));
				this.numbersArr.Add(gameObject);
				this.gosNumAttackPower.Add(gameObject);
			}
		}
		CardView.updateDigits(this.gosNumAttackPower, array);
		foreach (GameObject gameObject2 in this.gosNumAttackPower)
		{
			gameObject2.renderer.enabled = this.isUnit();
		}
	}

	// Token: 0x0600093D RID: 2365 RVA: 0x00048FB4 File Offset: 0x000471B4
	private void createGraphics_Countdown()
	{
		char[] array = (this.card.getAttackInterval() < 0) ? "-".ToCharArray() : Convert.ToString(this.card.getAttackInterval()).ToCharArray();
		if (array.Length != this.gosNumCountdown.Count)
		{
			this.destroyAndClear(this.gosNumCountdown);
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = PrimitiveFactory.createPlane(false);
				gameObject.name = "CountDown_Number";
				gameObject.tag = "blinkable_countdown";
				UnityUtil.addChild(base.gameObject, gameObject);
				gameObject.transform.localPosition = new Vector3(-0.294f - (float)(array.Length - 1) * 0.03f - (float)i * 0.35f, 0.01f, 0.4f);
				this.renderQueueOffsets.Add(gameObject.renderer, -4);
				gameObject.renderer.material = new Material(ResourceManager.LoadShader(this.shader));
				this.numbersArr.Add(gameObject);
				this.gosNumCountdown.Add(gameObject);
			}
		}
		CardView.updateDigits(this.gosNumCountdown, array);
		foreach (GameObject gameObject2 in this.gosNumCountdown)
		{
			gameObject2.renderer.enabled = this.isUnit();
		}
	}

	// Token: 0x0600093E RID: 2366 RVA: 0x00049138 File Offset: 0x00047338
	private void createGraphics_HitPoints()
	{
		char[] array = this.card.getHitPoints().ToString().ToCharArray();
		if (array.Length != this.gosNumHitPoints.Count)
		{
			this.destroyAndClear(this.gosNumHitPoints);
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = PrimitiveFactory.createPlane(false);
				gameObject.name = "Hitpoints_Number";
				gameObject.tag = "blinkable_health";
				UnityUtil.addChild(base.gameObject, gameObject);
				gameObject.transform.localPosition = new Vector3(-2.58f - (float)(array.Length - 1) * 0.03f - (float)i * 0.35f, 0.01f, 0.4f);
				this.renderQueueOffsets.Add(gameObject.renderer, -4);
				gameObject.renderer.material = new Material(ResourceManager.LoadShader(this.shader));
				this.numbersArr.Add(gameObject);
				this.gosNumHitPoints.Add(gameObject);
			}
		}
		CardView.updateDigits(this.gosNumHitPoints, array);
		foreach (GameObject gameObject2 in this.gosNumHitPoints)
		{
			gameObject2.renderer.enabled = this.isUnit();
		}
	}

	// Token: 0x0600093F RID: 2367 RVA: 0x00007EF8 File Offset: 0x000060F8
	private bool isUnit()
	{
		return this.getCardType().kind.isUnit();
	}

	// Token: 0x06000940 RID: 2368 RVA: 0x0004929C File Offset: 0x0004749C
	protected override void onUpdateGraphics()
	{
		this.updateShowHelpCollider();
		this.statsBG.renderer.enabled = this.isUnit();
		this.createGraphics_Cost();
		this.createGraphics_AttackPower();
		this.createGraphics_Countdown();
		this.createGraphics_HitPoints();
		this.createText_ActiveAbilities();
		if (this.showTexts)
		{
			this.createText_PassiveAbilities();
			this.createText_Buffs();
		}
		if (this.foilRainbow != null)
		{
			Object.Destroy(this.foilRainbow);
			this.foilRainbow = null;
		}
		this.runFoilRainbow = true;
		this.enableFoilRendering(base.renderer.enabled);
		int num = 0;
		if (this.showTexts)
		{
			foreach (GameObject gameObject in this.gosPassiveAbilities)
			{
				TextMesh component = gameObject.GetComponent<TextMesh>();
				string s2 = CardView.wrap(component.text);
				component.text = this.colorTagText(component.text);
				BoxCollider component2 = gameObject.GetComponent<BoxCollider>();
				if (component2 != null)
				{
					Object.DestroyImmediate(component2);
				}
				BoxCollider boxCollider = gameObject.AddComponent<BoxCollider>();
				boxCollider.center += new Vector3(0f, -2f, 0f);
				component.transform.localPosition = new Vector3(3.3f, 0.01f, 1f + 0.32f * (float)num);
				num += StringUtil.countLines(s2);
			}
		}
		foreach (TextMesh textMesh in base.GetComponentsInChildren<TextMesh>())
		{
			if (textMesh.name == "3DText_pieceType")
			{
				string text = this.card.getPieceKindText();
				if (this.card.getPieceType().Length > 0)
				{
					text = text + ": " + this.card.getPieceType();
				}
				textMesh.text = text;
			}
			if (textMesh.name == "3DText_desc")
			{
				string text2 = this.colorTagText(this.card.getDescription());
				string[] array = text2.Split(new char[]
				{
					'\n'
				});
				textMesh.text = string.Join("\n", Enumerable.ToArray<string>(Enumerable.Select<string, string>(array, (string s) => CardView.wrap(s)))) + "\n";
				float num2 = 0.32f * (float)num;
				if (num2 > 0f)
				{
					num2 += 0.13f;
				}
				textMesh.transform.localPosition = new Vector3(3.3f, 0.01f, 1f + num2);
			}
			if (textMesh.name == "3DText_flavor")
			{
				string[] array2 = this.card.getDescriptionFlavor().Split(new char[]
				{
					'\n'
				});
				textMesh.text = string.Join("\n", Enumerable.ToArray<string>(Enumerable.Select<string, string>(array2, (string s) => CardView.wrap(s)))) + "\n";
				textMesh.transform.localPosition = new Vector3(0f, 0.01f, ((!this.card.hasTriggeredAbility()) ? 3.25f : 2.75f) - (float)textMesh.text.Length / 200f);
			}
			if (textMesh.name == "3DText_title")
			{
				textMesh.text = this.card.getName();
				textMesh.transform.localPosition = new Vector3(0f, 0.01f, -4.246f);
			}
			if (textMesh.name == "3DText_tier")
			{
				if (this.hasVisuals() || this.card.isTier(1))
				{
					textMesh.text = string.Empty;
				}
				else
				{
					textMesh.text = "T" + this.card.getTier();
				}
			}
		}
		if (this.type == CardView.Type.Normal)
		{
			int rarity = this.card.getRarity();
			string text3 = this.card.getCardType().getResource().ToString().ToLower();
			Texture2D mainTexture = ResourceManager.LoadTexture(string.Concat(new object[]
			{
				"Scrolls/NewNewGraphics/scrolls__scrollbase_",
				text3,
				"_",
				rarity
			}));
			base.renderer.material.mainTexture = mainTexture;
		}
		if (this.type == CardView.Type.Small)
		{
			base.renderer.material.mainTexture = ResourceManager.LoadTexture("Scrolls/NewNewGraphics/small");
		}
		string filename = this.card.getResourceType().battleIconFilename();
		this.ico.renderer.material.mainTexture = ResourceManager.LoadTexture(filename);
		this.cardImage.renderer.material.mainTexture = App.AssetLoader.LoadCardImage(this.resources());
		this.setLayer(base.gameObject.layer);
		this.setTransparency(this.alpha);
		this.textsArr.RemoveAll((GameObject g) => g == null);
	}

	// Token: 0x06000941 RID: 2369 RVA: 0x00049820 File Offset: 0x00047A20
	private string tagsToColors2(string s)
	{
		return "<material=0>" + s.Replace("<", "<material=1&gt;<color=" + CardView.ColorCardReference + "&gt;").Replace(">", "</color></material>").Replace("[", "<material=1><color=" + CardView.ColorWord + ">").Replace("]", "</color></material>").Replace("&gt;", ">") + "</material>";
	}

	// Token: 0x06000942 RID: 2370 RVA: 0x000498A8 File Offset: 0x00047AA8
	private string tagsToColors(string s)
	{
		return s.Replace("<", "<b%tmp%<color=" + CardView.ColorWord + "%tmp%").Replace(">", "</color></b>").Replace("[", "<color=" + CardView.ColorCardReference + ">").Replace("]", "</color>").Replace("%tmp%", ">");
	}

	// Token: 0x06000943 RID: 2371 RVA: 0x00007F0A File Offset: 0x0000610A
	private string colorTagText(string s)
	{
		return GUIUtil.RtColor(CardView.wrap(this.tagsToColors(s)), CardView.fontColor);
	}

	// Token: 0x06000944 RID: 2372 RVA: 0x00007F22 File Offset: 0x00006122
	private string colorTagText2(string s)
	{
		return GUIUtil.RtColor(CardView.wrap(this.tagsToColors2(s)), CardView.fontColor);
	}

	// Token: 0x06000945 RID: 2373 RVA: 0x00007F3A File Offset: 0x0000613A
	private static string wrap(string s)
	{
		return StringUtil.wordWrappedIgnoreTags(s, 34);
	}

	// Token: 0x06000946 RID: 2374 RVA: 0x00007F44 File Offset: 0x00006144
	public void applyHighResTexture()
	{
		if (this.hasSetHighResTexture)
		{
			return;
		}
		this.highResFetcher.fetch(this.resources());
	}

	// Token: 0x06000947 RID: 2375 RVA: 0x00049920 File Offset: 0x00047B20
	private void onFetchedCardImage(Texture2D image)
	{
		this.cardImage.renderer.material.color = Color.white;
		this.cardImage.renderer.material.mainTexture = image;
		this.cardImage.renderer.enabled = true;
		this.hasSetHighResTexture = true;
	}

	// Token: 0x06000948 RID: 2376 RVA: 0x00007F63 File Offset: 0x00006163
	private void setStatistics(CardStat cardStat, CardHistory[] history)
	{
		if (this.hasFetchedStats)
		{
			return;
		}
		this.hasFetchedStats = true;
		this.createStatisticsTexts(cardStat);
		this.createHistoryTexts(history);
		this.setRenderQueue(this.rendQ);
		this.resetLayerWithAllChildren();
	}

	// Token: 0x06000949 RID: 2377 RVA: 0x00049978 File Offset: 0x00047B78
	private void createStatisticsTexts(CardStat cardStat)
	{
		if (this.statsBoard == null)
		{
			return;
		}
		string text = string.Empty;
		string text2 = string.Empty;
		string text3 = string.Empty;
		string text4 = string.Empty;
		string text5 = string.Empty;
		if (cardStat != null)
		{
			if (cardStat.created != null)
			{
				long? timestamp = cardStat.timestamp;
				if (timestamp != null)
				{
					DateTime dateTime;
					dateTime..ctor(1970, 1, 1);
					dateTime = dateTime.AddMilliseconds((double)cardStat.timestamp.Value);
					text = dateTime.ToString("yyyy-MM-dd") + " (" + cardStat.created + ")";
				}
			}
			long? totalGames = cardStat.totalGames;
			if (totalGames != null)
			{
				string text6 = text2;
				text2 = string.Concat(new object[]
				{
					text6,
					"Matches: ",
					cardStat.totalGames,
					"\n"
				});
			}
			long? played = cardStat.played;
			if (played != null)
			{
				string text6 = text2;
				text2 = string.Concat(new object[]
				{
					text6,
					"Played: ",
					cardStat.played,
					"\n"
				});
			}
			long? wins = cardStat.wins;
			if (wins != null)
			{
				string text6 = text3;
				text3 = string.Concat(new object[]
				{
					text6,
					"Wins: ",
					cardStat.wins,
					"\n"
				});
			}
			long? sacrificed = cardStat.sacrificed;
			if (sacrificed != null)
			{
				string text6 = text3;
				text3 = string.Concat(new object[]
				{
					text6,
					"Sacrificed: ",
					cardStat.sacrificed,
					"\n"
				});
			}
			long? heal = cardStat.heal;
			if (heal != null)
			{
				string text6 = text4;
				text4 = string.Concat(new object[]
				{
					text6,
					"Heal: ",
					cardStat.heal,
					"\n"
				});
			}
			long? damage = cardStat.damage;
			if (damage != null)
			{
				string text6 = text4;
				text4 = string.Concat(new object[]
				{
					text6,
					"Damage: ",
					cardStat.damage,
					"\n"
				});
			}
			long? unitKills = cardStat.unitKills;
			if (unitKills != null)
			{
				string text6 = text5;
				text5 = string.Concat(new object[]
				{
					text6,
					"Units Killed: ",
					cardStat.unitKills,
					"\n"
				});
			}
			long? idolKills = cardStat.idolKills;
			if (idolKills != null)
			{
				string text6 = text5;
				text5 = string.Concat(new object[]
				{
					text6,
					"Idols Killed: ",
					cardStat.idolKills,
					"\n"
				});
			}
			long? destroyed = cardStat.destroyed;
			if (destroyed != null)
			{
				string text6 = text5;
				text5 = string.Concat(new object[]
				{
					text6,
					"Destroyed: ",
					cardStat.destroyed,
					"\n"
				});
			}
		}
		else
		{
			text = "No statistics collected yet";
		}
		Font fontTitle = CardView.FontTitle;
		Font fontTextBold = CardView.FontTextBold;
		this.createStatisticsText(this.statsBoard, fontTitle, "STATISTICS", 29, new Vector3(0f, 0f, 0f), 1);
		this.createStatisticsText(this.statsBoard, fontTextBold, text, 27, new Vector3(0f, 0f, 0.77f), 1);
		this.createStatisticsText(this.statsBoard, fontTextBold, text2, 27, new Vector3(3.4f, 0f, 1.78f), 0);
		this.createStatisticsText(this.statsBoard, fontTextBold, text3, 27, new Vector3(-0.5f, 0f, 1.78f), 0);
		this.createStatisticsText(this.statsBoard, fontTextBold, text4, 22, new Vector3(3.4f, 0f, 3.62f), 0);
		this.createStatisticsText(this.statsBoard, fontTextBold, text5, 22, new Vector3(-0.5f, 0f, 3.62f), 0);
	}

	// Token: 0x0600094A RID: 2378 RVA: 0x00049DA4 File Offset: 0x00047FA4
	private void createHistoryTexts(CardHistory[] history)
	{
		if (this.historyBoard == null)
		{
			return;
		}
		Font fontTitle = CardView.FontTitle;
		Font fontTextBold = CardView.FontTextBold;
		this.createStatisticsText(this.historyBoard, fontTitle, "OWNER HISTORY", 29, new Vector3(0f, 0f, 0f), 1);
		for (int i = 0; i < history.Length; i++)
		{
			CardHistory cardHistory = history[i];
			string profileName = cardHistory.profileName;
			string text = cardHistory.date.Replace("minutes", "min");
			float num = 0.9f + 0.85f * (float)i;
			float num2 = 3.2f;
			float num3 = -0.9f;
			if (profileName.Length >= 16 && text.Contains("months"))
			{
				num2 += 0.35f;
				num3 -= 0.29f;
			}
			else if (profileName.Length >= 15)
			{
				num2 += 0.3f;
				num3 -= 0.4f;
			}
			this.createStatisticsText(this.historyBoard, fontTextBold, profileName, 27, new Vector3(num2, 0f, num), 0);
			this.createStatisticsText(this.historyBoard, fontTextBold, text, 22, new Vector3(num3, 0f, num + 0.05f), 0);
		}
	}

	// Token: 0x0600094B RID: 2379 RVA: 0x00049EF0 File Offset: 0x000480F0
	private GameObject createStatisticsText(GameObject parent, Font font, string text, int fontSize, Vector3 position, TextAnchor anchor)
	{
		GameObject gameObject = new GameObject("TextMesh_Stats");
		UnityUtil.addChild(parent, gameObject);
		gameObject.transform.localScale = new Vector3(0.8f, 1f, 1f);
		gameObject.transform.localEulerAngles = new Vector3(90f, 180f, 0f);
		gameObject.transform.localPosition = position;
		gameObject.name = "3DText_stats";
		this.textsArr.Add(gameObject);
		MeshRenderer meshRenderer = gameObject.AddComponent<MeshRenderer>();
		meshRenderer.material = font.material;
		meshRenderer.material.shader = ResourceManager.LoadShader(CardView.fontShader);
		meshRenderer.material.color = CardView.fontColor;
		this.renderQueueOffsets.Add(meshRenderer, -7);
		TextMesh textMesh = gameObject.AddComponent<TextMesh>();
		textMesh.anchor = anchor;
		textMesh.lineSpacing = 0.85f;
		textMesh.fontSize = fontSize;
		textMesh.characterSize = 0.3f;
		textMesh.text = text;
		textMesh.font = font;
		return gameObject;
	}

	// Token: 0x0600094C RID: 2380 RVA: 0x00049FF4 File Offset: 0x000481F4
	private void updateShowHelpCollider()
	{
		BoxCollider boxCollider = this.firstPageText.GetComponent<BoxCollider>();
		bool flag = boxCollider != null;
		bool flag2 = this.wantShowHelp && this.canShowHelp();
		if (flag == flag2)
		{
			return;
		}
		if (!flag)
		{
			boxCollider = this.firstPageText.AddComponent<BoxCollider>();
			boxCollider.center = new Vector3(0f, 0f, 2.15f);
			boxCollider.size = new Vector3(7f, 0f, 2.4f);
		}
		else
		{
			Object.Destroy(boxCollider);
		}
	}

	// Token: 0x0600094D RID: 2381 RVA: 0x00007F98 File Offset: 0x00006198
	public CardView enableShowHelp()
	{
		this.wantShowHelp = true;
		this.updateShowHelpCollider();
		return this;
	}

	// Token: 0x0600094E RID: 2382 RVA: 0x00007FA8 File Offset: 0x000061A8
	public CardView enableShowHistory()
	{
		return this.enableShowStats(true);
	}

	// Token: 0x0600094F RID: 2383 RVA: 0x00007FB1 File Offset: 0x000061B1
	public CardView enableShowStats()
	{
		return this.enableShowStats(false);
	}

	// Token: 0x06000950 RID: 2384 RVA: 0x0004A084 File Offset: 0x00048284
	public CardView enableShowStats(bool onlyHistory)
	{
		if (this.canShowStats || !this.card.isAtleastTier(2))
		{
			return this;
		}
		this.canShowStats = true;
		if (!onlyHistory)
		{
			this.statsBoard = this.createDefaultPage("StatsBoard");
			this.updateLayerFor(new GameObject[]
			{
				this.statsBoard
			});
			this.pages.Add(this.statsBoard);
		}
		this.historyBoard = this.createDefaultPage("HistoryBoard");
		this.updateLayerFor(new GameObject[]
		{
			this.historyBoard
		});
		this.pages.Add(this.historyBoard);
		this.nextPageButton = PrimitiveFactory.createPlane(true);
		this.nextPageButton.name = "NextPageArrow";
		this.nextPageButton.renderer.material = this.createMaterial();
		this.nextPageButton.renderer.material.mainTexture = ResourceManager.LoadTexture("Scrolls/Stats/statsbutton");
		this.updateLayerFor(new GameObject[]
		{
			this.nextPageButton
		});
		UnityUtil.addChild(base.gameObject, this.nextPageButton);
		this.nextPageButton.transform.localPosition = new Vector3(-4.12f, 0f, 2.46f);
		this.nextPageButton.transform.localScale = new Vector3(0.12f, 1f, 0.12f);
		this.registerRenderQueueOffset(this.nextPageButton.renderer, 2);
		return this;
	}

	// Token: 0x06000951 RID: 2385 RVA: 0x0004A200 File Offset: 0x00048400
	private void updateLayerFor(params GameObject[] gs)
	{
		foreach (GameObject gameObject in gs)
		{
			gameObject.layer = this.currentLayer;
		}
	}

	// Token: 0x06000952 RID: 2386 RVA: 0x0004A234 File Offset: 0x00048434
	private GameObject createDefaultPage(string name)
	{
		GameObject gameObject = new GameObject(name);
		UnityUtil.addChild(base.gameObject, gameObject);
		gameObject.transform.localScale = new Vector3(1f, 1f, 0.45f);
		gameObject.transform.localPosition = new Vector3(0f, 0f, 1.04f);
		return gameObject;
	}

	// Token: 0x06000953 RID: 2387 RVA: 0x00007FBA File Offset: 0x000061BA
	public static Vector3 CardLocalScale()
	{
		return CardView.CardLocalScale(1f);
	}

	// Token: 0x06000954 RID: 2388 RVA: 0x00007FC6 File Offset: 0x000061C6
	public static Vector3 CardLocalScale(float scaled)
	{
		return new Vector3(scaled * 0.093f, scaled * 0.001f, scaled * 0.15686f);
	}

	// Token: 0x06000955 RID: 2389 RVA: 0x0004A294 File Offset: 0x00048494
	public void effectAnimDone(EffectPlayer effect, bool loop)
	{
		if (effect.name == "scroll_foil_twinkle")
		{
			this.renderQueueOffsets.Remove(effect.renderer);
			Object.Destroy(effect.gameObject);
			Object.Destroy(this.foilTwinkle);
		}
		else
		{
			effect.playEffect(effect.getAnimPlayer().lastAnim);
		}
	}

	// Token: 0x06000956 RID: 2390 RVA: 0x000028DF File Offset: 0x00000ADF
	public void locator(EffectPlayer effect, AnimLocator loc)
	{
	}

	// Token: 0x06000957 RID: 2391 RVA: 0x00007FE2 File Offset: 0x000061E2
	public void onCardStatsReceived(GetCardStatsMessage m)
	{
		if (!this.canShowStats)
		{
			return;
		}
		if (this.card.id != m.cardId)
		{
			return;
		}
		this.setStatistics(m.cardStat, m.history);
	}

	// Token: 0x06000958 RID: 2392 RVA: 0x0004A2F4 File Offset: 0x000484F4
	public void onCardTypeUpdated(CardTypeUpdateMessage update)
	{
		if (this.card == null)
		{
			return;
		}
		if (this.card.typeId == 0)
		{
			return;
		}
		if (!update.hasUpdateFor(this.card))
		{
			return;
		}
		this.card.update();
		this.forceUpdateGraphics(this.card);
	}

	// Token: 0x06000959 RID: 2393 RVA: 0x0004A348 File Offset: 0x00048548
	public void onCostUpdate(EMCostUpdate m)
	{
		if (this.card == null)
		{
			return;
		}
		if (m.profileId != 0 && this._profileId != m.profileId)
		{
			return;
		}
		int costOrOverridden = this.getCostOrOverridden();
		int cost = m.getCost(this.card, costOrOverridden);
		if (cost != costOrOverridden)
		{
			this.overrideCost(cost);
		}
		this.createGraphics_Cost();
	}

	// Token: 0x0600095A RID: 2394 RVA: 0x00008019 File Offset: 0x00006219
	public override void renderAsEnabled(bool enabled, float time)
	{
		this.setColor((!enabled) ? AbstractCardView.DisabledColor : AbstractCardView.EnabledColor, time);
	}

	// Token: 0x040006A3 RID: 1699
	public const string DefaultShaderName = "Scrolls/Unlit/Transparent";

	// Token: 0x040006A4 RID: 1700
	private const string DefaultOpaqueShaderName = "Scrolls/Unlit/Color";

	// Token: 0x040006A5 RID: 1701
	private const float MinVisibleAlpha = 0.01f;

	// Token: 0x040006A6 RID: 1702
	private static string RESOURCE_DIRECTORY = OsSpec.getDownloadDataPath() + "/cardImages/";

	// Token: 0x040006A7 RID: 1703
	private string shader = "Scrolls/Unlit/Transparent";

	// Token: 0x040006A8 RID: 1704
	private static readonly string fontShader = "Scrolls/GUI/3DTextShader";

	// Token: 0x040006A9 RID: 1705
	private static readonly string fontShaderColored = "Scrolls/GUI/3DTextShaderColor";

	// Token: 0x040006AA RID: 1706
	private static readonly Color fontColor = new Color(0.23f, 0.16f, 0.125f);

	// Token: 0x040006AB RID: 1707
	private static readonly Color cardTextFontColor = new Color(0.23f, 0.16f, 0.125f);

	// Token: 0x040006AC RID: 1708
	private Color foilPlateColor = new Color(1f, 1f, 1f);

	// Token: 0x040006AD RID: 1709
	private iCardRule callBackTarget;

	// Token: 0x040006AE RID: 1710
	private GameObject cardImage;

	// Token: 0x040006AF RID: 1711
	private AudioScript audioScript;

	// Token: 0x040006B0 RID: 1712
	private static readonly Font FontTitle;

	// Token: 0x040006B1 RID: 1713
	private static readonly Font FontText;

	// Token: 0x040006B2 RID: 1714
	private static readonly Font FontTextBold;

	// Token: 0x040006B3 RID: 1715
	private static readonly Font FontTextBoldItalic;

	// Token: 0x040006B4 RID: 1716
	private GameObject icoBG;

	// Token: 0x040006B5 RID: 1717
	private GameObject ico;

	// Token: 0x040006B6 RID: 1718
	private GameObject statsBG;

	// Token: 0x040006B7 RID: 1719
	private GameObject foilPlate;

	// Token: 0x040006B8 RID: 1720
	private string tooltip = string.Empty;

	// Token: 0x040006B9 RID: 1721
	private bool useRendQ;

	// Token: 0x040006BA RID: 1722
	private GameObject firstPageText;

	// Token: 0x040006BB RID: 1723
	private GameObject nextPageButton;

	// Token: 0x040006BC RID: 1724
	private GameObject infoButton;

	// Token: 0x040006BD RID: 1725
	private GameObject helpOverlay;

	// Token: 0x040006BE RID: 1726
	private Pages pages = new Pages();

	// Token: 0x040006BF RID: 1727
	private Vector3 startPos;

	// Token: 0x040006C0 RID: 1728
	public string attackType;

	// Token: 0x040006C1 RID: 1729
	public bool cardMoving;

	// Token: 0x040006C2 RID: 1730
	private TilePosition pos;

	// Token: 0x040006C3 RID: 1731
	private List<EnchantmentInfo> buffs;

	// Token: 0x040006C4 RID: 1732
	private bool hasSetHighResTexture;

	// Token: 0x040006C5 RID: 1733
	private List<GameObject> numbersArr = new List<GameObject>();

	// Token: 0x040006C6 RID: 1734
	private List<GameObject> textsArr = new List<GameObject>();

	// Token: 0x040006C7 RID: 1735
	private int rendQ;

	// Token: 0x040006C8 RID: 1736
	private List<GameObject> gosPassiveAbilities = new List<GameObject>();

	// Token: 0x040006C9 RID: 1737
	private List<GameObject> gosActiveAbilities = new List<GameObject>();

	// Token: 0x040006CA RID: 1738
	private List<GameObject> gosBuffs = new List<GameObject>();

	// Token: 0x040006CB RID: 1739
	private bool selected;

	// Token: 0x040006CC RID: 1740
	private bool locked;

	// Token: 0x040006CD RID: 1741
	private int? overriddenCost;

	// Token: 0x040006CE RID: 1742
	private EffectPlayer effectGlow;

	// Token: 0x040006CF RID: 1743
	private EffectPlayer effectWhite;

	// Token: 0x040006D0 RID: 1744
	private List<GameObject> effectPlayers = new List<GameObject>();

	// Token: 0x040006D1 RID: 1745
	public bool doHandleInput = true;

	// Token: 0x040006D2 RID: 1746
	public bool doHandleClicks = true;

	// Token: 0x040006D3 RID: 1747
	private bool disableTooltip;

	// Token: 0x040006D4 RID: 1748
	private int _profileId;

	// Token: 0x040006D5 RID: 1749
	private ColorChanger colorChanger = new ColorChanger();

	// Token: 0x040006D6 RID: 1750
	private Dictionary<Renderer, int> renderQueueOffsets = new Dictionary<Renderer, int>();

	// Token: 0x040006D7 RID: 1751
	private Camera rayCastCamera;

	// Token: 0x040006D8 RID: 1752
	private CardView.Type type;

	// Token: 0x040006D9 RID: 1753
	private Color glowColor = Color.white;

	// Token: 0x040006DA RID: 1754
	private float glowAlpha;

	// Token: 0x040006DB RID: 1755
	private GameObject lockGo;

	// Token: 0x040006DC RID: 1756
	private bool largeLock = true;

	// Token: 0x040006DD RID: 1757
	private bool _isFlying;

	// Token: 0x040006DE RID: 1758
	private long nextFoilTimeStamp = -1L;

	// Token: 0x040006DF RID: 1759
	private bool runFoilRainbow = true;

	// Token: 0x040006E0 RID: 1760
	private bool foilRainbowDisabled;

	// Token: 0x040006E1 RID: 1761
	private float fadeModifier;

	// Token: 0x040006E2 RID: 1762
	private float alpha = 1f;

	// Token: 0x040006E3 RID: 1763
	public Color baseColor = Color.white;

	// Token: 0x040006E4 RID: 1764
	private bool isFading;

	// Token: 0x040006E5 RID: 1765
	private Color fadeSrcColor;

	// Token: 0x040006E6 RID: 1766
	private Color fadeDstColor;

	// Token: 0x040006E7 RID: 1767
	private float fadeStartTime;

	// Token: 0x040006E8 RID: 1768
	private float fadeDuration;

	// Token: 0x040006E9 RID: 1769
	private int currentLayer;

	// Token: 0x040006EA RID: 1770
	private bool isInited;

	// Token: 0x040006EB RID: 1771
	private GameObject foilRainbow;

	// Token: 0x040006EC RID: 1772
	private EffectPlayer foilAnim;

	// Token: 0x040006ED RID: 1773
	private GameObject foilTwinkle;

	// Token: 0x040006EE RID: 1774
	private bool costGrayout;

	// Token: 0x040006EF RID: 1775
	private bool cardGrayedOut;

	// Token: 0x040006F0 RID: 1776
	private List<GameObject> gosNumCost = new List<GameObject>();

	// Token: 0x040006F1 RID: 1777
	private int lastCost = -1;

	// Token: 0x040006F2 RID: 1778
	private List<GameObject> gosNumAttackPower = new List<GameObject>();

	// Token: 0x040006F3 RID: 1779
	private List<GameObject> gosNumCountdown = new List<GameObject>();

	// Token: 0x040006F4 RID: 1780
	private List<GameObject> gosNumHitPoints = new List<GameObject>();

	// Token: 0x040006F5 RID: 1781
	private bool showTexts = true;

	// Token: 0x040006F6 RID: 1782
	private static readonly string ColorWord = ColorUtil.ToHexString(ColorUtil.FromHex24(1854061u));

	// Token: 0x040006F7 RID: 1783
	private static readonly string ColorCardReference = CardView.ColorWord;

	// Token: 0x040006F8 RID: 1784
	private TextureFetcher highResFetcher;

	// Token: 0x040006F9 RID: 1785
	private bool hasFetchedStats;

	// Token: 0x040006FA RID: 1786
	private bool wantShowHelp;

	// Token: 0x040006FB RID: 1787
	private bool canShowStats;

	// Token: 0x040006FC RID: 1788
	private GameObject statsBoard;

	// Token: 0x040006FD RID: 1789
	private GameObject historyBoard;

	// Token: 0x0200011E RID: 286
	public enum Type
	{
		// Token: 0x04000702 RID: 1794
		Normal,
		// Token: 0x04000703 RID: 1795
		Small
	}

	// Token: 0x0200011F RID: 287
	public class OpCastInfo
	{
		// Token: 0x0600095E RID: 2398 RVA: 0x00008048 File Offset: 0x00006248
		public OpCastInfo(BattleMode callback, int xMod)
		{
			this.callback = callback;
			this.xMod = xMod;
		}

		// Token: 0x04000704 RID: 1796
		internal BattleMode callback;

		// Token: 0x04000705 RID: 1797
		internal int xMod;
	}
}
