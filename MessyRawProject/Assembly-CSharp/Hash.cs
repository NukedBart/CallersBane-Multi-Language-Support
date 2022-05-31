using System;
using System.Security.Cryptography;
using System.Text;

// Token: 0x02000164 RID: 356
public class Hash
{
	// Token: 0x06000B13 RID: 2835 RVA: 0x000094A8 File Offset: 0x000076A8
	public static string sha256(string s)
	{
		return Hash.toHex(new SHA256Managed().ComputeHash(Hash.toBytes(s)));
	}

	// Token: 0x06000B14 RID: 2836 RVA: 0x000094BF File Offset: 0x000076BF
	private static byte[] toBytes(string s)
	{
		return Encoding.Unicode.GetBytes(s);
	}

	// Token: 0x06000B15 RID: 2837 RVA: 0x00051370 File Offset: 0x0004F570
	private static string toHex(byte[] bytes)
	{
		StringBuilder stringBuilder = new StringBuilder();
		foreach (byte b in bytes)
		{
			stringBuilder.Append(string.Format("{0:x2}", b));
		}
		return stringBuilder.ToString();
	}
}
