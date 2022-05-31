using System;
using System.Collections;
using System.Collections.Generic;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x0200008F RID: 143
public class Idol : MonoBehaviour, iEffect
{
	// Token: 0x0600053A RID: 1338 RVA: 0x0000552D File Offset: 0x0000372D
	private void Awake()
	{
		this.NumMat = new Material(ResourceManager.LoadShader("Transparent/Diffuse"));
		this.NumMat.renderQueue = 10002;
		this.audioScript = App.AudioScript;
	}

	// Token: 0x0600053B RID: 1339 RVA: 0x00038144 File Offset: 0x00036344
	private void Update()
	{
		if (this.flashTimer != -1f && Time.time - this.flashTimer > 2f)
		{
			float num = 1f - 4f * (Time.time - this.flashTimer - 2f);
			if (num < 0f)
			{
				num = 0f;
			}
			this.iconFrame.renderer.material.color = new Color(1f, 1f, 1f, num);
			for (int i = 0; i < this.numbersObjArr.Count; i++)
			{
				this.numbersObjArr[i].renderer.material.color = new Color(1f, 1f, 1f, num);
			}
			if (num <= 0f)
			{
				this.flashTimer = -1f;
			}
		}
	}

	// Token: 0x0600053C RID: 1340 RVA: 0x00038234 File Offset: 0x00036434
	public void init(BattleMode battleMode, bool left, int idolIndex, int hitPoints, int maxHitPoints, string resource, bool showUnitStats)
	{
		this.left = left;
		this.idolIndex = idolIndex;
		this.lastHitPoints = hitPoints;
		this.hitPoints = hitPoints;
		this.maxHealth = maxHitPoints;
		this.resource = resource;
		this.battleMode = battleMode;
		this.idolMaterial = new Material(ResourceManager.LoadShader("Scrolls/Transparent/Diffuse/Double"));
		this.idolMaterial.renderQueue = 10000;
		float num = 1f - 0.2f * this.distanceFromCenter() * this.distanceFromCenter();
		this.idolMaterial.color = new Color(num, num, num, 1f);
		this.setTexture(this.getTextureIdFromHitPoints(hitPoints));
		this.setupIdolIcons();
		if (showUnitStats)
		{
			this.showStats(showUnitStats);
		}
		this.createShadow(base.gameObject.transform.position);
		if (!left)
		{
			base.gameObject.tag = "blinkable_idol";
		}
	}

	// Token: 0x0600053D RID: 1341 RVA: 0x0000555F File Offset: 0x0000375F
	private float distanceFromCenter()
	{
		return 0.5f * (float)Mathf.Abs(2 - this.idolIndex);
	}

	// Token: 0x0600053E RID: 1342 RVA: 0x00005575 File Offset: 0x00003775
	public bool isLeft()
	{
		return this.left;
	}

	// Token: 0x0600053F RID: 1343 RVA: 0x0000557D File Offset: 0x0000377D
	public int row()
	{
		return this.idolIndex;
	}

	// Token: 0x06000540 RID: 1344 RVA: 0x00038320 File Offset: 0x00036520
	private void setTexture(string resNum)
	{
		Texture2D mainTexture = ResourceManager.LoadTexture("BattleMode/Crystals/" + this.resource + string.Empty + resNum);
		this.idolMaterial.mainTexture = mainTexture;
		base.renderer.material = this.idolMaterial;
		if (this.shadow != null)
		{
			this.shadow.renderer.material.mainTexture = mainTexture;
		}
	}

	// Token: 0x06000541 RID: 1345 RVA: 0x00038390 File Offset: 0x00036590
	public void setShadowColor(Color color)
	{
		this.shadowColor = color;
		if (this.shadow == null)
		{
			return;
		}
		this.shadow.renderer.material.SetColor("_Color", this.shadowColor);
		this.shadow.renderer.material.SetFloat("_Lerp", 1f);
	}

	// Token: 0x06000542 RID: 1346 RVA: 0x000383F8 File Offset: 0x000365F8
	private void setupIdolIcons()
	{
		Material material = new Material(ResourceManager.LoadShader("Transparent/Diffuse"));
		material.renderQueue = 10001;
		this.iconFrame = PrimitiveFactory.createPlane(false);
		this.iconFrame.transform.parent = base.transform.parent;
		this.iconFrame.transform.localScale = new Vector3(0.05f, 1f, 0.026865672f);
		this.iconFrame.transform.localEulerAngles = new Vector3(39f, 90f, 0f);
		this.iconFrame.renderer.material = material;
		Texture2D mainTexture = ResourceManager.LoadTexture("BattleUI/battlegui_idolhealth");
		this.iconFrame.renderer.material.mainTexture = mainTexture;
		this.iconFrame.renderer.material.color = new Color(1f, 1f, 1f, 0f);
		this.iconFrame.name = "iconFrame";
		this.iconFrame.transform.position = this.getClampedTransformPosition();
	}

