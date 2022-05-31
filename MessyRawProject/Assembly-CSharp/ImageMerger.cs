using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200043F RID: 1087
public class ImageMerger
{
	// Token: 0x06001839 RID: 6201 RVA: 0x00011771 File Offset: 0x0000F971
	public ImageMerger(IEnumerable<ImageMergerComponent.Pos> images)
	{
		this._gameObject = new GameObject();
		this._merger = this._gameObject.AddComponent<ImageMergerComponent>();
		this._merger.init(images);
	}

	// Token: 0x0600183A RID: 6202 RVA: 0x000117A1 File Offset: 0x0000F9A1
	public void dispose()
	{
		Object.Destroy(this._gameObject);
	}

	// Token: 0x0600183B RID: 6203 RVA: 0x000117AE File Offset: 0x0000F9AE
	public bool isReady()
	{
		return this._merger.isReady();
	}

	// Token: 0x0600183C RID: 6204 RVA: 0x000117BB File Offset: 0x0000F9BB
	public Texture2D getTexture()
	{
		return this._merger.getTexture();
	}

	// Token: 0x04001510 RID: 5392
	private GameObject _gameObject;

	// Token: 0x04001511 RID: 5393
	private ImageMergerComponent _merger;
}
