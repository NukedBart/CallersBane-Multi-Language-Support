using System;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x02000480 RID: 1152
	public class JSONBool : JSONNode
	{
		// Token: 0x17000171 RID: 369
		// (get) Token: 0x060019F7 RID: 6647 RVA: 0x00012CE4 File Offset: 0x00010EE4
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Boolean;
			}
		}

		// Token: 0x17000172 RID: 370
		// (get) Token: 0x060019F8 RID: 6648 RVA: 0x00004AAC File Offset: 0x00002CAC
		public override bool IsBoolean
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060019F9 RID: 6649 RVA: 0x00096A40 File Offset: 0x00094C40
		public override JSONNode.Enumerator GetEnumerator()
		{
			return default(JSONNode.Enumerator);
		}

		// Token: 0x17000173 RID: 371
		// (get) Token: 0x060019FA RID: 6650 RVA: 0x00012CE7 File Offset: 0x00010EE7
		// (set) Token: 0x060019FB RID: 6651 RVA: 0x00096B88 File Offset: 0x00094D88
		public override string Value
		{
			get
			{
				return this.m_Data.ToString();
			}
			set
			{
				bool data;
				if (bool.TryParse(value, ref data))
				{
					this.m_Data = data;
				}
			}
		}

		// Token: 0x17000174 RID: 372
		// (get) Token: 0x060019FC RID: 6652 RVA: 0x00012CF4 File Offset: 0x00010EF4
		// (set) Token: 0x060019FD RID: 6653 RVA: 0x00012CFC File Offset: 0x00010EFC
		public override bool AsBool
		{
			get
			{
				return this.m_Data;
			}
			set
			{
				this.m_Data = value;
			}
		}

		// Token: 0x060019FE RID: 6654 RVA: 0x00012D05 File Offset: 0x00010F05
		public JSONBool(bool aData)
		{
			this.m_Data = aData;
		}

		// Token: 0x060019FF RID: 6655 RVA: 0x00012CB9 File Offset: 0x00010EB9
		public JSONBool(string aData)
		{
			this.Value = aData;
		}

		// Token: 0x06001A00 RID: 6656 RVA: 0x00012D14 File Offset: 0x00010F14
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append(this.m_Data ? "true" : "false");
		}

		// Token: 0x06001A01 RID: 6657 RVA: 0x00012D31 File Offset: 0x00010F31
		public override bool Equals(object obj)
		{
			return obj != null && obj is bool && this.m_Data == (bool)obj;
		}

		// Token: 0x06001A02 RID: 6658 RVA: 0x00012D50 File Offset: 0x00010F50
		public override int GetHashCode()
		{
			return this.m_Data.GetHashCode();
		}

		// Token: 0x040015DC RID: 5596
		private bool m_Data;
	}
}
