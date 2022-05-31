using System;

// Token: 0x02000446 RID: 1094
public class IntCoord
{
	// Token: 0x06001852 RID: 6226 RVA: 0x0001188E File Offset: 0x0000FA8E
	public IntCoord(int x, int y)
	{
		this.x = x;
		this.y = y;
	}

	// Token: 0x06001853 RID: 6227 RVA: 0x000118A4 File Offset: 0x0000FAA4
	private bool Equals(IntCoord rhs)
	{
		return this.x == rhs.x && this.y == rhs.y;
	}

	// Token: 0x06001854 RID: 6228 RVA: 0x00092934 File Offset: 0x00090B34
	public override string ToString()
	{
		return string.Concat(new object[]
		{
			"C(",
			this.x,
			",",
			this.y,
			")"
		});
	}

	// Token: 0x06001855 RID: 6229 RVA: 0x000118C8 File Offset: 0x0000FAC8
	public static IntCoord operator +(IntCoord a, IntCoord b)
	{
		return new IntCoord(a.x + b.x, a.y + b.y);
	}

	// Token: 0x06001856 RID: 6230 RVA: 0x000118E9 File Offset: 0x0000FAE9
	public static IntCoord operator -(IntCoord a, IntCoord b)
	{
		return new IntCoord(a.x - b.x, a.y - b.y);
	}

	// Token: 0x0400152B RID: 5419
	public int x;

	// Token: 0x0400152C RID: 5420
	public int y;
}
