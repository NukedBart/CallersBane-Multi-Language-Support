using System;
using System.Security.Cryptography;
using System.Text;

// Token: 0x02000163 RID: 355
public class RSACrypt
{
	// Token: 0x06000B0E RID: 2830 RVA: 0x00009472 File Offset: 0x00007672
	private RSACrypt(RSACryptoServiceProvider rsa)
	{
		this.rsa = rsa;
	}

	// Token: 0x06000B0F RID: 2831 RVA: 0x00009481 File Offset: 0x00007681
	private byte[] encrypt(string s)
	{
		return this.rsa.Encrypt(Encoding.UTF8.GetBytes(s), false);
	}

	// Token: 0x06000B10 RID: 2832 RVA: 0x0000949A File Offset: 0x0000769A
	public string encrypt64(string s)
	{
		return Convert.ToBase64String(this.encrypt(s));
	}

	// Token: 0x06000B11 RID: 2833 RVA: 0x00051350 File Offset: 0x0004F550
	public static RSACrypt fromXmlKey(string publicXmlKey)
	{
		RSACryptoServiceProvider rsacryptoServiceProvider = new RSACryptoServiceProvider();
		rsacryptoServiceProvider.FromXmlString(publicXmlKey);
		return new RSACrypt(rsacryptoServiceProvider);
	}

	// Token: 0x04000882 RID: 2178
	private RSACryptoServiceProvider rsa;
}
