using System;
using System.Collections;
using System.Collections.Generic;
using Gui;
using NSCampaign;
using UnityEngine;

// Token: 0x020000D9 RID: 217
public class Campaign : AbstractCommListener, IOkCallback
{
	// Token: 0x0600075A RID: 1882 RVA: 0x000418A0 File Offset: 0x0003FAA0
	private void Start()
	{
		App.LobbyMenu.SetEnabled(false);
		this.comm = App.Communicator;
		this.comm.addListener(this);
		this.mapStarter = new MapStarter(this.comm, "Aron map");
		this.labelSkin = (GUISkin)ResourceManager.Load("_GUISkins/Campaign");
		int fontSize = (int)(40f * (float)Screen.height / 1280f);
		this.labelSkin.label.fontSize = fontSize;
		this.gui3d = new Gui3D(Camera.main);
		HexUtil.SetTileWidth((float)(Screen.height / 6));
	}

	// Token: 0x0600075B RID: 1883 RVA: 0x00041940 File Offset: 0x0003FB40
	private void discoverTile(int x, int y, int id, bool fadeIn)
	{
		FieldTile fieldTile = this.field.discoverTile(x, y, id);
		if (fieldTile != null)
		{
			if (fadeIn)
			{
				base.StartCoroutine(this.FadeTile(fieldTile));
			}
			else
			{
				fieldTile.alpha = 1f;
			}
		}
	}

	// Token: 0x0600075C RID: 1884 RVA: 0x00041988 File Offset: 0x0003FB88
	private IEnumerator FadeTile(FieldTile tile)
	{
		float t = 0f;
		while (t < 1f)
		{
			t += Time.deltaTime * 2f;
			if (t > 1f)
			{
				t = 1f;
			}
			tile.alpha = t * t * (3f - 2f * t);
			yield return null;
		}
		yield break;
	}

	// Token: 0x0600075D RID: 1885 RVA: 0x000419AC File Offset: 0x0003FBAC
	public override void handleMessage(Message msg)
	{
		if (msg is DeckListMessage)
		{
			this.decksArr = new List<DeckInfo>(((DeckListMessage)msg).GetAllDecks());
			App.InviteManager.setDeckList(this.decksArr);
		}
		if (msg is GameInfoMessage)
		{
			SceneLoader.loadScene("_BattleModeView");
		}
		if (msg is OkMessage && ((OkMessage)msg).op == "AdventureStart")
		{
			this.comm.send(new AdventureMapDataMessage());
		}
		if (msg is AdventureMapDataMessage)
		{
			AdventureMapDataMessage adventureMapDataMessage = (AdventureMapDataMessage)msg;
			this.field = new Field(adventureMapDataMessage.width, adventureMapDataMessage.height);
			this.tilePrototypes = new Dictionary<int, TilePrototype>();
			foreach (TilePrototype tilePrototype in adventureMapDataMessage.tileTypes)
			{
				this.tilePrototypes.Add(tilePrototype.id, tilePrototype);
				GraphicsTileMaterialCache.setTileGraphics(tilePrototype.id, tilePrototype.bitmap);
			}
			this.mobPrototypeArr = adventureMapDataMessage.mobTypes;
			this.comm.send(new AdventureDataMessage());
		}
		if (msg is AdventureMoveResponseMessage)
		{
			AdventureMoveResponseMessage adventureMoveResponseMessage = (AdventureMoveResponseMessage)msg;
			this.playerRenderer.MoveFrom();
			this.player.x = adventureMoveResponseMessage.position.x;
			this.player.y = adventureMoveResponseMessage.position.y;
			foreach (MapPos mapPos in adventureMoveResponseMessage.discovered)
			{
				this.discoverTile(mapPos.x, mapPos.y, mapPos.tileId, true);
			}
			this.walkable = adventureMoveResponseMessage.walkable;
			foreach (Mob mob in this.mobArr)
			{
				if (mob.x == this.player.x && mob.y == this.player.y)
				{
					App.GameActionManager.StartGame(GameActionManager.StartType.START_SINGLEPLAYER_ADVENTURE);
				}
			}
		}
		if (msg is AdventureDataMessage)
		{
			AdventureDataMessage adventureDataMessage = (AdventureDataMessage)msg;
			this.player = new Player(adventureDataMessage.position.x, adventureDataMessage.position.y);
			this.walkable = adventureDataMessage.walkable;
			this.avatarInfo = adventureDataMessage.getAvatar();
			this.avatarTextures = Avatar.Create(this.avatarInfo, 0);
			this.playerRenderer = new UnitRenderer(this.player);
			this.level = new Level(this.field);
			this.level.addPlayer(this.player);
			this.mobArr = (Mob[])adventureDataMessage.mobs.Clone();
			foreach (Mob mob2 in this.mobArr)
			{
				mob2.mobPrototype = this.getMobPrototype(mob2.mobTypeId);
				this.level.addMob(mob2);
			}
			foreach (MapPos mapPos2 in adventureDataMessage.discovered)
			{
				this.discoverTile(mapPos2.x, mapPos2.y, mapPos2.tileId, false);
			}
		}
		if (this.mapStarter != null)
		{
			this.mapStarter.handle(msg);
			if (this.mapStarter.isDone())
			{
				this.mapStarter = null;
			}
		}
		base.handleMessage(msg);
	}

