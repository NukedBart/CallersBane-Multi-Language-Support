using System;
using System.Collections;
using System.IO;
using UnityEngine;

// Token: 0x020003C8 RID: 968
public class ScrollPrintUtil : MonoBehaviour, iCardRule
{
	// Token: 0x06001582 RID: 5506 RVA: 0x000833E0 File Offset: 0x000815E0
	private IEnumerator Start()
	{
		while (CardTypeManager.getInstance().getAll().Count <= 0)
		{
			yield return new WaitForSeconds(1f);
		}
		for (int i = CardTypeManager.getInstance().getAll().Count - 1; i >= 0; i--)
		{
			CardType cardType = CardTypeManager.getInstance().getAll()[i];
			yield return new WaitForSeconds(0.1f);
			if (this.currentView != null)
			{
				Object.DestroyImmediate(this.currentView.gameObject);
			}
			Vector3 cardPos = new Vector3(0f, 1.6f, 0f);
			GameObject cO = PrimitiveFactory.createPlane();
			Transform cardObject = cO.transform;
			cardObject.transform.position = cardPos;
			this.currentView = cardObject.gameObject.AddComponent<CardView>();
			this.currentView.name = "Card";
			this.currentView.init(this, new Card(0L, cardType, false), 1);
			this.currentView.applyHighResTexture();
			cardObject.transform.eulerAngles = new Vector3(90f, 180f, 0f);
			cardObject.transform.localScale = CardView.CardLocalScale(6f);
			yield return new WaitForSeconds(0.1f);
			yield return new WaitForEndOfFrame();
			Texture2D tex2D = new Texture2D(this.cardRenderTexture.width, this.cardRenderTexture.height, 5, false);
			RenderTexture.active = this.cardRenderTexture;
			tex2D.ReadPixels(new Rect(0f, 0f, (float)this.cardRenderTexture.width, (float)this.cardRenderTexture.height), 0, 0, false);
			tex2D.Apply();
			string path = Application.persistentDataPath + "/scrollprints_balance";
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			File.WriteAllBytes(path + "/" + cardType.name + ".png", tex2D.EncodeToPNG());
			Object.Destroy(tex2D);
			Log.info("Printing scroll: " + cardType.name);
		}
		Log.info("Finished - Please note that the exported images will have transparency bugs around text and unit stats");
		yield break;
	}

	// Token: 0x06001583 RID: 5507 RVA: 0x000028DF File Offset: 0x00000ADF
	public void HideCardView()
	{
	}

	// Token: 0x06001584 RID: 5508 RVA: 0x000028DF File Offset: 0x00000ADF
	public void ActivateTriggeredAbility(string id, TilePosition pos)
	{
	}

	// Token: 0x040012B7 RID: 4791
	private CardView currentView;

	// Token: 0x040012B8 RID: 4792
	[SerializeField]
	private RenderTexture cardRenderTexture;
}
