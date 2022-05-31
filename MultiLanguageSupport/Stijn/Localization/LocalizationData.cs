using System;
using System.Collections.Generic;
using UnityEngine;

namespace Stijn.Localization
{
	// Token: 0x02000486 RID: 1158
	public class LocalizationData
	{
		// Token: 0x17000182 RID: 386
		// (get) Token: 0x06001A37 RID: 6711 RVA: 0x00012E66 File Offset: 0x00011066
		public string GoogleScriptURL
		{
			get
			{
				return this.m_GoogleScriptURL;
			}
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06001A38 RID: 6712 RVA: 0x00012E6E File Offset: 0x0001106E
		public string DataFilePath
		{
			get
			{
				return this.m_DataFilePath;
			}
		}

		// Token: 0x06001A39 RID: 6713 RVA: 0x00097568 File Offset: 0x00095768
		public bool AddText(string key, List<string> translations)
		{
			if (this.m_Data == null)
			{
				Debug.Log("LocalizationData/AddText: Please parse the database first");
				return false;
			}
			if (string.IsNullOrEmpty(key))
			{
				Debug.Log("LocalizationData/AddText: Enter a key");
				return false;
			}
			int num = Enum.GetNames(typeof(LocalizationManager.Language)).Length;
			if (translations.Count != num)
			{
				Debug.Log("LocalizationData/AddText: Invalid amount of translations. Currently there are " + num + " languages available");
				return false;
			}
			this.m_Data[key] = translations;
			return true;
		}

		// Token: 0x06001A3A RID: 6714 RVA: 0x000975E4 File Offset: 0x000957E4
		public bool Deserialize(string fileText)
		{
			if (this.m_Data != null)
			{
				this.m_Data.Clear();
			}
			this.m_Data = new StringAndStringListDictionary();
			string[,] array = UtilityMethods.ParseCSVRaw(fileText);
			if (array == null)
			{
				return false;
			}
			if (array.GetLength(1) < 2)
			{
				Debug.LogError("The localization file does not contain any data!");
				return false;
			}
			for (int i = 0; i < array.GetLength(1); i++)
			{
				string text = array[0, i];
				List<string> list = new List<string>();
				for (int j = 1; j < array.GetLength(0); j++)
				{
					list.Add(array[j, i]);
				}
				if (this.m_Data.ContainsKey(text))
				{
					Debug.LogError("Localization parsing: " + text + " is already in use!");
				}
				else
				{
					this.m_Data.Add(text, list);
				}
			}
			Debug.Log("Localization update successful!");
			return true;
		}

		// Token: 0x06001A3C RID: 6716 RVA: 0x000976B4 File Offset: 0x000958B4
		public bool GetText(string key, LocalizationManager.Language language, out string text)
		{
			if (this.m_Data == null)
			{
				text = "DATA NOT READ: Please parse the database first.";
				return false;
			}
			if (string.IsNullOrEmpty(key))
			{
				text = "Enter a key";
				return false;
			}
			if (!this.m_Data.ContainsKey(key))
			{
				text = "INVALID KEY: " + key + " does not exist.";
				return false;
			}
			int num = (int)language;
			int num2 = Enum.GetNames(typeof(LocalizationManager.Language)).Length;
			if (num >= num2)
			{
				text = "INVALID LANGUAGE: There are currently only " + num2 + " available.";
				return false;
			}
			string text2 = this.m_Data[key][num];
			if (string.IsNullOrEmpty(text2))
			{
				text2 = string.Concat(new string[]
				{
					"No ",
					language.ToString(),
					" translation for ",
					key,
					" yet!"
				});
			}
			text = text2;
			return true;
		}

		// Token: 0x040015E5 RID: 5605
		[SerializeField]
		private string m_GoogleScriptURL = string.Empty;

		// Token: 0x040015E6 RID: 5606
		[SerializeField]
		private string m_DataFilePath = "./Localization.txt";

		// Token: 0x040015E7 RID: 5607
		[SerializeField]
		private StringAndStringListDictionary m_Data;
	}
}
