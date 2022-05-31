using System;

// Token: 0x020001BF RID: 447
public static class EnchantmentTypeExtension
{
	// Token: 0x06000E01 RID: 3585 RVA: 0x0000B194 File Offset: 0x00009394
	public static string getName(this EnchantmentType ins)
	{
		if (ins == EnchantmentType.BUFF)
		{
			return "Effect";
		}
		if (ins == EnchantmentType.STARTBUFF)
		{
			return string.Empty;
		}
		return StringUtil.capitalize(ins.ToString());
	}
}
