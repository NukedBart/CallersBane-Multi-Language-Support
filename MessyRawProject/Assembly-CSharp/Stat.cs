using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x020000CE RID: 206
public class Stat
{
	// Token: 0x06000709 RID: 1801 RVA: 0x0003F4D4 File Offset: 0x0003D6D4
	public Stat(GameObject parent, Stat.Type type, Vector3 pos)
	{
		this.type = type;
		string text = type.ToString().ToLower();
		this.root = new GameObject(text);
		UnityUtil.addChild(parent, this.root);
		this.root.transform.localPosition = pos;
		this.root.transform.localScale = Vector3.one * 0.004f;
		this.box = PrimitiveFactory.createTexturedPlane("BattleUI/stats/statbox", true);
		this.box.name = "box";
		UnityUtil.addChild(this.root, this.box);
		this.box.transform.localScale = Stat.fromPixels(97f, 39f);
		this.icon = PrimitiveFactory.createTexturedPlane("BattleUI/stats/icon_" + text, true);
		this.icon.name = "icon";
		UnityUtil.addChild(this.root, this.icon);
		this.icon.transform.localPosition = new Vector3(260f, 100f, 0f);
		this.icon.transform.localScale = Stat.fromPixels(58f, 54f) * 0.931f;
		if (Stat.canHaveIconBackground(type))
		{
			this.iconBg = PrimitiveFactory.createTexturedPlane("BattleUI/stats/icon_health_armor", true);
			this.iconBg.name = "icon_bg";
			this.iconBg.SetActive(false);
			UnityUtil.addChild(this.root, this.iconBg);
			this.iconBg.transform.localPosition = new Vector3(200f, 40f, 0f);
			this.iconBg.transform.localScale = Stat.fromPixels(58f, 54f);
		}
		this.digits = new GameObject("digits");
		this.digits.AddComponent<DigitGrower>();
		UnityUtil.addChild(this.root, this.digits);
		this.digits.transform.localScale = new Vector3(1.4f, 1f, 1.4f);
		this.digits.transform.localPosition = new Vector3(-200f, 30f, -20f);
	}

	// Token: 0x0600070A RID: 1802 RVA: 0x000066E8 File Offset: 0x000048E8
	public static Vector3 fromPixels(float width, float height)
	{
		return new Vector3(width, 1f, height);
	}

	// Token: 0x0600070B RID: 1803 RVA: 0x00005376 File Offset: 0x00003576
	public static bool canHaveIconBackground(Stat.Type type)
	{
		return type == Stat.Type.Health;
	}

	// Token: 0x0600070C RID: 1804 RVA: 0x0003F724 File Offset: 0x0003D924
	public void showIconBackground(bool show)
	{
		if (this.iconBg.activeSelf == show)
		{
			return;
		}
		this.iconBg.SetActive(show);
		this.icon.transform.localScale = Stat.fromPixels(58f, 54f) * ((!show) ? 1f : 0.75f);
	}

	// Token: 0x0600070D RID: 1805 RVA: 0x0003F788 File Offset: 0x0003D988
	public List<GameObject> getGameObjectsWithRenderers()
	{
		List<GameObject> list = new List<GameObject>();
		list.Add(this.box);
		list.Add(this.icon);
		List<GameObject> list2 = list;
		if (Stat.canHaveIconBackground(this.type))
		{
			list2.Add(this.iconBg);
		}
		return list2;
	}

	// Token: 0x0400050E RID: 1294
	public GameObject root;

	// Token: 0x0400050F RID: 1295
	public GameObject box;

	// Token: 0x04000510 RID: 1296
	public GameObject icon;

	// Token: 0x04000511 RID: 1297
	public GameObject digits;

	// Token: 0x04000512 RID: 1298
	public GameObject iconBg;

	// Token: 0x04000513 RID: 1299
	private Stat.Type type;

	// Token: 0x020000CF RID: 207
	public enum Type
	{
		// Token: 0x04000515 RID: 1301
		Attack,
		// Token: 0x04000516 RID: 1302
		Health,
		// Token: 0x04000517 RID: 1303
		Countdown
	}
}
