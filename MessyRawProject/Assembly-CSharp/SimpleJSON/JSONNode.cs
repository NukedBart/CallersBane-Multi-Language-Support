using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace SimpleJSON
{
	// Token: 0x02000471 RID: 1137
	public abstract class JSONNode
	{
		// Token: 0x17000136 RID: 310
		// (get) Token: 0x06001942 RID: 6466
		public abstract JSONNodeType Tag { get; }

		// Token: 0x17000137 RID: 311
		public virtual JSONNode this[int aIndex]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x17000138 RID: 312
		public virtual JSONNode this[string aKey]
		{
			get
			{
				return null;
			}
			set
			{
			}
		}

		// Token: 0x17000139 RID: 313
		// (get) Token: 0x06001947 RID: 6471 RVA: 0x0001260D File Offset: 0x0001080D
		// (set) Token: 0x06001948 RID: 6472 RVA: 0x000028DF File Offset: 0x00000ADF
		public virtual string Value
		{
			get
			{
				return "";
			}
			set
			{
			}
		}

		// Token: 0x1700013A RID: 314
		// (get) Token: 0x06001949 RID: 6473 RVA: 0x000059E4 File Offset: 0x00003BE4
		public virtual int Count
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700013B RID: 315
		// (get) Token: 0x0600194A RID: 6474 RVA: 0x000059E4 File Offset: 0x00003BE4
		public virtual bool IsNumber
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700013C RID: 316
		// (get) Token: 0x0600194B RID: 6475 RVA: 0x000059E4 File Offset: 0x00003BE4
		public virtual bool IsString
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700013D RID: 317
		// (get) Token: 0x0600194C RID: 6476 RVA: 0x000059E4 File Offset: 0x00003BE4
		public virtual bool IsBoolean
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700013E RID: 318
		// (get) Token: 0x0600194D RID: 6477 RVA: 0x000059E4 File Offset: 0x00003BE4
		public virtual bool IsNull
		{
			get
			{
				return false;
			}
		}

		// Token: 0x1700013F RID: 319
		// (get) Token: 0x0600194E RID: 6478 RVA: 0x000059E4 File Offset: 0x00003BE4
		public virtual bool IsArray
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000140 RID: 320
		// (get) Token: 0x0600194F RID: 6479 RVA: 0x000059E4 File Offset: 0x00003BE4
		public virtual bool IsObject
		{
			get
			{
				return false;
			}
		}

		// Token: 0x17000141 RID: 321
		// (get) Token: 0x06001950 RID: 6480 RVA: 0x000059E4 File Offset: 0x00003BE4
		// (set) Token: 0x06001951 RID: 6481 RVA: 0x000028DF File Offset: 0x00000ADF
		public virtual bool Inline
		{
			get
			{
				return false;
			}
			set
			{
			}
		}

		// Token: 0x06001952 RID: 6482 RVA: 0x000028DF File Offset: 0x00000ADF
		public virtual void Add(string aKey, JSONNode aItem)
		{
		}

		// Token: 0x06001953 RID: 6483 RVA: 0x00012614 File Offset: 0x00010814
		public virtual void Add(JSONNode aItem)
		{
			this.Add("", aItem);
		}

		// Token: 0x06001954 RID: 6484 RVA: 0x000038F9 File Offset: 0x00001AF9
		public virtual JSONNode Remove(string aKey)
		{
			return null;
		}

		// Token: 0x06001955 RID: 6485 RVA: 0x000038F9 File Offset: 0x00001AF9
		public virtual JSONNode Remove(int aIndex)
		{
			return null;
		}

		// Token: 0x06001956 RID: 6486 RVA: 0x000059E7 File Offset: 0x00003BE7
		public virtual JSONNode Remove(JSONNode aNode)
		{
			return aNode;
		}

		// Token: 0x06001957 RID: 6487 RVA: 0x000028DF File Offset: 0x00000ADF
		public virtual void Clear()
		{
		}

		// Token: 0x17000142 RID: 322
		// (get) Token: 0x06001958 RID: 6488 RVA: 0x00012622 File Offset: 0x00010822
		public virtual IEnumerable<JSONNode> Children
		{
			get
			{
				yield break;
			}
		}

		// Token: 0x17000143 RID: 323
		// (get) Token: 0x06001959 RID: 6489 RVA: 0x0001262B File Offset: 0x0001082B
		public IEnumerable<JSONNode> DeepChildren
		{
			get
			{
				foreach (JSONNode jsonnode in this.Children)
				{
					foreach (JSONNode jsonnode2 in jsonnode.DeepChildren)
					{
						yield return jsonnode2;
					}
					IEnumerator<JSONNode> enumerator2 = null;
				}
				IEnumerator<JSONNode> enumerator = null;
				yield break;
				yield break;
			}
		}

		// Token: 0x0600195A RID: 6490 RVA: 0x00095BC8 File Offset: 0x00093DC8
		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.WriteToStringBuilder(stringBuilder, 0, 0, JSONTextMode.Compact);
			return stringBuilder.ToString();
		}

		// Token: 0x0600195B RID: 6491 RVA: 0x00095BEC File Offset: 0x00093DEC
		public virtual string ToString(int aIndent)
		{
			StringBuilder stringBuilder = new StringBuilder();
			this.WriteToStringBuilder(stringBuilder, 0, aIndent, JSONTextMode.Indent);
			return stringBuilder.ToString();
		}

		// Token: 0x0600195C RID: 6492
		internal abstract void WriteToStringBuilder(StringBuilder aSB, int aIndent, int aIndentInc, JSONTextMode aMode);

		// Token: 0x0600195D RID: 6493
		public abstract JSONNode.Enumerator GetEnumerator();

		// Token: 0x17000144 RID: 324
		// (get) Token: 0x0600195E RID: 6494 RVA: 0x0001263B File Offset: 0x0001083B
		public IEnumerable<KeyValuePair<string, JSONNode>> Linq
		{
			get
			{
				return new JSONNode.LinqEnumerator(this);
			}
		}

		// Token: 0x17000145 RID: 325
		// (get) Token: 0x0600195F RID: 6495 RVA: 0x00012643 File Offset: 0x00010843
		public JSONNode.KeyEnumerator Keys
		{
			get
			{
				return new JSONNode.KeyEnumerator(this.GetEnumerator());
			}
		}

		// Token: 0x17000146 RID: 326
		// (get) Token: 0x06001960 RID: 6496 RVA: 0x00012650 File Offset: 0x00010850
		public JSONNode.ValueEnumerator Values
		{
			get
			{
				return new JSONNode.ValueEnumerator(this.GetEnumerator());
			}
		}

		// Token: 0x17000147 RID: 327
		// (get) Token: 0x06001961 RID: 6497 RVA: 0x00095C10 File Offset: 0x00093E10
		// (set) Token: 0x06001962 RID: 6498 RVA: 0x0001265D File Offset: 0x0001085D
		public virtual double AsDouble
		{
			get
			{
				double result = 0.0;
				if (double.TryParse(this.Value, ref result))
				{
					return result;
				}
				return 0.0;
			}
			set
			{
				this.Value = value.ToString();
			}
		}

		// Token: 0x17000148 RID: 328
		// (get) Token: 0x06001963 RID: 6499 RVA: 0x0001266C File Offset: 0x0001086C
		// (set) Token: 0x06001964 RID: 6500 RVA: 0x00012675 File Offset: 0x00010875
		public virtual int AsInt
		{
			get
			{
				return (int)this.AsDouble;
			}
			set
			{
				this.AsDouble = (double)value;
			}
		}

		// Token: 0x17000149 RID: 329
		// (get) Token: 0x06001965 RID: 6501 RVA: 0x0001267F File Offset: 0x0001087F
		// (set) Token: 0x06001966 RID: 6502 RVA: 0x00012675 File Offset: 0x00010875
		public virtual float AsFloat
		{
			get
			{
				return (float)this.AsDouble;
			}
			set
			{
				this.AsDouble = (double)value;
			}
		}

		// Token: 0x1700014A RID: 330
		// (get) Token: 0x06001967 RID: 6503 RVA: 0x00095C44 File Offset: 0x00093E44
		// (set) Token: 0x06001968 RID: 6504 RVA: 0x00012688 File Offset: 0x00010888
		public virtual bool AsBool
		{
			get
			{
				bool result = false;
				if (bool.TryParse(this.Value, ref result))
				{
					return result;
				}
				return !string.IsNullOrEmpty(this.Value);
			}
			set
			{
				this.Value = (value ? "true" : "false");
			}
		}

		// Token: 0x1700014B RID: 331
		// (get) Token: 0x06001969 RID: 6505 RVA: 0x0001269F File Offset: 0x0001089F
		public virtual JSONArray AsArray
		{
			get
			{
				return this as JSONArray;
			}
		}

		// Token: 0x1700014C RID: 332
		// (get) Token: 0x0600196A RID: 6506 RVA: 0x000126A7 File Offset: 0x000108A7
		public virtual JSONObject AsObject
		{
			get
			{
				return this as JSONObject;
			}
		}

		// Token: 0x0600196B RID: 6507 RVA: 0x000126AF File Offset: 0x000108AF
		public static implicit operator JSONNode(string s)
		{
			return new JSONString(s);
		}

		// Token: 0x0600196C RID: 6508 RVA: 0x000126B7 File Offset: 0x000108B7
		public static implicit operator string(JSONNode d)
		{
			if (!(d == null))
			{
				return d.Value;
			}
			return null;
		}

		// Token: 0x0600196D RID: 6509 RVA: 0x000126CA File Offset: 0x000108CA
		public static implicit operator JSONNode(double n)
		{
			return new JSONNumber(n);
		}

		// Token: 0x0600196E RID: 6510 RVA: 0x000126D2 File Offset: 0x000108D2
		public static implicit operator double(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsDouble;
			}
			return 0.0;
		}

		// Token: 0x0600196F RID: 6511 RVA: 0x000126ED File Offset: 0x000108ED
		public static implicit operator JSONNode(float n)
		{
			return new JSONNumber((double)n);
		}

		// Token: 0x06001970 RID: 6512 RVA: 0x000126F6 File Offset: 0x000108F6
		public static implicit operator float(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsFloat;
			}
			return 0f;
		}

		// Token: 0x06001971 RID: 6513 RVA: 0x000126ED File Offset: 0x000108ED
		public static implicit operator JSONNode(int n)
		{
			return new JSONNumber((double)n);
		}

		// Token: 0x06001972 RID: 6514 RVA: 0x0001270D File Offset: 0x0001090D
		public static implicit operator int(JSONNode d)
		{
			if (!(d == null))
			{
				return d.AsInt;
			}
			return 0;
		}

		// Token: 0x06001973 RID: 6515 RVA: 0x00012720 File Offset: 0x00010920
		public static implicit operator JSONNode(bool b)
		{
			return new JSONBool(b);
		}

		// Token: 0x06001974 RID: 6516 RVA: 0x00012728 File Offset: 0x00010928
		public static implicit operator bool(JSONNode d)
		{
			return !(d == null) && d.AsBool;
		}

		// Token: 0x06001975 RID: 6517 RVA: 0x0001273B File Offset: 0x0001093B
		public static implicit operator JSONNode(KeyValuePair<string, JSONNode> aKeyValue)
		{
			return aKeyValue.Value;
		}

		// Token: 0x06001976 RID: 6518 RVA: 0x00095C74 File Offset: 0x00093E74
		public static bool operator ==(JSONNode a, object b)
		{
			if (a == b)
			{
				return true;
			}
			bool flag = a is JSONNull || a == null || a is JSONLazyCreator;
			bool flag2 = b is JSONNull || b == null || b is JSONLazyCreator;
			return (flag && flag2) || (!flag && a.Equals(b));
		}

		// Token: 0x06001977 RID: 6519 RVA: 0x00012744 File Offset: 0x00010944
		public static bool operator !=(JSONNode a, object b)
		{
			return !(a == b);
		}

		// Token: 0x06001978 RID: 6520 RVA: 0x00012750 File Offset: 0x00010950
		public override bool Equals(object obj)
		{
			return this == obj;
		}

		// Token: 0x06001979 RID: 6521 RVA: 0x00012756 File Offset: 0x00010956
		public override int GetHashCode()
		{
			return base.GetHashCode();
		}

		// Token: 0x1700014D RID: 333
		// (get) Token: 0x0600197A RID: 6522 RVA: 0x0001275E File Offset: 0x0001095E
		internal static StringBuilder EscapeBuilder
		{
			get
			{
				if (JSONNode.m_EscapeBuilder == null)
				{
					JSONNode.m_EscapeBuilder = new StringBuilder();
				}
				return JSONNode.m_EscapeBuilder;
			}
		}

		// Token: 0x0600197B RID: 6523 RVA: 0x00095CCC File Offset: 0x00093ECC
		internal static string Escape(string aText)
		{
			StringBuilder escapeBuilder = JSONNode.EscapeBuilder;
			escapeBuilder.Length = 0;
			if (escapeBuilder.Capacity < aText.Length + aText.Length / 10)
			{
				escapeBuilder.Capacity = aText.Length + aText.Length / 10;
			}
			int i = 0;
			while (i < aText.Length)
			{
				char c = aText.get_Chars(i);
				switch (c)
				{
				case '\b':
					escapeBuilder.Append("\\b");
					break;
				case '\t':
					escapeBuilder.Append("\\t");
					break;
				case '\n':
					escapeBuilder.Append("\\n");
					break;
				case '\v':
					goto IL_E2;
				case '\f':
					escapeBuilder.Append("\\f");
					break;
				case '\r':
					escapeBuilder.Append("\\r");
					break;
				default:
					if (c != '"')
					{
						if (c != '\\')
						{
							goto IL_E2;
						}
						escapeBuilder.Append("\\\\");
					}
					else
					{
						escapeBuilder.Append("\\\"");
					}
					break;
				}
				IL_121:
				i++;
				continue;
				IL_E2:
				if (c < ' ' || (JSONNode.forceASCII && c > '\u007f'))
				{
					ushort num = (ushort)c;
					escapeBuilder.Append("\\u").Append(num.ToString("X4"));
					goto IL_121;
				}
				escapeBuilder.Append(c);
				goto IL_121;
			}
			string result = escapeBuilder.ToString();
			escapeBuilder.Length = 0;
			return result;
		}

		// Token: 0x0600197C RID: 6524 RVA: 0x00095E1C File Offset: 0x0009401C
		private static void ParseElement(JSONNode ctx, string token, string tokenName, bool quoted)
		{
			if (quoted)
			{
				ctx.Add(tokenName, token);
				return;
			}
			string text = token.ToLower();
			if (text == "false" || text == "true")
			{
				ctx.Add(tokenName, text == "true");
				return;
			}
			if (text == "null")
			{
				ctx.Add(tokenName, null);
				return;
			}
			double n;
			if (double.TryParse(token, ref n))
			{
				ctx.Add(tokenName, n);
				return;
			}
			ctx.Add(tokenName, token);
		}

		// Token: 0x0600197D RID: 6525 RVA: 0x00095EB0 File Offset: 0x000940B0
		public static JSONNode Parse(string aJSON)
		{
			Stack<JSONNode> stack = new Stack<JSONNode>();
			JSONNode jsonnode = null;
			int i = 0;
			StringBuilder stringBuilder = new StringBuilder();
			string text = "";
			bool flag = false;
			bool flag2 = false;
			while (i < aJSON.Length)
			{
				char c = aJSON.get_Chars(i);
				if (c <= ',')
				{
					if (c <= ' ')
					{
						switch (c)
						{
						case '\t':
							break;
						case '\n':
						case '\r':
							goto IL_33E;
						case '\v':
						case '\f':
							goto IL_330;
						default:
							if (c != ' ')
							{
								goto IL_330;
							}
							break;
						}
						if (flag)
						{
							stringBuilder.Append(aJSON.get_Chars(i));
						}
					}
					else if (c != '"')
					{
						if (c != ',')
						{
							goto IL_330;
						}
						if (flag)
						{
							stringBuilder.Append(aJSON.get_Chars(i));
						}
						else
						{
							if (stringBuilder.Length > 0 || flag2)
							{
								JSONNode.ParseElement(jsonnode, stringBuilder.ToString(), text, flag2);
							}
							text = "";
							stringBuilder.Length = 0;
							flag2 = false;
						}
					}
					else
					{
						flag = !flag;
						flag2 = (flag2 || flag);
					}
				}
				else
				{
					if (c <= ']')
					{
						if (c != ':')
						{
							switch (c)
							{
							case '[':
								if (flag)
								{
									stringBuilder.Append(aJSON.get_Chars(i));
									goto IL_33E;
								}
								stack.Push(new JSONArray());
								if (jsonnode != null)
								{
									jsonnode.Add(text, stack.Peek());
								}
								text = "";
								stringBuilder.Length = 0;
								jsonnode = stack.Peek();
								goto IL_33E;
							case '\\':
								i++;
								if (flag)
								{
									char c2 = aJSON.get_Chars(i);
									if (c2 <= 'f')
									{
										if (c2 == 'b')
										{
											stringBuilder.Append('\b');
											goto IL_33E;
										}
										if (c2 == 'f')
										{
											stringBuilder.Append('\f');
											goto IL_33E;
										}
									}
									else
									{
										if (c2 == 'n')
										{
											stringBuilder.Append('\n');
											goto IL_33E;
										}
										switch (c2)
										{
										case 'r':
											stringBuilder.Append('\r');
											goto IL_33E;
										case 't':
											stringBuilder.Append('\t');
											goto IL_33E;
										case 'u':
										{
											string text2 = aJSON.Substring(i + 1, 4);
											stringBuilder.Append((char)int.Parse(text2, 512));
											i += 4;
											goto IL_33E;
										}
										}
									}
									stringBuilder.Append(c2);
									goto IL_33E;
								}
								goto IL_33E;
							case ']':
								break;
							default:
								goto IL_330;
							}
						}
						else
						{
							if (flag)
							{
								stringBuilder.Append(aJSON.get_Chars(i));
								goto IL_33E;
							}
							text = stringBuilder.ToString();
							stringBuilder.Length = 0;
							flag2 = false;
							goto IL_33E;
						}
					}
					else if (c != '{')
					{
						if (c != '}')
						{
							goto IL_330;
						}
					}
					else
					{
						if (flag)
						{
							stringBuilder.Append(aJSON.get_Chars(i));
							goto IL_33E;
						}
						stack.Push(new JSONObject());
						if (jsonnode != null)
						{
							jsonnode.Add(text, stack.Peek());
						}
						text = "";
						stringBuilder.Length = 0;
						jsonnode = stack.Peek();
						goto IL_33E;
					}
					if (flag)
					{
						stringBuilder.Append(aJSON.get_Chars(i));
					}
					else
					{
						if (stack.Count == 0)
						{
							throw new Exception("JSON Parse: Too many closing brackets");
						}
						stack.Pop();
						if (stringBuilder.Length > 0 || flag2)
						{
							JSONNode.ParseElement(jsonnode, stringBuilder.ToString(), text, flag2);
							flag2 = false;
						}
						text = "";
						stringBuilder.Length = 0;
						if (stack.Count > 0)
						{
							jsonnode = stack.Peek();
						}
					}
				}
				IL_33E:
				i++;
				continue;
				IL_330:
				stringBuilder.Append(aJSON.get_Chars(i));
				goto IL_33E;
			}
			if (flag)
			{
				throw new Exception("JSON Parse: Quotation marks seems to be messed up.");
			}
			return jsonnode;
		}

		// Token: 0x040015B5 RID: 5557
		public static bool forceASCII;

		// Token: 0x040015B6 RID: 5558
		[ThreadStatic]
		private static StringBuilder m_EscapeBuilder;

		// Token: 0x02000472 RID: 1138
		public struct Enumerator
		{
			// Token: 0x1700014E RID: 334
			// (get) Token: 0x06001980 RID: 6528 RVA: 0x00012776 File Offset: 0x00010976
			public bool IsValid
			{
				get
				{
					return this.type > JSONNode.Enumerator.Type.None;
				}
			}

			// Token: 0x06001981 RID: 6529 RVA: 0x00012781 File Offset: 0x00010981
			public Enumerator(List<JSONNode>.Enumerator aArrayEnum)
			{
				this.type = JSONNode.Enumerator.Type.Array;
				this.m_Object = default(Dictionary<string, JSONNode>.Enumerator);
				this.m_Array = aArrayEnum;
			}

			// Token: 0x06001982 RID: 6530 RVA: 0x0001279D File Offset: 0x0001099D
			public Enumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
			{
				this.type = JSONNode.Enumerator.Type.Object;
				this.m_Object = aDictEnum;
				this.m_Array = default(List<JSONNode>.Enumerator);
			}

			// Token: 0x1700014F RID: 335
			// (get) Token: 0x06001983 RID: 6531 RVA: 0x0009621C File Offset: 0x0009441C
			public KeyValuePair<string, JSONNode> Current
			{
				get
				{
					if (this.type == JSONNode.Enumerator.Type.Array)
					{
						return new KeyValuePair<string, JSONNode>(string.Empty, this.m_Array.Current);
					}
					if (this.type == JSONNode.Enumerator.Type.Object)
					{
						return this.m_Object.Current;
					}
					return new KeyValuePair<string, JSONNode>(string.Empty, null);
				}
			}

			// Token: 0x06001984 RID: 6532 RVA: 0x000127B9 File Offset: 0x000109B9
			public bool MoveNext()
			{
				if (this.type == JSONNode.Enumerator.Type.Array)
				{
					return this.m_Array.MoveNext();
				}
				return this.type == JSONNode.Enumerator.Type.Object && this.m_Object.MoveNext();
			}

			// Token: 0x040015B7 RID: 5559
			private JSONNode.Enumerator.Type type;

			// Token: 0x040015B8 RID: 5560
			private Dictionary<string, JSONNode>.Enumerator m_Object;

			// Token: 0x040015B9 RID: 5561
			private List<JSONNode>.Enumerator m_Array;

			// Token: 0x02000473 RID: 1139
			private enum Type
			{
				// Token: 0x040015BB RID: 5563
				None,
				// Token: 0x040015BC RID: 5564
				Array,
				// Token: 0x040015BD RID: 5565
				Object
			}
		}

		// Token: 0x02000474 RID: 1140
		public struct ValueEnumerator
		{
			// Token: 0x06001985 RID: 6533 RVA: 0x000127E6 File Offset: 0x000109E6
			public ValueEnumerator(List<JSONNode>.Enumerator aArrayEnum)
			{
				this = new JSONNode.ValueEnumerator(new JSONNode.Enumerator(aArrayEnum));
			}

			// Token: 0x06001986 RID: 6534 RVA: 0x000127F4 File Offset: 0x000109F4
			public ValueEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
			{
				this = new JSONNode.ValueEnumerator(new JSONNode.Enumerator(aDictEnum));
			}

			// Token: 0x06001987 RID: 6535 RVA: 0x00012802 File Offset: 0x00010A02
			public ValueEnumerator(JSONNode.Enumerator aEnumerator)
			{
				this.m_Enumerator = aEnumerator;
			}

			// Token: 0x17000150 RID: 336
			// (get) Token: 0x06001988 RID: 6536 RVA: 0x00096268 File Offset: 0x00094468
			public JSONNode Current
			{
				get
				{
					KeyValuePair<string, JSONNode> keyValuePair = this.m_Enumerator.Current;
					return keyValuePair.Value;
				}
			}

			// Token: 0x06001989 RID: 6537 RVA: 0x0001280B File Offset: 0x00010A0B
			public bool MoveNext()
			{
				return this.m_Enumerator.MoveNext();
			}

			// Token: 0x0600198A RID: 6538 RVA: 0x00012818 File Offset: 0x00010A18
			public JSONNode.ValueEnumerator GetEnumerator()
			{
				return this;
			}

			// Token: 0x040015BE RID: 5566
			private JSONNode.Enumerator m_Enumerator;
		}

		// Token: 0x02000475 RID: 1141
		public struct KeyEnumerator
		{
			// Token: 0x0600198B RID: 6539 RVA: 0x00012820 File Offset: 0x00010A20
			public KeyEnumerator(List<JSONNode>.Enumerator aArrayEnum)
			{
				this = new JSONNode.KeyEnumerator(new JSONNode.Enumerator(aArrayEnum));
			}

			// Token: 0x0600198C RID: 6540 RVA: 0x0001282E File Offset: 0x00010A2E
			public KeyEnumerator(Dictionary<string, JSONNode>.Enumerator aDictEnum)
			{
				this = new JSONNode.KeyEnumerator(new JSONNode.Enumerator(aDictEnum));
			}

			// Token: 0x0600198D RID: 6541 RVA: 0x0001283C File Offset: 0x00010A3C
			public KeyEnumerator(JSONNode.Enumerator aEnumerator)
			{
				this.m_Enumerator = aEnumerator;
			}

			// Token: 0x17000151 RID: 337
			// (get) Token: 0x0600198E RID: 6542 RVA: 0x00096288 File Offset: 0x00094488
			public JSONNode Current
			{
				get
				{
					KeyValuePair<string, JSONNode> keyValuePair = this.m_Enumerator.Current;
					return keyValuePair.Key;
				}
			}

			// Token: 0x0600198F RID: 6543 RVA: 0x00012845 File Offset: 0x00010A45
			public bool MoveNext()
			{
				return this.m_Enumerator.MoveNext();
			}

			// Token: 0x06001990 RID: 6544 RVA: 0x00012852 File Offset: 0x00010A52
			public JSONNode.KeyEnumerator GetEnumerator()
			{
				return this;
			}

			// Token: 0x040015BF RID: 5567
			private JSONNode.Enumerator m_Enumerator;
		}

		// Token: 0x02000476 RID: 1142
		public class LinqEnumerator : IEnumerator<KeyValuePair<string, JSONNode>>, IEnumerator, IDisposable, IEnumerable<KeyValuePair<string, JSONNode>>, IEnumerable
		{
			// Token: 0x06001991 RID: 6545 RVA: 0x0001285A File Offset: 0x00010A5A
			internal LinqEnumerator(JSONNode aNode)
			{
				this.m_Node = aNode;
				if (this.m_Node != null)
				{
					this.m_Enumerator = this.m_Node.GetEnumerator();
				}
			}

			// Token: 0x17000152 RID: 338
			// (get) Token: 0x06001992 RID: 6546 RVA: 0x00012888 File Offset: 0x00010A88
			public KeyValuePair<string, JSONNode> Current
			{
				get
				{
					return this.m_Enumerator.Current;
				}
			}

			// Token: 0x17000153 RID: 339
			// (get) Token: 0x06001993 RID: 6547 RVA: 0x00012895 File Offset: 0x00010A95
			object IEnumerator.Current
			{
				get
				{
					return this.m_Enumerator.Current;
				}
			}

			// Token: 0x06001994 RID: 6548 RVA: 0x000128A7 File Offset: 0x00010AA7
			public bool MoveNext()
			{
				return this.m_Enumerator.MoveNext();
			}

			// Token: 0x06001995 RID: 6549 RVA: 0x000128B4 File Offset: 0x00010AB4
			public void Dispose()
			{
				this.m_Node = null;
				this.m_Enumerator = default(JSONNode.Enumerator);
			}

			// Token: 0x06001996 RID: 6550 RVA: 0x000128C9 File Offset: 0x00010AC9
			public IEnumerator<KeyValuePair<string, JSONNode>> GetEnumerator()
			{
				return new JSONNode.LinqEnumerator(this.m_Node);
			}

			// Token: 0x06001997 RID: 6551 RVA: 0x000128D6 File Offset: 0x00010AD6
			public void Reset()
			{
				if (this.m_Node != null)
				{
					this.m_Enumerator = this.m_Node.GetEnumerator();
				}
			}

			// Token: 0x06001998 RID: 6552 RVA: 0x000128C9 File Offset: 0x00010AC9
			IEnumerator IEnumerable.GetEnumerator()
			{
				return new JSONNode.LinqEnumerator(this.m_Node);
			}

			// Token: 0x040015C0 RID: 5568
			private JSONNode m_Node;

			// Token: 0x040015C1 RID: 5569
			private JSONNode.Enumerator m_Enumerator;
		}
	}
}
