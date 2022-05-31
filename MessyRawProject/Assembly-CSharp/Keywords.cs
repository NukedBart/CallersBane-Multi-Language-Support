using System;
using System.Collections.Generic;

// Token: 0x0200010A RID: 266
public class Keywords
{
	// Token: 0x06000882 RID: 2178 RVA: 0x00044A40 File Offset: 0x00042C40
	public static List<KeywordDescription> find(string s)
	{
		List<KeywordDescription> list = new List<KeywordDescription>();
		foreach (MappedString mappedString in Keywords._createKeywords(s, "<", ">"))
		{
			list.Add(KeywordDescription.fromCardReference(mappedString.key, mappedString.value));
		}
		foreach (MappedString mappedString2 in Keywords._createKeywords(s, "[", "]"))
		{
			list.Add(KeywordDescription.fromWord(mappedString2.key, mappedString2.value));
		}
		return list;
	}

	// Token: 0x06000883 RID: 2179 RVA: 0x000076F2 File Offset: 0x000058F2
	public static string clearFromTags(string s)
	{
		return s.Replace("[", string.Empty).Replace("]", string.Empty).Replace("<", string.Empty).Replace(">", string.Empty);
	}

	// Token: 0x06000884 RID: 2180 RVA: 0x00044B20 File Offset: 0x00042D20
	private static List<MappedString> _createKeywords(string s, string begin, string end)
	{
		MappedStringManager instance = MappedStringManager.getInstance();
		List<MappedString> list = new List<MappedString>();
		foreach (string text in StringUtil.getStringsInsideDelimiters(s, begin, end))
		{
			MappedString mappedString = instance.get(text);
			if (mappedString == null)
			{
				Log.error("Couldn't find description for: " + text);
			}
			else
			{
				list.Add(mappedString);
			}
		}
		return list;
	}
}
