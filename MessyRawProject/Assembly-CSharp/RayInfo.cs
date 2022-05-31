using System;
using UnityEngine;

// Token: 0x02000453 RID: 1107
public class RayInfo
{
	// Token: 0x060018A6 RID: 6310 RVA: 0x00011EA5 File Offset: 0x000100A5
	public RayInfo(Ray ray, int layerMask)
	{
		this.ray = ray;
		this.layerMask = layerMask;
	}

	// Token: 0x060018A7 RID: 6311 RVA: 0x00011EBB File Offset: 0x000100BB
	public RayInfo(Camera camera, Vector3 screen, int layerMask)
	{
		this.ray = camera.ScreenPointToRay(screen);
		this.layerMask = layerMask;
	}

	// Token: 0x04001543 RID: 5443
	public readonly Ray ray;

	// Token: 0x04001544 RID: 5444
	public readonly int layerMask;
}
