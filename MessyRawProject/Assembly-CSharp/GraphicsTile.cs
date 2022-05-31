using System;
using NSCampaign;
using UnityEngine;

// Token: 0x020000E8 RID: 232
public class GraphicsTile : MonoBehaviour
{
	// Token: 0x060007B8 RID: 1976 RVA: 0x000028DF File Offset: 0x00000ADF
	private void Start()
	{
	}

	// Token: 0x060007B9 RID: 1977 RVA: 0x00043670 File Offset: 0x00041870
	public void init(int baseX, int baseY, CampaignRenderer callBackTarget)
	{
		this.baseX = baseX;
		this.baseY = baseY;
		this._callbackTarget = callBackTarget;
		Object.Destroy(base.transform.GetComponent<MeshCollider>());
		GameObject gameObject = (GameObject)Object.Instantiate(ResourceManager.Load("Campaign/HexTile"));
		gameObject.transform.parent = base.transform;
		gameObject.renderer.enabled = false;
		gameObject.transform.localEulerAngles = new Vector3(0f, 90f, 0f);
		gameObject.transform.localPosition = new Vector3(0f, 0f, 1.8f);
		gameObject.transform.localScale = new Vector3(0.96922344f, 1f, 4f);
		TileMouseHandle tileMouseHandle = gameObject.AddComponent<TileMouseHandle>();
		tileMouseHandle.init(this);
		base.renderer.material.color = new Color(0f, 0f, 0f, 0f);
	}

	// Token: 0x060007BA RID: 1978 RVA: 0x00043768 File Offset: 0x00041968
	public void setTile(int x, int y, FieldTile tile)
	{
		this.tile = tile;
		this.currentX = x;
		this.currentY = y;
		if (tile == null)
		{
			base.renderer.material.color = new Color(0f, 0f, 0f, 0f);
		}
		else
		{
			this.setupGraphics();
		}
	}

	// Token: 0x060007BB RID: 1979 RVA: 0x00006DAE File Offset: 0x00004FAE
	private void setupGraphics()
	{
		base.renderer.material = GraphicsTileMaterialCache.getMaterial(this.tile, this.baseY);
	}

	// Token: 0x060007BC RID: 1980 RVA: 0x00006DCC File Offset: 0x00004FCC
	public int getTileType()
	{
		this.assertTile();
		return this.tile.id;
	}

	// Token: 0x060007BD RID: 1981 RVA: 0x00006DDF File Offset: 0x00004FDF
	public void discoverTile()
	{
		this.assertTile();
		if (this.tile.id < 0)
		{
			return;
		}
	}

	// Token: 0x060007BE RID: 1982 RVA: 0x00006DF9 File Offset: 0x00004FF9
	private void assertTile()
	{
		if (this.tile == null)
		{
			throw new NullReferenceException("BoardTile.tile is null!");
		}
	}

	// Token: 0x060007BF RID: 1983 RVA: 0x000028DF File Offset: 0x00000ADF
	private void Update()
	{
	}

	// Token: 0x060007C0 RID: 1984 RVA: 0x000028DF File Offset: 0x00000ADF
	public void OnMouseEnter()
	{
	}

	// Token: 0x060007C1 RID: 1985 RVA: 0x000028DF File Offset: 0x00000ADF
	public void OnMouseExit()
	{
	}

	// Token: 0x060007C2 RID: 1986 RVA: 0x00006E11 File Offset: 0x00005011
	public void OnMouseDown()
	{
		this._callbackTarget.clicked(this.currentX, this.currentY);
	}

	// Token: 0x040005C9 RID: 1481
	private CampaignRenderer _callbackTarget;

	// Token: 0x040005CA RID: 1482
	public int baseX;

	// Token: 0x040005CB RID: 1483
	public int baseY;

	// Token: 0x040005CC RID: 1484
	public int currentX;

	// Token: 0x040005CD RID: 1485
	public int currentY;

	// Token: 0x040005CE RID: 1486
	public Transform hexTile;

	// Token: 0x040005CF RID: 1487
	private FieldTile tile;
}
