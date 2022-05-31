using System;
using UnityEngine;

// Token: 0x0200009B RID: 155
public class Tile : MonoBehaviour
{
	// Token: 0x0600057C RID: 1404 RVA: 0x000057D8 File Offset: 0x000039D8
	public void init(BattleMode battleMode, bool isLeft, TileColor color, int row, int column)
	{
		this._battleMode = battleMode;
		this._isLeft = isLeft;
		this._color = color;
		this._row = row;
		this._column = column;
	}

	// Token: 0x0600057D RID: 1405 RVA: 0x000057FF File Offset: 0x000039FF
	public int row()
	{
		return this._row;
	}

	// Token: 0x0600057E RID: 1406 RVA: 0x00005807 File Offset: 0x00003A07
	public int column()
	{
		return this._column;
	}

	// Token: 0x0600057F RID: 1407 RVA: 0x0000580F File Offset: 0x00003A0F
	public bool isLeft()
	{
		return this._isLeft;
	}

	// Token: 0x06000580 RID: 1408 RVA: 0x00005817 File Offset: 0x00003A17
	public bool isRight()
	{
		return !this.isLeft();
	}

	// Token: 0x06000581 RID: 1409 RVA: 0x00005822 File Offset: 0x00003A22
	public TileColor color()
	{
		return this._color;
	}

	// Token: 0x06000582 RID: 1410 RVA: 0x0000582A File Offset: 0x00003A2A
	public TilePosition tilePosition()
	{
		return new TilePosition(this._color, this._row, this._column);
	}

	// Token: 0x06000583 RID: 1411 RVA: 0x000398F0 File Offset: 0x00037AF0
	private void FixedUpdate()
	{
		if (++this.subFrame >= 3)
		{
			this.subFrame = 0;
		}
		if (this.subFrame != 1)
		{
			return;
		}
		this.tileAnimationProgress++;
		if (this.markerType == Tile.SelectionType.Target)
		{
			this.referenceTile.renderer.material.color = new Color(1f, 1f, 1f, 0.4f);
			this.referenceTile.renderer.material.mainTexture = ResourceManager.LoadTexture("BattleMode/Tiles/HexTarget/" + (1 + this.tileAnimationProgress % 9));
		}
		else if (this.markerType == Tile.SelectionType.SelectedMove)
		{
			this.referenceTile.renderer.material.color = new Color(1f, 1f, 1f, 0.8f);
			this.referenceTile.renderer.material.mainTexture = ResourceManager.LoadTexture("BattleMode/Tiles/HexSelected/" + (1 + this.tileAnimationProgress % 23));
		}
		else if (this.markerType == Tile.SelectionType.Path)
		{
			this.referenceTile.renderer.material.color = new Color(1f, 1f, 1f, 0.4f);
			this.referenceTile.renderer.material.mainTexture = ResourceManager.LoadTexture("BattleMode/Tiles/HexPath/" + (1 + this.tileAnimationProgress % 11));
		}
		else if (this.markerType == Tile.SelectionType.Hover)
		{
			this.referenceTile.renderer.material.color = new Color(1f, 1f, 1f, 0.4f);
			this.referenceTile.renderer.material.mainTexture = ResourceManager.LoadTexture("BattleMode/Tiles/HexHover/" + (1 + this.tileAnimationProgress % 11));
		}
		else if (this.markerType == Tile.SelectionType.HoverStatic)
		{
			this.referenceTile.renderer.material.color = new Color(1f, 1f, 1f, 0.4f);
			this.referenceTile.renderer.material.mainTexture = ResourceManager.LoadTexture("BattleMode/Tiles/HexHover/" + 1);
		}
		this.updateMoveAnim();
	}

	// Token: 0x06000584 RID: 1412 RVA: 0x00039B6C File Offset: 0x00037D6C
	private void updateMoveAnim()
	{
		if (this.moveAnimationRotation != Tile.Rotation.None)
		{
			Texture2D moveAnimTexture = this.getMoveAnimTexture(this.tileAnimationProgress % 18);
			this.tileOverlay.renderer.material.mainTexture = moveAnimTexture;
		}
	}

