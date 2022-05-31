using System;

// Token: 0x020002C0 RID: 704
public class ServerInfoMessage : Message
{
	// Token: 0x06001295 RID: 4757 RVA: 0x0000DF6E File Offset: 0x0000C16E
	public Version getVersion()
	{
		return new Version(this.version);
	}

	// Token: 0x06001296 RID: 4758 RVA: 0x00077F40 File Offset: 0x00076140
	public bool shouldCheckForUpdate()
	{
		Version version = this.getVersion();
		Version gameVersion = SharedConstants.getGameVersion();
		Version v = new Version(version.getMajor(), version.getMinor());
		Version version2 = new Version(gameVersion.getMajor(), gameVersion.getMinor());
		return version2.isLowerThan(v);
	}

	// Token: 0x06001297 RID: 4759 RVA: 0x00077F88 File Offset: 0x00076188
	public ServerRole serverRoles()
	{
		ServerRole serverRole = (ServerRole)0;
		string[] array = this.roles.Split(new char[]
		{
			','
		});
		foreach (string text in array)
		{
			if (Enum.IsDefined(typeof(ServerRole), text))
			{
				serverRole |= (ServerRole)((int)Enum.Parse(typeof(ServerRole), text));
			}
		}
		return serverRole;
	}

	// Token: 0x04000F66 RID: 3942
	public string assetURL;

	// Token: 0x04000F67 RID: 3943
	public string newsURL;

	// Token: 0x04000F68 RID: 3944
	public string version;

	// Token: 0x04000F69 RID: 3945
	public string roles = "LOOKUP";
}
