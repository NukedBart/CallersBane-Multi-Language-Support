using System;
using System.Collections.Generic;
using Animation.Serialization;
using Gui;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x02000397 RID: 919
public class Avatar
{
	// Token: 0x0600148C RID: 5260 RVA: 0x0007F874 File Offset: 0x0007DA74
	private Avatar(bool animate, int excludeIdBits, AvatarInfo avatar)
	{
		this.animate = animate;
		this.excludeIdBits = excludeIdBits;
		int count = Avatar.animData.getAnimPartDatasForFrame(0).Count;
		for (int i = 0; i < count; i++)
		{
			Avatar.PosPart posPart = new Avatar.PosPart();
			posPart.enabled = this.isExcluded(i);
			this.bodyParts.Add(posPart);
			this.bodyPartOffsets.Add(0f);
			this.initialBodyPartOffsets.Add(0f);
		}
		this.fillInBodyOffsets(0, this.initialBodyPartOffsets);
		if (avatar != null)
		{
			this.set(AvatarPartName.ARM_FRONT, avatar.armFront.id);
			this.set(AvatarPartName.ARM_BACK, avatar.armBack.id);
			this.set(AvatarPartName.BODY, avatar.body.id);
			this.set(AvatarPartName.HEAD, avatar.head.id);
			this.set(AvatarPartName.LEG, avatar.leg.id);
		}
	}

	// Token: 0x0600148D RID: 5261 RVA: 0x0007F998 File Offset: 0x0007DB98
	static Avatar()
	{
		Avatar.animDesc = ResourceManager.instance.getBundledAnimationData_RawPath("Profile/avatars");
		if (Avatar.animDesc.hasAnimation(0))
		{
			Avatar.animData = Avatar.animDesc.getAnimation(0);
		}
		Avatar.updatePosPartDescs("MALE_1", Avatar.posPartsDesc);
		Avatar.updatePosPartDescs("FEMALE_1", Avatar.posPartsDesc);
	}

	// Token: 0x0600148E RID: 5262 RVA: 0x0000F21A File Offset: 0x0000D41A
	public static Avatar ProfilePageAvatar(AvatarInfo avatar)
	{
		return new Avatar(true, 0, avatar);
	}

	// Token: 0x0600148F RID: 5263 RVA: 0x0000F224 File Offset: 0x0000D424
	public static Avatar Create(AvatarInfo avatar, int excludeIdBits)
	{
		return new Avatar(false, excludeIdBits, avatar);
	}

	// Token: 0x06001490 RID: 5264 RVA: 0x0000F22E File Offset: 0x0000D42E
	public static Avatar Create(AvatarInfo avatar)
	{
		return Avatar.Create(avatar, 0);
	}

	// Token: 0x06001491 RID: 5265 RVA: 0x0007FA18 File Offset: 0x0007DC18
	public static int getExcludeBits(params AvatarPartName[] exclude)
	{
		int num = 0;
		foreach (AvatarPartName avatarPartName in exclude)
		{
			num |= 1 << (int)avatarPartName;
		}
		return num;
	}

	// Token: 0x06001492 RID: 5266 RVA: 0x0007FA50 File Offset: 0x0007DC50
	private void _setTexture(int id, ImageMergerComponent.Pos part)
	{
		if (part == null)
		{
			return;
		}
		if (this.isExcluded(id))
		{
			return;
		}
		this.bodyParts[id].pos = part;
		this.bodyParts[id].zOrder = this.getZindex(id);
		this.sortedBodyPartsDirtyFlag = true;
	}

	// Token: 0x06001493 RID: 5267 RVA: 0x0007FAA4 File Offset: 0x0007DCA4
	public void update()
	{
		if (Avatar.animData != null)
		{
			int frame = (int)(Time.time * Avatar.animData.fps) % Avatar.animData.getNumFrames();
			this.fillInBodyOffsets(frame, this.bodyPartOffsets);
		}
		this.updateBody();
	}

	// Token: 0x06001494 RID: 5268 RVA: 0x0000F237 File Offset: 0x0000D437
	private void setHeadFrontMost(bool frontMost)
	{
		this._headFrontMost = frontMost;
	}

