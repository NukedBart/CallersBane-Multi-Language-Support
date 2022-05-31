using System;

// Token: 0x020002DD RID: 733
public class Request : IComparable
{
	// Token: 0x060012D7 RID: 4823 RVA: 0x0000E1E1 File Offset: 0x0000C3E1
	public int CompareTo(object other)
	{
		if (other is Request)
		{
			return this.from.CompareTo(((Request)other).from);
		}
		return 0;
	}

	// Token: 0x04000FA7 RID: 4007
	public Person from;

	// Token: 0x04000FA8 RID: 4008
	public Person to;

	// Token: 0x04000FA9 RID: 4009
	public FriendRequest request;
}
