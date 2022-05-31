using System;
using System.Collections.Generic;
using Animation.Serialization;
using JsonFx.Json;
using UnityEngine;

namespace Irrelevant.Assets
{
	// Token: 0x0200001C RID: 28
	public static class UnitAnimDescriptionFactory
	{
		// Token: 0x0600017C RID: 380 RVA: 0x0001F640 File Offset: 0x0001D840
		public static UnitAnimDescription fromProtoFolder(string folder)
		{
			PD_Atlas pd_Atlas = new PD_Atlas();
			if (!ProtoUtil.deserializeInto(folder + "/spritespos.bytes", pd_Atlas, typeof(PD_Atlas)))
			{
				return null;
			}
			PD_AnimCollection pd_AnimCollection = new PD_AnimCollection();
			if (!ProtoUtil.deserializeInto(folder + "/anims.bytes", pd_AnimCollection, typeof(PD_AnimCollection)))
			{
				return null;
			}
			return new UnitAnimDescription(new AtlasAnimsBundle(pd_Atlas, pd_AnimCollection));
		}

		// Token: 0x0600017D RID: 381 RVA: 0x000033A7 File Offset: 0x000015A7
		private static UnitAnimDescription fromJson(string desc, string data)
		{
			return new UnitAnimDescription(UnitAnimDescriptionFactory.bundleFromJson(desc, data));
		}

		// Token: 0x0600017E RID: 382 RVA: 0x000033B5 File Offset: 0x000015B5
		private static AtlasAnimsBundle bundleFromJson(string desc, string data)
		{
			return new AtlasAnimsBundle(UnitAnimDescriptionFactory.atlasFromJson(desc), UnitAnimDescriptionFactory.animationsFromJson(data));
		}

		// Token: 0x0600017F RID: 383 RVA: 0x0001F6AC File Offset: 0x0001D8AC
		public static UnitAnimDescription fromImageAndRect(Texture2D tex, float x, float y, float scale)
		{
			PD_AtlasItem pd_AtlasItem = new PD_AtlasItem();
			pd_AtlasItem.id = 0;
			pd_AtlasItem.w = tex.width;
			pd_AtlasItem.h = tex.height;
			pd_AtlasItem.realW = (int)((float)pd_AtlasItem.w * scale);
			pd_AtlasItem.realH = (int)((float)pd_AtlasItem.h * scale);
			PD_AnimFramePart pd_AnimFramePart = new PD_AnimFramePart();
			pd_AnimFramePart.a = (pd_AnimFramePart.d = 1f);
			pd_AnimFramePart.tx = x;
			pd_AnimFramePart.ty = y;
			pd_AnimFramePart.frame = (int)(pd_AnimFramePart.layer = (pd_AnimFramePart.meshId = 0));
			int width = tex.width;
			int height = tex.height;
			List<PD_AtlasItem> list = new List<PD_AtlasItem>();
			list.Add(pd_AtlasItem);
			PD_Atlas atlas = new PD_Atlas(width, height, list);
			List<PD_Anim> list2 = new List<PD_Anim>();
			List<PD_Anim> list3 = list2;
			string text = "idle";
			List<PD_AnimFramePart> list4 = new List<PD_AnimFramePart>();
			list4.Add(pd_AnimFramePart);
			list3.Add(new PD_Anim(text, list4, new List<PD_AnimLocator>(), -1f));
			List<PD_Anim> list5 = list2;
			string text2 = "charge";
			list4 = new List<PD_AnimFramePart>();
			list4.Add(pd_AnimFramePart);
			list5.Add(new PD_Anim(text2, list4, new List<PD_AnimLocator>(), -1f));
			List<PD_Anim> list6 = list2;
			string text3 = "attack";
			list4 = new List<PD_AnimFramePart>();
			list4.Add(pd_AnimFramePart);
			List<PD_AnimFramePart> list7 = list4;
			List<PD_AnimLocator> list8 = new List<PD_AnimLocator>();
			list8.Add(new PD_AnimLocator("hit", 0));
			list6.Add(new PD_Anim(text3, list7, list8, -1f));
			List<PD_Anim> list9 = list2;
			string text4 = "retreat";
			list4 = new List<PD_AnimFramePart>();
			list4.Add(pd_AnimFramePart);
			list9.Add(new PD_Anim(text4, list4, new List<PD_AnimLocator>(), -1f));
			return new UnitAnimDescription(new AtlasAnimsBundle(atlas, new PD_AnimCollection(list2)))
			{
				textureReference = tex
			};
		}

		// Token: 0x06000180 RID: 384 RVA: 0x0001F84C File Offset: 0x0001DA4C
		private static PD_AnimCollection animationsFromJson(string data)
		{
			if (data == null)
			{
				return null;
			}
			List<PD_Anim> list = new List<PD_Anim>();
			JsonReader jsonReader = new JsonReader();
			object[] array = (object[])jsonReader.Read(data);
			foreach (object obj in array)
			{
				Dictionary<string, object> animRoot = (Dictionary<string, object>)obj;
				PD_Anim pd_Anim = UnitAnimDescriptionFactory.parseAnimation(animRoot);
				if (pd_Anim != null)
				{
					list.Add(pd_Anim);
				}
			}
			return new PD_AnimCollection(list);
		}

