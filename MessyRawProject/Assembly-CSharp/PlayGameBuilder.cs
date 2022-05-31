using System;
using System.Reflection;

// Token: 0x02000324 RID: 804
public class PlayGameBuilder
{
	// Token: 0x06001350 RID: 4944 RVA: 0x0000E5FA File Offset: 0x0000C7FA
	public PlayGameBuilder(Message msg)
	{
		this.msg = msg;
	}

	// Token: 0x06001351 RID: 4945 RVA: 0x0000E609 File Offset: 0x0000C809
	public PlayGameBuilder setDeck(string deck)
	{
		return this._setProperty<string>("deck", deck);
	}

	// Token: 0x06001352 RID: 4946 RVA: 0x0000E617 File Offset: 0x0000C817
	public PlayGameBuilder setLevelId(int levelId)
	{
		return this._setProperty<int>("levelId", levelId);
	}

	// Token: 0x06001353 RID: 4947 RVA: 0x0000E625 File Offset: 0x0000C825
	public PlayGameBuilder setOpponent(int profileId)
	{
		return this._setProperty<int>("profileId", profileId);
	}

	// Token: 0x06001354 RID: 4948 RVA: 0x00078A0C File Offset: 0x00076C0C
	public PlayGameBuilder setRobotName(AiDifficulty difficulty)
	{
		if (this.msg is PlaySinglePlayerCustomQuickmatchMessage)
		{
			return this._setProperty<AiDifficulty>("difficulty", difficulty);
		}
		return this._setProperty<string>("robotName", "Robot" + StringUtil.capitalize(difficulty.ToString()));
	}

	// Token: 0x06001355 RID: 4949 RVA: 0x0000E633 File Offset: 0x0000C833
	public PlayGameBuilder setCustomGameId(int customGameId)
	{
		return this._setProperty<int>("customGameId", customGameId);
	}

	// Token: 0x06001356 RID: 4950 RVA: 0x00078A5C File Offset: 0x00076C5C
	private PlayGameBuilder _setProperty<T>(string name, T value)
	{
		Type type = this.msg.GetType();
		FieldInfo field = type.GetField(name);
		if (field == null)
		{
			throw new NotSupportedException("Missing field: " + name);
		}
		object obj = Convert.ChangeType(value, field.FieldType);
		field.SetValue(this.msg, obj);
		return this;
	}

	// Token: 0x06001357 RID: 4951 RVA: 0x0000E641 File Offset: 0x0000C841
	public static implicit operator Message(PlayGameBuilder b)
	{
		return b.msg;
	}

	// Token: 0x04001041 RID: 4161
	private Message msg;
}