	// Token: 0x06001495 RID: 5269 RVA: 0x0007FAEC File Offset: 0x0007DCEC
	private float getZindex(int part)
	{
		int[] array = new int[]
		{
			0,
			1,
			2,
			10,
			30
		};
		if (this._headFrontMost)
		{
			array[3] = 40;
		}
		return (float)array[part];
	}

	// Token: 0x06001496 RID: 5270 RVA: 0x0007FB20 File Offset: 0x0007DD20
	private void updateBody()
	{
		if (!this.animate)
		{
			return;
		}
		for (int i = 0; i < this.bodyParts.Count; i++)
		{
			if (!this.isExcluded(i))
			{
				this.bodyParts[i].yOffset = this.bodyPartOffsets[i] - this.initialBodyPartOffsets[i];
			}
		}
	}

	// Token: 0x06001497 RID: 5271 RVA: 0x0000F240 File Offset: 0x0000D440
	public void set(AvatarPartName partType, int partId)
	{
		this.set((int)partType, partId);
	}

	// Token: 0x06001498 RID: 5272 RVA: 0x0007FB90 File Offset: 0x0007DD90
	public void set(int id, int partId)
	{
		if (this.isExcluded(id))
		{
			return;
		}
		AvatarPart avatarPart = AvatarPartTypeManager.getInstance().get(partId);
		if (avatarPart == null)
		{
			return;
		}
		if (id == 3)
		{
			this.setHeadFrontMost(AvatarPartTypeManager.isHeadFrontMost(avatarPart.set));
		}
		if (id == 0)
		{
			AvatarPartTypeManager instance = AvatarPartTypeManager.getInstance();
			int frontArmIdForBackArm = instance.getFrontArmIdForBackArm(partId);
			this.set(AvatarPartName.ARM_FRONT, frontArmIdForBackArm);
		}
		this._setTexture(id, Avatar.GetPosPart(avatarPart.getFullFilename()));
	}

	// Token: 0x06001499 RID: 5273 RVA: 0x0007FC04 File Offset: 0x0007DE04
	private void fillInBodyOffsets(int frame, List<float> dst)
	{
		List<PD_AnimFramePart> animPartDatasForFrame = Avatar.animData.getAnimPartDatasForFrame(frame);
		foreach (PD_AnimFramePart pd_AnimFramePart in animPartDatasForFrame)
		{
			dst[(int)pd_AnimFramePart.meshId] = pd_AnimFramePart.ty;
		}
	}

	// Token: 0x0600149A RID: 5274 RVA: 0x0000F24A File Offset: 0x0000D44A
	public bool isExcluded(int id)
	{
		return (1 << id & this.excludeIdBits) != 0;
	}

	// Token: 0x0600149B RID: 5275 RVA: 0x0007FC70 File Offset: 0x0007DE70
	public Rect getRect(float height)
	{
		float num = height / (float)Avatar.DefaultHeight;
		return new Rect(0f, 0f, num * (float)Avatar.DefaultWidth, num * (float)Avatar.DefaultHeight);
	}

	// Token: 0x0600149C RID: 5276 RVA: 0x0007FCA8 File Offset: 0x0007DEA8
	public void draw(IGui gui, Rect rect, Color color, bool headingLeft)
	{
		if (this.sortedBodyPartsDirtyFlag)
		{
			this.sortedBodyParts.Clear();
			this.sortedBodyParts.AddRange(this.bodyParts);
			this.sortedBodyParts.Sort((Avatar.PosPart a, Avatar.PosPart b) => a.zOrder.CompareTo(b.zOrder));
			this.sortedBodyPartsDirtyFlag = false;
		}
		float num = rect.width / (float)Avatar.DefaultWidth;
		Rect texCoords;
		texCoords..ctor((float)((!headingLeft) ? 1 : 0), 0f, (float)((!headingLeft) ? -1 : 1), 1f);
		Color color2 = gui.GetColor();
		gui.SetColor(color);
		foreach (Avatar.PosPart posPart in this.sortedBodyParts)
		{
			ImageMergerComponent.Pos pos = posPart.pos;
			if (pos != null && !pos.isEmpty())
			{
				string filename = "Profile/sets/" + posPart.pos.filename;
				Texture2D texture2D = ResourceManager.LoadTexture(filename);
				if (!(texture2D == null))
				{
					Rect full;
					full..ctor(rect);
					if (pos.layerWidth != (float)Avatar.DefaultWidth)
					{
						full.width = (float)((int)(full.width * (pos.layerWidth / (float)Avatar.DefaultWidth)));
						if (!headingLeft)
						{
							full.x = rect.x - (full.width - rect.width);
						}
					}
					full.y += posPart.yOffset * 0.3f * num;
					Rect dst = pos.dst;
					if (!headingLeft)
					{
						dst.x = 1f - dst.width - dst.x;
					}
					Rect dst2 = GeomUtil.cropShare(full, dst);
					gui.DrawTextureWithTexCoords(dst2, texture2D, texCoords);
				}
			}
		}
		gui.SetColor(color2);
	}

