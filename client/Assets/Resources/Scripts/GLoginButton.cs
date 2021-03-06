﻿using UnityEngine;
using Facebook.MiniJSON;
using System;
using System.Collections;
using System.Net;
using UnityEngine.UI;
using Facebook.Unity;
using System.Runtime.InteropServices;


public class GLoginButton : MonoBehaviour {
    public Button[] _gLoginBtn;
    //public GameObject ConnectingPanel; // 連線中

    private static AndroidJavaObject login = null;
    private static AndroidJavaObject currentActivity = null;
    private bool _loginSuccess = false;  //設定資料
    private bool _setPhoto = false;      //設定頭像
    private string stringData = string.Empty;
    private bool _loginDone = false;
    private bool _setPhotoDone = false;
    private IDictionary dict;

    #if UNITY_IOS
    [DllImport("__Internal")]
    private static extern void _loginCheck ();
    [DllImport("__Internal")]
    private static extern void _login ();
    [DllImport("__Internal")]
    private static extern void _loginOut ();
    #endif

    void Start () {
        if (_gLoginBtn.Length != 0)
        {
            for (int i = 0; i < _gLoginBtn.Length; i++)
            {
				if (_gLoginBtn [i] != null) {
					_gLoginBtn [i].onClick.AddListener (delegate {
						GLogin ();
					});
				}
            }
        }

        //Button btn = GetComponent<Button>();
        //btn.onClick.AddListener(delegate
        //{
        //    GLogin();
        //});
#if UNITY_ANDROID && !UNITY_EDITOR
        var javaUnityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = javaUnityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        var loginClass = new AndroidJavaClass("com.foxgame.google.GoogleSignInDialog");
        login = loginClass.CallStatic<AndroidJavaObject>("getInstance");
        login.CallStatic("checkInit", this.gameObject.name, "OnConnected", currentActivity);
#elif UNITY_IOS && !UNITY_EDITOR
	    _loginCheck();
#endif
    }

	private void AutoLoginCheck()
	{
		string cName = CryptoPrefs.GetString("USERNAME");
		string cToken = CryptoPrefs.GetString("USERTOKEN");
		if (!string.IsNullOrEmpty(cName) && !string.IsNullOrEmpty(cToken))
		{
			string type = "C1";
			_setPhotoDone = true;
			MJApi.Login(type, cName, cToken, LoginCallback);
			AccountManager.Instance.ShowConnecting ();
		}
	}

    private void GLogin()
    {
        Debug.Log("GLogin()");
#if UNITY_ANDROID && !UNITY_EDITOR
        login.CallStatic("Login", this.gameObject.name, "OnConnected", currentActivity);
#elif UNITY_IOS && !UNITY_EDITOR
	    _login ();
#endif
    }   

    public void GLoginOut()
    {
        Debug.Log("GLoginOut()");
#if UNITY_ANDROID && !UNITY_EDITOR
        if(login!=null)
            login.CallStatic("LoginOut");
#elif UNITY_IOS && !UNITY_EDITOR
        _loginOut ();
#endif
    }

    private void LoginCallback(WebExceptionStatus status, string result)
    {
		AccountManager.Instance.HideConnecting ();   

        if (status != WebExceptionStatus.Success)
        {
			Debug.Log("Google LoginCallback Failed! " + result);
        }
        else
        {
            dict = Json.Deserialize(result) as IDictionary;
            _loginSuccess = true;
        }
    }

	private IEnumerator getPhoto(string url)
    {
		if (url != null)
		{
			Texture2D tex = new Texture2D(1, 1, TextureFormat.DXT1, false);
			WWW www = new WWW(url);
			yield return www;
			www.LoadImageIntoTexture(tex);
			stringData = Convert.ToBase64String(tex.EncodeToJPG());
		}
		_setPhoto = true;
    }

