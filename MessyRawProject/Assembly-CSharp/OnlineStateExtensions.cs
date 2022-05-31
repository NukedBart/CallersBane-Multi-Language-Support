using System;

// Token: 0x020002DB RID: 731
public static class OnlineStateExtensions
{
	// Token: 0x060012D3 RID: 4819 RVA: 0x000783C8 File Offset: 0x000765C8
	public static string prefixString(this OnlineState? state)
	{
		string s = state.ToString();
		if (state == null)
		{
			return string.Empty;
		}
		if (state == OnlineState.LOBBY)
		{
			return string.Empty;
		}
		if (state == OnlineState.GAME)
		{
			s = "Match";
		}
		return "[" + StringUtil.capitalize(s) + "] ";
	}
}
