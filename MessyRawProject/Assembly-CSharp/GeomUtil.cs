using System;
using UnityEngine;

// Token: 0x0200043E RID: 1086
public static class GeomUtil
{
	// Token: 0x0600181C RID: 6172 RVA: 0x00011567 File Offset: 0x0000F767
	public static Rect scaleCentered(Rect r, float kx)
	{
		return GeomUtil.scaleCentered(r, kx, kx);
	}

	// Token: 0x0600181D RID: 6173 RVA: 0x00011571 File Offset: 0x0000F771
	public static Rect scaleCentered(Rect r, float kx, float ky)
	{
		return GeomUtil.scaleAround(r, new Vector2(r.x + 0.5f * r.width, r.y + 0.5f * r.height), kx, ky);
	}

	// Token: 0x0600181E RID: 6174 RVA: 0x000115AA File Offset: 0x0000F7AA
	public static Rect scaleAround(Rect r, Vector2 c, float kx)
	{
		return GeomUtil.scaleAround(r, c, kx, kx);
	}

	// Token: 0x0600181F RID: 6175 RVA: 0x000115B5 File Offset: 0x0000F7B5
	public static Rect scaleAround(Rect r, Vector2 c, float kx, float ky)
	{
		return GeomUtil.resize(r, c, kx * r.width, ky * r.height);
	}

	// Token: 0x06001820 RID: 6176 RVA: 0x000115D0 File Offset: 0x0000F7D0
	public static Rect resizeCentered(Rect r, float w)
	{
		return GeomUtil.resize(r, new Vector2(r.x + r.width / 2f, r.y + r.height / 2f), w, r.height);
	}

	// Token: 0x06001821 RID: 6177 RVA: 0x0001160F File Offset: 0x0000F80F
	public static Rect resizeCentered(Rect r, float w, float h)
	{
		return GeomUtil.resize(r, new Vector2(r.x + r.width / 2f, r.y + r.height / 2f), w, h);
	}

	// Token: 0x06001822 RID: 6178 RVA: 0x00092050 File Offset: 0x00090250
	public static Rect resize(Rect r, Vector2 c, float w, float h)
	{
		float num = c.x - (c.x - r.x) * (w / r.width);
		float num2 = c.y - (c.y - r.y) * (h / r.height);
		return new Rect(num, num2, w, h);
	}

	// Token: 0x06001823 RID: 6179 RVA: 0x000920AC File Offset: 0x000902AC
	public static Rect centerAt(Rect r, Vector2 c)
	{
		Vector2 center = r.center;
		Vector2 vector = c - center;
		return new Rect(r.x + vector.x, r.y + vector.y, r.width, r.height);
	}

	// Token: 0x06001824 RID: 6180 RVA: 0x00011648 File Offset: 0x0000F848
	public static Rect inflate(Rect r, float margin)
	{
		return GeomUtil.inflate(r, margin, margin);
	}

	// Token: 0x06001825 RID: 6181 RVA: 0x00011652 File Offset: 0x0000F852
	public static Rect inflate(Rect r, float dx, float dy)
	{
		return new Rect(r.x - dx, r.y - dy, r.width + dx + dx, r.height + dy + dy);
	}

	// Token: 0x06001826 RID: 6182 RVA: 0x00011681 File Offset: 0x0000F881
	public static Rect floor(Rect r)
	{
		return new Rect(Mth.floor(r.x), Mth.floor(r.y), r.width, r.height);
	}