	// Token: 0x06000585 RID: 1413 RVA: 0x00005843 File Offset: 0x00003A43
	private void OnMouseEnter()
	{
		this._battleMode.tileOver(this.tilePosition());
	}

	// Token: 0x06000586 RID: 1414 RVA: 0x000028DF File Offset: 0x00000ADF
	private void OnMouseDown()
	{
	}

	// Token: 0x06000587 RID: 1415 RVA: 0x00005856 File Offset: 0x00003A56
	private void OnMouseExit()
	{
		this._battleMode.tileOut(base.gameObject, this._row, this._column);
	}

	// Token: 0x06000588 RID: 1416 RVA: 0x00039BAC File Offset: 0x00037DAC
	public void setReference(GameObject referenceTile)
	{
		this.referenceTile = referenceTile;
		referenceTile.renderer.material.color = new Color(1f, 1f, 1f, 0.1f);
		this.tileOverlay = PrimitiveFactory.createPlane(false);
		this.mimic(this.tileOverlay, referenceTile);
		this.tileOverlay.renderer.enabled = false;
	}

	// Token: 0x06000589 RID: 1417 RVA: 0x00005875 File Offset: 0x00003A75
	public GameObject getReference()
	{
		return this.referenceTile;
	}

	// Token: 0x0600058A RID: 1418 RVA: 0x00039C14 File Offset: 0x00037E14
	public void mimic(GameObject dst, GameObject src)
	{
		dst.transform.localPosition = src.transform.localPosition;
		dst.transform.localScale = src.transform.localScale;
		dst.transform.localRotation = src.transform.localRotation;
		Material material = new Material(ResourceManager.LoadShader("Scrolls/Transparent/Diffuse/Double"));
		material.color = new Color(1f, 1f, 1f, 0.2f);
		material.renderQueue = src.renderer.material.renderQueue;
		dst.renderer.material = material;
		dst.renderer.castShadows = src.renderer.castShadows;
		dst.renderer.receiveShadows = src.renderer.receiveShadows;
	}

	// Token: 0x0600058B RID: 1419 RVA: 0x0000587D File Offset: 0x00003A7D
	public void setMarked(Tile.SelectionType markedID)
	{
		this.markedID = markedID;
		this.markInternal(markedID, 0.1f);
	}

	// Token: 0x0600058C RID: 1420 RVA: 0x00039CE0 File Offset: 0x00037EE0
	public void unmark()
	{
		this.tileOverlay.renderer.material.color = new Color(1f, 1f, 1f, 0.2f);
		this.markInternal(this.markedID, 0.1f);
	}

	// Token: 0x0600058D RID: 1421 RVA: 0x00039D2C File Offset: 0x00037F2C
	public void mark(Tile.SelectionType markedID)
	{
		if (markedID == Tile.SelectionType.Hover)
		{
			this.tileOverlay.renderer.material.color = new Color(1f, 1f, 1f, 0.6f);
		}
		this.markInternal(markedID, 1f);
	}

	// Token: 0x0600058E RID: 1422 RVA: 0x00039D7C File Offset: 0x00037F7C
	private void markInternal(Tile.SelectionType markedID, float alpha)
	{
		if (markedID == Tile.SelectionType.Selected)
		{
			if (this.targetAnimBack == null)
			{
				this.setNormal();
				this.targetAnimBack = this.createTargetAnim("battlegui-cardglow_cast_target", 89000, "hex_target_runes_back");
				this.targetAnimBack.renderer.material.color = this.currentTileColor;
				this.targetAnimFront = this.createTargetAnim("battlegui-cardglow_cast_target", 89001, "hex_target_runes_front");
			}
		}
		else if (this.targetAnimBack != null)
		{
			Object.Destroy(this.targetAnimBack);
			this.targetAnimBack = null;
			Object.Destroy(this.targetAnimFront);
			this.targetAnimFront = null;
		}
		if (markedID == Tile.SelectionType.None)
		{
			this.setNormal();
		}
		this.markerType = markedID;
	}

