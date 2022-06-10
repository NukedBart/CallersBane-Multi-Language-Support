using System;
using UnityEngine;

namespace Stijn.Localization
{
	// Token: 0x0200048B RID: 1163
	[Serializable]
	public class LocalizationLanguage
	{
		// Token: 0x17000184 RID: 388
		// (get) Token: 0x06001A72 RID: 6770 RVA: 0x0001305E File Offset: 0x0001125E
		public string Key
		{
			get
			{
				return this.m_Key;
			}
		}

		// Token: 0x17000185 RID: 389
		// (get) Token: 0x06001A73 RID: 6771 RVA: 0x00013066 File Offset: 0x00011266
		public string LocalizationKey
		{
			get
			{
				return this.m_LocalizationKey;
			}
		}

		// Token: 0x17000186 RID: 390
		// (get) Token: 0x06001A74 RID: 6772 RVA: 0x0001306E File Offset: 0x0001126E
		public string CultureCode
		{
			get
			{
				return this.m_CultureCode;
			}
		}

		// Token: 0x06001A75 RID: 6773 RVA: 0x00013076 File Offset: 0x00011276
		public LocalizationLanguage(string key, string localizationKey, string cultureCode)
		{
			this.m_Key = key;
			this.m_LocalizationKey = localizationKey;
			this.m_CultureCode = cultureCode;
		}

		// Token: 0x040015F6 RID: 5622
		[SerializeField]
		private string m_Key = string.Empty;

		// Token: 0x040015F7 RID: 5623
		[SerializeField]
		private string m_LocalizationKey = string.Empty;

		// Token: 0x040015F8 RID: 5624
		[SerializeField]
		private string m_CultureCode = string.Empty;
	}
}
