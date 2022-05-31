using System;
using System.Collections;
using System.Collections.Generic;
using Gui;
using UnityEngine;

// Token: 0x02000123 RID: 291
public class CardViewManager
{
	// Token: 0x06000972 RID: 2418 RVA: 0x000081B0 File Offset: 0x000063B0
	public static CardViewManager getInstance()
	{
		return CardViewManager._instance;
	}

	// Token: 0x06000973 RID: 2419 RVA: 0x000081B7 File Offset: 0x000063B7
	public bool isReady()
	{
		return this.ready;
	}

	// Token: 0x06000974 RID: 2420 RVA: 0x000081BF File Offset: 0x000063BF
	public void build(MonoBehaviour behaviour)
	{
		this.coroutiner = behaviour;
		this.coroutiner.StartCoroutine(this._generate());
	}

	// Token: 0x06000975 RID: 2421 RVA: 0x0004A5E8 File Offset: 0x000487E8
	public Batcher.TexAndCoord get(CardType ct)
	{
		Batcher.TexAndCoord texAndCoord = this.coordinates[ct.id];
		if (ct.name == "Leeching Ring")
		{
			Log.warning("tac: " + texAndCoord);
		}
		Rect uv = texAndCoord.uv;
		uv.y += uv.height * 0.3f;
		uv.height *= 0.7f;
		uv.x += uv.width * 0.1f;
		uv.width *= 0.8f;
		return new Batcher.TexAndCoord(texAndCoord.tex, uv);
	}

	// Token: 0x06000976 RID: 2422 RVA: 0x0004A6A0 File Offset: 0x000488A0
	private IEnumerator generateSheet(GameObject g, List<ICardView> views, int startIndex)
	{
		Batcher batcher = g.AddComponent<Batcher>();
		List<GameObject> gameObjects = new List<GameObject>();
		int countPerSheet = Batcher.getCountPerSheet(this.CardWidth, this.CardHeight);
		List<CardType> allCardTypes = CardTypeManager.getInstance().getAll();
		for (int i = 0; i < countPerSheet; i++)
		{
			int cardTypeIndex = startIndex + i;
			if (cardTypeIndex >= allCardTypes.Count)
			{
				break;
			}
			views[i].updateGraphics(new Card((long)i, allCardTypes[cardTypeIndex]));
			gameObjects.Add(views[i].getTransform().gameObject);
		}
		batcher.init(gameObjects, this.CardWidth, this.CardHeight);
		Rect innerRect = new Rect(0f, 0f, (float)this.CardWidth, (float)this.CardHeight);
		innerRect.y = 0.05f * innerRect.height;
		innerRect.height = 0.95f * innerRect.height;
		batcher.setInnerRect(innerRect);
		while (!batcher.isReady())
		{
			yield return new WaitForEndOfFrame();
		}
		for (int j = 0; j < gameObjects.Count; j++)
		{
			int cardTypeIndex2 = startIndex + j;
			Batcher.TexAndCoord coord = batcher._texCoords[j];
			this.coordinates.Add(allCardTypes[cardTypeIndex2].id, coord);
		}
		this.textures.AddRange(batcher._textures);
		Object.Destroy(batcher);
		yield break;
	}

	// Token: 0x06000977 RID: 2423 RVA: 0x0004A6E8 File Offset: 0x000488E8
	public IEnumerator _generate()
	{
		yield return this.coroutiner.StartCoroutine(this.WaitForCardTypes());
		GameObject g = new GameObject("CardViewManager_builder");
		Camera camera = g.AddComponent<Camera>();
		Gui3D gui = new Gui3D(camera);
		List<CardType> allCardTypes = CardTypeManager.getInstance().getAll();
		int countPerSheet = Batcher.getCountPerSheet(this.CardWidth, this.CardHeight);
		Log.warning("Beginning to create CardViews");
		List<ICardView> views = new List<ICardView>();
		CardType defaultType = new CardType();
		defaultType.costGrowth = 1;
		for (int i = 0; i < countPerSheet; i++)
		{
			views.Add(CardViewManager.createCardView(new Card((long)i, defaultType), 31));
			yield return new WaitForEndOfFrame();
		}
		Log.warning("Finished creating CardViews");
		for (int j = 0; j < allCardTypes.Count; j += countPerSheet)
		{
			yield return this.coroutiner.StartCoroutine(this.generateSheet(g, views, j));
		}
		this.ready = true;
		yield break;
	}

	// Token: 0x06000978 RID: 2424 RVA: 0x0004A704 File Offset: 0x00048904
	private IEnumerator WaitForCardTypes()
	{
		while (CardTypeManager.getInstance().size() == 0)
		{
			yield return new WaitForSeconds(0.1f);
		}
		yield break;
	}

	// Token: 0x06000979 RID: 2425 RVA: 0x0004A718 File Offset: 0x00048918
	private static ICardView createCardView(Card card, int layer)
	{
		GameObject gameObject = PrimitiveFactory.createPlane(true);
		CardView cardView = gameObject.AddComponent<CardView>();
		cardView.setType(CardView.Type.Small);
		cardView.init(null, card, -1);
		cardView.setLayer(layer);
		cardView.setTooltipEnabled(false);
		return cardView;
	}

	// Token: 0x04000714 RID: 1812
	private static CardViewManager _instance = new CardViewManager();

	// Token: 0x04000715 RID: 1813
	private MonoBehaviour coroutiner;

	// Token: 0x04000716 RID: 1814
	public List<Texture> textures = new List<Texture>();

	// Token: 0x04000717 RID: 1815
	private readonly int CardWidth = 200;

	// Token: 0x04000718 RID: 1816
	private readonly int CardHeight = 256;

	// Token: 0x04000719 RID: 1817
	private bool ready;

	// Token: 0x0400071A RID: 1818
	private Dictionary<int, Batcher.TexAndCoord> coordinates = new Dictionary<int, Batcher.TexAndCoord>();
}
