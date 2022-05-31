using System;
using UnityEngine;

// Token: 0x020003E8 RID: 1000
public class Shaders
{
	// Token: 0x060015F8 RID: 5624 RVA: 0x000100B4 File Offset: 0x0000E2B4
	public static Material matMilkBurn()
	{
		return new Material(ResourceManager.LoadShader(Shaders.fnMilkBurn));
	}

	// Token: 0x04001321 RID: 4897
	public static readonly string fnMilkBurn = "Scrolls/Transparent/Diffuse/Double/ColorCV";
}
