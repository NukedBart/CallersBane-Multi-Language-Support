using System;
using System.Collections.Generic;
using Gui;
using UnityEngine;

// Token: 0x02000440 RID: 1088
public class ImageMergerComponent : MonoBehaviour
{
	// Token: 0x0600183E RID: 6206 RVA: 0x000117DD File Offset: 0x0000F9DD
	public void init(IEnumerable<ImageMergerComponent.Pos> images)
	{
		this._images = images;
		this._cam = base.gameObject.AddComponent<Camera>();
		this._cam.orthographic = true;
		this._gui = new Gui3D(base.camera);
	}

	// Token: 0x0600183F RID: 6207 RVA: 0x00092548 File Offset: 0x00090748
	public void Update()
	{
		if (this._images == null)
		{
			return;
		}
		if (this._texture != null)
		{
			return;
		}
		RenderTexture temporary = RenderTexture.GetTemporary(2048, 2048);
		this._cam.targetTexture = temporary;
		foreach (ImageMergerComponent.Pos pos in this._images)
		{
			Texture2D texture2D = ResourceManager.LoadTexture(pos.filename);
			texture2D.wrapMode = 1;
			this._gui.DrawTexture(this._unit.r(pos.dst), texture2D);
		}
		this._cam.targetTexture = null;
		Texture2D texture2D2 = new Texture2D(Math.Min(2048, Screen.width), Math.Min(2048, Screen.height));
		RenderTexture.active = temporary;
		texture2D2.ReadPixels(new Rect(0f, 0f, (float)texture2D2.width, (float)texture2D2.height), 0, 0, false);
		texture2D2.Apply();
		RenderTexture.active = null;
		RenderTexture.ReleaseTemporary(temporary);
		this._texture = texture2D2;
	}

	// Token: 0x06001840 RID: 6208 RVA: 0x00011814 File Offset: 0x0000FA14
	public bool isReady()
	{
		return this.getTexture() != null;
	}

	// Token: 0x06001841 RID: 6209 RVA: 0x00011822 File Offset: 0x0000FA22
	public Texture2D getTexture()
	{
		return this._texture;
	}

	// Token: 0x04001512 RID: 5394
	private Camera _cam;

	// Token: 0x04001513 RID: 5395
	private Gui3D _gui;

	// Token: 0x04001514 RID: 5396
	private IEnumerable<ImageMergerComponent.Pos> _images;

	// Token: 0x04001515 RID: 5397
	private Texture2D _texture;

	// Token: 0x04001516 RID: 5398
	private MockupCalc _unit = new MockupCalc(1, 1);

	// Token: 0x04001517 RID: 5399
	private GameObject _go;

	// Token: 0x02000441 RID: 1089
	public class Pos
	{
		// Token: 0x06001842 RID: 6210 RVA: 0x00092680 File Offset: 0x00090880
		public Pos(string filename)
		{
			if (!string.IsNullOrEmpty(filename))
			{
				if (filename.LastIndexOf('.') >= 0)
				{
					filename = filename.Substring(0, filename.LastIndexOf('.'));
				}
				this.filename = filename;
			}
		}

		// Token: 0x06001843 RID: 6211 RVA: 0x0001182A File Offset: 0x0000FA2A
		public ImageMergerComponent.Pos setRect(Rect dst)
		{
			this.dst = dst;
			return this;
		}

		// Token: 0x06001844 RID: 6212 RVA: 0x00011834 File Offset: 0x0000FA34
		public bool isEmpty()
		{
			return this.filename == null || this.dst.width <= 0.0001f;
		}

		// Token: 0x04001518 RID: 5400
		public string filename;

		// Token: 0x04001519 RID: 5401
		public Rect dst = new Rect(0f, 0f, 1f, 1f);

		// Token: 0x0400151A RID: 5402
		public float layerWidth;

		// Token: 0x0400151B RID: 5403
		public float layerHeight;

		// Token: 0x0400151C RID: 5404
		private bool _empty;
	}
}
