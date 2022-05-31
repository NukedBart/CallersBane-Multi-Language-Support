using System;
using UnityEngine;

// Token: 0x02000257 RID: 599
public static class ResourceColor
{
	// Token: 0x060011D5 RID: 4565 RVA: 0x0000D8F4 File Offset: 0x0000BAF4
	public static Color getGui(ResourceType t)
	{
		return ColorUtil.Maximize(ColorUtil.Darken(ResourceColor.get(t), 0.4f));
	}

	// Token: 0x060011D6 RID: 4566 RVA: 0x00077784 File Offset: 0x00075984
	public static Color get(ResourceType t)
	{
		if (t == ResourceType.GROWTH)
		{
			return ResourceColor.Growth;
		}
		if (t == ResourceType.ORDER)
		{
			return ResourceColor.Order;
		}
		if (t == ResourceType.ENERGY)
		{
			return ResourceColor.Energy;
		}
		if (t == ResourceType.DECAY)
		{
			return ResourceColor.Decay;
		}
		if (t == ResourceType.SPECIAL)
		{
			return ResourceColor.Wild;
		}
		return Color.white;
	}

	// Token: 0x04000E71 RID: 3697
	public static Color Growth = ColorUtil.FromInts(214, 244, 162);

	// Token: 0x04000E72 RID: 3698
	public static Color Order = ColorUtil.FromInts(213, 236, 255);

	// Token: 0x04000E73 RID: 3699
	public static Color Energy = ColorUtil.FromInts(255, 214, 101);

	// Token: 0x04000E74 RID: 3700
	public static Color Decay = ColorUtil.FromInts(226, 193, 248);

	// Token: 0x04000E75 RID: 3701
	public static Color Wild = ColorUtil.FromInts(255, 255, 255);
}
