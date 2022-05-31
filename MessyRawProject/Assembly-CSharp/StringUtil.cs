using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

// Token: 0x02000455 RID: 1109
public static class StringUtil
{
	// Token: 0x060018B2 RID: 6322 RVA: 0x00011F4F File Offset: 0x0001014F
	public static string capitalize(string s)
	{
		if (s.Length <= 1)
		{
			return s.ToUpper();
		}
		return char.ToUpper(s.get_Chars(0)) + s.Substring(1).ToLower();
	}

	// Token: 0x060018B3 RID: 6323 RVA: 0x00011F86 File Offset: 0x00010186
	public static int countLines(string s)
	{
		if (string.IsNullOrEmpty(s))
		{
			return 0;
		}
		return 1 + CollectionUtil.count<char>(s, '\n');
	}

	// Token: 0x060018B4 RID: 6324 RVA: 0x00092DE8 File Offset: 0x00090FE8
	public static string[] toStringArray<T>(IEnumerable<T> objects)
	{
		List<string> list = new List<string>();
		foreach (T t in objects)
		{
			list.Add(t.ToString());
		}
		return list.ToArray();
	}

	// Token: 0x060018B5 RID: 6325 RVA: 0x00092E54 File Offset: 0x00091054
	public static string replicate(string s, int count)
	{
		StringBuilder stringBuilder = new StringBuilder(s.Length * count);
		for (int i = 0; i < count; i++)
		{
			stringBuilder.Append(s);
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060018B6 RID: 6326 RVA: 0x00092E90 File Offset: 0x00091090
	public static string keepCharacters(string s, Predicate<char> predicate)
	{
		return new string(Enumerable.ToArray<char>(Enumerable.Where<char>(s, (char ch) => predicate.Invoke(ch))));
	}

	// Token: 0x060018B7 RID: 6327 RVA: 0x00092EC8 File Offset: 0x000910C8
	public static Group removeWords(string s, int numWords)
	{
		if (numWords < 0)
		{
			throw new ArgumentException("numWords can't be negative: " + numWords);
		}
		Match match = Regex.Match(s, "[\\s]*(?:[\\S]*[\\s]){" + numWords.ToString() + "}(.*)");
		if (match.Groups.Count != 2)
		{
			return null;
		}
		return match.Groups[1];
	}

	// Token: 0x060018B8 RID: 6328 RVA: 0x00092F30 File Offset: 0x00091130
	public static string wordWrapped(string s, int maxCharacters)
	{
		string[] array = s.Split(new char[]
		{
			' '
		});
		string text = string.Empty;
		int num = 0;
		foreach (string text2 in array)
		{
			if (num + text2.Length + 1 <= maxCharacters)
			{
				text = text + " " + text2;
				num += text2.Length + 1;
			}
			else
			{
				text = text + "\n" + text2;
				num = text2.Length;
			}
		}
		return text.Remove(0, 1);
	}

	// Token: 0x060018B9 RID: 6329 RVA: 0x00011F9F File Offset: 0x0001019F
	public static string wordWrappedIgnoreTags(string s, int maxLength)
	{
		return StringUtil.wordWrappedIgnoreTags(s, maxLength, new char[]
		{
			' '
		});
	}

	// Token: 0x060018BA RID: 6330 RVA: 0x00092FC4 File Offset: 0x000911C4
	public static string wordWrappedIgnoreTags(string s, int maxLength, char[] splitChars)
	{
		StringBuilder stringBuilder = new StringBuilder(s.Length + 10);
		int num = 0;
		int num2 = 0;
		int num3 = -1;
		bool flag = false;
		for (int i = 0; i < s.Length; i++)
		{
			char c = s.get_Chars(i);
			stringBuilder.Append(c);
			if (c == '\n')
			{
				num2 = (num = 0);
				num3 = -1;
			}
			else
			{
				if (c == '<')
				{
					flag = true;
				}
				if (c == '>')
				{
					flag = false;
				}
				else if (!flag)
				{
					num2++;
					num++;
					if (StringUtil.contains<char>(splitChars, c) || i == s.Length - 1)
					{
						if (num > maxLength && num3 >= 0)
						{
							num = num2;
							stringBuilder.Insert(num3, '\n');
						}
						num3 = stringBuilder.Length;
						num2 = 0;
					}
				}
			}
		}
		return stringBuilder.ToString();
	}

	// Token: 0x060018BB RID: 6331 RVA: 0x000930A4 File Offset: 0x000912A4
	private static bool contains<T>(T[] collection, T e)
	{
		foreach (T t in collection)
		{
			if (t.Equals(e))
			{
				return true;
			}
		}
		return false;
	}

	// Token: 0x060018BC RID: 6332 RVA: 0x000930EC File Offset: 0x000912EC
	public static string coordinate(string conjugate, List<string> items)
	{
		if (items == null || items.Count == 0)
		{
			return string.Empty;
		}
		if (items.Count == 1)
		{
			return items[0];
		}
		string text = items[0];
		for (int i = 1; i < items.Count - 1; i++)
		{
			text = text + ", " + items[i];
		}
		return string.Concat(new string[]
		{
			text,
			" ",
			conjugate,
			" ",
			items[items.Count - 1]
		});
	}

	// Token: 0x060018BD RID: 6333 RVA: 0x0009318C File Offset: 0x0009138C
	public static int Parse(string s, int defaultValue)
	{
		int result;
		if (int.TryParse(s, ref result))
		{
			return result;
		}
		return defaultValue;
	}

	// Token: 0x060018BE RID: 6334 RVA: 0x000931AC File Offset: 0x000913AC
	public static float Parse(string s, float defaultValue)
	{
		float result;
		if (float.TryParse(s, ref result))
		{
			return result;
		}
		return defaultValue;
	}

	// Token: 0x060018BF RID: 6335 RVA: 0x000931CC File Offset: 0x000913CC
	public static double Parse(string s, double defaultValue)
	{
		double result;
		if (double.TryParse(s, ref result))
		{
			return result;
		}
		return defaultValue;
	}

	// Token: 0x060018C0 RID: 6336 RVA: 0x000931EC File Offset: 0x000913EC
	public static bool Parse(string s, bool defaultValue)
	{
		s = s.Trim().ToLower();
		if (s == "false" || s == "f")
		{
			return false;
		}
		if (s == "true" || s == "t")
		{
			return true;
		}
		int num = 0;
		if (int.TryParse(s, ref num))
		{
			return num != 0;
		}
		return defaultValue;
	}

	// Token: 0x060018C1 RID: 6337 RVA: 0x00011FB3 File Offset: 0x000101B3
	public static string justifyCenter(string s, int length)
	{
		return StringUtil.justifyCenter(s, length, ' ');
	}

	// Token: 0x060018C2 RID: 6338 RVA: 0x00093264 File Offset: 0x00091464
	public static string justifyCenter(string s, int length, char fill)
	{
		int length2 = s.Length;
		if (length2 >= length)
		{
			return s;
		}
		int num = (length - length2) / 2;
		int count = length - num - length2;
		string s2 = new string(fill, 1);
		return StringUtil.replicate(s2, num) + s + StringUtil.replicate(s2, count);
	}

	// Token: 0x060018C3 RID: 6339 RVA: 0x00011FBE File Offset: 0x000101BE
	public static bool IsValidEmail(string email)
	{
		return new Regex("^[\\w\\d!#$%&'*+/=?^_`{|}~-]+(?:\\.[\\w\\d!#$%&'*+/=?^_`{|}~-]+)*@(?:[\\w\\d](?:[\\w\\d-]*[\\w\\d])?\\.)+[\\w\\d](?:[\\w\\d-]*[\\w\\d])?$").IsMatch(email);
	}

	// Token: 0x060018C4 RID: 6340 RVA: 0x000932AC File Offset: 0x000914AC
	public static List<string> getStringsInsideDelimiters(string s, string begin, string end)
	{
		List<string> list = new List<string>();
		int length = begin.Length;
		int length2 = end.Length;
		int num = 0;
		for (;;)
		{
			int num2 = s.IndexOf(begin, num);
			int num3 = s.IndexOf(end, num);
			if (num2 < 0 || num3 < 0)
			{
				break;
			}
			num2 += length;
			num = num3 + length2;
			string text = s.Substring(num2, num3 - num2).Replace('\n', ' ');
			list.Add(text);
		}
		return list;
	}
}