	// Token: 0x0600149D RID: 5277 RVA: 0x0000F25F File Offset: 0x0000D45F
	public void draw(Rect rect, bool headingLeft)
	{
		this.draw(UnityGui2D.getInstance(), rect, Color.white, headingLeft);
	}

	// Token: 0x0600149E RID: 5278 RVA: 0x0007FEC0 File Offset: 0x0007E0C0
	private static ImageMergerComponent.Pos GetPosPart(string fn)
	{
		if (Avatar.posPartsDesc.ContainsKey(fn))
		{
			return Avatar.posPartsDesc[fn];
		}
		return new ImageMergerComponent.Pos(fn)
		{
			layerWidth = (float)Avatar.DefaultWidth,
			layerHeight = (float)Avatar.DefaultHeight
		};
	}

	// Token: 0x0600149F RID: 5279 RVA: 0x0007FF0C File Offset: 0x0007E10C
	private static void updatePosPartDescs(string set, Dictionary<string, ImageMergerComponent.Pos> d)
	{
		ImageMergePartFileReader.Desc desc = ImageMergePartFileReader.readAsset("Profile/sets/" + set + "/desc");
		if (desc == null)
		{
			return;
		}
		foreach (ImageMergePartFileReader.Layer layer in desc.layers)
		{
			ImageMergerComponent.Pos[] positions = layer.getPositions(set + "/");
			if (positions.Length != 1)
			{
				Log.error(string.Concat(new object[]
				{
					"Expected exactly 1 part per layer in Avatar image set! Found: ",
					positions.Length,
					" in ",
					layer.layer,
					" @ ",
					set
				}));
			}
			else
			{
				ImageMergerComponent.Pos pos = positions[0];
				d[pos.filename] = pos;
			}
		}
	}

	// Token: 0x040011C4 RID: 4548
	private const string BasePath = "Profile/sets/";

	// Token: 0x040011C5 RID: 4549
	private bool animate;

	// Token: 0x040011C6 RID: 4550
	private int excludeIdBits;

	// Token: 0x040011C7 RID: 4551
	private bool _headFrontMost;

	// Token: 0x040011C8 RID: 4552
	private List<Avatar.PosPart> bodyParts = new List<Avatar.PosPart>();

	// Token: 0x040011C9 RID: 4553
	private List<Avatar.PosPart> sortedBodyParts = new List<Avatar.PosPart>();

	// Token: 0x040011CA RID: 4554
	private bool sortedBodyPartsDirtyFlag = true;

	// Token: 0x040011CB RID: 4555
	private List<float> bodyPartOffsets = new List<float>();

	// Token: 0x040011CC RID: 4556
	private List<float> initialBodyPartOffsets = new List<float>();

	// Token: 0x040011CD RID: 4557
	public static readonly int DefaultWidth = 567;

	// Token: 0x040011CE RID: 4558
	public static readonly int DefaultHeight = 991;

	// Token: 0x040011CF RID: 4559
	private static UnitAnimDescription animDesc;

	// Token: 0x040011D0 RID: 4560
	private static AnimData animData;

	// Token: 0x040011D1 RID: 4561
	private static Dictionary<string, ImageMergerComponent.Pos> posPartsDesc = new Dictionary<string, ImageMergerComponent.Pos>();

	// Token: 0x02000398 RID: 920
	private class PosPart
	{
		// Token: 0x040011D3 RID: 4563
		public bool enabled;

		// Token: 0x040011D4 RID: 4564
		public ImageMergerComponent.Pos pos;

		// Token: 0x040011D5 RID: 4565
		public float yOffset;

		// Token: 0x040011D6 RID: 4566
		public float zOrder;
	}
}
