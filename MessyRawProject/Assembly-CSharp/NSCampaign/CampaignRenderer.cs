using System;
using System.Collections;
using UnityEngine;

namespace NSCampaign
{
	// Token: 0x020000E5 RID: 229
	public class CampaignRenderer : MonoBehaviour
	{
		// Token: 0x06000799 RID: 1945 RVA: 0x00042B68 File Offset: 0x00040D68
		public void init(Campaign campaign)
		{
			this.campaign = campaign;
			Vector4 limits = campaign.level.field.getLimits();
			CampaignRenderer.setTileWidth((float)(Screen.width / 4));
			this.mapLimits = new Vector4(CampaignRenderer.getXfor(limits.x, limits.z), CampaignRenderer.getXfor(limits.y, limits.w), CampaignRenderer.getYfor(limits.x, limits.z), CampaignRenderer.getYfor(limits.y, limits.w));
			this.createPlayer();
			this.createMobs();
			this.setupBoard(1f);
			this.refresh(false);
			float num = (this.mapLimits.x + this.mapLimits.y) / 2f;
			float num2 = (this.mapLimits.z + this.mapLimits.w) / 2f;
			Camera.main.transform.position = new Vector3(num, 10f, num2);
		}

		// Token: 0x0600079A RID: 1946 RVA: 0x00042C68 File Offset: 0x00040E68
		private void setupBoard(float zoom)
		{
			if (this.graphicsField != null)
			{
				this.graphicsField.destroy();
			}
			this.setZoom(zoom);
			Vector3 vector = Camera.main.ScreenToWorldPoint(new Vector3(0f, 0f, 0f));
			Vector3 vector2 = Camera.main.ScreenToWorldPoint(new Vector3((float)Screen.width, (float)Screen.height, 0f));
			int num = 2 + (int)((vector2.x - vector.x) / (CampaignRenderer.TileWidth * this.ZoomOutMax));
			int num2 = 3 + (int)((vector2.z - vector.z) / (CampaignRenderer.TileHeight * this.ZoomOutMax));
			this.graphicsField = base.gameObject.AddComponent<GraphicsField>();
			this.graphicsField.init(num, num2);
			Log.info(string.Concat(new object[]
			{
				"p0, p1: ",
				vector,
				", ",
				vector2,
				" : ",
				num,
				", ",
				num2
			}));
			for (int i = 0; i < num2; i++)
			{
				for (int j = 0; j < num; j++)
				{
					float xfor = CampaignRenderer.getXfor(j, i);
					float yfor = CampaignRenderer.getYfor(j, i);
					GameObject gameObject = PrimitiveFactory.createPlane();
					gameObject.transform.position = new Vector3(xfor, 0f, yfor);
					gameObject.transform.localScale = new Vector3(0.17140263f, 1f, 0.2f);
					gameObject.transform.eulerAngles = new Vector3(0f, 180f, 0f);
					GraphicsTile graphicsTile = gameObject.gameObject.AddComponent<GraphicsTile>();
					graphicsTile.init(j, i, this);
					this.graphicsField.tiles[j, i] = graphicsTile;
				}
			}
		}

		// Token: 0x0600079B RID: 1947 RVA: 0x00042E60 File Offset: 0x00041060
		private void createPlayer()
		{
			int xPos = 3;
			int zPos = 3;
			Material material = new Material(ResourceManager.LoadShader("Transparent/Diffuse"));
			material.mainTexture = ResourceManager.LoadTexture("Campaign/player");
			material.renderQueue = 999;
			this.playerGo = GameObject.CreatePrimitive(4);
			this.playerGo.name = "Player";
			this.playerGo.transform.localScale = new Vector3(0.05f, 0.1f, 0.09f);
			this.playerGo.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
			this.playerGo.renderer.material = material;
			this.player = this.playerGo.AddComponent<GraphicsPlayer>();
			this.player.init(this, xPos, zPos);
		}

		// Token: 0x0600079C RID: 1948 RVA: 0x00042F30 File Offset: 0x00041130
		private void createMobs()
		{
			this.mobsGOArr = new GameObject[this.campaign.mobArr.Length];
			for (int i = 0; i < this.campaign.mobArr.Length; i++)
			{
				Material material = new Material(ResourceManager.LoadShader("Transparent/Diffuse"));
				material.mainTexture = ResourceManager.LoadTexture("Campaign/player");
				material.renderQueue = 999;
				GameObject gameObject = GameObject.CreatePrimitive(4);
				gameObject.name = "Mob_" + i;
				gameObject.transform.localScale = new Vector3(0.05f, 0.1f, 0.09f);
				gameObject.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
				gameObject.renderer.material = material;
				this.mobsGOArr[i] = gameObject;
			}
		}

