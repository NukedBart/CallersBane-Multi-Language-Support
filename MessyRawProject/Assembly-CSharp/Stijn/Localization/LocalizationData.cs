using System;
using System.Collections.Generic;
using SimpleJSON;
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

		// Token: 0x06001A38 RID: 6712 RVA: 0x0009779C File Offset: 0x0009599C
		public bool AddText(string key, List<string> translations)
		{
			if (this.m_Data == null || this.m_Languages == null)
			{
				Debug.Log("LocalizationData/AddText: Please parse the database first");
				return false;
			}
			if (string.IsNullOrEmpty(key))
			{
				Debug.Log("LocalizationData/AddText: Enter a key");
				return false;
			}
			if (translations.Count < this.m_Languages.Count)
			{
				for (int i = 0; i < this.m_Languages.Count - translations.Count; i++)
				{
					translations.Add(string.Empty);
				}
			}
			if (this.m_Data.ContainsKey(key))
			{
				List<string> list = this.m_Data[key];
				for (int j = 0; j < translations.Count; j++)
				{
					if (string.IsNullOrEmpty(translations[j]))
					{
						translations[j] = list[j];
					}
				}
			}
			this.m_Data[key] = translations;
			return true;
		}

		// Token: 0x06001A3A RID: 6714 RVA: 0x0009786C File Offset: 0x00095A6C
		public bool AddLanguage(string key, string localizationKey, string cultureCode)
		{
			Debug.Log(string.Concat(new string[]
			{
				"Adding Language: ",
				key,
				" ",
				localizationKey,
				" ",
				cultureCode
			}));
			if (this.GetLanguage(key) == null)
			{
				this.m_Languages.Add(new LocalizationLanguage(key, localizationKey, cultureCode));
				return true;
			}
			return false;
		}

		// Token: 0x06001A3B RID: 6715 RVA: 0x000978CC File Offset: 0x00095ACC
		public LocalizationLanguage GetLanguage(string key)
		{
			int languageID = this.GetLanguageID(key);
			if (languageID < 0)
			{
				return null;
			}
			return this.m_Languages[languageID];
		}

		// Token: 0x06001A3C RID: 6716 RVA: 0x00012E8C File Offset: 0x0001108C
		public LocalizationLanguage GetLanguage(int id)
		{
			if (this.m_Languages == null)
			{
				return null;
			}
			if (id < 0 || id >= this.m_Languages.Count)
			{
				return null;
			}
			return this.m_Languages[id];
		}

		// Token: 0x06001A3D RID: 6717 RVA: 0x00012EB8 File Offset: 0x000110B8
		public int GetLanguageID(LocalizationLanguage language)
		{
			if (language == null)
			{
				return -1;
			}
			return this.GetLanguageID(language.Key);
		}

		// Token: 0x06001A3E RID: 6718 RVA: 0x000978F4 File Offset: 0x00095AF4
		public int GetLanguageID(string key)
		{
			if (this.m_Languages == null)
			{
				return -1;
			}
			int result = -1;
			for (int i = 0; i < this.m_Languages.Count; i++)
			{
				if (this.m_Languages[i].Key == key)
				{
					result = i;
					break;
				}
			}
			return result;
		}

		// Token: 0x06001A3F RID: 6719 RVA: 0x00097944 File Offset: 0x00095B44
		public bool AddText(string key, string translation, LocalizationLanguage language)
		{
			if (language == null)
			{
				Debug.Log("INVALID LANGUAGE: Language is null.");
				return false;
			}
			int languageID = this.GetLanguageID(language);
			if (languageID < 0)
			{
				Debug.Log("INVALID LANGUAGE: No language with key '" + language.Key + "' exists.");
				return false;
			}
			return this.AddText(key, translation, languageID);
		}

		// Token: 0x06001A40 RID: 6720 RVA: 0x00097994 File Offset: 0x00095B94
		public bool AddText(string key, string translation, string languageKey)
		{
			int languageID = this.GetLanguageID(languageKey);
			if (languageID < 0)
			{
				Debug.Log("INVALID LANGUAGE: No language with key '" + languageKey + "' exists.");
				return false;
			}
			return this.AddText(key, translation, languageID);
		}

		// Token: 0x06001A41 RID: 6721 RVA: 0x000979D0 File Offset: 0x00095BD0
		public bool AddText(string key, string translation, int languageID)
		{
			if (this.m_Data == null || this.m_Languages == null)
			{
				Debug.Log("LocalizationData/AddText: Please parse the database first");
				return false;
			}
			List<string> list;
			if (this.m_Data.ContainsKey(key))
			{
				list = this.m_Data[key];
				if (list.Count < this.m_Languages.Count)
				{
					for (int i = 0; i < this.m_Languages.Count - list.Count; i++)
					{
						list.Add(string.Empty);
					}
				}
			}
			else
			{
				list = new List<string>(this.m_Languages.Count);
				for (int j = 0; j < this.m_Languages.Count; j++)
				{
					list.Add(string.Empty);
				}
				this.m_Data[key] = list;
			}
			list[languageID] = translation;
			return true;
		}

		// Token: 0x06001A42 RID: 6722 RVA: 0x00097A9C File Offset: 0x00095C9C
		public bool GetText(string key, LocalizationLanguage language, out string text)
		{
			if (language == null)
			{
				text = "INVALID LANGUAGE: Language is null.";
				return false;
			}
			int languageID = this.GetLanguageID(language);
			if (languageID < 0)
			{
				text = "INVALID LANGUAGE: No language with key '" + language.Key + "' exists.";
				return false;
			}
			return this.GetText(key, languageID, out text);
		}

		// Token: 0x06001A43 RID: 6723 RVA: 0x00097AE4 File Offset: 0x00095CE4
		public bool GetText(string key, string languageKey, out string text)
		{
			int languageID = this.GetLanguageID(languageKey);
			if (languageID < 0)
			{
				text = "INVALID LANGUAGE: No language with key '" + languageKey + "' exists.";
				return false;
			}
			return this.GetText(key, languageID, out text);
		}

		// Token: 0x06001A44 RID: 6724 RVA: 0x00097B1C File Offset: 0x00095D1C
		public bool GetText(string key, int languageID, out string text)
		{
			if (this.m_Data == null)
			{
				text = "DATA NOT READ: Please parse the database first.";
				return false;
			}
			if (this.m_Languages == null)
			{
				text = "INVALID LANGUAGE: There are no languages available.";
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
			if (languageID >= this.m_Languages.Count)
			{
				text = "INVALID LANGUAGE: There are currently only " + this.m_Languages.Count + " available.";
				return false;
			}
			string text2 = this.m_Data[key][languageID];
			if (string.IsNullOrEmpty(text2))
			{
				LocalizationLanguage localizationLanguage = this.m_Languages[languageID];
				text2 = "No translation for " + key + " yet!";
			}
			text = text2;
			return true;
		}

		// Token: 0x06001A45 RID: 6725 RVA: 0x00012ECB File Offset: 0x000110CB
		public void Clear()
		{
			if (this.m_Languages != null)
			{
				this.m_Languages.Clear();
			}
			if (this.m_Data != null)
			{
				this.m_Data.Clear();
			}
		}

		// Token: 0x06001A46 RID: 6726 RVA: 0x00097BEC File Offset: 0x00095DEC
		public bool DeserializeFromCSV(string fileText)
		{
			if (this.m_Languages == null)
			{
				this.m_Languages = new List<LocalizationLanguage>();
			}
			if (this.m_Data == null)
			{
				this.m_Data = new StringAndStringListDictionary();
			}
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
					this.m_Data[text] = list;
				}
				else
				{
					this.m_Data.Add(text, list);
				}
			}
			Debug.Log("Localization update successful!");
			return true;
		}

		// Token: 0x06001A47 RID: 6727 RVA: 0x00097CBC File Offset: 0x00095EBC
		public bool DeserializeFromJSON(string fileText)
		{
			if (this.m_Languages == null)
			{
				this.m_Languages = new List<LocalizationLanguage>();
			}
			if (this.m_Data == null)
			{
				this.m_Data = new StringAndStringListDictionary();
			}
			JSONNode jsonnode = null;
			try
			{
				jsonnode = JSON.Parse(fileText);
			}
			catch (Exception ex)
			{
				Debug.LogWarning("Unable to parse JSON file: " + ex.ToString());
			}
			if (jsonnode == null)
			{
				Debug.LogWarning("Unable to parse JSON file!");
				return false;
			}
			JSONNode jsonnode2 = jsonnode[LocalizationData.s_LanguagesVariableName];
			if (jsonnode2 == null)
			{
				Debug.LogWarning("No '" + LocalizationData.s_LanguagesVariableName + "' node found!");
				return false;
			}
			JSONObject asObject = jsonnode2.AsObject;
			if (asObject == null)
			{
				Debug.LogWarning("The '" + LocalizationData.s_LanguagesVariableName + "' node is not an Object");
				return false;
			}
			JSONNode jsonnode3 = jsonnode[LocalizationData.s_TextVariableName];
			if (jsonnode3 == null)
			{
				Debug.LogWarning("No '" + LocalizationData.s_TextVariableName + "' node found!");
				return false;
			}
			JSONObject asObject2 = jsonnode3.AsObject;
			if (asObject2 == null)
			{
				Debug.LogWarning("The '" + LocalizationData.s_TextVariableName + "' node is not an Object");
				return false;
			}
			List<int> list = new List<int>();
			foreach (JSONNode jsonnode4 in asObject.Keys)
			{
				if (jsonnode4 != null && jsonnode4.IsString)
				{
					string value = jsonnode4.Value;
					int languageID = this.GetLanguageID(value);
					if (languageID >= 0)
					{
						list.Add(languageID);
					}
					else
					{
						JSONNode jsonnode5 = asObject[jsonnode4.Value];
						if (jsonnode5 != null && !jsonnode5.IsObject)
						{
							Debug.LogWarning("Language '" + jsonnode4.Value + "' isn't an object");
						}
						else
						{
							JSONObject asObject3 = jsonnode5.AsObject;
							JSONNode jsonnode6 = asObject3[LocalizationData.s_LanguageLocalizationKeyVariableName];
							if (jsonnode6 == null)
							{
								Debug.LogWarning(string.Concat(new string[]
								{
									"Language '",
									jsonnode4.Value,
									"' doesn't have a '",
									LocalizationData.s_LanguageLocalizationKeyVariableName,
									"' node"
								}));
							}
							else if (!jsonnode6.IsString)
							{
								Debug.LogWarning(string.Concat(new string[]
								{
									"Language '",
									jsonnode4.Value,
									"' has a non-string '",
									LocalizationData.s_LanguageLocalizationKeyVariableName,
									"' node"
								}));
							}
							else
							{
								string value2 = jsonnode6.Value;
								JSONNode jsonnode7 = asObject3[LocalizationData.s_LanguageCultureCodeVariableName];
								if (jsonnode7 == null)
								{
									Debug.LogWarning(string.Concat(new string[]
									{
										"Language '",
										jsonnode4.Value,
										"' doesn't have a '",
										LocalizationData.s_LanguageCultureCodeVariableName,
										"' node"
									}));
								}
								else if (!jsonnode7.IsString)
								{
									Debug.LogWarning(string.Concat(new string[]
									{
										"Language '",
										jsonnode4.Value,
										"' has a non-string '",
										LocalizationData.s_LanguageCultureCodeVariableName,
										"' node"
									}));
								}
								else
								{
									string value3 = jsonnode7.Value;
									this.AddLanguage(value, value2, value3);
									list.Add(this.m_Languages.Count - 1);
								}
							}
						}
					}
				}
			}
			foreach (JSONNode jsonnode8 in asObject2.Keys)
			{
				if (jsonnode8 != null && jsonnode8.IsString)
				{
					string value4 = jsonnode8.Value;
					JSONNode jsonnode9 = asObject2[jsonnode8.Value];
					if (jsonnode9 != null)
					{
						if (jsonnode9.IsArray)
						{
							JSONArray asArray = jsonnode9.AsArray;
							for (int i = 0; i < asArray.Count; i++)
							{
								JSONNode jsonnode10 = asArray[i];
								if (!jsonnode10.IsString)
								{
									Debug.LogWarning(string.Concat(new object[]
									{
										"The '",
										value4,
										"' value for language ",
										list[i],
										" is not a string"
									}));
								}
								else
								{
									this.AddText(value4, jsonnode10.Value, list[i]);
								}
							}
						}
						else if (jsonnode9.IsString && list.Count == 1)
						{
							this.AddText(value4, jsonnode9.Value, list[0]);
						}
						else
						{
							Debug.LogWarning("The '" + value4 + "' value is not an an Array (or string if there is only 1 language");
						}
					}
				}
			}
			Debug.Log("Localization update successful!");
			return true;
		}

		// Token: 0x17000183 RID: 387
		// (get) Token: 0x06001A49 RID: 6729 RVA: 0x00012F1D File Offset: 0x0001111D
		public string DataPath
		{
			get
			{
				return this.m_DataPath;
			}
		}

		// Token: 0x040015E5 RID: 5605
		[SerializeField]
		private string m_GoogleScriptURL = string.Empty;

		// Token: 0x040015E6 RID: 5606
		[SerializeField]
		private StringAndStringListDictionary m_Data;

		// Token: 0x040015E7 RID: 5607
		private static string s_LanguagesVariableName = "languages";

		// Token: 0x040015E8 RID: 5608
		private static string s_LanguageLocalizationKeyVariableName = "localization_key";

		// Token: 0x040015E9 RID: 5609
		private static string s_LanguageCultureCodeVariableName = "culture_code";

		// Token: 0x040015EA RID: 5610
		private static string s_TextVariableName = "text";

		// Token: 0x040015EB RID: 5611
		[SerializeField]
		private string m_DataPath = "./Localization/";

		// Token: 0x040015EC RID: 5612
		[SerializeField]
		private List<LocalizationLanguage> m_Languages;
	}
}
