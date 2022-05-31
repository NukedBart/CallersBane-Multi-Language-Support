using System;
using NSCampaign;
using UnityEngine;

// Token: 0x020000E7 RID: 231
public class GraphicsPlayer : MonoBehaviour
{
	// Token: 0x060007B1 RID: 1969 RVA: 0x000028DF File Offset: 0x00000ADF
	private void Start()
	{
	}

	// Token: 0x060007B2 RID: 1970 RVA: 0x00006D6A File Offset: 0x00004F6A
	public void init(CampaignRenderer callBackTarget, int xPos, int zPos)
	{
		this._callbackTarget = callBackTarget;
		this.xPos = xPos;
		this.zPos = zPos;
	}

	// Token: 0x060007B3 RID: 1971 RVA: 0x00006D81 File Offset: 0x00004F81
	public void setPosition(int xPos, int zPos)
	{
		Log.info("setPosition");
		this.xPos = xPos;
		this.zPos = zPos;
	}

	// Token: 0x060007B4 RID: 1972 RVA: 0x00006D9B File Offset: 0x00004F9B
	public IntCoord getPosition()
	{
		return new IntCoord(this.xPos, this.zPos);
	}

	// Token: 0x060007B5 RID: 1973 RVA: 0x000028DF File Offset: 0x00000ADF
	private void Update()
	{
	}

	// Token: 0x040005C4 RID: 1476
	private int xPos;

	// Token: 0x040005C5 RID: 1477
	private int zPos;

	// Token: 0x040005C6 RID: 1478
	private int lastXPos;

	// Token: 0x040005C7 RID: 1479
	private int lastZPos;

	// Token: 0x040005C8 RID: 1480
	private CampaignRenderer _callbackTarget;
}
