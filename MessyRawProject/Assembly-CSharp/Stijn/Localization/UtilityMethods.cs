using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SimpleJSON;
using UnityEngine;

namespace Stijn.Localization
{
	// Token: 0x02000484 RID: 1156
	public static class UtilityMethods
	{
		// Token: 0x06001A2E RID: 6702 RVA: 0x00097158 File Offset: 0x00095358
		public static string[,] ParseCSV(string filename)
		{
			string fileText = string.Empty;
			try
			{
				fileText = File.ReadAllText(filename, Encoding.UTF8);
			}
			catch (Exception ex)
			{
				Debug.LogError(ex.Message);
				return null;
			}
			return UtilityMethods.ParseCSVRaw(fileText);
		}

		// Token: 0x06001A2F RID: 6703 RVA: 0x000971A0 File Offset: 0x000953A0
		public static string[,] ParseCSVRaw(string fileText)
		{
			string[,] array = new string[0, 0];
			List<string> list = new List<string>(fileText.Split(new string[]
			{
				UtilityMethods.ROW_SEPARATOR
			}, 0));
			list.RemoveAll((string rowName) => string.IsNullOrEmpty(rowName));
			for (int i = 0; i < list.Count; i++)
			{
				string[] array2 = list[i].Split(new string[]
				{
					UtilityMethods.COLUMN_SEPARATOR
				}, 0);
				if (array.Length == 0)
				{
					array = new string[array2.Length, list.Count];
				}
				for (int j = 0; j < array2.Length; j++)
				{
					if (j >= array.GetLength(0))
					{
						Debug.LogWarning("Row consists of more columns than the first row of the table! Source: " + list[i]);
						return null;
					}
					array[j, i] = array2[j];
				}
			}
			return array;
		}

		// Token: 0x06001A30 RID: 6704 RVA: 0x00097280 File Offset: 0x00095480
		public static JSONNode DictionaryToJSON(Dictionary<string, string> dictionary)
		{
			JSONNode jsonnode = new JSONObject();
			foreach (KeyValuePair<string, string> keyValuePair in dictionary)
			{
				jsonnode.Add(keyValuePair.Key, new JSONString(keyValuePair.Value));
			}
			return jsonnode;
		}

		// Token: 0x06001A31 RID: 6705 RVA: 0x000972E8 File Offset: 0x000954E8
		public static bool CheckIntInJSONObject(JSONNode jsonObject, string intName, int? minValue, int? maxValue, int? disallowedValue, string errorPrefix, string errorSuffix, ref StringBuilder errorStringBuilder)
		{
			bool result = true;
			if (jsonObject[intName] == null)
			{
				errorStringBuilder.AppendLine(string.Concat(new string[]
				{
					errorPrefix,
					" '",
					intName,
					"' that doesn't exist. ",
					errorSuffix
				}));
				result = false;
			}
			else if (!jsonObject[intName].IsNumber)
			{
				errorStringBuilder.AppendLine(string.Concat(new string[]
				{
					errorPrefix,
					" '",
					intName,
					"' that isn't a valid number. ",
					errorSuffix
				}));
				result = false;
			}
			else
			{
				if (minValue != null)
				{
					float asFloat = jsonObject[intName].AsFloat;
					int? num = minValue;
					float? num2 = (num != null) ? new float?((float)num.GetValueOrDefault()) : default(float?);
					if (asFloat < num2.GetValueOrDefault() & num2 != null)
					{
						errorStringBuilder.AppendLine(string.Concat(new string[]
						{
							errorPrefix,
							" '",
							intName,
							"' that is out of range. ",
							errorSuffix
						}));
						result = false;
					}
				}
				if (maxValue != null)
				{
					float asFloat2 = jsonObject[intName].AsFloat;
					int? num = maxValue;
					float? num2 = (num != null) ? new float?((float)num.GetValueOrDefault()) : default(float?);
					if (asFloat2 > num2.GetValueOrDefault() & num2 != null)
					{
						errorStringBuilder.AppendLine(string.Concat(new string[]
						{
							errorPrefix,
							" '",
							intName,
							"' that is out of range. ",
							errorSuffix
						}));
						result = false;
					}
				}
				if (disallowedValue != null)
				{
					float asFloat3 = jsonObject[intName].AsFloat;
					int? num = disallowedValue;
					float? num2 = (num != null) ? new float?((float)num.GetValueOrDefault()) : default(float?);
					if (asFloat3 == num2.GetValueOrDefault() & num2 != null)
					{
						errorStringBuilder.AppendLine(string.Concat(new string[]
						{
							errorPrefix,
							" '",
							intName,
							"' that is an invalid number. ",
							errorSuffix
						}));
						result = false;
					}
				}
			}
			return result;
		}

