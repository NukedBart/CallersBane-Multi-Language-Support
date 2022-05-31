using System;

// Token: 0x0200040D RID: 1037
[Serializable]
public class PerCharacterKerning
{
	// Token: 0x060016FA RID: 5882 RVA: 0x00010780 File Offset: 0x0000E980
	public PerCharacterKerning(string character, float kerning)
	{
		this.First = character;
		this.Second = kerning;
	}

	// Token: 0x060016FB RID: 5883 RVA: 0x000107A1 File Offset: 0x0000E9A1
	public PerCharacterKerning(char character, float kerning)
	{
		this.First = string.Empty + character;
		this.Second = kerning;
	}

	// Token: 0x060016FC RID: 5884 RVA: 0x000107D1 File Offset: 0x0000E9D1
	public char GetChar()
	{
		return this.First.get_Chars(0);
	}

	// Token: 0x060016FD RID: 5885 RVA: 0x000107DF File Offset: 0x0000E9DF
	public float GetKerningValue()
	{
		return this.Second;
	}

	// Token: 0x04001476 RID: 5238
	public string First = string.Empty;

	// Token: 0x04001477 RID: 5239
	public float Second;
}
