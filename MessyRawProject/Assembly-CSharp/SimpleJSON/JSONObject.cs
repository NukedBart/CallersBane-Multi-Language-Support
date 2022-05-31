using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x0200047B RID: 1147
	public class JSONObject : JSONNode
	{
		// Token: 0x17000161 RID: 353
		// (get) Token: 0x060019C4 RID: 6596 RVA: 0x00012AF3 File Offset: 0x00010CF3
		// (set) Token: 0x060019C5 RID: 6597 RVA: 0x00012AFB File Offset: 0x00010CFB
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

		// Token: 0x17000162 RID: 354
		// (get) Token: 0x060019C6 RID: 6598 RVA: 0x0000D620 File Offset: 0x0000B820
		public override JSONNodeType Tag
		{
			get
			{
				return JSONNodeType.Object;
			}
		}

		// Token: 0x17000163 RID: 355
		// (get) Token: 0x060019C7 RID: 6599 RVA: 0x00004AAC File Offset: 0x00002CAC
		public override bool IsObject
		{
			get
			{
				return true;
			}
		}

		// Token: 0x060019C8 RID: 6600 RVA: 0x00012B04 File Offset: 0x00010D04
		public override JSONNode.Enumerator GetEnumerator()
		{
			return new JSONNode.Enumerator(this.m_Dict.GetEnumerator());
		}

		// Token: 0x17000164 RID: 356
		public override JSONNode this[string aKey]
		{
			get
			{
				if (this.m_Dict.ContainsKey(aKey))
				{
					return this.m_Dict[aKey];
				}
				return new JSONLazyCreator(this, aKey);
			}
			set
			{
				if (value == null)
				{
					value = JSONNull.CreateOrGet();
				}
				if (this.m_Dict.ContainsKey(aKey))
				{
					this.m_Dict[aKey] = value;
					return;
				}
				this.m_Dict.Add(aKey, value);
			}
		}

		// Token: 0x17000165 RID: 357
		public override JSONNode this[int aIndex]
		{
			get
			{
				if (aIndex < 0 || aIndex >= this.m_Dict.Count)
				{
					return null;
				}
				return Enumerable.ElementAt<KeyValuePair<string, JSONNode>>(this.m_Dict, aIndex).Value;
			}
			set
			{
				if (value == null)
				{
					value = JSONNull.CreateOrGet();
				}
				if (aIndex < 0 || aIndex >= this.m_Dict.Count)
				{
					return;
				}
				string key = Enumerable.ElementAt<KeyValuePair<string, JSONNode>>(this.m_Dict, aIndex).Key;
				this.m_Dict[key] = value;
			}
		}

		// Token: 0x17000166 RID: 358
		// (get) Token: 0x060019CD RID: 6605 RVA: 0x00012B75 File Offset: 0x00010D75
		public override int Count
		{
			get
			{
				return this.m_Dict.Count;
			}
		}

		// Token: 0x060019CE RID: 6606 RVA: 0x000966EC File Offset: 0x000948EC
		public override void Add(string aKey, JSONNode aItem)
		{
			if (aItem == null)
			{
				aItem = JSONNull.CreateOrGet();
			}
			if (string.IsNullOrEmpty(aKey))
			{
				this.m_Dict.Add(Guid.NewGuid().ToString(), aItem);
				return;
			}
			if (this.m_Dict.ContainsKey(aKey))
			{
				this.m_Dict[aKey] = aItem;
				return;
			}
			this.m_Dict.Add(aKey, aItem);
		}

		// Token: 0x060019CF RID: 6607 RVA: 0x00012B82 File Offset: 0x00010D82
		public override JSONNode Remove(string aKey)
		{
			if (!this.m_Dict.ContainsKey(aKey))
			{
				return null;
			}
			JSONNode result = this.m_Dict[aKey];
			this.m_Dict.Remove(aKey);
			return result;
		}

		// Token: 0x060019D0 RID: 6608 RVA: 0x0009675C File Offset: 0x0009495C
		public override JSONNode Remove(int aIndex)
		{
			if (aIndex < 0 || aIndex >= this.m_Dict.Count)
			{
				return null;
			}
			KeyValuePair<string, JSONNode> keyValuePair = Enumerable.ElementAt<KeyValuePair<string, JSONNode>>(this.m_Dict, aIndex);
			this.m_Dict.Remove(keyValuePair.Key);
			return keyValuePair.Value;
		}

		// Token: 0x060019D1 RID: 6609 RVA: 0x000967A4 File Offset: 0x000949A4
		public override JSONNode Remove(JSONNode aNode)
		{
			JSONNode result;
			try
			{
				KeyValuePair<string, JSONNode> keyValuePair = Enumerable.First<KeyValuePair<string, JSONNode>>(Enumerable.Where<KeyValuePair<string, JSONNode>>(this.m_Dict, (KeyValuePair<string, JSONNode> k) => k.Value == aNode));
				this.m_Dict.Remove(keyValuePair.Key);
				result = aNode;
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x17000167 RID: 359
		// (get) Token: 0x060019D2 RID: 6610 RVA: 0x00012BAD File Offset: 0x00010DAD
		public override IEnumerable<JSONNode> Children
		{
			get
			{
				foreach (KeyValuePair<string, JSONNode> keyValuePair in this.m_Dict)
				{
					yield return keyValuePair.Value;
				}
				Dictionary<string, JSONNode>.Enumerator enumerator = default(Dictionary<string, JSONNode>.Enumerator);
				yield break;
				yield break;
			}
		}

		// Token: 0x060019D3 RID: 6611 RVA: 0x00096810 File Offset: 0x00094A10
		internal override void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode)
		{
			aSB.Append('{');
			bool flag = true;
			if (this.inline)
			{
				aMode = JSONTextMode.Compact;
			}
			foreach (KeyValuePair<string, JSONNode> keyValuePair in this.m_Dict)
			{
				if (!flag)
				{
					aSB.Append(',');
				}
				flag = false;
				if (aMode == JSONTextMode.Indent)
				{
					aSB.AppendLine();
				}
				if (aMode == JSONTextMode.Indent)
				{
					aSB.Append(' ', aIndent + aIndentInc);
				}
				aSB.Append('"').Append(JSONNode.Escape(keyValuePair.Key)).Append('"');
				if (aMode == JSONTextMode.Compact)
				{
					aSB.Append(':');
				}
				else
				{
					aSB.Append(" : ");
				}
				keyValuePair.Value.WriteToStringBuilder(aSB, aIndent + aIndentInc, aIndentInc, aMode);
			}
			if (aMode == JSONTextMode.Indent)
			{
				aSB.AppendLine().Append(' ', aIndent);
			}
			aSB.Append('}');
		}

		// Token: 0x060019D4 RID: 6612 RVA: 0x00012BBD File Offset: 0x00010DBD
		public override void Clear()
		{
			this.m_Dict.Clear();
		}

		// Token: 0x040015D2 RID: 5586
		private Dictionary<string, JSONNode> m_Dict = new Dictionary<string, JSONNode>();

		// Token: 0x040015D3 RID: 5587
		private bool inline;
	}
}
