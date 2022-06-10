using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Stijn.Localization
{
	// Token: 0x02000487 RID: 1159
	public static class LocalizationManager
	{
		// Token: 0x1400000A RID: 10
		// (add) Token: 0x06001A4A RID: 6730 RVA: 0x0009816C File Offset: 0x0009636C
		// (remove) Token: 0x06001A4B RID: 6731 RVA: 0x000981A0 File Offset: 0x000963A0
		public static event LocalizationManager.LocalizationLoadedDelegate LocalizationLoadedEvent;

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06001A4C RID: 6732 RVA: 0x000981D4 File Offset: 0x000963D4
		// (remove) Token: 0x06001A4D RID: 6733 RVA: 0x00098208 File Offset: 0x00096408
		public static event LocalizationManager.LocalizationLoadedDelegate LocalizationLoadedFromGoogleSheetsEvent;

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06001A4E RID: 6734 RVA: 0x0009823C File Offset: 0x0009643C
		// (remove) Token: 0x06001A4F RID: 6735 RVA: 0x00098270 File Offset: 0x00096470
		public static event LocalizationManager.LanguageDelegate LanguageChangedEvent;

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x06001A50 RID: 6736 RVA: 0x000982A4 File Offset: 0x000964A4
		// (remove) Token: 0x06001A51 RID: 6737 RVA: 0x000982D8 File Offset: 0x000964D8
		public static event LocalizationManager.TokenDelegate TokenChangedEvent;

		// Token: 0x06001A52 RID: 6738 RVA: 0x00012F25 File Offset: 0x00011125
		public static void Initialize()
		{
			LocalizationManager.LoadLocalizationData();
		}

		// Token: 0x06001A53 RID: 6739 RVA: 0x000028DF File Offset: 0x00000ADF
		public static void Deinitialize()
		{
		}

		// Token: 0x06001A54 RID: 6740 RVA: 0x00012F2C File Offset: 0x0001112C
		public static void AddText(string key, List<string> translations)
		{
			if (LocalizationManager.s_Data != null)
			{
				LocalizationManager.s_Data.AddText(key, translations);
			}
		}

		// Token: 0x06001A55 RID: 6741 RVA: 0x0009830C File Offset: 0x0009650C
		public static void SetCustomToken(string key, string value)
		{
			if (LocalizationManager.s_CustomTokens == null)
			{
				LocalizationManager.s_CustomTokens = new Dictionary<string, string>();
			}
			if (LocalizationManager.s_CustomTokens.ContainsKey(key))
			{
				LocalizationManager.s_CustomTokens[key] = value;
			}
			else
			{
				LocalizationManager.s_CustomTokens.Add(key, value);
			}
			if (LocalizationManager.TokenChangedEvent != null)
			{
				LocalizationManager.TokenChangedEvent(key);
			}
		}

		// Token: 0x06001A56 RID: 6742 RVA: 0x00012F42 File Offset: 0x00011142
		public static void RemoveCustomToken(string key)
		{
			if (LocalizationManager.s_CustomTokens == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(key) || !LocalizationManager.s_CustomTokens.ContainsKey(key))
			{
				return;
			}
			LocalizationManager.s_CustomTokens.Remove(key);
			if (LocalizationManager.TokenChangedEvent != null)
			{
				LocalizationManager.TokenChangedEvent(key);
			}
		}

		// Token: 0x06001A57 RID: 6743 RVA: 0x00012F80 File Offset: 0x00011180
		public static string GetText(string key, params object[] customTokens)
		{
			LocalizationManager.LoadLocalizationData();
			return LocalizationManager.GetText(key, LocalizationManager.GetLanguage(), customTokens);
		}

		// Token: 0x06001A58 RID: 6744 RVA: 0x00098364 File Offset: 0x00096564
		private static string ReplaceTokens(string text, params object[] customTokens)
		{
			if (customTokens == null || customTokens.Length == 0)
			{
				return text;
			}
			bool flag = false;
			int num = 0;
			int num2 = 0;
			StringBuilder stringBuilder = new StringBuilder();
			while (!flag)
			{
				int num3 = text.IndexOf(LocalizationManager.s_TokenBrackets[0], num);
				if (num3 < 0)
				{
					stringBuilder.Append(text.Substring(num, text.Length - num));
					flag = true;
				}
				else
				{
					int num4 = text.IndexOf(LocalizationManager.s_TokenBrackets[1], num3 + LocalizationManager.s_TokenBrackets[0].Length);
					if (num4 < 0)
					{
						stringBuilder.Append(text.Substring(num, text.Length - num));
						flag = true;
					}
					else
					{
						stringBuilder.Append(text.Substring(num, num3 - num));
						string text2 = text.Substring(num3 + LocalizationManager.s_TokenBrackets[0].Length, num4 - num3 - LocalizationManager.s_TokenBrackets[0].Length);
						num = num3 + LocalizationManager.s_TokenBrackets[0].Length + text2.Length + LocalizationManager.s_TokenBrackets[1].Length;
						if (text2 == LocalizationManager.s_VersionNumberToken)
						{
							stringBuilder.Append(Application.unityVersion);
						}
						else if (LocalizationManager.s_CustomTokens != null && LocalizationManager.s_CustomTokens.ContainsKey(text2))
						{
							stringBuilder.Append(LocalizationManager.s_CustomTokens[text2]);
						}
						else if (customTokens != null && num2 < customTokens.Length)
						{
							stringBuilder.Append(customTokens[num2].ToString());
							num2++;
							if (num2 >= customTokens.Length)
							{
								stringBuilder.Append(text.Substring(num, text.Length - num));
								flag = true;
							}
						}
						else
						{
							stringBuilder.Append(LocalizationManager.s_TokenBrackets[0] + text2 + LocalizationManager.s_TokenBrackets[1]);
							stringBuilder.Append(text.Substring(num, text.Length - num));
							flag = true;
						}
					}
				}
			}
			return stringBuilder.ToString();
		}

		// Token: 0x06001A59 RID: 6745 RVA: 0x00098524 File Offset: 0x00096724
		private static string ReplaceCurlyBraceTokens(string text, params object[] customTokens)
		{
			bool flag = false;
			int num = 0;
			int num2 = 0;
			List<string> list = new List<string>();
			while (!flag)
			{
				int num3 = text.IndexOf("{", num);
				if (num3 < 0)
				{
					flag = true;
				}
				else
				{
					int num4 = text.IndexOf("}", num3 + 1);
					if (num4 < 0)
					{
						flag = true;
					}
					else
					{
						string text2 = text.Substring(num3 + 1, num4 - num3 - 1);
						string text3 = list.Count.ToString();
						text = text.Remove(num3 + 1, num4 - num3 - 1);
						text = text.Insert(num3 + 1, text3);
						num = num3 + text3.Length + 1;
						if (text2 == LocalizationManager.s_VersionNumberToken)
						{
							list.Add(Application.unityVersion);
						}
						else if (LocalizationManager.s_CustomTokens != null && LocalizationManager.s_CustomTokens.ContainsKey(text2))
						{
							list.Add(LocalizationManager.s_CustomTokens[text2]);
						}
						else if (customTokens != null && num2 < customTokens.Length)
						{
							list.Add(customTokens[num2].ToString());
							num2++;
						}
						else
						{
							list.Add(string.Empty);
						}
					}
				}
			}
			if (list.Count > 0)
			{
				string text4 = text;
				object[] array = list.ToArray();
				object[] array2 = array;
				return string.Format(text4, array2);
			}
			return text;
		}

		// Token: 0x06001A5A RID: 6746 RVA: 0x00012F93 File Offset: 0x00011193
		public static bool IsTextAvailable(string key)
		{
			return LocalizationManager.IsTextAvailable(key, LocalizationManager.s_Language);
		}

		// Token: 0x06001A5B RID: 6747 RVA: 0x00012FA0 File Offset: 0x000111A0
		private static void LoadLocalizationData()
		{
			if (LocalizationManager.s_Data == null)
			{
				LocalizationManager.s_Data = new LocalizationData();
				LocalizationManager.DeserializeFromFolder();
				LocalizationManager.SetLanguage("CHINESE");
			}
		}

		// Token: 0x06001A5C RID: 6748 RVA: 0x00012FC3 File Offset: 0x000111C3
		static LocalizationManager()
		{
			LocalizationManager.s_Data = null;
		}

		// Token: 0x06001A5D RID: 6749 RVA: 0x00012FF6 File Offset: 0x000111F6
		public static void SetLanguage(string languageKey)
		{
			if (LocalizationManager.s_Data != null)
			{
				LocalizationManager.SetLanguage(LocalizationManager.s_Data.GetLanguage(languageKey));
			}
		}

		// Token: 0x06001A5E RID: 6750 RVA: 0x0001300F File Offset: 0x0001120F
		public static void SetLanguage(int languageID)
		{
			if (LocalizationManager.s_Data != null)
			{
				LocalizationManager.SetLanguage(LocalizationManager.s_Data.GetLanguage(languageID));
			}
		}

		// Token: 0x06001A5F RID: 6751 RVA: 0x00013028 File Offset: 0x00011228
		private static void SetLanguage(LocalizationLanguage language)
		{
			if (language == null)
			{
				return;
			}
			LocalizationManager.s_Language = language;
			if (LocalizationManager.LanguageChangedEvent != null)
			{
				LocalizationManager.LanguageChangedEvent(language);
			}
		}

		// Token: 0x06001A60 RID: 6752 RVA: 0x00013046 File Offset: 0x00011246
		public static LocalizationLanguage GetLanguage()
		{
			return LocalizationManager.s_Language;
		}

		// Token: 0x06001A61 RID: 6753 RVA: 0x0009865C File Offset: 0x0009685C
		public static string GetText(string key, LocalizationLanguage language, params object[] customTokens)
		{
			if (key == null)
			{
				return "NO KEY FOUND";
			}
			key = key.Trim();
			LocalizationManager.LoadLocalizationData();
			string text = string.Empty;
			LocalizationManager.s_Data.GetText(key, language, out text);
			text = Regex.Unescape(text);
			text = LocalizationManager.ReplaceTokens(text, customTokens);
			return text;
		}

		// Token: 0x06001A62 RID: 6754 RVA: 0x000986A4 File Offset: 0x000968A4
		public static bool IsTextAvailable(string key, LocalizationLanguage language)
		{
			if (string.IsNullOrEmpty(key))
			{
				return false;
			}
			LocalizationManager.LoadLocalizationData();
			if (LocalizationManager.s_Data == null)
			{
				return false;
			}
			string empty = string.Empty;
			return LocalizationManager.s_Data.GetText(key, language, out empty);
		}

		// Token: 0x06001A63 RID: 6755 RVA: 0x0001304D File Offset: 0x0001124D
		public static void ClearLocalizationData()
		{
			LocalizationManager.LoadLocalizationData();
			LocalizationManager.s_Data.Clear();
		}

		// Token: 0x06001A64 RID: 6756 RVA: 0x000986E0 File Offset: 0x000968E0
		public static bool DeserializeFromFile(bool json)
		{
			LocalizationManager.LoadLocalizationData();
			string dataPath = LocalizationManager.s_Data.DataPath;
			string fileText = string.Empty;
			try
			{
				fileText = File.ReadAllText(dataPath, Encoding.UTF8);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				if (LocalizationManager.LocalizationLoadedEvent != null)
				{
					LocalizationManager.LocalizationLoadedEvent(false);
				}
				return false;
			}
			bool flag;
			if (!json)
			{
				flag = LocalizationManager.s_Data.DeserializeFromCSV(fileText);
			}
			else
			{
				flag = LocalizationManager.s_Data.DeserializeFromJSON(fileText);
			}
			if (LocalizationManager.LocalizationLoadedEvent != null)
			{
				LocalizationManager.LocalizationLoadedEvent(flag);
			}
			return flag;
		}

		// Token: 0x06001A65 RID: 6757 RVA: 0x00098774 File Offset: 0x00096974
		public static bool DeserializeFromFolder()
		{
			LocalizationManager.LoadLocalizationData();
			bool flag = true;
			string dataPath = LocalizationManager.s_Data.DataPath;
			DirectoryInfo directoryInfo = new DirectoryInfo(dataPath);
			if (directoryInfo == null || !directoryInfo.Exists)
			{
				directoryInfo = new FileInfo(dataPath).Directory;
				if (directoryInfo == null || !directoryInfo.Exists)
				{
					return false;
				}
			}
			FileInfo[] files = directoryInfo.GetFiles("*.json");
			if (files == null)
			{
				return false;
			}
			foreach (FileInfo fileInfo in files)
			{
				if (fileInfo != null && flag)
				{
					string fileText = string.Empty;
					try
					{
						fileText = File.ReadAllText(fileInfo.FullName, Encoding.UTF8);
					}
					catch (Exception ex)
					{
						Debug.LogError(ex.Message);
						if (LocalizationManager.LocalizationLoadedEvent != null)
						{
							LocalizationManager.LocalizationLoadedEvent(false);
						}
						return false;
					}
					flag = LocalizationManager.s_Data.DeserializeFromJSON(fileText);
				}
			}
			if (LocalizationManager.LocalizationLoadedEvent != null)
			{
				LocalizationManager.LocalizationLoadedEvent(flag);
			}
			return flag;
		}

		// Token: 0x040015ED RID: 5613
		private static string[] s_TokenBrackets = new string[]
		{
			"[",
			"]"
		};

		// Token: 0x040015EE RID: 5614
		private static string s_VersionNumberToken = "VERSION_NUMBER";

		// Token: 0x040015EF RID: 5615
		private static Dictionary<string, string> s_CustomTokens;

		// Token: 0x040015F0 RID: 5616
		private static LocalizationData s_Data;

		// Token: 0x040015F5 RID: 5621
		private static LocalizationLanguage s_Language = null;

		// Token: 0x02000488 RID: 1160
		// (Invoke) Token: 0x06001A67 RID: 6759
		public delegate void LocalizationLoadedDelegate(bool success);

		// Token: 0x02000489 RID: 1161
		// (Invoke) Token: 0x06001A6C RID: 6764
		public delegate void LanguageDelegate(LocalizationLanguage language);

		// Token: 0x0200048A RID: 1162
		// (Invoke) Token: 0x06001A6F RID: 6767
		public delegate void TokenDelegate(string token);
	}
}
