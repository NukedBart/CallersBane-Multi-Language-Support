using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x0200006D RID: 109
public class ScreenShake : MonoBehaviour
{
	// Token: 0x06000444 RID: 1092 RVA: 0x000028DF File Offset: 0x00000ADF
	private void Start()
	{
	}

	// Token: 0x06000445 RID: 1093 RVA: 0x00004C4A File Offset: 0x00002E4A
	public void init(Camera camera)
	{
		this.cam = camera;
		this.defaultRotation = this.cam.transform.localEulerAngles;
		this.defaultPosition = this.cam.transform.localPosition;
	}

	// Token: 0x06000446 RID: 1094 RVA: 0x00004C7F File Offset: 0x00002E7F
	public void shake(float x, float y)
	{
		this.shake(x, y, Vector2.zero, 0.5f);
	}

	// Token: 0x06000447 RID: 1095 RVA: 0x000314B0 File Offset: 0x0002F6B0
	public void shake(float x, float y, Vector2 bias, float duration)
	{
		Vector2 vector;
		vector..ctor(x, y);
		if (vector.sqrMagnitude < 0.01f)
		{
			return;
		}
		float num = this.durationMultiplier * Mathf.Sqrt(vector.magnitude);
		this.shakes.Add(new ScreenShake.ShakeData(x, y, duration * num, bias));
	}

	// Token: 0x06000448 RID: 1096 RVA: 0x00031504 File Offset: 0x0002F704
	private void Update()
	{
		if (this.cam == null)
		{
			return;
		}
		float time = Time.time;
		float num = time - this.lastTimeStamp;
		if (num < this.minimumWaitTime)
		{
			return;
		}
		this.lastTimeStamp = time;
		Vector2 vector = default(Vector2);
		List<ScreenShake.ShakeData> list = new List<ScreenShake.ShakeData>();
		foreach (ScreenShake.ShakeData shakeData in this.shakes)
		{
			vector += this.getOffsetFor(shakeData);
			if (!shakeData.isDone())
			{
				list.Add(shakeData);
			}
		}
		this.shakes = list;
		Vector3 localEulerAngles = this.defaultRotation + new Vector3(vector.y, 0f, 0f);
		base.camera.transform.localEulerAngles = localEulerAngles;
		Vector3 localPosition = this.defaultPosition + new Vector3(0f, 0f, vector.x) * 0.25f;
		base.camera.transform.localPosition = localPosition;
	}

	// Token: 0x06000449 RID: 1097 RVA: 0x0003163C File Offset: 0x0002F83C
	private Vector2 getOffsetFor(ScreenShake.ShakeData d)
	{
		Vector2 vector = this.strengthMultiplier * d.getBias();
		float num = d.angle += 2.5132742f + 1.2566371f * (float)this.rnd.NextDouble();
		float num2 = Mathf.Cos(num);
		float num3 = Mathf.Sin(num);
		float num4 = this.strengthMultiplier * d.left();
		Vector2 vector2 = num4 * (this.calculateOffset(d, 1f, 0.2f, 0f) + this.calculateOffset(d, 10f, 0.4f, 1000f));
		Vector2 vector3 = 0.05f * num4 * new Vector2(RandomUtil.random(-d.x, d.x), RandomUtil.random(-d.y, d.y));
		Vector2 vector4 = vector;
		if (this.randomHighFrequency)
		{
			vector4 += vector3;
		}
		if (this.lowFrequency)
		{
			vector4 += vector2 * 0.4f;
		}
		return vector4;
	}

	// Token: 0x0600044A RID: 1098 RVA: 0x00031750 File Offset: 0x0002F950
	private Vector2 calculateOffset(ScreenShake.ShakeData d, float speed, float mag, float distance)
	{
		float time = Time.time;
		float num = Mathf.PerlinNoise(speed * time, distance) - 0.5f;
		float num2 = Mathf.PerlinNoise(distance, time * speed) - 0.5f;
		return mag * new Vector2(d.x * num, d.y * num2);
	}

	// Token: 0x040002C8 RID: 712
	private List<ScreenShake.ShakeData> shakes = new List<ScreenShake.ShakeData>();

	// Token: 0x040002C9 RID: 713
	private Camera cam;

	// Token: 0x040002CA RID: 714
	private Vector3 defaultRotation;

	// Token: 0x040002CB RID: 715
	private Vector3 defaultPosition;

	// Token: 0x040002CC RID: 716
	[SerializeField]
	private float strengthMultiplier = 1f;

	// Token: 0x040002CD RID: 717
	[SerializeField]
	private float durationMultiplier = 0.2f;

	// Token: 0x040002CE RID: 718
	[SerializeField]
	private float minimumWaitTime = 0.026f;

	// Token: 0x040002CF RID: 719
	[SerializeField]
	private bool randomHighFrequency = true;

	// Token: 0x040002D0 RID: 720
	[SerializeField]
	private bool lowFrequency;

	// Token: 0x040002D1 RID: 721
	[SerializeField]
	private Vector2 bias = Vector2.zero;

	// Token: 0x040002D2 RID: 722
	private float lastTimeStamp;

	// Token: 0x040002D3 RID: 723
	private Random rnd = new Random();

	// Token: 0x0200006E RID: 110
	private class ShakeData
	{
		// Token: 0x0600044B RID: 1099 RVA: 0x00004C93 File Offset: 0x00002E93
		public ShakeData(float x, float y, float maxTime, Vector2 bias)
		{
			this.x = x;
			this.y = y;
			this.bias = bias;
			this._time = new TimeInfo(maxTime);
		}

		// Token: 0x0600044C RID: 1100 RVA: 0x00004CBD File Offset: 0x00002EBD
		private static float b(float v)
		{
			return 4f * (v - v * v);
		}

		// Token: 0x0600044D RID: 1101 RVA: 0x00004CCA File Offset: 0x00002ECA
		public float left()
		{
			return this._time.left();
		}

		// Token: 0x0600044E RID: 1102 RVA: 0x00004CD7 File Offset: 0x00002ED7
		public bool isDone()
		{
			return this._time.isDone();
		}

		// Token: 0x0600044F RID: 1103 RVA: 0x00004CE4 File Offset: 0x00002EE4
		public Vector2 getBias()
		{
			return Vector2.zero;
		}

		// Token: 0x040002D4 RID: 724
		public float x;

		// Token: 0x040002D5 RID: 725
		public float y;

		// Token: 0x040002D6 RID: 726
		public float angle;

		// Token: 0x040002D7 RID: 727
		private Vector2 bias;

		// Token: 0x040002D8 RID: 728
		private TimeInfo _time;
	}
}
