using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

// Token: 0x020003DB RID: 987
internal class SettingsSerializer
{
	// Token: 0x060015B4 RID: 5556 RVA: 0x00084A60 File Offset: 0x00082C60
	public static string Write(object s)
	{
		StringBuilder stringBuilder = new StringBuilder();
		SettingsSerializer.handleGroup(s, string.Empty, stringBuilder);
		return stringBuilder.ToString().Replace("\n", Environment.NewLine);
	}

	// Token: 0x060015B5 RID: 5557 RVA: 0x0000FD66 File Offset: 0x0000DF66
	private static bool implementsInterface(Type t, Type interfaceType)
	{
		return Enumerable.Contains<Type>(t.GetInterfaces(), interfaceType);
	}

	// Token: 0x060015B6 RID: 5558 RVA: 0x00084A94 File Offset: 0x00082C94
	private static void handleGroup(object g, string fullname, StringBuilder sb)
	{
		if (SettingsSerializer.isTransient(g))
		{
			return;
		}
		FieldInfo[] fields = g.GetType().GetFields(20);
		foreach (FieldInfo fieldInfo in fields)
		{
			if (!SettingsSerializer.isTransient(fieldInfo.FieldType))
			{
				if (!SettingsSerializer.isTransient(fieldInfo.GetCustomAttributes(false)))
				{
					object value = fieldInfo.GetValue(g);
					if (value == null)
					{
						throw new NullReferenceException(fullname + fieldInfo.Name + " is null");
					}
					if (SettingsSerializer.implementsInterface(fieldInfo.FieldType, typeof(ISettingsGroup)))
					{
						if (fullname == string.Empty)
						{
							string text = StringUtil.capitalize(fieldInfo.FieldType.Name);
							string s = ' ' + text.Replace("_", " ") + ' ';
							sb.AppendLine();
							sb.AppendLine(StringUtil.justifyCenter(s, 39, '#'));
						}
						SettingsSerializer.handleGroup((ISettingsGroup)value, fullname + fieldInfo.Name + '.', sb);
					}
					if (SettingsSerializer.implementsInterface(fieldInfo.FieldType, typeof(ISettingsValue)))
					{
						ISettingsValue settingsValue = (ISettingsValue)value;
						string text2 = settingsValue.ToString();
						if (text2 != null)
						{
							string text3 = fullname + fieldInfo.Name + " = " + text2;
							sb.Append(text3 + '\n');
						}
					}
				}
			}
		}
	}

	// Token: 0x060015B7 RID: 5559 RVA: 0x0000FD74 File Offset: 0x0000DF74
	private static bool isTransient(object o)
	{
		return SettingsSerializer.isTransient(o.GetType());
	}

	// Token: 0x060015B8 RID: 5560 RVA: 0x0000FD81 File Offset: 0x0000DF81
	private static bool isTransient(Type type)
	{
		return SettingsSerializer.isTransient(Attribute.GetCustomAttributes(type));
	}

	// Token: 0x060015B9 RID: 5561 RVA: 0x0000FD8E File Offset: 0x0000DF8E
	private static bool isTransient(Attribute[] attrs)
	{
		return Enumerable.Any<Attribute>(attrs, (Attribute a) => a.GetType() == typeof(Transient));
	}

	// Token: 0x060015BA RID: 5562 RVA: 0x00084C20 File Offset: 0x00082E20
	public static T Read<T>(string text)
	{
		T t = Activator.CreateInstance<T>();
		SettingsSerializer.ReadInto(text, t);
		return t;
	}

	// Token: 0x060015BB RID: 5563 RVA: 0x00084C44 File Offset: 0x00082E44
	public static object ReadInto(string text, object target)
	{
		text = text.Replace(Environment.NewLine, "\n");
		string[] array = text.Split(new char[]
		{
			'\n'
		});
		foreach (string text2 in array)
		{
			string text3 = text2.Trim();
			if (!(text3 == string.Empty) && !text3.StartsWith("#"))
			{
				string[] array3 = text3.Split(new char[]
				{
					'='
				}, 2);
				if (array3.Length == 2)
				{
					string[] keys = array3[0].Split(new char[]
					{
						'.'
					});
					string value = array3[1];
					try
					{
						SettingsSerializer.inject(target, keys, value);
					}
					catch (Exception s)
					{
						Log.warning(s);
					}
				}
			}
		}
		return target;
	}

	// Token: 0x060015BC RID: 5564 RVA: 0x00084D2C File Offset: 0x00082F2C
	public static Dictionary<string, object> ReadAsTags(string text)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		text = text.Replace(Environment.NewLine, "\n");
		string[] array = text.Split(new char[]
		{
			'\n'
		});
		foreach (string text2 in array)
		{
			string text3 = text2.Trim();
			if (!(text3 == string.Empty) && !text3.StartsWith("#"))
			{
				string[] array3 = text3.Split(new char[]
				{
					'='
				}, 2);
				string text4 = array3[0].Trim().ToLower();
				if (array3.Length == 1)
				{
					dictionary[text4] = true;
				}
				else if (array3.Length == 2)
				{
					string s = array3[1].Trim();
					dictionary[text4] = SettingsSerializer.convertToPrimitiveOrString(s);
				}
			}
		}
		return dictionary;
	}

	// Token: 0x060015BD RID: 5565 RVA: 0x00084E20 File Offset: 0x00083020
	public static object convertToPrimitiveOrString(string s)
	{
		string text = s.ToLower();
		if (text == "true" || text == "false")
		{
			return text == "true";
		}
		try
		{
			return int.Parse(s);
		}
		catch (FormatException)
		{
		}
		catch (OverflowException)
		{
		}
		try
		{
			return float.Parse(s);
		}
		catch (FormatException)
		{
		}
		catch (OverflowException)
		{
		}
		return s;
	}

	// Token: 0x060015BE RID: 5566 RVA: 0x00084EE0 File Offset: 0x000830E0
	private static void inject(object root, string[] keys, string value)
	{
		object obj = root;
		foreach (string text in keys)
		{
			FieldInfo field = SettingsSerializer.getField(obj, text);
			if (field == null)
			{
				return;
			}
			obj = field.GetValue(obj);
			if (obj == null)
			{
				throw new NullReferenceException(string.Concat(new object[]
				{
					"field ",
					text,
					" is null in ",
					root
				}));
			}
		}
		ISettingsValue settingsValue = obj as ISettingsValue;
		if (settingsValue != null)
		{
			settingsValue.load(value.Trim());
		}
	}

	// Token: 0x060015BF RID: 5567 RVA: 0x00084F70 File Offset: 0x00083170
	private static FieldInfo getField(object obj, string name)
	{
		name = name.ToLower().Trim();
		FieldInfo[] fields = obj.GetType().GetFields(20);
		FieldInfo[] array = Enumerable.ToArray<FieldInfo>(Enumerable.Where<FieldInfo>(fields, (FieldInfo f) => f.Name.ToLower() == name));
		if (array.Length == 0)
		{
			return null;
		}
		if (array.Length >= 2)
		{
			throw new InvalidProgramException("More than one field named: " + name);
		}
		return array[0];
	}
}
