using System;

// Token: 0x020003CD RID: 973
public class GlobalSettings
{
	// Token: 0x040012E8 RID: 4840
	public GlobalSettings.Graphics graphics = new GlobalSettings.Graphics();

	// Token: 0x040012E9 RID: 4841
	public GlobalSettings.User user = new GlobalSettings.User();

	// Token: 0x040012EA RID: 4842
	public GlobalSettings.Music music = new GlobalSettings.Music();

	// Token: 0x020003CE RID: 974
	public class Graphics : ISettingsGroup
	{
		// Token: 0x040012EB RID: 4843
		public VideoMode resolution = VideoMode.getDefault();
	}

	// Token: 0x020003CF RID: 975
	public class User : ISettingsGroup
	{
		// Token: 0x060015A4 RID: 5540 RVA: 0x0000FC68 File Offset: 0x0000DE68
		public bool isRemembered()
		{
			return !string.IsNullOrEmpty(this.name.value);
		}

		// Token: 0x040012EC RID: 4844
		public SvString name = new SvString();

		// Token: 0x040012ED RID: 4845
		public SvInt server_index = new SvInt(0);
	}

	// Token: 0x020003D0 RID: 976
	public class Music : ISettingsGroup
	{
		// Token: 0x040012EE RID: 4846
		public SvFloat volume = new SvFloat(0.25f).Unit();
	}
}
