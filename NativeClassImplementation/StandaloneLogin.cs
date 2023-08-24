using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using CommConfig;
using UnityEngine;

// Token: 0x020003EF RID: 1007
public class StandaloneLogin : AbstractCommListener, IOverlayClickCallback, IOkCallback, ICancelCallback, IOkStringCallback, IOkStringCancelCallback
{
	// Token: 0x06001600 RID: 5632 RVA: 0x00085430 File Offset: 0x00083630
	public StandaloneLogin()
	{
		List<LoginServer> list = new List<LoginServer>();
		list.Add(new LoginServer("Amazon", Host.Amazon.ip().ToString()));
		list.Add(new LoginServer("Localhost", Host.Localhost.ip().ToString()));
		this.servers = list;
		base..ctor();
	}

	// Token: 0x06001602 RID: 5634 RVA: 0x000100CF File Offset: 0x0000E2CF
	private void Awake()
	{
		if (this.ShouldUseLauncherLogin())
		{
			this.isLauncherLogin = true;
			this.signInPreparation();
		}
		this.loadSettings();
	}

	// Token: 0x06001603 RID: 5635 RVA: 0x000100EF File Offset: 0x0000E2EF
	public override void OnDestroy()
	{
		base.OnDestroy();
		PlayerPrefs.Save();
	}

	// Token: 0x06001604 RID: 5636 RVA: 0x00085520 File Offset: 0x00083720
	private static bool useNewLogin()
	{
		string value = App.Config.globalSettings.user.name.value;
		return value != null && value.Contains("@");
	}

	// Token: 0x06001605 RID: 5637 RVA: 0x0000D0A1 File Offset: 0x0000B2A1
	private bool ShouldUseLauncherLogin()
	{
		return App.StartedWithLauncher && MiniCommunicator.accessToken != null && !App.HasTriedFirstLogin;
	}

