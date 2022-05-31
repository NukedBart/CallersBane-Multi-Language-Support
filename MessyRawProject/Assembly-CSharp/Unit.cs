using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x020000B5 RID: 181
public class Unit : MonoBehaviour, iAnim, iEffect
{
	// Token: 0x06000645 RID: 1605 RVA: 0x00005F2C File Offset: 0x0000412C
	public void setBaseScale(float scale)
	{
		this.baseScaleMult = scale;
		this.BaseScale = 0.14285715f * scale;
	}

	// Token: 0x06000646 RID: 1606 RVA: 0x00005F42 File Offset: 0x00004142
	private void Start()
	{
		this.audioScript = App.AudioScript;
		base.gameObject.renderer.castShadows = false;
		base.gameObject.renderer.receiveShadows = false;
		this.attackSlowdownDummy = new GameObject("unit_tween_dummy");
	}

	// Token: 0x06000647 RID: 1607 RVA: 0x0003B75C File Offset: 0x0003995C
	public void initForDeckbuilder(string url, CardType cardType)
	{
		this.unitAnimationOnly = true;
		this.useUnlitShader = true;
		this.init(url, null, 0f, new TilePosition(), Unit.StatsState.NONE, 1, new Card(0L, cardType));
	}

	// Token: 0x06000648 RID: 1608 RVA: 0x0003B794 File Offset: 0x00039994
	public void init(string URL, Unit.ICallback callBackTarget, float zPos, TilePosition pos, Unit.StatsState statsState, int directionMod, Card card)
	{
		this.callBackTarget = callBackTarget;
		if (this.audioScript == null)
		{
			this.audioScript = App.AudioScript;
		}
		this.numberMaterial = Shaders.matMilkBurn();
		this.numberMaterial.renderQueue = 91601;
		this.statsNumsArr.Add(-10);
		this.statsNumsArr.Add(-10);
		this.statsNumsArr.Add(-10);
		this.card = card;
		this.cardType = card.getCardType();
		this.trSound = new TagSoundReader(this.cardType);
		this.hitPoints = this.cardType.hp;
		this.attackPower = this.cardType.ap;
		this.attackCounter = this.cardType.ac;
		this.scalePosModifier = ScalePosModifier.create(this.cardType);
		this.directionMod = directionMod;
		this.zPos = zPos;
		this.cardTypeHasAnimation = !this.cardType.hasTag("unit_no_anim");
		this._bundleId = string.Empty + this.cardType.animationBundle;
		base.gameObject.tag = "blinkable_unit";
		if (!this.unitAnimationOnly)
		{
			string defaultName = "Sounds/hyperduck/UI/ui_summon_unit";
			this.playSound(this.trSound.get("sound_loadunit", defaultName));
		}
		if (callBackTarget != null)
		{
			this.setupUnitIcons();
			if (statsState == Unit.StatsState.FLASH)
			{
				this.flashUnitStats();
			}
			else if (statsState == Unit.StatsState.LONGFLASH)
			{
				this.flashUnitStats(0.5f);
			}
			else if (statsState == Unit.StatsState.HOLD)
			{
				this.showStats(true);
			}
		}
		this.recreateUnitMaterials();
		this.attackFullyCompleted();
		base.renderer.enabled = false;
		this.setTilePosition(pos);
		base.StartCoroutine(EnumeratorUtil.chain(new IEnumerator[]
		{
			this.downloadAssets()
		}));
	}

	// Token: 0x06000649 RID: 1609 RVA: 0x00005F81 File Offset: 0x00004181
	private int calculateRenderQueue()
	{
		return (this.overriddenRenderQueue >= 0) ? this.overriddenRenderQueue : Unit.getRowRenderQueue(this.pos.row);
	}

	// Token: 0x0600064A RID: 1610 RVA: 0x00005FAA File Offset: 0x000041AA
	public static int getRowRenderQueue(int row)
	{
		return 90000 + row;
	}

	// Token: 0x0600064B RID: 1611 RVA: 0x00005FB3 File Offset: 0x000041B3
	private Material createMaterial()
	{
		return new Material((!this.useUnlitShader) ? Unit._defaultMaterial : Unit._unlitMaterial);
	}

	// Token: 0x0600064C RID: 1612 RVA: 0x0003B974 File Offset: 0x00039B74
	private void recreateUnitMaterials()
	{
		foreach (Material material in new Material[]
		{
			this.normalMaterial,
			this.enchantmentMaterial,
			this.hitMaterial,
			this.healMaterial,
			this.frostWindMaterial,
			this.buffMaterial,
			this.shadowMaterial
		})
		{
			if (material != null)
			{
				Object.Destroy(material, 0.25f);
			}
		}
		this.normalMaterial = this.createMaterial();
		this.createFlavor();
		this.enchantmentMaterial = this.createMaterial();
		this.enchantmentMaterial.color = Unit.enchantmentStartColor;
		this.hitMaterial = this.createMaterial();
		this.healMaterial = this.createMaterial();
		this.healMaterial.SetColor("_Color", new Color(0.76862746f, 1f, 0.49411765f, 1f));
		this.frostWindMaterial = new Material(ResourceManager.LoadShader("Scrolls/FrostWind"));
		this.frostWindMaterial.SetColor("_A", new Color(0.8f, 0.8f, 0.82f, 0f));
		this.frostWindMaterial.SetTexture("_Texture2", ResourceManager.LoadTexture("Shader/FrostTexture"));
		this.frostWindMaterial.SetFloat("_Lerp", 0f);
		this.buffMaterial = this.createMaterial();
		this.buffMaterial.SetColor("_Color", Unit.enchantmentStartColor);
		this.buffMaterial.SetFloat("_Lerp", 1f);
		this.shadowMaterial = this.createMaterial();
		this.shadowMaterial.SetColor("_Color", Unit.shadowColor);
		this.shadowMaterial.SetFloat("_Lerp", 1f);
	}

	// Token: 0x0600064D RID: 1613 RVA: 0x0003BB40 File Offset: 0x00039D40
	private void createFlavor()
	{
		if (!this.animLoaded || this.hasCreatedFlavor)
		{
			return;
		}
		this.hasCreatedFlavor = true;
		this.flavorTalk = FlavorTalk.get(this.cardType);
		if (!this.flavorTalk)
		{
			return;
		}
		if (this.cardType.name == "Harvester")
		{
			this.normalMaterial = new Material(ResourceManager.LoadShader("Scrolls/Transparent/Diffuse/Double/Texture/Color"));
			this.normalMaterial.SetTexture("_Texture2", ResourceManager.LoadTexture("BattleMode/Flavor/Spain"));
			this.normalMaterial.SetFloat("_Lerp", 0.5f);
			this.say(this.flavorTalk.greeting);
		}
	}

	// Token: 0x0600064E RID: 1614 RVA: 0x00005FD4 File Offset: 0x000041D4
	public void say(string text)
	{
		if (text == null)
		{
			return;
		}
		base.gameObject.AddComponent<SpeechBubble>().init(text);
	}

	// Token: 0x0600064F RID: 1615 RVA: 0x0003BBFC File Offset: 0x00039DFC
	private UnitAnimDescription loadUnitAnimDescription()
	{
		string folder = Unit.localPath.Invoke(StorageEnvironment.getAnimationBundleUnzipPath(this.cardType));
		this.serializationType = "PBuf";
		return UnitAnimDescriptionFactory.fromProtoFolder(folder);
	}

	// Token: 0x06000650 RID: 1616 RVA: 0x0003BC30 File Offset: 0x00039E30
	private bool loadBundle()
	{
		UnitAnimDescription unitAnimDescription = this.getUnitAnimDescription();
		if (unitAnimDescription == null)
		{
			return false;
		}
		this.animLoaded = true;
		this.recreateUnitMaterials();
		this.refreshRenderQueues();
		this.setUnitAnim(unitAnimDescription);
		if (this.unitAnimationOnly)
		{
			base.renderer.enabled = true;
		}
		if (this.animPlayer.hasAnimationId("projectile"))
		{
			this.isProjectile = true;
			this._projectileAnim = new AnimPlayer(App.Clocks.animClock);
			this._projectileAnim.setDescription(unitAnimDescription, null);
			this._projectileAnim.setAnimationId("projectile");
			if (this.cardType.hasTag("fast_projectile") || this.animPlayer.hasAnimationId("fastprojectile"))
			{
				this.isFastProjectile = true;
			}
		}
		this.playAnimation("Idle");
		return true;
	}

	// Token: 0x06000651 RID: 1617 RVA: 0x0003BD08 File Offset: 0x00039F08
	private void setUnitAnim(UnitAnimDescription data)
	{
		this.animPlayer.setDescription(data, this);
		this.attackNamer = Unit.AnimNamerFactory.create(this.animPlayer, "attack");
		this.normalMaterial.mainTexture = data.textureReference;
		this.enchantmentMaterial.mainTexture = data.textureReference;
		this.shadowMaterial.mainTexture = data.textureReference;
		this.shadowMaterial.renderQueue = 9999;
	}

	// Token: 0x06000652 RID: 1618 RVA: 0x0003BD7C File Offset: 0x00039F7C
	private UnitAnimDescription getUnitAnimDescription()
	{
		if (!this.cardTypeHasAnimation)
		{
			return null;
		}
		if (this.cardType.useDummyAnimationBundle())
		{
			uint hex = (uint)(this.getName().GetHashCode() & 16777215);
			this.normalMaterial.shader = ResourceManager.LoadShader("Scrolls/Transparent/Diffuse/Double");
			this.normalMaterial.SetColor("_Color", ColorUtil.FromHex24(hex));
			this.normalMaterial.color = ColorUtil.FromHex24(hex);
			string animFolder = "defaultunit_forward";
			if (this.card.getPieceKind() == CardType.Kind.STRUCTURE)
			{
				animFolder = "defaultunit_structure";
			}
			return ResourceManager.instance.getBundledAnimationData(animFolder);
		}
		string text = Unit.localPath.Invoke(StorageEnvironment.getAnimationBundleZipPath(this.cardType));
		string text2 = Unit.localPath.Invoke(StorageEnvironment.getAnimationBundleUnzipPath(this.cardType));
		if (!Directory.Exists(text2) && !File.Exists(text))
		{
			return null;
		}
		UnitAnimDescription unitAnimDescription = ResourceManager.instance.getUnitData(this._bundleId);
		if (unitAnimDescription != null)
		{
			return unitAnimDescription;
		}
		try
		{
			long num = TimeUtil.CurrentTimeMillis();
			this.serializationType = "none";
			unitAnimDescription = this.loadUnitAnimDescription();
			if (unitAnimDescription == null)
			{
				FileUtil.extractZip(text, text2);
				unitAnimDescription = this.loadUnitAnimDescription();
				this.serializationType = "JZip";
			}
			long num2 = TimeUtil.CurrentTimeMillis() - num;
			Unit.totalTime += num2;
			Texture2D texture2D = this.loadBundleImage("sprites.png");
			texture2D.filterMode = 2;
			texture2D.anisoLevel = 0;
			unitAnimDescription.textureReference = texture2D;
			unitAnimDescription.nameAnimations(this._bundleId);
			ResourceManager.instance.assignUnitData(this._bundleId, unitAnimDescription);
		}
		catch (Exception ex)
		{
			Log.error("error: " + ex);
			try
			{
				File.Delete(text);
			}
			catch (Exception)
			{
			}
			try
			{
				Directory.Delete(text2, true);
			}
			catch (Exception)
			{
			}
			return null;
		}
		return unitAnimDescription;
	}

