using System;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x02000481 RID: 1153
	public class JSONNull : JSONNode
	{
		// Token: 0x06001A03 RID: 6659 RVA: 0x00012D5D File Offset: 0x00010F5D
		public static JSONNull CreateOrGet()
		{
			if (JSONNull.reuseSameInstance)
			{
				return JSONNull.m_StaticInstance;
			}
			return new JSONNull();
		}

		// Token: 0x06001A04 RID: 6660 RVA: 0x00012D71 File Offset: 0x00010F71
		private JSONNull()
		{
		}

		// Token: 0x17000175 RID: 373
		// (get) Token: 0x06001A05 RID: 6661 RVA: 0x00012D79 File Offset: 0x00010F79
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.NullValue;
			}
		}

		// Token: 0x17000176 RID: 374
		// (get) Token: 0x06001A06 RID: 6662 RVA: 0x00004AAC File Offset: 0x00002CAC
		public override bool IsNull
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06001A07 RID: 6663 RVA: 0x00096A40 File Offset: 0x00094C40
		public override JSONNode.Enumerator GetEnumerator()
		{
			return default(JSONNode.Enumerator);
		}

		// Token: 0x17000177 RID: 375
		// (get) Token: 0x06001A08 RID: 6664 RVA: 0x00012D7C File Offset: 0x00010F7C
		// (set) Token: 0x06001A09 RID: 6665 RVA: 0x000028DF File Offset: 0x00000ADF
		public override string Value
		{
			get
			{
				return "null";
			}
			set
			{
			}
		}

		// Token: 0x17000178 RID: 376
		// (get) Token: 0x06001A0A RID: 6666 RVA: 0x000059E4 File Offset: 0x00003BE4
		// (set) Token: 0x06001A0B RID: 6667 RVA: 0x000028DF File Offset: 0x00000ADF
		public override bool AsBool
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x06001A0C RID: 6668 RVA: 0x00012D83 File Offset: 0x00010F83
		public override bool Equals(object obj)
		{
			return this == obj || obj is JSONNull;
		}

		// Token: 0x06001A0D RID: 6669 RVA: 0x000059E4 File Offset: 0x00003BE4
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x06001A0E RID: 6670 RVA: 0x00012D94 File Offset: 0x00010F94
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append("null");
		}

		// Token: 0x040015DD RID: 5597
		private static JSONNull m_StaticInstance = new JSONNull();

		// Token: 0x040015DE RID: 5598
		public static bool reuseSameInstance = true;
	}
}