	// Token: 0x0600058F RID: 1423 RVA: 0x00039E48 File Offset: 0x00038048
	public void setChargeAnimation(int animID)
	{
		if (this.chargeAnimID != animID)
		{
			if (this.chargeAnim != null)
			{
				Object.Destroy(this.chargeAnim);
			}
			if (animID != -1)
			{
				this.chargeAnim = this.createAnim("charge", 89000);
			}
			this.chargeAnimID = animID;
		}
	}

	// Token: 0x06000590 RID: 1424 RVA: 0x00039EA4 File Offset: 0x000380A4
	private void setNormal()
	{
		this.setMoveAnim(null);
		this.referenceTile.renderer.material.color = new Color(1f, 1f, 1f, 0.1f);
		this.referenceTile.renderer.material.mainTexture = ResourceManager.LoadTexture("BattleMode/Tiles/hexagon_standard");
	}

	// Token: 0x06000591 RID: 1425 RVA: 0x00005892 File Offset: 0x00003A92
	private GameObject createAnim(string animFolder, int renderQueue)
	{
		return this.createAnim(animFolder, renderQueue, new Vector3(0.8f, 0.9f, 1f), null);
	}

	// Token: 0x06000592 RID: 1426 RVA: 0x00039F08 File Offset: 0x00038108
	private GameObject createTargetAnim(string animFolder, int renderQueue, string name)
	{
		GameObject gameObject = this.createAnim(animFolder, renderQueue, new Vector3(0.3465f, 0.334f, 1f), name);
		EffectPlayer component = gameObject.GetComponent<EffectPlayer>();
		AnimPlayer animPlayer = component.getAnimPlayer();
		float fps = animPlayer.getFrameAnimation().getFps();
		int numFrames = animPlayer.getFrameAnimation().getNumFrames();
		float num = (float)numFrames / fps;
		float frame = (float)((int)(Time.time * fps + (float)(this.row() * numFrames) / 14.75f) % numFrames);
		animPlayer.setFrame(frame);
		return gameObject;
	}

	// Token: 0x06000593 RID: 1427 RVA: 0x00039F8C File Offset: 0x0003818C
	private GameObject createAnim(string animFolder, int renderQueue, Vector3 scale, string animName)
	{
		GameObject gameObject = this._battleMode.createEffectAnimation(animFolder, renderQueue, this.referenceTile.transform.position, 1, scale, new Vector3(90f, 270f, 0f), true);
		if (animName == null)
		{
			return gameObject;
		}
		EffectPlayer component = gameObject.GetComponent<EffectPlayer>();
		component.getAnimPlayer().setAnimationId(animName);
		return gameObject;
	}

	// Token: 0x06000594 RID: 1428 RVA: 0x00039FEC File Offset: 0x000381EC
	public void setMoveAnim(TilePosition origin)
	{
		if (origin == null)
		{
			this.tileOverlay.renderer.enabled = false;
			this.moveAnimationRotation = Tile.Rotation.None;
			return;
		}
		Tile tile = this._battleMode.getTile(origin);
		this.moveAnimBaseFilename = "BattleMode/Tiles/HexMoveDiagonal/hex_move_diag_";
		Tile.Rotation rotation = this.moveAnimationRotation = this.getGeneralDirectionTo(tile);
		this.tileOverlay.renderer.enabled = true;
		this.tileOverlay.transform.localEulerAngles = this.referenceTile.transform.localEulerAngles;
		this.tileOverlay.transform.localScale = this.referenceTile.transform.localScale;
		if (rotation == Tile.Rotation.W || rotation == Tile.Rotation.E)
		{
			this.moveAnimBaseFilename = "BattleMode/Tiles/HexMove/hex_move_side_";
		}
		if (rotation == Tile.Rotation.W || rotation == Tile.Rotation.SW || rotation == Tile.Rotation.NW)
		{
			this.tileOverlay.renderer.transform.localEulerAngles = new Vector3(0f, 270f, 0f);
		}
		if (rotation == Tile.Rotation.SE)
		{
			Vector3 localScale = this.referenceTile.renderer.transform.localScale;
			localScale.z = -localScale.z;
			this.tileOverlay.renderer.transform.localScale = localScale;
		}
		if (rotation == Tile.Rotation.NW)
		{
			Vector3 localScale2 = this.referenceTile.renderer.transform.localScale;
			localScale2.z = -localScale2.z;
			this.tileOverlay.renderer.transform.localScale = localScale2;
		}
		this.updateMoveAnim();
	}

