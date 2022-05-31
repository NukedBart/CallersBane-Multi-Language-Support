using System;
using UnityEngine;

// Token: 0x0200003E RID: 62
public class WorldImage
{
	// Token: 0x0600028E RID: 654 RVA: 0x00003DF2 File Offset: 0x00001FF2
	public WorldImage(string filename, Vector3 position, Vector3 scale, Vector3 rotation)
	{
		this.filename = filename;
		this.pos = position;
		this.scale = scale;
		this.rot = rotation;
	}

	// Token: 0x0600028F RID: 655 RVA: 0x00003E17 File Offset: 0x00002017
	public WorldImage(string filename, Vector3 position, Vector3 scale) : this(filename, position, scale, new Vector3(39f, 90f, 0f))
	{
	}

	// Token: 0x0400016B RID: 363
	public string filename;

	// Token: 0x0400016C RID: 364
	public Vector3 pos;

	// Token: 0x0400016D RID: 365
	public Vector3 scale;

	// Token: 0x0400016E RID: 366
	public Vector3 rot;
}
