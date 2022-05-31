using System;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x02000075 RID: 117
public class EffectPlayer : MonoBehaviour, iAnim
{
	// Token: 0x06000476 RID: 1142 RVA: 0x00004EAD File Offset: 0x000030AD
	public void setMaterialToUse(Material m)
	{
		this._materialToUse = m;
	}

	// Token: 0x17000049 RID: 73
	// (set) Token: 0x06000477 RID: 1143 RVA: 0x00004EB6 File Offset: 0x000030B6
	public int layer
	{
		set
		{
			this.animPlayer.layer = value;
		}
	}

	// Token: 0x1700004A RID: 74
	// (set) Token: 0x06000478 RID: 1144 RVA: 0x00004EC4 File Offset: 0x000030C4
	public float frame
	{
		set
		{
			this.animPlayer.setFrame(value);
		}
	}

	// Token: 0x06000479 RID: 1145 RVA: 0x00004ED2 File Offset: 0x000030D2
	public EffectPlayer startInSeconds(float s)
	{
		this.frame = -this.getAnimPlayer().createFrameAnimation().getFps() * s;
		return this;
	}

	// Token: 0x0600047A RID: 1146 RVA: 0x00004EEE File Offset: 0x000030EE
	public AnimPlayer getAnimPlayer()
	{
		return this.animPlayer;
	}

	// Token: 0x0600047B RID: 1147 RVA: 0x000028DF File Offset: 0x00000ADF
	private void Start()
	{
	}

	// Token: 0x0600047C RID: 1148 RVA: 0x00031A1C File Offset: 0x0002FC1C
	private void postError()
	{
		if (!this.hasPostedResourceError)
		{
			Log.error(string.Concat(new string[]
			{
				"EffectPlayer::Update. name: ",
				this.effectName,
				" @ ",
				this.animFolder,
				" could not be loaded"
			}));
			this.hasPostedResourceError = true;
		}
	}

	// Token: 0x0600047D RID: 1149 RVA: 0x00031A78 File Offset: 0x0002FC78
	private void LateUpdate()
	{
		if (!base.renderer.enabled)
		{
			return;
		}
		Vector3 vector;
		vector..ctor((float)this.directionMod * this.baseScale.x, this.baseScale.y, this.baseScale.z);
		if (this.scaleTransform != null)
		{
			vector *= this.scaleTransform.localScale.x;
		}
		else if (base.transform.parent)
		{
			vector *= base.transform.parent.localScale.x;
		}
		else
		{
			vector.Scale(base.transform.localScale);
		}
		this.animPlayer.Update(base.transform, base.renderer.material, vector);
		if (this.log)
		{
			Log.info(string.Concat(new object[]
			{
				"t (",
				this.animPlayer.lastAnim,
				"): ",
				Time.time,
				", f: ",
				this.animPlayer.getFrameAnimation().getFrame()
			}));
		}
	}

	// Token: 0x0600047E RID: 1150 RVA: 0x00004EF6 File Offset: 0x000030F6
	public void animationDone(AnimPlayer player)
	{
		if (this.callbackTarget != null)
		{
			this.callbackTarget.effectAnimDone(this, this.loop);
		}
	}

	// Token: 0x0600047F RID: 1151 RVA: 0x00004F15 File Offset: 0x00003115
	public void locator(AnimPlayer p, AnimLocator loc)
	{
		this.callbackTarget.locator(this, loc);
	}

	// Token: 0x06000480 RID: 1152 RVA: 0x00004F24 File Offset: 0x00003124
	public static void preload(string animFolder)
	{
		ResourceManager.instance.getBundledAnimationData(animFolder);
	}

	// Token: 0x06000481 RID: 1153 RVA: 0x00031BC4 File Offset: 0x0002FDC4
	public void init(string animFolder, int directionMod, iEffect callbackTarget, int renQ, Vector3 baseScale, bool loop, string name, int animId)
	{
		this.animFolder = animFolder;
		this.init(ResourceManager.instance.getBundledAnimationData(animFolder), directionMod, callbackTarget, renQ, baseScale, loop, name, animId);
	}

