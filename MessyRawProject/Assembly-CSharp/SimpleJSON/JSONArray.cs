using System;
using System.Collections.Generic;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x02000479 RID: 1145
	public class JSONArray : JSONNode
	{
		// Token: 0x17000158 RID: 344
		// (get) Token: 0x060019AB RID: 6571 RVA: 0x0001298E File Offset: 0x00010B8E
		// (set) Token: 0x060019AC RID: 6572 RVA: 0x00012996 File Offset: 0x00010B96
		public override bool Inline
		{
			get
			{
				return this.inline;
			}
			set
			{
				this.inline = value;
			}
		}

		// Token: 0x17000159 RID: 345
		// (get) Token: 0x060019AD RID: 6573 RVA: 0x00004AAC File Offset: 0x00002CAC
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Array;
			}
		}

		// Token: 0x1700015A RID: 346
		// (get) Token: 0x060019AE RID: 6574 RVA: 0x00004AAC File Offset: 0x00002CAC
		public override bool IsArray
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060019AF RID: 6575 RVA: 0x0001299F File Offset: 0x00010B9F
		public override JSONNode.Enumerator GetEnumerator()
		{
			return new JSONNode.Enumerator(this.m_List.GetEnumerator());
		}

		// Token: 0x1700015B RID: 347
		public override JSONNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= this.m_List.Count)
				{
					return new JSONLazyCreator(this);
				}
				return this.m_List[aIndex];
			}
			set
			{
				if (value == null)
				{
					value = JSONNull.CreateOrGet();
				}
				if (aIndex < 0 || aIndex >= this.m_List.Count)
				{
					this.m_List.Add(value);
					return;
				}
				this.m_List[aIndex] = value;
			}
		}

		// Token: 0x1700015C RID: 348
		public override JSONNode this[string aKey]
		{
			get
			{
				return new JSONLazyCreator(this);
			}
			set
			{
				if (value == null)
				{
					value = JSONNull.CreateOrGet();
				}
				this.m_List.Add(value);
			}
		}

		// Token: 0x1700015D RID: 349
		// (get) Token: 0x060019B4 RID: 6580 RVA: 0x00012A3C File Offset: 0x00010C3C
		public override int Count
		{
			get
			{
				return this.m_List.Count;
			}
		}

		// Token: 0x060019B5 RID: 6581 RVA: 0x00012A1E File Offset: 0x00010C1E
		public override void Add(string aKey, JSONNode aItem)
		{
			if (aItem == null)
			{
				aItem = JSONNull.CreateOrGet();
			}
			this.m_List.Add(aItem);
		}

		// Token: 0x060019B6 RID: 6582 RVA: 0x00012A49 File Offset: 0x00010C49
		public override JSONNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= this.m_List.Count)
			{
				return null;
			}
			JSONNode result = this.m_List[aIndex];
			this.m_List.RemoveAt(aIndex);
			return result;
		}

		// Token: 0x060019B7 RID: 6583 RVA: 0x00012A77 File Offset: 0x00010C77
		public override JSONNode Remove(JSONNode aNode)
		{
			this.m_List.Remove(aNode);
			return aNode;
		}

		// Token: 0x1700015E RID: 350
		// (get) Token: 0x060019B8 RID: 6584 RVA: 0x00012A87 File Offset: 0x00010C87
		public override IEnumerable<JSONNode> Children
		{
			get
			{
				foreach (JSONNode jsonnode in this.m_List)
				{
					yield return jsonnode;
				}
				List<JSONNode>.Enumerator enumerator = default(List<JSONNode>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x060019B9 RID: 6585 RVA: 0x0009649C File Offset: 0x0009469C
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append('[');
			int count = this.m_List.Count;
			if (this.inline)
			{
				aMode = JSONTextMode.Compact;
			}
			for (int i = 0; i < count; i++)
			{
				if (i > 0)
				{
					aSB.Append(',');
				}
				if (aMode == JSONTextMode.Indent)
				{
					aSB.AppendLine();
				}
				if (aMode == JSONTextMode.Indent)
				{
					aSB.Append(' ', aIndent + aIndentInc);
				}
				this.m_List[i].WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
			}
			if (aMode == JSONTextMode.Indent)
			{
				aSB.AppendLine().Append(' ', aIndent);
			}
			aSB.Append(']');
		}

		// Token: 0x040015CB RID: 5579
		private List<JSONNode> m_List = new List<JSONNode>();

		// Token: 0x040015CC RID: 5580
		private bool inline;
	}
}
