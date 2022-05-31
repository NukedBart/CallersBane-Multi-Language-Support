using System;
using System.Collections;
using System.Collections.Generic;
using Gui;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x0200010F RID: 271
public class PersistentRuleCardView : MonoBehaviour, iEffect
{
	// Token: 0x06000892 RID: 2194 RVA: 0x00044CE4 File Offset: 0x00042EE4
	public void init(Card card, TileColor color, int duration)
	{
		this._card = card;
		this._playerColor = color;
		this._id = ++PersistentRuleCardView._runningId;
		this._root = new GameObject("root");
		UnityUtil.addChild(base.gameObject, this._root);
		Vector3 localScale = base.gameObject.transform.localScale;
		this._root.transform.localScale = new Vector3(1f / localScale.x, 1f / localScale.y, 1f / localScale.z);
		base.name = "LingeringSpell";
		base.renderer.material = PersistentRuleCardView.material;
		base.renderer.material.mainTexture = ResourceManager.LoadTexture("Scrolls/Lingering/lingering_spell_frame");
		this.gui = new Gui3D(UnityUtil.getFirstOrtographicCamera());
		Texture2D tex = App.AssetLoader.LoadCardImage(card.getCardImage());
		this._cardImage = PrimitiveFactory.createTexturedPlane(tex, PersistentRuleCardView.material);
		this._cardImage.name = "cardImage_" + card.getName();
		UnityUtil.addChild(base.gameObject, this._cardImage);
		Vector3 localPosition = this._cardImage.transform.localPosition;
		localPosition.y -= 0.001f;
		this._cardImage.renderer.material.color = new Color(1f, 0f, 0f, 0f);
		this._cardImage.transform.localPosition = localPosition;
		this._cardImage.transform.localScale = new Vector3(0.8828125f, 1f, 0.8366337f);
		Texture2D tex2 = ResourceManager.LoadTexture("BattleUI/stats/bg_cd");
		this._durationPlate = PrimitiveFactory.createTexturedPlane(tex2, PersistentRuleCardView.material);
		this._durationPlate.name = "duration_plate";
		UnityUtil.addChild(this._root, this._durationPlate);
		Vector3 localPosition2 = this._durationPlate.transform.localPosition;
		localPosition2.z += 0.8f;
		localPosition2.y += 0.0015f;
		this._durationPlate.transform.localPosition = localPosition2;
		this._durationPlate.transform.localScale = new Vector3(0.095f, 1f, 0.048f);
		this.set(duration);
		this.setColor(new Color(1f, 1f, 1f, 0f));
		this.createAnim(2, 0.002f, false).getAnimPlayer().setSpeed(2.5f);
		UnityUtil.setLayerRecursively(base.gameObject, 9);
		new TextureFetcher(this, new TextureFetcher.onFetched(this.onFetchedCardImage)).fetch(card.getCardImage());
	}

	// Token: 0x06000893 RID: 2195 RVA: 0x00044FB4 File Offset: 0x000431B4
	private void setEffectColor(Color color)
	{
		foreach (EffectPlayer effectPlayer in this._effectPlayers)
		{
			effectPlayer.renderer.material.color = color;
		}
	}

	// Token: 0x06000894 RID: 2196 RVA: 0x000077EE File Offset: 0x000059EE
	private void onFetchedCardImage(Texture2D image)
	{
		this._cardImage.renderer.material.mainTexture = image;
	}

	// Token: 0x06000895 RID: 2197 RVA: 0x00007806 File Offset: 0x00005A06
	public Card card()
	{
		return this._card;
	}

	// Token: 0x06000896 RID: 2198 RVA: 0x0000780E File Offset: 0x00005A0E
	public TileColor playerColor()
	{
		return this._playerColor;
	}

	// Token: 0x06000897 RID: 2199 RVA: 0x00045018 File Offset: 0x00043218
	public int CompareTo(PersistentRuleCardView other)
	{
		if (this._lastDuration != other._lastDuration)
		{
			return this._lastDuration - other._lastDuration;
		}
		int num = other._id - this._id;
		if (num != 0)
		{
			return num;
		}
		return (int)(this.card().id - other.card().id);
	}

