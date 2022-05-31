using System;
using System.Net.Sockets;
using System.Text;
using UnityEngine;

// Token: 0x02000166 RID: 358
public class GameSocket : MonoBehaviour
{
	// Token: 0x06000B19 RID: 2841 RVA: 0x000094D8 File Offset: 0x000076D8
	public GameSocket Init(ISocketListener listener)
	{
		this.listener = listener;
		return this;
	}

	// Token: 0x06000B1A RID: 2842 RVA: 0x00051410 File Offset: 0x0004F610
	public bool Connect(IpPort address)
	{
		this.Close();
		this.address = address;
		Socket socket = new Socket(2, 1, 6);
		socket.Blocking = true;
		try
		{
			socket.Connect(address.ip, address.port);
		}
		catch (Exception ex)
		{
			Log.warning("Error connecting to socket: " + ex);
			return false;
		}
		this.hasBeenConnected = true;
		this.wasConnectedLastFrame = true;
		this.socket = socket;
		this.socket.SendTimeout = GameSocket.TimeoutMilliseconds;
		this.socket.Blocking = false;
		this.msgParser.clearAll();
		Log.warning("CLEARING ALL PARSED MESSAGES 1");
		return true;
	}

	// Token: 0x06000B1B RID: 2843 RVA: 0x000094E2 File Offset: 0x000076E2
	public IpPort getAddress()
	{
		return this.address;
	}

	// Token: 0x06000B1C RID: 2844 RVA: 0x000514C8 File Offset: 0x0004F6C8
	public bool Close()
	{
		this.wasConnectedLastFrame = false;
		if (this.socket == null)
		{
			return false;
		}
		Log.warning("CLOSING SOCKET: ");
		try
		{
			this.socket.Close();
		}
		catch (SocketException ex)
		{
			Log.error("socket close failed with SocketException: " + ex);
		}
		catch (ObjectDisposedException ex2)
		{
			Log.error("socket close failed with ObjectDisposedException: " + ex2);
		}
		this.socket = null;
		return true;
	}

	// Token: 0x06000B1D RID: 2845 RVA: 0x000094EA File Offset: 0x000076EA
	public void CloseAndFakeDisconnect()
	{
		this.closeAndPostDisconnect();
	}

	// Token: 0x06000B1E RID: 2846 RVA: 0x000094F2 File Offset: 0x000076F2
	private void closeAndPostDisconnect()
	{
		this.pendingReconnect = 0;
		if (this.Close())
		{
			Log.info("Posting disconnect");
			this.listener.OnDisconnect(this);
		}
	}

	// Token: 0x06000B1F RID: 2847 RVA: 0x0000951C File Offset: 0x0000771C
	public void Dispose()
	{
		Log.warning("CLEARING ALL PARSED MESSAGES 2 (abort)");
		this.msgParser.abort();
		this.Close();
	}

	// Token: 0x06000B20 RID: 2848 RVA: 0x0000953A File Offset: 0x0000773A
	private void OnDestroy()
	{
		this.Dispose();
	}

	// Token: 0x06000B21 RID: 2849 RVA: 0x00009542 File Offset: 0x00007742
	public void SetFakeData(string data)
	{
		this.msgParser.clearAll();
		this.msgParser.feed(data);
	}

	// Token: 0x06000B22 RID: 2850 RVA: 0x0000955B File Offset: 0x0000775B
	public IMessageParser GetMessageParser()
	{
		return this.msgParser;
	}

	// Token: 0x06000B23 RID: 2851 RVA: 0x00051554 File Offset: 0x0004F754
	public bool Send(Message m)
	{
		if (!this.IsConnected())
		{
			Log.warning("Trying to send: " + m.GetType() + " but we aren't connected");
			return false;
		}
		this._readSocketCompletely();
		string text = m.ToString().Replace("\\u007F", "#").Replace("\\b", "#");
		return this.Send(Encoding.UTF8.GetBytes(text));
	}

	// Token: 0x06000B24 RID: 2852 RVA: 0x000515C4 File Offset: 0x0004F7C4
	private bool Send(byte[] bytes)
	{
		try
		{
			int i = bytes.Length;
			int num = 0;
			while (i > 0)
			{
				int num2 = i;
				Log.info("Sending <= " + num2 + " bytes");
				int num3 = this.socket.Send(bytes, num, num2, 0);
				num += num3;
				i -= num3;
				if (num3 < 0)
				{
					throw new InvalidOperationException("count is negative! " + num3);
				}
				Log.info(string.Concat(new object[]
				{
					"Sent ",
					num,
					" bytes (of scheduled ",
					bytes.Length,
					"), to ",
					this.socket.Connected,
					", ",
					this.socket.RemoteEndPoint.ToString()
				}));
			}
		}
		catch (SocketException ex)
		{
			Log.error(string.Concat(new object[]
			{
				"Socket.Send failed with error: ",
				ex.ErrorCode,
				"/",
				ex.NativeErrorCode,
				"/",
				ex.SocketErrorCode
			}));
			this.closeAndPostDisconnect();
			return false;
		}
		return true;
	}

