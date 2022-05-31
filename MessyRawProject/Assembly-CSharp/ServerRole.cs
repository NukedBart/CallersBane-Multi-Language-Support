using System;

// Token: 0x02000317 RID: 791
[Flags]
public enum ServerRole
{
	// Token: 0x0400102C RID: 4140
	LOBBY = 1,
	// Token: 0x0400102D RID: 4141
	GAME = 2,
	// Token: 0x0400102E RID: 4142
	RESOURCE = 4,
	// Token: 0x0400102F RID: 4143
	LOOKUP = 8,
	// Token: 0x04001030 RID: 4144
	GRAPHITE = 16
}