	// Token: 0x06000898 RID: 2200 RVA: 0x00007816 File Offset: 0x00005A16
	public void set(int duration)
	{
		if (duration == this._lastDuration)
		{
			return;
		}
		this._lastDuration = duration;
		PersistentRuleCardView.f(this._durationPlate, this._gameObjects, duration);
	}

	// Token: 0x06000899 RID: 2201 RVA: 0x0000783E File Offset: 0x00005A3E
	public void remove(float fadeTime)
	{
		base.StartCoroutine(this.fadeOut(fadeTime));
	}

	// Token: 0x0600089A RID: 2202 RVA: 0x00045074 File Offset: 0x00043274
	private IEnumerator fadeIn(float fadeTime)
	{
		float st = Time.time;
		for (;;)
		{
			float t = (Time.time - st) / fadeTime;
			float a = Math.Min(t, 1f);
			Color color = new Color(1f, 1f, 1f, a);
			PersistentRuleCardView.setColor(this._durationPlate, color);
			if (t >= 1f)
			{
				break;
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	// Token: 0x0600089B RID: 2203 RVA: 0x0000784E File Offset: 0x00005A4E
	private void setColor(Color color)
	{
		PersistentRuleCardView.setColor(base.gameObject, color);
	}

	// Token: 0x0600089C RID: 2204 RVA: 0x000450A0 File Offset: 0x000432A0
	private static void setColor(GameObject gameObject, Color color)
	{
		UnityUtil.traverse(gameObject, delegate(GameObject g)
		{
			if (g.renderer)
			{
				g.renderer.material.color = color;
			}
		});
	}

	// Token: 0x0600089D RID: 2205 RVA: 0x000450CC File Offset: 0x000432CC
	private IEnumerator fadeOut(float fadeTime)
	{
		float st = Time.time;
		foreach (EffectPlayer e in this._effectPlayers)
		{
			Object.Destroy(e.gameObject);
		}
		this._effectPlayers.Clear();
		for (;;)
		{
			float t = (Time.time - st) / fadeTime;
			float a = Math.Max(1f - t, 0f);
			Color color = new Color(1f, 1f, 1f, a);
			UnityUtil.traverse(base.gameObject, delegate(GameObject g)
			{
				if (g.renderer)
				{
					g.renderer.material.color = color;
				}
			});
			if (t <= 0f)
			{
				break;
			}
			yield return new WaitForEndOfFrame();
		}
		Object.Destroy(base.gameObject);
		yield break;
	}

	// Token: 0x0600089E RID: 2206 RVA: 0x0000785C File Offset: 0x00005A5C
	private static Vector3 getCostPos(int len, int i)
	{
		if (len == 1)
		{
			return new Vector3(-1.72f, 0.006f, 0f);
		}
		return new Vector3(-1f - 2.5f * (float)i, 0.006f, 0f);
	}

	// Token: 0x0600089F RID: 2207 RVA: 0x000450F8 File Offset: 0x000432F8
	private static void f(GameObject parent, List<GameObject> objs, int value)
	{
		bool flag = value > 1000000;
		int num = (!flag) ? value : 0;
		char[] array = Convert.ToString(num).ToCharArray();
		int len = array.Length;
		if (array.Length != objs.Count)
		{
			PersistentRuleCardView.destroyAndClear(objs);
			for (int i = 0; i < array.Length; i++)
			{
				GameObject gameObject = PrimitiveFactory.createPlane(false);
				UnityUtil.addChild(parent, gameObject);
				gameObject.transform.localPosition = PersistentRuleCardView.getCostPos(len, i);
				gameObject.gameObject.layer = Layers.BattleModeUI_NoHandManager;
				objs.Add(gameObject);
			}
		}
		for (int j = 0; j < objs.Count; j++)
		{
			GameObject gameObject2 = objs[j];
			string filename = (!flag) ? ("BattleUI/battlegui_number_" + array[j]) : "BattleUI/battlegui_number_inf";
			float num2 = 0.3f;
			float num3 = 0.75f;
			if (flag)
			{
				num2 = 0.45f;
				num3 = 0.56f;
			}
			gameObject2.transform.localScale = new Vector3(0.5f * num2, 1f, 0.5f * num3);
			iTween.ScaleTo(gameObject2, iTween.Hash(new object[]
			{
				"x",
				num2,
				"z",
				num3,
				"time",
				0.7f,
				"easetype",
				iTween.EaseType.elastic
			}));
			gameObject2.renderer.material = PersistentRuleCardView.material;
			gameObject2.renderer.material.mainTexture = ResourceManager.LoadTexture(filename);
		}
	}

	// Token: 0x060008A0 RID: 2208 RVA: 0x000452B0 File Offset: 0x000434B0
	private static void destroyAndClear(IList<GameObject> objs)
	{
		foreach (GameObject gameObject in objs)
		{
			Object.Destroy(gameObject);
		}
		objs.Clear();
	}

	// Token: 0x060008A1 RID: 2209 RVA: 0x00007897 File Offset: 0x00005A97
	public static bool isLingeringSpell(GameObject g)
	{
		return g.name.StartsWith("LingeringSpell");
	}

	// Token: 0x060008A2 RID: 2210 RVA: 0x00045308 File Offset: 0x00043508
	private EffectPlayer createAnim(int animId, float yoffset, bool loop)
	{
		GameObject gameObject = new GameObject("anim_" + animId);
		gameObject.AddComponent<MeshRenderer>();
		UnityUtil.addChild(base.gameObject, gameObject);
		gameObject.transform.localEulerAngles = new Vector3(90f, 180f, 0f);
		gameObject.transform.localPosition = new Vector3(0f, yoffset, 0f);
		EffectPlayer effectPlayer = gameObject.AddComponent<EffectPlayer>();
		effectPlayer.scaleTransform = base.transform;
		AnimConf conf = new AnimConf().Bundle("lingering_spell").AnimId(animId).Scale(3.9f).Loop(loop).Layer(10);
		effectPlayer.init(conf, this);
		return effectPlayer;
	}

	// Token: 0x060008A3 RID: 2211 RVA: 0x00003FDC File Offset: 0x000021DC
	public void effectAnimDone(EffectPlayer effect, bool loop)
	{
		DefaultIEffectCallback.instance().effectAnimDone(effect, loop);
	}

	// Token: 0x060008A4 RID: 2212 RVA: 0x000453C4 File Offset: 0x000435C4
	public void locator(EffectPlayer effect, AnimLocator loc)
	{
		base.StartCoroutine(this.fadeIn(0.25f));
		this._effectPlayers.Add(this.createAnim(0, -0.02f, true));
		this._effectPlayers.Add(this.createAnim(1, 0f, true));
		this._cardImage.renderer.material.color = Color.white;
		base.renderer.material.color = Color.white;
	}

	// Token: 0x04000651 RID: 1617
	private int _lastDuration = int.MinValue;

	// Token: 0x04000652 RID: 1618
	private Card _card;

	// Token: 0x04000653 RID: 1619
	private TileColor _playerColor;

	// Token: 0x04000654 RID: 1620
	private GameObject _root;

	// Token: 0x04000655 RID: 1621
	private GameObject _cardImage;

	// Token: 0x04000656 RID: 1622
	private GameObject _durationPlate;

	// Token: 0x04000657 RID: 1623
	private static int _runningId = 0;

	// Token: 0x04000658 RID: 1624
	private int _id;

	// Token: 0x04000659 RID: 1625
	private Gui3D gui;

	// Token: 0x0400065A RID: 1626
	public static Material material = new Material(ResourceManager.LoadShader("Scrolls/Unlit/Transparent"));

	// Token: 0x0400065B RID: 1627
	public static Material effectMaterial = new Material(ResourceManager.LoadShader("Scrolls/StoreEffect/Unlit/Transparent"));

	// Token: 0x0400065C RID: 1628
	private List<GameObject> _gameObjects = new List<GameObject>();

	// Token: 0x0400065D RID: 1629
	private List<EffectPlayer> _effectPlayers = new List<EffectPlayer>();

	// Token: 0x0400065E RID: 1630
	public static IComparer<PersistentRuleCardView> comparer = new PersistentRuleCardView.Comparer();

	// Token: 0x02000110 RID: 272
	private class Comparer : IComparer<PersistentRuleCardView>
	{
		// Token: 0x060008A6 RID: 2214 RVA: 0x000078A9 File Offset: 0x00005AA9
		public int Compare(PersistentRuleCardView a, PersistentRuleCardView b)
		{
			return -a.CompareTo(b);
		}
	}
}
