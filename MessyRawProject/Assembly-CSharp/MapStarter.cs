using System;
using System.Collections.Generic;

// Token: 0x020000E1 RID: 225
internal class MapStarter
{
	// Token: 0x0600078F RID: 1935 RVA: 0x00006BFE File Offset: 0x00004DFE
	public MapStarter(Communicator comm) : this(comm, null)
	{
	}

	// Token: 0x06000790 RID: 1936 RVA: 0x00006C08 File Offset: 0x00004E08
	public MapStarter(Communicator comm, string wantedMap)
	{
		this.comm = comm;
		this.wantedMapName = wantedMap;
		comm.sendRequest(new AdventureMapListMessage());
	}

	// Token: 0x06000791 RID: 1937 RVA: 0x00042A00 File Offset: 0x00040C00
	public void handle(Message msg)
	{
		if (msg is AdventureMapListMessage)
		{
			AdventureMapListMessage adventureMapListMessage = (AdventureMapListMessage)msg;
			this.maps.Clear();
			bool flag = false;
			foreach (AdventureMapInfo adventureMapInfo in adventureMapListMessage.maps)
			{
				if (this.wantedMapName != null && adventureMapInfo.name.ToLower() == this.wantedMapName.ToLower())
				{
					this.wantedMapName = adventureMapInfo.name;
					flag = true;
				}
				this.maps.Add(adventureMapInfo);
			}
			if (flag)
			{
				this.comm.sendRequest(new AdventureStartMessage(this.wantedMapName));
			}
			else
			{
				this.comm.sendRequest(new AdventureCurrentMessage());
			}
		}
		if (msg is AdventureCurrentMessage)
		{
			string text = ((AdventureCurrentMessage)msg).mapName;
			if (text == string.Empty && this.maps.Count > 0)
			{
				text = this.maps[0].name;
			}
			if (text != string.Empty)
			{
				this.comm.sendRequest(new AdventureStartMessage(text));
			}
		}
		if (msg is OkMessage && ((OkMessage)msg).op == "AdventureStart")
		{
			this._done = true;
		}
	}

	// Token: 0x06000792 RID: 1938 RVA: 0x00006C35 File Offset: 0x00004E35
	public bool isDone()
	{
		return this._done;
	}

	// Token: 0x040005A1 RID: 1441
	private bool _done;

	// Token: 0x040005A2 RID: 1442
	private Communicator comm;

	// Token: 0x040005A3 RID: 1443
	private List<AdventureMapInfo> maps = new List<AdventureMapInfo>();

	// Token: 0x040005A4 RID: 1444
	private string wantedMapName;
}
