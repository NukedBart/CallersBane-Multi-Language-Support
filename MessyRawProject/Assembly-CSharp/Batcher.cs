using System;
using System.Collections.Generic;
using Gui;
using UnityEngine;

// Token: 0x0200041E RID: 1054
public class Batcher : MonoBehaviour
{
	// Token: 0x0600176A RID: 5994 RVA: 0x000906D4 File Offset: 0x0008E8D4
	public Batcher init(IEnumerable<GameObject> gameObjects, int width, int height)
	{
		Batcher.verifyTextureSizeAllowed(width, height);
		this._texSize = Batcher.getMaxTextureSize();
		this.setupCamera();
		this._gameObjects = new List<GameObject>(gameObjects);
		this._gui = new Gui3D(this._cam);
		this._gui.setViewportSize(this._texSize.x, this._texSize.y);
		this._width = width;
		this._height = height;
		this._innerRect = new Rect(0f, 0f, (float)this._width, (float)this._height);
		return this;
	}

	// Token: 0x0600176B RID: 5995 RVA: 0x00010DCC File Offset: 0x0000EFCC
	public Batcher setInnerRect(Rect rect)
	{
		this._innerRect = rect;
		return this;
	}

	// Token: 0x0600176C RID: 5996 RVA: 0x0009076C File Offset: 0x0008E96C
	private void setupCamera()
	{
		this._go = new GameObject();
		this._cam = this._go.AddComponent<Camera>();
		this._cam.orthographic = true;
		this._cam.orthographicSize = this._texSize.y / 2f;
		this._cam.aspect = this._texSize.x / this._texSize.y;
	}

	// Token: 0x0600176D RID: 5997 RVA: 0x00010DD6 File Offset: 0x0000EFD6
	private static Rect createUV(Rect area, float w, float h)
	{
		return GUIUtil.createUVwh(area.x / w, area.y / h, area.width / w, area.height / h);
	}