	// Token: 0x06000B25 RID: 2853 RVA: 0x00051724 File Offset: 0x0004F924
	public Message Receive()
	{
		Message message = this._receive();
		if (this.pendingReconnect > 0 && message == null)
		{
			Log.warning("Pending " + this.pendingReconnect);
			this.pendingReconnect++;
		}
		return message;
	}

	// Token: 0x06000B26 RID: 2854 RVA: 0x00051774 File Offset: 0x0004F974
	private Message _receive()
	{
		if (RandomUtil.random() < -0.001f)
		{
			Log.error("RANDOMLY DISCONNECTING SOCKET");
			this.socket.Close();
			return null;
		}
		if (this.IsConnected())
		{
			this._readSocketCompletely();
		}
		return this.msgParser.nextMessage();
	}

	// Token: 0x06000B27 RID: 2855 RVA: 0x000517C4 File Offset: 0x0004F9C4
	private void _readSocketCompletely()
	{
		SocketError socketError;
		for (;;)
		{
			socketError = 0;
			int num = this.socket.Receive(this.socketBuf, 0, this.socketBuf.Length, 0, ref socketError);
			if (num > 0)
			{
				int chars = this.msgDecoder.GetChars(this.socketBuf, 0, num, this.decodeBuf, 0, true);
				GameSocket._replaceInvalidCharacters(this.decodeBuf, chars);
				string text = new string(this.decodeBuf, 0, chars);
				this.msgParser.feed(text);
				Log.info(string.Concat(new object[]
				{
					"Read ",
					chars,
					" b: ",
					text.Substring(0, Math.Min(65, text.Length)),
					"... from ",
					this.socket.RemoteEndPoint.ToString()
				}));
			}
			if (socketError != null && socketError != 10035)
			{
				break;
			}
			if (num <= 0)
			{
				return;
			}
		}
		this.closeAndPostDisconnect();
		throw new SocketException(socketError);
	}

	// Token: 0x06000B28 RID: 2856 RVA: 0x00009563 File Offset: 0x00007763
	public bool IsConnected()
	{
		return this.socket != null && this.socket.Connected && GameSocket.pollIsConnected(this.socket);
	}

	// Token: 0x06000B29 RID: 2857 RVA: 0x0000958D File Offset: 0x0000778D
	public void Update()
	{
		if (this.pendingReconnect == 20)
		{
			this.closeAndPostDisconnect();
		}
		this.updateConnectionCheck();
	}

	// Token: 0x06000B2A RID: 2858 RVA: 0x000518BC File Offset: 0x0004FABC
	private void updateConnectionCheck()
	{
		if (this.socket == null && !this.hasBeenConnected)
		{
			return;
		}
		bool flag = this.IsConnected();
		if (flag == this.wasConnectedLastFrame)
		{
			return;
		}
		this.wasConnectedLastFrame = flag;
		if (!flag)
		{
			this.pendingReconnect = 1;
		}
	}

	// Token: 0x06000B2B RID: 2859 RVA: 0x00051908 File Offset: 0x0004FB08
	private static void _replaceInvalidCharacters(char[] buffer, int length)
	{
		int num = buffer.Length;
		if (length >= 0)
		{
			num = Math.Min(num, length);
		}
		for (int i = 0; i < num; i++)
		{
			if (buffer[i] == '\u007f')
			{
				buffer[i] = '#';
			}
			if (buffer[i] == '\b')
			{
				buffer[i] = '#';
			}
		}
	}

	// Token: 0x06000B2C RID: 2860 RVA: 0x00051958 File Offset: 0x0004FB58
	private static bool pollIsConnected(Socket socket)
	{
		bool result;
		try
		{
			result = (socket != null && (!socket.Poll(1, 0) || socket.Available != 0));
		}
		catch (SocketException)
		{
			result = false;
		}
		return result;
	}

	// Token: 0x04000883 RID: 2179
	private const int BufSize = 65536;

	// Token: 0x04000884 RID: 2180
	private static readonly int TimeoutMilliseconds = 20000;

	// Token: 0x04000885 RID: 2181
	private ISocketListener listener;

	// Token: 0x04000886 RID: 2182
	private Socket socket;

	// Token: 0x04000887 RID: 2183
	private IpPort address;

	// Token: 0x04000888 RID: 2184
	private bool hasBeenConnected;

	// Token: 0x04000889 RID: 2185
	private bool wasConnectedLastFrame;

	// Token: 0x0400088A RID: 2186
	private Decoder msgDecoder = Encoding.UTF8.GetDecoder();

	// Token: 0x0400088B RID: 2187
	private ThreadedMessageParser msgParser = new ThreadedMessageParser().start();

	// Token: 0x0400088C RID: 2188
	private byte[] socketBuf = new byte[65536];

	// Token: 0x0400088D RID: 2189
	private char[] decodeBuf = new char[65536];

	// Token: 0x0400088E RID: 2190
	private int pendingReconnect;
}
