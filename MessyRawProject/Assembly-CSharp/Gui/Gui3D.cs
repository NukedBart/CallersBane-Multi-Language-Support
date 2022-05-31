using System;
using System.Collections.Generic;
using UnityEngine;

namespace Gui
{
	// Token: 0x020001D9 RID: 473
	public class Gui3D : IGui
	{
		// Token: 0x06000EE2 RID: 3810 RVA: 0x00063B34 File Offset: 0x00061D34
		public Gui3D(Camera camera)
		{
			this._defaultMaterial = new Material(ResourceManager.LoadShader("Scrolls/Unlit/Transparent"));
			this.setCamera(camera);
			this.setBaseDepth(10f);
		}

		// Token: 0x06000EE4 RID: 3812 RVA: 0x00063BB8 File Offset: 0x00061DB8
		public void setRenderQueue(int renderQueue, bool retroactive)
		{
			this._renderQueue = renderQueue;
			if (retroactive)
			{
				for (int i = 0; i < this._current; i++)
				{
					this._images[i].g.renderer.material.renderQueue = renderQueue;
				}
			}
		}

		// Token: 0x06000EE5 RID: 3813 RVA: 0x0000BF6A File Offset: 0x0000A16A
		public int getRenderQueue()
		{
			return this._renderQueue;
		}

		// Token: 0x06000EE6 RID: 3814 RVA: 0x0000BF72 File Offset: 0x0000A172
		public void setCamera(Camera camera)
		{
			this._cam = camera;
		}

		// Token: 0x06000EE7 RID: 3815 RVA: 0x0000BF7B File Offset: 0x0000A17B
		public Camera getCamera()
		{
			return this._cam;
		}

		// Token: 0x06000EE8 RID: 3816 RVA: 0x0000BF83 File Offset: 0x0000A183
		public Gui3D setBaseDepth(float depth)
		{
			this._baseDepth = depth;
			this.setDepth(depth);
			return this;
		}

		// Token: 0x06000EE9 RID: 3817 RVA: 0x0000BF94 File Offset: 0x0000A194
		public Gui3D setDefaultMaterial(Material material)
		{
			this._defaultMaterial = material;
			return this;
		}

		// Token: 0x06000EEA RID: 3818 RVA: 0x00063C0C File Offset: 0x00061E0C
		public void setLayer(int layer)
		{
			this._layer = layer;
			for (int i = 0; i < this._images.Count; i++)
			{
				this._images[i].g.layer = layer;
			}
		}

		// Token: 0x06000EEB RID: 3819 RVA: 0x0000BF9E File Offset: 0x0000A19E
		public void setDepth(float depth)
		{
			this._depth = depth;
		}

		// Token: 0x06000EEC RID: 3820 RVA: 0x0000BFA7 File Offset: 0x0000A1A7
		public float getDepth()
		{
			return this.getDepth(false);
		}

		// Token: 0x06000EED RID: 3821 RVA: 0x0000BFB0 File Offset: 0x0000A1B0
		public float getDepth(bool includeBaseDepth)
		{
			return (!includeBaseDepth) ? this._depth : (this._baseDepth + this._depth);
		}

		// Token: 0x06000EEE RID: 3822 RVA: 0x0000BFD0 File Offset: 0x0000A1D0
		public void frameBegin()
		{
			this._current = 0;
			this._numExternal = 0;
			this.setDepth(this._baseDepth);
		}

		// Token: 0x06000EEF RID: 3823 RVA: 0x00063C54 File Offset: 0x00061E54
		public void frameEnd()
		{
			for (int i = this._current; i < this._numRendered; i++)
			{
				this._images[i].g.SetActive(false);
			}
			this._numRendered = this._current;
		}

		// Token: 0x06000EF0 RID: 3824 RVA: 0x00063CA0 File Offset: 0x00061EA0
		public void blit(Rect rect, Texture tex, Rect texCoords)
		{
			this._assureSize(this._current + 1);
			Gui3D.ImObj imObj = this._images[this._current];
			GameObject g = imObj.g;
			g.SetActive(true);
			this._setup(g, rect);
			Texture mainTexture = g.renderer.material.mainTexture;
			if (null == mainTexture || mainTexture.GetNativeTextureID() != tex.GetNativeTextureID())
			{
				g.renderer.material.mainTexture = tex;
			}
			if (!texCoords.Equals(imObj.uvRect))
			{
				g.renderer.material.mainTextureOffset = new Vector2(texCoords.x, texCoords.y);
				g.renderer.material.mainTextureScale = new Vector2(texCoords.width, texCoords.height);
				imObj.uvRect = texCoords;
			}
			g.renderer.material.color = this._defaultColor;
			this._current++;
		}

		// Token: 0x06000EF1 RID: 3825 RVA: 0x0000BFEC File Offset: 0x0000A1EC
		public void SetColor(Color color)
		{
			this._defaultColor = color;
		}