	// Token: 0x06001606 RID: 5638 RVA: 0x0008555C File Offset: 0x0008375C
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
		if (StandaloneLogin.useNewLogin())
		{
			return PlayerPrefs.HasKey("accessToken");
		}
		return PlayerPrefs.HasKey("debugUsername");
	}

	// Token: 0x06001607 RID: 5639 RVA: 0x000855DC File Offset: 0x000837DC
	private void Start()
	{
		Application.targetFrameRate = 60;
		this.audioScript = App.AudioScript;
		this.loginGUISkin = (GUISkin)ResourceManager.Load("_GUISkins/Login");
		this.loginGUISkin.textField.alignment = 3;
		this._emptySkin = (GUISkin)ResourceManager.Load("_GUISkins/EmptySkin");
		this._regularUI = (GUISkin)ResourceManager.Load("_GUISkins/RegularUI");
		this._closeButtonSkin = (GUISkin)ResourceManager.Load("_GUISkins/CloseButton");
		this.loadTextures();
		App.AudioScript.PlayMusic("Music/Theme");
		Input.eatKeyPressOnTextFieldFocus = false;
		this.comm = App.Communicator;
		this.comm.addListener(this);
		App.Config.ApplyResolution();
		base.StartCoroutine(this.Start2());
	}

	// Token: 0x06001608 RID: 5640 RVA: 0x000856AC File Offset: 0x000838AC
	private IEnumerator Start2()
	{
		yield return base.StartCoroutine(App.ApplicationController.CheckForUpdateCoroutine());
		while (!this.comm.ReadyToUse)
		{
			yield return null;
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
			if (StandaloneLogin.useNewLogin())
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
		this.state = StandaloneLogin.State.Registration;
		this.setRegistrationState(StandaloneLogin.State.Login);
		App.ShowSignoutReasonIfAny();
		yield break;
	}

	// Token: 0x06001609 RID: 5641 RVA: 0x000028DF File Offset: 0x00000ADF
	public void OverlayClicked()
	{
	}

	// Token: 0x0600160A RID: 5642 RVA: 0x000856C8 File Offset: 0x000838C8
	private void loadSettings()
	{
		this._rememberMeChecked = false;
		if (App.Config.globalSettings.user.isRemembered())
		{
			this._rememberMeChecked = true;
			this.username = App.Config.globalSettings.user.name;
		}
	}

	// Token: 0x0600160B RID: 5643 RVA: 0x000100FC File Offset: 0x0000E2FC
	private void loadTextures()
	{
		this._imgBg = ResourceManager.LoadTexture("Login/login__0008_BG");
		this._imgRing = ResourceManager.LoadTexture("Login/_0002_ring");
		this._imgLogo = ResourceManager.LoadTexture("Logos/scrollslogo");
	}

	// Token: 0x0600160C RID: 5644 RVA: 0x0008571C File Offset: 0x0008391C
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

	// Token: 0x0600160D RID: 5645 RVA: 0x00071C28 File Offset: 0x0006FE28
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

	// Token: 0x0600160E RID: 5646 RVA: 0x00085768 File Offset: 0x00083968
	private bool validateRegistrationInput()
	{
		string text = this.regEmail.Trim();
		string text2 = this.regUser.Trim();
		if (this.isMojangRegistration && (string.IsNullOrEmpty(text) || !StringUtil.IsValidEmail(text)))
		{
			this.regError = "You must enter a valid email address.";
			return false;
		}
		if (string.IsNullOrEmpty(text2))
		{
			this.regError = "You must choose a username.";
			return false;
		}
		if (text2.Length > 16)
		{
			this.regError = "Your username cannot be longer than 16 characters.";
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
		if (this.isMojangRegistration && this.regPwd.ToLowerInvariant().Contains(text))
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
		if (this.isMojangRegistration && this.getDateOfBirth() == null)
		{
			this.regError = "You need to enter a birth date";
			return false;
		}
		return true;
	}

	// Token: 0x0600160F RID: 5647 RVA: 0x00085B60 File Offset: 0x00083D60
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
		if (this.isMojangRegistration)
		{
			base.StartCoroutine("_registerMojangCoroutine");
		}
		else
		{
			base.StartCoroutine("_registerStandaloneCoroutine");
		}
	}

	// Token: 0x06001610 RID: 5648 RVA: 0x00085BCC File Offset: 0x00083DCC
	private IEnumerator beginSignin()
	{
		App.HasTriedFirstLogin = true;
		this.signInPreparation();
		MiniCommunicator.clearAllCredentials();
		App.Communicator.storeAuthHash(this.password);
		Coroutine routine = null;
		if (!this.username.Contains("@"))
		{
			routine = base.StartCoroutine(this._oldSignIn(false));
		}
		else
		{
			routine = base.StartCoroutine(this._newSignIn(false));
		}
		yield return routine;
		yield break;
	}

	// Token: 0x06001611 RID: 5649 RVA: 0x0001012E File Offset: 0x0000E32E
	private void signInPreparation()
	{
		this._rotateTime = Time.time;
		this.signInSent = true;
		this.errorMess = string.Empty;
	}

	// Token: 0x06001612 RID: 5650 RVA: 0x00085BE8 File Offset: 0x00083DE8
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

	// Token: 0x06001613 RID: 5651 RVA: 0x00085C14 File Offset: 0x00083E14
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
			req = new RefreshRequest(StandaloneLogin.getAccessToken());
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

	// Token: 0x06001614 RID: 5652 RVA: 0x00085C40 File Offset: 0x00083E40
	private IEnumerator MakeAuthRequest(string username, string password)
	{
		this.authRequest = new AuthRequest(username, password);
		yield return null;
		yield break;
	}

	// Token: 0x06001615 RID: 5653 RVA: 0x00085C78 File Offset: 0x00083E78
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

	// Token: 0x06001616 RID: 5654 RVA: 0x00085C94 File Offset: 0x00083E94
	private IEnumerator _registerStandaloneCoroutine()
	{
		this.registerAccountReq = -1;
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
			this.failRegistrationWith("Ouch!");
			yield break;
		}
		this.registerSent = false;
		this.username = this.regUser.Trim();
		this.password = string.Empty;
		App.Popups.ShowOk(this, "registrationok", "Account registered", I18n.Text("Thanks for signing up to play {GAME_NAME}! When you're done, press Ok to log in!"), "Ok");
		yield break;
	}

	// Token: 0x06001617 RID: 5655 RVA: 0x00085CB0 File Offset: 0x00083EB0
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

	// Token: 0x06001618 RID: 5656 RVA: 0x00085CCC File Offset: 0x00083ECC
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

	// Token: 0x06001619 RID: 5657 RVA: 0x00085D84 File Offset: 0x00083F84
	private void OnGUI()
	{
		GUI.depth = 13;
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
			Texture2D texture2D = ResourceManager.LoadTexture("Logos/scrollslogo");
			GUI.DrawTexture(this.mockJunk.rAspectW(667f, 180f, 594f, (float)(594 * texture2D.height / texture2D.width)), this._imgLogo);
		}
		int fontSize = GUI.skin.label.fontSize;
		TextAnchor alignment = GUI.skin.label.alignment;
		GUI.skin.label.fontSize = Screen.height / 40;
		GUI.skin.label.alignment = 5;
		if (App.Communicator.UseHost == Host.Amazon_Test)
		{
			GUI.Label(new Rect((float)Screen.width * 0.69f, (float)Screen.height * 0.91f, (float)Screen.width * 0.3f, (float)Screen.height * 0.03f), "[TEST SERVER]");
		}
		GUI.Label(new Rect((float)Screen.width * 0.69f, (float)Screen.height * 0.935f, (float)Screen.width * 0.3f, (float)Screen.height * 0.03f), "Version " + SharedConstants.getGameVersion());
		GUI.Label(new Rect((float)Screen.width * 0.69f, (float)Screen.height * 0.96f, (float)Screen.width * 0.3f, (float)Screen.height * 0.03f), "© 2012 - 2015 Mojang AB. All rights reserved.");
		GUI.skin.label.fontSize = fontSize;
		GUI.skin.label.alignment = alignment;
		int fontSize2 = this._regularUI.button.fontSize;
		int fontSize3 = this._regularUI.label.fontSize;
		Color textColor = this._regularUI.button.normal.textColor;
		GUIStyle button = this._regularUI.button;
		int fontSize4 = Screen.height / 50;
		this._regularUI.label.fontSize = fontSize4;
		button.fontSize = fontSize4;
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
		else if (!this.isRegistration)
		{
			GUIStyle guistyle2 = new GUIStyle(this.loginGUISkin.label);
			guistyle2.fontSize = Screen.height / 38;
			float num = 772f;
			GUI.Label(this.mockJunk.r(num, 420f, 422f, 60f), "Email", guistyle2);
			GUI.Label(this.mockJunk.r(num, 519f, 422f, 60f), "Password", guistyle2);
			GUI.Label(this.mockJunk.r(num, 614f, 422f, 60f), "Remember me", guistyle2);
			this.username = this.drawTextBox("username", this.mockJunk.r(762f, 457f, 412f, 47f), this.username, false);
			this.password = this.drawTextBox("password", this.mockJunk.r(762f, 554f, 412f, 47f), this.password, true);
			App.Config.globalSettings.user.name.value = ((!this._rememberMeChecked) ? string.Empty : this.username);
			if (!StandaloneLogin.focusInited && !this.isLauncherLogin && !this.isRememberMeLogin)
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
				StandaloneLogin.focusInited = true;
			}
			if ((GUI.GetNameOfFocusedControl() == "password" || GUI.GetNameOfFocusedControl() == "username") && (Input.GetKeyDown(13) || (Input.GetKeyDown(271) && !App.Popups.IsShowingPopup())) && this.comm.ReadyToUse)
			{
				this.login();
			}
			GUI.skin = this._emptySkin;
			float x = num + 1.1f * guistyle2.CalcSize(new GUIContent("Remember me")).x * 1920f / (float)Screen.width;
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
					this.setRegistrationState(StandaloneLogin.State.Registration);
				}
				if (this.showForgotPasswordButton && LobbyMenu.drawButton(LobbyMenu.getButtonPositioner((float)((!flag) ? 1 : 2), 10f, 150f).getButtonRect((float)((!flag) ? 0 : 1), this.mockJunk.Y(770f)), "Forgot password?", this._regularUI))
				{
					Application.OpenURL("https://account.mojang.com/password");
				}
			}
			GUI.enabled = true;
		}
		else
		{
			this.OnGUI_Register();
		}
		this._regularUI.button.fontSize = fontSize2;
		this._regularUI.label.fontSize = fontSize3;
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
			TextAnchor alignment2 = GUI.skin.label.alignment;
			int fontSize5 = GUI.skin.label.fontSize;
			GUI.skin.label.alignment = 1;
			GUI.skin.label.fontSize = Screen.height / 40;
			GUI.Label(new Rect(0f, 0.669f * (float)Screen.height, (float)Screen.width, (float)(Screen.height / 15)), "<color=#ff6655>" + this.errorMess + "</color>");
			GUI.skin.label.alignment = alignment2;
			GUI.skin.label.fontSize = fontSize5;
			GUI.skin = skin;
		}
		if (this.isRegistration && this.regError != string.Empty)
		{
			GUISkin skin2 = GUI.skin;
			GUI.skin = this.loginGUISkin;
			TextAnchor alignment3 = GUI.skin.label.alignment;
			int fontSize6 = GUI.skin.label.fontSize;
			GUI.skin.label.alignment = 1;
			GUI.skin.label.fontSize = Screen.height / 50;
			GUI.Label(new Rect(0f, 0.79f * (float)Screen.height, (float)Screen.width, (float)(Screen.height / 35)), "<color=#ff6655>" + this.regError + "</color>");
			GUI.skin.label.alignment = alignment3;
			GUI.skin.label.fontSize = fontSize6;
			GUI.skin = skin2;
		}
		Texture2D texture2D2 = ResourceManager.LoadTexture("Logos/mojang_logo_bw");
		float num4 = (float)Screen.height * 0.023f;
		float num5 = num4 * (float)texture2D2.width / (float)texture2D2.height;
		Rect rect2;
		rect2..ctor((float)Screen.height * 0.02f, (float)Screen.height * 0.98f - num4, num5, num4);
		GUI.color = Color.white;
		GUI.DrawTexture(rect2, texture2D2);
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

	// Token: 0x0600161A RID: 5658 RVA: 0x0001014D File Offset: 0x0000E34D
	private string drawTextBox(string name, Rect rect, string text, bool isPassword)
	{
		return StandaloneLogin.drawTextBox(this._emptySkin.button, name, rect, text, isPassword);
	}

	// Token: 0x0600161B RID: 5659 RVA: 0x00072D34 File Offset: 0x00070F34
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

	// Token: 0x0600161C RID: 5660 RVA: 0x00010164 File Offset: 0x0000E364
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

	// Token: 0x0600161D RID: 5661 RVA: 0x00086A90 File Offset: 0x00084C90
	private void failWith(string error)
	{
		if (error.StartsWith("Could not find a Scrolls profile"))
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
		StandaloneLogin.clearAccessToken();
	}

	// Token: 0x0600161E RID: 5662 RVA: 0x00086B04 File Offset: 0x00084D04
	private void failRegistrationWith(string error)
	{
		this.regError = error;
		this.registerSent = false;
		this.regPwd = (this.regPwdConfirm = string.Empty);
	}

	// Token: 0x0600161F RID: 5663 RVA: 0x00086B34 File Offset: 0x00084D34
	public override void handleMessage(Message msg)
	{
		if (msg is FailMessage)
		{
			FailMessage failMessage = (FailMessage)msg;
			if (failMessage.isType(typeof(RegisterAccountMessage)))
			{
				this.registerAccountReq = 0;
			}
			else
			{
				base.StopCoroutine("SignInTimeout");
				this.failWith(failMessage.info);
				if (failMessage.isType(typeof(FirstConnectMessage)))
				{
					StandaloneLogin.clearAccessToken();
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

	// Token: 0x06001620 RID: 5664 RVA: 0x0001019D File Offset: 0x0000E39D
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

	// Token: 0x06001621 RID: 5665 RVA: 0x0000D1A0 File Offset: 0x0000B3A0
	public static string getAccessToken()
	{
		return PlayerPrefs.GetString("accessToken", null);
	}

	// Token: 0x06001622 RID: 5666 RVA: 0x000101DA File Offset: 0x0000E3DA
	public static void clearAccessToken()
	{
		MiniCommunicator.accessToken = null;
		PlayerPrefs.DeleteKey("accessToken");
		PlayerPrefs.DeleteKey("loggedInUser");
		PlayerPrefs.DeleteKey("debugUsername");
		PlayerPrefs.DeleteKey("debugPassword");
	}

	// Token: 0x06001623 RID: 5667 RVA: 0x0001020A File Offset: 0x0000E40A
	public void PopupOk(string popupType)
	{
		if (popupType == "registrationok")
		{
			this.setRegistrationState(StandaloneLogin.State.Login);
		}
	}

	// Token: 0x06001624 RID: 5668 RVA: 0x00086C3C File Offset: 0x00084E3C
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
		if (this.isMojangRegistration)
		{
			GUI.Label(this.mockJunk.r(num2, 303f, 422f, 60f), "Email (needs verification)", guistyle2);
			GUI.Label(this.mockJunk.r(num2, 503f, 422f, 60f), "Display name", guistyle2);
			GUI.Label(this.mockJunk.r(num2, 608f, 422f, 60f), "Date of birth", guistyle2);
			this.regEmail = this.drawTextBox("regEmail", this.mockJunk.r(num, 340f, 412f, 47f), this.regEmail, false);
			this.regUser = this.drawTextBox("regUser", this.mockJunk.r(num, 540f, 412f, 47f), this.regUser, false);
		}
		else
		{
			GUI.Label(this.mockJunk.r(num2, 303f, 422f, 60f), "Display name", guistyle2);
			this.regUser = this.drawTextBox("regUser", this.mockJunk.r(num, 340f, 412f, 47f), this.regUser, false);
			num3 -= 215f;
		}
		GUI.Label(this.mockJunk.r(num2, 393f, 200f, 60f), "Password", guistyle2);
		GUI.Label(this.mockJunk.r(num2 + 212f, 393f, 200f, 60f), "Repeat", guistyle2);
		this.regPwd = this.drawTextBox("regPwd", this.mockJunk.r(num, 430f, 200f, 47f), this.regPwd, true);
		this.regPwdConfirm = this.drawTextBox("regPwdConfirm", this.mockJunk.r(num + 212f, 430f, 200f, 47f), this.regPwdConfirm, true);
		if ((GUI.GetNameOfFocusedControl() == "regEmail" || GUI.GetNameOfFocusedControl() == "regUser" || GUI.GetNameOfFocusedControl() == "regPwd" || GUI.GetNameOfFocusedControl() == "regPwdConfirm") && (Input.GetKeyDown(13) || (Input.GetKeyDown(271) && !App.Popups.IsShowingPopup())) && this.comm.ReadyToUse)
		{
			this.register();
		}
		GUI.skin = this._emptySkin;
		int fontSize = GUI.skin.button.fontSize;
		int fontSize2 = this._regularUI.label.fontSize;
		GUIStyle button = GUI.skin.button;
		int fontSize3 = Screen.height / 60;
		this._regularUI.label.fontSize = fontSize3;
		button.fontSize = fontSize3;
		int num4 = 80;
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
		int num5 = (!flag) ? 0 : 10;
		Rect checkboxRect = this.mockJunk.r(1130f, 42f + num3 + (float)num4, 44f, 40f);
		Rect checkboxRect2 = this.mockJunk.r(1130f, 97f + num3 + (float)num4, 44f, 40f);
		Rect labelRect = this.mockJunk.r(num2, 42f + num3 + (float)num4, 380f, 42f);
		if (flag)
		{
			labelRect.x -= (float)num5;
			labelRect.width += (float)(2 * num5);
			checkboxRect.x += (float)num5;
			checkboxRect2.x += (float)num5;
		}
		if (labelRect.xMax > checkboxRect.x)
		{
			labelRect.xMax = checkboxRect.x;
		}
		if (new Checkbox(I18n.Text("I accept the {GAME_NAME} EULA and the Terms and Conditions (including the Privacy Policy)"), labelRect, checkboxRect, this._regularUI, 0.7f).Draw(ref this.regAcceptTerms))
		{
		}
		if (this.isMojangRegistration)
		{
			labelRect.y = this.mockJunk.Y((float)(725 + num4));
			if (new Checkbox("I want Mojang to send me news and updates by email", labelRect, checkboxRect2, this._regularUI, 0.7f).Draw(ref this.regAcceptsNewsletters))
			{
			}
		}
		GUI.skin = (GUISkin)ResourceManager.Load("_GUISkins/EmptySkin");
		GUI.enabled = this.comm.ReadyToUse;
		if (LobbyMenu.drawButton(LobbyMenu.getButtonPositioner(2f, 10f, 150f).getButtonRect(0f, this.mockJunk.Y(172f + num3 + (float)num4)), "Register", this._regularUI))
		{
			this.register();
		}
		if (LobbyMenu.drawButton(LobbyMenu.getButtonPositioner(2f, 10f, 150f).getButtonRect(1f, this.mockJunk.Y(172f + num3 + (float)num4)), "Cancel", this._regularUI))
		{
			this.setRegistrationState(StandaloneLogin.State.Login);
		}
		GUI.enabled = true;
	}

	// Token: 0x06001625 RID: 5669 RVA: 0x000872DC File Offset: 0x000854DC
	private void setRegistrationState(StandaloneLogin.State newState)
	{
		if (this.state == newState)
		{
			return;
		}
		this.isRegistration = (newState == StandaloneLogin.State.Registration);
		this.state = newState;
		Object.Destroy(this.serverDrop);
		Object.Destroy(this.yearDrop);
		Object.Destroy(this.monthDrop);
		Object.Destroy(this.dayDrop);
		this.didYouKnow.SetActive(this.state == StandaloneLogin.State.Login);
		if (this.state == StandaloneLogin.State.Registration)
		{
			this.setupDropdowns();
			this.regError = string.Empty;
		}
		else if (this.state == StandaloneLogin.State.SelectServer)
		{
			this.setupServerDrop();
		}
		else if (this.state == StandaloneLogin.State.Login)
		{
		}
	}

	// Token: 0x06001626 RID: 5670 RVA: 0x0008738C File Offset: 0x0008558C
	private void setupServerDrop()
	{
		if (this.isMojangRegistration)
		{
			return;
		}
		Object.Destroy(this.serverDrop);
		List<string> list = Enumerable.ToList<string>(Enumerable.Select<LoginServer, string>(this.servers, (LoginServer x) => x.name));
		list.Add("-- Manage servers --");
		this.serverDrop = new GameObject("server").AddComponent<Dropdown>();
		this.serverDrop.SetRect(new Rect(0f, 0f, 200f, 40f));
		this.serverDrop.Init(list.ToArray(), 5f, true, true, -1);
		this.serverDrop.SetNothingSelectedIndex(list.Count - 1);
		this.serverDrop.SetEnabled(true);
		this.serverDrop.DropdownChangedEvent += this.HandleServerDropDropdownChangedEvent;
	}

	// Token: 0x06001627 RID: 5671 RVA: 0x00087474 File Offset: 0x00085674
	private void HandleServerDropDropdownChangedEvent(int selectedIndex, string selection)
	{
		if (selectedIndex == this.servers.Count)
		{
			string[] array = Enumerable.ToArray<string>(Enumerable.Select<LoginServer, string>(this.servers, (LoginServer x) => x.name + " # " + x.address.ToString()));
			App.Popups.ShowTextArea(this, "lookupservers", "Manage Servers", "One server per line: Name # address. Example for a local computer server: My Server # localhost", "Ok", "Cancel", string.Join("\n", array), true, true);
			return;
		}
		IpPort address = this.servers[selectedIndex].address;
		if (address == App.Communicator.getAddress())
		{
			return;
		}
		base.StartCoroutine(App.Communicator.connectToLookup(address));
	}

	// Token: 0x06001628 RID: 5672 RVA: 0x00087528 File Offset: 0x00085728
	private void setupDropdowns()
	{
		if (!this.isMojangRegistration)
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
		StandaloneLogin.initDateDropdown(this.yearDrop, list.ToArray());
		StandaloneLogin.initDateDropdown(this.monthDrop, StandaloneLogin.dateStrings("MM", 12));
		StandaloneLogin.initDateDropdown(this.dayDrop, StandaloneLogin.dateStrings("DD", 31));
		this.yearDrop.SetEnabled(true);
		this.monthDrop.SetEnabled(true);
		this.dayDrop.SetEnabled(true);
		this.yearDrop.DropdownChangedEvent += this.HandleYearOrMonthDropDropdownChangedEvent;
		this.monthDrop.DropdownChangedEvent += this.HandleYearOrMonthDropDropdownChangedEvent;
	}

	// Token: 0x06001629 RID: 5673 RVA: 0x00087700 File Offset: 0x00085900
	private void HandleYearOrMonthDropDropdownChangedEvent(int selectedIndex, string selection)
	{
		int num;
		int num2;
		if (!int.TryParse(this.yearDrop.GetSelectedItem(), ref num) || !int.TryParse(this.monthDrop.GetSelectedItem(), ref num2))
		{
			return;
		}
		int upto = DateTime.DaysInMonth(num, num2);
		StandaloneLogin.initDateDropdown(this.dayDrop, StandaloneLogin.dateStrings("DD", upto));
	}

	// Token: 0x0600162A RID: 5674 RVA: 0x0000D203 File Offset: 0x0000B403
	private static void initDateDropdown(Dropdown dateDrop, string[] values)
	{
		dateDrop.Init(values, 6f, true, true, 12);
	}

	// Token: 0x0600162B RID: 5675 RVA: 0x0007434C File Offset: 0x0007254C
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

	// Token: 0x0600162C RID: 5676 RVA: 0x0008775C File Offset: 0x0008595C
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

	// Token: 0x0600162D RID: 5677 RVA: 0x000028DF File Offset: 0x00000ADF
	public void PopupCancel(string popupType)
	{
	}

	// Token: 0x0600162E RID: 5678 RVA: 0x000877C8 File Offset: 0x000859C8
	public void PopupOk(string popupType, string choice)
	{
		this.servers.Clear();
		string[] array = choice.Split(new char[]
		{
			'\n'
		});
		foreach (string text in array)
		{
			int num = text.IndexOf("#");
			if (num >= 0)
			{
				this.servers.Add(new LoginServer(text.Substring(0, num).Trim(), text.Substring(num + 1).Trim()));
			}
		}
		this.setupServerDrop();
		Log.warning("----> " + popupType + " -->" + choice);
	}

	// Token: 0x0400132F RID: 4911
	private const string PlayerPrefsAccessToken = "accessToken";

	// Token: 0x04001330 RID: 4912
	private const string PlayerPrefsLoggedInUser = "loggedInUser";

	// Token: 0x04001331 RID: 4913
	private const string PlayerPrefsDebugUser = "debugUsername";

	// Token: 0x04001332 RID: 4914
	private const string PlayerPrefsDebugPW = "debugPassword";

	// Token: 0x04001333 RID: 4915
	private const int STATE_FADETOBLACK = 3;

	// Token: 0x04001334 RID: 4916
	private Communicator comm;

	// Token: 0x04001335 RID: 4917
	private string username = string.Empty;

	// Token: 0x04001336 RID: 4918
	private string password = string.Empty;

	// Token: 0x04001337 RID: 4919
	private string errorMess = string.Empty;

	// Token: 0x04001338 RID: 4920
	private GUISkin loginGUISkin;

	// Token: 0x04001339 RID: 4921
	private string regEmail = string.Empty;

	// Token: 0x0400133A RID: 4922
	private string regUser = string.Empty;

	// Token: 0x0400133B RID: 4923
	private string regPwd = string.Empty;

	// Token: 0x0400133C RID: 4924
	private string regPwdConfirm = string.Empty;

	// Token: 0x0400133D RID: 4925
	private string regError = string.Empty;

	// Token: 0x0400133E RID: 4926
	private bool regAcceptTerms;

	// Token: 0x0400133F RID: 4927
	private bool regAcceptsNewsletters;

	// Token: 0x04001340 RID: 4928
	private GUISkin _emptySkin;

	// Token: 0x04001341 RID: 4929
	private GUISkin _regularUI;

	// Token: 0x04001342 RID: 4930
	private GUISkin _closeButtonSkin;

	// Token: 0x04001343 RID: 4931
	private Texture2D _imgBg;

	// Token: 0x04001344 RID: 4932
	private Texture2D _imgRing;

	// Token: 0x04001345 RID: 4933
	private Texture2D _imgLogo;

	// Token: 0x04001346 RID: 4934
	private string _sceneToLoad;

	// Token: 0x04001347 RID: 4935
	private float _fadeStartTime;

	// Token: 0x04001348 RID: 4936
	private float _ringScale = 1f;

	// Token: 0x04001349 RID: 4937
	private float _fadeTime;

	// Token: 0x0400134A RID: 4938
	private bool _rememberMeChecked;

	// Token: 0x0400134B RID: 4939
	private float _rotateAcc = 7.5f;

	// Token: 0x0400134C RID: 4940
	private float _rotateTime;

	// Token: 0x0400134D RID: 4941
	private bool showNoProfile;

	// Token: 0x0400134E RID: 4942
	private bool isMojangRegistration;

	// Token: 0x0400134F RID: 4943
	private int registerAccountReq = -1;

	// Token: 0x04001350 RID: 4944
	private AudioScript audioScript;

	// Token: 0x04001351 RID: 4945
	[SerializeField]
	private bool debugLoginStayOnScene;

	// Token: 0x04001352 RID: 4946
	private int _state;

	// Token: 0x04001353 RID: 4947
	private MockupCalc mockJunk = new MockupCalc(1920, 1080);

	// Token: 0x04001354 RID: 4948
	private MockupCalc mockUnit = new MockupCalc(1, 1);

	// Token: 0x04001355 RID: 4949
	private DidYouKnow didYouKnow;

	// Token: 0x04001356 RID: 4950
	private GUIBlackOverlayButton overlay;

	// Token: 0x04001357 RID: 4951
	private bool isLauncherLogin;

	// Token: 0x04001358 RID: 4952
	private bool isRememberMeLogin;

	// Token: 0x04001359 RID: 4953
	private bool isRegistration;

	// Token: 0x0400135A RID: 4954
	private StandaloneLogin.State state;

	// Token: 0x0400135B RID: 4955
	private Dropdown serverDrop;

	// Token: 0x0400135C RID: 4956
	private bool showForgotPasswordButton;

	// Token: 0x0400135D RID: 4957
	private List<LoginServer> servers;

	// Token: 0x0400135E RID: 4958
	private bool signInSent;

	// Token: 0x0400135F RID: 4959
	private bool registerSent;

	// Token: 0x04001360 RID: 4960
	private AuthRequest authRequest;

	// Token: 0x04001361 RID: 4961
	private bool requestedDidYouKnow;

	// Token: 0x04001362 RID: 4962
	private float angle;

	// Token: 0x04001363 RID: 4963
	private static bool focusInited;

	// Token: 0x04001364 RID: 4964
	private bool _initiated;

	// Token: 0x04001365 RID: 4965
	private Dropdown yearDrop;

	// Token: 0x04001366 RID: 4966
	private Dropdown monthDrop;

	// Token: 0x04001367 RID: 4967
	private Dropdown dayDrop;

	// Token: 0x020003F0 RID: 1008
	private enum State
	{
		// Token: 0x0400136B RID: 4971
		SelectServer,
		// Token: 0x0400136C RID: 4972
		Login,
		// Token: 0x0400136D RID: 4973
		Registration
	}
}
