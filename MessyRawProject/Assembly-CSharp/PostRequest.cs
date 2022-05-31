using System;
using System.IO;
using System.Net;
using System.Text;
using JsonFx.Json;

// Token: 0x0200014A RID: 330
public class PostRequest
{
	// Token: 0x06000AA9 RID: 2729 RVA: 0x00008FE0 File Offset: 0x000071E0
	public PostRequest(string url, object payload)
	{
		this.PostJson(url, payload);
	}

	// Token: 0x06000AAA RID: 2730 RVA: 0x000504D4 File Offset: 0x0004E6D4
	public void PostJson(string url, object payload)
	{
		string text = new JsonWriter().Write(payload);
		byte[] bytes = Encoding.UTF8.GetBytes(text.ToCharArray());
		WebClient webClient = new WebClient();
		webClient.Headers.Add("Content-Type", "application/json");
		webClient.UploadDataCompleted += new UploadDataCompletedEventHandler(this.UploadDataCompleted);
		webClient.UploadDataAsync(new Uri(url), "POST", bytes);
	}

	// Token: 0x06000AAB RID: 2731 RVA: 0x00050540 File Offset: 0x0004E740
	private void UploadDataCompleted(object sender, UploadDataCompletedEventArgs e)
	{
		if (e.Error == null)
		{
			this.data = Encoding.Default.GetString(e.Result);
		}
		else if (e.Error.GetType().Name == "WebException")
		{
			WebException ex = (WebException)e.Error;
			this.error = ex.Message;
			if (ex.Response != null)
			{
				this.errorCode = ((HttpWebResponse)ex.Response).StatusCode;
				using (StreamReader streamReader = new StreamReader(ex.Response.GetResponseStream()))
				{
					this.errorData = streamReader.ReadToEnd();
				}
			}
		}
		this.done = true;
	}

	// Token: 0x06000AAC RID: 2732 RVA: 0x00008FF0 File Offset: 0x000071F0
	public bool isDone()
	{
		return this.done;
	}

	// Token: 0x06000AAD RID: 2733 RVA: 0x00008FF8 File Offset: 0x000071F8
	public bool hasError()
	{
		return this.error != null;
	}

	// Token: 0x06000AAE RID: 2734 RVA: 0x00009006 File Offset: 0x00007206
	public virtual string getError()
	{
		return this.error;
	}

	// Token: 0x06000AAF RID: 2735 RVA: 0x0000900E File Offset: 0x0000720E
	public int getErrorCode()
	{
		return this.errorCode;
	}

	// Token: 0x06000AB0 RID: 2736 RVA: 0x00009016 File Offset: 0x00007216
	protected T buildError<T>()
	{
		return new JsonReader().Read<T>(this.errorData);
	}

	// Token: 0x06000AB1 RID: 2737 RVA: 0x00009028 File Offset: 0x00007228
	protected T buildResponse<T>()
	{
		return new JsonReader().Read<T>(this.data);
	}

	// Token: 0x04000831 RID: 2097
	private bool done;

	// Token: 0x04000832 RID: 2098
	private string data;

	// Token: 0x04000833 RID: 2099
	private int errorCode;

	// Token: 0x04000834 RID: 2100
	private string error;

	// Token: 0x04000835 RID: 2101
	private string errorData;
}