		// Token: 0x0600079D RID: 1949 RVA: 0x00043010 File Offset: 0x00041210
		public void movePlayer(int x, int y, bool encounter)
		{
			Hashtable hashtable = new Hashtable();
			hashtable.Add("x", x);
			hashtable.Add("y", y);
			hashtable.Add("encounter", encounter);
			iTween.MoveTo(this.playerGo, iTween.Hash(new object[]
			{
				"x",
				CampaignRenderer.getXfor(x, y),
				"y",
				-0.1f,
				"z",
				CampaignRenderer.getYfor(x, y),
				"easetype",
				iTween.EaseType.easeOutExpo,
				"time",
				0.9f,
				"oncompletetarget",
				this.campaign.gameObject,
				"oncomplete",
				"movePlayerDone",
				"oncompleteparams",
				hashtable
			}));
		}

		// Token: 0x0600079E RID: 1950 RVA: 0x00043110 File Offset: 0x00041310
		public void update()
		{
			if (this.campaign == null)
			{
				return;
			}
			bool flag = true;
			if (flag)
			{
				this.refresh(true);
			}
			if (this.playerGo.GetComponent<iTween>() == null)
			{
				this.playerGo.transform.position = new Vector3(CampaignRenderer.getXfor(this.campaign.player.x, this.campaign.player.y), -0.1f, CampaignRenderer.getYfor(this.campaign.player.x, this.campaign.player.y));
			}
			this.playerGo.renderer.material.renderQueue = -1;
			for (int i = 0; i < this.mobsGOArr.Length; i++)
			{
				this.mobsGOArr[i].transform.position = new Vector3(CampaignRenderer.getXfor(this.campaign.mobArr[i].x, this.campaign.mobArr[i].y), -0.1f, CampaignRenderer.getYfor(this.campaign.mobArr[i].x, this.campaign.mobArr[i].y));
			}
		}

		// Token: 0x0600079F RID: 1951 RVA: 0x00043258 File Offset: 0x00041458
		private void FixedUpdate()
		{
			float num = 0.1f / this.getZoom();
			float num2 = 0f;
			float num3 = 0f;
			if (Input.GetKey(276))
			{
				num2 -= num;
			}
			if (Input.GetKey(275))
			{
				num2 += num;
			}
			if (Input.GetKey(274))
			{
				num3 -= num;
			}
			if (Input.GetKey(273))
			{
				num3 += num;
			}
			if (Input.GetKey(270))
			{
				this.setZoom(this.getZoom() * 1.02f);
			}
			if (Input.GetKey(269))
			{
				this.setZoom(this.getZoom() / 1.02f);
			}
			if (num2 != 0f || num3 != 0f)
			{
				if (num2 != 0f && num3 != 0f)
				{
					num2 *= 0.70710677f;
					num3 *= 0.70710677f;
				}
				this.moveCamera(num2, num3);
			}
		}

		// Token: 0x060007A0 RID: 1952 RVA: 0x00006CBA File Offset: 0x00004EBA
		public float getZoom()
		{
			return this._camZoom;
		}

		// Token: 0x060007A1 RID: 1953 RVA: 0x00043350 File Offset: 0x00041550
		public void setZoom(float zoom)
		{
			float camZoom = this._camZoom;
			this._camZoom = Mth.clamp(zoom, this.ZoomOutMax, this.ZoomInMax);
			Camera.main.orthographicSize = (float)Screen.width / this._camZoom;
		}

		// Token: 0x060007A2 RID: 1954 RVA: 0x00043394 File Offset: 0x00041594
		private void refresh(bool translate)
		{
			Vector3 world = Camera.main.ScreenToWorldPoint(Vector3.zero);
			IntCoord intCoord = CampaignRenderer.approxWorldToTile(world);
			if ((intCoord.y & 1) == 1)
			{
				intCoord.y--;
			}
			translate = (translate && !intCoord.Equals(this.lpos));
			IntCoord intCoord2 = intCoord - this.lpos;
			this.lpos = intCoord;
			for (int i = 0; i < this.graphicsField.width; i++)
			{
				int num = intCoord.x + i;
				for (int j = 0; j < this.graphicsField.height; j++)
				{
					int num2 = intCoord.y + j;
					GraphicsTile graphicsTile = this.graphicsField.tiles[i, j];
					if (translate)
					{
						graphicsTile.gameObject.transform.Translate(CampaignRenderer.getXfor(0, 0) - CampaignRenderer.getXfor(intCoord2.x, 0), 0f, CampaignRenderer.getYfor(0, 0) - CampaignRenderer.getYfor(0, intCoord2.y));
					}
					if (!this.campaign.field.inRange(num, num2))
					{
						graphicsTile.setTile(num, num2, null);
					}
					else
					{
						int num3 = num - graphicsTile.currentX;
						int num4 = num2 - graphicsTile.currentY;
						graphicsTile.setTile(num, num2, this.campaign.field.tiles[num, num2]);
						int num5 = graphicsTile.currentX - graphicsTile.baseX;
						int num6 = graphicsTile.currentY - graphicsTile.baseY;
					}
				}
			}
		}

