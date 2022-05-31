using System;
using System.Collections;
using UnityEngine;

// Token: 0x02000065 RID: 101
internal class DigitGrower : MonoBehaviour
{
	// Token: 0x0600041A RID: 1050 RVA: 0x000028DF File Offset: 0x00000ADF
	private void Start()
	{
	}

	// Token: 0x0600041B RID: 1051 RVA: 0x00004B0A File Offset: 0x00002D0A
	public void reset()
	{
		base.StopCoroutine("grow");
		base.StartCoroutine("grow");
		base.StopCoroutine("flash");
		base.StartCoroutine("flash");
	}

	// Token: 0x0600041C RID: 1052 RVA: 0x00030614 File Offset: 0x0002E814
	private IEnumerator grow()
	{
		float start = Time.time;
		float T = 0.5f;
		while (Time.time - start < T)
		{
			float t = (Time.time - start) / T;
			if (t < 0.3f)
			{
				float tt = t;
				this.scale(1f + 0.15f * tt / 0.3f);
			}
			else if (t < 0.5f)
			{
				if (t > 0.4f)
				{
					float tt2 = t - 0.4f;
					this.scale(1.15f + 0.2f * tt2 / 0.1f);
				}
			}
			else if (t < 0.6f)
			{
				float tt3 = t - 0.5f;
				this.scale(1.4f - 0.2f * tt3 / 0.1f);
			}
			else if (t > 0.7f && t < 0.8f)
			{
				float tt4 = t - 0.7f;
				this.scale(1.2f - 0.2f * tt4 / 0.1f);
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	// Token: 0x0600041D RID: 1053 RVA: 0x00030630 File Offset: 0x0002E830
	private IEnumerator flash()
	{
		float start = Time.time;
		float T = 0.5f;
		while (Time.time - start < T)
		{
			float t = (Time.time - start) / T;
			if (t >= 0.45f)
			{
				if (t < 1f)
				{
					float tt = t - 0.45f;
					float v = 0.75f * (1f - tt / 0.55f);
					foreach (Renderer r in base.GetComponentsInChildren<Renderer>())
					{
						r.material.SetFloat("_Lerp", v);
					}
				}
			}
			yield return new WaitForEndOfFrame();
		}
		yield break;
	}

	// Token: 0x0600041E RID: 1054 RVA: 0x00004B3A File Offset: 0x00002D3A
	private void scale(float s)
	{
		base.gameObject.transform.localScale = Vector3.one * s;
	}
}
