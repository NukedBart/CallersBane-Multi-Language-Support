using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using CommConfig;
using UnityEngine;

// Token: 0x02000211 RID: 529
public class Login : AbstractCommListener, IOverlayClickCallback, IOkCallback, ICancelCallback, IOkCancelCallback, IOkStringCallback, IOkStringCancelCallback
{
	// Token: 0x060010D5 RID: 4309 RVA: 0x0000D05F File Offset: 0x0000B25F
	private void Awake()
	{
		App.ServerSettings = new ServerSettingsMessage();
		if (this.ShouldUseLauncherLogin())
		{
			this.isLauncherLogin = true;
			this.signInPreparation();
		}
		this.loadSettings();
	}

	// Token: 0x060010D6 RID: 4310 RVA: 0x0000D089 File Offset: 0x0000B289
	public override void OnDestroy()
	{
		base.OnDestroy();
		PlayerPrefs.Save();
		App.ApplicationController.setConnectionCheckEnabled(true);
	}

	// Token: 0x060010D7 RID: 4311 RVA: 0x00071960 File Offset: 0x0006FB60
	private static bool useNewLogin()
	{
		if (App.IsStandalone)
		{
			return false;
		}
		string value = App.Config.globalSettings.user.name.value;
		return value != null && value.Contains("@");
	}

	// Token: 0x060010D8 RID: 4312 RVA: 0x0000D0A1 File Offset: 0x0000B2A1
	private bool ShouldUseLauncherLogin()
	{
		return App.StartedWithLauncher && MiniCommunicator.accessToken != null && !App.HasTriedFirstLogin;
	}

	// Token: 0x060010D9 RID: 4313 RVA: 0x000719A8 File Offset: 0x0006FBA8
	private bool ShouldUseAutomaticLogin()
	{
		if (!this._rememberMeChecked)
		{
			return false;
		}
		if (App.HasTriedFirstLogin)
		{
			return false;
		}
		if (App.StartedWithLauncher)
		{
			return false;
		}
		if (App.ApplicationController.NeedUpdate())
		{
			return false;
		}
		if (Application.isEditor && Input.GetKey(306))
		{
			return false;
		}
		if (Login.useNewLogin())
		{
			return PlayerPrefs.HasKey("accessToken");
		}
		return PlayerPrefs.HasKey("debugUsername");
	}

	// Token: 0x060010DA RID: 4314 RVA: 0x00071A28 File Offset: 0x0006FC28
	private void Start()
	{
		Application.targetFrameRate = 60;
		this.audioScript = App.AudioScript;
		this.loginGUISkin = (GUISkin)ResourceManager.Load("_GUISkins/Login");
		this.loginGUISkin.textField.alignment = 3;
		this.styleInfo = new GUIStyle(this.loginGUISkin.label);
		this.styleInfo.alignment = 5;
		this._emptySkin = (GUISkin)ResourceManager.Load("_GUISkins/EmptySkin");
		this._regularUI = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		this._closeButtonSkin = (GUISkin)ResourceManager.Load("_GUISkins/CloseButton");
		this.loadTextures();
		App.AudioScript.PlayMusic("Music/Theme");
		Input.eatKeyPressOnTextFieldFocus = false;
		this.comm = App.Communicator;
		this.comm.addListener(this);
		this.serversListFile = new TextFiler(Application.persistentDataPath + "/servers.txt");
		this.saveDefaultServerListIfNeeded();
		this.servers = LoginServer.serverListFromString(this.serversListFile.load());
		App.Config.ApplyResolution();
		if (App.IsStandalone)
		{
			App.ApplicationController.setConnectionCheckEnabled(false);
			this.setRegistrationState(Login.State.SelectServer);
		}
		base.StartCoroutine(this.Start2());
	}

	// Token: 0x060010DB RID: 4315 RVA: 0x00071B6C File Offset: 0x0006FD6C
	private IEnumerator Start2()
	{
		yield return base.StartCoroutine(App.ApplicationController.CheckForUpdateCoroutine());
		if (!App.IsStandalone)
		{
			this.setRegistrationState(Login.State.Login);
		}
		while (!this.comm.ReadyToUse)
		{
			if (App.IsStandalone && App.Communicator.lastLookupConnectResult == false)
			{
				App.Popups.ShowOk(this, "lookupFailed", "Connection failed", "Could not connect to the selected server.", "Ok");
				yield break;
			}
			yield return null;
		}
		if (App.IsStandalone)
		{
			this.setRegistrationState(Login.State.Login);
		}
		if (this.ShouldUseLauncherLogin())
		{
			App.HasTriedFirstLogin = true;
			this.isLauncherLogin = true;
			this.comm.sendLogin();
		}
		if (this.ShouldUseAutomaticLogin())
		{
			App.HasTriedFirstLogin = true;
			this.isRememberMeLogin = true;
			if (Login.useNewLogin())
			{
				base.StartCoroutine(this._newSignIn(true));
			}
			else
			{
				base.StartCoroutine(this._oldSignIn(true));
			}
		}
		else
		{
			this.password = string.Empty;
		}
		if (this.isLauncherLogin || this.isRememberMeLogin)
		{
			base.StartCoroutine("SignInTimeout");
		}
		this.didYouKnow = new GameObject("Did You Know").AddComponent<DidYouKnow>();
		this.overlay = new GameObject("Black Fade").AddComponent<GUIBlackOverlayButton>();
		this.overlay.Init(this, 5, false);
		this.overlay.SetAlpha(0f);
		this.overlay.enabled = false;
		if (!App.IsStandalone)
		{
			this.setRegistrationState(Login.State.Login);
		}
		App.ShowSignoutReasonIfAny();
		yield break;
	}

	// Token: 0x060010DC RID: 4316 RVA: 0x000028DF File Offset: 0x00000ADF
	public void OverlayClicked()
	{
	}

	// Token: 0x060010DD RID: 4317 RVA: 0x00071B88 File Offset: 0x0006FD88
	private void loadSettings()
	{
		this._rememberMeChecked = false;
		if (App.Config.globalSettings.user.isRemembered())
		{
			this._rememberMeChecked = true;
			this.username = App.Config.globalSettings.user.name;
		}
	}

	// Token: 0x060010DE RID: 4318 RVA: 0x0000D0C2 File Offset: 0x0000B2C2
	private void loadTextures()
	{
		this._imgBg = ResourceManager.LoadTexture("Login/login__0008_BG");
		this._imgRing = ResourceManager.LoadTexture("Login/_0002_ring");
		this._imgLogo = ResourceManager.LoadTexture("Logos/scrollslogo");
	}

	// Token: 0x060010DF RID: 4319 RVA: 0x00071BDC File Offset: 0x0006FDDC
	private void login()
	{
		if (this.signInSent)
		{
			return;
		}
		if (string.IsNullOrEmpty(this.username.Trim()))
		{
			return;
		}
		if (string.IsNullOrEmpty(this.password))
		{
			return;
		}
		base.StartCoroutine("beginSignin");
	}

