using System;
using UnityEngine;

// Token: 0x020003E6 RID: 998
public class VideoMode : ISettingsGroup
{
	// Token: 0x060015F0 RID: 5616 RVA: 0x00010066 File Offset: 0x0000E266
	public VideoMode(int width, int height, bool fullscreen)
	{
		this.width = new SvInt(width);
		this.height = new SvInt(height);
		this.fullscreen = new SvBool(fullscreen);
	}

	// Token: 0x060015F1 RID: 5617 RVA: 0x00085250 File Offset: 0x00083450
	public static bool isAllowed(VideoMode resolution)
	{
		foreach (Resolution resolution2 in Screen.resolutions)
		{
			if (resolution2.width == resolution.width && resolution2.height == resolution.height)
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060015F2 RID: 5618 RVA: 0x000852B8 File Offset: 0x000834B8
	public static VideoMode getDefault()
	{
		Resolution resolution = Screen.resolutions[Screen.resolutions.Length - 1];
		return new VideoMode(resolution.width, resolution.height, false);
	}

	// Token: 0x060015F3 RID: 5619 RVA: 0x000852F4 File Offset: 0x000834F4
	public static VideoMode getHighest()
	{
		int num = 0;
		int num2 = 0;
		Resolution? resolution = default(Resolution?);
		foreach (Resolution resolution2 in Screen.resolutions)
		{
			if (resolution2.width >= num && resolution2.height >= num2)
			{
				num = resolution2.width;
				num2 = resolution2.height;
				resolution = new Resolution?(resolution2);
			}
		}
		if (resolution != null)
		{
			return new VideoMode(resolution.Value.width, resolution.Value.height, false);
		}
		return VideoMode.getDefault();
	}

	// Token: 0x060015F4 RID: 5620 RVA: 0x00010092 File Offset: 0x0000E292
	public static VideoMode getCurrent()
	{
		return new VideoMode(Screen.width, Screen.height, Screen.fullScreen);
	}

	// Token: 0x0400131D RID: 4893
	public SvInt width;

	// Token: 0x0400131E RID: 4894
	public SvInt height;

	// Token: 0x0400131F RID: 4895
	public SvBool fullscreen;
}
