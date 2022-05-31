using System;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x0200047F RID: 1151
	public class JSONNumber : JSONNode
	{
		// Token: 0x1700016D RID: 365
		// (get) Token: 0x060019EA RID: 6634 RVA: 0x00012C89 File Offset: 0x00010E89
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Number;
			}
		}

		// Token: 0x1700016E RID: 366
		// (get) Token: 0x060019EB RID: 6635 RVA: 0x00004AAC File Offset: 0x00002CAC
		public override bool IsNumber
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060019EC RID: 6636 RVA: 0x00096A40 File Offset: 0x00094C40
		public override JSONNode.Enumerator GetEnumerator()
		{
			return default(JSONNode.Enumerator);
		}

		// Token: 0x1700016F RID: 367
		// (get) Token: 0x060019ED RID: 6637 RVA: 0x00012C8C File Offset: 0x00010E8C
		// (set) Token: 0x060019EE RID: 6638 RVA: 0x00096AAC File Offset: 0x00094CAC
		public override string Value
		{
			get
			{
				return this.m_Data.ToString();
			}
			set
			{
				double data;
				if (double.TryParse(value, ref data))
				{
					this.m_Data = data;
				}
			}
		}

		// Token: 0x17000170 RID: 368
		// (get) Token: 0x060019EF RID: 6639 RVA: 0x00012C99 File Offset: 0x00010E99
		// (set) Token: 0x060019F0 RID: 6640 RVA: 0x00012CA1 File Offset: 0x00010EA1
		public override double AsDouble
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

		// Token: 0x060019F1 RID: 6641 RVA: 0x00012CAA File Offset: 0x00010EAA
		public JSONNumber(double aData)
		{
			this.m_Data = aData;
		}

		// Token: 0x060019F2 RID: 6642 RVA: 0x00012CB9 File Offset: 0x00010EB9
		public JSONNumber(string aData)
		{
			this.Value = aData;
		}

		// Token: 0x060019F3 RID: 6643 RVA: 0x00012CC8 File Offset: 0x00010EC8
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append(this.m_Data);
		}

		// Token: 0x060019F4 RID: 6644 RVA: 0x00096ACC File Offset: 0x00094CCC
		private static bool IsNumeric(object value)
		{
			return value is int || value is uint || value is float || value is double || value is decimal || value is long || value is ulong || value is short || value is ushort || value is sbyte || value is byte;
		}

		// Token: 0x060019F5 RID: 6645 RVA: 0x00096B34 File Offset: 0x00094D34
		public override bool Equals(object obj)
		{
			if (obj == null)
			{
				return false;
			}
			if (base.Equals(obj))
			{
				return true;
			}
			JSONNumber jsonnumber = obj as JSONNumber;
			if (jsonnumber != null)
			{
				return this.m_Data == jsonnumber.m_Data;
			}
			return JSONNumber.IsNumeric(obj) && Convert.ToDouble(obj) == this.m_Data;
		}

		// Token: 0x060019F6 RID: 6646 RVA: 0x00012CD7 File Offset: 0x00010ED7
		public override int GetHashCode()
		{
			return this.m_Data.GetHashCode();
		}

		// Token: 0x040015DB RID: 5595
		private double m_Data;
	}
}