		// Token: 0x06000181 RID: 385 RVA: 0x0001F8C8 File Offset: 0x0001DAC8
		private static PD_Anim parseAnimation(Dictionary<string, object> animRoot)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)animRoot["anim"];
			string text = null;
			float num = -1f;
			object i = null;
			if (dictionary.TryGetValue("fps", ref i))
			{
				num = UnitAnimDescriptionFactory._toFloat(i);
			}
			object obj = null;
			if (dictionary.TryGetValue("name", ref obj))
			{
				text = (string)obj;
			}
			if (((object[])animRoot["frames"]).Length <= 0)
			{
				return null;
			}
			object[][] array = (object[][])animRoot["frames"];
			List<PD_AnimFramePart> list = new List<PD_AnimFramePart>();
			foreach (object array3 in array)
			{
				PD_AnimFramePart pd_AnimFramePart = new PD_AnimFramePart();
				pd_AnimFramePart.frame = (int)array3[0];
				pd_AnimFramePart.meshId = short.Parse((string)array3[1]) - 1;
				pd_AnimFramePart.layer = (short)((int)array3[2]);
				if (array3.Length > 4)
				{
					pd_AnimFramePart.colorBits = (long)((ulong)Convert.ToUInt32(array3[4]));
				}
				float[] array4 = UnitAnimDescriptionFactory._toFloats(array3[3]);
				pd_AnimFramePart.a = array4[0];
				pd_AnimFramePart.b = array4[1];
				pd_AnimFramePart.c = array4[2];
				pd_AnimFramePart.d = array4[3];
				pd_AnimFramePart.tx = array4[4];
				pd_AnimFramePart.ty = array4[5];
				list.Add(pd_AnimFramePart);
			}
			return new PD_Anim(text, list, UnitAnimDescriptionFactory.parseAnimLocators(animRoot), num);
		}

		// Token: 0x06000182 RID: 386 RVA: 0x0001FA40 File Offset: 0x0001DC40
		private static List<PD_AnimLocator> parseAnimLocators(Dictionary<string, object> animRoot)
		{
			object[] array = (object[])animRoot["locators"];
			if (array.Length == 0)
			{
				return new List<PD_AnimLocator>();
			}
			Dictionary<string, object>[] array2 = (Dictionary<string, object>[])array;
			List<PD_AnimLocator> list = new List<PD_AnimLocator>();
			foreach (Dictionary<string, object> dictionary in array2)
			{
				PD_AnimLocator pd_AnimLocator = new PD_AnimLocator((string)dictionary["name"], (int)dictionary["frame"]);
				if (dictionary.ContainsKey("pos"))
				{
					Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["pos"];
					pd_AnimLocator.pos = new PD_AnimLocator.Pos(0.01f * (float)Convert.ToDouble(dictionary2["x"]), 0.01f * (float)Convert.ToDouble(dictionary2["y"]), (float)Convert.ToDouble(dictionary2["rot"]), (float)Convert.ToDouble(dictionary2["sx"]), (float)Convert.ToDouble(dictionary2["sy"]));
				}
				list.Add(pd_AnimLocator);
			}
			return list;
		}

		// Token: 0x06000183 RID: 387 RVA: 0x0001FB60 File Offset: 0x0001DD60
		private static PD_Atlas atlasFromJson(string jsonData)
		{
			if (jsonData == null)
			{
				return null;
			}
			JsonReader jsonReader = new JsonReader();
			return UnitAnimDescriptionFactory.parseAtlas((Dictionary<string, object>)jsonReader.Read(jsonData));
		}

		// Token: 0x06000184 RID: 388 RVA: 0x0001FB8C File Offset: 0x0001DD8C
		private static PD_Atlas parseAtlas(Dictionary<string, object> data)
		{
			List<PD_AtlasItem> list = new List<PD_AtlasItem>();
			object[][] array = (object[][])data["atlas"];
			foreach (object array3 in array)
			{
				list.Add(new PD_AtlasItem
				{
					id = short.Parse((string)array3[0]),
					x = (int)array3[1],
					y = (int)array3[2],
					w = (int)array3[3],
					h = (int)array3[4],
					realW = (int)array3[5],
					realH = (int)array3[6]
				});
			}
			int num = (int)data["width"];
			int num2 = (int)data["height"];
			return new PD_Atlas(num, num2, list);
		}

		// Token: 0x06000185 RID: 389 RVA: 0x000033C8 File Offset: 0x000015C8
		private static float _toFloat(object i)
		{
			if (i is double)
			{
				return (float)((double)i);
			}
			if (i is int)
			{
				return (float)((int)i);
			}
			return (float)i;
		}

		// Token: 0x06000186 RID: 390 RVA: 0x0001FC7C File Offset: 0x0001DE7C
		private static float[] _toFloats(object array)
		{
			float[] array3;
			if (array is double[])
			{
				double[] array2 = (double[])array;
				array3 = new float[array2.Length];
				for (int i = 0; i < array2.Length; i++)
				{
					array3[i] = (float)array2[i];
				}
			}
			else if (array is object[])
			{
				object[] array4 = (object[])array;
				array3 = new float[array4.Length];
				for (int j = 0; j < array4.Length; j++)
				{
					array3[j] = UnitAnimDescriptionFactory._toFloat(array4[j]);
				}
			}
			else
			{
				if (!(array is int[]))
				{
					throw new Exception("Unknown datatype " + array + " found when converting to floats");
				}
				int[] array5 = (int[])array;
				array3 = new float[array5.Length];
				for (int k = 0; k < array5.Length; k++)
				{
					array3[k] = (float)array5[k];
				}
			}
			return array3;
		}
	}
}