	// Token: 0x0600176E RID: 5998 RVA: 0x000907E0 File Offset: 0x0008E9E0
	public void Update()
	{
		if (this._gameObjects == null)
		{
			return;
		}
		if (this._ready)
		{
			return;
		}
		long num = TimeUtil.CurrentTimeMillis();
		Vector2 maxTextureSize = Batcher.getMaxTextureSize();
		int num2 = (int)maxTextureSize.x;
		int num3 = (int)maxTextureSize.y;
		int num4 = (int)(1f / (float)this._height);
		int num5 = (int)(1f / (float)this._width);
		int num6 = 0;
		float num7 = (float)this._width / (float)num2;
		float num8 = (float)this._height / (float)num3;
		Batcher.RectPlacer rectPlacer = new Batcher.RectPlacer(num2, num3, this._width, this._height);
		RenderTexture renderTexture = null;
		for (int i = 0; i < this._gameObjects.Count; i++)
		{
			Batcher.RectPlacer.Pos pos = rectPlacer.get(i);
			if (pos.firstInSheet)
			{
				renderTexture = RenderTexture.GetTemporary(num2, num3);
				this._cam.targetTexture = renderTexture;
			}
			this._texCoords.Add(new Batcher.TexAndCoord(Batcher.createUV(pos.rect, (float)num2, (float)num3)));
			Rect dst;
			dst..ctor(pos.rect);
			dst.x += this._innerRect.x;
			dst.y += this._innerRect.y;
			dst.width = this._innerRect.width;
			dst.height = this._innerRect.height;
			this._gui.DrawObject(dst, this._gameObjects[i]);
			if (pos.lastInSheet || i == this._gameObjects.Count - 1)
			{
				this._cam.Render();
				this._cam.targetTexture = null;
				RenderTexture.active = renderTexture;
				Texture2D texture2D = new Texture2D(num2, num3, 5, false);
				texture2D.filterMode = 2;
				texture2D.name = "tex_" + (Batcher.k + 1);
				texture2D.ReadPixels(new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), 0, 0, false);
				texture2D.Compress(true);
				texture2D.Apply(false, false);
				this._textures.Add(texture2D);
				RenderTexture.active = null;
				RenderTexture.ReleaseTemporary(renderTexture);
				for (int j = num6; j <= i; j++)
				{
					this._texCoords[j] = new Batcher.TexAndCoord(texture2D, this._texCoords[j].uv);
				}
				num6 = i;
			}
		}
		this._ready = true;
		long num9 = TimeUtil.CurrentTimeMillis() - num;
		Log.warning("tt: " + num9);
	}

	// Token: 0x0600176F RID: 5999 RVA: 0x00010E01 File Offset: 0x0000F001
	public bool isReady()
	{
		return this._ready;
	}

	// Token: 0x06001770 RID: 6000 RVA: 0x00090A8C File Offset: 0x0008EC8C
	public static int getCountPerSheet(int width, int height)
	{
		Vector2 maxTextureSize = Batcher.getMaxTextureSize();
		return new Batcher.RectPlacer((int)maxTextureSize.x, (int)maxTextureSize.y, width, height).getCountPerSheet();
	}

	// Token: 0x06001771 RID: 6001 RVA: 0x00010E09 File Offset: 0x0000F009
	public static Vector2 getMaxTextureSize()
	{
		return new Vector2(1024f, 1024f);
	}

	// Token: 0x06001772 RID: 6002 RVA: 0x00090ABC File Offset: 0x0008ECBC
	private static void verifyTextureSizeAllowed(int w, int h)
	{
		Vector2 maxTextureSize = Batcher.getMaxTextureSize();
		if ((float)w > maxTextureSize.x || (float)h > maxTextureSize.y)
		{
			throw new ArgumentException(string.Concat(new object[]
			{
				"texture size > max: ",
				w,
				", ",
				h,
				" > ",
				Batcher.getMaxTextureSize()
			}));
		}
	}

	// Token: 0x040014CA RID: 5322
	private Camera _cam;

	// Token: 0x040014CB RID: 5323
	private Gui3D _gui;

	// Token: 0x040014CC RID: 5324
	public List<GameObject> _gameObjects;

	// Token: 0x040014CD RID: 5325
	public List<Texture> _textures = new List<Texture>();

	// Token: 0x040014CE RID: 5326
	public List<Batcher.TexAndCoord> _texCoords = new List<Batcher.TexAndCoord>();

	// Token: 0x040014CF RID: 5327
	private GameObject _go;

	// Token: 0x040014D0 RID: 5328
	private int _width;

	// Token: 0x040014D1 RID: 5329
	private int _height;

	// Token: 0x040014D2 RID: 5330
	private Rect _innerRect;

	// Token: 0x040014D3 RID: 5331
	private bool _ready;

	// Token: 0x040014D4 RID: 5332
	private Vector2 _texSize;

	// Token: 0x040014D5 RID: 5333
	private static int k;

	// Token: 0x0200041F RID: 1055
	public class TexAndCoord
	{
		// Token: 0x06001773 RID: 6003 RVA: 0x00010E1A File Offset: 0x0000F01A
		public TexAndCoord(Texture tex, Rect uv)
		{
			this.tex = tex;
			this.uv = uv;
		}

		// Token: 0x06001774 RID: 6004 RVA: 0x00010E30 File Offset: 0x0000F030
		public TexAndCoord(Rect uv)
		{
			this.uv = uv;
		}

		// Token: 0x06001775 RID: 6005 RVA: 0x00090B34 File Offset: 0x0008ED34
		public void apply(Renderer renderer)
		{
			renderer.material.mainTexture = this.tex;
			renderer.material.mainTextureOffset = new Vector2(this.uv.x, this.uv.y);
			renderer.material.mainTextureScale = new Vector2(this.uv.width, this.uv.height);
		}

		// Token: 0x06001776 RID: 6006 RVA: 0x00010E3F File Offset: 0x0000F03F
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"[TexAndCoord]",
				this.uv,
				"@",
				this.tex.name
			});
		}

		// Token: 0x040014D6 RID: 5334
		public readonly Texture tex;

		// Token: 0x040014D7 RID: 5335
		public readonly Rect uv;
	}

	// Token: 0x02000420 RID: 1056
	private class RectPlacer
	{
		// Token: 0x06001777 RID: 6007 RVA: 0x00090BAC File Offset: 0x0008EDAC
		public RectPlacer(int dstWidth, int dstHeight, int srcWidth, int srcHeight)
		{
			this._dstWidth = dstWidth;
			this._dstHeight = dstHeight;
			this._srcWidth = srcWidth;
			this._srcHeight = srcHeight;
			Log.warning(string.Concat(new object[]
			{
				"rect-placer: ",
				dstWidth,
				", ",
				dstHeight,
				", ",
				srcWidth,
				", ",
				srcHeight
			}));
			this._cols = dstWidth / srcWidth;
			this._rows = dstHeight / srcHeight;
			this._perSheet = this._cols * this._rows;
		}

		// Token: 0x06001778 RID: 6008 RVA: 0x00090C58 File Offset: 0x0008EE58
		public Batcher.RectPlacer.Pos get(int i)
		{
			int num = i % this._perSheet;
			return new Batcher.RectPlacer.Pos
			{
				sheet = i / this._perSheet,
				firstInSheet = (num == 0),
				lastInSheet = (num == this._perSheet - 1),
				rect = new Rect((float)(num % this._cols * this._srcWidth), (float)(num / this._cols * this._srcHeight), (float)this._srcWidth, (float)this._srcHeight)
			};
		}

		// Token: 0x06001779 RID: 6009 RVA: 0x00010E78 File Offset: 0x0000F078
		public int getCountPerSheet()
		{
			return this._perSheet;
		}

		// Token: 0x040014D8 RID: 5336
		private readonly int _dstWidth;

		// Token: 0x040014D9 RID: 5337
		private readonly int _dstHeight;

		// Token: 0x040014DA RID: 5338
		private readonly int _srcWidth;

		// Token: 0x040014DB RID: 5339
		private readonly int _srcHeight;

		// Token: 0x040014DC RID: 5340
		private int _rows;

		// Token: 0x040014DD RID: 5341
		private int _cols;

		// Token: 0x040014DE RID: 5342
		private int _perSheet;

		// Token: 0x02000421 RID: 1057
		public class Pos
		{
			// Token: 0x040014DF RID: 5343
			public Rect rect;

			// Token: 0x040014E0 RID: 5344
			public int sheet;

			// Token: 0x040014E1 RID: 5345
			public bool firstInSheet;

			// Token: 0x040014E2 RID: 5346
			public bool lastInSheet;
		}
	}
}