	// Token: 0x060010E0 RID: 4320 RVA: 0x00071C28 File Offset: 0x0006FE28
	public static string validatePassword(string password, string confirmPassword, string username)
	{
		string text = "Your password must be at least 8 characters";
		if (string.IsNullOrEmpty(password))
		{
			return text + ".";
		}
		if (!new Regex("[^\\d]").IsMatch(password))
		{
			return text + ", and cannot consist of only digits.";
		}
		if (!new Regex("[^A-Z]").IsMatch(password))
		{
			return text + ", and cannot consist of only uppercase letters.";
		}
		if (!new Regex("[^a-z]").IsMatch(password))
		{
			return text + ", and cannot consist of only lowercase letters.";
		}
		if (!new Regex("[^\\W|_]").IsMatch(password))
		{
			return text + ", and cannot consist of only symbols.";
		}
		if (new Regex("\\s").IsMatch(password))
		{
			return text + ", and cannot contain whitespace symbols.";
		}
		if (password.Length < 8)
		{
			return text + ".";
		}
		if (password.Length > 256)
		{
			return "Your password cannot exceed 256 characters in length.";
		}
		string text2 = password.ToLowerInvariant();
		if (username != null && text2.Contains(username.ToLowerInvariant()))
		{
			return "You cannot include your username or email address in your password.";
		}
		string[] array = new string[]
		{
			"minecraft",
			"cobalt",
			"scrolls",
			"mojang",
			"qwerty",
			"12345",
			"123456",
			"1234567",
			"12345678",
			"12345679",
			"creeper",
			"monkey",
			"football",
			"abc123",
			"abc1234",
			"abc12345",
			"abc123456",
			"password",
			"canada",
			"fuckyou",
			"fuckme",
			"computer",
			"baseball",
			"dragon",
			"password1",
			"minecraft1",
			"jordan",
			"thomas",
			"master",
			"tigger",
			"696969",
			"qwerty123",
			"qwerty12",
			"qwerty1",
			"asdfasdf",
			"m1n3cr4ft"
		};
		foreach (string text3 in array)
		{
			if (text2.Contains(text3))
			{
				return "Your password cannot be used because it contains one or more restricted words.";
			}
		}
		if (password != confirmPassword)
		{
			return "Passwords do not match.";
		}
		return null;
	}

	// Token: 0x060010E1 RID: 4321 RVA: 0x00071ED4 File Offset: 0x000700D4
	private bool validateRegistrationInput()
	{
		string text = this.regEmail.Trim();
		string text2 = this.regUser.Trim();
		if (!App.IsStandalone && (string.IsNullOrEmpty(text) || !StringUtil.IsValidEmail(text)))
		{
			this.regError = "You must enter a valid email address.";
			return false;
		}
		if (App.IsStandalone && Enumerable.Contains<char>(text2, '@'))
		{
			this.regError = "Invalid character in display name: '@'";
			return false;
		}
		if (string.IsNullOrEmpty(text2))
		{
			this.regError = "You must choose a username.";
			return false;
		}
		if (text2.Length < 3 || text2.Length > 16)
		{
			this.regError = "Your username cannot be shorter than 3 characters or longer than 16 characters.";
			return false;
		}
		if (!new Regex("^\\w{3,16}").IsMatch(text2))
		{
			this.regError = "Valid characters in username are: letters, digits, _ (underscore).";
			return false;
		}
		string text3 = "Your password must be at least 8 characters";
		if (string.IsNullOrEmpty(this.regPwd))
		{
			this.regError = text3 + ".";
			return false;
		}
		if (!new Regex("[^\\d]").IsMatch(this.regPwd))
		{
			this.regError = text3 + ", and cannot consist of only digits.";
			return false;
		}
		if (!new Regex("[^A-Z]").IsMatch(this.regPwd))
		{
			this.regError = text3 + ", and cannot consist of only uppercase letters.";
			return false;
		}
		if (!new Regex("[^a-z]").IsMatch(this.regPwd))
		{
			this.regError = text3 + ", and cannot consist of only lowercase letters.";
			return false;
		}
		if (!new Regex("[^\\W|_]").IsMatch(this.regPwd))
		{
			this.regError = text3 + ", and cannot consist of only symbols.";
			return false;
		}
		if (new Regex("\\s").IsMatch(this.regPwd))
		{
			this.regError = text3 + ", and cannot contain whitespace symbols.";
			return false;
		}
		if (this.regPwd.Length < 8)
		{
			this.regError = text3 + ".";
			return false;
		}
		if (this.regPwd.Length > 256)
		{
			this.regError = "Your password cannot exceed 256 characters in length.";
			return false;
		}
		if (!App.IsStandalone && this.regPwd.ToLowerInvariant().Contains(text))
		{
			this.regError = "You cannot include your email address in your password.";
			return false;
		}
		string[] array = new string[]
		{
			"minecraft",
			"cobalt",
			"scrolls",
			"mojang",
			"qwerty",
			"12345",
			"123456",
			"1234567",
			"12345678",
			"12345679",
			"creeper",
			"monkey",
			"football",
			"abc123",
			"abc1234",
			"abc12345",
			"abc123456",
			"password",
			"canada",
			"fuckyou",
			"fuckme",
			"computer",
			"baseball",
			"dragon",
			"password1",
			"minecraft1",
			"jordan",
			"thomas",
			"master",
			"tigger",
			"696969",
			"qwerty123",
			"qwerty12",
			"qwerty1",
			"asdfasdf",
			"m1n3cr4ft"
		};
		foreach (string text4 in array)
		{
			if (this.regPwd.ToLowerInvariant().Contains(text4))
			{
				this.regError = "Your password cannot be used because it contains one or more restricted words.";
				return false;
			}
		}
		if (this.regPwd != this.regPwdConfirm)
		{
			this.regError = "Passwords do not match.";
			return false;
		}
		if (!this.regAcceptTerms)
		{
			this.regError = "You must accept the EULA, Terms & conditions and privacy policy to continue.";
			return false;
		}
		if (!App.IsStandalone && this.getDateOfBirth() == null)
		{
			this.regError = "You need to enter a birth date";
			return false;
		}
		return true;
	}

	// Token: 0x060010E2 RID: 4322 RVA: 0x00072318 File Offset: 0x00070518
	private void register()
	{
		if (this.registerSent)
		{
			return;
		}
		if (!this.validateRegistrationInput())
		{
			return;
		}
		this._rotateTime = Time.time;
		this.registerSent = true;
		this.regError = string.Empty;
		if (App.IsStandalone)
		{
			base.StartCoroutine("_registerStandaloneCoroutine");
		}
		else
		{
			base.StartCoroutine("_registerMojangCoroutine");
		}
	}

	// Token: 0x060010E3 RID: 4323 RVA: 0x00072384 File Offset: 0x00070584
	private IEnumerator beginSignin()
	{
		App.HasTriedFirstLogin = true;
		this.signInPreparation();
		MiniCommunicator.clearAllCredentials();
		App.Communicator.storeAuthHash(this.password);
		IEnumerator enumerator2;
		if (Login.useNewLogin())
		{
			IEnumerator enumerator = this._newSignIn(false);
			enumerator2 = enumerator;
		}
		else
		{
			enumerator2 = this._oldSignIn(false);
		}
		yield return base.StartCoroutine(enumerator2);
		yield break;
	}

	// Token: 0x060010E4 RID: 4324 RVA: 0x0000D0F4 File Offset: 0x0000B2F4
	private void signInPreparation()
	{
		this._rotateTime = Time.time;
		this.signInSent = true;
		this.errorMess = string.Empty;
	}

	// Token: 0x060010E5 RID: 4325 RVA: 0x000723A0 File Offset: 0x000705A0
	private IEnumerator _oldSignIn(bool isAutomaticLogin)
	{
		if (Application.isEditor && isAutomaticLogin)
		{
			App.Communicator.storeUserCredentials(PlayerPrefs.GetString("debugUsername"), PlayerPrefs.GetString("debugPassword"));
		}
		else
		{
			App.Communicator.storeUserCredentials(this.username.Trim(), this.password);
			if (this._rememberMeChecked)
			{
				string trimmedUser = this.username.Trim();
				if (Application.isEditor)
				{
					PlayerPrefs.SetString("debugUsername", trimmedUser);
					PlayerPrefs.SetString("debugPassword", this.password);
				}
			}
		}
		this.comm.sendLogin();
		base.StartCoroutine("SignInTimeout");
		yield break;
	}

