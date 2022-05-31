using System;
using System.Collections.Generic;
using JsonFx.Json;

// Token: 0x02000288 RID: 648
public class NewEffectsMessage : Message
{
	// Token: 0x06001231 RID: 4657 RVA: 0x00077A88 File Offset: 0x00075C88
	private static EffectMessage getEffectFromString(string s)
	{
		int num = s.IndexOf('"');
		int num2 = s.IndexOf('"', num + 1);
		if (num < 0 || num2 < 0)
		{
			return null;
		}
		string text = s.Substring(num + 1, num2 - (num + 1));
		string text2 = "EM" + text;
		Type type = Type.GetType(text2);
		if (type == null)
		{
			Log.warning("Missing EffectMessage class: " + text2);
			return null;
		}
		if (!type.IsSubclassOf(typeof(EffectMessage)))
		{
			Log.error(text2 + " is not a subclass of EffectMessage!");
			return null;
		}
		int num3 = s.IndexOf('{', num2);
		int num4 = s.LastIndexOf('}');
		if (num3 < 0 || num4 < 0)
		{
			return null;
		}
		string text3 = s.Substring(num3, num4 + 1 - num3);
		JsonReader jsonReader = new JsonReader();
		EffectMessage effectMessage = jsonReader.Read(text3, type) as EffectMessage;
		if (effectMessage != null)
		{
			effectMessage.type = text;
			effectMessage.setRawText(s);
		}
		else
		{
			Log.warning("EffectMessage not correctly implemented: " + text);
			Log.warning("::" + text3 + "::");
		}
		return effectMessage;
	}

	// Token: 0x06001232 RID: 4658 RVA: 0x00077BB4 File Offset: 0x00075DB4
	public static List<EffectMessage> parseEffects(string msg)
	{
		List<EffectMessage> list = new List<EffectMessage>();
		IServerMessageSplitter serverMessageSplitter = new JsonMessageSplitter(2);
		serverMessageSplitter.feed(msg);
		for (;;)
		{
			string nextMessage = serverMessageSplitter.getNextMessage();
			if (nextMessage == null)
			{
				break;
			}
			EffectMessage effectFromString = NewEffectsMessage.getEffectFromString(nextMessage);
			if (effectFromString != null)
			{
				list.Add(effectFromString);
			}
		}
		return list;
	}
}