		// Token: 0x060007A3 RID: 1955 RVA: 0x00043544 File Offset: 0x00041744
		public void moveCamera(float dx, float dy)
		{
			float num = Mth.clamp(this._camX + dx, -100000f, 100000f + this.mapLimits.y);
			float num2 = Mth.clamp(this._camY + dy, -100000f, 100000f + this.mapLimits.w);
			Camera.main.transform.Translate(num - this._camX, num2 - this._camY, 0f);
			this._camX = num;
			this._camY = num2;
		}

		// Token: 0x060007A4 RID: 1956 RVA: 0x00006CC2 File Offset: 0x00004EC2
		public void clicked(int tileX, int tileY)
		{
			this.campaign.tryMovePlayer(tileX, tileY);
		}

		// Token: 0x060007A5 RID: 1957 RVA: 0x00006CD1 File Offset: 0x00004ED1
		private static void setTileWidth(float width)
		{
			CampaignRenderer.TileWidth = width;
			CampaignRenderer.TileHeight = width * 0.5f / 1.4f;
		}

		// Token: 0x060007A6 RID: 1958 RVA: 0x000435CC File Offset: 0x000417CC
		private static IntCoord approxWorldToTile(Vector3 world)
		{
			int x = Mth.iFloor(world.x / CampaignRenderer.TileWidth);
			int y = Mth.iFloor(world.z / CampaignRenderer.TileHeight);
			return new IntCoord(x, y);
		}

		// Token: 0x060007A7 RID: 1959 RVA: 0x00006CEB File Offset: 0x00004EEB
		private static float getXfor(float x, float y)
		{
			return CampaignRenderer.getXfor(Mth.iFloor(x), Mth.iFloor(y));
		}

		// Token: 0x060007A8 RID: 1960 RVA: 0x00006CFE File Offset: 0x00004EFE
		private static float getXfor(int x, int y)
		{
			return CampaignRenderer.TileWidth * (float)x + (float)(y % 2) * (CampaignRenderer.TileWidth / 2f);
		}

		// Token: 0x060007A9 RID: 1961 RVA: 0x00006D19 File Offset: 0x00004F19
		private static float getYfor(float x, float y)
		{
			return CampaignRenderer.getYfor(Mth.iFloor(x), Mth.iFloor(y));
		}

		// Token: 0x060007AA RID: 1962 RVA: 0x00006D2C File Offset: 0x00004F2C
		private static float getYfor(int x, int y)
		{
			return CampaignRenderer.TileHeight * (float)y;
		}

		// Token: 0x060007AB RID: 1963 RVA: 0x00006D36 File Offset: 0x00004F36
		public static int getRenderQueueForRow(int row)
		{
			return 1000 - row;
		}

		// Token: 0x040005B3 RID: 1459
		private Campaign campaign;

		// Token: 0x040005B4 RID: 1460
		private GraphicsField graphicsField;

		// Token: 0x040005B5 RID: 1461
		private GraphicsPlayer player;

		// Token: 0x040005B6 RID: 1462
		private Vector4 mapLimits;

		// Token: 0x040005B7 RID: 1463
		private GameObject playerGo;

		// Token: 0x040005B8 RID: 1464
		private GameObject[] mobsGOArr;

		// Token: 0x040005B9 RID: 1465
		private float ZoomInMax = 4f;

		// Token: 0x040005BA RID: 1466
		private float ZoomOutMax = 0.5f;

		// Token: 0x040005BB RID: 1467
		private float _camZoom = 1f;

		// Token: 0x040005BC RID: 1468
		private float _camX;

		// Token: 0x040005BD RID: 1469
		private float _camY;

		// Token: 0x040005BE RID: 1470
		private IntCoord lpos = new IntCoord(0, 0);

		// Token: 0x040005BF RID: 1471
		private static float TileWidth = 1.4f;

		// Token: 0x040005C0 RID: 1472
		private static float TileHeight = 0.5f;
	}
}
