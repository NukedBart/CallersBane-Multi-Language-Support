using System;

// Token: 0x02000362 RID: 866
public class BuyStoreItemResponseMessage : Message
{
	// Token: 0x060013B2 RID: 5042 RVA: 0x0000E95D File Offset: 0x0000CB5D
	public bool isSingleDeckMessage()
	{
		return this.isSingleItem() && this.numDecks() == 1;
	}

	// Token: 0x060013B3 RID: 5043 RVA: 0x0000E976 File Offset: 0x0000CB76
	public bool isSingleAvatarMessage()
	{
		return this.isSingleItem() && this.numAvatars() == 1;
	}

	// Token: 0x060013B4 RID: 5044 RVA: 0x0000E98F File Offset: 0x0000CB8F
	public bool isSingleIdolMessage()
	{
		return this.isSingleItem() && this.numIdols() == 1;
	}

	// Token: 0x060013B5 RID: 5045 RVA: 0x0000E9A8 File Offset: 0x0000CBA8
	public bool isCardsOnlyMessage()
	{
		return this.numCards() >= 1 && this.numCards() == this.countItems();
	}

	// Token: 0x060013B6 RID: 5046 RVA: 0x0000E9C7 File Offset: 0x0000CBC7
	public bool isEmpty()
	{
		return this.countItems() == 0;
	}

	// Token: 0x060013B7 RID: 5047 RVA: 0x0000E9D2 File Offset: 0x0000CBD2
	private int count(object[] objects)
	{
		if (objects == null)
		{
			return 0;
		}
		return objects.Length;
	}

	// Token: 0x060013B8 RID: 5048 RVA: 0x00078D64 File Offset: 0x00076F64
	public string getUnlockString()
	{
		string text = "The following items have been unlocked:\n";
		if (this.cards != null && this.cards.Length > 0)
		{
			text += "\nCards: ";
			for (int i = 0; i < this.cards.Length; i++)
			{
				text = text + "<color=#f0cc66>" + this.cards[i].getName() + "</color>";
				if (i < this.cards.Length - 1)
				{
					text += ", ";
				}
			}
		}
		if (this.deckInfos != null && this.deckInfos.Length > 0)
		{
			text += "\nDecks: ";
			for (int j = 0; j < this.deckInfos.Length; j++)
			{
				text = text + "<color=#f0cc66>" + this.deckInfos[j].name + "</color>";
				if (j < this.deckInfos.Length - 1)
				{
					text += ", ";
				}
			}
		}
		if (this.avatars != null && this.avatars.Length > 0)
		{
			text += "\nAvatars: ";
			for (int k = 0; k < this.avatars.Length; k++)
			{
				text = text + "<color=#f0cc66>" + this.avatars[k].name + "</color>";
				if (k < this.avatars.Length - 1)
				{
					text += ", ";
				}
			}
		}
		if (this.avatarParts != null && this.avatarParts.Length > 0)
		{
			text += "\nAvatar parts: ";
			for (int l = 0; l < this.avatarParts.Length; l++)
			{
				AvatarPart avatarPart = AvatarPartTypeManager.getInstance().get(this.avatarParts[l]);
				if (avatarPart.part != AvatarPartName.ARM_BACK)
				{
					string text2 = text;
					text = string.Concat(new string[]
					{
						text2,
						"<color=#f0cc66>",
						avatarPart.getPartNameString(),
						"</color> (",
						avatarPart.getSetString(),
						")"
					});
					if (l < this.avatarParts.Length - 1)
					{
						text += ", ";
					}
				}
			}
		}
		if (this.idols != null && this.idols.Length > 0)
		{
			text += "\nIdols: ";
			for (int m = 0; m < this.idols.Length; m++)
			{
				text = text + "<color=#f0cc66>" + IdolTypeManager.getInstance().get(this.idols[m]).name + "</color>";
				if (m < this.idols.Length - 1)
				{
					text += ", ";
				}
			}
		}
		return text;
	}

	// Token: 0x060013B9 RID: 5049 RVA: 0x0000E9DF File Offset: 0x0000CBDF
	public AvatarInfo getAvatar()
	{
		return this.avatars[0].getAvatarInfo();
	}

	// Token: 0x060013BA RID: 5050 RVA: 0x0000E9EE File Offset: 0x0000CBEE
	private int numCards()
	{
		if (this.cards == null)
		{
			return 0;
		}
		return this.cards.Length;
	}

	// Token: 0x060013BB RID: 5051 RVA: 0x0000EA05 File Offset: 0x0000CC05
	private int numDecks()
	{
		if (this.deckInfos == null)
		{
			return 0;
		}
		return this.deckInfos.Length;
	}

	// Token: 0x060013BC RID: 5052 RVA: 0x0000EA1C File Offset: 0x0000CC1C
	private int numAvatars()
	{
		if (this.avatars == null)
		{
			return 0;
		}
		return this.avatars.Length;
	}

	// Token: 0x060013BD RID: 5053 RVA: 0x0000EA33 File Offset: 0x0000CC33
	private int numAvatarParts()
	{
		if (this.avatarParts == null)
		{
			return 0;
		}
		return this.avatarParts.Length;
	}

	// Token: 0x060013BE RID: 5054 RVA: 0x0000EA4A File Offset: 0x0000CC4A
	private int numIdols()
	{
		if (this.idols == null)
		{
			return 0;
		}
		return this.idols.Length;
	}

	// Token: 0x060013BF RID: 5055 RVA: 0x0000EA61 File Offset: 0x0000CC61
	private int countItems()
	{
		return this.numCards() + this.numDecks() + this.numAvatars() + this.numAvatarParts() + this.numIdols();
	}

	// Token: 0x060013C0 RID: 5056 RVA: 0x0000EA85 File Offset: 0x0000CC85
	private bool isSingleItem()
	{
		return this.countItems() == 1;
	}

	// Token: 0x040010E9 RID: 4329
	public Card[] cards;

	// Token: 0x040010EA RID: 4330
	public DeckInfo[] deckInfos;

	// Token: 0x040010EB RID: 4331
	public AvatarInfoDeserializer[] avatars;

	// Token: 0x040010EC RID: 4332
	public int[] avatarParts;

	// Token: 0x040010ED RID: 4333
	public short[] idols;
}
