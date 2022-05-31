using System;
using UnityEngine;

// Token: 0x02000094 RID: 148
public class Projectile : MonoBehaviour
{
	// Token: 0x06000565 RID: 1381 RVA: 0x000028DF File Offset: 0x00000ADF
	private void Start()
	{
	}

	// Token: 0x06000566 RID: 1382 RVA: 0x00039354 File Offset: 0x00037554
	public void init(Vector3 attackTo, Unit callBackTarget, bool isSiege, bool isFastProjectile)
	{
		this.isSiege = isSiege;
		this.callBackTarget = callBackTarget;
		this.attackTo = attackTo;
		this.startPoint = new Vector3(base.transform.position.x, base.transform.position.y, base.transform.position.z);
		this.endPoint = new Vector3(attackTo.x, attackTo.y + 0.2f, attackTo.z);
		if (isSiege)
		{
			this.siegeStartTime = App.Clocks.tweenClock.getTime();
			this.isSiegeProjectileAlive = true;
			base.transform.position = this.startPoint;
		}
		else
		{
			float num = Math.Abs(this.startPoint.z - this.endPoint.z);
			if (isFastProjectile)
			{
				num = 0.05f + num / 4f;
			}
			iTween.MoveTo(base.gameObject, iTween.Hash(new object[]
			{
				"z",
				this.endPoint.z,
				"time",
				num * 0.08f,
				"easetype",
				"linear",
				"oncompletetarget",
				base.gameObject,
				"oncomplete",
				"tweenComplete"
			}));
		}
	}

	// Token: 0x06000567 RID: 1383 RVA: 0x000394C8 File Offset: 0x000376C8
	private void Update()
	{
		if (!this.isSiegeProjectileAlive)
		{
			return;
		}
		float num = (App.Clocks.tweenClock.getTime() - this.siegeStartTime) / this.siegeAliveTime;
		float num2 = Mathf.Clamp01(num);
		float num3 = -1f;
		float num4 = -this.startPoint.y + (0f - 10f * num2 - 10f * num3 * (num2 * num2));
		base.transform.position = new Vector3(Mathf.Lerp(this.startPoint.x, this.endPoint.x, num2), -num4, Mathf.Lerp(this.startPoint.z, this.endPoint.z, num2));
		if (num >= 0.9999f)
		{
			this.isSiegeProjectileAlive = false;
			this.tweenComplete();
		}
	}

	// Token: 0x06000568 RID: 1384 RVA: 0x000056B6 File Offset: 0x000038B6
	private void tweenComplete()
	{
		this.callBackTarget.projectileDone(this, this.attackTo);
	}

	// Token: 0x040003D7 RID: 983
	private Unit callBackTarget;

	// Token: 0x040003D8 RID: 984
	private Vector3 attackTo;

	// Token: 0x040003D9 RID: 985
	private Vector3 startPoint;

	// Token: 0x040003DA RID: 986
	private Vector3 endPoint;

	// Token: 0x040003DB RID: 987
	private bool isSiege;

	// Token: 0x040003DC RID: 988
	private bool isSiegeProjectileAlive;

	// Token: 0x040003DD RID: 989
	private float siegeStartTime;

	// Token: 0x040003DE RID: 990
	private float siegeAliveTime = 1.2f;
}
