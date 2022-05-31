using System;

// Token: 0x0200009F RID: 159
public class AiTutorialTicker : EmptyTutorial
{
	// Token: 0x060005A0 RID: 1440 RVA: 0x00005951 File Offset: 0x00003B51
	public void addText(string text)
	{
		if (this.ticker.isEmpty())
		{
			this.ticker.add(text);
		}
		this.ticker.add(text);
	}

	// Token: 0x060005A1 RID: 1441 RVA: 0x0000597B File Offset: 0x00003B7B
	public override string getText()
	{
		return this.ticker.getText();
	}

	// Token: 0x060005A2 RID: 1442 RVA: 0x00005988 File Offset: 0x00003B88
	public override bool isRunning()
	{
		return this.ticker.isRunning();
	}

	// Token: 0x060005A3 RID: 1443 RVA: 0x00005995 File Offset: 0x00003B95
	public override void next()
	{
		this.ticker.next();
		Log.warning("NEXT: " + this.getText());
	}

	// Token: 0x0400041E RID: 1054
	private AiTutorialTicker.AiTicker ticker = new AiTutorialTicker.AiTicker();

	// Token: 0x020000A0 RID: 160
	private class AiTicker : TutorialTicker
	{
		// Token: 0x060005A4 RID: 1444 RVA: 0x000059B7 File Offset: 0x00003BB7
		public AiTicker() : base(new TutorialTicker.Line[0])
		{
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x000059C5 File Offset: 0x00003BC5
		public void add(string text)
		{
			base.addLine(new TutorialTicker.Line(text, new TutorialTicker.Tag[0]));
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x000059D9 File Offset: 0x00003BD9
		public bool isEmpty()
		{
			return !base.hasLines();
		}
	}
}
