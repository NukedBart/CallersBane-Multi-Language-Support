using System;
using System.Collections.Generic;
using Animation.Serialization;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x02000016 RID: 22
public class FrameAnimation
{
	// Token: 0x0600014E RID: 334 RVA: 0x000030FB File Offset: 0x000012FB
	public FrameAnimation()
	{
	}

	// Token: 0x0600014F RID: 335 RVA: 0x0001E79C File Offset: 0x0001C99C
	public FrameAnimation(AnimData animData)
	{
		this.setData(animData, null);
	}

	// Token: 0x06000150 RID: 336 RVA: 0x00003136 File Offset: 0x00001336
	public void setData(AnimData data)
	{
		this.setData(data, null);
	}

	// Token: 0x06000151 RID: 337 RVA: 0x0001E7EC File Offset: 0x0001C9EC
	public void setData(AnimData data, AnimPartDataModifier modifier)
	{
		this._data = data;
		this._modifier = modifier;
		this._numFrames = data.getNumFrames();
		this._rotMatrix = this._getRotMatrix(0.01f);
		this._buildLocators();
		if (this._modifier != null)
		{
			this._combined = null;
		}
	}

	// Token: 0x06000152 RID: 338 RVA: 0x00003140 File Offset: 0x00001340
	public int getFrame()
	{
		return this._currentFrame;
	}

	// Token: 0x06000153 RID: 339 RVA: 0x00003148 File Offset: 0x00001348
	public void setFrame(int frame)
	{
		this.setFrame(frame);
	}

	// Token: 0x06000154 RID: 340 RVA: 0x0001E83C File Offset: 0x0001CA3C
	public void setFrame(int frame, bool roundTrip)
	{
		this._hitLocators.Clear();
		if (frame != this._currentFrame)
		{
			int currentFrame = this._currentFrame;
			this._currentFrame = frame;
			if (roundTrip)
			{
				this._addLocators(currentFrame + 1, this.getNumFrames() - 1);
				this._addLocators(0, currentFrame);
			}
			else if (frame > currentFrame)
			{
				this._addLocators(currentFrame + 1, frame);
			}
			else if (frame < currentFrame)
			{
				this._addLocators(currentFrame + 1, this.getNumFrames() - 1);
				this._addLocators(0, frame);
			}
			if (this._modifier != null)
			{
				this.Destroy();
				this._combined = this._data.buildMeshForFrame(frame, this._modifier);
			}
			else
			{
				this._combined = this._data.getMeshForFrame(frame);
			}
		}
	}

	// Token: 0x06000155 RID: 341 RVA: 0x00003151 File Offset: 0x00001351
	public void drawAt(Transform transform, Material material, int layer)
	{
		this.drawAt(transform, material, new Vector3(1f, 1f, 1f), layer);
	}

	// Token: 0x06000156 RID: 342 RVA: 0x0001E908 File Offset: 0x0001CB08
	public void drawAt(Transform transform, Material material, Vector3 scale, int layer)
	{
		if (this._data == null)
		{
			return;
		}
		Matrix4x4 matrix4x = GraphicsUtils.multMatrix(Matrix4x4.TRS(transform.position, transform.rotation, scale), this._rotMatrix);
		Graphics.DrawMesh(this._combined, matrix4x, material, layer);
	}

	// Token: 0x06000157 RID: 343 RVA: 0x00003170 File Offset: 0x00001370
	public void setFps(float fps)
	{
		this._fps = fps;
	}

	// Token: 0x06000158 RID: 344 RVA: 0x0001E950 File Offset: 0x0001CB50
	public float getFps()
	{
		if (this._fps != -1E+10f)
		{
			return this._fps;
		}
		try
		{
			return this._data.fps;
		}
		catch (Exception s)
		{
			Log.warning(s);
		}
		return 24f;
	}

	// Token: 0x06000159 RID: 345 RVA: 0x00003179 File Offset: 0x00001379
	public int getNumFrames()
	{
		return this._numFrames;
	}