	// Token: 0x06000482 RID: 1154 RVA: 0x00004F32 File Offset: 0x00003132
	public void init(string animFolder, int directionMod, iEffect callbackTarget, int renQ, Vector3 baseScale, int animId)
	{
		this.animFolder = animFolder;
		this.init(ResourceManager.instance.getBundledAnimationData(animFolder), directionMod, callbackTarget, renQ, baseScale, animId);
	}

	// Token: 0x06000483 RID: 1155 RVA: 0x00004F54 File Offset: 0x00003154
	public void init(UnitAnimDescription anim, int directionMod, iEffect callbackTarget, int renQ, Vector3 baseScale, bool loop, string name, int animId)
	{
		this.effectName = name;
		this.loop = loop;
		this.init(anim, directionMod, callbackTarget, renQ, baseScale, animId);
	}

	// Token: 0x06000484 RID: 1156 RVA: 0x00031BF8 File Offset: 0x0002FDF8
	public void init(AnimConf conf, iEffect callback)
	{
		this.init(conf.bundle, (!conf.flipX) ? 1 : -1, callback, conf.renderQueue, conf.scale, conf.loop, string.Empty, conf.animId);
		if (conf.layer >= 0)
		{
			this.layer = conf.layer;
		}
		if (conf.waitForUpdate)
		{
			this.getAnimPlayer().waitForUpdate();
		}
	}

	// Token: 0x06000485 RID: 1157 RVA: 0x00031C70 File Offset: 0x0002FE70
	public void init(UnitAnimDescription anim, int directionMod, iEffect callbackTarget, int renQ, Vector3 baseScale, int animId)
	{
		this.baseScale = baseScale;
		this.callbackTarget = callbackTarget;
		this.directionMod = directionMod;
		this.animPlayer = new AnimPlayer(App.Clocks.animClock);
		base.renderer.material = (this._materialToUse ?? new Material(ResourceManager.LoadShader("Scrolls/Transparent/Diffuse/Double")));
		if (renQ >= 0)
		{
			base.renderer.material.renderQueue = renQ;
		}
		if (anim != null)
		{
			base.renderer.material.mainTexture = anim.textureReference;
			this.animPlayer.setDescription(anim, this);
			this.playEffect(animId);
		}
		else
		{
			this.postError();
		}
	}

	// Token: 0x06000486 RID: 1158 RVA: 0x00004F75 File Offset: 0x00003175
	public void playEffect(int id)
	{
		if (id < 0)
		{
			id = 0;
		}
		this.animPlayer.setAnimationId(id);
	}

	// Token: 0x06000487 RID: 1159 RVA: 0x00004F8D File Offset: 0x0000318D
	public void playEffect(string name)
	{
		this.animPlayer.setAnimationId(name);
	}

	// Token: 0x06000488 RID: 1160 RVA: 0x00004F9B File Offset: 0x0000319B
	public void playEffect()
	{
		this.playEffect(this.animPlayer.getLastAnimationId());
	}

	// Token: 0x06000489 RID: 1161 RVA: 0x00004FAE File Offset: 0x000031AE
	public void DestroyWithParent(GameObject parent)
	{
		Object.Destroy(parent);
	}

	// Token: 0x040002E1 RID: 737
	private AnimPlayer animPlayer;

	// Token: 0x040002E2 RID: 738
	public Transform scaleTransform;

	// Token: 0x040002E3 RID: 739
	private string _bundleId;

	// Token: 0x040002E4 RID: 740
	private bool _inverted;

	// Token: 0x040002E5 RID: 741
	private int directionMod;

	// Token: 0x040002E6 RID: 742
	private string animFolder;

	// Token: 0x040002E7 RID: 743
	private int effectStage;

	// Token: 0x040002E8 RID: 744
	private iEffect callbackTarget;

	// Token: 0x040002E9 RID: 745
	[SerializeField]
	private Vector3 baseScale;

	// Token: 0x040002EA RID: 746
	private bool loop;

	// Token: 0x040002EB RID: 747
	public string effectName = "unnamed";

	// Token: 0x040002EC RID: 748
	public bool log;

	// Token: 0x040002ED RID: 749
	private Material _materialToUse;

	// Token: 0x040002EE RID: 750
	private bool hasPostedResourceError;
}