	// Token: 0x06000595 RID: 1429 RVA: 0x0003A178 File Offset: 0x00038378
	private Tile.Rotation getGeneralDirectionTo(Tile t)
	{
		Vector3 vector = t.referenceTile.transform.position - this.referenceTile.transform.position;
		if (vector.x > 0.1f)
		{
			return (vector.z <= 0f) ? Tile.Rotation.NE : Tile.Rotation.NW;
		}
		if (vector.x < -0.1f)
		{
			return (vector.z <= 0f) ? Tile.Rotation.SE : Tile.Rotation.SW;
		}
		return (vector.z <= 0f) ? Tile.Rotation.E : Tile.Rotation.W;
	}

	// Token: 0x06000596 RID: 1430 RVA: 0x0003A218 File Offset: 0x00038418
	private Texture2D getMoveAnimTexture(int i)
	{
		string filename = string.Concat(new object[]
		{
			this.moveAnimBaseFilename,
			(17 - i).ToString("D4"),
			"_Frame-",
			i + 5
		});
		return ResourceManager.LoadTexture(filename);
	}

	// Token: 0x06000597 RID: 1431 RVA: 0x000058B1 File Offset: 0x00003AB1
	public void setTileTargetColor(Color color)
	{
		this.currentTileColor = color;
	}

	// Token: 0x040003F8 RID: 1016
	public const float AlphaNormal = 0.1f;

	// Token: 0x040003F9 RID: 1017
	private int subFrame;

	// Token: 0x040003FA RID: 1018
	private BattleMode _battleMode;

	// Token: 0x040003FB RID: 1019
	private int _row;

	// Token: 0x040003FC RID: 1020
	private int _column;

	// Token: 0x040003FD RID: 1021
	private bool _isLeft;

	// Token: 0x040003FE RID: 1022
	private TileColor _color;

	// Token: 0x040003FF RID: 1023
	private GameObject referenceTile;

	// Token: 0x04000400 RID: 1024
	private GameObject tileOverlay;

	// Token: 0x04000401 RID: 1025
	private Tile.SelectionType markedID;

	// Token: 0x04000402 RID: 1026
	private GameObject targetAnimBack;

	// Token: 0x04000403 RID: 1027
	private GameObject targetAnimFront;

	// Token: 0x04000404 RID: 1028
	private GameObject chargeAnim;

	// Token: 0x04000405 RID: 1029
	private int chargeAnimID = -1;

	// Token: 0x04000406 RID: 1030
	private int tileAnimationProgress;

	// Token: 0x04000407 RID: 1031
	private Tile.Rotation moveAnimationRotation;

	// Token: 0x04000408 RID: 1032
	private string moveAnimBaseFilename;

	// Token: 0x04000409 RID: 1033
	private Tile.SelectionType markerType;

	// Token: 0x0400040A RID: 1034
	private Color currentTileColor = Color.white;

	// Token: 0x0200009C RID: 156
	public enum SelectionType
	{
		// Token: 0x0400040C RID: 1036
		None,
		// Token: 0x0400040D RID: 1037
		Target,
		// Token: 0x0400040E RID: 1038
		Selected,
		// Token: 0x0400040F RID: 1039
		SelectedMove,
		// Token: 0x04000410 RID: 1040
		Path,
		// Token: 0x04000411 RID: 1041
		Hover,
		// Token: 0x04000412 RID: 1042
		HoverStatic
	}

	// Token: 0x0200009D RID: 157
	private enum Rotation
	{
		// Token: 0x04000414 RID: 1044
		None,
		// Token: 0x04000415 RID: 1045
		W,
		// Token: 0x04000416 RID: 1046
		NW,
		// Token: 0x04000417 RID: 1047
		NE,
		// Token: 0x04000418 RID: 1048
		E,
		// Token: 0x04000419 RID: 1049
		SE,
		// Token: 0x0400041A RID: 1050
		SW
	}
}
