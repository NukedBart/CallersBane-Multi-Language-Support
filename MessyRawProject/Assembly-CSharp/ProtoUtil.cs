using System;
using System.IO;
using ProtoBuf.Meta;

// Token: 0x02000451 RID: 1105
public class ProtoUtil
{
	// Token: 0x06001899 RID: 6297 RVA: 0x00092B94 File Offset: 0x00090D94
	public static bool deserializeInto(string filename, object into, Type type)
	{
		if (!File.Exists(filename))
		{
			return false;
		}
		bool result;
		using (FileStream fileStream = new FileStream(filename, 3))
		{
			if (fileStream == null)
			{
				result = false;
			}
			else
			{
				result = ProtoUtil.deserializeInto(fileStream, into, type);
			}
		}
		return result;
	}

	// Token: 0x0600189A RID: 6298 RVA: 0x00011DD8 File Offset: 0x0000FFD8
	public static bool deserializeInto(byte[] bytes, object into, Type type)
	{
		return ProtoUtil.deserializeInto(new MemoryStream(bytes), into, type);
	}

	// Token: 0x0600189B RID: 6299 RVA: 0x00011DE7 File Offset: 0x0000FFE7
	public static bool deserializeInto(Stream stream, object into, Type type)
	{
		return ProtoUtil.getSerializer(type).Deserialize(stream, into, type) != null;
	}

	// Token: 0x0600189C RID: 6300 RVA: 0x00092BF4 File Offset: 0x00090DF4
	public static void serialize(object obj, string filename)
	{
		using (FileStream fileStream = new FileStream(filename, 2))
		{
			ProtoUtil.getSerializer(obj).Serialize(fileStream, obj);
		}
	}

	// Token: 0x0600189D RID: 6301 RVA: 0x00011DFD File Offset: 0x0000FFFD
	private static TypeModel getSerializer(object obj)
	{
		return ProtoUtil.getSerializer(obj.GetType());
	}

	// Token: 0x0600189E RID: 6302 RVA: 0x00011E0A File Offset: 0x0001000A
	private static TypeModel getSerializer(Type type)
	{
		if (ProtoUtil.animationSerializer.CanSerialize(type))
		{
			return ProtoUtil.animationSerializer;
		}
		Log.critical("Can not serialize Type " + type);
		return null;
	}

	// Token: 0x04001541 RID: 5441
	private static TypeModel animationSerializer = new AnimationSerializer();
}