	// Token: 0x06000543 RID: 1347 RVA: 0x00005585 File Offset: 0x00003785
	private void OnMouseEnter()
	{
		if (!this.alive())
		{
			return;
		}
		this.audioScript.PlaySFX("Sounds/hyperduck/UI/ui_idol_mouseover");
		this.flashIdolStats();
	}

	// Token: 0x06000544 RID: 1348 RVA: 0x00038518 File Offset: 0x00036718
	private Vector3 getClampedTransformPosition()
	{
		Vector3 position = base.transform.position;
		if (AspectRatio.now.ratio >= AspectRatio._4_3.ratio)
		{
			return position;
		}
		Vector3 vector = Camera.main.WorldToScreenPoint(position);
		float num = (float)Screen.width * 0.055f;
		if (vector.x >= num && vector.x <= (float)Screen.width - num)
		{
			return position;
		}
		vector.x = Mathf.Clamp(vector.x, num, (float)Screen.width - num);
		return Camera.main.ScreenToWorldPoint(vector);
	}

	// Token: 0x06000545 RID: 1349 RVA: 0x000385B0 File Offset: 0x000367B0
	private void clearDigits()
	{
		for (int i = 0; i < this.numbersObjArr.Count; i++)
		{
			Object.Destroy(this.numbersObjArr[i]);
		}
		this.numbersObjArr.Clear();
	}

	// Token: 0x06000546 RID: 1350 RVA: 0x000385F8 File Offset: 0x000367F8
	private void createDigits()
	{
		this.clearDigits();
		char[] array = Convert.ToString(this.hitPoints).ToCharArray();
		for (int i = 0; i < array.Length; i++)
		{
			GameObject gameObject = PrimitiveFactory.createPlane(false);
			gameObject.name = string.Concat(new object[]
			{
				"digit",
				i,
				'_',
				array[i]
			});
			gameObject.transform.parent = this.iconFrame.transform;
			gameObject.transform.localScale = new Vector3(0.03f, 1f, 0.08f);
			iTween.ScaleTo(gameObject, iTween.Hash(new object[]
			{
				"x",
				0.3f,
				"z",
				0.8f,
				"time",
				0.7f,
				"easetype",
				iTween.EaseType.elastic
			}));
			gameObject.transform.eulerAngles = new Vector3(39f, 90f, 0f);
			gameObject.transform.localPosition = new Vector3(-0.8f - ((array.Length <= 1) ? 1f : 0f) - (float)(array.Length - 1) * 0.03f - (float)i * 2.25f, 0f, 0.03f);
			gameObject.renderer.material = this.NumMat;
			gameObject.renderer.material.mainTexture = ResourceManager.LoadTexture("BattleUI/battlegui_number_" + array[i]);
			gameObject.renderer.enabled = this.iconFrame.renderer.enabled;
			this.numbersObjArr.Add(gameObject);
		}
	}

	// Token: 0x06000547 RID: 1351 RVA: 0x000387D4 File Offset: 0x000369D4
	public void showStats(bool showIdolStats)
	{
		if (!this.alive())
		{
			showIdolStats = false;
		}
		this.showIdolStats = showIdolStats;
		if (showIdolStats)
		{
			this.iconFrame.renderer.material.color = new Color(1f, 1f, 1f, 1f);
		}
		else
		{
			this.iconFrame.renderer.material.color = new Color(1f, 1f, 1f, 0f);
		}
		this.iconFrame.transform.position = this.getClampedTransformPosition();
		float num = base.transform.position.x / 20f;
		float num2 = base.transform.position.z / 20f;
		this.iconFrame.transform.Translate(0f - num2, 0f, -0.26f - num);
		this.clearDigits();
		if (showIdolStats)
		{
			this.createDigits();
		}
		this.flashTimer = -1f;
	}

	// Token: 0x06000548 RID: 1352 RVA: 0x000388E8 File Offset: 0x00036AE8
	public void flashIdolStats()
	{
		if (!this.alive())
		{
			return;
		}
		this.iconFrame.renderer.material.color = new Color(1f, 1f, 1f, 1f);
		if (!this.showIdolStats)
		{
			this.iconFrame.transform.position = this.getClampedTransformPosition();
			float num = base.transform.position.x / 20f;
			float num2 = base.transform.position.z / 20f;
			this.iconFrame.transform.Translate(0f - num2, 0f, -0.26f - num);
			this.flashTimer = Time.time;
		}
		this.createDigits();
	}

