using System;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x02000482 RID: 1154
	internal class JSONLazyCreator : JSONNode
	{
		// Token: 0x17000179 RID: 377
		// (get) Token: 0x06001A10 RID: 6672 RVA: 0x00012DB4 File Offset: 0x00010FB4
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.None;
			}
		}

		// Token: 0x06001A11 RID: 6673 RVA: 0x00096A40 File Offset: 0x00094C40
		public override JSONNode.Enumerator GetEnumerator()
		{
			return default(JSONNode.Enumerator);
		}

		// Token: 0x06001A12 RID: 6674 RVA: 0x00012DB7 File Offset: 0x00010FB7
		public JSONLazyCreator(JSONNode aNode)
		{
			this.m_Node = aNode;
			this.m_Key = null;
		}

		// Token: 0x06001A13 RID: 6675 RVA: 0x00012DCD File Offset: 0x00010FCD
		public JSONLazyCreator(JSONNode aNode, string aKey)
		{
			this.m_Node = aNode;
			this.m_Key = aKey;
		}

		// Token: 0x06001A14 RID: 6676 RVA: 0x00012DE3 File Offset: 0x00010FE3
		private void Set(JSONNode aVal)
		{
			if (this.m_Key == null)
			{
				this.m_Node.Add(aVal);
			}
			else
			{
				this.m_Node.Add(this.m_Key, aVal);
			}
			this.m_Node = null;
		}

		// Token: 0x1700017A RID: 378
		public override JSONNode this[int aIndex]
		{
			get
			{
				return new JSONLazyCreator(this);
			}
			set
			{
				JSONArray jsonarray = new JSONArray();
				jsonarray.Add(value);
				this.Set(jsonarray);
			}
		}

		// Token: 0x1700017B RID: 379
		public override JSONNode this[string aKey]
		{
			get
			{
				return new JSONLazyCreator(this, aKey);
			}
			set
			{
				JSONObject jsonobject = new JSONObject();
				jsonobject.Add(aKey, value);
				this.Set(jsonobject);
			}
		}

		// Token: 0x06001A19 RID: 6681 RVA: 0x00096BF0 File Offset: 0x00094DF0
		public override void Add(JSONNode aItem)
		{
			JSONArray jsonarray = new JSONArray();
			jsonarray.Add(aItem);
			this.Set(jsonarray);
		}

		// Token: 0x06001A1A RID: 6682 RVA: 0x00096BCC File Offset: 0x00094DCC
		public override void Add(string aKey, JSONNode aItem)
		{
			JSONObject jsonobject = new JSONObject();
			jsonobject.Add(aKey, aItem);
			this.Set(jsonobject);
		}

		// Token: 0x06001A1B RID: 6683 RVA: 0x00012E1D File Offset: 0x0001101D
		public static bool operator ==(JSONLazyCreator a, object b)
		{
			return b == null || a == b;
		}

		// Token: 0x06001A1C RID: 6684 RVA: 0x00012E28 File Offset: 0x00011028
		public static bool operator !=(JSONLazyCreator a, object b)
		{
			return !(a == b);
		}

		// Token: 0x06001A1D RID: 6685 RVA: 0x00012E1D File Offset: 0x0001101D
		public override bool Equals(object obj)
		{
			return obj == null || this == obj;
		}

		// Token: 0x06001A1E RID: 6686 RVA: 0x000059E4 File Offset: 0x00003BE4
		public override int GetHashCode()
		{
			return 0;
		}

		// Token: 0x1700017C RID: 380
		// (get) Token: 0x06001A1F RID: 6687 RVA: 0x00096C14 File Offset: 0x00094E14
		// (set) Token: 0x06001A20 RID: 6688 RVA: 0x00096C38 File Offset: 0x00094E38
		public override int AsInt
		{
			get
			{
				JSONNumber aVal = new JSONNumber(0.0);
				this.Set(aVal);
				return 0;
			}
			set
			{
				JSONNumber aVal = new JSONNumber((double)value);
				this.Set(aVal);
			}
		}

		// Token: 0x1700017D RID: 381
		// (get) Token: 0x06001A21 RID: 6689 RVA: 0x00096C54 File Offset: 0x00094E54
		// (set) Token: 0x06001A22 RID: 6690 RVA: 0x00096C38 File Offset: 0x00094E38
		public override float AsFloat
		{
			get
			{
				JSONNumber aVal = new JSONNumber(0.0);
				this.Set(aVal);
				return 0f;
			}
			set
			{
				JSONNumber aVal = new JSONNumber((double)value);
				this.Set(aVal);
			}
		}

		// Token: 0x1700017E RID: 382
		// (get) Token: 0x06001A23 RID: 6691 RVA: 0x00096C7C File Offset: 0x00094E7C
		// (set) Token: 0x06001A24 RID: 6692 RVA: 0x00096CA8 File Offset: 0x00094EA8
		public override double AsDouble
		{
			get
			{
				JSONNumber aVal = new JSONNumber(0.0);
				this.Set(aVal);
				return 0.0;
			}
			set
			{
				JSONNumber aVal = new JSONNumber(value);
				this.Set(aVal);
			}
		}

		// Token: 0x1700017F RID: 383
		// (get) Token: 0x06001A25 RID: 6693 RVA: 0x00096CC4 File Offset: 0x00094EC4
		// (set) Token: 0x06001A26 RID: 6694 RVA: 0x00096CE0 File Offset: 0x00094EE0
		public override bool AsBool
		{
			get
			{
				JSONBool aVal = new JSONBool(false);
				this.Set(aVal);
				return false;
			}
			set
			{
				JSONBool aVal = new JSONBool(value);
				this.Set(aVal);
			}
		}

		// Token: 0x17000180 RID: 384
		// (get) Token: 0x06001A27 RID: 6695 RVA: 0x00096CFC File Offset: 0x00094EFC
		public override JSONArray AsArray
		{
			get
			{
				JSONArray jsonarray = new JSONArray();
				this.Set(jsonarray);
				return jsonarray;
			}
		}

		// Token: 0x17000181 RID: 385
		// (get) Token: 0x06001A28 RID: 6696 RVA: 0x00096D18 File Offset: 0x00094F18
		public override JSONObject AsObject
		{
			get
			{
				JSONObject jsonobject = new JSONObject();
				this.Set(jsonobject);
				return jsonobject;
			}
		}

		// Token: 0x06001A29 RID: 6697 RVA: 0x00012D94 File Offset: 0x00010F94
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append("null");
		}

		// Token: 0x040015DF RID: 5599
		private JSONNode m_Node;

		// Token: 0x040015E0 RID: 5600
		private string m_Key;
	}
}
