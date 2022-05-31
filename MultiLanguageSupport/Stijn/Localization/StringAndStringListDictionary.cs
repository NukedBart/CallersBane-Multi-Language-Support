using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stijn.Localization
{
	// Token: 0x0200046D RID: 1133
	[Serializable]
	public class StringAndStringListDictionary : Dictionary<string, List<string>>
	{
		// Token: 0x0600193C RID: 6460 RVA: 0x00095B00 File Offset: 0x00093D00
		public void OnBeforeSerialize()
		{
			this.m_Dictionary.Clear();
			foreach (KeyValuePair<string, List<string>> keyValuePair in this)
			{
				StringAndStringListDictionary.StringAndStringListPair stringAndStringListPair = new StringAndStringListDictionary.StringAndStringListPair(keyValuePair.Key, keyValuePair.Value);
				this.m_Dictionary.Add(stringAndStringListPair);
			}
		}

		// Token: 0x0600193D RID: 6461 RVA: 0x00095B74 File Offset: 0x00093D74
		public void OnAfterDeserialize()
		{
			base.Clear();
			for (int i = 0; i < this.m_Dictionary.Count; i++)
			{
				base.Add(this.m_Dictionary[i].Key, this.m_Dictionary[i].ValueList);
			}
		}

		// Token: 0x040015A6 RID: 5542
		[SerializeField]
		private List<StringAndStringListDictionary.StringAndStringListPair> m_Dictionary = new List<StringAndStringListDictionary.StringAndStringListPair>();

		// Token: 0x0200046E RID: 1134
		[Serializable]
		private class StringAndStringListPair
		{
			// Token: 0x17000134 RID: 308
			// (get) Token: 0x0600193F RID: 6463 RVA: 0x000125E7 File Offset: 0x000107E7
			public string Key
			{
				get
				{
					return this.m_Key;
				}
			}

			// Token: 0x17000135 RID: 309
			// (get) Token: 0x06001940 RID: 6464 RVA: 0x000125EF File Offset: 0x000107EF
			public List<string> ValueList
			{
				get
				{
					return this.m_ValueList;
				}
			}

			// Token: 0x06001941 RID: 6465 RVA: 0x000125F7 File Offset: 0x000107F7
			public StringAndStringListPair(string key, List<string> valueList)
			{
				this.m_Key = key;
				this.m_ValueList = valueList;
			}

			// Token: 0x040015A7 RID: 5543
			[SerializeField]
			private string m_Key;

			// Token: 0x040015A8 RID: 5544
			[SerializeField]
			private List<string> m_ValueList;
		}
	}
}