	// Token: 0x06001827 RID: 6183 RVA: 0x000116AE File Offset: 0x0000F8AE
	public static Rect getCentered(Rect rect, bool centerX, bool centerY)
	{
		return GeomUtil.getCentered(rect, new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), centerX, centerY);
	}

	// Token: 0x06001828 RID: 6184 RVA: 0x000920FC File Offset: 0x000902FC
	public static Rect getCentered(Rect toCenter, Rect area, bool centerX, bool centerY)
	{
		float num = (!centerX) ? toCenter.x : (area.x + (area.width - toCenter.width) / 2f);
		float num2 = (!centerY) ? toCenter.y : (area.y + (area.height - toCenter.height) / 2f);
		return new Rect(num, num2, toCenter.width, toCenter.height);
	}

	// Token: 0x06001829 RID: 6185 RVA: 0x00092180 File Offset: 0x00090380
	public static Rect getCropped(Rect toCrop, Rect bounds)
	{
		Rect result;
		result..ctor(toCrop);
		result.xMin = Mth.clamp(result.xMin, bounds.xMin, bounds.xMax);
		result.yMin = Mth.clamp(result.yMin, bounds.yMin, bounds.yMax);
		result.xMax = Mth.clamp(result.xMax, bounds.xMin, bounds.xMax);
		result.yMax = Mth.clamp(result.yMax, bounds.yMin, bounds.yMax);
		return result;
	}

	// Token: 0x0600182A RID: 6186 RVA: 0x0009221C File Offset: 0x0009041C
	public static Rect cropShare(Rect full, Rect share)
	{
		float width = full.width;
		float height = full.height;
		return new Rect(full.x + width * share.x, full.y + height * share.y, width * share.width, height * share.height);
	}

	// Token: 0x0600182B RID: 6187 RVA: 0x00092274 File Offset: 0x00090474
	public static Vector2 clamp(Rect bounds, Vector2 v)
	{
		Rect cropped = GeomUtil.getCropped(new Rect(v.x, v.y, 0f, 0f), bounds);
		return new Vector2(cropped.x, cropped.y);
	}

	// Token: 0x0600182C RID: 6188 RVA: 0x000922B8 File Offset: 0x000904B8
	public static Vector3 clamp(Rect bounds, Vector3 v)
	{
		Rect cropped = GeomUtil.getCropped(new Rect(v.x, v.y, 0f, 0f), bounds);
		return new Vector3(cropped.x, cropped.y, v.z);
	}

	// Token: 0x0600182D RID: 6189 RVA: 0x000116D3 File Offset: 0x0000F8D3
	public static Rect getTranslated(Rect r, float x, float y)
	{
		return new Rect(r.x + x, r.y + y, r.width, r.height);
	}

	// Token: 0x0600182E RID: 6190 RVA: 0x00092304 File Offset: 0x00090504
	public static bool overlaps(Rect a, Rect b)
	{
		return a.xMax > b.x && a.yMax > b.y && a.x < b.xMax && a.y < b.yMax;
	}

	// Token: 0x0600182F RID: 6191 RVA: 0x00092364 File Offset: 0x00090564
	public static bool overlapsOrTouches(Rect a, Rect b)
	{
		return a.xMax >= b.x && a.yMax >= b.y && a.x <= b.xMax && a.y <= b.yMax;
	}

	// Token: 0x06001830 RID: 6192 RVA: 0x000923C0 File Offset: 0x000905C0
	public static Vector3 PixelAlign(Vector3 p)
	{
		Vector3 vector = Camera.main.WorldToScreenPoint(p);
		Vector3 vector2 = Camera.main.ScreenToWorldPoint(new Vector3((float)Math.Round((double)vector.x), (float)Math.Round((double)vector.y), vector.z));
		return new Vector3(vector2.x, vector2.y, p.z);
	}

	// Token: 0x06001831 RID: 6193 RVA: 0x000116FA File Offset: 0x0000F8FA
	public static Vector3 getTranslated(Vector3 v, float x, float y, float z)
	{
		return new Vector3(v.x + x, v.y + y, v.z + z);
	}

	// Token: 0x06001832 RID: 6194 RVA: 0x00092428 File Offset: 0x00090628
	public static Rect getCoveringRect(params Rect[] rects)
	{
		if (rects.Length == 0)
		{
			throw new ArgumentException("Can't cover 0 rects");
		}
		Rect rect = rects[0];
		float x = rect.x;
		float y = rect.y;
		float xMax = rect.xMax;
		float yMax = rect.yMax;
		for (int i = 1; i < rects.Length; i++)
		{
			rect = rects[i];
			if (rect.x < x)
			{
				x = rect.x;
			}
			if (rect.y < y)
			{
				y = rect.y;
			}
			if (rect.xMax > xMax)
			{
				xMax = rect.xMax;
			}
			if (rect.yMax > yMax)
			{
				yMax = rect.yMax;
			}
		}
		return new Rect(x, y, xMax - x, yMax - y);
	}

	// Token: 0x06001833 RID: 6195 RVA: 0x0001171C File Offset: 0x0000F91C
	public static float getWidthFromHeight(float height, float aspect)
	{
		return height * aspect;
	}

	// Token: 0x06001834 RID: 6196 RVA: 0x00011721 File Offset: 0x0000F921
	public static float getWidthFromHeight(float height, float srcWidth, float srcHeight)
	{
		return GeomUtil.getWidthFromHeight(height, srcWidth / srcHeight);
	}

	// Token: 0x06001835 RID: 6197 RVA: 0x00092500 File Offset: 0x00090700
	public static Vector2 getRandomPoint(Rect rect)
	{
		float num = Random.Range(0f, rect.width);
		float num2 = Random.Range(0f, rect.height);
		return new Vector2(rect.x + num, rect.y + num2);
	}

	// Token: 0x06001836 RID: 6198 RVA: 0x0001172C File Offset: 0x0000F92C
	public static Vector2 v3tov2(Vector3 v)
	{
		return new Vector2(v.x, v.y);
	}

	// Token: 0x06001837 RID: 6199 RVA: 0x00011741 File Offset: 0x0000F941
	public static Vector3 v2tov3(Vector2 v)
	{
		return new Vector3(v.x, v.y, 0f);
	}

	// Token: 0x06001838 RID: 6200 RVA: 0x0001175B File Offset: 0x0000F95B
	public static Vector3 v2tov3(Vector2 v, float z)
	{
		return new Vector3(v.x, v.y, z);
	}
}
