using System;
using UnityEngine;

// Token: 0x02000076 RID: 118
public class AnimConf
{
	// Token: 0x0600048B RID: 1163 RVA: 0x00004FB6 File Offset: 0x000031B6
	public AnimConf Bundle(string s)
	{
		this.bundle = s;
		return this;
	}

	// Token: 0x0600048C RID: 1164 RVA: 0x00004FC0 File Offset: 0x000031C0
	public AnimConf AnimId(int animId)
	{
		this.animId = animId;
		return this;
	}

	// Token: 0x0600048D RID: 1165 RVA: 0x00004FCA File Offset: 0x000031CA
	public AnimConf Loop()
	{
		return this.Loop(true);
	}

	// Token: 0x0600048E RID: 1166 RVA: 0x00004FD3 File Offset: 0x000031D3
	public AnimConf Loop(bool loop)
	{
		this.loop = loop;
		return this;
	}

	// Token: 0x0600048F RID: 1167 RVA: 0x00004FDD File Offset: 0x000031DD
	public AnimConf Layer(int layer)
	{
		this.layer = layer;
		return this;
	}

	// Token: 0x06000490 RID: 1168 RVA: 0x00004FE7 File Offset: 0x000031E7
	public AnimConf Scale(Vector3 s)
	{
		this.scale = s;
		return this;
	}

	// Token: 0x06000491 RID: 1169 RVA: 0x00004FF1 File Offset: 0x000031F1
	public AnimConf Scale(float k)
	{
		this.scale = Vector3.one * k;
		return this;
	}

	// Token: 0x06000492 RID: 1170 RVA: 0x00005005 File Offset: 0x00003205
	public AnimConf KScale(float k)
	{
		this.scale *= k;
		return this;
	}

	// Token: 0x06000493 RID: 1171 RVA: 0x0000501A File Offset: 0x0000321A
	public AnimConf Offset(Vector3 o)
	{
		this.pos += o;
		return this;
	}

	// Token: 0x06000494 RID: 1172 RVA: 0x0000502F File Offset: 0x0000322F
	public AnimConf RenderQueue(int rq)
	{
		this.renderQueue = rq;
		return this;
	}

	// Token: 0x06000495 RID: 1173 RVA: 0x00031D78 File Offset: 0x0002FF78
	public AnimConf ForwardOffset(Vector3 o)
	{
		this.pos.x = this.pos.x + o.x;
		this.pos.y = this.pos.y + o.y;
		this.pos.z = ((!this.flipX) ? (this.pos.z - o.z) : (this.pos.z + o.z));
		return this;
	}

	// Token: 0x06000496 RID: 1174 RVA: 0x00005039 File Offset: 0x00003239
	public AnimConf FlipX()
	{
		return this.FlipX(true);
	}

	// Token: 0x06000497 RID: 1175 RVA: 0x00005042 File Offset: 0x00003242
	public AnimConf FlipX(bool flipped)
	{
		this.flipX = flipped;
		return this;
	}

	// Token: 0x06000498 RID: 1176 RVA: 0x0000504C File Offset: 0x0000324C
	public AnimConf WaitForUpdate()
	{
		this.waitForUpdate = true;
		return this;
	}

	// Token: 0x040002EF RID: 751
	public string bundle;

	// Token: 0x040002F0 RID: 752
	public int animId;

	// Token: 0x040002F1 RID: 753
	public bool flipX;

	// Token: 0x040002F2 RID: 754
	public Vector3 scale = Vector3.one * 0.3f;

	// Token: 0x040002F3 RID: 755
	public Vector3 pos;

	// Token: 0x040002F4 RID: 756
	public Vector3 eulerAngles = new Vector3(51f, 270f, 0f);

	// Token: 0x040002F5 RID: 757
	public bool loop;

	// Token: 0x040002F6 RID: 758
	public int layer = -1;

	// Token: 0x040002F7 RID: 759
	public int renderQueue = -1;

	// Token: 0x040002F8 RID: 760
	public bool waitForUpdate;
}
