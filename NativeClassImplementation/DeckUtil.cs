using System;
using System.Text;

// Token: 0x0200042A RID: 1066
public class DeckUtil
{
	// Token: 0x060017A4 RID: 6052 RVA: 0x000913F0 File Offset: 0x0008F5F0
	public static string GetFormattedDeckNames(string[] deckNames)
	{
		StringBuilder stringBuilder = new StringBuilder();
		stringBuilder.Append("<color=#ccbbaa>");
		int num = 0;
		while (num < deckNames.Length && num < 2)
		{
			stringBuilder.Append(deckNames[num]);
			if (num < deckNames.Length)
			{
				stringBuilder.Append("\n");
			}
			num++;
		}
		stringBuilder.Append("</color>");
		int num2 = deckNames.Length - 2;
		if (num2 > 0)
		{
			stringBuilder.Append("<color=#665555>(" + num2 + " more)</color>");
		}
		return stringBuilder.ToString();
	}
}