	// Token: 0x0600075E RID: 1886 RVA: 0x00041D38 File Offset: 0x0003FF38
	private void Update()
	{
		this.cam = Vector2.Lerp(this.cam, this.camSeek, 0.1f);
		this.zoom = Mathf.Lerp(this.zoom, this.zoomSeek, 0.1f);
		HexUtil.SetTileWidth((float)Screen.height / this.zoom);
		if (this.playerRenderer != null)
		{
			this.playerRenderer.Update();
			this.camSeek = new Vector2(this.playerRenderer.VX() / ((float)this.field.width * HexUtil.TileWidth), this.playerRenderer.VY() / ((float)this.field.height * HexUtil.TileWidth));
		}
		this.zoomSeek = Mathf.Clamp(this.zoomSeek - Input.GetAxis("Mouse ScrollWheel") * 10f, 4f, 10f);
		this.gui3d.frameBegin();
		if (this.field != null)
		{
			float num = (float)this.field.width * HexUtil.TileWidth;
			float num2 = (float)this.field.height * HexUtil.TileWidth;
			float num3 = this.cam.x * num - (float)(Screen.width / 2) + HexUtil.TileWidth / 2f;
			float num4 = this.cam.y * num2 - (float)(Screen.height / 2) + HexUtil.TileHeight * 2f;
			float num5 = 0.7827586f;
			float num6 = HexUtil.TileWidth * 1.25f;
			float num7 = num6 / num5;
			int num8 = Mathf.Max(0, (int)(num4 / HexUtil.TileHeight) - 3);
			int num9 = Mathf.Min(this.field.width, num8 + (int)((float)Screen.height / HexUtil.TileHeight) + 4);
			int num10 = Mathf.Max(0, (int)(num3 / HexUtil.TileWidth) - 1);
			int num11 = Mathf.Min(this.field.height, num10 + (int)((float)Screen.width / HexUtil.TileWidth) + 3);
			Vector2 screenMousePos = GUIUtil.getScreenMousePos();
			this.gui3d.setDepth(1f);
			this.gui3d.DrawTexture(new Rect(0f, (float)Screen.height * 0.7f, (float)Screen.width, (float)Screen.height * 0.3f), ResourceManager.LoadTexture("BattleUI/battlegui_gradient_white"));
			this.gui3d.GetLastMaterial().color = this.gradientColor;
			if (this.playerRenderer != null && this.avatarTextures != null)
			{
				float num12 = (float)Screen.height * 0.6f;
				float num13 = num12 * 0.567f;
				this.gui3d.setDepth(0.5f);
				this.avatarTextures.draw(this.gui3d, new Rect(0f - num12 * 0.2f, (float)Screen.height - num12 * 0.6f, num13, num12), Color.white, false);
				this.gui3d.GetLastMaterial().shader = ResourceManager.LoadShader("Unlit/Transparent");
			}
			for (int i = num8; i < num9; i++)
			{
				this.gui3d.setDepth(HexUtil.GetZ(i));
				for (int j = num10; j < num11; j++)
				{
					if (this.field.isDiscovered(j, i))
					{
						FieldTile fieldTile = this.field.tiles[j, i];
						float num14 = HexUtil.GetXFor(j, i) - num3;
						float num15 = HexUtil.GetYFor(j, i) - num4;
						Vector2 center;
						center..ctor(num14 + num6 / 2f, num15 + num7 * 2f / 3f);
						if (num14 + num6 > 0f && num14 < (float)Screen.width && num15 + num7 > 0f && num15 < (float)Screen.height)
						{
							if (Input.GetMouseButtonDown(0) && this.tileHovered(screenMousePos, center))
							{
								Mob mob = this.findMob(j, i);
								if (mob != null)
								{
									App.Popups.ShowOk(this, "mobpopup", mob.mobPrototype.name, mob.mobPrototype.name, "Ok");
								}
								App.Communicator.send(new AdventureMoveMessage(j, i));
							}
							this.gui3d.DrawTexture(new Rect(num14, num15, num6, num7), GraphicsTileMaterialCache.getTexture(fieldTile));
							this.gui3d.GetLastMaterial().shader = ResourceManager.LoadShader("Scrolls/Unlit/Transparent");
							this.gui3d.GetLastMaterial().SetColor("_Color", new Color(0.5f, 0.5f, 0.5f, fieldTile.alpha));
						}
						if (this.player != null)
						{
							if (this.isWalkable(j, i))
							{
								this.gui3d.DrawTexture(new Rect(num14, num15, num6, num7), GraphicsTileMaterialCache.getTexture(fieldTile));
								this.gui3d.GetLastMaterial().shader = ResourceManager.LoadShader("Particles/Additive");
								float num16 = 0.2f + 0.08f * Mathf.Sin(Time.time * 6f);
								this.gui3d.GetLastMaterial().SetColor("_TintColor", new Color(0.4f, 0.4f, 0.4f, num16));
							}
							if (j == this.player.x && i == this.player.y)
							{
								this.gui3d.DrawTexture(new Rect(num14, num15, num6, num7), GraphicsTileMaterialCache.getTexture(fieldTile));
								this.gui3d.GetLastMaterial().shader = ResourceManager.LoadShader("Particles/Additive");
								this.gui3d.GetLastMaterial().SetColor("_TintColor", new Color(0.4f, 0.4f, 0.4f, 0.35f));
							}
						}
					}
				}
			}
			if (this.mobArr != null)
			{
				foreach (Mob mob2 in this.mobArr)
				{
					if (!this.field.isDiscovered(mob2.x, mob2.y))
					{
					}
					this.gui3d.setDepth(HexUtil.GetZ(mob2.y));
					this.gui3d.DrawTexture(new Rect(HexUtil.GetXFor(mob2.x, mob2.y) - num3 + HexUtil.TileWidth * 0.35f, HexUtil.GetYFor(mob2.x, mob2.y) - num4 + HexUtil.TileHeight * 1f, HexUtil.TileWidth / 2f, HexUtil.TileWidth / 2f * 167f / 93f), ResourceManager.LoadTexture("Campaign/player"));
					this.gui3d.GetLastMaterial().shader = ResourceManager.LoadShader("Unlit/Transparent");
				}
			}
			if (this.playerRenderer != null)
			{
				this.gui3d.setDepth(this.playerRenderer.VZ());
				this.gui3d.GetLastMaterial().shader = ResourceManager.LoadShader("Unlit/Transparent");
				if (this.avatarTextures != null)
				{
					this.avatarTextures.draw(this.gui3d, new Rect(this.playerRenderer.VX() - num3 + HexUtil.TileWidth * 0.15f, this.playerRenderer.VY() - num4 - HexUtil.TileHeight * 0.5f, HexUtil.TileWidth * 0.8f, HexUtil.TileWidth * 0.8f / 0.567f), Color.white, true);
				}
			}
		}
		this.gui3d.frameEnd();
	}