	// Token: 0x06000549 RID: 1353 RVA: 0x000389B8 File Offset: 0x00036BB8
	private string getTextureIdFromHitPoints(int hitPoints)
	{
		if (hitPoints <= 0)
		{
			return "05";
		}
		if (this.maxHealth < 5)
		{
			return "0" + (5 - hitPoints);
		}
		float num = (float)hitPoints / (float)this.maxHealth;
		if (num >= 0.999f)
		{
			return "01";
		}
		if (num >= 0.7f)
		{
			return "02";
		}
		if (num >= 0.4f)
		{
			return "03";
		}
		return "04";
	}

	// Token: 0x0600054A RID: 1354 RVA: 0x00038A34 File Offset: 0x00036C34
	public void setHitPoints(int hitPoints, bool mayShowGoldText, bool showExplosion)
	{
		this.hitPoints = hitPoints;
		if (showExplosion)
		{
			if (hitPoints <= 0)
			{
				this.audioScript.PlaySFX("Sounds/hyperduck/death_idol");
			}
			else if (hitPoints < this.lastHitPoints)
			{
				this.audioScript.PlaySFX("Sounds/hyperduck/impact_idol");
			}
		}
		string textureIdFromHitPoints = this.getTextureIdFromHitPoints(hitPoints);
		if (hitPoints > 0)
		{
			this.setTexture(textureIdFromHitPoints);
		}
		else
		{
			if (!this._surrendered)
			{
				if (!showExplosion)
				{
					this._hasPlayedIdolCracks = (this._hasPlayedIdolExplosion = true);
					this.setTexture(textureIdFromHitPoints);
				}
				this.playIdolCracks();
				if (mayShowGoldText)
				{
					base.StartCoroutine(this.addGoldTextSoon());
				}
			}
			else
			{
				this.setTexture(textureIdFromHitPoints);
			}
			this.removeShadow();
			this.showStats(false);
		}
		this.flashIdolStats();
		this.lastHitPoints = hitPoints;
	}

	// Token: 0x0600054B RID: 1355 RVA: 0x00038B08 File Offset: 0x00036D08
	private IEnumerator addGoldTextSoon()
	{
		yield return new WaitForSeconds(1.75f);
		this.battleMode.addGoldText(this);
		yield break;
	}

	// Token: 0x0600054C RID: 1356 RVA: 0x000055A9 File Offset: 0x000037A9
	public void surrender()
	{
		this._surrendered = true;
		this._hasPlayedIdolExplosion = true;
		this._hasPlayedIdolCracks = true;
		this.setHitPoints(0, false, true);
	}

	// Token: 0x0600054D RID: 1357 RVA: 0x00038B24 File Offset: 0x00036D24
	private void playIdolCracks()
	{
		if (this._hasPlayedIdolCracks)
		{
			return;
		}
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<MeshRenderer>();
		EffectPlayer effectPlayer = gameObject.AddComponent<EffectPlayer>();
		float num = 0.4f;
		effectPlayer.init("idol-death-cracks", 1, this, 89998, new Vector3(num, num, num), false, "IdolExplosion-cracks", 0);
		effectPlayer.transform.position = base.gameObject.transform.position;
		effectPlayer.transform.eulerAngles = new Vector3(51f, 270f, 0f);
		this._hasPlayedIdolCracks = true;
	}

	// Token: 0x0600054E RID: 1358 RVA: 0x00038BBC File Offset: 0x00036DBC
	private void playIdolExplosion()
	{
		if (this._hasPlayedIdolExplosion)
		{
			return;
		}
		GameObject gameObject = new GameObject();
		gameObject.AddComponent<MeshRenderer>();
		EffectPlayer effectPlayer = gameObject.AddComponent<EffectPlayer>();
		float num = 0.4f;
		effectPlayer.init("idol-death", 1, this, 89999, new Vector3(num, num, num), false, "IdolExplosion-death", 0);
		effectPlayer.transform.position = base.gameObject.transform.position;
		effectPlayer.transform.eulerAngles = new Vector3(51f, 270f, 0f);
		GameObject gameObject2 = new GameObject();
		gameObject2.AddComponent<MeshRenderer>();
		EffectPlayer effectPlayer2 = gameObject2.AddComponent<EffectPlayer>();
		float num2 = 0.45f;
		effectPlayer2.init("dust-puff-big", 1, this, 90000, new Vector3(num2, num2, num2), false, "IdolExplosion-dust", 0);
		effectPlayer2.transform.position = base.gameObject.transform.position;
		effectPlayer2.transform.eulerAngles = new Vector3(51f, 270f, 0f);
		GameObject gameObject3 = new GameObject();
		gameObject3.AddComponent<MeshRenderer>();
		EffectPlayer effectPlayer3 = gameObject3.AddComponent<EffectPlayer>();
		float num3 = 0.6f;
		effectPlayer3.init("transp", 1, this, 90001, new Vector3(num3, num3, num3), false, "IdolExplosion-transp", 0);
		effectPlayer3.transform.position = base.gameObject.transform.position;
		effectPlayer3.transform.eulerAngles = new Vector3(51f, 270f, 0f);
		this._hasPlayedIdolExplosion = true;
		this.battleMode.cameraShake(1.5f);
		this.battleMode.flashScreen(0.1f);
	}

