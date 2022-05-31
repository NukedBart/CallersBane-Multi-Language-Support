using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000051 RID: 81
public class BattlePopup : MonoBehaviour
{
	// Token: 0x060003A4 RID: 932 RVA: 0x000047CA File Offset: 0x000029CA
	private void Start()
	{
		this.startTime = Time.time;
	}

	// Token: 0x060003A5 RID: 933 RVA: 0x000047CA File Offset: 0x000029CA
	public void init()
	{
		this.startTime = Time.time;
	}

	// Token: 0x060003A6 RID: 934 RVA: 0x0002DF10 File Offset: 0x0002C110
	protected float getT()
	{
		float num = Time.time - (this.startTime + this.delay);
		return num / this.lifeTime;
	}

	// Token: 0x060003A7 RID: 935 RVA: 0x0002DF3C File Offset: 0x0002C13C
	private void Update()
	{
		float t = this.getT();
		if (t < 0f)
		{
			return;
		}
		if (!this.inited)
		{
			this.setup();
			this.inited = true;
		}
		if (t > 1f)
		{
			Object.Destroy(base.gameObject);
			return;
		}
		this.update(t);
	}

	// Token: 0x060003A8 RID: 936 RVA: 0x000028DF File Offset: 0x00000ADF
	protected virtual void setup()
	{
	}

	// Token: 0x060003A9 RID: 937 RVA: 0x0002DF94 File Offset: 0x0002C194
	protected virtual void update(float t)
	{
		foreach (GameObject gameObject in this.objects)
		{
			Color color = gameObject.renderer.material.color;
			float num = 1f;
			if (t < 0.2f)
			{
				num = 5f * t;
			}
			if (t > 0.75f)
			{
				num = 4f * (1f - t);
			}
			gameObject.renderer.material.color = ColorUtil.GetWithAlpha(color, num);
			gameObject.transform.Translate(0f, num * num * num * Time.deltaTime * 0.35f, 0f);
		}
	}

	// Token: 0x04000259 RID: 601
	private float startTime;

	// Token: 0x0400025A RID: 602
	private bool inited;

	// Token: 0x0400025B RID: 603
	protected float lifeTime = 1.5f;

	// Token: 0x0400025C RID: 604
	protected float delay;

	// Token: 0x0400025D RID: 605
	protected List<GameObject> objects = new List<GameObject>();
}