	// Token: 0x0600075F RID: 1887 RVA: 0x000424C4 File Offset: 0x000406C4
	public Mob findMob(int x, int y)
	{
		foreach (Mob mob in this.mobArr)
		{
			if (mob.x == x && mob.y == y)
			{
				return mob;
			}
		}
		return null;
	}

	// Token: 0x06000760 RID: 1888 RVA: 0x0004250C File Offset: 0x0004070C
	public bool isWalkable(int x, int y)
	{
		if (this.walkable != null)
		{
			foreach (Pos pos in this.walkable)
			{
				if (pos.x == x && pos.y == y)
				{
					return true;
				}
			}
		}
		return false;
	}

	// Token: 0x06000761 RID: 1889 RVA: 0x00042560 File Offset: 0x00040760
	public bool tileHovered(Vector2 screenCoords, Vector2 center)
	{
		float num = HexUtil.TileWidth / 2f;
		float num2 = HexUtil.TileWidth / 2f * 0.5f;
		return Mathf.Pow(screenCoords.x - center.x, 2f) / (num * num) + Mathf.Pow(screenCoords.y - center.y, 2f) / (num2 * num2) <= 1f;
	}

	// Token: 0x06000762 RID: 1890 RVA: 0x000425D0 File Offset: 0x000407D0
	public MobPrototype getMobPrototype(int prototypeId)
	{
		for (int i = 0; i < this.mobPrototypeArr.Length; i++)
		{
			if (prototypeId == this.mobPrototypeArr[i].id)
			{
				return this.mobPrototypeArr[i];
			}
		}
		return null;
	}

