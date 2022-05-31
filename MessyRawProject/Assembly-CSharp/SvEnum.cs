using System;

// Token: 0x020003E1 RID: 993
public class SvEnum<T> : ISettingsValue where T : struct, IConvertible
{
	// Token: 0x060015CF RID: 5583 RVA: 0x0000FE46 File Offset: 0x0000E046
	public SvEnum(T v)
	{
		if (!typeof(T).IsEnum)
		{
			throw new InvalidCastException(typeof(T) + " must be enum");
		}
		this.value = v;
	}

	// Token: 0x060015D0 RID: 5584 RVA: 0x00085038 File Offset: 0x00083238
	public void load(string s)
	{
		try
		{
			this.value = (T)((object)Enum.Parse(typeof(T), s, true));
		}
		catch (ArgumentException)
		{
		}
	}

	// Token: 0x060015D1 RID: 5585 RVA: 0x0000FE83 File Offset: 0x0000E083
	public override string ToString()
	{
		return this.value.ToString().ToLower();
	}

	// Token: 0x060015D2 RID: 5586 RVA: 0x0000FE9B File Offset: 0x0000E09B
	public static implicit operator T(SvEnum<T> e)
	{
		return e.value;
	}

	// Token: 0x04001314 RID: 4884
	public T value;
}
