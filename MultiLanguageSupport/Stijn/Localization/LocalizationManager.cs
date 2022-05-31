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
		// (add) Token: 0x06001A3D RID: 6717 RVA: 0x00097790 File Offset: 0x00095990
		// (remove) Token: 0x06001A3E RID: 6718 RVA: 0x000977C4 File Offset: 0x000959C4
		public static event LocalizationManager.LocalizationLoadedDelegate LocalizationLoadedEvent;

		// Token: 0x1400000B RID: 11
		// (add) Token: 0x06001A3F RID: 6719 RVA: 0x000977F8 File Offset: 0x000959F8
		// (remove) Token: 0x06001A40 RID: 6720 RVA: 0x0009782C File Offset: 0x00095A2C
		public static event LocalizationManager.LocalizationLoadedDelegate LocalizationLoadedFromGoogleSheetsEvent;

		// Token: 0x1400000C RID: 12
		// (add) Token: 0x06001A41 RID: 6721 RVA: 0x00097860 File Offset: 0x00095A60
		// (remove) Token: 0x06001A42 RID: 6722 RVA: 0x00097894 File Offset: 0x00095A94
		public static event LocalizationManager.LanguageDelegate LanguageChangedEvent;

		// Token: 0x1400000D RID: 13
		// (add) Token: 0x06001A43 RID: 6723 RVA: 0x000978C8 File Offset: 0x00095AC8
		// (remove) Token: 0x06001A44 RID: 6724 RVA: 0x000978FC File Offset: 0x00095AFC
		public static event LocalizationManager.TokenDelegate TokenChangedEvent;

		// Token: 0x06001A45 RID: 6725 RVA: 0x00012E94 File Offset: 0x00011094
		public static void Initialize()
		{
			LocalizationManager.LoadLocalizationData();
		}

		// Token: 0x06001A46 RID: 6726 RVA: 0x000028DF File Offset: 0x00000ADF
		public static void Deinitialize()
		{
		}

		// Token: 0x06001A47 RID: 6727 RVA: 0x00012E9B File Offset: 0x0001109B
		public static void AddText(string key, List<string> translations)
		{
			if (LocalizationManager.s_Data != null)
			{
				LocalizationManager.s_Data.AddText(key, translations);
			}
		}

		// Token: 0x06001A48 RID: 6728 RVA: 0x00012EB1 File Offset: 0x000110B1
		public static void SetLanguage(LocalizationManager.Language language)
		{
			LocalizationManager.s_Language = language;
			string text = LocalizationManager.s_LanguageCultures[(int)language];
			if (LocalizationManager.LanguageChangedEvent != null)
			{
				LocalizationManager.LanguageChangedEvent(language);
			}
		}

		// Token: 0x06001A49 RID: 6729 RVA: 0x00097930 File Offset: 0x00095B30
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

		// Token: 0x06001A4A RID: 6730 RVA: 0x00012ED3 File Offset: 0x000110D3
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

		// Token: 0x06001A4B RID: 6731 RVA: 0x00012F11 File Offset: 0x00011111
		public static LocalizationManager.Language GetLanguage()
		{
			return LocalizationManager.s_Language;
		}

		// Token: 0x06001A4C RID: 6732 RVA: 0x00012F18 File Offset: 0x00011118
		public static string GetText(string key, params object[] customTokens)
		{
			return LocalizationManager.GetText(key, LocalizationManager.GetLanguage(), customTokens);
		}

		// Token: 0x06001A4D RID: 6733 RVA: 0x00097988 File Offset: 0x00095B88
		public static string GetText(string key, LocalizationManager.Language language, params object[] customTokens)
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

		// Token: 0x06001A4E RID: 6734 RVA: 0x000979D0 File Offset: 0x00095BD0
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

		// Token: 0x06001A4F RID: 6735 RVA: 0x00097B90 File Offset: 0x00095D90
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

		// Token: 0x06001A50 RID: 6736 RVA: 0x00012F26 File Offset: 0x00011126
		public static bool IsTextAvailable(string key)
		{
			return LocalizationManager.IsTextAvailable(key, LocalizationManager.s_Language);
		}

		// Token: 0x06001A51 RID: 6737 RVA: 0x00097CC8 File Offset: 0x00095EC8
		public static bool IsTextAvailable(string key, LocalizationManager.Language language)
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

		// Token: 0x06001A52 RID: 6738 RVA: 0x00012F33 File Offset: 0x00011133
		private static void LoadLocalizationData()
		{
			if (LocalizationManager.s_Data == null)
			{
				LocalizationManager.s_Data = new LocalizationData();
				LocalizationManager.DeserializeFromFile();
			}
		}

		// Token: 0x06001A53 RID: 6739 RVA: 0x00097D04 File Offset: 0x00095F04
		public static bool DeserializeFromFile()
		{
			string dataFilePath = LocalizationManager.s_Data.DataFilePath;
			string fileText = string.Empty;
			try
			{
				fileText = File.ReadAllText(dataFilePath, Encoding.UTF8);
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
			LocalizationManager.LoadLocalizationData();
			bool flag = LocalizationManager.s_Data.Deserialize(fileText);
			if (LocalizationManager.LocalizationLoadedEvent != null)
			{
				LocalizationManager.LocalizationLoadedEvent(flag);
			}
			return flag;
		}

		// Token: 0x040015E8 RID: 5608
		private static string[] s_LanguageCultures = new string[]
		{
			"en-GB",
			"nl-NL",
			"de-DE",
			"zh-CN"
		};

		// Token: 0x040015E9 RID: 5609
		private static string[] s_TokenBrackets = new string[]
		{
			"[",
			"]"
		};

		// Token: 0x040015EA RID: 5610
		private static string s_VersionNumberToken = "VERSION_NUMBER";

		// Token: 0x040015EB RID: 5611
		private static Dictionary<string, string> s_CustomTokens;

		// Token: 0x040015EC RID: 5612
		private static LocalizationManager.Language s_Language = LocalizationManager.Language.Chinese;

		// Token: 0x040015ED RID: 5613
		private static LocalizationData s_Data = null;

		// Token: 0x02000488 RID: 1160
		public enum Language
		{
			// Token: 0x040015F3 RID: 5619
			English,
			// Token: 0x040015F4 RID: 5620
			Dutch,
			// Token: 0x040015F5 RID: 5621
			German,
			// Token: 0x040015F6 RID: 5622
			Chinese
		}

		// Token: 0x02000489 RID: 1161
		public enum LanguageAbbreviation
		{
			// Token: 0x040015F8 RID: 5624
			en,
			// Token: 0x040015F9 RID: 5625
			nl,
			// Token: 0x040015FA RID: 5626
			de,
			// Token: 0x040015FB RID: 5627
			cn
		}

		// Token: 0x0200048A RID: 1162
		// (Invoke) Token: 0x06001A56 RID: 6742
		public delegate void LocalizationLoadedDelegate(bool success);

		// Token: 0x0200048B RID: 1163
		// (Invoke) Token: 0x06001A5A RID: 6746
		public delegate void LanguageDelegate(LocalizationManager.Language language);

		// Token: 0x0200048C RID: 1164
		// (Invoke) Token: 0x06001A5E RID: 6750
		public delegate void TokenDelegate(string token);
	}
}
