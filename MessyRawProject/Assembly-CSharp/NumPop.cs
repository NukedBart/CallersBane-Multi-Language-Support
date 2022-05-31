using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000092 RID: 146
public class NumPop : MonoBehaviour
{
	// Token: 0x0600055E RID: 1374 RVA: 0x00038FA0 File Offset: 0x000371A0
	public void init(string resourceName, Vector3 pos, float delay)
	{
		this.startTime = Time.time;
		this.delay = delay;
		this.alpha = 4f;
		base.gameObject.name = "NumPop_" + resourceName;
		base.transform.name = "NumPop_" + resourceName;
		Material material = new Material(ResourceManager.LoadShader("Scrolls/Transparent/Diffuse/Double"));
		material.renderQueue = 95500;
		float size = 0.05f;
		this.addIcon(resourceName, pos, material, size);
	}

	// Token: 0x0600055F RID: 1375 RVA: 0x00039024 File Offset: 0x00037224
	public void init(int num, Vector3 pos, string color, float delay)
	{
		this.startTime = Time.time;
		this.delay = delay;
		this.alpha = 4f;
		base.gameObject.name = "NumPop_" + num;
		base.transform.name = "NumPop_" + num;
		Material material = new Material(ResourceManager.LoadShader("Scrolls/Transparent/Diffuse/Double"));
		material.renderQueue = 95500;
		float size = 0.04f + Math.Min((float)num * 0.01f, 0.05f);
		char[] array = Convert.ToString(num).ToCharArray();
		this.numTweens = array.Length;
		for (int i = 0; i < array.Length; i++)
		{
			this.addIcon(color + "_" + array[i], new Vector3(pos.x, pos.y, pos.z + (float)i * 0.15f), material, size);
		}
	}

	// Token: 0x06000560 RID: 1376 RVA: 0x00039120 File Offset: 0x00037320
	private void addIcon(string resourceName, Vector3 pos, Material material, float size)
	{
		GameObject gameObject = PrimitiveFactory.createPlane(false);
		gameObject.transform.localScale = new Vector3(0f, 1f, 0f);
		gameObject.transform.eulerAngles = new Vector3(39f, 90f, 0f);
		gameObject.transform.localPosition = pos;
		gameObject.renderer.material = material;
		gameObject.renderer.material.mainTexture = ResourceManager.LoadTexture("Numbers/" + resourceName);
		iTween.ScaleTo(gameObject, iTween.Hash(new object[]
		{
			"x",
			size,
			"z",
			size,
			"time",
			0.7f,
			"easetype",
			iTween.EaseType.elastic,
			"oncompletetarget",
			base.gameObject,
			"oncomplete",
			"tweeningDone",
			"delay",
			this.delay
		}));
		this.numArr.Add(gameObject);
	}

	// Token: 0x06000561 RID: 1377 RVA: 0x0000567D File Offset: 0x0000387D
	private void tweeningDone()
	{
		this.numTweens--;
	}

	// Token: 0x06000562 RID: 1378 RVA: 0x00039250 File Offset: 0x00037450
	private void FixedUpdate()
	{
		if (Time.time >= this.startTime + this.delay)
		{
			this.alpha -= 0.06f;
			for (int i = 0; i < this.numArr.Count; i++)
			{
				Color withAlpha = ColorUtil.GetWithAlpha(this.numArr[i].renderer.material.color, this.alpha);
				this.numArr[i].renderer.material.color = withAlpha;
				this.numArr[i].transform.Translate(0f, 0f, -0.01f);
			}
			if (this.alpha <= 0f)
			{
				for (int j = 0; j < this.numArr.Count; j++)
				{
					Object.Destroy(this.numArr[j]);
				}
				Object.Destroy(base.gameObject);
			}
		}
	}

	// Token: 0x040003CF RID: 975
	private GameObject cube;

	// Token: 0x040003D0 RID: 976
	private float alpha;

	// Token: 0x040003D1 RID: 977
	private List<GameObject> numArr = new List<GameObject>();

	// Token: 0x040003D2 RID: 978
	private int numTweens;

	// Token: 0x040003D3 RID: 979
	private float startTime;

	// Token: 0x040003D4 RID: 980
	private float delay;

	// Token: 0x02000093 RID: 147
	public class PosTime
	{
		// Token: 0x06000563 RID: 1379 RVA: 0x0000568D File Offset: 0x0000388D
		public PosTime(Vector3 position, float time)
		{
			this.position = position;
			this.time = time;
		}

		// Token: 0x040003D5 RID: 981
		public Vector3 position;

		// Token: 0x040003D6 RID: 982
		public float time;
	}
}
