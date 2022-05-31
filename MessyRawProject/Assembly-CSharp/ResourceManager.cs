using System;
using System.Collections.Generic;
using Animation.Serialization;
using Irrelevant.Assets;
using UnityEngine;

// Token: 0x02000031 RID: 49
public class ResourceManager
{
	// Token: 0x17000035 RID: 53
	// (get) Token: 0x06000237 RID: 567 RVA: 0x00003A95 File Offset: 0x00001C95
	public static ResourceManager instance
	{
		get
		{
			if (ResourceManager._instance == null)
			{
				ResourceManager._instance = new ResourceManager();
			}
			return ResourceManager._instance;
		}
	}

	// Token: 0x06000238 RID: 568 RVA: 0x0002160C File Offset: 0x0001F80C
	public Texture2D tryGetTexture2D(string id)
	{
		Texture2D result = null;
		this._textures2D.TryGetValue(id, ref result);
		return result;
	}

	// Token: 0x06000239 RID: 569 RVA: 0x00003AB0 File Offset: 0x00001CB0
	public void assignTexture2D(string id, Texture2D tex)
	{
		this.assignTexture2D(id, tex, true);
	}

	// Token: 0x0600023A RID: 570 RVA: 0x00003ABB File Offset: 0x00001CBB
	public void assignTexture2D(string id, Texture2D tex, bool overwrite)
	{
		if (overwrite || !this._textures2D.ContainsKey(id))
		{
			this._textures2D[id] = tex;
		}
	}

	// Token: 0x0600023B RID: 571 RVA: 0x0002162C File Offset: 0x0001F82C
	public UnitAnimDescription getUnitData(string id)
	{
		UnitAnimDescription result = null;
		this._units.TryGetValue(id, ref result);
		return result;
	}

	// Token: 0x0600023C RID: 572 RVA: 0x00003AE1 File Offset: 0x00001CE1
	public void assignUnitData(string id, UnitAnimDescription unit)
	{
		this.assignUnitData(id, unit, true);
	}

	// Token: 0x0600023D RID: 573 RVA: 0x00003AEC File Offset: 0x00001CEC
	public void assignUnitData(string id, UnitAnimDescription unit, bool overwrite)
	{
		if (overwrite || !this._units.ContainsKey(id))
		{
			this._units[id] = unit;
		}
	}

	// Token: 0x0600023E RID: 574 RVA: 0x00003B12 File Offset: 0x00001D12
	public UnitAnimDescription getBundledAnimationData(string animFolder)
	{
		return this.getBundledAnimationData_RawPath("BattleMode/Animations/" + animFolder);
	}

	// Token: 0x0600023F RID: 575 RVA: 0x0002164C File Offset: 0x0001F84C
	public UnitAnimDescription getBundledAnimationData_RawPath(string folder)
	{
		string id = "bundled:" + folder;
		UnitAnimDescription unitAnimDescription = this.getUnitData(id);
		if (unitAnimDescription == null)
		{
			Texture2D texture2D = ResourceManager.LoadTexture(folder + "/sprites");
			if (texture2D == null)
			{
				return null;
			}
			texture2D.filterMode = 2;
			texture2D.anisoLevel = 5;
			PD_Atlas pd_Atlas = new PD_Atlas();
			if (!ProtoUtil.deserializeInto(((TextAsset)ResourceManager.Load(folder + "/spritespos")).bytes, pd_Atlas, typeof(PD_Atlas)))
			{
				return null;
			}
			PD_AnimCollection pd_AnimCollection = new PD_AnimCollection();
			if (!ProtoUtil.deserializeInto(((TextAsset)ResourceManager.Load(folder + "/anims")).bytes, pd_AnimCollection, typeof(PD_AnimCollection)))
			{
				return null;
			}
			unitAnimDescription = new UnitAnimDescription(new AtlasAnimsBundle(pd_Atlas, pd_AnimCollection));
			unitAnimDescription.textureReference = texture2D;
			unitAnimDescription.nameAnimations(folder);
			this.assignUnitData(id, unitAnimDescription);
		}
		return unitAnimDescription;
	}

	// Token: 0x06000240 RID: 576 RVA: 0x00021738 File Offset: 0x0001F938
	public static Texture2D LoadTexture(string filename)
	{
		Texture2D texture2D = (Texture2D)ResourceManager.resourceAbstraction.Load(filename);
		if (texture2D == null)
		{
			Debug.LogError("Couldn't find texture: " + filename);
		}
		return texture2D;
	}

	// Token: 0x06000241 RID: 577 RVA: 0x00021774 File Offset: 0x0001F974
	public static Shader LoadShader(string filename)
	{
		if (filename.StartsWith("Scrolls/"))
		{
			string filename2 = filename.Replace("/", string.Empty).Replace("Scrolls", "Shader/");
			return (Shader)ResourceManager.Load(filename2);
		}
		return Shader.Find(filename);
	}

	// Token: 0x06000242 RID: 578 RVA: 0x00003B25 File Offset: 0x00001D25
	public static Object Load(string filename)
	{
		return ResourceManager.resourceAbstraction.Load(filename);
	}

	// Token: 0x06000243 RID: 579 RVA: 0x00003B25 File Offset: 0x00001D25
	public static Object Load(string filename, Type t)
	{
		return ResourceManager.resourceAbstraction.Load(filename);
	}

	// Token: 0x04000125 RID: 293
	private static ResourceAbstraction resourceAbstraction = new ResourceAbstraction();

	// Token: 0x04000126 RID: 294
	private static ResourceManager _instance = null;

	// Token: 0x04000127 RID: 295
	private Dictionary<string, Texture2D> _textures2D = new Dictionary<string, Texture2D>();

	// Token: 0x04000128 RID: 296
	private Dictionary<string, UnitAnimDescription> _units = new Dictionary<string, UnitAnimDescription>();
}