	// Token: 0x0600054F RID: 1359 RVA: 0x000055C9 File Offset: 0x000037C9
	public int getHitPoints()
	{
		return this.hitPoints;
	}

	// Token: 0x06000550 RID: 1360 RVA: 0x000055D1 File Offset: 0x000037D1
	public bool alive()
	{
		return this.hitPoints > 0;
	}

	// Token: 0x06000551 RID: 1361 RVA: 0x000055DC File Offset: 0x000037DC
	public Vector3 getPosition()
	{
		return base.transform.position;
	}

	// Token: 0x06000552 RID: 1362 RVA: 0x00038D70 File Offset: 0x00036F70
	private void createShadow(Vector3 pos)
	{
		if (this.shadow != null)
		{
			return;
		}
		this.shadow = UnityUtil.addChild(base.transform.parent.gameObject, PrimitiveFactory.createPlane(false));
		PlaneModifier.shear(this.shadow, -5f, 0f);
		this.shadow.name = "Shadow";
		this.shadow.transform.position = base.gameObject.transform.position;
		Vector3 localScale = base.gameObject.transform.localScale;
		this.shadow.transform.localScale = new Vector3(localScale.x, localScale.y, localScale.z * 0.5f);
		this.shadow.transform.eulerAngles = base.gameObject.transform.eulerAngles;
		this.shadow.transform.Translate(0f, 0f, 0.1f);
		Vector3 position = this.shadow.transform.position;
		position.y = 0.05f;
		position.z = Mathf.Sign(position.z) * 4.55f;
		this.shadow.transform.position = position;
		Material material = new Material(ResourceManager.LoadShader("Scrolls/Transparent/Diffuse/Double/Color"));
		this.shadow.renderer.material = material;
		this.shadow.renderer.material.renderQueue = 9999;
		this.shadow.renderer.material.mainTexture = this.idolMaterial.mainTexture;
		this.shadow.renderer.enabled = true;
		this.setShadowColor(this.shadowColor);
	}

	// Token: 0x06000553 RID: 1363 RVA: 0x000055E9 File Offset: 0x000037E9
	private void removeShadow()
	{
		if (this.shadow == null)
		{
			return;
		}
		Object.Destroy(this.shadow);
		this.shadow = null;
	}

	// Token: 0x06000554 RID: 1364 RVA: 0x0000560F File Offset: 0x0000380F
	public void effectAnimDone(EffectPlayer effect, bool loop)
	{
		if (effect.effectName == "IdolExplosion-cracks")
		{
			this.playIdolExplosion();
		}
		Object.Destroy(effect.gameObject);
	}

	// Token: 0x06000555 RID: 1365 RVA: 0x00005637 File Offset: 0x00003837
	public void locator(EffectPlayer effect, AnimLocator loc)
	{
		if (loc.name == "flash")
		{
			this.setTexture("05");
		}
	}

	// Token: 0x040003B4 RID: 948
	private const string AnimNameIdolCracks = "IdolExplosion-cracks";

	// Token: 0x040003B5 RID: 949
	private int idolIndex;

	// Token: 0x040003B6 RID: 950
	private int hitPoints;

	// Token: 0x040003B7 RID: 951
	private int lastHitPoints;

	// Token: 0x040003B8 RID: 952
	private int maxHealth = 1;

	// Token: 0x040003B9 RID: 953
	private bool left;

	// Token: 0x040003BA RID: 954
	private string resource;

	// Token: 0x040003BB RID: 955
	private Material idolMaterial;

	// Token: 0x040003BC RID: 956
	private BattleMode battleMode;

	// Token: 0x040003BD RID: 957
	private bool showIdolStats;

	// Token: 0x040003BE RID: 958
	private GameObject iconFrame;

	// Token: 0x040003BF RID: 959
	private List<GameObject> numbersObjArr = new List<GameObject>();

	// Token: 0x040003C0 RID: 960
	private Material NumMat;

	// Token: 0x040003C1 RID: 961
	private float flashTimer = -1f;

	// Token: 0x040003C2 RID: 962
	private AudioScript audioScript;

	// Token: 0x040003C3 RID: 963
	private Color shadowColor = ColorUtil.FromHex24(4006939u, 0.5f);

	// Token: 0x040003C4 RID: 964
	private bool _surrendered;

	// Token: 0x040003C5 RID: 965
	private bool _hasPlayedIdolExplosion;

	// Token: 0x040003C6 RID: 966
	private bool _hasPlayedIdolCracks;

	// Token: 0x040003C7 RID: 967
	private GameObject shadow;
}