		// Token: 0x06000EF2 RID: 3826 RVA: 0x0000BFF5 File Offset: 0x0000A1F5
		public Color GetColor()
		{
			return this._defaultColor;
		}

		// Token: 0x06000EF3 RID: 3827 RVA: 0x0000BFFD File Offset: 0x0000A1FD
		public void DrawTexture(Rect dst, Texture tex)
		{
			this.blit(dst, tex, Gui3D._unitRect);
		}

		// Token: 0x06000EF4 RID: 3828 RVA: 0x0000C00C File Offset: 0x0000A20C
		public void DrawTextureWithTexCoords(Rect dst, Texture tex, Rect texCoords)
		{
			this.blit(dst, tex, texCoords);
		}

		// Token: 0x06000EF5 RID: 3829 RVA: 0x0000C017 File Offset: 0x0000A217
		public void DrawObject(Rect dst, GameObject go)
		{
			this._numExternal++;
			this.orientPlane(go);
			this._setup(go, dst);
		}

		// Token: 0x06000EF6 RID: 3830 RVA: 0x0000C036 File Offset: 0x0000A236
		public void DrawObject(float x, float y, GameObject go)
		{
			this.DrawObject(new Rect(x, y, 0f, 0f), go);
		}

		// Token: 0x06000EF7 RID: 3831 RVA: 0x0000C050 File Offset: 0x0000A250
		public Material GetLastMaterial()
		{
			if (this._current == 0)
			{
				Log.error("No such material!");
			}
			return this._images[this._current - 1].g.renderer.material;
		}

		// Token: 0x06000EF8 RID: 3832 RVA: 0x00063DAC File Offset: 0x00061FAC
		private void _setup(GameObject g, Rect r)
		{
			if (!this._cam.isOrthoGraphic)
			{
				throw new NotImplementedException("Only supporting ortographic cameras");
			}
			Gui3D.PosData position = this.getPosition(r);
			g.transform.localPosition = position.pos;
			if (r.width > 0f && r.height > 0f)
			{
				g.transform.localScale = new Vector3(position.w, 1f, position.h);
			}
		}

		// Token: 0x06000EF9 RID: 3833 RVA: 0x0000C089 File Offset: 0x0000A289
		public Gui3D.PosData getPosition(float x, float y)
		{
			return this.getPosition(new Vector2(x, y));
		}

		// Token: 0x06000EFA RID: 3834 RVA: 0x0000C098 File Offset: 0x0000A298
		public Gui3D.PosData getPosition(Vector2 p)
		{
			return this.getPosition(new Rect(p.x, p.y, 0f, 0f));
		}

		// Token: 0x06000EFB RID: 3835 RVA: 0x00063E30 File Offset: 0x00062030
		public Gui3D.PosData getPosition(Rect r)
		{
			float num = this._getCurrentDepth();
			float num2 = this._cam.orthographicSize * 2f;
			float num3 = num2 * this._cam.aspect;
			float num4 = num3 / ((!this._isFixedSize) ? ((float)Screen.width) : this._fixedWidth);
			float num5 = num2 / ((!this._isFixedSize) ? ((float)Screen.height) : this._fixedHeight);
			float num6 = r.width * num4;
			float num7 = r.height * num5;
			float num8 = (r.x + this.currentTransform.x) * num4 + 0.5f * (num6 - num3);
			float num9 = (r.y + this.currentTransform.y) * num5 + 0.5f * (num7 - num2);
			Vector3 pos;
			pos..ctor(num8, -num9, num);
			if (r.width > 0f && r.height > 0f)
			{
				return new Gui3D.PosData(pos, 0.1f * num6, 0.1f * num7);
			}
			return new Gui3D.PosData(pos);
		}

		// Token: 0x06000EFC RID: 3836 RVA: 0x00063F50 File Offset: 0x00062150
		private void _assureSize(int size)
		{
			if (size > this._images.Count && size > 1 && (size & 127) == 0)
			{
				Debug.LogWarning("GUI3D. ImagePool size: " + size + ". Have you forgot to call frameEnd?");
			}
			for (int i = this._images.Count; i < size; i++)
			{
				this._addImage();
			}
		}

		// Token: 0x06000EFD RID: 3837 RVA: 0x00063FBC File Offset: 0x000621BC
		private void _addImage()
		{
			GameObject gameObject = PrimitiveFactory.createPlane(false);
			gameObject.name = "Gui3D element";
			gameObject.layer = this._layer;
			this.orientPlane(gameObject);
			gameObject.renderer.material = this._defaultMaterial;
			if (this._renderQueue >= 0)
			{
				gameObject.renderer.material.renderQueue = this._renderQueue;
			}
			gameObject.renderer.material.color = this._defaultColor;
			this._images.Add(new Gui3D.ImObj(gameObject));
		}

