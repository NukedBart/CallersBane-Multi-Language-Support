using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// Token: 0x0200015F RID: 351
public static class ConnectionRegistry
{
	// Token: 0x06000AFF RID: 2815 RVA: 0x00050F50 File Offset: 0x0004F150
	public static void register(MiniCommunicator comm)
	{
		foreach (ConnectionRegistry._ConnectionCheck connectionCheck in ConnectionRegistry._comms)
		{
			if (connectionCheck.comm == comm)
			{
				return;
			}
		}
		ConnectionRegistry._comms.Add(new ConnectionRegistry._ConnectionCheck(comm));
	}

	// Token: 0x06000B00 RID: 2816 RVA: 0x00050FC8 File Offset: 0x0004F1C8
	public static void unregister(MiniCommunicator comm)
	{
		ConnectionRegistry._comms.RemoveAll((ConnectionRegistry._ConnectionCheck cc) => cc.comm == comm);
	}

	// Token: 0x06000B01 RID: 2817 RVA: 0x00050FFC File Offset: 0x0004F1FC
	public static bool updateDisconnectCheck()
	{
		bool flag = !Enumerable.All<ConnectionRegistry._ConnectionCheck>(ConnectionRegistry._comms, (ConnectionRegistry._ConnectionCheck cc) => cc.updateIsConnected(0.5f));
		if (flag)
		{
			if (ConnectionRegistry._disconnectTimestamp < 0L)
			{
				ConnectionRegistry._disconnectTimestamp = TimeUtil.CurrentTimeMillis();
			}
		}
		else
		{
			ConnectionRegistry.resetDisconnectedTime();
		}
		return flag;
	}

	// Token: 0x06000B02 RID: 2818 RVA: 0x000093DB File Offset: 0x000075DB
	public static void resetDisconnectedTime()
	{
		ConnectionRegistry._disconnectTimestamp = -1L;
	}

	// Token: 0x06000B03 RID: 2819 RVA: 0x000093E4 File Offset: 0x000075E4
	public static float secondsDisconnected()
	{
		if (ConnectionRegistry._disconnectTimestamp < 0L)
		{
			return -1f;
		}
		return (float)(TimeUtil.CurrentTimeMillis() - ConnectionRegistry._disconnectTimestamp) * 0.001f;
	}

	// Token: 0x0400087B RID: 2171
	private const float MinimumDisconnectTime = 0.5f;

	// Token: 0x0400087C RID: 2172
	private static List<ConnectionRegistry._ConnectionCheck> _comms = new List<ConnectionRegistry._ConnectionCheck>();

	// Token: 0x0400087D RID: 2173
	private static long _disconnectTimestamp = 0L;

	// Token: 0x02000160 RID: 352
	private class _ConnectionCheck
	{
		// Token: 0x06000B05 RID: 2821 RVA: 0x00009417 File Offset: 0x00007617
		public _ConnectionCheck(MiniCommunicator c)
		{
			this.comm = c;
			this._lastTime = Time.time;
		}

		// Token: 0x06000B06 RID: 2822 RVA: 0x00009431 File Offset: 0x00007631
		public bool updateIsConnected(float minimumDisconnectedSeconds)
		{
			if (this.comm.isDisconnected())
			{
				return Time.time - this._lastTime < minimumDisconnectedSeconds;
			}
			this._lastTime = Time.time;
			return true;
		}

		// Token: 0x0400087F RID: 2175
		public readonly MiniCommunicator comm;

		// Token: 0x04000880 RID: 2176
		private float _lastTime;
	}
}
