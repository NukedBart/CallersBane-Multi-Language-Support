using System;
using System.Collections.Generic;
using System.Reflection;
using JsonFx.Json;
using JsonFx.Model;
using JsonFx.Serialization;
using JsonFx.Serialization.Filters;
using JsonFx.Serialization.Resolvers;

// Token: 0x0200031F RID: 799
public static class MessageFactory
{
	// Token: 0x06001343 RID: 4931 RVA: 0x00078768 File Offset: 0x00076968
	static MessageFactory()
	{
		PocoResolverStrategy pocoResolverStrategy = new PocoResolverStrategy();
		MessageFactory._jsonEncoder = new JsonWriter(new DataWriterSettings(new CallbackResolverStrategy
		{
			IsPropertyIgnored = new CallbackResolverStrategy.PropertyIgnoredDelegate(pocoResolverStrategy.IsPropertyIgnored),
			IsFieldIgnored = new CallbackResolverStrategy.FieldIgnoredDelegate(pocoResolverStrategy.IsFieldIgnored),
			GetName = new CallbackResolverStrategy.GetNameDelegate(pocoResolverStrategy.GetName),
			SortMembers = new CallbackResolverStrategy.SortMembersDelegate(pocoResolverStrategy.SortMembers),
			GetValueIgnored = new CallbackResolverStrategy.GetValueIgnoredDelegate(MessageFactory.isValueIgnored)
		}, new IDataFilter<ModelTokenType>[0])
		{
			PrettyPrint = true
		});
	}

	// Token: 0x06001344 RID: 4932 RVA: 0x00078808 File Offset: 0x00076A08
	public static string getMessageName(string json)
	{
		if (json == null)
		{
			return null;
		}
		int num = json.LastIndexOf("\"msg\"");
		if (num < 0)
		{
			return null;
		}
		int num2 = json.IndexOf('"', num + "\"msg\"".Length) + 1;
		int num3 = json.IndexOf('"', num2);
		if (num2 <= 0 || num3 < 0)
		{
			return null;
		}
		return json.Substring(num2, num3 - num2);
	}

	// Token: 0x06001345 RID: 4933 RVA: 0x0000E5B2 File Offset: 0x0000C7B2
	public static Message create(string jsonData)
	{
		return MessageFactory.create(MessageFactory.getMessageName(jsonData), jsonData);
	}

	// Token: 0x06001346 RID: 4934 RVA: 0x00078870 File Offset: 0x00076A70
	public static Message create(string msgName, string jsonData)
	{
		string text = msgName + "Message";
		Type type = Type.GetType(text);
		if (type == null && MessageFactory._whineMessageSet.Add(text))
		{
			Log.error("Missing Message class: " + text);
		}
		if (type != null && !type.IsSubclassOf(typeof(Message)) && MessageFactory._whineMessageSet.Add(text))
		{
			Log.error(text + " is not a subclass of Message!");
		}
		if (type == null)
		{
			type = typeof(Message);
		}
		JsonReader jsonReader = new JsonReader();
		Message message = null;
		try
		{
			message = (jsonReader.Read(jsonData, type) as Message);
		}
		catch (Exception ex)
		{
			Log.warning(string.Empty + text + ": " + ex.ToString());
		}
		if (message == null)
		{
			Log.error("Failed parsing " + text + " :: Do your class have an empty (or no) constructor?\nData: " + jsonData);
			return null;
		}
		message.setRawText(jsonData);
		return message;
	}

	// Token: 0x06001347 RID: 4935 RVA: 0x0000E5C0 File Offset: 0x0000C7C0
	public static string toString(Message msg)
	{
		return MessageFactory._jsonEncoder.Write(msg);
	}

	// Token: 0x06001348 RID: 4936 RVA: 0x00078978 File Offset: 0x00076B78
	private static ValueIgnoredDelegate isValueIgnored(MemberInfo memberInfo)
	{
		return delegate(object instance, object memberValue)
		{
			object[] customAttributes = memberInfo.GetCustomAttributes(false);
			if (customAttributes != null)
			{
				foreach (object obj in customAttributes)
				{
					if (obj.GetType() == typeof(ServerToClient))
					{
						return true;
					}
					if (obj.GetType() == typeof(Transient))
					{
						return true;
					}
				}
			}
			return memberValue == null;
		};
	}

	// Token: 0x0400103B RID: 4155
	private static HashSet<string> _whineMessageSet = new HashSet<string>();

	// Token: 0x0400103C RID: 4156
	private static JsonWriter _jsonEncoder;
}