		// Token: 0x06000EFE RID: 3838 RVA: 0x0000C0BD File Offset: 0x0000A2BD
		public void orientPlane(GameObject g)
		{
			g.transform.parent = this._cam.transform;
			g.transform.localPosition = Vector3.zero;
			g.transform.localEulerAngles = Gui3D.getFaceCameraRotation();
		}

		// Token: 0x06000EFF RID: 3839 RVA: 0x0000C0F5 File Offset: 0x0000A2F5
		private float _getCurrentDepth()
		{
			return this._depth - 0.001f * (float)(this._current + this._numExternal);
		}

		// Token: 0x06000F00 RID: 3840 RVA: 0x0000C112 File Offset: 0x0000A312
		public void setViewportSize(float width, float height)
		{
			this._isFixedSize = true;
			this._fixedWidth = width;
			this._fixedHeight = height;
		}

		// Token: 0x06000F01 RID: 3841 RVA: 0x0000C129 File Offset: 0x0000A329
		public void pushTransform()
		{
			this.transforms.Add(this.currentTransform);
		}

		// Token: 0x06000F02 RID: 3842 RVA: 0x0000C13C File Offset: 0x0000A33C
		public void popTransform()
		{
			if (this.transforms.Count == 0)
			{
				Debug.LogError("Gui3D transform stack is exhausted!");
			}
			else
			{
				this.currentTransform = this.transforms[0];
				this.transforms.RemoveAt(0);
			}
		}

		// Token: 0x06000F03 RID: 3843 RVA: 0x0000C17B File Offset: 0x0000A37B
		public void resetTransform()
		{
			this.currentTransform = Vector2.zero;
		}

		// Token: 0x06000F04 RID: 3844 RVA: 0x0000C188 File Offset: 0x0000A388
		public void translate(float x, float y)
		{
			this.currentTransform.x = this.currentTransform.x + x;
			this.currentTransform.y = this.currentTransform.y + y;
		}

		// Token: 0x06000F05 RID: 3845 RVA: 0x0000C1B0 File Offset: 0x0000A3B0
		public static Vector3 getFaceCameraRotation()
		{
			return Gui3D._rotation_FaceCamera;
		}

		// Token: 0x04000B79 RID: 2937
		private static Vector3 _rotation_FaceCamera = new Vector3(90f, 180f, 0f);

		// Token: 0x04000B7A RID: 2938
		private int _layer;

		// Token: 0x04000B7B RID: 2939
		private List<Gui3D.ImObj> _images = new List<Gui3D.ImObj>();

		// Token: 0x04000B7C RID: 2940
		private Camera _cam;

		// Token: 0x04000B7D RID: 2941
		private float _depth;

		// Token: 0x04000B7E RID: 2942
		private float _baseDepth = 10f;

		// Token: 0x04000B7F RID: 2943
		private int _renderQueue = -1;

		// Token: 0x04000B80 RID: 2944
		private Material _defaultMaterial;

		// Token: 0x04000B81 RID: 2945
		private Color _defaultColor = Color.white;

		// Token: 0x04000B82 RID: 2946
		private static Rect _unitRect = new Rect(0f, 0f, 1f, 1f);

		// Token: 0x04000B83 RID: 2947
		private bool _isFixedSize;

		// Token: 0x04000B84 RID: 2948
		private float _fixedWidth;

		// Token: 0x04000B85 RID: 2949
		private float _fixedHeight;

		// Token: 0x04000B86 RID: 2950
		private List<Vector2> transforms = new List<Vector2>();

		// Token: 0x04000B87 RID: 2951
		private Vector2 currentTransform = new Vector2(0f, 0f);

		// Token: 0x04000B88 RID: 2952
		private int _current;

		// Token: 0x04000B89 RID: 2953
		private int _numExternal;

		// Token: 0x04000B8A RID: 2954
		private int _numRendered;

		// Token: 0x020001DA RID: 474
		public class PosData
		{
			// Token: 0x06000F06 RID: 3846 RVA: 0x0000C1B7 File Offset: 0x0000A3B7
			public PosData(Vector3 pos) : this(pos, 0f, 0f)
			{
			}

			// Token: 0x06000F07 RID: 3847 RVA: 0x0000C1CA File Offset: 0x0000A3CA
			public PosData(Vector3 pos, float w, float h)
			{
				this.pos = pos;
				this.w = w;
				this.h = h;
			}

			// Token: 0x04000B8B RID: 2955
			public Vector3 pos;

			// Token: 0x04000B8C RID: 2956
			public float w;

			// Token: 0x04000B8D RID: 2957
			public float h;
		}

		// Token: 0x020001DB RID: 475
		private class ImObj
		{
			// Token: 0x06000F08 RID: 3848 RVA: 0x0000C1E7 File Offset: 0x0000A3E7
			public ImObj(GameObject gameObject)
			{
				this.g = gameObject;
				this.uvRect = Gui3D._unitRect;
			}

			// Token: 0x04000B8E RID: 2958
			public GameObject g;

			// Token: 0x04000B8F RID: 2959
			public Rect uvRect;
		}
	}
}
