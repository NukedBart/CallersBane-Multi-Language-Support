using System;
using UnityEngine;

// Token: 0x020000EB RID: 235
public class TileMouseHandle : MonoBehaviour
{
	// Token: 0x060007CD RID: 1997 RVA: 0x000028DF File Offset: 0x00000ADF
	private void Start()
	{
	}

	// Token: 0x060007CE RID: 1998 RVA: 0x00006EC2 File Offset: 0x000050C2
	public void init(GraphicsTile callBackTarget)
	{
		this._callBackTarget = callBackTarget;
	}

	// Token: 0x060007CF RID: 1999 RVA: 0x00006ECB File Offset: 0x000050CB
	private void OnMouseEnter()
	{
		this._callBackTarget.OnMouseEnter();
	}

	// Token: 0x060007D0 RID: 2000 RVA: 0x00006ED8 File Offset: 0x000050D8
	private void OnMouseExit()
	{
		this._callBackTarget.OnMouseExit();
	}

	// Token: 0x060007D1 RID: 2001 RVA: 0x00006EE5 File Offset: 0x000050E5
	private void OnMouseDown()
	{
		this._callBackTarget.OnMouseDown();
	}

	// Token: 0x060007D2 RID: 2002 RVA: 0x000028DF File Offset: 0x00000ADF
	private void Update()
	{
	}

	// Token: 0x040005D4 RID: 1492
	private GraphicsTile _callBackTarget;
}