		// Token: 0x06001A32 RID: 6706 RVA: 0x00097508 File Offset: 0x00095708
		public static bool CheckVectorInJSONObject(JSONNode jsonObject, string vectorName, int? vectorSize, float? minValue, float? maxValue, float? disallowedValue, string errorPrefix, string errorSuffix, ref StringBuilder errorStringBuilder)
		{
			bool result = true;
			if (!jsonObject[vectorName].IsArray)
			{
				errorStringBuilder.AppendLine(string.Concat(new string[]
				{
					errorPrefix,
					" '",
					vectorName,
					"' isn't a valid array. ",
					errorSuffix
				}));
				result = false;
			}
			else
			{
				JSONArray asArray = jsonObject[vectorName].AsArray;
				if (vectorSize != null)
				{
					int count = asArray.Count;
					int? num = vectorSize;
					if (!(count == num.GetValueOrDefault() & num != null))
					{
						errorStringBuilder.AppendLine(string.Concat(new object[]
						{
							errorPrefix,
							" '",
							vectorName,
							"' array isn't exactly ",
							vectorSize,
							" numbers. ",
							errorSuffix
						}));
						return false;
					}
				}
				for (int i = 0; i < asArray.Count; i++)
				{
					if (!asArray[i].IsNumber)
					{
						errorStringBuilder.AppendLine(string.Concat(new object[]
						{
							errorPrefix,
							" '",
							vectorName,
							"' array has a non-number in slot ",
							i,
							". ",
							errorSuffix
						}));
						result = false;
					}
					else
					{
						if (minValue != null)
						{
							float asFloat = asArray[i].AsFloat;
							float? num2 = minValue;
							if (asFloat < num2.GetValueOrDefault() & num2 != null)
							{
								errorStringBuilder.AppendLine(string.Concat(new object[]
								{
									errorPrefix,
									" '",
									vectorName,
									"' array has out of range number in slot ",
									i,
									". ",
									errorSuffix
								}));
								result = false;
							}
						}
						if (maxValue != null)
						{
							float asFloat2 = asArray[i].AsFloat;
							float? num2 = maxValue;
							if (asFloat2 > num2.GetValueOrDefault() & num2 != null)
							{
								errorStringBuilder.AppendLine(string.Concat(new object[]
								{
									errorPrefix,
									" '",
									vectorName,
									"' array has out of range number in slot ",
									i,
									". ",
									errorSuffix
								}));
								result = false;
							}
						}
						if (disallowedValue != null)
						{
							float asFloat3 = asArray[i].AsFloat;
							float? num2 = disallowedValue;
							if (asFloat3 == num2.GetValueOrDefault() & num2 != null)
							{
								errorStringBuilder.AppendLine(string.Concat(new object[]
								{
									errorPrefix,
									" '",
									vectorName,
									"' array has invalid number in slot ",
									i,
									". ",
									errorSuffix
								}));
								result = false;
							}
						}
					}
				}
			}
			return result;
		}

		// Token: 0x040015E1 RID: 5601
		public static string ROW_SEPARATOR = ";|;";

		// Token: 0x040015E2 RID: 5602
		public static string COLUMN_SEPARATOR = ";-;";
	}
}