	// Token: 0x06000763 RID: 1891 RVA: 0x000068B1 File Offset: 0x00004AB1
	public void tryMovePlayer(int tileX, int tileY)
	{
		this.comm.sendRequest(new AdventureMoveMessage(tileX, tileY));
	}

	// Token: 0x06000764 RID: 1892 RVA: 0x00042614 File Offset: 0x00040814
	public void movePlayerDone(Hashtable values)
	{
		this.player.x = (int)values["x"];
		this.player.y = (int)values["y"];
		App.GameActionManager.StartGame(GameActionManager.StartType.START_SINGLEPLAYER_ADVENTURE);
	}

	// Token: 0x06000765 RID: 1893 RVA: 0x000028DF File Offset: 0x00000ADF
	public void PopupOk(string popupType)
	{
	}

	// Token: 0x04000576 RID: 1398
	public Level level;

	// Token: 0x04000577 RID: 1399
	public Field field;

	// Token: 0x04000578 RID: 1400
	public Player player;

	// Token: 0x04000579 RID: 1401
	public Mob[] mobArr;

	// Token: 0x0400057A RID: 1402
	public MobPrototype[] mobPrototypeArr;

	// Token: 0x0400057B RID: 1403
	private GUISkin labelSkin;

	// Token: 0x0400057C RID: 1404
	private Communicator comm;

	// Token: 0x0400057D RID: 1405
	private List<DeckInfo> decksArr = new List<DeckInfo>();

	// Token: 0x0400057E RID: 1406
	private MapStarter mapStarter;

	// Token: 0x0400057F RID: 1407
	private Gui3D gui3d;

	// Token: 0x04000580 RID: 1408
	private UnitRenderer playerRenderer;

	// Token: 0x04000581 RID: 1409
	private AvatarInfo avatarInfo;

	// Token: 0x04000582 RID: 1410
	private Avatar avatarTextures;

	// Token: 0x04000583 RID: 1411
	private Dictionary<int, TilePrototype> tilePrototypes;

	// Token: 0x04000584 RID: 1412
	private Pos[] walkable;

	// Token: 0x04000585 RID: 1413
	public Color gradientColor = new Color(0f, 0f, 0f, 0.5f);

	// Token: 0x04000586 RID: 1414
	private Vector2 cam = new Vector2(0f, 0f);

	// Token: 0x04000587 RID: 1415
	private Vector2 camSeek = new Vector2(0f, 0f);

	// Token: 0x04000588 RID: 1416
	private float zoom = 6f;

	// Token: 0x04000589 RID: 1417
	private float zoomSeek = 6f;
}