	// Token: 0x060010E6 RID: 4326 RVA: 0x000723CC File Offset: 0x000705CC
	private IEnumerator _newSignIn(bool isRefresh)
	{
		PostRequest req;
		if (isRefresh)
		{
			if (!PlayerPrefs.HasKey("accessToken"))
			{
				this.failWith("Could not read access token. Please relogin.");
				yield break;
			}
			req = new RefreshRequest(Login.getAccessToken());
		}
		else
		{
			req = new AuthRequest(this.username.Trim(), this.password);
		}
		DateTime startTime = DateTime.Now;
		while (!req.isDone() && (DateTime.Now - startTime).TotalMilliseconds < 10000.0)
		{
			yield return new WaitForEndOfFrame();
		}
		if (!req.isDone())
		{
			this.failWith("Timeout contacting authentication server");
			yield break;
		}
		if (req.hasError())
		{
			this.failWith("Authentication failed: " + req.getError());
			if (req.getErrorCode() == 403 && req is AuthRequest)
			{
				this.showForgotPasswordButton = true;
			}
			yield break;
		}
		if (isRefresh)
		{
			RefreshRequest rr = (RefreshRequest)req;
			MiniCommunicator.accessToken = rr.getResponse();
			if (this._rememberMeChecked)
			{
				PlayerPrefs.SetString("accessToken", rr.getResponse().accessToken);
			}
		}
		else
		{
			AuthRequest ar = (AuthRequest)req;
			MiniCommunicator.accessToken = ar.getResponse();
			if (this._rememberMeChecked)
			{
				PlayerPrefs.SetString("accessToken", ar.getResponse().accessToken);
				PlayerPrefs.SetString("loggedInUser", ar.getResponse().user.id);
			}
		}
		this.comm.sendLogin();
		base.StartCoroutine("SignInTimeout");
		yield break;
	}

	// Token: 0x060010E7 RID: 4327 RVA: 0x000723F8 File Offset: 0x000705F8
	private IEnumerator _registerMojangCoroutine()
	{
		RegisterRequest req = new RegisterRequest(this.regEmail.Trim(), this.regPwd, this.regUser.Trim(), this.regAcceptsNewsletters, this.getDateOfBirth().Value);
		DateTime startTime = DateTime.Now;
		while (!req.isDone() && (DateTime.Now - startTime).TotalMilliseconds < 10000.0)
		{
			yield return new WaitForEndOfFrame();
		}
		if (!req.isDone())
		{
			this.failRegistrationWith("Timeout contacting registration server.");
			yield break;
		}
		if (req.hasError())
		{
			this.failRegistrationWith(req.getError());
			yield break;
		}
		this.registerSent = false;
		this.username = this.regEmail.Trim();
		this.password = string.Empty;
		App.Popups.ShowOk(this, "registrationok", "Account registered", I18n.Text("Thanks for signing up to play {GAME_NAME}! We've sent an email with instructions on how to activate your account. When you're done, press Ok to log in!"), "Ok");
		yield break;
	}

	// Token: 0x060010E8 RID: 4328 RVA: 0x00072414 File Offset: 0x00070614
	private IEnumerator _registerStandaloneCoroutine()
	{
		this.registerAccountReq = -1;
		this.registerAccountError = string.Empty;
		string user = this.regUser.Trim();
		this.comm.send(new RegisterAccountMessage(user, this.regPwd));
		DateTime startTime = DateTime.Now;
		yield return new WaitForEndOfFrame();
		while (this.registerAccountReq < 0 && (DateTime.Now - startTime).TotalMilliseconds < 10000.0)
		{
			yield return new WaitForEndOfFrame();
		}
		if (this.registerAccountReq < 0)
		{
			this.failRegistrationWith("Timeout contacting registration server.");
			yield break;
		}
		if (this.registerAccountReq == 0)
		{
			this.failRegistrationWith("Registration failed: " + this.registerAccountError);
			yield break;
		}
		this.registerSent = false;
		this.username = this.regUser.Trim();
		this.password = string.Empty;
		App.Popups.ShowOk(this, "registrationok", "Account registered", I18n.Text("Thanks for signing up to play {GAME_NAME}!\n\nPlease note that Mojang and Microsoft are not affiliated with this community server. We take no responsibility for any content or modifications it may contain.\n\nWhen you're done, press Ok to log in!"), "Ok");
		App.Popups.SetLarge(true);
		yield break;
	}

	// Token: 0x060010E9 RID: 4329 RVA: 0x00072430 File Offset: 0x00070630
	private IEnumerator SignInTimeout()
	{
		yield return new WaitForSeconds(15f);
		this.signInSent = false;
		this.password = string.Empty;
		this.isLauncherLogin = false;
		this.isRememberMeLogin = false;
		App.Popups.ShowOk(this, "signintimeout", "Connection issue", "There seems to be a problem connecting to the server. Please visit help.mojang.com for assistance if the issue persists.", "Ok");
		yield break;
	}

	// Token: 0x060010EA RID: 4330 RVA: 0x0007244C File Offset: 0x0007064C
	private void Update()
	{
		if (this._state == 3)
		{
			float num = 0.7f;
			float num2 = Time.time - this._fadeStartTime;
			this._fadeTime = num2 / num;
			if (this._fadeTime > 1f)
			{
				this._fadeTime = 1f;
			}
			this._ringScale = 1f + this._fadeTime;
		}
		if (this.didYouKnow != null && !this.requestedDidYouKnow && this.comm.ReadyToUse && !this.isLauncherLogin && !this.isRememberMeLogin)
		{
			this.didYouKnow.Init();
			this.requestedDidYouKnow = true;
		}
	}

