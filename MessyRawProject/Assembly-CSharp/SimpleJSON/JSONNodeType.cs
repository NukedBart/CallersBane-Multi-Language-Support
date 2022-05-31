using System;

namespace SimpleJSON
{
	// Token: 0x0200046F RID: 1135
	public enum JSONNodeType
	{
		// Token: 0x040015AA RID: 5546
		Array = 1,
		// Token: 0x040015AB RID: 5547
		Object,
		// Token: 0x040015AC RID: 5548
		String,
		// Token: 0x040015AD RID: 5549
		Number,
		// Token: 0x040015AE RID: 5550
		NullValue,
		// Token: 0x040015AF RID: 5551
		Boolean,
		// Token: 0x040015B0 RID: 5552
		None,
		// Token: 0x040015B1 RID: 5553
		Custom = 255
	}
}
