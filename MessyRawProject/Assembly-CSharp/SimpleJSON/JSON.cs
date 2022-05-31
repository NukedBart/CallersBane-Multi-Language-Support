using System;

namespace SimpleJSON
{
	// Token: 0x02000483 RID: 1155
	public static class JSON
	{
		// Token: 0x06001A2A RID: 6698 RVA: 0x00012E34 File Offset: 0x00011034
		public static JSONNode Parse(string aJSON)
		{
			return JSONNode.Parse(aJSON);
		}

		// Token: 0x06001A2B RID: 6699 RVA: 0x00096D34 File Offset: 0x00094F34
		public static JSONNode CreateJSONNodeByTag(JSONNodeType tag)
		{
			switch (tag)
			{
			case JSONNodeType.Array:
				return new JSONArray();
			case JSONNodeType.Object:
				return new JSONObject();
			case JSONNodeType.String:
				return new JSONString(string.Empty);
			case JSONNodeType.Number:
				return new JSONNumber(0.0);
			case JSONNodeType.NullValue:
			case JSONNodeType.None:
				break;
			case JSONNodeType.Boolean:
				return new JSONBool(false);
			default:
				if (tag != JSONNodeType.Custom)
				{
				}
				break;
			}
			return null;
		}

		// Token: 0x06001A2C RID: 6700 RVA: 0x00096DA0 File Offset: 0x00094FA0
		public static JSONNode Copy(JSONNode jsonNode)
		{
			if (jsonNode == null)
			{
				return null;
			}
			JSONNode jsonnode = JSON.CreateJSONNodeByTag(jsonNode.Tag);
			if (jsonnode == null)
			{
				return null;
			}
			JSON.CopyTo(jsonnode, jsonNode);
			return jsonnode;
		}

		// Token: 0x06001A2D RID: 6701 RVA: 0x00096DD8 File Offset: 0x00094FD8
		public static void CopyTo(JSONNode targetJsonNode, JSONNode sourceJsonNode)
		{
			if (sourceJsonNode.IsObject)
			{
				foreach (JSONNode d in sourceJsonNode.Keys)
				{
					string aKey = d;
					JSONNode jsonnode = sourceJsonNode[aKey];
					if (jsonnode.IsArray || jsonnode.IsObject)
					{
						if (targetJsonNode[aKey] == null)
						{
							if (jsonnode.IsArray)
							{
								targetJsonNode[aKey] = new JSONArray();
							}
							if (jsonnode.IsObject)
							{
								targetJsonNode[aKey] = new JSONObject();
							}
						}
						JSON.CopyTo(targetJsonNode[aKey], jsonnode);
					}
					else
					{
						targetJsonNode[aKey] = jsonnode;
					}
				}
				return;
			}
			if (sourceJsonNode.IsArray)
			{
				for (int i = 0; i < sourceJsonNode.Count; i++)
				{
					JSONNode jsonnode2 = sourceJsonNode[i];
					if (jsonnode2.IsArray || jsonnode2.IsObject)
					{
						if (targetJsonNode[i] == null)
						{
							if (jsonnode2.IsArray)
							{
								targetJsonNode[i] = new JSONArray();
							}
							if (jsonnode2.IsObject)
							{
								targetJsonNode[i] = new JSONObject();
							}
						}
						JSON.CopyTo(targetJsonNode[i], jsonnode2);
					}
					else
					{
						targetJsonNode[i] = jsonnode2;
					}
				}
				return;
			}
			targetJsonNode.Value = sourceJsonNode.Value;
		}
	}
}