	// Token: 0x060010EB RID: 4331 RVA: 0x00072504 File Offset: 0x00070704
	private void OnGUI()
	{
		GUI.depth = 13;
		this.styleInfo.fontSize = Screen.height / 40;
		if (this.signInSent || this.registerSent)
		{
			this._rotateAcc = 7.5f + (Time.time - this._rotateTime) * (Time.time - this._rotateTime) * 25f;
		}
		else
		{
			this._rotateAcc -= 25f * Time.deltaTime;
		}
		this._rotateAcc = Mth.clamp(this._rotateAcc, 7.5f, 250f);
		GUI.enabled = (!this.signInSent && !this.registerSent);
		GUI.skin = this.loginGUISkin;
		GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), this._imgBg);
		this.angle += Time.deltaTime * this._rotateAcc;
		Vector2 vector = this.mockUnit.p(0.5f, 0.5f);
		GUIUtility.RotateAroundPivot(this.angle, vector);
		Vector2 vector2 = this.mockUnit.p(this._ringScale * 0.1823f, this._ringScale * 0.1823f * (float)Screen.width / (float)Screen.height);
		GUI.color = new Color(1f, 1f, 1f, 0.25f);
		GUI.DrawTexture(new Rect(vector.x - vector2.x, vector.y - vector2.y, vector2.x + vector2.x, vector2.y + vector2.y), this._imgRing);
		GUI.color = Color.white;
		GUIUtility.RotateAroundPivot(-this.angle, vector);
		if (!this.isRegistration)
		{
			float num = 200f;
			float num2 = num * (float)this._imgLogo.width / (float)this._imgLogo.height;
			Rect rect = this.mockJunk.rAspectW(964f - 0.5f * num2, 180f, num2, num);
			GUI.DrawTexture(rect, this._imgLogo);
		}
		if (!App.IsStandalone && App.Communicator.UseHost == Host.Amazon_Test)
		{
			GUI.Label(new Rect((float)Screen.width * 0.69f, (float)Screen.height * 0.91f, (float)Screen.width * 0.3f, (float)Screen.height * 0.03f), "[TEST SERVER]", this.styleInfo);
		}
		string text = "Version " + SharedConstants.getGameVersion();
		if (App.IsStandalone)
		{
			text += " - Standalone version";
		}
		GUI.Label(new Rect((float)Screen.width * 0.69f, (float)Screen.height * 0.935f, (float)Screen.width * 0.3f, (float)Screen.height * 0.03f), text, this.styleInfo);
		GUI.Label(new Rect((float)Screen.width * 0.69f, (float)Screen.height * 0.96f, (float)Screen.width * 0.3f, (float)Screen.height * 0.03f), "© 2012 - 2015 Mojang AB. All rights reserved.", this.styleInfo);
		int fontSize = this._regularUI.button.fontSize;
		int fontSize2 = this._regularUI.label.fontSize;
		Color textColor = this._regularUI.button.normal.textColor;
		GUIStyle button = this._regularUI.button;
		int fontSize3 = Screen.height / 50;
		this._regularUI.label.fontSize = fontSize3;
		button.fontSize = fontSize3;
		GUIStyleState normal = this._regularUI.button.normal;
		Color color = Color.black;
		this._regularUI.button.active.textColor = color;
		color = color;
		this._regularUI.button.hover.textColor = color;
		normal.textColor = color;
		if (this.isLauncherLogin || this.isRememberMeLogin)
		{
			GUIStyle guistyle = new GUIStyle(this.loginGUISkin.label);
			guistyle.fontSize = Screen.height / 24;
			guistyle.alignment = 1;
			guistyle.normal.textColor = Color.white;
			GUI.Label(this.mockJunk.r(0f, 515f, 1920f, 100f), "<color=#ccbb95>Loading...</color>", guistyle);
		}
		else if (this.state == Login.State.Login)
		{
			this.OnGUI_Login();
		}
		else if (this.state == Login.State.SelectServer)
		{
			this.OnGUI_SelectServer();
		}
		else if (this.state == Login.State.Registration)
		{
			this.OnGUI_Register();
		}
		this._regularUI.button.fontSize = fontSize;
		this._regularUI.label.fontSize = fontSize2;
		GUIStyleState normal2 = this._regularUI.button.normal;
		color = textColor;
		this._regularUI.button.active.textColor = color;
		color = color;
		this._regularUI.button.hover.textColor = color;
		normal2.textColor = color;
		float num3 = (float)Screen.height * 0.03f;
		if (GUI.Button(new Rect((float)Screen.width - num3, 0f, num3, num3), string.Empty, this._closeButtonSkin.button))
		{
			Application.Quit();
		}
		if (!this.isRegistration && this.errorMess != string.Empty)
		{
			GUISkin skin = GUI.skin;
			GUI.skin = this.loginGUISkin;
			TextAnchor alignment = GUI.skin.label.alignment;
			int fontSize4 = GUI.skin.label.fontSize;
			GUI.skin.label.alignment = 1;
			GUI.skin.label.fontSize = Screen.height / 40;
			GUI.Label(new Rect(0f, 0.669f * (float)Screen.height, (float)Screen.width, (float)(Screen.height / 15)), "<color=#ff6655>" + this.errorMess + "</color>");
			GUI.skin.label.alignment = alignment;
			GUI.skin.label.fontSize = fontSize4;
			GUI.skin = skin;
		}
		if (this.isRegistration && this.regError != string.Empty)
		{
			GUISkin skin2 = GUI.skin;
			GUI.skin = this.loginGUISkin;
			TextAnchor alignment2 = GUI.skin.label.alignment;
			int fontSize5 = GUI.skin.label.fontSize;
			GUI.skin.label.alignment = 1;
			GUI.skin.label.fontSize = Screen.height / 50;
			GUI.Label(new Rect(0f, 0.79f * (float)Screen.height, (float)Screen.width, (float)(Screen.height / 35)), "<color=#ff6655>" + this.regError + "</color>");
			GUI.skin.label.alignment = alignment2;
			GUI.skin.label.fontSize = fontSize5;
			GUI.skin = skin2;
		}
		Texture2D texture2D = ResourceManager.LoadTexture("Logos/mojang_logo_bw");
		float num4 = (float)Screen.height * 0.023f;
		float num5 = num4 * (float)texture2D.width / (float)texture2D.height;
		Rect rect2;
		rect2..ctor((float)Screen.height * 0.02f, (float)Screen.height * 0.98f - num4, num5, num4);
		GUI.color = Color.white;
		GUI.DrawTexture(rect2, texture2D);
		GUI.enabled = true;
		if (this._state == 3)
		{
			float num6 = this._fadeTime;
			if (num6 > 1f)
			{
				num6 = 1f;
			}
			this.overlay.SetAlpha(num6);
			if (num6 >= 1f && !this._initiated)
			{
				this._initiated = true;
				App.ChatUI.Initiate();
				App.GameActionManager.Initiate();
				App.LobbyMenu.SetEnabled(true);
				App.AssetLoader.Init();
				SceneLoader.loadScene(this._sceneToLoad);
			}
		}
	}

	// Token: 0x060010EC RID: 4332 RVA: 0x0000D113 File Offset: 0x0000B313
	private string drawTextBox(string name, Rect rect, string text, bool isPassword)
	{
		return Login.drawTextBox(this._emptySkin.button, name, rect, text, isPassword);
	}

	// Token: 0x060010ED RID: 4333 RVA: 0x00072D34 File Offset: 0x00070F34
	public static string drawTextBox(GUIStyle style, string name, Rect rect, string text, bool isPassword)
	{
		GUI.SetNextControlName(name);
		Rect rect2;
		rect2..ctor(rect);
		int fontSize = style.fontSize;
		style.fontSize = (int)((float)(28 * Screen.height) / 1080f);
		GUI.color = new Color(1f, 1f, 1f, 0.5f);
		GUI.Box(rect2, string.Empty);
		GUI.color = Color.white;
		float num = 0.0056f * (float)Screen.width;
		float num2 = 0.007f * (float)Screen.height;
		rect2.x += num;
		rect2.y += num2;
		rect2.width -= 2f * num;
		rect2.height -= 2f * num2;
		string result = (!isPassword) ? GUI.TextField(rect2, text, style) : GUI.PasswordField(rect2, text, '*', style);
		style.fontSize = fontSize;
		return result;
	}

	// Token: 0x060010EE RID: 4334 RVA: 0x0000D12A File Offset: 0x0000B32A
	private void scheduleLoadScene(string sceneToLoad)
	{
		if (this._state != 3 && sceneToLoad != null)
		{
			this.overlay.enabled = true;
			this._state = 3;
			this._sceneToLoad = sceneToLoad;
			this._fadeStartTime = Time.time;
		}
	}

	// Token: 0x060010EF RID: 4335 RVA: 0x00072E2C File Offset: 0x0007102C
	private void failWith(string error)
	{
		if (error.StartsWith(I18n.Text("Could not find a Scrolls profile")))
		{
			error = I18n.Text("<color=#ccbbaa>No {GAME_NAME} profile found. <color=#ffcc66>Try the demo</color> or <color=#ffcc66>get the game!</color>\nIf you've just created a demo account, please make sure to <color=#ffcc66>verify your email.</color></color>");
			this.showNoProfile = true;
		}
		else
		{
			this.showNoProfile = false;
		}
		this.errorMess = error;
		this.signInSent = false;
		this.isLauncherLogin = false;
		this.isRememberMeLogin = false;
		this.password = string.Empty;
		base.StopCoroutine("SignInTimeout");
		Login.clearAccessToken();
	}

	// Token: 0x060010F0 RID: 4336 RVA: 0x00072EA4 File Offset: 0x000710A4
	private void failRegistrationWith(string error)
	{
		this.regError = error;
		this.registerSent = false;
		this.regPwd = (this.regPwdConfirm = string.Empty);
	}

	// Token: 0x060010F1 RID: 4337 RVA: 0x00072ED4 File Offset: 0x000710D4
	public override void handleMessage(Message msg)
	{
		if (msg is FailMessage)
		{
			FailMessage failMessage = (FailMessage)msg;
			if (failMessage.isType(typeof(RegisterAccountMessage)))
			{
				this.registerAccountReq = 0;
				this.registerAccountError = failMessage.info;
			}
			else
			{
				base.StopCoroutine("SignInTimeout");
				this.failWith(failMessage.info);
				if (failMessage.isType(typeof(FirstConnectMessage)))
				{
					Login.clearAccessToken();
				}
			}
		}
		if (msg is MessageMessage && ((MessageMessage)msg).isStarterDeck())
		{
			App.SceneValues.selectPreconstructed.Add(((MessageMessage)msg).type);
		}
		if (msg is OkMessage)
		{
			OkMessage okMessage = (OkMessage)msg;
			if (okMessage.isType(typeof(RegisterAccountMessage)))
			{
				this.registerAccountReq = 1;
			}
			else if (okMessage.isType(typeof(JoinLobbyMessage)))
			{
				base.StopCoroutine("SignInTimeout");
				this.onLoginComplete();
			}
		}
		base.handleMessage(msg);
	}

	// Token: 0x060010F2 RID: 4338 RVA: 0x0000D163 File Offset: 0x0000B363
	private void onLoginComplete()
	{
		if (this.debugLoginStayOnScene)
		{
			return;
		}
		if (App.SceneValues.selectPreconstructed.IsEmpty())
		{
			this.scheduleLoadScene("_HomeScreen");
		}
		else
		{
			this.scheduleLoadScene("_SelectPreconstructed");
		}
	}

	// Token: 0x060010F3 RID: 4339 RVA: 0x0000D1A0 File Offset: 0x0000B3A0
	public static string getAccessToken()
	{
		return PlayerPrefs.GetString("accessToken", null);
	}

	// Token: 0x060010F4 RID: 4340 RVA: 0x0000D1AD File Offset: 0x0000B3AD
	public static void clearAccessToken()
	{
		MiniCommunicator.accessToken = null;
		Log.info("Clearing access token");
		PlayerPrefs.DeleteKey("accessToken");
		PlayerPrefs.DeleteKey("loggedInUser");
		PlayerPrefs.DeleteKey("debugUsername");
		PlayerPrefs.DeleteKey("debugPassword");
	}

	// Token: 0x060010F5 RID: 4341 RVA: 0x00072FE8 File Offset: 0x000711E8
	public void PopupOk(string popupType)
	{
		if (popupType == "registrationUserAccept")
		{
			this.register();
		}
		if (popupType == "registrationok")
		{
			this.setRegistrationState(Login.State.Login);
		}
		if (popupType == "lookupFailed")
		{
			App.SignOut();
		}
	}

	// Token: 0x060010F6 RID: 4342 RVA: 0x00073038 File Offset: 0x00071238
	private void OnGUI_Login()
	{
		if (App.IsStandalone && this.currentServer != null)
		{
			GUI.Label(new Rect((float)Screen.width * 0.69f, (float)Screen.height * 0.91f, (float)Screen.width * 0.3f, (float)Screen.height * 0.03f), "Server '" + this.currentServer.name + "'", this.styleInfo);
		}
		GUIStyle guistyle = new GUIStyle(this.loginGUISkin.label);
		guistyle.fontSize = Screen.height / 38;
		float num = 772f;
		GUI.Label(this.mockJunk.r(num, 420f, 422f, 60f), (!App.IsStandalone) ? "Email" : "Display name", guistyle);
		GUI.Label(this.mockJunk.r(num, 519f, 422f, 60f), "Password", guistyle);
		GUI.Label(this.mockJunk.r(num, 614f, 422f, 60f), "Remember me", guistyle);
		this.username = this.drawTextBox("username", this.mockJunk.r(762f, 457f, 412f, 47f), this.username, false);
		this.password = this.drawTextBox("password", this.mockJunk.r(762f, 554f, 412f, 47f), this.password, true);
		App.Config.globalSettings.user.name.value = ((!this._rememberMeChecked) ? string.Empty : this.username);
		App.Config.globalSettings.user.server_index.value = this.servers.IndexOf(this.currentServer);
		if (!Login.focusInited && !this.isLauncherLogin && !this.isRememberMeLogin)
		{
			string nameOfFocusedControl = GUI.GetNameOfFocusedControl();
			if (this.username != string.Empty && this.password == string.Empty)
			{
				GUI.FocusControl("password");
			}
			else
			{
				GUI.FocusControl("username");
			}
			Login.focusInited = true;
		}
		if ((GUI.GetNameOfFocusedControl() == "password" || GUI.GetNameOfFocusedControl() == "username") && (Input.GetKeyDown(13) || (Input.GetKeyDown(271) && !App.Popups.IsShowingPopup())) && this.comm.ReadyToUse)
		{
			this.login();
		}
		GUI.skin = this._emptySkin;
		float x = num + 1.1f * guistyle.CalcSize(new GUIContent("Remember me")).x * 1920f / (float)Screen.width;
		Rect rect = this.mockJunk.r(x, 624f, 22f, 23f);
		if (this._rememberMeChecked)
		{
			if (GUI.Button(rect, ResourceManager.LoadTexture("Login/login__0000_check_true")))
			{
				this._rememberMeChecked = !this._rememberMeChecked;
			}
		}
		else if (GUI.Button(rect, ResourceManager.LoadTexture("Login/login__0001_check_false")))
		{
			this._rememberMeChecked = !this._rememberMeChecked;
		}
		GUI.enabled = this.comm.ReadyToUse;
		if (LobbyMenu.drawButton(LobbyMenu.getButtonPositioner(1f, 0f).getButtonRect(0f, this.mockJunk.Y(670f)), "Log in", this._regularUI))
		{
			this.login();
		}
		if (this.showNoProfile)
		{
			if (LobbyMenu.drawButton(LobbyMenu.getButtonPositioner(3f, 10f, 150f).getButtonRect(0f, this.mockJunk.Y(790f)), "Try demo", this._regularUI))
			{
				Application.OpenURL("https://account.mojang.com/demo/scrolls?agent=scrolls");
			}
			if (LobbyMenu.drawButton(LobbyMenu.getButtonPositioner(3f, 10f, 150f).getButtonRect(1f, this.mockJunk.Y(790f)), "Buy game", this._regularUI))
			{
				Application.OpenURL("https://scrolls.com/buy");
			}
			if (LobbyMenu.drawButton(LobbyMenu.getButtonPositioner(3f, 10f, 150f).getButtonRect(2f, this.mockJunk.Y(790f)), "Verify email", this._regularUI))
			{
				Application.OpenURL("https://account.mojang.com/me/mustVerifyEmail");
			}
		}
		else
		{
			int num2 = (!this.showForgotPasswordButton) ? 0 : 1;
			bool flag = true;
			if (flag && LobbyMenu.drawButton(LobbyMenu.getButtonPositioner((float)(1 + num2), 10f, 150f).getButtonRect(0f, this.mockJunk.Y(770f)), "Need an account?", this._regularUI))
			{
				this.setRegistrationState(Login.State.Registration);
			}
			if (this.showForgotPasswordButton && LobbyMenu.drawButton(LobbyMenu.getButtonPositioner((float)((!flag) ? 1 : 2), 10f, 150f).getButtonRect((float)((!flag) ? 0 : 1), this.mockJunk.Y(770f)), "Forgot password?", this._regularUI))
			{
				Application.OpenURL("https://account.mojang.com/password");
			}
		}
		GUI.enabled = true;
	}

	// Token: 0x060010F7 RID: 4343 RVA: 0x000735DC File Offset: 0x000717DC
	private void OnGUI_SelectServer()
	{
		this.centerServerDrop();
		Rect buttonRect = LobbyMenu.getButtonPositioner(1f, 0f).getButtonRect(0f, this.mockJunk.Y(720f));
		Rect buttonRect2 = LobbyMenu.getButtonPositioner(1f, 0f).getButtonRect(0f, this.mockJunk.Y(570f));
		if (LobbyMenu.drawButton(buttonRect2, "Connect", this._regularUI))
		{
			this.HandleServerDropDropdownChangedEvent(this.serverDrop.GetSelectedIndex(), this.serverDrop.GetSelectedItem());
		}
		if (LobbyMenu.drawButton(buttonRect, "Edit servers", this._regularUI))
		{
			string[] array = Enumerable.ToArray<string>(Enumerable.Select<LoginServer, string>(this.servers, (LoginServer x) => x.name + " # " + x.address.ToString()));
			App.Popups.ShowTextArea(this, "lookupservers", "Manage Servers", "One server per line: Name # address. Example for a local computer server: My Server # localhost", "Ok", "Cancel", string.Join("\n", array), true, true);
		}
	}

	// Token: 0x060010F8 RID: 4344 RVA: 0x000736EC File Offset: 0x000718EC
	private void OnGUI_Register()
	{
		float num = 752f;
		float num2 = num + 10f;
		GUIStyle guistyle = new GUIStyle(this.loginGUISkin.label);
		guistyle.fontSize = Screen.height / 20;
		GUI.Label(this.mockJunk.r(num2, 213f, 422f, 60f), "Register account", guistyle);
		GUIStyle guistyle2 = new GUIStyle(this.loginGUISkin.label);
		guistyle2.fontSize = Screen.height / 38;
		float num3 = 628f;
		int num4 = 80;
		int num5;
		if (App.IsStandalone)
		{
			GUI.Label(this.mockJunk.r(num2, 303f, 422f, 60f), "Display name", guistyle2);
			this.regUser = this.drawTextBox("regUser", this.mockJunk.r(num, 340f, 412f, 47f), this.regUser, false);
			num3 -= 215f;
			num4 = 80;
			num5 = 64;
		}
		else
		{
			GUI.Label(this.mockJunk.r(num2, 303f, 422f, 60f), "Email (needs verification)", guistyle2);
			GUI.Label(this.mockJunk.r(num2, 503f, 422f, 60f), "Display name", guistyle2);
			GUI.Label(this.mockJunk.r(num2, 608f, 422f, 60f), "Date of birth", guistyle2);
			this.regEmail = this.drawTextBox("regEmail", this.mockJunk.r(num, 340f, 412f, 47f), this.regEmail, false);
			this.regUser = this.drawTextBox("regUser", this.mockJunk.r(num, 540f, 412f, 47f), this.regUser, false);
			num5 = 172;
		}
		GUI.Label(this.mockJunk.r(num2, 393f, 200f, 60f), "Password", guistyle2);
		GUI.Label(this.mockJunk.r(num2 + 212f, 393f, 200f, 60f), "Repeat", guistyle2);
		this.regPwd = this.drawTextBox("regPwd", this.mockJunk.r(num, 430f, 200f, 47f), this.regPwd, true);
		this.regPwdConfirm = this.drawTextBox("regPwdConfirm", this.mockJunk.r(num + 212f, 430f, 200f, 47f), this.regPwdConfirm, true);
		if ((GUI.GetNameOfFocusedControl() == "regEmail" || GUI.GetNameOfFocusedControl() == "regUser" || GUI.GetNameOfFocusedControl() == "regPwd" || GUI.GetNameOfFocusedControl() == "regPwdConfirm") && (Input.GetKeyDown(13) || (Input.GetKeyDown(271) && !App.Popups.IsShowingPopup())) && this.comm.ReadyToUse)
		{
			App.Popups.ShowOkCancel(this, "registrationUserAccept", "Account information", "The server stores data about your cards, match history, settings, ingame store purchases, and other things to allow the game to work normally and for your progress to be persistent. The game does not collect any personal data or tie this to you; all information is connected only to your username and password.\n\nPlease confirm that you are at least 16 years of age, and that you agree with the above.", "Agree", "Cancel");
		}
		GUI.skin = this._emptySkin;
		if (!App.IsStandalone)
		{
			int fontSize = GUI.skin.button.fontSize;
			int fontSize2 = this._regularUI.label.fontSize;
			GUIStyle button = GUI.skin.button;
			int fontSize3 = Screen.height / 60;
			this._regularUI.label.fontSize = fontSize3;
			button.fontSize = fontSize3;
			if (GUI.Button(this.mockJunk.r(num2, num3 + (float)num4, 105f, 45f), I18n.Text("<color=#ffcc55>{GAME_NAME} EULA</color>")))
			{
				Application.OpenURL("https://account.mojang.com/documents/scrolls_eula");
			}
			if (GUI.Button(this.mockJunk.r(num2 + 125f, num3 + (float)num4, 140f, 45f), "<color=#ffcc55>Terms & conditions</color>"))
			{
				Application.OpenURL("https://account.mojang.com/terms");
			}
			if (GUI.Button(this.mockJunk.r(num2 + 295f, num3 + (float)num4, 130f, 45f), "<color=#ffcc55>Privacy policy</color>"))
			{
				Application.OpenURL("https://account.mojang.com/terms#privacy");
			}
			GUI.skin.button.fontSize = fontSize;
			this._regularUI.label.fontSize = fontSize2;
			bool flag = AspectRatio.now.isNarrower(AspectRatio._3_2);
			int num6 = (!flag) ? 0 : 10;
			Rect checkboxRect = this.mockJunk.r(1130f, 42f + num3 + (float)num4, 44f, 40f);
			Rect checkboxRect2 = this.mockJunk.r(1130f, 97f + num3 + (float)num4, 44f, 40f);
			Rect labelRect = this.mockJunk.r(num2, 42f + num3 + (float)num4, 380f, 42f);
			if (flag)
			{
				labelRect.x -= (float)num6;
				labelRect.width += (float)(2 * num6);
				checkboxRect.x += (float)num6;
				checkboxRect2.x += (float)num6;
			}
			if (labelRect.xMax > checkboxRect.x)
			{
				labelRect.xMax = checkboxRect.x;
			}
			if (new Checkbox(I18n.Text("I accept the {GAME_NAME} EULA and the Terms and Conditions (including the Privacy Policy)"), labelRect, checkboxRect, this._regularUI, 0.7f).Draw(ref this.regAcceptTerms))
			{
			}
			if (!App.IsStandalone)
			{
				labelRect.y = this.mockJunk.Y((float)(725 + num4));
				if (new Checkbox("I want Mojang to send me news and updates by email", labelRect, checkboxRect2, this._regularUI, 0.7f).Draw(ref this.regAcceptsNewsletters))
				{
				}
			}
		}
		GUI.skin = (GUISkin)ResourceManager.Load("_GUISkins/EmptySkin");
		GUI.enabled = this.comm.ReadyToUse;
		if (LobbyMenu.drawButton(LobbyMenu.getButtonPositioner(2f, 10f, 150f).getButtonRect(0f, this.mockJunk.Y((float)num5 + num3 + (float)num4)), "Register", this._regularUI) && this.validateRegistrationInput())
		{
			App.Popups.ShowOkCancel(this, "registrationUserAccept", "Account information", "The server stores data about your cards, match history, settings, ingame store purchases, and other things to allow the game to work normally and for your progress to be persistent. The game does not collect any personal data or tie this to you; all information is connected only to your username and password.\n\nPlease confirm that you are at least 16 years of age, and that you agree with the above.", "Agree", "Cancel");
		}
		if (LobbyMenu.drawButton(LobbyMenu.getButtonPositioner(2f, 10f, 150f).getButtonRect(1f, this.mockJunk.Y((float)num5 + num3 + (float)num4)), "Cancel", this._regularUI))
		{
			this.setRegistrationState(Login.State.Login);
		}
		GUI.enabled = true;
	}

	// Token: 0x060010F9 RID: 4345 RVA: 0x0000D1E7 File Offset: 0x0000B3E7
	public override void onConnect(OnConnectData data)
	{
		if (data.type == OnConnectData.ConnectType.Connect)
		{
			this.setRegistrationState(Login.State.Login);
		}
		base.onConnect(data);
	}

	// Token: 0x060010FA RID: 4346 RVA: 0x000028DF File Offset: 0x00000ADF
	private void forceSetRegistrationState(Login.State newState)
	{
	}

	// Token: 0x060010FB RID: 4347 RVA: 0x00073DE8 File Offset: 0x00071FE8
	private void setRegistrationState(Login.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		this.isRegistration = (newState == Login.State.Registration);
		this.state = newState;
		Object.Destroy(this.serverDrop);
		Object.Destroy(this.yearDrop);
		Object.Destroy(this.monthDrop);
		Object.Destroy(this.dayDrop);
		if (this.didYouKnow != null)
		{
			this.didYouKnow.SetActive(this.state == Login.State.Login);
		}
		if (this.state == Login.State.Registration)
		{
			this.setupDropdowns();
			this.regAcceptTerms = App.IsStandalone;
			this.regError = string.Empty;
		}
		else if (this.state == Login.State.SelectServer)
		{
			this.setupServerDrop();
			this.comm.setMaxReconnectTries(0);
		}
		else if (this.state == Login.State.Login)
		{
			this.comm.setMaxReconnectTries(-1);
		}
	}

	// Token: 0x060010FC RID: 4348 RVA: 0x00073ECC File Offset: 0x000720CC
	private void setupServerDrop()
	{
		if (!App.IsStandalone)
		{
			return;
		}
		Object.Destroy(this.serverDrop);
		List<string> list = Enumerable.ToList<string>(Enumerable.Select<LoginServer, string>(this.servers, (LoginServer x) => x.name));
		if (list.Count == 0)
		{
			list.Add(string.Empty);
		}
		this.serverDrop = new GameObject("server").AddComponent<Dropdown>();
		this.centerServerDrop();
		this.serverDrop.Init(list.ToArray(), 5f, true, true, 10);
		this.serverDrop.SetNothingSelectedIndex(list.Count - 1);
		this.serverDrop.SetSelectedIndex(App.Config.globalSettings.user.server_index);
		this.serverDrop.SetEnabled(true);
	}

	// Token: 0x060010FD RID: 4349 RVA: 0x00073FAC File Offset: 0x000721AC
	private void centerServerDrop()
	{
		Rect rect = GUIUtil.centeredScreen(0.2f, 0.05f);
		rect.height = LobbyMenu.getButtonPositioner(1f, 0f).getButtonRect(0f, 0f).height;
		this.serverDrop.SetRect(rect);
	}

	// Token: 0x060010FE RID: 4350 RVA: 0x00074004 File Offset: 0x00072204
	private void HandleServerDropDropdownChangedEvent(int selectedIndex, string selection)
	{
		if (selectedIndex < 0 || this.servers.Count == 0)
		{
			return;
		}
		if (selectedIndex == this.servers.Count)
		{
			string[] array = Enumerable.ToArray<string>(Enumerable.Select<LoginServer, string>(this.servers, (LoginServer x) => x.name + " # " + x.address.ToString()));
			App.Popups.ShowTextArea(this, "lookupservers", "Manage Servers", "One server per line: Name # address. Example for a local computer server: My Server # localhost", "Ok", "Cancel", string.Join("\n", array), true, true);
			return;
		}
		this.currentServer = this.servers[selectedIndex];
		App.Config.globalSettings.user.server_index.value = selectedIndex;
		if (string.IsNullOrEmpty(this.currentServer.address.ip))
		{
			return;
		}
		IpPort address = this.currentServer.address;
		if (address.Equals(App.Communicator.getAddress()))
		{
			return;
		}
		base.StartCoroutine(App.Communicator.connectToLookup(address));
	}

	// Token: 0x060010FF RID: 4351 RVA: 0x00074118 File Offset: 0x00072318
	private void setupDropdowns()
	{
		if (App.IsStandalone)
		{
			return;
		}
		float num = (float)Screen.height * 0.03f;
		int num2 = (int)(num * 3.5f);
		float num3 = 0.008f * (float)Screen.height;
		int num4 = 752;
		this.yearDrop = new GameObject("year").AddComponent<Dropdown>();
		this.monthDrop = new GameObject("month").AddComponent<Dropdown>();
		this.dayDrop = new GameObject("day").AddComponent<Dropdown>();
		Rect rect = this.mockJunk.r((float)num4, 650f, 130f, 35f);
		this.dayDrop.SetRect(rect);
		rect.x += rect.width + num3;
		this.monthDrop.SetRect(rect);
		rect.x += rect.width + num3;
		this.yearDrop.SetRect(rect);
		DateTime now = DateTime.Now;
		List<string> list = new List<string>();
		list.Add("YYYY");
		for (int i = 0; i < 100; i++)
		{
			list.Add(string.Empty + (now.Year - i));
		}
		Login.initDateDropdown(this.yearDrop, list.ToArray());
		Login.initDateDropdown(this.monthDrop, Login.dateStrings("MM", 12));
		Login.initDateDropdown(this.dayDrop, Login.dateStrings("DD", 31));
		this.yearDrop.SetEnabled(true);
		this.monthDrop.SetEnabled(true);
		this.dayDrop.SetEnabled(true);
		this.yearDrop.DropdownChangedEvent += this.HandleYearOrMonthDropDropdownChangedEvent;
		this.monthDrop.DropdownChangedEvent += this.HandleYearOrMonthDropDropdownChangedEvent;
	}

	// Token: 0x06001100 RID: 4352 RVA: 0x000742F0 File Offset: 0x000724F0
	private void HandleYearOrMonthDropDropdownChangedEvent(int selectedIndex, string selection)
	{
		int num;
		int num2;
		if (!int.TryParse(this.yearDrop.GetSelectedItem(), ref num) || !int.TryParse(this.monthDrop.GetSelectedItem(), ref num2))
		{
			return;
		}
		int upto = DateTime.DaysInMonth(num, num2);
		Login.initDateDropdown(this.dayDrop, Login.dateStrings("DD", upto));
	}

	// Token: 0x06001101 RID: 4353 RVA: 0x0000D203 File Offset: 0x0000B403
	private static void initDateDropdown(Dropdown dateDrop, string[] values)
	{
		dateDrop.Init(values, 6f, true, true, 12);
	}

	// Token: 0x06001102 RID: 4354 RVA: 0x0007434C File Offset: 0x0007254C
	private static string[] dateStrings(string header, int upto)
	{
		List<string> list = new List<string>();
		list.Add(header);
		for (int i = 1; i <= upto; i++)
		{
			list.Add(i.ToString("00"));
		}
		return list.ToArray();
	}

	// Token: 0x06001103 RID: 4355 RVA: 0x00074390 File Offset: 0x00072590
	private DateTime? getDateOfBirth()
	{
		int num;
		int num2;
		int num3;
		if (int.TryParse(this.yearDrop.GetSelectedItem(), ref num) && int.TryParse(this.monthDrop.GetSelectedItem(), ref num2) && int.TryParse(this.dayDrop.GetSelectedItem(), ref num3))
		{
			return new DateTime?(new DateTime(num, num2, num3));
		}
		return default(DateTime?);
	}

	// Token: 0x06001104 RID: 4356 RVA: 0x000028DF File Offset: 0x00000ADF
	public void PopupCancel(string popupType)
	{
	}

	// Token: 0x06001105 RID: 4357 RVA: 0x000743FC File Offset: 0x000725FC
	public void PopupOk(string popupType, string choice)
	{
		if (popupType == "lookupservers")
		{
			if (!choice.Contains("\r\n"))
			{
				choice = choice.Replace("\n", "\r\n");
			}
			this.servers = LoginServer.serverListFromString(choice);
			this.serversListFile.save(choice);
			this.setupServerDrop();
		}
	}

	// Token: 0x06001106 RID: 4358 RVA: 0x0007445C File Offset: 0x0007265C
	private void saveDefaultServerListIfNeeded()
	{
		if (!this.serversListFile.exists())
		{
			string path = Path.Combine(Application.dataPath, "default_servers.txt");
			string text = new TextFiler(path).load();
			this.serversListFile.save(text ?? "Scrollsguide # play.scrollsguide.com");
		}
	}

	// Token: 0x04000D3A RID: 3386
	private const string PlayerPrefsAccessToken = "accessToken";

	// Token: 0x04000D3B RID: 3387
	private const string PlayerPrefsLoggedInUser = "loggedInUser";

	// Token: 0x04000D3C RID: 3388
	private const string PlayerPrefsDebugUser = "debugUsername";

	// Token: 0x04000D3D RID: 3389
	private const string PlayerPrefsDebugPW = "debugPassword";

	// Token: 0x04000D3E RID: 3390
	private const int STATE_FADETOBLACK = 3;

	// Token: 0x04000D3F RID: 3391
	private Communicator comm;

	// Token: 0x04000D40 RID: 3392
	private string username = string.Empty;

	// Token: 0x04000D41 RID: 3393
	private string password = string.Empty;

	// Token: 0x04000D42 RID: 3394
	private string errorMess = string.Empty;

	// Token: 0x04000D43 RID: 3395
	private GUISkin loginGUISkin;

	// Token: 0x04000D44 RID: 3396
	private string regEmail = string.Empty;

	// Token: 0x04000D45 RID: 3397
	private string regUser = string.Empty;

	// Token: 0x04000D46 RID: 3398
	private string regPwd = string.Empty;

	// Token: 0x04000D47 RID: 3399
	private string regPwdConfirm = string.Empty;

	// Token: 0x04000D48 RID: 3400
	private string regError = string.Empty;

	// Token: 0x04000D49 RID: 3401
	private bool regAcceptTerms;

	// Token: 0x04000D4A RID: 3402
	private bool regAcceptsNewsletters;

	// Token: 0x04000D4B RID: 3403
	private GUISkin _emptySkin;

	// Token: 0x04000D4C RID: 3404
	private GUISkin _regularUI;

	// Token: 0x04000D4D RID: 3405
	private GUISkin _closeButtonSkin;

	// Token: 0x04000D4E RID: 3406
	private Texture2D _imgBg;

	// Token: 0x04000D4F RID: 3407
	private Texture2D _imgRing;

	// Token: 0x04000D50 RID: 3408
	private Texture2D _imgLogo;

	// Token: 0x04000D51 RID: 3409
	private string _sceneToLoad;

	// Token: 0x04000D52 RID: 3410
	private float _fadeStartTime;

	// Token: 0x04000D53 RID: 3411
	private float _ringScale = 1f;

	// Token: 0x04000D54 RID: 3412
	private float _fadeTime;

	// Token: 0x04000D55 RID: 3413
	private bool _rememberMeChecked;

	// Token: 0x04000D56 RID: 3414
	private float _rotateAcc = 7.5f;

	// Token: 0x04000D57 RID: 3415
	private float _rotateTime;

	// Token: 0x04000D58 RID: 3416
	private bool showNoProfile;

	// Token: 0x04000D59 RID: 3417
	private int registerAccountReq = -1;

	// Token: 0x04000D5A RID: 3418
	private string registerAccountError = string.Empty;

	// Token: 0x04000D5B RID: 3419
	private AudioScript audioScript;

	// Token: 0x04000D5C RID: 3420
	private GUIStyle styleInfo;

	// Token: 0x04000D5D RID: 3421
	[SerializeField]
	private bool debugLoginStayOnScene;

	// Token: 0x04000D5E RID: 3422
	private int _state;

	// Token: 0x04000D5F RID: 3423
	private MockupCalc mockJunk = new MockupCalc(1920, 1080);

	// Token: 0x04000D60 RID: 3424
	private MockupCalc mockUnit = new MockupCalc(1, 1);

	// Token: 0x04000D61 RID: 3425
	private DidYouKnow didYouKnow;

	// Token: 0x04000D62 RID: 3426
	private GUIBlackOverlayButton overlay;

	// Token: 0x04000D63 RID: 3427
	private bool isLauncherLogin;

	// Token: 0x04000D64 RID: 3428
	private bool isRememberMeLogin;

	// Token: 0x04000D65 RID: 3429
	private bool isRegistration;

	// Token: 0x04000D66 RID: 3430
	private Login.State state;

	// Token: 0x04000D67 RID: 3431
	private Dropdown serverDrop;

	// Token: 0x04000D68 RID: 3432
	private bool showForgotPasswordButton;

	// Token: 0x04000D69 RID: 3433
	private LoginServer currentServer;

	// Token: 0x04000D6A RID: 3434
	private TextFiler serversListFile;

	// Token: 0x04000D6B RID: 3435
	private List<LoginServer> servers = new List<LoginServer>();

	// Token: 0x04000D6C RID: 3436
	private bool signInSent;

	// Token: 0x04000D6D RID: 3437
	private bool registerSent;

	// Token: 0x04000D6E RID: 3438
	private AuthRequest authRequest;

	// Token: 0x04000D6F RID: 3439
	private bool requestedDidYouKnow;

	// Token: 0x04000D70 RID: 3440
	private float angle;

	// Token: 0x04000D71 RID: 3441
	private static bool focusInited;

	// Token: 0x04000D72 RID: 3442
	private bool _initiated;

	// Token: 0x04000D73 RID: 3443
	private Dropdown yearDrop;

	// Token: 0x04000D74 RID: 3444
	private Dropdown monthDrop;

	// Token: 0x04000D75 RID: 3445
	private Dropdown dayDrop;

	// Token: 0x02000212 RID: 530
	private enum State
	{
		// Token: 0x04000D7A RID: 3450
		None,
		// Token: 0x04000D7B RID: 3451
		SelectServer,
		// Token: 0x04000D7C RID: 3452
		Login,
		// Token: 0x04000D7D RID: 3453
		Registration
	}
}