	// Token: 0x0600015A RID: 346 RVA: 0x00003181 File Offset: 0x00001381
	public float getLength()
	{
		return (float)this._numFrames / this.getFps();
	}

	// Token: 0x0600015B RID: 347 RVA: 0x0001E9B0 File Offset: 0x0001CBB0
	public List<AnimLocator> getLocatorsForFrames(int from, int to)
	{
		if (this._data == null)
		{
			Log.error("data is null");
		}
		List<AnimLocator> list = new List<AnimLocator>();
		foreach (AnimLocator animLocator in this._animLocators)
		{
			if (animLocator.frame >= from && animLocator.frame <= to)
			{
				list.Add(animLocator);
			}
		}
		return list;
	}

	// Token: 0x0600015C RID: 348 RVA: 0x00003191 File Offset: 0x00001391
	public List<AnimLocator> getLocatorsUpToFrame(int frame)
	{
		return this.getLocatorsForFrames(this._currentFrame + 1, frame);
	}

	// Token: 0x0600015D RID: 349 RVA: 0x000031A2 File Offset: 0x000013A2
	public List<AnimLocator> getLocatorsForCurrentFrame()
	{
		return this.getLocatorsForFrames(this._currentFrame, this._currentFrame);
	}

	// Token: 0x0600015E RID: 350 RVA: 0x000031B6 File Offset: 0x000013B6
	public List<AnimLocator> getLocators()
	{
		return this._animLocators;
	}

	// Token: 0x0600015F RID: 351 RVA: 0x000031BE File Offset: 0x000013BE
	public List<AnimLocator> getHitLocators()
	{
		return this._hitLocators;
	}

	// Token: 0x06000160 RID: 352 RVA: 0x000031C6 File Offset: 0x000013C6
	private Matrix4x4 _getRotMatrix(float scale)
	{
		return this._getRotMatrix(scale, scale, scale);
	}

	// Token: 0x06000161 RID: 353 RVA: 0x000031D1 File Offset: 0x000013D1
	private Matrix4x4 _getRotMatrix(float scalex, float scaley, float scalez)
	{
		return Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0f, 180f, 180f), new Vector3(scalex, scaley, scalez));
	}

	// Token: 0x06000162 RID: 354 RVA: 0x000031F9 File Offset: 0x000013F9
	private void _addLocators(int from, int to)
	{
		this._hitLocators.AddRange(this.getLocatorsForFrames(from, to));
	}

	// Token: 0x06000163 RID: 355 RVA: 0x0001EA40 File Offset: 0x0001CC40
	private void _buildLocators()
	{
		this._animLocators = new List<AnimLocator>();
		foreach (PD_AnimLocator locator in this._data.getAnimLocators())
		{
			this._animLocators.Add(new AnimLocator(locator));
		}
	}

	// Token: 0x06000164 RID: 356 RVA: 0x0000320E File Offset: 0x0000140E
	internal void Destroy()
	{
		if (this._combined == null)
		{
			return;
		}
		if (this._modifier != null)
		{
			Object.Destroy(this._combined);
		}
		this._combined = null;
	}

	// Token: 0x04000094 RID: 148
	private const float NO_FPS = -1E+10f;

	// Token: 0x04000095 RID: 149
	private float _fps = -1E+10f;

	// Token: 0x04000096 RID: 150
	private int _numFrames = 1;

	// Token: 0x04000097 RID: 151
	private int _currentFrame = -999;

	// Token: 0x04000098 RID: 152
	private AnimData _data;

	// Token: 0x04000099 RID: 153
	private AnimPartDataModifier _modifier;

	// Token: 0x0400009A RID: 154
	private Mesh _combined;

	// Token: 0x0400009B RID: 155
	private Matrix4x4 _rotMatrix;

	// Token: 0x0400009C RID: 156
	private List<AnimLocator> _animLocators = new List<AnimLocator>();

	// Token: 0x0400009D RID: 157
	private List<AnimLocator> _hitLocators = new List<AnimLocator>();
}