    public void OnConnected(string result)
    {
        //Debug.Log("OnConnected() = " + result);

        string uName = string.Empty;
        string uGid = string.Empty;
		string uMail = string.Empty;
		string uPhoto = string.Empty;
        string[] tokens = result.Split(new string[] { "," }, StringSplitOptions.None);

        if (tokens[0] != null)
            uMail = tokens[0];

        if (uMail == "No Init")
        {
			AutoLoginCheck ();
        }
        else
        {
			CryptoPrefs.SetString("USERTYPE", "G");
			CryptoPrefs.SetString("USERMAIL", uMail);

            if (tokens[1] != null)
                uGid = tokens[1];
            if (tokens[2] != null)
                uName = tokens[2];
			if (tokens[3] != null)
				uPhoto = tokens[3];

            string Photo = CryptoPrefs.GetString("USERPHOTO");
			if (string.IsNullOrEmpty(Photo))
				StartCoroutine(getPhoto(uPhoto));
			else
                _setPhotoDone = true;

            string stype = "G";
            string token = CryptoPrefs.GetString("USERTOKEN");
            if (string.IsNullOrEmpty(token))
            {
                //Debug.Log("Call MJApi.AddMember ===");
                MJApi.AddMember(uGid, uMail, "1", uName, stype, LoginCallback);
            }
            else
            {
                //Debug.Log("Call MJApi.login ===");
                MJApi.Login(stype, uMail, token, LoginCallback);
            }
			AccountManager.Instance.ShowConnecting ();
            //UIManager.instance.StartSetEnterLoading();
        }
    }

    public void setPhotoCallback(WebExceptionStatus status, string result)
    {
        if (status != WebExceptionStatus.Success)
        {
			Debug.Log("Google setPhotoCallback Failed! " + result);
        }
        //Debug.Log("Glogin setPhotoCallback =  " + result);
    }

    void Update() {
        if (_loginSuccess) {

            string uName = string.Empty;
            string uToken = string.Empty;
            string uLevel = string.Empty;
            string uCoin = string.Empty;
			string ufLogin = string.Empty;
			string ulTotal = string.Empty;
			string uWin = string.Empty;
			string uLose = string.Empty;

            if (dict["Name"] != null)
            {
                uName = dict["Name"].ToString();
                CryptoPrefs.SetString("USERNAME", uName);
            }
            if (dict["Token"] != null)
            {
                uToken = dict["Token"].ToString();
                CryptoPrefs.SetString("USERTOKEN", uToken);
            }
            if (dict["Level"] != null)
            {
                uLevel = dict["Level"].ToString();
                CryptoPrefs.SetString("USERLEVEL", uLevel);
            }
            if (dict["Coin"] != null)
            {
                uCoin = dict["Coin"].ToString();
                CryptoPrefs.SetString("USERCOIN", uCoin);
            }
			if (dict["fLogin"] != null)
			{
				ufLogin = dict["fLogin"].ToString();
				CryptoPrefs.SetString("USERFLOGIN", ufLogin);
			}
			if (dict["LoginTotal"] != null)
			{
				ulTotal = dict["LoginTotal"].ToString();
				CryptoPrefs.SetString("USERLOGINTOTAL", ulTotal);
			}
			if (dict["Win"] != null)
			{
				uWin = dict["Win"].ToString();
				CryptoPrefs.SetString("USERWIN", uWin);
			}
			if (dict["Lose"] != null)
			{
				uLose = dict["Lose"].ToString();
				CryptoPrefs.SetString("USERLOSE", uLose);
			}
            _loginSuccess = false;
            _loginDone = true;
        }
        if (_setPhoto)
        {
            string sName = string.Empty;
            string sToken = string.Empty;
            string sPhoto = string.Empty;
            sName = CryptoPrefs.GetString("USERNAME");
            sToken = CryptoPrefs.GetString("USERTOKEN");
            if (stringData != string.Empty &&
                 sName != string.Empty &&
                 sToken != string.Empty)
            {
                CryptoPrefs.SetString("USERPHOTO", stringData);
                sPhoto = CryptoPrefs.GetString("USERPHOTO");
                MJApi.setUserPhoto(sToken, sName, sPhoto, setPhotoCallback);
                _setPhoto = false;
                _setPhotoDone = true;
            }
        }

        if (_loginDone && _setPhotoDone)
        {
            UIManager.instance.StartSetEnterLoading();
            _loginDone = false;
            _setPhotoDone = false;
        }
    }
}
