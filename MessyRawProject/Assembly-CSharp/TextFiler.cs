using System;

// Token: 0x0200021B RID: 539
public class TextFiler
{
	// Token: 0x06001139 RID: 4409 RVA: 0x0000D2E2 File Offset: 0x0000B4E2
	public TextFiler(string path)
	{
		this.path = path;
	}

	// Token: 0x0600113A RID: 4410 RVA: 0x0000D2F1 File Offset: 0x0000B4F1
	public string load()
	{
		return FileUtil.readFileContents(this.path);
	}

	// Token: 0x0600113B RID: 4411 RVA: 0x0000D2FE File Offset: 0x0000B4FE
	public void save(string servers)
	{
		FileUtil.writeFileContents(this.path, servers);
	}

	// Token: 0x0600113C RID: 4412 RVA: 0x0000D30C File Offset: 0x0000B50C
	public bool exists()
	{
		return FileUtil.fileExists(this.path);
	}

	// Token: 0x04000DA4 RID: 3492
	private string path;
}
