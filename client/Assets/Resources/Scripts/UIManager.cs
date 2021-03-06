﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Net;
using Facebook.MiniJSON;
using DG.Tweening;

public class UIManager : MonoBehaviour {

    public static UIManager instance;
    public Transform MainPanel;
    public GameObject[] LoginBtns;

    private GameObject loginPanel;
    private GameObject registerPanel;
    private GameObject forgotPanel;
    private GameObject rulePanel;
    private Transform enterLoadingPanel;
    private Animator enterLoadingAnim;
    private Animator mainPanelAnim;
    private IDictionary dict;
    private bool _registerSuccess = false;
    private Transform _connectingSign;
    private Text _connectingText;
    private Text _loadingText;
    private Transform _slogan;

    void Start() {
        instance = this;

        if (!MainPanel)
            Debug.LogError("No found MainPanel");
        else {
            mainPanelAnim = MainPanel.GetComponent<Animator>();

            //抓取各個 panel
            loginPanel = MainPanel.Find("LoginPanel").gameObject;
            registerPanel = MainPanel.Find("RegisterPanel").gameObject;
            forgotPanel = MainPanel.Find("ForgotPanel").gameObject;
            rulePanel = MainPanel.Find("RulePanel").gameObject;
            enterLoadingPanel = MainPanel.Find("LoadingPanel");

            _loadingText = enterLoadingPanel.Find("LoadingBar/Text").GetComponent<Text>();
            _slogan = loginPanel.transform.Find("slogan");

            if (!loginPanel)
                Debug.Log("No found LoginPanel");
            if (!registerPanel)
                Debug.Log("No found RegisterPanel");
            if (!forgotPanel)
                Debug.Log("No found ForgotPanel");
            if (!rulePanel)
                Debug.Log("No found RulePanel");
            if (!enterLoadingPanel)
                Debug.Log("No found LoadingPanel");
            else
                enterLoadingAnim = enterLoadingPanel.GetComponent<Animator>();

            //if (ConnectingPanel) {
            //    _connectingSign = ConnectingPanel.transform.Find("Image/Img").transform;
            //    _connectingText = ConnectingPanel.transform.Find("Image/Text").GetComponent<Text>();
            //    if (_connectingSign && _connectingText)
            //        ConnectingAnim();
            //}

            if (_loadingText)
                _loadingText.DOText("載入中...", 3).SetLoops(-1, LoopType.Restart);
        }

        if (LoginBtns.Length == 0)
            Debug.LogError("No found GameLobbyButton");

        InitialLoginPanel();
        InitialAnim();
        StartCoroutine("PlayOP");
        

    }

    // 遊戲流程
    IEnumerator PlayOP() {
        yield return new WaitForSeconds(1f);
        mainPanelAnim.SetTrigger("loginFlag"); //畫面淡入
    }

    // 01-入口按鈕樣式初始化
    private void InitialLoginPanel()
    {
        for (int i = 0; i < 4; i++)
            LoginBtns[i].SetActive(true);
        for (int i = 4; i < 8; i++)
            LoginBtns[i].SetActive(false);
    }

    // 01-點擊"17玩麻將"按鈕，其他入口按鈕樣式改變
    public void Click17Play() {
        foreach (GameObject go in LoginBtns) go.SetActive((!go.activeSelf));
    }

    // 02-點擊"註冊"按鈕
    public void GoRegisterPage() {
        registerPanel.SetActive(true);
    }

    // 02-離開註冊醬玩會員
    public void ExitRegisterPage()
    {
        registerPanel.SetActive(false);
    }

    // 03-進入忘記密碼頁
    public void GoForgotPage() {
        forgotPanel.SetActive(true);
    }

    // 03-離開忘記密碼頁
    public void ExitForgotPage()
    {
        forgotPanel.SetActive(false);
        ForgotUI.instance.ResetAllInput();
    }

    // 04-進入服務條款頁
    public void GoRulePage()
    {
        rulePanel.SetActive(true);
    }

    // 04-離開服務條款頁
    public void ExitRulePage()
    {
        rulePanel.SetActive(false);
    }

    // 05-準備進入讀取畫面
    public void StartSetEnterLoading()
    {
        loginPanel.SetActive(false);
        EnterLoading.instance.StartLoading();
    }

    public void PlayNowButton()
    {
        loginPanel.SetActive(false);
        EnterLoading.instance._autoToNextScene = false;
        EnterLoading.instance.StartLoading();

        string[] ran_names = {
                "雲盤金城武",
                "高雄彭玉燕",
                "鼓山張學友",
                "唐山綾波零",
                "成大金城武",
                "韓國林志穎",
                "釜山林志玲",
                "左營林志玲",
                "太極張三豐",
                "三民陳金城",
                "台南李炳輝",
                "彰化波多野",
                "屏東張韶涵",
                "台北郭金發",
                "基隆日本橋"
            };
        int idx = UnityEngine.Random.Range(0, ran_names.Length - 1);
        string uName = ran_names[idx];

        string id = RegisterUI.GetUniqueKey(24);
        string mail = RegisterUI.GetUniqueKey(8)+ "@17";
        string pass = RegisterUI.GetUniqueKey(8);
        string stype = "C";

		CryptoPrefs.SetString("USERTYPE", "P");
		CryptoPrefs.SetString("USERMAIL", mail);
        MJApi.AddMember(id, mail, pass, uName, stype, RegisterCallback);
    }

    //註冊 Callback
    private void RegisterCallback(WebExceptionStatus status, string result)
    {
        if (status != WebExceptionStatus.Success)
        {
			Debug.Log("Play Now RegisterCallback Failed! " + result);
        }
        else
        {
            //Debug.Log("ConnectSuccess! " + result);
            dict = Json.Deserialize(result) as IDictionary;
            _registerSuccess = true;
        }
     }

    void Update() {

        if (_registerSuccess) {
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

            EnterLoading.instance._autoToNextScene = true;

            _registerSuccess = false;
        }
    }

    private void InitialAnim() {
        if (_slogan)
        {
            _slogan.DOScale(new Vector3(1.05f, 1.05f, 1), 0.8f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
            //_slogan.DOBlendableMoveBy(new Vector3(0, -0.1f, 0f), 1.6f).SetEase(Ease.Linear).SetLoops(-1, LoopType.Yoyo);
        }
    }
}
