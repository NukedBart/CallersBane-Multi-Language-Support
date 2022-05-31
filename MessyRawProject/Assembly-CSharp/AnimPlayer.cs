using System;
using System.Collections.Generic;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class AnimPlayer
{
	// Token: 0x06000137 RID: 311 RVA: 0x00002F75 File Offset: 0x00001175
	public AnimPlayer(iTween.ITimer timer)
	{
		this._timer = timer;
	}

	// Token: 0x06000138 RID: 312 RVA: 0x00002FA8 File Offset: 0x000011A8
	public void setDescription(UnitAnimDescription unitDesc, iAnim callBackTarget)
	{
		this.setDescription(unitDesc, callBackTarget, null);
	}

	// Token: 0x06000139 RID: 313 RVA: 0x00002FB3 File Offset: 0x000011B3
	public void setDescription(UnitAnimDescription unitDesc, iAnim callBackTarget, AnimPartDataModifier modifier)
	{
		this._unitDesc = unitDesc;
		this._callBackTarget = callBackTarget;
		this._animModifier = modifier;
	}

	// Token: 0x0600013A RID: 314 RVA: 0x00002FCA File Offset: 0x000011CA
	public void setAnimationId(string p)
	{
		this.lastAnim = p;
		if (this._unitDesc == null)
		{
			return;
		}
		if (this.hasAnimationId(p))
		{
			this.setAnimationId(this._unitDesc.getAnimationId(p));
		}
	}

	// Token: 0x0600013B RID: 315 RVA: 0x00002FFD File Offset: 0x000011FD
	public UnitAnimDescription getUnitDesc()
	{
		return this._unitDesc;
	}

	// Token: 0x0600013C RID: 316 RVA: 0x00003005 File Offset: 0x00001205
	public void setSpeed(float s)
	{
		this._speed = s;
	}

	// Token: 0x0600013D RID: 317 RVA: 0x0001E4D0 File Offset: 0x0001C6D0
	public void setAnimationId(int id)
	{
		this._currentAnimId = id;
		this._hasCalledCallback = false;
		if (this._unitDesc == null)
		{
			return;
		}
		AnimData animation = this._unitDesc.getAnimation(id);
		if (animation == null)
		{
			return;
		}
		if (this._anim != null)
		{
			this._anim.Destroy();
		}
		this._anim = new FrameAnimation(animation);
		this._fframe = 0f;
		this.lastAnim = animation.name;
	}

	// Token: 0x0600013E RID: 318 RVA: 0x0000300E File Offset: 0x0000120E
	public string[] getAnimationNames()
	{
		return this._unitDesc.getAnimationNames();
	}

	// Token: 0x0600013F RID: 319 RVA: 0x0001E544 File Offset: 0x0001C744
	public void UpdateOnly()
	{
		if (this._anim == null)
		{
			return;
		}
		if (this._waitTilNextUpdate)
		{
			this._waitTilNextUpdate = false;
		}
		else
		{
			this.setFrame(this._fframe + this._speed * this._timer.getDeltaFrameTime() * this._anim.getFps());
		}
		if (this._callBackTarget != null && !this._hasCalledCallback && this._fframe >= (float)this._anim.getNumFrames())
		{
			this._hasCalledCallback = true;
			this._anim.setFrame(this._anim.getNumFrames() - 1, false);
			this._postLocators(this._anim.getHitLocators());
			this._callBackTarget.animationDone(this);
		}
		if (this._hasCalledCallback)
		{
			return;
		}
		this._lastUpdatedFrame = (int)this._fframe;
		if (this._lastUpdatedFrame >= this._anim.getNumFrames())
		{
			this._lastUpdatedFrame = this._anim.getNumFrames() - 1;
		}
		else if (this._lastUpdatedFrame < 0)
		{
			this._lastUpdatedFrame = 0;
		}
		this._anim.setFrame(this._lastUpdatedFrame, false);
		this._postLocators(this._anim.getHitLocators());
	}

	// Token: 0x06000140 RID: 320 RVA: 0x0001E688 File Offset: 0x0001C888
	private void _postLocators(List<AnimLocator> locators)
	{
		if (this._callBackTarget == null)
		{
			return;
		}
		foreach (AnimLocator loc in locators)
		{
			this._callBackTarget.locator(this, loc);
		}
	}

	// Token: 0x06000141 RID: 321 RVA: 0x0000301B File Offset: 0x0000121B
	public void Update(Transform transform, Material material)
	{
		this.Update(transform, material, new Vector3(1f, 1f, 1f));
	}

	// Token: 0x06000142 RID: 322 RVA: 0x00003039 File Offset: 0x00001239
	public void Update(Transform transform, Material material, Vector3 scale)
	{
		this.UpdateOnly();
		this.Draw(transform, material, scale);
	}

	// Token: 0x06000143 RID: 323 RVA: 0x0000304A File Offset: 0x0000124A
	public void Draw(Transform transform, Material material, Vector3 scale)
	{
		if (this._anim != null && this._fframe >= 0f)
		{
			this._anim.drawAt(transform, material, scale, this.layer);
		}
	}

	// Token: 0x06000144 RID: 324 RVA: 0x0000307B File Offset: 0x0000127B
	public void waitForUpdate()
	{
		this._waitTilNextUpdate = true;
	}

	// Token: 0x06000145 RID: 325 RVA: 0x00003084 File Offset: 0x00001284
	public void setFrame(float frame)
	{
		this._fframe = frame;
	}

	// Token: 0x06000146 RID: 326 RVA: 0x0000308D File Offset: 0x0000128D
	public int getFrame()
	{
		return this._lastUpdatedFrame;
	}

	// Token: 0x06000147 RID: 327 RVA: 0x0001E6F0 File Offset: 0x0001C8F0
	public bool hasLocator(params string[] names)
	{
		if (this._unitDesc == null || this._anim == null)
		{
			return false;
		}
		foreach (AnimLocator animLocator in this._anim.getLocators())
		{
			foreach (string text in names)
			{
				if (animLocator.name == text)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000148 RID: 328 RVA: 0x00003095 File Offset: 0x00001295
	public bool hasAnimationId(string p)
	{
		return this._unitDesc != null && this._unitDesc.getAnimationId(p) >= 0;
	}

	// Token: 0x06000149 RID: 329 RVA: 0x000030B6 File Offset: 0x000012B6
	public int getLastAnimationId()
	{
		return this._currentAnimId;
	}

	// Token: 0x0600014A RID: 330 RVA: 0x000030BE File Offset: 0x000012BE
	public FrameAnimation createFrameAnimation(int id)
	{
		return new FrameAnimation(this._unitDesc.getAnimation(id));
	}

	// Token: 0x0600014B RID: 331 RVA: 0x000030D1 File Offset: 0x000012D1
	public FrameAnimation createFrameAnimation()
	{
		return this.createFrameAnimation(this._currentAnimId);
	}

	// Token: 0x0600014C RID: 332 RVA: 0x000030DF File Offset: 0x000012DF
	public FrameAnimation createFrameAnimation(string name)
	{
		return this.createFrameAnimation(this._unitDesc.getAnimationId(name));
	}

	// Token: 0x0600014D RID: 333 RVA: 0x000030F3 File Offset: 0x000012F3
	public FrameAnimation getFrameAnimation()
	{
		return this._anim;
	}

	// Token: 0x04000087 RID: 135
	public string lastAnim = string.Empty;

	// Token: 0x04000088 RID: 136
	private bool _hasCalledCallback;

	// Token: 0x04000089 RID: 137
	private iAnim _callBackTarget;

	// Token: 0x0400008A RID: 138
	private UnitAnimDescription _unitDesc;

	// Token: 0x0400008B RID: 139
	private FrameAnimation _anim;

	// Token: 0x0400008C RID: 140
	private AnimPartDataModifier _animModifier;

	// Token: 0x0400008D RID: 141
	private int _currentAnimId = -1;

	// Token: 0x0400008E RID: 142
	private float _fframe;

	// Token: 0x0400008F RID: 143
	private int _lastUpdatedFrame = -1;

	// Token: 0x04000090 RID: 144
	private float _speed = 1f;

	// Token: 0x04000091 RID: 145
	public int layer;

	// Token: 0x04000092 RID: 146
	private bool _waitTilNextUpdate;

	// Token: 0x04000093 RID: 147
	private iTween.ITimer _timer;
}
