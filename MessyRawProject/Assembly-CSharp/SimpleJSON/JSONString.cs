using System;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x0200047E RID: 1150
	public class JSONString : JSONNode
	{
		// Token: 0x1700016A RID: 362
		// (get) Token: 0x060019E1 RID: 6625 RVA: 0x0000DDC8 File Offset: 0x0000BFC8
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.String;
			}
		}

		// Token: 0x1700016B RID: 363
		// (get) Token: 0x060019E2 RID: 6626 RVA: 0x00004AAC File Offset: 0x00002CAC
		public override bool IsString
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060019E3 RID: 6627 RVA: 0x00096A40 File Offset: 0x00094C40
		public override JSONNode.Enumerator GetEnumerator()
		{
			return default(JSONNode.Enumerator);
		}

		// Token: 0x1700016C RID: 364
		// (get) Token: 0x060019E4 RID: 6628 RVA: 0x00012C3A File Offset: 0x00010E3A
		// (set) Token: 0x060019E5 RID: 6629 RVA: 0x00012C42 File Offset: 0x00010E42
		public override string Value
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

		// Token: 0x060019E6 RID: 6630 RVA: 0x00012C4B File Offset: 0x00010E4B
		public JSONString(string aData)
		{
			this.m_Data = aData;
		}

		// Token: 0x060019E7 RID: 6631 RVA: 0x00012C5A File Offset: 0x00010E5A
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append('"').Append(JSONNode.Escape(this.m_Data)).Append('"');
		}

		// Token: 0x060019E8 RID: 6632 RVA: 0x00096A58 File Offset: 0x00094C58
		public override bool Equals(object obj)
		{
			if (base.Equals(obj))
			{
				return true;
			}
			string text = obj as string;
			if (text != null)
			{
				return this.m_Data == text;
			}
			JSONString jsonstring = obj as JSONString;
			return jsonstring != null && this.m_Data == jsonstring.m_Data;
		}

		// Token: 0x060019E9 RID: 6633 RVA: 0x00012C7C File Offset: 0x00010E7C
		public override int GetHashCode()
		{
			return this.m_Data.GetHashCode();
		}

		// Token: 0x040015DA RID: 5594
		private string m_Data;
	}
}