	// Token: 0x06000653 RID: 1619 RVA: 0x00005FEF File Offset: 0x000041EF
	public bool isPartiallyLoaded()
	{
		return this.previewLoaded || this.animLoaded;
	}

	// Token: 0x06000654 RID: 1620 RVA: 0x00006005 File Offset: 0x00004205
	public bool isFullyLoaded()
	{
		return (!this.cardType.hasTag("unit_no_anim")) ? this.animLoaded : this.previewLoaded;
	}

	// Token: 0x06000655 RID: 1621 RVA: 0x0003BF90 File Offset: 0x0003A190
	private void setupUnitIcons()
	{
		this.iconFrame = new GameObject("statsboard");
		UnityUtil.addChild(base.gameObject, this.iconFrame);
		this.iconFrame.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
		this.iconFrame.transform.localEulerAngles = new Vector3(90f, 180f, 0f);
		this.attackStat = new Stat(this.iconFrame, Stat.Type.Attack, new Vector3(2f, 0f, 2.35f));
		this.statsboardSymbols.AddRange(this.attackStat.getGameObjectsWithRenderers());
		this.countdownStat = new Stat(this.iconFrame, Stat.Type.Countdown, new Vector3(0f, 0f, 4.5f));
		this.statsboardSymbols.AddRange(this.countdownStat.getGameObjectsWithRenderers());
		this.healthStat = new Stat(this.iconFrame, Stat.Type.Health, new Vector3(-2f, 0f, 2.35f));
		this.statsboardSymbols.AddRange(this.healthStat.getGameObjectsWithRenderers());
		this.flashDigits = false;
		this.statsNumberHitPoints();
		this.statsNumberAttackPower();
		this.statsNumberAttackCounter();
		foreach (GameObject gameObject in this.statsboardSymbols)
		{
			gameObject.renderer.material.renderQueue = 91600;
		}
	}

	// Token: 0x06000656 RID: 1622 RVA: 0x0003C130 File Offset: 0x0003A330
	private void statsSetAlpha(float alpha)
	{
		this.doForAllStatsObjs(delegate(GameObject g)
		{
			g.renderer.material.color = ColorUtil.GetWithAlpha(g.renderer.material.color, alpha);
		});
	}

	// Token: 0x06000657 RID: 1623 RVA: 0x0003C15C File Offset: 0x0003A35C
	private void statsSetEnabled(bool enabled)
	{
		this.doForAllStatsObjs(delegate(GameObject g)
		{
			g.renderer.enabled = enabled;
		});
	}

	// Token: 0x06000658 RID: 1624 RVA: 0x0003C188 File Offset: 0x0003A388
	public void showStats(bool showUnitStats)
	{
		if (!showUnitStats && this._alwaysShowStats)
		{
			return;
		}
		this.showUnitStats = showUnitStats;
		this.statsboardSymbols.ForEach(delegate(GameObject g)
		{
			g.renderer.enabled = showUnitStats;
		});
		if (showUnitStats)
		{
			this.statsSetAlpha(1f);
		}
		this.statsNumberAttackPower();
		this.statsNumberAttackCounter();
		this.statsNumberHitPoints();
		this.flashTimer = -1f;
	}

	// Token: 0x06000659 RID: 1625 RVA: 0x0000602D File Offset: 0x0000422D
	private void doForAllStatsObjs(Action<GameObject> f)
	{
		this.doForAllStatsObjs(true, f);
	}

	// Token: 0x0600065A RID: 1626 RVA: 0x0003C210 File Offset: 0x0003A410
	private void doForAllStatsObjs(bool stats, Action<GameObject> f)
	{
		if (stats)
		{
			foreach (GameObject gameObject in this.statsboardSymbols)
			{
				f.Invoke(gameObject);
			}
		}
		foreach (GameObject gameObject2 in this.attackPowerObjArr)
		{
			f.Invoke(gameObject2);
		}
		foreach (GameObject gameObject3 in this.hitPointsObjArr)
		{
			f.Invoke(gameObject3);
		}
		foreach (GameObject gameObject4 in this.attackCounterObjArr)
		{
			f.Invoke(gameObject4);
		}
	}

	// Token: 0x0600065B RID: 1627 RVA: 0x00006037 File Offset: 0x00004237
	public void flashUnitStats()
	{
		this.flashUnitStats(0f);
	}

	// Token: 0x0600065C RID: 1628 RVA: 0x00006044 File Offset: 0x00004244
	public void flashUnitStats(float extraTime)
	{
		if (this._alwaysShowStats)
		{
			return;
		}
		this.showUnitStats = true;
		this.statsSetAlpha(1f);
		this.statsSetEnabled(this.showUnitStats);
		this.flashTimer = Time.time + extraTime;
	}

	// Token: 0x0600065D RID: 1629 RVA: 0x0003C354 File Offset: 0x0003A554
	private void statsNumberAttackPower()
	{
		if (this.attackPower == this.statsNumsArr[0])
		{
			this.attackPowerObjArr.ForEach(delegate(GameObject g)
			{
				g.renderer.enabled = this.showUnitStats;
			});
			return;
		}
		if (!this.showUnitStats)
		{
			this.flashUnitStats();
		}
		this.attackPowerObjArr.ForEach(delegate(GameObject g)
		{
			Object.Destroy(g);
		});
		this.attackPowerObjArr.Clear();
		this.attackPowerObjArr.AddRange(this.createSymbols(this.attackPower, this.attackStat.digits.transform));
		this.attackPowerObjArr.ForEach(delegate(GameObject g)
		{
			g.tag = "blinkable_attack";
		});
		Color buffColor = this.getBuffColor(this.attackPower - this.card.getAttackPower());
		this.colorObjects(this.attackPowerObjArr, buffColor);
		this.statsNumsArr[0] = this.attackPower;
		if (this.flashDigits)
		{
			this.attackStat.digits.GetComponent<DigitGrower>().reset();
		}
	}

	// Token: 0x0600065E RID: 1630 RVA: 0x0003C46C File Offset: 0x0003A66C
	private void statsNumberAttackCounter()
	{
		if (this.attackCounter == this.statsNumsArr[1])
		{
			this.attackCounterObjArr.ForEach(delegate(GameObject g)
			{
				g.renderer.enabled = this.showUnitStats;
			});
			return;
		}
		if (!this.showUnitStats)
		{
			this.flashUnitStats();
		}
		this.attackCounterObjArr.ForEach(delegate(GameObject g)
		{
			Object.Destroy(g);
		});
		this.attackCounterObjArr.Clear();
		this.attackCounterObjArr.AddRange(this.createSymbols(this.attackCounter, this.countdownStat.digits.transform));
		this.attackCounterObjArr.ForEach(delegate(GameObject g)
		{
			g.tag = "blinkable_countdown";
		});
		this.statsNumsArr[1] = this.attackCounter;
		if (this.flashDigits)
		{
			this.countdownStat.digits.GetComponent<DigitGrower>().reset();
		}
	}

	// Token: 0x0600065F RID: 1631 RVA: 0x0003C55C File Offset: 0x0003A75C
	private void statsNumberHitPoints()
	{
		if (this.hitPoints == this.statsNumsArr[2] || this.hitPoints < 0)
		{
			this.hitPointsObjArr.ForEach(delegate(GameObject g)
			{
				g.renderer.enabled = this.showUnitStats;
			});
			return;
		}
		if (!this.showUnitStats)
		{
			this.flashUnitStats();
		}
		this.hitPointsObjArr.ForEach(delegate(GameObject g)
		{
			Object.Destroy(g);
		});
		this.hitPointsObjArr.Clear();
		this.hitPointsObjArr.AddRange(this.createSymbols(this.hitPoints, this.healthStat.digits.transform));
		this.hitPointsObjArr.ForEach(delegate(GameObject g)
		{
			g.tag = "blinkable_health";
		});
		Color buffColor = this.getBuffColor(this.hitPoints - this.card.getHitPoints());
		this.colorObjects(this.hitPointsObjArr, buffColor);
		this.statsNumsArr[2] = this.hitPoints;
		if (this.flashDigits)
		{
			this.healthStat.digits.GetComponent<DigitGrower>().reset();
		}
	}

	// Token: 0x06000660 RID: 1632 RVA: 0x0000607D File Offset: 0x0000427D
	private Color getBuffColor(int difference)
	{
		if (difference > 0)
		{
			return ((BattleMode)this.callBackTarget).overColor;
		}
		if (difference < 0)
		{
			return ((BattleMode)this.callBackTarget).underColor;
		}
		return Color.white;
	}

	// Token: 0x06000661 RID: 1633 RVA: 0x0003C680 File Offset: 0x0003A880
	private void colorObjects(List<GameObject> objects, Color color)
	{
		foreach (GameObject gameObject in objects)
		{
			gameObject.renderer.material.color = color;
		}
	}

	// Token: 0x06000662 RID: 1634 RVA: 0x0003C6E0 File Offset: 0x0003A8E0
	private GameObject createSymbol(char digit, Transform parent, Vector2 offset)
	{
		GameObject gameObject = PrimitiveFactory.createPlane(false);
		Texture2D mainTexture = ResourceManager.LoadTexture("BattleUI/battlegui_number_" + digit);
		UnityUtil.addChild(parent.gameObject, gameObject);
		gameObject.name = string.Empty + digit;
		gameObject.transform.localScale = new Vector3(30f, 1f, 40f) * 0.5f;
		gameObject.transform.localPosition = new Vector3(offset.x, 0f, offset.y);
		gameObject.renderer.material = this.numberMaterial;
		gameObject.renderer.material.mainTexture = mainTexture;
		gameObject.renderer.enabled = this.showUnitStats;
		return gameObject;
	}

	// Token: 0x06000663 RID: 1635 RVA: 0x0003C7B0 File Offset: 0x0003A9B0
	private List<GameObject> createSymbols(int value, Transform parent)
	{
		List<GameObject> list = new List<GameObject>();
		char[] array;
		if (value >= 0)
		{
			array = Convert.ToString(value).ToCharArray();
		}
		else
		{
			(array = new char[1])[0] = '-';
		}
		char[] array2 = array;
		int num = array2.Length - 1;
		for (int i = 0; i < array2.Length; i++)
		{
			Vector2 zero = Vector2.zero;
			if (array2.Length > 1)
			{
				zero.x = (float)(80 - 210 * i);
			}
			GameObject gameObject = this.createSymbol(array2[i], parent, zero);
			iTween.ScaleTo(gameObject, iTween.Hash(new object[]
			{
				"x",
				28,
				"z",
				40,
				"time",
				0.7f,
				"easetype",
				iTween.EaseType.elastic
			}));
			list.Add(gameObject);
		}
		return list;
	}

