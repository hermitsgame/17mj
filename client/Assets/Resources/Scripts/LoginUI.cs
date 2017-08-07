﻿using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Collections;
using Facebook.MiniJSON;

public class LoginUI : MonoBehaviour {
    public static LoginUI Instance;

	public InputField ClubLoginAccount;       // 登入帳號
	public InputField ClubLoginPass;          // 登入密碼
	public GameObject ClubLoginHint_Account;  // 帳號錯誤提示
    public GameObject ClubLoginHint_Password; // 密碼錯誤提示
    public GameObject ConnectingPanel; // 連線中

    private Text _loginHintPass;
    private bool _hideConnecting = false;    //隱藏連線中
    private bool _showLoginHintPass = false; //顯示錯誤訊息
    private bool _loginSuccess = false;    //設定資料
    private IDictionary dict;

    void Awake() {
        Instance = this;
    }

    void Start() {
        if(!ClubLoginAccount)
            Debug.Log("No found Login Account InputField");
        else
            ClubLoginAccount.text = "";

        if (!ClubLoginPass)
            Debug.Log("No found Login Password InputField");
        else
            ClubLoginPass.text = "";

        if (!ClubLoginHint_Account)
            Debug.Log("No found Login LoginHint_Account");
        if (!ClubLoginHint_Password)
            Debug.Log("No found Login LoginHint_Password");
        else
            _loginHintPass = ClubLoginHint_Password.GetComponentInChildren<Text>();
        if (!ConnectingPanel)
            Debug.Log("No found Login Connecting Panel");
        
    }

	//登入按鈕
	public void ClubSigninClick() {
		string userName = ClubLoginAccount.text;
		string userPass = ClubLoginPass.text;

        if (!CheckInput.instance.CheckPass(userPass)) //檢查密碼
        {
            _loginHintPass.text = "密碼格式錯誤";
            ClubLoginHint_Password.SetActive(true);
        }
        if (!CheckInput.instance.CheckEmail(userName)) //檢查帳號
        {  
            ClubLoginHint_Account.GetComponentInChildren<Text>().text = "帳號格式錯誤";
            ClubLoginHint_Account.SetActive (true);
		} else {
            string stype = "C";

            if (ConnectingPanel) {
                ConnectingPanel.SetActive(true);
                UIManager.instance.PlayConnectingAnim();
            }
                
            MJApi.Login(stype, userName, userPass, LoginCallback);
        }
	}

    public void LoginCallback(WebExceptionStatus status, string result)
    {
        //if (ConnectingPanel) {
        //    ConnectingPanel.SetActive(false);
        //    UIManager.instance.StopConnectingAnim();
        //}
            
        _hideConnecting = true;

        if (status!=WebExceptionStatus.Success){
            //_loginHintPass.text = "登入失敗: 輸入資訊錯誤";
            //ClubLoginHint_Password.SetActive (true);
            _showLoginHintPass = true;
            //Debug.Log("登入失敗: 輸入資訊錯誤");		
        } else {
            dict = Json.Deserialize(result) as IDictionary;
            _loginSuccess = true;
        }
    }

    //點擊錯誤提示區塊 清空欄位並聚焦
    public void ClickHintBlock(Button targetHint) {
        targetHint.gameObject.SetActive(false);
        targetHint.GetComponentInParent<InputField>().ActivateInputField();
    }

    public void Logout()
    {
        CryptoPrefs.DeleteKey("USERPHOTO");
        CryptoPrefs.DeleteKey("USERNAME");
        CryptoPrefs.DeleteKey("USERLEVEL");
        CryptoPrefs.DeleteKey("USERCOIN");
        CryptoPrefs.DeleteKey("USERONLINE");
        CryptoPrefs.DeleteKey("USERTOKEN");
    }

    void Update() {
        if (_hideConnecting) {
            if (ConnectingPanel)
            {
                ConnectingPanel.SetActive(false);
                UIManager.instance.StopConnectingAnim();
            }

            _hideConnecting = false;
        }

        if (_showLoginHintPass) {
            _loginHintPass.text = "登入失敗：輸入資訊錯誤";
            if (ClubLoginHint_Password)
                ClubLoginHint_Password.SetActive(true);
            _showLoginHintPass = false;
        }

        if (_loginSuccess)
        {
            string uName = string.Empty;
            string uToken = string.Empty;
            string uLevel = string.Empty;
            string uCoin = string.Empty;

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
            UIManager.instance.StartSetEnterLoading(); //載入下個場景
            _loginSuccess = false;
        }
    }
}