	// Token: 0x06000664 RID: 1636 RVA: 0x000060B4 File Offset: 0x000042B4
	public void unitAttack()
	{
		this.unitAttack(base.gameObject.transform.position, 1f);
	}

	// Token: 0x06000665 RID: 1637 RVA: 0x0003C898 File Offset: 0x0003AA98
	public void unitAttack(Vector3 attackTo, float mod)
	{
		this.attackTo = new Vector3(attackTo.x, attackTo.y, attackTo.z);
		this.projectileAttackTo = new Vector3(attackTo.x, attackTo.y, attackTo.z - mod);
		if (this.flavorTalk)
		{
			this.say(this.flavorTalk.attack);
		}
		this.isAttackDone = false;
		this._attackDoneCounter = 0;
		if (this.isProjectile)
		{
			this.playAnimation("Attack");
		}
		else
		{
			if (this.firstAttack)
			{
				this.playOptionalTagSound("sound_prepare");
			}
			if (this.animPlayer.hasAnimationId("preattack") && this.firstAttack)
			{
				this.playAnimation("PrePreAttack");
				this.firstAttack = false;
			}
			else
			{
				this.startAttack();
			}
		}
	}

	// Token: 0x06000666 RID: 1638 RVA: 0x0003C984 File Offset: 0x0003AB84
	public void attackDone()
	{
		if (this.isProjectile)
		{
			return;
		}
		if (++this._attackDoneCounter == 2)
		{
			iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
			{
				"z",
				this.zPos,
				"time",
				0.25f,
				"easetype",
				"linear",
				"oncompletetarget",
				base.gameObject,
				"oncomplete",
				"tweenComplete",
				"oncompleteparams",
				"Back"
			}));
			this.playAnimation("Back");
		}
	}

	// Token: 0x06000667 RID: 1639 RVA: 0x000060D1 File Offset: 0x000042D1
	public void attackFullyCompleted()
	{
		this.firstAttack = true;
		this.attackNamer = Unit.AnimNamerFactory.create(this.animPlayer, "attack");
	}

	// Token: 0x06000668 RID: 1640 RVA: 0x0003CA48 File Offset: 0x0003AC48
	private void startAttack()
	{
		this.playAnimation("PreAttack");
		if (!this.unitAnimationOnly)
		{
			GameObject gameObject = this.createEffectAnimation("run-dust-1", this.currentRenderQueueWithOffset(1), new Vector3(base.transform.position.x + Unit.DustOffset.y, base.transform.position.y, base.transform.position.z + Unit.DustOffset.x * (float)this.directionMod), this.directionMod, 0.8f, false, string.Empty, 0);
			gameObject.GetComponent<EffectPlayer>().startInSeconds(Unit.DustDelay);
		}
		float num = (Math.Abs(base.transform.position.z - this.attackTo.z) - Unit.StopDistance) / 7.4f;
		if (this.unitAnimationOnly)
		{
			num = 1f;
		}
		iTween.MoveTo(this.attackSlowdownDummy, iTween.Hash(new object[]
		{
			"z",
			100,
			"time",
			num,
			"easetype",
			"linear",
			"oncompletetarget",
			base.gameObject,
			"oncomplete",
			"tweenComplete",
			"oncompleteparams",
			"PreAttack"
		}));
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"z",
			this.attackTo.z,
			"delay",
			num,
			"time",
			Unit.StopTime,
			"easetype",
			Unit.EaseType
		}));
		iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
		{
			"z",
			this.attackTo.z + (float)this.directionMod * Unit.StopDistance,
			"time",
			num,
			"easetype",
			"linear"
		}));
	}

	// Token: 0x06000669 RID: 1641 RVA: 0x000060F0 File Offset: 0x000042F0
	public void moveUnitStart()
	{
		this._moving = true;
	}

	// Token: 0x0600066A RID: 1642 RVA: 0x000060F9 File Offset: 0x000042F9
	public void moveUnitComplete()
	{
		if (!this._moving)
		{
			Log.warning("Unit::moveUnitComplete - Unit wasn't moving!");
		}
		this._moving = false;
		this.effectDone();
	}

	// Token: 0x0600066B RID: 1643 RVA: 0x0003CC94 File Offset: 0x0003AE94
	public string getMoveAbility()
	{
		foreach (ActiveAbility activeAbility in this.getActiveAbilities())
		{
			if (activeAbility.isMoveLike())
			{
				return activeAbility.id;
			}
		}
		return ActiveAbility.Move;
	}

	// Token: 0x0600066C RID: 1644 RVA: 0x0000611D File Offset: 0x0000431D
	public void Destroy()
	{
		Object.Destroy(base.gameObject);
		this._isDestroyed = true;
	}

	// Token: 0x0600066D RID: 1645 RVA: 0x00006131 File Offset: 0x00004331
	public bool isDestroyed()
	{
		return this._isDestroyed;
	}

	// Token: 0x0600066E RID: 1646 RVA: 0x0003CCD8 File Offset: 0x0003AED8
	private void tweenComplete(string anim)
	{
		if (anim != null)
		{
			if (Unit.<>f__switch$mapE == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(2);
				dictionary.Add("PreAttack", 0);
				dictionary.Add("Back", 1);
				Unit.<>f__switch$mapE = dictionary;
			}
			int num;
			if (Unit.<>f__switch$mapE.TryGetValue(anim, ref num))
			{
				if (num != 0)
				{
					if (num == 1)
					{
						this.playAnimation("Idle");
					}
				}
				else if (this.animPlayer.getUnitDesc() != null)
				{
					this.playAnimation("Attack");
				}
				else
				{
					iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
					{
						"z",
						this.zPos,
						"time",
						0.25f,
						"easetype",
						"linear",
						"oncompletetarget",
						base.gameObject,
						"oncomplete",
						"tweenComplete",
						"oncompleteparams",
						"Back"
					}));
					this.postAttackEffectDone();
				}
			}
		}
	}

	// Token: 0x0600066F RID: 1647 RVA: 0x00006139 File Offset: 0x00004339
	private void _setAnimationId(string anim)
	{
		this.currentAnimPsds.Clear();
		this.animPlayer.setAnimationId(anim);
	}

	// Token: 0x06000670 RID: 1648 RVA: 0x0003CE04 File Offset: 0x0003B004
	public void playAnimation(string anim)
	{
		if (anim != null)
		{
			if (Unit.<>f__switch$mapF == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(8);
				dictionary.Add("Idle", 0);
				dictionary.Add("PrePreAttack", 1);
				dictionary.Add("PreAttack", 2);
				dictionary.Add("Attack", 3);
				dictionary.Add("AttackProjectile", 4);
				dictionary.Add("Back", 5);
				dictionary.Add("Damage", 6);
				dictionary.Add("ActivateAbility", 7);
				Unit.<>f__switch$mapF = dictionary;
			}
			int num;
			if (Unit.<>f__switch$mapF.TryGetValue(anim, ref num))
			{
				switch (num)
				{
				case 0:
					this._setAnimationId("idle");
					this.setAnimState(Unit.AnimState.Idle);
					break;
				case 1:
					this._setAnimationId("preattack");
					this.setAnimState(Unit.AnimState.PrePreAttack);
					break;
				case 2:
					this._setAnimationId("charge");
					this.setAnimState(Unit.AnimState.PreAttack);
					break;
				case 3:
					this._setAnimationId(this.attackNamer.getAttackName());
					if (!this.animPlayer.hasLocator(new string[]
					{
						Unit.LocatorHit,
						Unit.LocatorProjectile
					}) || this.startAttackSoundEarly())
					{
						this.playAttackSound();
					}
					this.setAnimState(Unit.AnimState.Attack);
					break;
				case 4:
					this.setAnimState(Unit.AnimState.ProjectileAttack);
					break;
				case 5:
					this._setAnimationId("retreat");
					this.setAnimState(Unit.AnimState.Back);
					break;
				case 6:
					this._setAnimationId("gethit");
					this.damage(1);
					break;
				case 7:
					if (this.animPlayer.hasAnimationId("action"))
					{
						this.isAttackDone = false;
					}
					else
					{
						this.postAttackEffectDone();
					}
					this._setAnimationId("action");
					this.setAnimState(Unit.AnimState.Action);
					if (!this.animPlayer.hasLocator(new string[]
					{
						Unit.LocatorAction,
						Unit.LocatorAbility
					}) || this.startAbilitySoundEarly())
					{
						this.playOptionalTagSound("sound_ability");
					}
					break;
				}
			}
		}
	}

	// Token: 0x06000671 RID: 1649 RVA: 0x00006152 File Offset: 0x00004352
	private bool startAttackSoundEarly()
	{
		if (App.useExternalResources())
		{
			this.cardType.refreshTagsFromDisk();
		}
		return this.cardType.getTag<string>("sound_attack_start", "hit").ToLower() == "init";
	}

	// Token: 0x06000672 RID: 1650 RVA: 0x0000618D File Offset: 0x0000438D
	private bool startAbilitySoundEarly()
	{
		if (App.useExternalResources())
		{
			this.cardType.refreshTagsFromDisk();
		}
		return this.cardType.getTag<string>("sound_ability_start", "init").ToLower() == "init";
	}

	// Token: 0x06000673 RID: 1651 RVA: 0x000061C8 File Offset: 0x000043C8
	public void damage(int damageType)
	{
		this._hitTime = Time.time;
		this._hitTimeMax = 0.1f;
		if (damageType == 2)
		{
			this._hitTime = -99999.9f;
			this._hitTimeMax = 0.5f;
		}
	}

	// Token: 0x06000674 RID: 1652 RVA: 0x0003D01C File Offset: 0x0003B21C
	private void removeSummonAnims(bool removeEndAnims)
	{
		foreach (GameObject gameObject in this.summonAnims)
		{
			if (gameObject)
			{
				EffectPlayer component = gameObject.GetComponent<EffectPlayer>();
				if (removeEndAnims || !component.effectName.Contains("-end"))
				{
					Object.Destroy(gameObject);
				}
			}
		}
		this.summonAnims.Clear();
	}

	// Token: 0x06000675 RID: 1653 RVA: 0x0003D0B8 File Offset: 0x0003B2B8
	public void effectAnimDone(EffectPlayer effect, bool loop)
	{
		if (this.updateSummonAnim(effect))
		{
			return;
		}
		if (effect.effectName.StartsWith("fx_"))
		{
			Object.Destroy(effect.gameObject);
			return;
		}
		if (effect.effectName == "Poison" && !this.hasPoisonEnchantment())
		{
			Object.Destroy(effect.gameObject);
			this.poisonAnims.Clear();
			return;
		}
		DefaultIEffectCallback.instance().effectAnimDone(effect, loop);
	}

	// Token: 0x06000676 RID: 1654 RVA: 0x0003D138 File Offset: 0x0003B338
	private void playSummonEffect(string id, int relativeRenderQ)
	{
		Vector3 vector = base.transform.position + new Vector3(0.15f, 0f, 0f);
		int renQ = this.currentRenderQueueWithOffset(relativeRenderQ);
		this.summonAnims.Add(this.createEffectAnimation(id, renQ, vector, this.directionMod, 0.5f, false, id, 0));
	}

	// Token: 0x06000677 RID: 1655 RVA: 0x0003D194 File Offset: 0x0003B394
	private bool updateSummonAnim(EffectPlayer effect)
	{
		if (this.summonState == Unit.SummonState.Done || this.summonState == Unit.SummonState.None)
		{
			return false;
		}
		string effectName = effect.effectName;
		if (this.summonState == Unit.SummonState.PlayDone && "summon1-front-end" == effectName)
		{
			this.removeSummonAnims(true);
			this.effectDone();
			this.summonState++;
		}
		if (this.summonState == Unit.SummonState.PlayLoop && ("summon1-front-start" == effectName || "summon1-loop" == effectName))
		{
			if (this.previewLoaded || this.animLoaded)
			{
				this.summonState++;
			}
			else
			{
				this.removeSummonAnims(false);
				this.playSummonEffect("summon1-loop", 1);
			}
		}
		if (this.summonState == Unit.SummonState.PlayEnd)
		{
			base.renderer.enabled = true;
			this.removeSummonAnims(false);
			this.playSummonEffect("summon1-front-end", 1);
			this.playSummonEffect("summon1-back-end", -1);
			this.summonState++;
			this.playOptionalTagSound("sound_summon");
		}
		return true;
	}

	// Token: 0x06000678 RID: 1656 RVA: 0x000061FD File Offset: 0x000043FD
	private void effectDone()
	{
		if (this.callBackTarget != null)
		{
			this.callBackTarget.effectDone();
		}
	}

	// Token: 0x06000679 RID: 1657 RVA: 0x0003D2B4 File Offset: 0x0003B4B4
	public void animationDone(AnimPlayer p)
	{
		switch (this.currentAnimState)
		{
		case Unit.AnimState.Idle:
			this.playAnimation("Idle");
			break;
		case Unit.AnimState.PrePreAttack:
			this.startAttack();
			break;
		case Unit.AnimState.PreAttack:
			this.playAnimation("PreAttack");
			break;
		case Unit.AnimState.Attack:
			if (this.isProjectile || this.unitAnimationOnly)
			{
				if (this.unitAnimationOnly)
				{
					this.attackFullyCompleted();
				}
				this.playAnimation("Idle");
			}
			else
			{
				this.tryPostAttackEffectDone();
				this.attackDone();
			}
			break;
		case Unit.AnimState.ProjectileAttack:
			this.playAnimation("Back");
			break;
		case Unit.AnimState.Damage:
			this.playAnimation("Idle");
			break;
		case Unit.AnimState.Action:
			this.playAnimation("Idle");
			this.tryPostAttackEffectDone();
			break;
		}
	}

	// Token: 0x0600067A RID: 1658 RVA: 0x00006215 File Offset: 0x00004415
	private void setAnimState(Unit.AnimState newState)
	{
		this.currentAnimState = newState;
		this._animationDone = (newState == Unit.AnimState.Idle);
	}

	// Token: 0x0600067B RID: 1659 RVA: 0x00006228 File Offset: 0x00004428
	public bool isAnimationDone()
	{
		return this._animationDone;
	}

	// Token: 0x0600067C RID: 1660 RVA: 0x00006230 File Offset: 0x00004430
	public void locator(EffectPlayer effect, AnimLocator loc)
	{
		this.locator(null, loc);
	}

	// Token: 0x0600067D RID: 1661 RVA: 0x0003D3A4 File Offset: 0x0003B5A4
	private bool isLastLocatorOfType(FrameAnimation anim, AnimLocator loc)
	{
		List<AnimLocator> locatorsUpToFrame = anim.getLocatorsUpToFrame(int.MaxValue);
		return !Enumerable.Any<AnimLocator>(locatorsUpToFrame, (AnimLocator a) => a.name == loc.name);
	}

	// Token: 0x0600067E RID: 1662 RVA: 0x0003D3E0 File Offset: 0x0003B5E0
	public void locator(AnimPlayer p, AnimLocator loc)
	{
		if (loc.name == "ability" || loc.name == "action")
		{
			if (!this.startAbilitySoundEarly())
			{
				this.playOptionalTagSound("sound_ability");
			}
			this.tryPostAttackEffectDone();
		}
		if (loc.name == "screenshake" && this.callBackTarget != null)
		{
			this.callBackTarget.cameraShake(0.5f);
		}
		if (loc.name.StartsWith("fx_"))
		{
			int num = loc.name.IndexOf(':');
			string asset = (num >= 0) ? loc.name.Substring(3, num - 3) : loc.name.Substring(3);
			string text = (num >= 0) ? loc.name.Substring(num + 1) : null;
			int renQ = this.currentRenderQueueWithOffset(1);
			GameObject gameObject = this.createEffectAnimation(asset, renQ, base.transform.position, loc, this.directionMod, this.BaseScale, false, 0);
			if (text != null)
			{
				EffectPlayer component = gameObject.GetComponent<EffectPlayer>();
				if (component != null)
				{
					component.getAnimPlayer().setAnimationId(text);
				}
			}
			gameObject.transform.Translate(this.getDrawOffset());
		}
		if (loc.name.StartsWith("pfx_"))
		{
			Unit.ParticleSystemData particleSystemData = this.currentAnimPsds.Find((Unit.ParticleSystemData x) => x.name == loc.name);
			if (particleSystemData == null)
			{
				string text2 = loc.name.Substring(4);
				string text3 = text2;
				GameObject gameObject2 = (GameObject)Object.Instantiate(ResourceManager.Load("@Particles/" + text2 + "/" + text3, typeof(GameObject)));
				ParticleSystem component2 = gameObject2.GetComponent<ParticleSystem>();
				if (component2 == null)
				{
					Object.Destroy(gameObject2);
					return;
				}
				GameObject gameObject3 = gameObject2;
				int effectRenderQueue = this.calculateRenderQueue() + 1;
				UnityUtil.traverse(gameObject3, delegate(GameObject x)
				{
					if (x.renderer != null)
					{
						int renderQueue = (!(x.name == "Explosion 01")) ? effectRenderQueue : (effectRenderQueue + 1);
						x.renderer.material.renderQueue = renderQueue;
					}
				});
				Object.Destroy(gameObject3, 5f);
				particleSystemData = new Unit.ParticleSystemData(loc.name, gameObject3, component2);
				this.currentAnimPsds.Add(particleSystemData);
			}
			if (particleSystemData != null)
			{
				GameObject g = particleSystemData.g;
				this.positionAtLocator(g, loc);
				g.transform.localEulerAngles = new Vector3(0f, 0f, 51f);
				g.transform.Translate(this.getDrawOffset());
				if (this.directionMod > 0)
				{
					g.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
				}
			}
		}
		if (loc.name == Unit.LocatorHit)
		{
			if (!this.startAttackSoundEarly())
			{
				this.playAttackSound();
			}
			if (this.isLastLocatorOfType(p.getFrameAnimation(), loc))
			{
				this.tryPostAttackEffectDone();
			}
			else if (this.callBackTarget != null)
			{
				this.callBackTarget.playHitAnimation();
			}
		}
		if (loc.name == Unit.LocatorProjectile)
		{
			if (!this.startAttackSoundEarly())
			{
				this.playAttackSound();
			}
			GameObject gameObject4 = new GameObject();
			gameObject4.name = "Projectile";
			this.positionAtLocator(gameObject4, loc);
			Projectile projectile = gameObject4.AddComponent<Projectile>();
			projectile.init(this.projectileAttackTo, this, this.isSiege(), this.isFastProjectile);
			this.projectiles.Add(gameObject4);
		}
	}

	// Token: 0x0600067F RID: 1663 RVA: 0x0003D7D4 File Offset: 0x0003B9D4
	private void positionAtLocator(GameObject g, AnimLocator loc)
	{
		g.transform.eulerAngles = new Vector3(51f, 270f, 0f);
		g.transform.position = base.transform.position;
		Vector3 vector;
		vector..ctor((float)this.directionMod * loc.pos.x * this.BaseScale, -loc.pos.y * this.BaseScale, 0f);
		g.transform.Translate(vector * this.baseScaleMult);
	}

	// Token: 0x06000680 RID: 1664 RVA: 0x0003D868 File Offset: 0x0003BA68
	private Vector3 getDrawOffset()
	{
		float num = this.baseScaleMult * this.scalePosModifier.scale;
		ScalePosModifier scalePosModifier = this.scalePosModifier;
		return new Vector3(num * scalePosModifier.offset.x * num, num * (0.4f + scalePosModifier.offset.y), 0.02f * (float)this.pos.column);
	}

	// Token: 0x06000681 RID: 1665 RVA: 0x0000623A File Offset: 0x0000443A
	private bool isProjectileActive()
	{
		return this.projectiles.Count != 0;
	}

	// Token: 0x06000682 RID: 1666 RVA: 0x0003D8C8 File Offset: 0x0003BAC8
	private bool isSiege()
	{
		return this.cardType.hasTag("unit_siege") || (!this.cardType.hasTag("unit_no_siege") && this.isProjectile && this.cardType.kind == CardType.Kind.STRUCTURE);
	}

	// Token: 0x06000683 RID: 1667 RVA: 0x0000624D File Offset: 0x0000444D
	private void tryPostAttackEffectDone()
	{
		if (!this.isAttackDone)
		{
			this.postAttackEffectDone();
		}
	}

	// Token: 0x06000684 RID: 1668 RVA: 0x00006260 File Offset: 0x00004460
	private void postAttackEffectDone()
	{
		this.effectDone();
		this.isAttackDone = true;
	}

	// Token: 0x06000685 RID: 1669 RVA: 0x0003D920 File Offset: 0x0003BB20
	private void Update()
	{
		this.handleEditorKeyInput();
		this.renderUnit();
		this.renderProjectiles();
		this.setHasGlowingBuff(this.pendingBuffLevel);
		if (this.flashTimer != -1f && Time.time - this.flashTimer > 2f)
		{
			float num = 1f - 4f * (Time.time - this.flashTimer - 2f);
			this.statsSetAlpha(Mathf.Clamp01(num));
			if (this.flashTimer != -1f && Time.time - this.flashTimer > 2.5f)
			{
				this.showStats(false);
			}
		}
	}

	// Token: 0x06000686 RID: 1670 RVA: 0x0003D9CC File Offset: 0x0003BBCC
	private void handleEditorKeyInput()
	{
		if (!Application.isEditor)
		{
			return;
		}
		if (UnityUtil.GetKeyDown(114, new KeyCode[]
		{
			308
		}))
		{
			this.doForAllStatsObjs(false, delegate(GameObject x)
			{
				Object.Destroy(x);
			});
			this.hitPointsObjArr.Clear();
			this.attackPowerObjArr.Clear();
			this.attackCounterObjArr.Clear();
			this.statsNumsArr.Clear();
			this.statsNumsArr.Add(-10);
			this.statsNumsArr.Add(-10);
			this.statsNumsArr.Add(-10);
		}
	}

	// Token: 0x06000687 RID: 1671 RVA: 0x0003DA78 File Offset: 0x0003BC78
	private float calculateUpdatedGrowScale(bool isAp)
	{
		return 1f;
	}

	// Token: 0x06000688 RID: 1672 RVA: 0x0003DA8C File Offset: 0x0003BC8C
	private void renderProjectiles()
	{
		Vector3 vector;
		vector..ctor((float)this.directionMod * this.BaseScale, this.BaseScale, this.BaseScale);
		vector *= this.scalePosModifier.scale * 1.1f;
		foreach (GameObject gameObject in this.projectiles)
		{
			Vector3 position = gameObject.transform.position;
			Vector3 drawOffset = this.getDrawOffset();
			gameObject.transform.Translate(drawOffset);
			this._projectileAnim.Update(gameObject.transform, this.normalMaterial, vector);
			gameObject.transform.position = position;
		}
	}

	// Token: 0x06000689 RID: 1673 RVA: 0x0003DB60 File Offset: 0x0003BD60
	private void renderUnit()
	{
		if ((!this.animLoaded && !this.previewLoaded) || !base.renderer.enabled)
		{
			return;
		}
		this.animPlayer.UpdateOnly();
		Vector3 vector;
		vector..ctor((float)this.directionMod * this.BaseScale, this.BaseScale, this.BaseScale);
		vector *= this.scalePosModifier.scale * 1.1f;
		string text = "ap";
		if (this.cardType.getTag<string>("grower", ref text))
		{
			vector *= this.calculateUpdatedGrowScale(text.ToLower() != "hp");
		}
		Material material = this.normalMaterial;
		Vector3 position = base.transform.position;
		base.transform.Translate(this.getDrawOffset());
		if (this._frostWindTime != 0f)
		{
			if (this._frostWindTime == -99999.9f)
			{
				this._frostWindTime = Time.time;
			}
			float num = Time.time - this._frostWindTime;
			float num2 = 3f;
			if (num > num2)
			{
				this._frostWindTime = 0f;
				this.animPlayer.setSpeed(1f);
			}
			else if (num >= 0f)
			{
				material = this.frostWindMaterial;
				material.renderQueue = this.currentRenderQueue();
				material.mainTexture = this.normalMaterial.mainTexture;
				float x = num / num2;
				float num3 = Mth.linearUFD(x, 0.025f, 0.3f);
				float num4 = num3;
				this.animPlayer.setSpeed(1f - num4);
				material.color = new Color(num4, num4, 1f, 1f);
				material.SetFloat("_Lerp", num4);
			}
		}
		else if (this._hitTime != 0f)
		{
			if (this._hitTime == -99999.9f)
			{
				this._hitTime = Time.time;
			}
			float num5 = Time.time - this._hitTime;
			if (num5 > this._hitTimeMax)
			{
				this._hitTime = 0f;
			}
			else
			{
				material = this.hitMaterial;
				material.renderQueue = this.currentRenderQueue();
				material.mainTexture = this.normalMaterial.mainTexture;
				float num6 = num5 / this._hitTimeMax;
				float num7 = 4f * (num6 - num6 * num6);
				material.SetFloat("_Lerp", num7);
			}
		}
		else if (this._healTime != 0f)
		{
			float num8 = Time.time - this._healTime;
			if (num8 > 0f)
			{
				float num9 = 0.3f;
				if (num8 > num9)
				{
					this._healTime = 0f;
				}
				else
				{
					material = this.healMaterial;
					material.renderQueue = this.currentRenderQueue();
					material.mainTexture = this.normalMaterial.mainTexture;
					float num10 = num8 / num9;
					float num11 = 1f - num10;
					material.SetFloat("_Lerp", num11);
				}
			}
		}
		if (this.buffLevel >= Unit.BuffLevel.Glow)
		{
			this.buffMaterial.renderQueue = this.currentRenderQueueWithOffset(-1);
			this.buffMaterial.mainTexture = this.normalMaterial.mainTexture;
			float num12 = 1.2f;
			float num13 = Time.time % num12 / num12;
			Color color = Color.Lerp(Unit.enchantmentStartColor, Unit.enchantmentEndColor, num13);
			color.a = 1f - num13;
			this.buffMaterial.SetColor("_Color", color);
			float num14 = -0.09f * num13;
			Vector3 scale = vector * (1f + 0.175f * num13);
			base.transform.Translate(0f, -num14 * this.baseScaleMult, 0f);
			this.animPlayer.Draw(base.transform, this.buffMaterial, scale);
			base.transform.Translate(0f, num14 * this.baseScaleMult, 0f);
		}
		Vector3 position2 = base.transform.position;
		base.transform.Translate(this.getDrawOffset() * -0.675f);
		this.animPlayer.Draw(base.transform, this.shadowMaterial, new Vector3(vector.x, 0.5f * vector.y, vector.z));
		base.transform.position = position2;
		this.animPlayer.Draw(base.transform, material, vector);
		if (this.buffLevel >= Unit.BuffLevel.Glow)
		{
			float num15 = 0.5f + 0.5f * Mathf.Sin(6.28f * (Time.time % 1.2f) / 1.2f);
			this.enchantmentMaterial.SetFloat("_Lerp", 0.15f * num15);
			base.transform.Translate(0f, 0f, -0.01f);
			this.animPlayer.Draw(base.transform, this.enchantmentMaterial, vector);
			base.transform.Translate(0f, 0f, 0.01f);
		}
		base.transform.position = position;
	}

	// Token: 0x0600068A RID: 1674 RVA: 0x0000626F File Offset: 0x0000446F
	public void alwaysShowStats(bool always)
	{
		this._alwaysShowStats = always;
		this.showStats(always);
	}

	// Token: 0x0600068B RID: 1675 RVA: 0x0003E078 File Offset: 0x0003C278
	public void projectileDone(Projectile projectile, Vector3 landAt)
	{
		try
		{
			this.projectiles.Remove(projectile.gameObject);
			Object.Destroy(projectile);
			this.tryPostAttackEffectDone();
			if (!this.unitAnimationOnly)
			{
				if (!(this.cardType.name == "Magnetizer"))
				{
					this.playSound(this.trSound.get("sound_impact", "Sounds/explosion"));
					if (this.isSiege())
					{
						this.callBackTarget.cameraShake(1f);
					}
					int renderQueue = this.normalMaterial.renderQueue;
					bool flag = this.isSiege();
					string text = null;
					if (this.cardType.getTag<string>("anim_hit", ref text))
					{
						string[] array = text.Split(new char[]
						{
							','
						});
						foreach (string text2 in array)
						{
							Log.warning("Anim-name: " + text2);
							this.createEffectAnimation(text2, renderQueue + 1, landAt + new Vector3(0f, 0.25f, 0f), this.directionMod, 0.15f, false, string.Empty, 0);
						}
					}
					else if (this.cardType.name == "Destroyer")
					{
						this.createEffectAnimation("transp", renderQueue - 2, landAt, this.directionMod, 0.25f, false, string.Empty, 0);
						this.createEffectAnimation("MushroomExplosion1/back", renderQueue - 1, landAt, this.directionMod, 0.25f, false, string.Empty, 0);
						this.createEffectAnimation("MushroomExplosion1/mid", renderQueue + 1, landAt, this.directionMod, 0.25f, false, string.Empty, 0);
						this.createEffectAnimation("MushroomExplosion1/front", renderQueue + 2, landAt, this.directionMod, 0.25f, false, string.Empty, 0);
					}
					else if (this.cardType.name == "Hellspitter Mortar")
					{
						landAt..ctor(landAt.x, landAt.y + 0.4f, landAt.z);
						this.createEffectAnimation("transp", renderQueue - 2, landAt, this.directionMod, 0.15f, false, string.Empty, 0);
						this.createEffectAnimation("gunpowder_explosion_1", renderQueue + 1, landAt, this.directionMod, 0.117f, false, string.Empty, 0);
					}
					else if (this.cardType.name == "Catapult of Goo")
					{
						flag = false;
						this.createEffectAnimation("transp", renderQueue - 2, landAt, this.directionMod, 0.15f, false, string.Empty, 0);
						this.createEffectAnimation("slime", renderQueue + 1, landAt, this.directionMod, 0.5f, false, string.Empty, 0);
					}
					else
					{
						this.createEffectAnimation("hit_effects", renderQueue + 1, landAt, this.directionMod, 0.65f, false, string.Empty, 0);
					}
					if (flag)
					{
						this.createEffectAnimation("dust-puff-big", renderQueue + 1, landAt, this.directionMod, 0.25f, false, string.Empty, 0);
					}
				}
			}
		}
		catch (Exception ex)
		{
			Log.info("Error: " + ex);
		}
	}

	// Token: 0x0600068C RID: 1676 RVA: 0x0003E3CC File Offset: 0x0003C5CC
	private GameObject createEffectAnimation(string asset, int renQ, Vector3 pos, int dirMod, float baseScale, bool loop, string name, int animID)
	{
		AnimLocator locator = new AnimLocator(name, 0);
		return this.createEffectAnimation(asset, renQ, pos, locator, dirMod, baseScale, loop, animID);
	}

	// Token: 0x0600068D RID: 1677 RVA: 0x0003E3F4 File Offset: 0x0003C5F4
	private GameObject createEffectAnimation(string asset, int renQ, Vector3 pos, AnimLocator locator, int dirMod, float baseScale, bool loop, int animID)
	{
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<MeshRenderer>();
		EffectPlayer effectPlayer = gameObject.AddComponent<EffectPlayer>();
		AnimLocator.Pos pos2 = locator.pos;
		Vector3 baseScale2 = Vector3.one * baseScale;
		baseScale2.x *= pos2.sx;
		baseScale2.y *= pos2.sy;
		effectPlayer.init(asset, dirMod, this, renQ, baseScale2, loop, locator.name, animID);
		gameObject.transform.eulerAngles = new Vector3(51f, 270f, (locator.pos != null) ? (locator.pos.rotation * (float)dirMod) : 0f);
		gameObject.transform.position = pos;
		Vector3 vector;
		vector..ctor((float)dirMod * locator.pos.x * baseScale, -locator.pos.y * baseScale, 0f);
		gameObject.transform.Translate(vector);
		return gameObject;
	}

	// Token: 0x0600068E RID: 1678 RVA: 0x0003E4F4 File Offset: 0x0003C6F4
	private void OnDestroy()
	{
		foreach (GameObject gameObject in this.summonAnims)
		{
			Object.DestroyImmediate(gameObject);
		}
		Object.Destroy(this.iconFrame);
		Object.Destroy(this.attackSlowdownDummy);
	}

	// Token: 0x0600068F RID: 1679 RVA: 0x0000627F File Offset: 0x0000447F
	public float getAlpha()
	{
		return this._alpha;
	}

	// Token: 0x06000690 RID: 1680 RVA: 0x0003E564 File Offset: 0x0003C764
	public void setAlpha(float a)
	{
		this._alpha = a;
		base.renderer.material.color = ColorUtil.GetWithAlpha(base.renderer.material.color, a);
		this.shadowMaterial.color = ColorUtil.GetWithAlpha(this.shadowMaterial.color, a);
	}

	// Token: 0x06000691 RID: 1681 RVA: 0x00006287 File Offset: 0x00004487
	public void setZPos(float zPos)
	{
		this.zPos = zPos;
	}

	// Token: 0x06000692 RID: 1682 RVA: 0x00006290 File Offset: 0x00004490
	public TilePosition getTilePosition()
	{
		return this.pos;
	}

	// Token: 0x06000693 RID: 1683 RVA: 0x00006298 File Offset: 0x00004498
	public void setTilePosition(TilePosition pos)
	{
		this.pos = pos;
		this.refreshRenderQueues();
	}

	// Token: 0x06000694 RID: 1684 RVA: 0x000062A7 File Offset: 0x000044A7
	private void refreshRenderQueues()
	{
		this._currentRenderQueue = this.calculateRenderQueue();
		this.normalMaterial.renderQueue = this._currentRenderQueue;
		this.enchantmentMaterial.renderQueue = this._currentRenderQueue;
		this.updateParticleSystemRenderQueue(this.enchantedParticleSystem);
	}

	// Token: 0x06000695 RID: 1685 RVA: 0x000062E3 File Offset: 0x000044E3
	private int currentRenderQueue()
	{
		return this.currentRenderQueueWithOffset(0);
	}

	// Token: 0x06000696 RID: 1686 RVA: 0x000062EC File Offset: 0x000044EC
	private int currentRenderQueueWithOffset(int offset)
	{
		return this._currentRenderQueue + offset;
	}

	// Token: 0x06000697 RID: 1687 RVA: 0x000062F6 File Offset: 0x000044F6
	public void heal()
	{
		this._healTime = Time.time + 0.3f;
	}

	// Token: 0x06000698 RID: 1688 RVA: 0x00006309 File Offset: 0x00004509
	public void frostWind()
	{
		this._frostWindTime = Time.time + 0.3f;
	}

	// Token: 0x06000699 RID: 1689 RVA: 0x0000631C File Offset: 0x0000451C
	public void setHitPoints(int hitPoints)
	{
		this.hitPoints = hitPoints;
		this.statsNumberHitPoints();
	}

	// Token: 0x0600069A RID: 1690 RVA: 0x0000632B File Offset: 0x0000452B
	public bool setAttackCounter(int attackCounter)
	{
		this.attackCounter = attackCounter;
		this.statsNumberAttackCounter();
		return true;
	}

	// Token: 0x0600069B RID: 1691 RVA: 0x0000633B File Offset: 0x0000453B
	public void setAttackPower(int attackPower)
	{
		this.attackPower = attackPower;
		this.statsNumberAttackPower();
	}

	// Token: 0x0600069C RID: 1692 RVA: 0x0000634A File Offset: 0x0000454A
	public int getHitPoints()
	{
		return this.hitPoints;
	}

	// Token: 0x0600069D RID: 1693 RVA: 0x00006352 File Offset: 0x00004552
	public int getAttackPower()
	{
		return this.attackPower;
	}

	// Token: 0x0600069E RID: 1694 RVA: 0x0000635A File Offset: 0x0000455A
	public int getAttackInterval()
	{
		return this.attackCounter;
	}

	// Token: 0x0600069F RID: 1695 RVA: 0x00006362 File Offset: 0x00004562
	public int getOrigAttackInterval()
	{
		return this.cardType.ac;
	}

	// Token: 0x060006A0 RID: 1696 RVA: 0x0000636F File Offset: 0x0000456F
	public string getName()
	{
		return this.cardType.name;
	}

	// Token: 0x060006A1 RID: 1697 RVA: 0x0000637C File Offset: 0x0000457C
	public CardType.Kind getPieceKind()
	{
		return this.cardType.kind;
	}

	// Token: 0x060006A2 RID: 1698 RVA: 0x00006389 File Offset: 0x00004589
	public bool isType(string s)
	{
		return this.cardType.types.isType(s);
	}

	// Token: 0x060006A3 RID: 1699 RVA: 0x0000639C File Offset: 0x0000459C
	public bool isCountingDownAutomatically()
	{
		return !this.cardType.hasTag("unit_no_countdown");
	}

	// Token: 0x060006A4 RID: 1700 RVA: 0x000063B1 File Offset: 0x000045B1
	public ActiveAbility[] getActiveAbilities()
	{
		return this.cardType.abilities;
	}

	// Token: 0x060006A5 RID: 1701 RVA: 0x000063BE File Offset: 0x000045BE
	public TargetArea getTargetArea()
	{
		return this.cardType.targetArea;
	}

	// Token: 0x060006A6 RID: 1702 RVA: 0x000063CB File Offset: 0x000045CB
	public Card getCard()
	{
		return this.card;
	}

	// Token: 0x060006A7 RID: 1703 RVA: 0x0003E5BC File Offset: 0x0003C7BC
	public void setBuffs(EnchantmentInfo[] infos)
	{
		this.removeAllBuffs();
		if (infos == null)
		{
			return;
		}
		foreach (EnchantmentInfo info in infos)
		{
			this.addBuff(info);
		}
	}

	// Token: 0x060006A8 RID: 1704 RVA: 0x0003E5F8 File Offset: 0x0003C7F8
	private void addBuff(EnchantmentInfo info)
	{
		this.enchantmentInfos.Add(info);
		if (Unit.shouldGiveShield(info))
		{
			this.healthStat.showIconBackground(true);
		}
		if (info.name == "Poison" && this.poisonAnims.Count == 0)
		{
			this.playPoisonAnim();
		}
		if (info.type == EnchantmentType.ENCHANTMENT)
		{
			this.pendingBuffLevel = Unit.BuffLevel.Particles;
		}
		else if (info.type == EnchantmentType.BUFF && this.pendingBuffLevel == Unit.BuffLevel.None)
		{
			this.pendingBuffLevel = Unit.BuffLevel.Glow;
		}
	}

	// Token: 0x060006A9 RID: 1705 RVA: 0x000059E4 File Offset: 0x00003BE4
	private bool alwaysShowShield()
	{
		return false;
	}

	// Token: 0x060006AA RID: 1706 RVA: 0x000059E4 File Offset: 0x00003BE4
	private static bool shouldGiveShield(EnchantmentInfo info)
	{
		return false;
	}

	// Token: 0x060006AB RID: 1707 RVA: 0x000063D3 File Offset: 0x000045D3
	private void setHasGlowingBuff(Unit.BuffLevel level)
	{
		if (this.buffLevel == level)
		{
			return;
		}
		this.setHasBuffParticleSystem(level == Unit.BuffLevel.Particles);
		this.buffLevel = level;
	}

	// Token: 0x060006AC RID: 1708 RVA: 0x0003E688 File Offset: 0x0003C888
	private void setHasBuffParticleSystem(bool enabled)
	{
		if (enabled == (this.buffLevel == Unit.BuffLevel.Particles))
		{
			return;
		}
		if (enabled)
		{
			float num = 0.015384615f;
			GameObject gameObject = new GameObject("particleHolder");
			GameObject child = (GameObject)Object.Instantiate(ResourceManager.Load("@Particles/EnchantmentParticlePrefab"));
			UnityUtil.addChild(gameObject, child);
			UnityUtil.addChild(base.gameObject, gameObject);
			gameObject.transform.localEulerAngles = new Vector3(270f, 0f, 0f);
			gameObject.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f) * num;
			this.enchantedParticleSystem = gameObject;
			this.updateParticleSystemRenderQueue(this.enchantedParticleSystem);
		}
		else
		{
			Object.Destroy(this.enchantedParticleSystem);
			this.enchantedParticleSystem = null;
		}
	}

	// Token: 0x060006AD RID: 1709 RVA: 0x0003E754 File Offset: 0x0003C954
	private void updateParticleSystemRenderQueue(GameObject hierarchy)
	{
		if (hierarchy == null)
		{
			return;
		}
		int renderQueue = this.calculateRenderQueue() + 1;
		UnityUtil.traverse(hierarchy, delegate(GameObject x)
		{
			if (x.renderer != null)
			{
				x.renderer.material.renderQueue = renderQueue;
			}
		});
	}

	// Token: 0x060006AE RID: 1710 RVA: 0x0003E794 File Offset: 0x0003C994
	private void removeAllBuffs()
	{
		this.enchantmentInfos.Clear();
		this.pendingBuffLevel = Unit.BuffLevel.None;
		this.healthStat.showIconBackground(this.alwaysShowShield());
		base.renderer.material.SetFloat("_Lerp", 0f);
		base.renderer.material.color = Color.white;
		if (this.buffIcon != null)
		{
			Object.Destroy(this.buffIcon);
			this.buffIcon = null;
		}
		if (this.deBuffIcon != null)
		{
			Object.Destroy(this.deBuffIcon);
			this.deBuffIcon = null;
		}
	}

	// Token: 0x060006AF RID: 1711 RVA: 0x0003E83C File Offset: 0x0003CA3C
	private void playPoisonAnim()
	{
		this.poisonAnims.Clear();
		this.poisonAnims.Add(this.createEffectAnimation("poison-front", this.currentRenderQueueWithOffset(1), new Vector3(base.transform.position.x, base.transform.position.y + 0.35f, base.transform.position.z), this.directionMod, 0.45f, true, "Poison", 0));
		this.poisonAnims.Add(this.createEffectAnimation("poison-back", this.currentRenderQueueWithOffset(-1), new Vector3(base.transform.position.x, base.transform.position.y + 0.35f, base.transform.position.z), this.directionMod, 0.45f, true, "Poison", 0));
		foreach (GameObject gameObject in this.poisonAnims)
		{
			gameObject.transform.parent = base.transform;
		}
	}

	// Token: 0x060006B0 RID: 1712 RVA: 0x0003E998 File Offset: 0x0003CB98
	public void playEnchantmentAnim(Tags enchantTags)
	{
		if (enchantTags.has("curse"))
		{
			this.createEffectAnimation("curse", this.currentRenderQueueWithOffset(1), new Vector3(base.transform.position.x - 0.15f, base.transform.position.y + 0.25f, base.transform.position.z), this.directionMod, 0.15f, false, string.Empty, 0);
		}
		else
		{
			this.createEffectAnimation("enchantment-front", this.currentRenderQueueWithOffset(1), new Vector3(base.transform.position.x - 0.15f, base.transform.position.y + 0.25f, base.transform.position.z), this.directionMod, 0.3f, false, string.Empty, 0);
			this.createEffectAnimation("enchantment-back", this.currentRenderQueueWithOffset(-1), new Vector3(base.transform.position.x - 0.15f, base.transform.position.y + 0.25f, base.transform.position.z), this.directionMod, 0.3f, false, string.Empty, 0);
		}
	}

	// Token: 0x060006B1 RID: 1713 RVA: 0x000063F3 File Offset: 0x000045F3
	public List<EnchantmentInfo> getBuffs()
	{
		return this.enchantmentInfos;
	}

	// Token: 0x060006B2 RID: 1714 RVA: 0x0003EB10 File Offset: 0x0003CD10
	public bool hasTag(string tag)
	{
		if (this.cardType.hasTag(tag))
		{
			return true;
		}
		foreach (EnchantmentInfo enchantmentInfo in this.getBuffs())
		{
			CardType cardType = CardTypeManager.getInstance().get(enchantmentInfo.name);
			if (cardType != null && cardType.hasTag(tag))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060006B3 RID: 1715 RVA: 0x0003EBA4 File Offset: 0x0003CDA4
	private void playAttackSound()
	{
		string defaultName = (!this.isProjectile) ? "Sounds/hyperduck/attack_sword_02" : "Sounds/hyperduck/action_scattergun";
		TagSoundReader.Snd snd = this.trSound.get("sound_attack", defaultName);
		this.playSound(snd);
	}

	// Token: 0x060006B4 RID: 1716 RVA: 0x0003EBE8 File Offset: 0x0003CDE8
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

	// Token: 0x060006B5 RID: 1717 RVA: 0x000063FB File Offset: 0x000045FB
	private void playSound(TagSoundReader.Snd snd)
	{
		this.playSound(snd.name, snd.volume, snd.getPitch(), snd.delay);
	}

	// Token: 0x060006B6 RID: 1718 RVA: 0x0003EC7C File Offset: 0x0003CE7C
	private void playOptionalTagSound(string tagName)
	{
		TagSoundReader.Snd snd = this.trSound.get(tagName);
		if (snd)
		{
			this.playSound(snd);
		}
	}

	// Token: 0x060006B7 RID: 1719 RVA: 0x0000641B File Offset: 0x0000461B
	public AnimPlayer getAnimPlayer()
	{
		return this.animPlayer;
	}

	// Token: 0x060006B8 RID: 1720 RVA: 0x00006423 File Offset: 0x00004623
	private bool hasPoisonEnchantment()
	{
		return this.enchantmentInfos.Exists((EnchantmentInfo b) => b.name == "Poison");
	}

	// Token: 0x060006B9 RID: 1721 RVA: 0x0003ECA8 File Offset: 0x0003CEA8
	private IEnumerator downloadAssets()
	{
		if (!this.unitAnimationOnly)
		{
			this.playSummonEffect("summon1-front-start", 1);
			this.playSummonEffect("summon1-back-start", -1);
		}
		if (this.hasAnimationBundleOnDisk() && this.loadBundle())
		{
			yield break;
		}
		if (!this.hasPreviewImageOnDisk())
		{
			EnumeratorUtil.QueryValue<bool> status = new EnumeratorUtil.QueryValue<bool>();
			IEnumerator dl = Unit.downloadAndSave(StorageEnvironment.getPreviewPath(this.cardType), status);
			yield return base.StartCoroutine(dl);
		}
		this.setUnitAnim(PreviewImage.createPreviewAnimation(this.cardType));
		this.playAnimation("Idle");
		base.renderer.enabled = true;
		this.previewLoaded = true;
		if (this.cardTypeHasAnimation && !this.cardType.useDummyAnimationBundle())
		{
			EnumeratorUtil.QueryValue<bool> status2 = new EnumeratorUtil.QueryValue<bool>();
			IEnumerator dl2 = Unit.downloadAndSave(StorageEnvironment.getAnimationBundleZipPath(this.cardType), status2);
			yield return base.StartCoroutine(dl2);
		}
		this.loadBundle();
		yield break;
	}

	// Token: 0x060006BA RID: 1722 RVA: 0x0003ECC4 File Offset: 0x0003CEC4
	private bool hasPreviewImageOnDisk()
	{
		string previewPath = StorageEnvironment.getPreviewPath(this.cardType);
		string text = Unit.localPath.Invoke(previewPath);
		return File.Exists(text);
	}

	// Token: 0x060006BB RID: 1723 RVA: 0x0003ECF0 File Offset: 0x0003CEF0
	private bool hasAnimationBundleOnDisk()
	{
		string animationBundleZipPath = StorageEnvironment.getAnimationBundleZipPath(this.cardType);
		string text = Unit.localPath.Invoke(animationBundleZipPath);
		return File.Exists(text);
	}

	// Token: 0x060006BC RID: 1724 RVA: 0x0000644D File Offset: 0x0000464D
	private static IEnumerator downloadAndSave(string idfn)
	{
		return Unit.downloadAndSave(idfn, new EnumeratorUtil.QueryValue<bool>());
	}

	// Token: 0x060006BD RID: 1725 RVA: 0x0003ED1C File Offset: 0x0003CF1C
	private static IEnumerator downloadAndSave(string idfn, EnumeratorUtil.QueryValue<bool> b)
	{
		string url = Unit.remotePath.Invoke(idfn);
		WWW www = new WWW(url);
		while (!www.isDone)
		{
			yield return new WaitForEndOfFrame();
		}
		if (www.error != null)
		{
			b.value = new bool?(false);
			yield break;
		}
		FileUtil.saveContents(Unit.localPath.Invoke(idfn), www.bytes);
		b.value = new bool?(true);
		yield return null;
		yield break;
	}

	// Token: 0x060006BE RID: 1726 RVA: 0x0003ED4C File Offset: 0x0003CF4C
	public bool downloadAndSaveSync(string idfn)
	{
		EnumeratorUtil.QueryValue<bool> queryValue = new EnumeratorUtil.QueryValue<bool>();
		IEnumerator enumerator = Unit.downloadAndSave(idfn);
		while (enumerator.MoveNext())
		{
		}
		return queryValue.value != null && queryValue.value.Value;
	}

	// Token: 0x060006BF RID: 1727 RVA: 0x0000645A File Offset: 0x0000465A
	private bool downloadPreview()
	{
		return this.downloadAndSaveSync(StorageEnvironment.getPreviewPath(this.cardType));
	}

	// Token: 0x060006C0 RID: 1728 RVA: 0x0000646D File Offset: 0x0000466D
	private bool downloadBundle()
	{
		return this.downloadAndSaveSync(StorageEnvironment.getAnimationBundleZipPath(this.cardType));
	}

	// Token: 0x060006C1 RID: 1729 RVA: 0x0003ED98 File Offset: 0x0003CF98
	private string loadBundleText(string onlyFilename)
	{
		string text = StorageEnvironment.getAnimationBundleUnzipPath(this.cardType) + "/" + onlyFilename;
		string text2 = Unit.localPath.Invoke(text);
		return (!File.Exists(text2)) ? null : File.ReadAllText(text2);
	}

	// Token: 0x060006C2 RID: 1730 RVA: 0x0003EDE0 File Offset: 0x0003CFE0
	private Texture2D loadBundleImage(string onlyFilename)
	{
		string text = StorageEnvironment.getAnimationBundleUnzipPath(this.cardType) + "/" + onlyFilename;
		string text2 = Unit.localPath.Invoke(text);
		if (!File.Exists(text2))
		{
			return null;
		}
		Texture2D texture2D = ResourceManager.instance.tryGetTexture2D(text);
		if (texture2D)
		{
			return texture2D;
		}
		Texture2D texture2D2 = new Texture2D(8, 8, 5, false);
		byte[] array = File.ReadAllBytes(text2);
		if (!texture2D2.LoadImage(array))
		{
			return null;
		}
		ResourceManager.instance.assignTexture2D(text, texture2D2);
		return texture2D2;
	}

	// Token: 0x04000463 RID: 1123
	private const float TIME_STARTNEXT = -99999.9f;

	// Token: 0x04000464 RID: 1124
	public const int DAMAGETYPE_GETHIT = 1;

	// Token: 0x04000465 RID: 1125
	public const int DAMAGETYPE_SURGE = 2;

	// Token: 0x04000466 RID: 1126
	private const float _BASESCALE = 0.14285715f;

	// Token: 0x04000467 RID: 1127
	private AudioScript audioScript;

	// Token: 0x04000468 RID: 1128
	private GameObject attackSlowdownDummy;

	// Token: 0x04000469 RID: 1129
	private bool _isDestroyed;

	// Token: 0x0400046A RID: 1130
	public static Color shadowColor = ColorUtil.FromHex24(4006939u);

	// Token: 0x0400046B RID: 1131
	private bool flashDigits;

	// Token: 0x0400046C RID: 1132
	public Unit.ICallback callBackTarget;

	// Token: 0x0400046D RID: 1133
	private float zPos;

	// Token: 0x0400046E RID: 1134
	public int directionMod;

	// Token: 0x0400046F RID: 1135
	private float BaseScale = 0.14285715f;

	// Token: 0x04000470 RID: 1136
	private float baseScaleMult = 1f;

	// Token: 0x04000471 RID: 1137
	private TilePosition pos;

	// Token: 0x04000472 RID: 1138
	private bool showUnitStats;

	// Token: 0x04000473 RID: 1139
	private int hitPoints;

	// Token: 0x04000474 RID: 1140
	private int attackPower;

	// Token: 0x04000475 RID: 1141
	private int attackCounter;

	// Token: 0x04000476 RID: 1142
	private bool firstAttack = true;

	// Token: 0x04000477 RID: 1143
	private ScalePosModifier scalePosModifier;

	// Token: 0x04000478 RID: 1144
	private AnimPlayer animPlayer = new AnimPlayer(App.Clocks.animClock);

	// Token: 0x04000479 RID: 1145
	private bool animLoaded;

	// Token: 0x0400047A RID: 1146
	private bool previewLoaded;

	// Token: 0x0400047B RID: 1147
	private AnimPlayer _projectileAnim;

	// Token: 0x0400047C RID: 1148
	private List<GameObject> projectiles = new List<GameObject>();

	// Token: 0x0400047D RID: 1149
	private GameObject iconFrame;

	// Token: 0x0400047E RID: 1150
	private GameObject enchantedParticleSystem;

	// Token: 0x0400047F RID: 1151
	private Stat attackStat;

	// Token: 0x04000480 RID: 1152
	private Stat countdownStat;

	// Token: 0x04000481 RID: 1153
	private Stat healthStat;

	// Token: 0x04000482 RID: 1154
	private bool isProjectile;

	// Token: 0x04000483 RID: 1155
	private bool isFastProjectile;

	// Token: 0x04000484 RID: 1156
	private Vector3 attackTo;

	// Token: 0x04000485 RID: 1157
	private Vector3 projectileAttackTo;

	// Token: 0x04000486 RID: 1158
	private Unit.SummonState summonState = Unit.SummonState.PlayLoop;

	// Token: 0x04000487 RID: 1159
	private List<GameObject> summonAnims = new List<GameObject>();

	// Token: 0x04000488 RID: 1160
	private List<EnchantmentInfo> enchantmentInfos = new List<EnchantmentInfo>();

	// Token: 0x04000489 RID: 1161
	private Unit.BuffLevel buffLevel;

	// Token: 0x0400048A RID: 1162
	private Unit.BuffLevel pendingBuffLevel;

	// Token: 0x0400048B RID: 1163
	private bool hasArmor;

	// Token: 0x0400048C RID: 1164
	private GameObject buffIcon;

	// Token: 0x0400048D RID: 1165
	private GameObject deBuffIcon;

	// Token: 0x0400048E RID: 1166
	private Material numberMaterial;

	// Token: 0x0400048F RID: 1167
	private Material normalMaterial;

	// Token: 0x04000490 RID: 1168
	private Material hitMaterial;

	// Token: 0x04000491 RID: 1169
	private Material frostWindMaterial;

	// Token: 0x04000492 RID: 1170
	private Material healMaterial;

	// Token: 0x04000493 RID: 1171
	private Material buffMaterial;

	// Token: 0x04000494 RID: 1172
	private Material shadowMaterial;

	// Token: 0x04000495 RID: 1173
	private Material enchantmentMaterial;

	// Token: 0x04000496 RID: 1174
	private Card card;

	// Token: 0x04000497 RID: 1175
	private CardType cardType;

	// Token: 0x04000498 RID: 1176
	private TagSoundReader trSound;

	// Token: 0x04000499 RID: 1177
	private bool cardTypeHasAnimation;

	// Token: 0x0400049A RID: 1178
	private Unit.AnimNamer attackNamer;

	// Token: 0x0400049B RID: 1179
	private static readonly Color enchantmentStartColor = ColorUtil.FromInts(255, 255, 88);

	// Token: 0x0400049C RID: 1180
	private static readonly Color enchantmentEndColor = ColorUtil.FromInts(255, 75, 0);

	// Token: 0x0400049D RID: 1181
	public bool unitAnimationOnly;

	// Token: 0x0400049E RID: 1182
	private bool useUnlitShader;

	// Token: 0x0400049F RID: 1183
	public int overriddenRenderQueue = -1;

	// Token: 0x040004A0 RID: 1184
	private static readonly Material _defaultMaterial = new Material(ResourceManager.LoadShader("Scrolls/Transparent/Diffuse/Double/Color"));

	// Token: 0x040004A1 RID: 1185
	private static readonly Material _unlitMaterial = new Material(ResourceManager.LoadShader("Scrolls/Unlit/Transparent"));

	// Token: 0x040004A2 RID: 1186
	private FlavorTalk flavorTalk = FlavorTalk.empty;

	// Token: 0x040004A3 RID: 1187
	private bool hasCreatedFlavor;

	// Token: 0x040004A4 RID: 1188
	private static long totalTime;

	// Token: 0x040004A5 RID: 1189
	private string serializationType = string.Empty;

	// Token: 0x040004A6 RID: 1190
	private float flashTimer = -1f;

	// Token: 0x040004A7 RID: 1191
	private List<GameObject> attackPowerObjArr = new List<GameObject>();

	// Token: 0x040004A8 RID: 1192
	private List<GameObject> attackCounterObjArr = new List<GameObject>();

	// Token: 0x040004A9 RID: 1193
	private List<GameObject> hitPointsObjArr = new List<GameObject>();

	// Token: 0x040004AA RID: 1194
	private List<GameObject> statsboardSymbols = new List<GameObject>();

	// Token: 0x040004AB RID: 1195
	private List<int> statsNumsArr = new List<int>();

	// Token: 0x040004AC RID: 1196
	private int _attackDoneCounter;

	// Token: 0x040004AD RID: 1197
	private static readonly float DustDelay = 0.065f;

	// Token: 0x040004AE RID: 1198
	private static readonly Vector3 DustOffset = new Vector3(0f, 0.1f, 0f);

	// Token: 0x040004AF RID: 1199
	private static readonly float StopTime = 0.19f;

	// Token: 0x040004B0 RID: 1200
	private static readonly float StopDistance = 0.35f;

	// Token: 0x040004B1 RID: 1201
	private static readonly iTween.EaseType EaseType = iTween.EaseType.easeOutQuad;

	// Token: 0x040004B2 RID: 1202
	private bool _moving;

	// Token: 0x040004B3 RID: 1203
	private List<Unit.ParticleSystemData> currentAnimPsds = new List<Unit.ParticleSystemData>();

	// Token: 0x040004B4 RID: 1204
	private bool isAttackDone;

	// Token: 0x040004B5 RID: 1205
	private Unit.AnimState currentAnimState;

	// Token: 0x040004B6 RID: 1206
	private bool _animationDone;

	// Token: 0x040004B7 RID: 1207
	private float targetGrowScale = 1f;

	// Token: 0x040004B8 RID: 1208
	private float currentGrowScale = 1f;

	// Token: 0x040004B9 RID: 1209
	private bool _alwaysShowStats;

	// Token: 0x040004BA RID: 1210
	private float _alpha = 1f;

	// Token: 0x040004BB RID: 1211
	private int _currentRenderQueue;

	// Token: 0x040004BC RID: 1212
	private List<GameObject> poisonAnims = new List<GameObject>();

	// Token: 0x040004BD RID: 1213
	private static Func<string, string> localPath = new Func<string, string>(StorageEnvironment.getLocalPathFor);

	// Token: 0x040004BE RID: 1214
	private static Func<string, string> remotePath = new Func<string, string>(StorageEnvironment.getRemotePathFor);

	// Token: 0x040004BF RID: 1215
	private string _bundleId;

	// Token: 0x040004C0 RID: 1216
	private float _hitTime;

	// Token: 0x040004C1 RID: 1217
	private float _hitTimeMax = 0.1f;

	// Token: 0x040004C2 RID: 1218
	private float _healTime;

	// Token: 0x040004C3 RID: 1219
	private float _frostWindTime;

	// Token: 0x040004C4 RID: 1220
	private static readonly string LocatorHit = "hit";

	// Token: 0x040004C5 RID: 1221
	private static readonly string LocatorProjectile = "projectile";

	// Token: 0x040004C6 RID: 1222
	private static readonly string LocatorAction = "action";

	// Token: 0x040004C7 RID: 1223
	private static readonly string LocatorAbility = "ability";

	// Token: 0x020000B6 RID: 182
	private enum SummonState
	{
		// Token: 0x040004D0 RID: 1232
		None,
		// Token: 0x040004D1 RID: 1233
		PlayStart,
		// Token: 0x040004D2 RID: 1234
		PlayLoop,
		// Token: 0x040004D3 RID: 1235
		PlayEnd,
		// Token: 0x040004D4 RID: 1236
		PlayDone,
		// Token: 0x040004D5 RID: 1237
		Done
	}

	// Token: 0x020000B7 RID: 183
	private enum BuffLevel
	{
		// Token: 0x040004D7 RID: 1239
		None,
		// Token: 0x040004D8 RID: 1240
		Glow,
		// Token: 0x040004D9 RID: 1241
		Particles
	}

	// Token: 0x020000B8 RID: 184
	private enum AnimState
	{
		// Token: 0x040004DB RID: 1243
		Idle,
		// Token: 0x040004DC RID: 1244
		PrePreAttack,
		// Token: 0x040004DD RID: 1245
		PreAttack,
		// Token: 0x040004DE RID: 1246
		Attack,
		// Token: 0x040004DF RID: 1247
		ProjectileAttack,
		// Token: 0x040004E0 RID: 1248
		Back,
		// Token: 0x040004E1 RID: 1249
		Damage,
		// Token: 0x040004E2 RID: 1250
		Action
	}

	// Token: 0x020000B9 RID: 185
	private class ParticleSystemData
	{
		// Token: 0x060006CE RID: 1742 RVA: 0x000064CC File Offset: 0x000046CC
		public ParticleSystemData(string name, GameObject g, ParticleSystem ps)
		{
			this.name = name;
			this.g = g;
			this.ps = ps;
		}

		// Token: 0x060006CF RID: 1743 RVA: 0x000064E9 File Offset: 0x000046E9
		public void setRunning(bool running)
		{
			if (running == this._running)
			{
				return;
			}
			this.ps.enableEmission = running;
			this._running = running;
		}

		// Token: 0x040004E3 RID: 1251
		public string name;

		// Token: 0x040004E4 RID: 1252
		public ParticleSystem ps;

		// Token: 0x040004E5 RID: 1253
		public GameObject g;

		// Token: 0x040004E6 RID: 1254
		private bool _running;
	}

	// Token: 0x020000BA RID: 186
	public enum StatsState
	{
		// Token: 0x040004E8 RID: 1256
		NONE,
		// Token: 0x040004E9 RID: 1257
		FLASH,
		// Token: 0x040004EA RID: 1258
		LONGFLASH,
		// Token: 0x040004EB RID: 1259
		HOLD
	}

	// Token: 0x020000BB RID: 187
	public interface ICallback
	{
		// Token: 0x060006D0 RID: 1744
		void cameraShake(float strength);

		// Token: 0x060006D1 RID: 1745
		void effectDone();

		// Token: 0x060006D2 RID: 1746
		void playHitAnimation();
	}

	// Token: 0x020000BC RID: 188
	private interface AnimNamer
	{
		// Token: 0x060006D3 RID: 1747
		string getAttackName();
	}

	// Token: 0x020000BD RID: 189
	private abstract class BaseAnimNamer : Unit.AnimNamer
	{
		// Token: 0x060006D5 RID: 1749
		public abstract string getAttackName();
	}

	// Token: 0x020000BE RID: 190
	private class SingleNamer : Unit.BaseAnimNamer
	{
		// Token: 0x060006D6 RID: 1750 RVA: 0x0000650B File Offset: 0x0000470B
		public SingleNamer(string name)
		{
			this.name = name;
		}

		// Token: 0x060006D7 RID: 1751 RVA: 0x0000651A File Offset: 0x0000471A
		public override string getAttackName()
		{
			return this.name;
		}

		// Token: 0x040004EC RID: 1260
		private string name;
	}

	// Token: 0x020000BF RID: 191
	private class SequenceNamer : Unit.AnimNamer
	{
		// Token: 0x060006D8 RID: 1752 RVA: 0x00006522 File Offset: 0x00004722
		public SequenceNamer(params string[] names) : this(names)
		{
		}

		// Token: 0x060006D9 RID: 1753 RVA: 0x0000652B File Offset: 0x0000472B
		public SequenceNamer(IEnumerable<string> names)
		{
			this.names.AddRange(names);
		}

		// Token: 0x060006DA RID: 1754 RVA: 0x0003EE64 File Offset: 0x0003D064
		public string getAttackName()
		{
			return this.names[this.index++ % this.names.Count];
		}

		// Token: 0x040004ED RID: 1261
		private List<string> names = new List<string>();

		// Token: 0x040004EE RID: 1262
		private int index;
	}

	// Token: 0x020000C0 RID: 192
	private static class AnimNamerFactory
	{
		// Token: 0x060006DB RID: 1755 RVA: 0x0000654A File Offset: 0x0000474A
		public static Unit.AnimNamer create(AnimPlayer player, string name)
		{
			if (player == null || player.getUnitDesc() == null)
			{
				return new Unit.SequenceNamer(new string[]
				{
					name
				});
			}
			return new Unit.SequenceNamer(Unit.AnimNamerFactory.getSequenceNames(player, name));
		}

		// Token: 0x060006DC RID: 1756 RVA: 0x0003EE9C File Offset: 0x0003D09C
		private static List<string> getSequenceNames(AnimPlayer player, string name)
		{
			name = name.ToLower();
			string text = name + "_s";
			List<string> list = new List<string>();
			foreach (string text2 in player.getAnimationNames())
			{
				string text3 = text2.ToLower();
				if (text3 == name || text3.StartsWith(text))
				{
					list.Add(text3);
				}
			}
			list.Sort();
			return list;
		}
	}
}
